Shader "Hidden/TerrainOverlayShader" {

    Properties {

        _Overlay("Overlay", 2D) = "black" {}
        _Speed("Scan Speed", Range(0.0, 10.0)) = 1.0
        _ScanColor("Scan Color", Color) = (0, 0, 0, 0.5)
        _LineWidth("Scan Speed", Range(0.0, 10.0)) = 10.0

    }

    SubShader 
    {

        Tags 
        {
            "Queue" = "Transparent"
        }

        Blend SrcAlpha OneMinusSrcAlpha

        
        Cull Off ZWrite Off ZTest Always

        
        Pass 
        {


            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"



            struct v2f 
            {

                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;

            };

            v2f vert(appdata_base v) 
            {

                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;

            }

            sampler2D _Overlay;

            fixed4 frag(v2f i) : SV_Target 
            {

                
                // Swapping the axis

                float tempx = i.uv.x;
                float tempy = i.uv.y;

                i.uv.x = tempy;
                i.uv.y = tempx;

                //i.uv.y = 1.0 - i.uv.y;
                //i.uv.x = 1.0 - i.uv.x;


                fixed4 overlay = tex2D(_Overlay, i.uv);

                fixed4 output = (overlay * overlay.a * 0.5);

                return output;

            }

            ENDCG
        }

        Pass {

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"



            struct v2f 
            {

                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;

            };

            float scanlineAlpha(float y, float timeFactor, float width) {

                float checkUp = (((y - 0.001) < (timeFactor)) ? 1 : 0);
                float checkDown = (((y + 0.001) > (timeFactor)) ? 1 : 0);

                return checkUp * checkDown;
            }

             v2f vert(appdata_base v) 
            {

                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;

            }

            fixed4 _ScanColor;
            float _Speed;
            float _LineWidth;

            fixed4 frag(v2f i) : SV_Target 
            {

                
                // Inverting the coordinates to flip the texture.

                //i.uv.y = 1.0 - i.uv.y;
                //i.uv.x = 1.0 - i.uv.x;

                
                float timeFactor = (cos(_Time.y) > 0) ? sin(_Time.y) : 0;
                
                float scanAlpha = scanlineAlpha(i.uv.x, timeFactor, _LineWidth);
                fixed4 output = scanAlpha * _ScanColor;

                return output;

            }

            ENDCG
        }
    }
}