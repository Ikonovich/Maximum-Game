Shader "Hidden/ButtonCircleShader"
{
    Properties {

        _MainTex("Texture", 2D) = "white" {}
        _Radius("Outer Radius", float) = 1


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
                

                // Inverting the coordinates to flip the texture.

                //i.uv.y = 1.0 - i.uv.y;
                //i.uv.x = 1.0 - i.uv.x;

                float xPoint = i.uv.x - 0.5;
                float yPoint = i.uv.y - 0.5;


                fixed4 col = tex2D(_MainTex, i.uv);

                float radiusActual = sqrt((xPoint * xPoint) + (yPoint * yPoint));

                if (radiusActual > _Radius) {

                    col.a = 0;
                }


                return col;
            }
            ENDCG


        }
    }
}

