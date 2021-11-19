Shader "MagicSmoke/Scanlines"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Density ("Scanline Density", Range(0.1, 2.0)) = 1.2
        _RollSpeed ("Scanline Roll Speed", Range(0.0, 20)) = 4.0
        _ScanlineOpacity ("Scanline Opacity",  Range(0.0, 1.0)) = 0.35
        _OpacityNoise ("Opacity Noise", Range(0.0, 1.0)) = 0.2
        _Flickering ("Flickering Amount", Range(0, 0.7)) = 0.05

    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

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

            float random(float2 st) {
                return frac(sin(dot(st.xy, float2(12.9898,78.233))) * 43758.5453123);
            }

            float blend(float x, float y) {
                return (x < 0.5) ? (2.0 * x * y) : (1.0 - 2.0 * (1.0 - x) * (1.0 - y));
            }

            float3 blendOpacity(float3 x, float3 y, float opacity) {
                float3 z = float3(blend(x.r, y.r), blend(x.g, y.g), blend(x.b, y.b));
                return z * opacity + x * (1.0 - opacity);
            }


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            float _Density;
            float _RollSpeed;
            float _Flickering;
            float _OpacityNoise;
            float _ScanlineOpacity;

            float4 frag (v2f i) : SV_Target
            {
                    
                float3 col = tex2D(_MainTex, i.uv).rgb;

                float scaledTime = _Time.y * _RollSpeed;
                float2 resolution = _ScreenParams.xy;
                float count = resolution.y * _Density;
                float2 sl = float2(sin(scaledTime + i.uv.y * count), cos(scaledTime + i.uv.y * count));
                float3 scanlines = float3(sl.x, sl.y, sl.x);
                float randomFactor = random(i.uv * scaledTime);

                col += col * scanlines * _ScanlineOpacity;
                col += col * float3(randomFactor, randomFactor, randomFactor) * _OpacityNoise;
                col += col * sin(110.0 * scaledTime) * _Flickering;
                return float4(col, 1.0);

            }
            ENDCG
        }
    }
}
