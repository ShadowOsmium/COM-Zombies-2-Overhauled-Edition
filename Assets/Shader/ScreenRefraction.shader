Shader "Triniti/Particle/ScreenRefraction" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _NoiseTex ("NoiseTex", 2D) = "white" {}
        _Amplitude ("Amplitude", Range(0,2)) = 0
        _Offset ("Offset", Range(0,1)) = 0
    }
    
    SubShader {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "Reflection"="LaserScope" }
        Pass {
            Tags { "Queue"="Transparent" "RenderType"="Transparent" "Reflection"="LaserScope" }
            ZWrite Off
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            
            #include "UnityCG.cginc"
            
            struct appdata_t {
                float4 vertex : POSITION;
                float4 texcoord : TEXCOORD0;
            };
            
            struct v2f {
                float4 vertex : SV_POSITION;
                float4 texcoord : TEXCOORD0;
                float4 texcoord1 : TEXCOORD1;
            };
            
            float _Amplitude;
            
            v2f vert (appdata_t v) {
                v2f o;
				float4 pos = UnityObjectToClipPos(v.vertex);
  				o.vertex = pos;
  				o.texcoord = pos;
  				o.texcoord1 = (((v.texcoord - 0.5) * ((_Amplitude * 4.0) + 1.0)) + 0.5);
				return o;
            }
            
            sampler2D _NoiseTex;
            sampler2D _ScreenImage;
            
            float4 frag (v2f i) : SV_Target {
				float4 coord = float4((((i.texcoord.xy / i.texcoord.w) * 0.5) + 0.5), i.texcoord.zw);
				coord.y = (1.0 - coord.y);
				float4 normal = tex2D(_NoiseTex, i.texcoord1);
  				return tex2D(_ScreenImage, (coord + (dot ((normal.xyz * normal.w), float3(0.0, 0.0, 0.1)) * _Amplitude)));
            }
            ENDCG
        }
    }
    Fallback Off
}
