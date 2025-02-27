#include "includes/Lighting.hlsl"
#include "includes/Modulate.hlsl"

Texture2D<float4> DtTexture : register(t0, TEXTURE_SPACE);
SamplerState DtSampler : register(s0, TEXTURE_SPACE);

Texture2D<float4> Dm0Texture : register(t1, TEXTURE_SPACE);
SamplerState Dm0Sampler : register(s1, TEXTURE_SPACE);

Texture2D<float4> Dm1Texture : register(t2, TEXTURE_SPACE);
SamplerState Dm1Sampler : register(s2, TEXTURE_SPACE);

cbuffer MaterialParameters : register(b3, UNIFORM_SPACE)
{
    float4 Dc;
    float4 Ac;
    float TileRate0;
    float TileRate1;
    float FlipU;
    float FlipV;
};

struct Input
{
    float2 texCoord1: TEXCOORD0;
    float2 texCoord2: TEXCOORD1;
    float3 worldPosition: TEXCOORD2;
    float3 normal: TEXCOORD3;
    float4 color: TEXCOORD4;
    float4 viewPosition: TEXCOORD5;
#ifdef VERTEX_LIGHTING
    float3 diffuseTermFront: TEXCOORD6;
    float3 diffuseTermBack: TEXCOORD7;
#endif
    bool frontFacing: SV_IsFrontFace;
};

float4 main(Input input) : SV_Target0
{
    float2 uv = float2(
        FlipU > 0 ? 1 - input.texCoord1.x : input.texCoord1.x,
        FlipV > 0 ? 1 - input.texCoord1.y : input.texCoord1.y
    );

    float4 tex = DtTexture.Sample(DtSampler, uv);
    float4 detail0 = Dm0Texture.Sample(Dm0Sampler, uv * TileRate0);
    float4 detail1 = Dm1Texture.Sample(Dm1Sampler, uv * TileRate1);

    float4 baseColor;
#ifdef VERTEX_LIGHTING
    baseColor = ApplyVertexLighting(Ac, 0, Dc, tex, input.viewPosition, input.frontFacing ? input.diffuseTermFront : input.diffuseTermBack);
#else
    baseColor = ApplyPixelLighting(Ac, 0, Dc, tex, input.worldPosition, input.viewPosition, input.normal, input.frontFacing);
#endif

    baseColor = Mod2x(detail0, baseColor);

    float4 illum = Mod2x(detail1, tex);

    return float4(lerp(baseColor.rgb, illum.rgb, illum.a), 1.0);
}
