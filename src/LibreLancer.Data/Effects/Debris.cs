using System.Numerics;
using LibreLancer.Ini;

namespace LibreLancer.Data.Effects;

public class Debris
{
    [Entry("nickname", Required = true)] public string Nickname;
    [Entry("death_method")] public string DeathMethod;
    [Entry("lifetime")] public ValueRange<float> Lifetime;
    [Entry("linear_drag")] public float LinearDrag;
    [Entry("angular_drag")] public Vector3 AngularDrag;
    [Entry("rotation_inertia")] public Vector3 RotationInertia;
    [Entry("trail")] public string Trail;
    [Entry("explosion")] public string Explosion;
}
