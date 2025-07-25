// MIT License - Copyright (c) Callum McGing
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

using System.Collections.Generic;
using LibreLancer.Data.Ini;

namespace LibreLancer.Data.Universe
{
    [ParsedSection]
    public partial class ObjectProperties
    {
        [Entry("flag", Multiline = true)]
        public List<string> Flag = new List<string>();
    }
}
