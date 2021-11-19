Shader "Hidden/CurvedHealthbarShader"
{
    Properties {

        _MainTex("Texture", 2D) = "white" {}
        _Percent("Health Percent", float) = 0.5
        _InnerRadius("Inner Radius", float) = 0.3
        _MiddleRadius("Middle Radius", float) = 0.9
        _OuterRadius("Outer Radius", float) = 1.0


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

            fixed4 frag (v2f i) : SV_Target
            {

                fixed4 col = tex2D(_MainTex, i.uv);

                col.r = 1;
                col.g = 1;
                col.b = 1;
                col.a = 0;

                return col;
            }
            ENDCG
        }


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
            float _OuterRadius;
            float _MiddleRadius;
            float _InnerRadius;

            fixed4 frag (v2f i) : SV_Target
            {
                

                // Inverting the coordinates to flip the texture.

                //i.uv.y = 1.0 - i.uv.y;
                i.uv.x = 1.0 - i.uv.x;


                fixed4 col = tex2D(_MainTex, i.uv);

                float radiusActual = sqrt((i.uv.x * i.uv.x) + (i.uv.y * i.uv.y));

                if ((radiusActual > _InnerRadius) && (radiusActual < _MiddleRadius)) {
              
                   // col.g = 0.6 * (i.uv.x) * (i.uv.x);

                    col.b = (1 - i.uv.x);
                    col.r = (1 - i.uv.x);
                    col.a = 1;


                    
                    if (i.uv.x < _Percent) {

                        //col.a = 0;
                    }

                }
                else if ((radiusActual > _MiddleRadius) && (radiusActual < _OuterRadius)) {


                    col.r = 0.6 * (i.uv.x) * (i.uv.x);

                    col.b = (1 - i.uv.x) * .3;
                    col.g = (1 - i.uv.x);
                    col.a = 1;


                    
                    if (i.uv.x < _Percent) {

                        //col.a = 0;
                    }
                }
                else {

                    col.a = 0;
                }


                

                // Initial code

                // col.r = 0.8 * (1 - i.uv.x) * (1 - i.uv.x);

                // col.b = i.uv.x * .3;
                // col.g = i.uv.x;


                // // Makes the bar transparent if the percentage location of the pixel on the a axis
                // // is below the health percent.
                // if (i.uv.x > _Percent) {
                //     col.a = 0;
                // }

                // // Trims out a circle.

                // float radiusActual = sqrt((i.uv.x * i.uv.x) + (i.uv.y * i.uv.y));

                // if (radiusActual < _InnerRadius) {

                //     col.a = 0;


                // }

                
                // if (radiusActual > _OuterRadius) {

                //     col.a = 0;
                // }


                // if (i.uv.x > _Percent) {

                //     col.a = 0;
                // }

                return col;
            }
            ENDCG


        }
    }
}

