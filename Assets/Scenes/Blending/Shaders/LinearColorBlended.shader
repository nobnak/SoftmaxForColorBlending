Shader "Unlit/LinearColorBlended" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
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
            };

            struct v2f {
                float4 vertex : SV_POSITION;
				float4 clip : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.clip = o.vertex;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
				float2 clip = i.clip.xy / i.clip.w;
				float4 uvNpos = mul(_Clip_To_UV_Npos_Matrix, float4(clip, 0, 1));
				//return float4(0.5 * (clip.xy + 1), 0, 1);
				return float4(uvNpos.zw, 0, 1);
            }
            ENDCG
        }
    }
}
