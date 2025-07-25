Texture2D<float4> Texture : register(t0, TEXTURE_SPACE);
SamplerState Sampler : register(s0, TEXTURE_SPACE);

struct Input
{
    float2 TexCoord : TEXCOORD0;
    float4 Color : TEXCOORD1;
};

float4 main(Input input) : SV_Target0
{
#ifdef USECOLOR
    float4 src = Texture.Sample(Sampler, input.TexCoord);
    return input.Color * src;
#else
    float4 src = Texture.Sample(Sampler, input.TexCoord);
    if(input.TexCoord.x > 3.0) src.a = 1.0;
    return input.Color * src;
#endif
}
