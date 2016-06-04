Shader "TES Unity/Standard Tested"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0

		_Cutoff("Alpha Cutoff" , Float) = 0.5
	}
	SubShader
	{
		Tags { "RenderType"="TransparentCutout" "Queue"="AlphaTest" }
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite On

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input
		{
			float2 uv_MainTex;
		};
		
		fixed _Cutoff;
		half _Glossiness;
		half _Metallic;
		half4 _Color;

		void surf (Input IN, inout SurfaceOutputStandard o)
		{
			half4 c = tex2D (_MainTex, IN.uv_MainTex) ;

			if (c.a < _Cutoff) discard;

			o.Albedo = c.rgb * _Color.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a * _Color.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}