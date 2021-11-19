Shader "World Space Shader" {

    SubShader {

            Tags {
                "RenderType" = "Transparent"
                "Queue" = "Transparent"
                "IgnoreProjector" = "True"
            }

            Blend SrcAlpha OneMinusSrcAlpha

            Pass {

                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct vertexInput {
                    float4 vertex : POSITION;

                };

                struct vertexOutput {
                    float4 pos : SV_POSITION;
                    float4 world_space: TEXCOORD0;
                };

                vertexOutput vert(vertexInput input) {

                    vertexOutput output;

                    output.pos = UnityObjectToClipPos(input.vertex);

                    output.world_space = mul(unity_ObjectToWorld, input.vertex);

                    return output;
                }

                float4 frag(vertexOutput input) : COLOR {

                    float4 cords = input.world_space;

                    float dist = distance(input.world_space, float4(0.0, 0.0, 0.0, 1.0));

                    if ((input.world_space.x > 500) && (input.world_space.x < 700)) {

                        return float4(0.0, 1.0, 0.0, 1.0);
                    }
                    else {
                        return float4(0.0, 0.0, 1.0, 0.0);
                    }
                }

                ENDCG
            }
        }
    
}