Shader "Hidden/HealthbarShader"
{
    Properties {

        _MainTex("Texture", 2D) = "white" {}
        _Percent("Health Percent", float) = 1
        _Radius("Radius", float) = 1
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
            float _Radius;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                
                col.r = 0.8 * (1 - i.uv.x) * (1 - i.uv.x);

                col.b = i.uv.x * .3;
                col.g = i.uv.x;

                if (i.uv.x > _Percent) {
                    col.a = 0;
                }

                return col;
            }
            ENDCG


        }
    }
}

