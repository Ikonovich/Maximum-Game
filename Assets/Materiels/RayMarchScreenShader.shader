// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/RayMarchScreenShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {

        Tags 
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent" 
            "IgnoreProjector" = "True"

        }
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

#define MAX_STEPS 40

            uniform float4x4 _FrustrumCornersES;
            uniform sampler2D _MainTex;
            uniform float4 _MainTex_TexelSize;
            uniform float4x4 _CameraInvViewMatrix;
            uniform sampler2D _CameraDepthTexture;
            uniform float3 _CameraWS;
            uniform float3 _LightDir;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float3 ray : TEXCOORD1;
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;

                // Index passed by custom blit function in ScreenSpaceEffect.cs
                half index = v.vertex.z;
                v.vertex.z = 0.1;

                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv.xy;


                // Inverting y coordinates

                #if UNITY_UV_STARTS_AT_TOP
                if (_MainTex_TexelSize.y < 0)
                    o.uv.y = 1 - o.uv.y;
                #endif

                // Get the normalized eyespace view ray
                o.ray = _FrustrumCornersES[(int)index].xyz;


                // Normalizing the ray in the z axis
                o.ray /= abs(o.ray.z);

                // Transform the ray from eyespace to worldspace
                o.ray = mul(_CameraInvViewMatrix, o.ray);
                return o;

            }

            float Union(float item1, float item2) {

                return min(item1, item2);
            }

            float Difference(float item1, float item2) {

                return max(-item1, item2);
            }

            float Intersection(float item1, float item2) {

                return max(item1, item2);
            }




            float TorusPoint(float3 p, float2 t) {

                float2 q = float2(length(p.xz) - t.x, p.y);
                return length(q) - t.y;
            }

            uniform float4x4 _MatTorus_InvModel;

            // This function returns a float4 composed of the color
            // of the object being drawn as the RGB values and the
            // distance to the object point as the A value.
            float4 GetDist(float3 p) {

                float4 torusSample = mul(_MatTorus_InvModel, float4(p, 1));
                float t = TorusPoint(torusSample.xyz, float2(100, 10.2)); // torus 0
                float3 color = float3(0, 1, 1);

                //float s = length(float2(length(p.xy) - 0.5, p.z)) - 0.1; // torus 1

                float s = length(p + (float3(0, 0, 0))) - 100; // sphere implicit

                float c = (length(p + (float3(0, -200, 0))) * p.y) - 100;


                float3 bVec = abs(p) - float3(50.0, 50.0, 50.0); // box size
                float b = length(max(bVec, 0)) + min(max(bVec.x, max(bVec.y, bVec.z)), 0); // box



                float4 d = float4(color, min(min(t, s), b)); // blends torus, sphere, abd cubee,


                return float4(color, Union(s, t));

            }



            float3 GetNormal(float3 position) {

                float2 epsilon = float2(0.01, 0);

                float3 normal = GetDist(position).a - float3(
                    GetDist(position - epsilon.xyy).a,
                    GetDist(position - epsilon.yxy).a,
                    GetDist(position - epsilon.yyx).a
                    );

                return (normalize(normal));
            }


            fixed4 Raymarch(float3 ro, float3 rd, float s) {

                ro = ro + float3(-1000, -200, -1000);

                fixed4 output = float4(0,0,0,0);
                float t = 0; // Current distance travelled

                for (int i = 0; i < MAX_STEPS; i++) {

                    if (t >= s) {
                        output = fixed4(0, 0, 0, 0);
                        break;
                    }

                    float3 p = ro + rd * t;
                    float4 draw = GetDist(p);
                    float d = draw.a;

                    // If the distance is <= 0, hit an object.
                    if (d < 0.01) {

                        float r = draw.r;

                        float g = draw.g;

                        float b = draw.b;
                        float a = 1;


                        float3 n = GetNormal(p);
                        float lightSource = float3(dot(-_LightDir.xyz, n), dot(-_LightDir.xyz, n), dot(-_LightDir.xyz, n));
                        //output = fixed4(lightSource * sin(_Time.w) * r, lightSource * cos(0.5 * _Time.w) * g, lightSource * sin(_Time.w) * b, a); // Time dependent lighting
                        //output = fixed4(lightSource * r, lightSource * g, lightSource * b, a);   // Constant lighting

                        output = fixed4(r, g, b, a);
                        
                        break;
                    }

                    // Increment by the minimum possible distance
                    // until an object is hit.
                    t += d;

                }
                return output;
            }





            fixed4 frag (v2f i) : SV_Target
            {

                // Ray direction
                float3 rd = normalize(i.ray.xyz);

                // Camera position
                float3 ro = _CameraWS;

                float2 duv = i.uv;

                #if UNITY_UV_STARTS_AT_TOP
                if (_MainTex_TexelSize.y < 0) 
                    duv.y = 1 - duv.y;
                #endif

                // This converts from the depth buffer/eyespace to
                // the distance to the camera, by multiplying eyespace
                // depth by the length of the z normal ray from vert.

                float depth = LinearEyeDepth(tex2D(_CameraDepthTexture, duv).r);
                depth *= length(i.ray.xyz);


                // Calculating the output by taking the texture
                // and raymarch and blending them with alpha blending.

                fixed3 col = tex2D(_MainTex, i.uv);
                fixed4 rayCol = Raymarch(ro, rd, depth);


                fixed4 output = fixed4(col * (1.0 - rayCol.w) + rayCol.xyz * rayCol.w, 1.0);
                
                return output;
            }
            ENDCG
        }
    }
}
