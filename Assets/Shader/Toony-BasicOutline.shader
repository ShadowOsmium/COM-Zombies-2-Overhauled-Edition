Shader "Toon/Basic Outline"
{
	Properties
	{
		_Color("Main Color",         Color) = (0.5,0.5,0.5,1)
		_OutlineColor("Outline Color",      Color) = (0,0,0,1)
		_Outline("Outline width (m)",  Range(0.0005, 1.0)) = 0.04
		_OutlineScale("Outline multiplier", Range(0.1, 10.0)) = 5.0
		_MainTex("Base (RGB)",         2D) = "white" {}
		_Ramp("Toon Ramp (RGB)",    2D) = "gray" {}
		_ToonShade("Toon Cubemap (RGB)", CUBE) = "" {}
	}

		SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 200

		Pass
		{
			Name "OUTLINE"
			Tags { "LightMode" = "Always" }
			Cull Front
			ZWrite On
			ZTest LEqual
			ColorMask RGB

		Blend SrcAlpha OneMinusSrcAlpha

		CGPROGRAM
		#pragma vertex vert_outline
		#pragma fragment frag_outline
		#pragma target 3.0
		#include "UnityCG.cginc"

		struct appdata
		{
			float4 vertex : POSITION;
			float3 normal : NORMAL;
		};

		struct v2f_outline
		{
			float4 pos : SV_POSITION;
		};

		float _Outline;
		float _OutlineScale;
		float4 _OutlineColor;

		v2f_outline vert_outline(appdata v)
		{
			v2f_outline o;

			float3 worldNormal = UnityObjectToWorldNormal(v.normal);

			float4 worldPos = mul(unity_ObjectToWorld, v.vertex);

			worldPos.xyz += worldNormal * (_Outline * _OutlineScale);

			o.pos = UnityWorldToClipPos(worldPos);
			return o;
		}

		fixed4 frag_outline(v2f_outline i) : SV_Target
		{
			return _OutlineColor;
		}
		ENDCG
	}

		CGPROGRAM
		#pragma surface surf ToonRamp addshadow
		#pragma target 3.0
		#include "UnityCG.cginc"

		sampler2D _MainTex;
		sampler2D _Ramp;
		samplerCUBE _ToonShade;
		float4 _Color;

		struct Input
		{
			float2 uv_MainTex;
			float3 worldRefl;
		};

		void surf(Input IN, inout SurfaceOutput o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}

		fixed4 LightingToonRamp(SurfaceOutput s, fixed3 lightDir, fixed atten)
		{
			fixed diff = max(0, dot(s.Normal, lightDir));
			fixed3 ramp = tex2D(_Ramp, float2(diff, 0)).rgb;
			fixed4 col;
			col.rgb = s.Albedo * _LightColor0.rgb * ramp * atten;
			col.a = s.Alpha;
			return col;
		}
		ENDCG
	}

		Fallback "Diffuse"
}
