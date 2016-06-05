Shader "TES Unity/Alpha Tested"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Main Texture", 2D) = "white" {}
		_DetailTex("Detail Texture", 2D) = "white" {}
		_DarkTex("Occlusion Texture", 2D) = "white" {}
		_GlossTex("Gloss Texture", 2D) = "black" {}
		_GlowTex("Glow Texture", 2D) = "black" {}
		_Metallic("Metallic", Range(0,1)) = 0.0

		[HideInInspector]_Cutoff("cutoff", Float) = 0.5
	}
	SubShader
	{
		Tags { "RenderType"="TransparentCutout" "Queue"="AlphaTest" }
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite On

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows alpha
		#pragma target 3.0
		#include "MWCG.cginc"

		fixed _Cutoff;

		void surf (Input i, inout SurfaceOutputStandard o)
		{
			half4 diff = ReadDiffuse(i);

			if (diff.a < _Cutoff) discard;

			o.Albedo = diff.rgb;
			o.Emission = ReadGlow(i).rgb;
			o.Occlusion = ReadDark(i);
			o.Smoothness = ReadGloss(i);
			o.Metallic = _Metallic;
			o.Alpha = diff.a;
		}
		ENDCG
	}
	FallBack "Transparent/Cutout/Diffuse"
}