// MIT License - Copyright (c) Callum McGing
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

using System;
using System.Collections.Generic;
using LibreLancer.Data.Ini;
using LibreLancer.Data.IO;

namespace LibreLancer.Data
{
    public class FontsIni
    {
        public List<string> FontFiles = new List<string>();
        public List<UIFont> UIFonts = new List<UIFont>();
        public void AddFontsIni(string path, FileSystem vfs)
        {
            foreach (var section in IniFile.ParseFile(path, vfs))
            {
                if (section.Name.ToLowerInvariant() == "fontfiles")
                {
                    foreach(var entry in section)
                    {
                        FontFiles.Add(entry[0].ToString());
                    }
                }
                else if(section.Name.ToLowerInvariant() == "truetype")
                {
                    if (UIFont.TryParse(section, out var ui))
                        UIFonts.Add(ui);
                }
            }
        }
    }
}
