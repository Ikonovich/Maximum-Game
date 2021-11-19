Shader "Hidden/GridShader"
{
    Properties {

        _Overlay("Overlay", 2D) = "black" {}
        _GridSize("Grid Size", float) = 1000
      	_Grid2Size("Grid 2 Size", Float) = 1600
		_Grid3Size("Grid 3 Size", Float) = 3200
        _Transparency("Transparency", float) = 1
    }

    SubShader 
    {

        Tags 
        { 
            "RenderType" = "Overlay"
        }
        LOD 100
        //ZTest Always 

        Pass 
        {

            
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            
            float _GridSize;
            float _Grid2Size;
            float _Grid3Size;

            float _Transparency;


            struct appdata
            {
                float4 vertex : POSITION;
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
                o.uv = mul(unity_ObjectToWorld, v.vertex).xz;
                return o;
            }
 
            float DrawGrid(float2 uv, float size, float aa) {

                float2 offset = float2(10, 10);

                float aaThresh = aa;
                float aaMin = aa * 0.1;

                float2 gUV = (uv / size + aaThresh) + offset;

                gUV = frac(gUV);

                gUV -= aaThresh;

                gUV = smoothstep(aaThresh, aaMin, abs(gUV));
                float d = max(gUV.x, gUV.y);

                return d;
            }

            sampler2D _Overlay;


            fixed4 frag (v2f i) : SV_Target {

                fixed r = DrawGrid(i.uv, _GridSize, 0.13);
                fixed b = DrawGrid(i.uv, _Grid2Size, 0.005);
				fixed g = DrawGrid(i.uv, _Grid3Size, 0.002);
				//return float4(1*r*_Transparency,1*g*_Transparency,1*b*_Transparency,(r+b+g)*_Transparency);
                float4 grid = float4(0,1*g*_Transparency,1*b*_Transparency,(r+b+g)*_Transparency);


                fixed4 overlay = tex2D(_Overlay, i.uv);

                fixed4 output = (overlay * overlay.a); // + grid;

                return output;

            }


            ENDCG


        }
    }
}

