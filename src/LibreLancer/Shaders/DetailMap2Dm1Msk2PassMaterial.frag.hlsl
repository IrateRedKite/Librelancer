#include "includes/Lighting.hlsl"
#include "includes/Modulate.hlsl"

Texture2D<float4> DtTexture : register(t0, TEXTURE_SPACE);
SamplerState DtSampler : register(s0, TEXTURE_SPACE);

Texture2D<float4> DmTexture : register(t1, TEXTURE_SPACE);
SamplerState DmSampler : register(s1, TEXTURE_SPACE);

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

cbuffer UniformParameters : register(b3, UNIFORM_SPACE)
{
    float4 Ac;
    float4 Dc;
    float TileRate;
    float FlipU;
    float FlipV;
};

float4 main(Input input) : SV_Target0
{
    float2 uv = float2(
       FlipU > 0 ? 1 - input.texCoord1.x : input.texCoord1.x,
       FlipV > 0 ? 1 - input.texCoord1.y : input.texCoord1.y);
    float4 tex = DtTexture.Sample(DtSampler, uv);
    float4 detail = DmTexture.Sample(DmSampler, uv * TileRate);

    float4 baseColor;
#ifdef VERTEX_LIGHTING
    baseColor = ApplyVertexLighting(Ac, 0, Dc, tex, input.viewPosition, input.frontFacing ? input.diffuseTermFront : input.diffuseTermBack);
#else
    baseColor = ApplyPixelLighting(Ac, 0, Dc, tex, input.worldPosition, input.viewPosition, input.normal, input.frontFacing);
#endif

    float4 masked = tex.a < 0.99 ? Mod2x(baseColor, detail) : baseColor;

    return float4(masked.rgb, 1.0);
};
