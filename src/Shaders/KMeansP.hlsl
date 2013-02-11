#define NUM_CLUSTERS 128
float4 main(float2 TexCoord : TEXCOORD0, uniform sampler BaseMap : register(S0), uniform sampler ClusterMap : register(S1), 
	uniform float centroidStride : register(C0), uniform float numClusters : register(C1), 
	uniform float3 centroids[NUM_CLUSTERS] : register(C2)) : COLOR
{
	float2 clusterData = tex2D(ClusterMap, TexCoord);
	float bestCluster = clusterData.y;
	float bestClusterIndex = clusterData.x;
	float4 color = tex2D(BaseMap, TexCoord);
	int clustersPerPass = min(NUM_CLUSTERS, (int)numClusters);
	for(int i = 0; i < clustersPerPass; i++)
	{
		float3 diff = color-centroids[i];
		float len = dot(diff,diff);
		if(len <= bestCluster)
		{
			bestCluster = len;
			bestClusterIndex = centroidStride + i;
		}
	}
	return float4(bestClusterIndex,bestCluster,0,0);
}