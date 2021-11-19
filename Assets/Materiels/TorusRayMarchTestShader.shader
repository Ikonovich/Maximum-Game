Shader "Unlit/RayMarchTest"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { 
            "RenderType"="Transparent" 
            "Queue" = "Transparent"

        }
    
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

#define MAX_STEPS 100
#define MAX_DIST 100
#define SURF_DIST 0.001

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

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float GetDist(float3 position) {

                //float distance = length(position) - 0.5; // sphere

                float distance = length(float2(length(position.xy) - 0.5, position.z)) - 0.1; // torus

                return distance;

               
            }

            float Raymarch(float3 ro, float3 rd) {
                float dO = 0;
                float dS;

                for(int i = 0; i < MAX_STEPS; i++) {
                    float3 position = ro + dO * rd;
                    dS = GetDist(position);

                    dO += dS;

                    if (dS < SURF_DIST || dO > MAX_DIST) break;

                }

                return dO;

            }

            float3 GetNormal(float3 position) {

                float2 epsilon = float2(0.01, 0);

                float3 normal = GetDist(position) - float3(
                    GetDist(position - epsilon.xyy),
                    GetDist(position - epsilon.yxy),
                    GetDist(position - epsilon.yyx)
                    );

                return (normalize(normal));
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv - 0.5;
                float3 ro = float3(0,0, -3);
                float3 rd = normalize(float3(uv.x, uv.y, 1));

                float distance = Raymarch(ro, rd);

                
                fixed4 col = 1;
                col.a = 0;

                if (distance < MAX_DIST) {
                    float3 position = ro + rd * distance;
                    float3 normal = GetNormal(position);
                    col.rgb = normal;
                    col.a = 1;
                }
                return col;

            }
            ENDCG
        }
    }
}
