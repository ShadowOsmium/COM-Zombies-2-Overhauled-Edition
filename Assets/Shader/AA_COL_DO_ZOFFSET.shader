Shader "Triniti/Particle/AA_COL_DO_ZOFFSET" {
    Properties {
        _Color ("Main Color", Color) = (0.5, 0.5, 0.5, 0.5)
        _MainTex ("Particle Texture", 2D) = "white" {}
        _ZOffset ("Z Offset", Range(0, 1)) = 0
    }
    
    SubShader {
        Tags { "QUEUE"="Transparent" }
        Pass {
         Tags { "QUEUE"="Transparent" }
         BindChannels {
          Bind "vertex", Vertex
          Bind "color", Color
          Bind "texcoord", TexCoord
         }
         ZWrite Off
         Cull Off
         Fog {
          Color (0,0,0,0)
         }
         Blend SrcAlpha One
        CGPROGRAM

        #pragma vertex vert
        #pragma fragment frag

        struct appdata_t {
		    float4 vertex : POSITION;
		    float4 texcoord0 : TEXCOORD0;
		};

        struct v2f {
		    float4 vertex : POSITION;
            float2 texcoord0 : TEXCOORD0;
        };

        float4 _Color;
        sampler2D _MainTex;
        float _ZOffset;
        float4 _MainTex_ST;

        v2f vert(appdata_t v) {
            v2f o;
            float4 toWorld = mul(unity_ObjectToWorld, v.vertex);
            o.vertex = (UnityObjectToClipPos(float4(toWorld.xy, (toWorld.z + _ZOffset), toWorld.w)));
            o.texcoord0 = ((v.texcoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
            return o;
        }

        float4 frag(v2f i) : SV_TARGET {
            return ((tex2D(_MainTex, i.texcoord0) * _Color) * 2.0);
        }
        
        #include "UnityCG.cginc"

        ENDCG
        }

    }
}
