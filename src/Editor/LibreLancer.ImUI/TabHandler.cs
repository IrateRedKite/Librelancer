﻿// MIT License - Copyright (c) Callum McGing
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

using System;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using ImGuiNET;
namespace LibreLancer.ImUI
{
    public unsafe class TabHandler
    {
        public static bool VerticalTab(string text, bool v)
        {
            var font = ImGuiNative.igGetFont();
            var dlist = ImGuiNative.igGetWindowDrawList();

            var style = ImGui.GetStyle();
            ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 0);
            ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, 0);
            var text_color = ImGui.GetColorU32(ImGuiCol.Text);
            var color = Theme.VTabInactive;
            if (v) color = style.Colors[(int)ImGuiCol.FrameBgActive];

            var textSize = ImGui.CalcTextSize(text);
            float pad = style.FramePadding.X;
            var pos = (Vector2)ImGui.GetCursorScreenPos() + new Vector2(pad, pad);
            pos = new Vector2(MathF.Floor(pos.X), MathF.Floor(pos.Y));
            ImGui.PushStyleColor(ImGuiCol.Button, color);
            ImGui.PushID(text);
            bool ret = ImGui.Button("", new Vector2(textSize.Y + pad * 2,
                                                    textSize.X + pad * 2));
            ImGui.PopStyleColor();
            ImGui.PopStyleVar(2);

            for(int i = 0; i < text.Length; i++) {
                var glyph = ImGuiExt.igFontFindGlyph(font, text[i]);
                ImGuiNative.ImDrawList_PrimReserve(
                dlist, 6, 4
                );
                ImGuiNative.ImDrawList_PrimQuadUV(
                    dlist,
                    pos + new Vector2(font->FontSize - glyph->Y0, glyph->X0),
                    pos + new Vector2(font->FontSize - glyph->Y0, glyph->X1),
                    pos + new Vector2(font->FontSize - glyph->Y1, glyph->X1),
                    pos + new Vector2(font->FontSize - glyph->Y1, glyph->X0),
                    new Vector2(glyph->U0, glyph->V0),
                    new Vector2(glyph->U1, glyph->V0),
                    new Vector2(glyph->U1, glyph->V1),
                    new Vector2(glyph->U0, glyph->V1),
                    text_color
                );
                pos.Y += glyph->AdvanceX;
            }
            ImGui.PopID();
            return ret;
        }
    }
}
