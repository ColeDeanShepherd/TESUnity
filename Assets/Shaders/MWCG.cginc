// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'


half4 _Color;
sampler2D _MainTex;
sampler2D _DetailTex;
sampler2D _DarkTex;//occlusion?
sampler2D _GlossTex;
sampler2D _GlowTex;
half _Metallic;

struct Input
{
	float2 uv_MainTex;
	float2 uv_DetailTex;
	float2 uv_DarkTex;
	float2 uv_GlowTex;
	float2 uv_GlossTex;
};



void vertwavy(inout appdata_full v)
{
	float3 wpos = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1.0)).xyz;
	float3 wnor = mul(unity_ObjectToWorld, float4(v.normal.xyz, 0.0)).xyz;
	wpos.xz += wnor.xz * sin(_Time.y - wpos.y) * 0.1;
	v.vertex.xyz = mul(unity_WorldToObject, float4(wpos.xyz, 1.0)).xyz;
}




half4 ReadDiffuse(Input i)
{
	half4 res = 1.0;
	res *= tex2D(_MainTex, i.uv_MainTex);
	res *= tex2D(_DetailTex, i.uv_DetailTex);
	res *= _Color;
	return res;
}

half ReadDark(Input i)
{
	return Luminance(tex2D(_DarkTex, i.uv_DarkTex).rgb);
}

half ReadGloss(Input i)
{
	return Luminance(tex2D(_GlossTex, i.uv_GlossTex).rgb);
}

half4 ReadGlow(Input i)
{
	return tex2D(_GlowTex, i.uv_GlowTex);
}
