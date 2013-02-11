struct PSIN
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};

PSIN main(float4 Position : POSITION0, float2 TexCoord : TEXCOORD0,
uniform float2 InvRes : register(C0))
{
    PSIN OUT;

	OUT.Position = Position;
	OUT.TexCoord = TexCoord + 0.5 * InvRes;
	return OUT;
}