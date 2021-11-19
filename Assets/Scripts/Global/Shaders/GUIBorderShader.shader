Shader "Hidden/GUIborderShader"
{
    Properties {

        _MainTex("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (0,.5,1,1)
        _InnerRadius("Inner Radius", float) = 0.1
        _OuterRadius("Outer Radius", float) = .3
        _Angle ("Angle",Range(-3.14159, 3.14159)) = 0.4

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

            float GetAngle(float x, float y) {

                float theta = atan2(y, x);
               
                return theta;

            }

            sampler2D _MainTex;
            fixed4 _Color;
            float _OuterRadius;
            float _InnerRadius;
            float _Angle;
            

            fixed4 frag (v2f i) : SV_Target
            {
                
                fixed4 col = tex2D(_MainTex, i.uv);
                float angle = _Angle;


                // Inverting the coordinates to flip the texture.

                //i.uv.y = 1.0 - i.uv.y;
                //i.uv.x = 1.0 - i.uv.x;

                i.uv = i.uv * 2 - 1;

                float2 origin = float2(0, 1);
                float2 current = i.uv;

                float curAngle = GetAngle(i.uv.x, i.uv.y);

                if ((curAngle > angle) && (curAngle < angle + .1)) {
    
                        float4 output = _Color;
                        output.a = col.a * 0.2;
                        return output;
                }
                else {

                    col.a = 0;
                    return col;
                }


            }
            ENDCG

        }
    }
}

