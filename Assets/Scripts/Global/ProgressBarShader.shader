Shader "Hidden/ProgressBarShader" 
{
    Properties 
    {

        _MainTex("Texture", 2D) = "white" {}
        _Percent("Progress Percent", float) = 0.0
    }

    SubShader 
    {
        Tags 
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"

        }
        
        Blend SrcAlpha OneMinusSrcAlpha

        Pass 
        {

            CGPROGRAM 
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata 
            {

                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f 
            {

                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v) 
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            
            sampler2D _MainTex;
            float _Percent;

            fixed4 frag (v2f i) : SV_Target 
            {

                fixed4 col = tex2D(_MainTex, i.uv);

                col.r = 0.0;
                col.g = 0.0;
                col.b = 1.0;

                if (i.uv.x > _Percent) {
                    col.a = 0.0;
                    col.b = 0.0;

                }

                return col;
            }
            ENDCG
        }
    }
}