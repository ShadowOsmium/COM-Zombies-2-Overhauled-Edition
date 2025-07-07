// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'glstate_matrix_mvp' with 'UNITY_MATRIX_MVP'

Shader "Triniti/Environment/WIND_COL_AB_2S_DO" {
Properties {
 _Color ("Main Color", Color) = (1,1,1,1)
 _MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
 _Wind ("Wind params", Vector) = (1,1,1,1)
 _WindEdgeFlutter ("Wind edge fultter factor", Float) = 0.5
 _WindEdgeFlutterFreqScale ("Wind edge fultter freq scale", Float) = 0.5
 _offset ("         ", Float) = 0
}
SubShader { 
 LOD 100
 Tags { "LIGHTMODE"="ForwardBase" "QUEUE"="Transparent" "RenderType"="Transparent" }
 Pass {
  Tags { "LIGHTMODE"="ForwardBase" "QUEUE"="Transparent" "RenderType"="Transparent" }
  ZWrite Off
  //turn on culling if it's too goofy
  Cull Off
  Blend SrcAlpha OneMinusSrcAlpha
	CGPROGRAM

	#pragma vertex vert
	#pragma fragment frag

	struct appdata_t {
		float4 vertex : POSITION;
		float4 color : COLOR;
		float4 normal : NORMAL;
		float2 texcoord0 : TEXCOORD0;
	};

	struct v2f {
		float4 vertex : POSITION;
		float2 texcoord0 : TEXCOORD0;
	};

	sampler2D _MainTex;
	float4 _Color, _Wind, _MainTex_ST;
	float _WindEdgeFlutter, _WindEdgeFlutterFreqScale, _offset;

	v2f vert(appdata_t v) {
		v2f o;
  		float4 wind = float4(unity_WorldToObject[0].xyz * _Wind.xyz, (_Wind.w * v.color.x));
  		float4 bendTime = abs(((frac((((frac((((_Time.y * float2(_WindEdgeFlutterFreqScale, 1.0)).xx + float2(dot(v.vertex.xyz, _WindEdgeFlutter.xxx), 0.0)).xxyy * float4(1.975, 0.793, 0.375, 0.193))) * 2.0) - 1.0) + 0.5)) * 2.0) - 1.0));
  		float4 bendMult = ((bendTime * bendTime) * (3.0 - (2.0 * bendTime)));
		float3 bend;
  		bend.xz = ((_WindEdgeFlutter * 0.1) * normalize(v.normal)).xz;
  		bend.y = (v.color.x * 0.3);
		float2 bending = (bendMult.xz + bendMult.yw);
  		o.vertex = UnityObjectToClipPos(float4(((v.vertex.xyz + (((bending.xyx * bend) + ((wind.xyz * bending.y) * v.color.x)) * wind.w)) + (v.color.x * wind.xyz)), v.vertex.w));
  		o.texcoord0 = ((v.texcoord0 * _MainTex_ST.xy) + _MainTex_ST.zw);
		return o;
	}

	float4 frag(v2f i) : SV_TARGET {
  		return (tex2D(_MainTex, i.texcoord0) * (_Color * 2.0));
	}

	ENDCG
 }
}
}