Shader "Hidden/CustomAddPass" {

    Properties {

        _MainTex("Backing Texture", 2D) = "white" {}
        _Color ("Grid Color", Color) = (1, 0, 0, 1)

    }

    SubShader { 

        Tags { "RenderType" = "Transparent" }

        CGPROGRAM
        #pragma surface surf Lambert

        struct Input {
            float4 color : COLOR;
            float2 uv_MainTex;
        };

        sampler2D _MainTex;
        float4 _Color;

        void surf (Input IN, inout SurfaceOutput o) {

            o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
        }

        ENDCG



    }
    Fallback "Diffuse"

}