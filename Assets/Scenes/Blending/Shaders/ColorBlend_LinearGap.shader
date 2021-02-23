Shader "ColorBlend/LinearGap" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
		_GapColor("Gap Color", Color) = (0,0,0,0)
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

			float4 _Dir;
			float4 _GapColor;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
				float2 uv = i.uv;

				float dmax = 0;
				for (int i = 0; i < _Cells_Length; i++) {
					Cell cell = _Cells[i];
					float d = abs(dot((cell.uv - uv), _Dir));
					if (dmax < d) {
						dmax = d;
					}
				}

				float wsum = 0;
				float4 color = 0;
				for (i = 0; i < _Cells_Length; i++) {
					Cell cell = _Cells[i];
					float d = abs(dot((cell.uv - uv), _Dir));
					float w = (dmax - d) / dmax;
					wsum += w;
					color += w * _Colors[cell.icolor];
				}

				return color + saturate(1 - wsum) * _GapColor;
            }
            ENDCG
        }
    }
}
