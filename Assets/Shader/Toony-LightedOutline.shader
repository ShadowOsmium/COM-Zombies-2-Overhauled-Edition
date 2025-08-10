Shader "Toon/Lighted Outline" {
	Properties{
		_Color("Main Color", Color) = (0.5,0.5,0.5,1)
		_OutlineColor("Outline Color", Color) = (0,1,0,1)
		_Outline("Outline width", Range(0.0, 0.3)) = 0.1
		_MainTex("Base (RGB)", 2D) = "white" {}
		_ToonShade("ToonShader Cubemap(RGB)", CUBE) = "" {}
	}

		CGINCLUDE
#include "UnityCG.cginc"

		struct appdata {
		float4 vertex : POSITION;
		float3 normal : NORMAL;
	};

	struct v2f {
		float4 pos : SV_POSITION;
		fixed4 color : COLOR;
	};

	uniform float _Outline;
	uniform fixed4 _OutlineColor;

	v2f vert(appdata v) {
		v2f o;
		float3 worldNormal = UnityObjectToWorldNormal(v.normal);
		float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
		worldPos.xyz += worldNormal * (_Outline * 15.0);
		o.pos = UnityWorldToClipPos(worldPos);
		o.color = _OutlineColor;
		return o;
	}

	fixed4 frag(v2f i) : SV_Target{
		if (i.color.a <= 0.001) discard;
		return i.color;
	}
		ENDCG

		SubShader {
		Tags{ "RenderType" = "Opaque" }
			UsePass "Toon/Basic/BASE"

			Pass{
				Name "OUTLINE"
				Tags { "LightMode" = "Always" }
				Cull Front
				ZWrite On
				ColorMask RGB
				Blend SrcAlpha OneMinusSrcAlpha

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				ENDCG
		}
	}

	Fallback "Toon/Basic"
}
