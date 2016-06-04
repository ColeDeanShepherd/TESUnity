Shader "TES Unity/Standard"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex("Main Texture", 2D) = "white" {}
		_DetailTex("Detail Texture", 2D) = "white" {}
		_DarkTex("Occlusion Texture", 2D) = "white" {}
		_GlossTex("Gloss Texture", 2D) = "black" {}
		_GlowTex ("Glow Texture", 2D) = "black" {}
		_Metallic("Metallic", Range(0,1)) = 0.0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" "Queue"="Geometry"}
		ZWrite On

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _DetailTex;
		sampler2D _DarkTex;
		sampler2D _GlossTex;
		sampler2D _GlowTex;
		half _Metallic;
		half4 _Color;

		struct Input
		{
			float2 uv_MainTex;
			float2 uv_DetailTex;
			float2 uv_DarkTex;
			float2 uv_GlowTex;
			float2 uv_GlossTex;
		};

		half4 ReadDiffuse( Input i )
		{
			half4 res = 1.0;
			res *= tex2D(_MainTex, i.uv_MainTex);
			res *= tex2D(_DetailTex, i.uv_DetailTex);
			//res *= tex2D(_DarkTex, i.uv_DarkTex);
			res *= _Color;
			return res;
		}

		half4 ReadDark(Input i)
		{
			return tex2D(_DarkTex, i.uv_DarkTex);
		}

		half ReadGloss(Input i)
		{
			return Luminance(tex2D(_GlossTex, i.uv_GlossTex).rgb);
		}

		half4 ReadGlow(Input i)
		{
			return tex2D(_GlowTex, i.uv_GlowTex);
		}

		void surf (Input i, inout SurfaceOutputStandard o)
		{
			half4 diff = ReadDiffuse(i);
			o.Albedo = diff.rgb;
			o.Emission = ReadGlow(i).rgb;
			o.Occlusion = ReadDark(i);
			o.Smoothness = ReadGloss(i);
			o.Metallic = _Metallic;
			o.Alpha = diff.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}