using System.Collections.Generic;
using LibreLancer.Data.Ini;

namespace LibreLancer.Data.Universe.Rooms;

[ParsedSection]
public partial class FlashlightSet
{
    [Entry("icolor")]
    public Color3f IColor;
    [Entry("scale")]
    public float Scale;
    [Entry("gap")]
    public float Gap;
    [Entry("blink")]
    public float Blink;
    [Entry("endpause")]
    public float EndPause;
    [Entry("hardpoint", Multiline = true)]
    public List<string> Hardpoints = new List<string>();
    [Entry("numlights")]
    public int NumLights;
}
