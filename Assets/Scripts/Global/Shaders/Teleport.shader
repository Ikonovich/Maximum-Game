Shader "Hidden/TeleportShader"
{
    Properties {

        _MainTex("Texture", 2D) = "white" {}


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

            v2f vert(appdata v) 
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;

            }

            sampler2D _MainTex;

            fixed4 frag(v2f i) : SV_Target 
            {

                fixed4 col = tex2D(_MainTex, i.uv);

                

                col.r = 1;
                col.g = 1;
                col.b = 0;
                col.a = 0.5;


                return col;
            }
            ENDCG
        }

        Pass 
        {

            Tags 
            { 
            "LightMode" = "ForwardBase"
            }


            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc" // Provides UnityObjectToWorldNormal
            #include "UnityLightingCommon.cginc" // Provides _LightColor0

            struct v2f
            {
                float2 uv : TEXCOORD0;
                fixed4 diff : COLOR0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata_base v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;

                fixed4 lightCol = float4(1, 1, 1, 1);

                half3 worldNormal = UnityObjectToWorldNormal(v.normal);

                half light = max(0, dot(worldNormal,  1));

                o.diff = light * lightCol;
                return o;
            }

            sampler2D _MainTex;

            fixed4 frag(v2f i) : SV_TARGET {

                fixed4 col = tex2D(_MainTex, i.uv);

                // Multiplying color by lighting effect 
                col = col * i.diff;

                
                return col;

            
            }

            ENDCG
        }
    }
}

