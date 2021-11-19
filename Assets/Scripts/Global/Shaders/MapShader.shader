Shader "Hidden/MapShader" {

    Properties {

        _Background("Background Texture", 2D) = "red" {}
        _Overlay("Overlay", 2D) = "black" {}
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
                float4 vcolor : COLOR;

            };

            v2f vert(appdata_full v) 
            {

                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                o.vcolor = v.color;
                return o;

            }

            sampler2D _Background;
            sampler2D _Overlay;

            fixed4 frag(v2f i) : SV_Target 
            {

                // Inverting the coordinates to flip the texture.

                //i.uv.y = 1.0 - i.uv.y;
                //i.uv.x = 1.0 - i.uv.x;


                fixed4 background = tex2D(_Background, i.uv);
                //fixed4 overlay = tex2D(_Overlay, i.uv);

                fixed4 overlay = i.vcolor;

                fixed4 output = (overlay * overlay.a) + (background * (1 - overlay.a));

                

                return background;

            }

            ENDCG
        }
    }
}