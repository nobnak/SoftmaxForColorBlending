Shader "ColorBlend/Softmax" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
		_Temp ("Temperature", Range(0.01, 1)) = 1
    }
    SubShader {
        Tags { "RenderType"="Opaque" }

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
			#include "Commons.cginc"

            struct appdata {
                float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

			float _Temp;
			float4 _Dir;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
				float2 uv = i.uv;

				float maxd = 0;
				for (int i = 0; i < _Cells_Length; i++) {
					Cell cell = _Cells[i];
					float d = -abs(dot((cell.uv - uv), _Dir));
					if (maxd < d) {
						maxd = d;
					}
				}

				float wsum = 0;
				float4 color = 0;
				float wmax = 0;
				for (i = 0; i < _Cells_Length; i++) {
					Cell cell = _Cells[i];
					float d = -abs(dot((cell.uv - uv), _Dir));
					float w = exp((d - maxd) / _Temp);
					wsum += w;
					color += w * _Colors[cell.icolor];
					if (wmax < w) {
						wmax = w;
					}
				}
				if (wsum > 0) {
					color /= wsum;
					wmax /= wsum;
				}

				return color;
            }
            ENDCG
        }
    }
}
