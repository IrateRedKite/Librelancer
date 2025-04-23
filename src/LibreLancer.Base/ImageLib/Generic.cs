﻿// MIT License - Copyright (c) Callum McGing
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

using System.IO;
using LibreLancer.Graphics;
using StbImageSharp;

namespace LibreLancer.ImageLib;

public static class Generic
{
    public static Image ImageFromStream(Stream stream, bool flip = false)
    {
        if (LIF.StreamIsLIF(stream))
        {
            return LIF.ImagesFromStream(stream)[0];
        }

        var len = (int)stream.Length;
        var b = new byte[len];
        var pos = 0;
        int r;

        while ((r = stream.Read(b, pos, len - pos)) > 0)
        {
            pos += r;
        }

        /* stb_image it */
        StbImage.stbi_set_flip_vertically_on_load(flip ? 1 : 0);
        ImageResult image = ImageResult.FromMemory(b, ColorComponents.RedGreenBlueAlpha);
        for (var i = 0; i < image.Data.Length; i += 4)
        {
            (image.Data[i + 2], image.Data[i]) = (image.Data[i], image.Data[i + 2]);
        }
        return new Image
        {
            Width = image.Width, Height = image.Height, Data = image.Data
        };
    }

    public static Texture TextureFromStream(RenderContext context, Stream stream, bool flip = true)
    {
        if (DDS.StreamIsDDS(stream))
        {
            return DDS.FromStream(context, stream);
        }

        if (LIF.StreamIsLIF(stream))
        {
            return LIF.TextureFromStream(context, stream);
        }

        /* Read full stream */
        var len = (int)stream.Length;
        var b = new byte[len];
        var pos = 0;
        int r;

        while ((r = stream.Read(b, pos, len - pos)) > 0)
        {
            pos += r;
        }

        /* stb_image it */
        StbImage.stbi_set_flip_vertically_on_load(flip ? 1 : 0);
        ImageResult image = ImageResult.FromMemory(b, ColorComponents.RedGreenBlueAlpha);
        for (var i = 0; i < image.Data.Length; i += 4)
        {
            (image.Data[i + 2], image.Data[i]) = (image.Data[i], image.Data[i + 2]);
        }

        var texture = new Texture2D(context, image.Width, image.Height, false, SurfaceFormat.Bgra8);
        texture.SetData(image.Data);
        return texture;
    }
}
