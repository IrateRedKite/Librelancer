// MIT License - Copyright (c) Callum McGing
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

using System;
using System.Buffers;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibreLancer.Text.Pango
{
    class PangoBuiltText : BuiltRichText
    {
        internal IntPtr Handle;

        int height = 0;
        public override float Height => height;

        [DllImport("pangogame")]
        static extern void pg_destroytext(IntPtr text);
        [DllImport("pangogame")]
        static extern int pg_getheight(IntPtr text);
        [DllImport("pangogame")]
        static extern void pg_updatewidth(IntPtr text, int width); 
        internal PangoBuiltText(IntPtr ctx, IntPtr handle)
        {
            Handle = handle;
            height = pg_getheight(handle);
        }

        public override void Dispose()
        {
            pg_destroytext(Handle);
        }

        int width = -1;
        public override void Recalculate(float width)
        {
            if ((int)width == this.width)
                return;
            this.width = (int)width;
            pg_updatewidth(Handle, (int)width);
            height = pg_getheight(Handle);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    unsafe struct PGQuad
    {
        public PGTexture* Texture;
        public Rectangle Source;
        public Rectangle Dest;
        public Color4 Color;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct PGTexture
    {
        public IntPtr UserData;
    }

    unsafe class PangoText : RichTextEngine
    {
        [DllImport("pangogame")]
        static extern IntPtr pg_createcontext(IntPtr allocate, IntPtr update, IntPtr draw);
        [DllImport("pangogame")]
        public static extern IntPtr pg_buildtext(IntPtr ctx, IntPtr markups, IntPtr alignments, int count, int width);
        [DllImport("pangogame")]
        static extern IntPtr pg_drawtext(IntPtr ctx, IntPtr text);

        [DllImport("pangogame")]
        static extern void pg_drawstring(IntPtr ctx, IntPtr str, IntPtr fontName, float fontSize, TextAlignment align,
            int underline, float r, float g, float b, float a, Color4* shadow, float *oWidth, float *oHeight);

        [DllImport("pangogame")]
        static extern void pg_measurestring(IntPtr ctx, IntPtr str, IntPtr fontName, float fontSize, out float width,
            out float height);

        [DllImport("pangogame")]
        static extern float pg_lineheight(IntPtr ctx, IntPtr fontName, float fontSize);
        
        delegate void PGDrawCallback(PGQuad* quads, int count);
        delegate void PGAllocateTextureCallback(PGTexture* texture, int width, int height);
        delegate void PGUpdateTextureCallback(PGTexture* texture, IntPtr buffer, int x, int y, int width, int height);

        PGDrawCallback draw;
        PGAllocateTextureCallback alloc;
        PGUpdateTextureCallback update;
        private List<Texture2D> textures = new List<Texture2D>();
        Renderer2D ren;
        IntPtr ctx;
        public PangoText(Renderer2D renderer)
        {
            ren = renderer;
            draw = Draw;
            alloc = Alloc;
            update = Update;
            ctx = pg_createcontext(
                Marshal.GetFunctionPointerForDelegate(alloc),
                Marshal.GetFunctionPointerForDelegate(update),
                Marshal.GetFunctionPointerForDelegate(draw)
            );
        }

        void Draw(PGQuad* quads, int count)
        {
            if (drawX == int.MaxValue)
            {
                lastQuads = new PGQuad[count];
                for (int i = 0; i < lastQuads.Length; i++)
                    lastQuads[i] = quads[i];
                return;
            }
            for (int i = 0; i < count; i++)
            {
                var q = quads[i];
                q.Dest.X += drawX;
                q.Dest.Y += drawY;
                if (q.Texture == (PGTexture*) 0)
                {
                    ren.FillRectangle(q.Dest, q.Color);
                }
                else
                {
                    var t = textures[(int)q.Texture->UserData];
                    ren.Draw(t, q.Source, q.Dest, q.Color);
                }
            }
        }

        void Alloc(PGTexture *texture, int width, int height)
        {
            textures.Add(new Texture2D(width, height, false, SurfaceFormat.R8));
            texture->UserData = (IntPtr)(textures.Count - 1);
        }

        void Update(PGTexture *texture, IntPtr buffer, int x, int y, int width, int height)
        {
            var t = textures[(int)texture->UserData];
            GL.PixelStorei(GL.GL_UNPACK_ALIGNMENT, 1);
            var rect = new Rectangle(x, y, width, height);
            t.SetData(0, rect, buffer);
            GL.PixelStorei(GL.GL_UNPACK_ALIGNMENT, 4);
        }

        
        public override unsafe BuiltRichText BuildText(IList<RichTextNode> nodes, int width, float sizeMultiplier = 1f)
        {
            //Sort into paragraphs
            var paragraphs = new List<List<RichTextNode>>();
            paragraphs.Add(new List<RichTextNode>());
            int first = 0;
            while (nodes[first] is RichTextParagraphNode && first < nodes.Count) first++;
            var ta = (nodes[first] as RichTextTextNode).Alignment;
            paragraphs[paragraphs.Count - 1].Add(nodes[first]);
            foreach (var node in nodes.Skip(first + 1))
            {
                if (node is RichTextParagraphNode)
                    paragraphs.Add(new List<RichTextNode>());
                else
                {
                    var n = (RichTextTextNode)node;
                    var align = n.Alignment;
                    if (align != ta && paragraphs[paragraphs.Count - 1].Count > 0)
                        paragraphs.Add(new List<RichTextNode>());
                    paragraphs[paragraphs.Count - 1].Add(node);
                    ta = align;
                }
            }
            //Build markup
            var markups = new List<string>();
            var alignments = new List<TextAlignment>();
            for(int i = 0; i < paragraphs.Count; i++)
            {
                //make span
                var builder = new StringBuilder();
                TextAlignment a = TextAlignment.Left;
                foreach (var tn in paragraphs[i])
                {
                    var text = (RichTextTextNode)tn;
                    a = text.Alignment;
                    builder.Append("<span ");
                    if (text.Italic)
                        builder.Append("font_style=\"italic\" ");
                    else
                        builder.Append("font_style=\"normal\" ");
                    if (text.Bold)
                        builder.Append("font_weight=\"bold\" ");
                    else
                        builder.Append("font_weight=\"normal\" ");
                    if (text.Shadow.Enabled)
                    {
                        builder.Append("bgcolor=\"#");
                        builder.Append(((int)(text.Shadow.Color.R * 255f)).ToString("X2"));
                        builder.Append(((int)(text.Shadow.Color.G * 255f)).ToString("X2"));
                        builder.Append(((int)(text.Shadow.Color.B * 255f)).ToString("X2"));
                        builder.Append("\" ");
                    }
                    builder.Append("fgcolor=\"#");
                    builder.Append(((int)(text.Color.R * 255f)).ToString("X2"));
                    builder.Append(((int)(text.Color.G * 255f)).ToString("X2"));
                    builder.Append(((int)(text.Color.B * 255f)).ToString("X2"));
                    builder.Append("\" underline=\"");
                    if (text.Underline) builder.Append("single\" ");
                    else builder.Append("none\" ");
                    builder.Append("size=\"");
                    builder.Append((int)(text.FontSize * sizeMultiplier * 1024));
                    builder.Append("\" font_family=\"");
                    builder.Append(text.FontName);
                    builder.Append("\">");
                    builder.Append(System.Net.WebUtility.HtmlEncode(text.Contents));
                    builder.Append("</span>");
                }
                markups.Add(builder.ToString());
                alignments.Add(a);
            }
            //Pass
            IntPtr[] stringPointers = new IntPtr[markups.Count];
            for(int i = 0; i < markups.Count; i++)
            {
                stringPointers[i] = UnsafeHelpers.StringToHGlobalUTF8(markups[i]);
            }
            var aligns = alignments.ToArray();
            PangoBuiltText txt;
            fixed(IntPtr* stringPtr = stringPointers)
            {
                fixed(TextAlignment *alignPtr = aligns)
                {
                    txt = new PangoBuiltText(ctx, pg_buildtext(ctx, (IntPtr)stringPtr, (IntPtr)alignPtr, markups.Count, width));
                }
            }
            for (int i = 0; i < markups.Count; i++)
            {
                Marshal.FreeHGlobal(stringPointers[i]);
            }
            return txt;
        }

        int drawX, drawY;
        public override void RenderText(BuiltRichText txt, int x, int y)
        {
            drawX = x; drawY = y;
            pg_drawtext(ctx, ((PangoBuiltText)txt).Handle);
        }

        PGQuad[] lastQuads;
        // CACHES
        // TODO: Tune these
        struct StringResults
        {
            public int Hash;
            public float Size;
            public PGQuad[] Quads;
        }
        
        struct MeasureResults
        {
            public string Text;
            public int Font;
            public float Size;
            public Point Measured;
            public bool IsEqual(string text, int font, float size)
            {
                return ReferenceEquals(Text, text) &&
                       Math.Abs(Size - size) < 0.001f && Font == font;
            }
        }
        
        struct HeightResult
        {
            public int Hash;
            public float Size;
            public float LineHeight;
        }
        
        CircularBuffer<MeasureResults> measures = new CircularBuffer<MeasureResults>(64);
        CircularBuffer<StringResults> cachedStrings = new CircularBuffer<StringResults>(64);
        CircularBuffer<HeightResult> lineHeights = new CircularBuffer<HeightResult>(64);

        struct StringInfo
        {
            public int Underline;
            public unsafe int MakeHash(string fontName, string text)
            {
                fixed (StringInfo* si = &this)
                {
                    return FNV1A.Hash((IntPtr) si, sizeof(StringInfo),
                        FNV1A.Hash(text, FNV1A.Hash(fontName)));
                }
            }
        }

        internal ref struct UTF8ZHelper
        {
            private byte[] poolArray;
            private Span<byte> bytes;
            private Span<byte> utf8z;
            private bool used;
            public UTF8ZHelper(Span<byte> initialBuffer, ReadOnlySpan<char> value)
            {
                poolArray = null;
                bytes = initialBuffer;
                used = false;
                int maxSize = Encoding.UTF8.GetMaxByteCount(value.Length) + 1;
                if (bytes.Length < maxSize) {
                    poolArray = ArrayPool<byte>.Shared.Rent(maxSize);
                    bytes = new Span<byte>(poolArray);
                }
                int byteCount = Encoding.UTF8.GetBytes(value, bytes);
                bytes[byteCount] = 0;
                utf8z = bytes.Slice(0, byteCount + 1);
            }

            public Span<byte> ToUTF8Z()
            {
                return utf8z;
            }

            public void Dispose()
            {
                byte[] toReturn = poolArray;
                if (toReturn != null)
                {
                    poolArray = null;
                    ArrayPool<byte>.Shared.Return(toReturn);
                }
            }
        }

        
        public override void DrawStringBaseline(string fontName, float size, string text, float x, float y, Color4 color, bool underline = false, TextShadow shadow = default)
        {
            if(string.IsNullOrEmpty(fontName)) throw new InvalidOperationException("fontName null");
            var pixels = size * (96.0f / 72.0f);
            drawX = int.MaxValue;
            drawY = int.MaxValue;
            StringInfo info = new StringInfo()
            {
                Underline = underline ? 1 : 0
            };
            var hash = info.MakeHash(fontName, text);
            PGQuad[] quads = null;
            for (int i = 0; i < cachedStrings.Count; i++) {
                if (cachedStrings[i].Hash == hash && Math.Abs(cachedStrings[i].Size - size) < 0.001f)
                {
                    quads = cachedStrings[i].Quads;
                    break;
                }
            }
            if (quads == null)
            {
                using var textConv = new UTF8ZHelper(stackalloc byte[256], text);
                using var fontConv = new UTF8ZHelper(stackalloc byte[256], fontName);
                fixed(byte *tC = &textConv.ToUTF8Z().GetPinnableReference(),
                      tF = &fontConv.ToUTF8Z().GetPinnableReference())
                {
                    pg_drawstring(ctx, (IntPtr)tC, (IntPtr)tF, pixels, TextAlignment.Left, underline ? 1 : 0, 1, 1, 1, 1, (Color4*) 0,
                        (float*)0, (float*)0);
                }
                quads = lastQuads;
                lastQuads = null;
                cachedStrings.Enqueue(new StringResults() {Hash = hash, Size = size, Quads = quads});
            }

            drawX = (int) x;
            drawY = (int) y;
            if (shadow.Enabled)
            {
                for (int i = 0; i < quads.Length; i++)
                {
                    var q = quads[i];
                    q.Dest.X += drawX + 2;
                    q.Dest.Y += drawY + 2;
                    var t = textures[(int)q.Texture->UserData];
                    ren.Draw(t, q.Source, q.Dest, shadow.Color);
                }
            }
            for (int i = 0; i < quads.Length; i++)
            {
                var q = quads[i];
                q.Dest.X += drawX;
                q.Dest.Y += drawY;
                var t = textures[(int)q.Texture->UserData];
                ren.Draw(t, q.Source, q.Dest, color);
            }
        }

        
        public override Point MeasureString(string fontName, float size, string text)
        {
            if (string.IsNullOrEmpty(text)) return Point.Zero;
            if(string.IsNullOrEmpty(fontName)) throw new InvalidOperationException("fontName null");
            int fontHash = FNV1A.Hash(fontName);
            for (int i = 0; i < measures.Count; i++)
            {
                if (measures[i].IsEqual(text, fontHash, size))
                    return measures[i].Measured;
            }
            using var textConv = new UTF8ZHelper(stackalloc byte[256], text);
            using var fontConv = new UTF8ZHelper(stackalloc byte[256], fontName);
            fixed (byte* tC = &textConv.ToUTF8Z().GetPinnableReference(),
                tF = &fontConv.ToUTF8Z().GetPinnableReference())
            {
                pg_measurestring(ctx, (IntPtr)tC, (IntPtr)tF, size * (96.0f / 72.0f), out var width, out var height);
                var p =  new Point((int)width, (int)height);
                measures.Enqueue(new MeasureResults() {Text = text, Font = fontHash, Size = size, Measured = p});
                return p;
            }
        }
        public override float LineHeight(string fontName, float size)
        {
            if(string.IsNullOrEmpty(fontName)) throw new InvalidOperationException("LineHeight fontName cannot be null");
            int fontHash = FNV1A.Hash(fontName);
            for (int i = 0; i < lineHeights.Count; i++)
            {
                if (lineHeights[i].Hash == fontHash && Math.Abs(lineHeights[i].Size - size) < 0.001f)
                    return lineHeights[i].LineHeight;
            }
            using var fontConv = new UTF8ZHelper(stackalloc byte[256], fontName);
            fixed (byte* tF = &fontConv.ToUTF8Z().GetPinnableReference())
               
            {
                var retval = pg_lineheight(ctx, (IntPtr)tF, size * (96.0f / 72.0f));
                lineHeights.Enqueue(new HeightResult() {Hash = fontHash, Size = size, LineHeight = retval});
                return retval;
            }
        }

        void UpdateCache(ref CachedRenderString cache, string fontName, float size, string text, bool underline,
            TextAlignment alignment)
        {
            if (cache == null)
            {
                cache = new PangoRenderCache()
                {
                    FontName = fontName, FontSize = size, Text = text, Underline = underline,
                    Alignment = alignment
                };
            }
            if (cache is not PangoRenderCache pc) throw new ArgumentException("cache");
            if (pc.quads == null || pc.Update(fontName, text, size, underline, alignment))
            {
                var pixels = size * (96.0f / 72.0f);
                drawX = int.MaxValue;
                drawY = int.MaxValue;
                using var textConv = new UTF8ZHelper(stackalloc byte[256], text);
                using var fontConv = new UTF8ZHelper(stackalloc byte[256], fontName);
                float szX, szY;
                fixed(byte *tC = &textConv.ToUTF8Z().GetPinnableReference(),
                    tF = &fontConv.ToUTF8Z().GetPinnableReference())
                {
                    pg_drawstring(ctx, (IntPtr)tC, (IntPtr)tF, pixels, alignment, underline ? 1 : 0, 1, 1, 1, 1, (Color4*) 0, &szX, &szY);
                }
                pc.quads = lastQuads;
                pc.size = new Point((int) szX, (int) szY);
                lastQuads = null;
            }
        }

        public override void DrawStringCached(ref CachedRenderString cache, string fontName, float size, string text, float x, float y,
            Color4 color, bool underline = false, TextShadow shadow = default, TextAlignment alignment = TextAlignment.Left)
        {
            UpdateCache(ref cache, fontName, size, text, underline, alignment);
            var pc = (PangoRenderCache) cache;
            drawX = (int) x;
            drawY = (int) y;
            if (shadow.Enabled)
            {
                for (int i = 0; i < pc.quads.Length; i++)
                {
                    var q = pc.quads[i];
                    q.Dest.X += drawX + 2;
                    q.Dest.Y += drawY + 2;
                    var t = textures[(int)q.Texture->UserData];
                    ren.Draw(t, q.Source, q.Dest, shadow.Color);
                }
            }
            for (int i = 0; i < pc.quads.Length; i++)
            {
                var q = pc.quads[i];
                q.Dest.X += drawX;
                q.Dest.Y += drawY;
                var t = textures[(int)q.Texture->UserData];
                ren.Draw(t, q.Source, q.Dest, color);
            }
        }

        public override Point MeasureStringCached(ref CachedRenderString cache, string fontName, float size, string text, bool underline,
            TextAlignment alignment)
        {
            UpdateCache(ref cache, fontName, size, text, underline, alignment);
            var pc = (PangoRenderCache) cache;
            return pc.size;
        }

        class PangoRenderCache : CachedRenderString
        {
            internal PGQuad[] quads;
            internal Point size;
        }

        public override void Dispose()
        {
        }
    }
}
