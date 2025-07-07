Shader "GGYY/Model/2COL_2TEX_AB" {
Properties {
 _MainColor ("Main Color", Color) = (1,1,1,1)
 _MainTex ("MainTex(RGB)", 2D) = "" {}
 _SkinColor ("Skin Color", Color) = (1,1,1,1)
 _SkinTex ("SkinTex(RGB)", 2D) = "" {}
}
SubShader { 
 Pass {
  Tags { "QUEUE"="Transparent" }
  BindChannels {
   Bind "vertex", Vertex
   Bind "texcoord", TexCoord
  }
  Blend SrcAlpha OneMinusSrcAlpha

  CGPROGRAM

	#pragma vertex vert
	#pragma fragment frag

	struct appdata_t {
		float4 vertex : POSITION;
		float2 texcoord0 : TEXCOORD0;
		float2 texcoord1 : TEXCOORD1;
	};

	struct v2f {
		float4 vertex : SV_POSITION;
		float2 texcoord0 : TEXCOORD0;
		float2 texcoord1 : TEXCOORD1;
	};

	sampler2D _MainTex, _SkinTex;
	float4 _MainColor, _SkinColor, _MainTex_ST, _SkinTex_ST;

	v2f vert(appdata_t v) {
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
    	o.texcoord0 = ((v.texcoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
    	o.texcoord1 = ((v.texcoord0.xy * _SkinTex_ST.xy) + _SkinTex_ST.zw);
		return o;
	}

	float4 frag(v2f i) : SV_TARGET {
		float4 color1 = (tex2D( _MainTex, i.texcoord0) * _MainColor);
    	float4 color2 = (tex2D( _SkinTex, i.texcoord1) * _SkinColor);
    	return (((((color1 * color2) * 4.0) * color1.w) * color2.w) + color1);
	}


  ENDCG
}
}
}