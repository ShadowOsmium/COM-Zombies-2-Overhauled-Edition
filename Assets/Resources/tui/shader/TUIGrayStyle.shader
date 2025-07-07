Shader "Triniti/TUI/TUIGrayStyle" {
    Properties {
        _MainTex ("MainTex", 2D) = "" {}
        _Color ("Main Color", Color) = (1, 1, 1, 1)
    }
    SubShader { 
    LOD 200
    Tags { "QUEUE"="Transparent" }
    Pass {
    Tags { "QUEUE"="Transparent" }
    Blend SrcAlpha OneMinusSrcAlpha

    CGPROGRAM

    #pragma vertex vert
    #pragma fragment frag

    struct appdata_t { 
        float4 vertex : POSITION;
        float2 texcoord0 : TEXCOORD0;
    };

    sampler2D _MainTex;
    float4 _Color;

    appdata_t vert(appdata_t v) {
        appdata_t o;
        o.vertex = UnityObjectToClipPos(v.vertex);
        o.texcoord0 = v.texcoord0;
        return o;
    }

    float4 frag(appdata_t i) : SV_TARGET {
        float4 color = (tex2D(_MainTex, i.texcoord0) * _Color);
        return float4((((color.x * 0.1) + (color.y * 0.6)) + (color.z * 0.3)).xxx, color.w);
    }

    ENDCG
    }
    }
}