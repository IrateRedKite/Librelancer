// MIT License - Copyright (c) Callum McGing
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

using System;
using System.Collections.Generic;
using LibreLancer.Data.Ini;
using LibreLancer.Data.IO;

namespace LibreLancer.Data.Missions
{
    [ParsedIni]
    public partial class NewsIni
    {
        [Section("NewsItem")] public List<NewsItem> NewsItems = new List<NewsItem>();
        public void AddNewsIni(string path, FileSystem vfs)
        {
            ParseIni(path, vfs);
        }
    }

    [ParsedSection]
    public partial class NewsItem
    {
        [Entry("rank")] public string[] Rank;
        [Entry("icon")] public string Icon;
        [Entry("logo")] public string Logo;
        [Entry("category")] public int Category;
        [Entry("headline")] public int Headline;
        [Entry("text")] public int Text;
        [Entry("base", Multiline = true)] public List<string> Base = new List<string>();
        [Entry("autoselect", Presence = true)] public bool Autoselect;
        [Entry("audio", Presence = true)] public string Audio; //Unused in vanilla
    }
}
