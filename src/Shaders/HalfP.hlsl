float4 main(float2 TexCoord : TEXCOORD0, uniform sampler BaseMap : register(S0), uniform float2 InvRes : register(C0)) : COLOR
{
	float4 color = tex2D(BaseMap, TexCoord);
	color += tex2D(BaseMap, TexCoord+float2(InvRes.x,0));
	color += tex2D(BaseMap, TexCoord+float2(0,InvRes.y));
	color += tex2D(BaseMap, TexCoord+InvRes);
	color *= 0.25;
	return color;
}