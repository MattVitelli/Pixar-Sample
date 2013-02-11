float4 main(float2 TC : TEXCOORD0, uniform sampler BaseMap : register(S0), uniform float2 InvRes : register(C0), uniform float sharpness : register(C1)) : COLOR
{
	
	float3 avgValue = 0;
    
    float weight = 0;
    float variance = 0;

    avgValue = tex2D(BaseMap, TC);
    //avgValue /= dot(avgValue,1);
    for(int i = -1; i <= 1; i++)
    {
		for(int j = -1; j <= 1; j++)
		{
			float3 samp = tex2D(BaseMap, TC+float2(i,j)*InvRes);
			//samp /= dot(samp, 1);
			float v = dot(samp-avgValue, samp-avgValue);//length(samp - avgValue);
			//v *= v;
			variance += v;
			weight++;
		}
    }
    variance /= (weight-1);
    
    return pow(exp((variance)/-0.24),sharpness);
    //return (variance>Threshold) ? 0 : 1;
}