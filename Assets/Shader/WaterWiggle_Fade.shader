// Upgrade NOTE: replaced 'glstate_matrix_mvp' with 'UNITY_MATRIX_MVP'

Shader "Triniti/Extra/WaterWiggle_Fade" {
Properties {
 _Color ("Main Color", Color) = (1,1,1,1)
 _MainTex ("Main Texture (RGBA)", 2D) = "white" {}
 _WiggleTex ("Wiggle Texture (RB)", 2D) = "black" {}
 _FadeTex ("Fade Texture (RGBA)", 2D) = "white" {}
 _WiggleStrength ("Wiggle Strength", Range(0.01,0.1)) = 0.01
}
	SubShader{
 		LOD 190
 		Tags { "QUEUE"="Transparent" }
 		Pass {
 		 Tags { "QUEUE"="Transparent" }
 		 BindChannels {
 		  Bind "vertex", Vertex
 		  Bind "texcoord", TexCoord0
 		  Bind "texcoord1", TexCoord1
 		 }
 		 Blend SrcAlpha OneMinusSrcAlpha
		 CGPROGRAM
		 struct appdata_t {
			float4 vertex : POSITION;
			float4 texcoord0 : TEXCOORD0;
			float4 texcoord1 : TEXCOORD1;
		 };

		struct v2f {
			float4 vertex : POSITION;
            float2 texcoord0 : TEXCOORD0;
            float2 texcoord1 : TEXCOORD1;
			float2 texcoord2 : TEXCOORD2;
        };

		#pragma vertex vert
		#pragma fragment frag

		float4 _Color;
		sampler2D _MainTex;
		sampler2D _WiggleTex;
		sampler2D _FadeTex;
		float4 _MainTex_ST;
		float4 _WiggleTex_ST;
		float4 _FadeTex_ST;
		float _WiggleStrength;

		v2f vert(appdata_t v) {
			v2f o;
  			o.vertex = UnityObjectToClipPos(v.vertex);
  			o.texcoord0 = ((v.texcoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  			o.texcoord1 = ((v.texcoord1.xy * _WiggleTex_ST.xy) + _WiggleTex_ST.zw);
  			o.texcoord2 = ((v.texcoord0.xy * _FadeTex_ST.xy) + _FadeTex_ST.zw);
			return o;
		}

		float4 frag(v2f i) : SV_Target {
			float2 coord;
  			float4 wiggle = tex2D(_WiggleTex, float2((i.texcoord1.y + _CosTime.x), (i.texcoord1.x - _SinTime.x)));
  			coord.x = (i.texcoord0.x - (wiggle.x * _WiggleStrength));
  			coord.y = (i.texcoord0.y + ((wiggle.z * _WiggleStrength) * 1.5));
  			return (tex2D(_MainTex, coord) * _Color) * tex2D(_FadeTex, i.texcoord2);
		}

		 ENDCG
	}
}
}