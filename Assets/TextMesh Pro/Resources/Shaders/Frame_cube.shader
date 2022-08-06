Shader "TextMeshPro/Frame_cube"

{
    Properties
    {
		_Color("Color", color) = (1,1,1,1)
		_Width("Width", range(0,0.5)) = 0.1
    }
    SubShader
    {
		Tags { "Queue"="Transparent" }
		Pass {

  //如果要显示背面的线框，取消下面两个注释即可
			cull off
			ZWrite off
			blend srcalpha oneminussrcalpha
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			fixed4 _Color;
			fixed _Width;

			struct a2v {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert(a2v v) {
				v2f o;

				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;

				return o;
			}

			float4 frag(v2f i) : SV_Target {

				fixed4 col = fixed4(0,0,0,0);//Cube的基础颜色
				
				col += saturate(step(i.uv.x, _Width) + step(1 - _Width, i.uv.x) + step(i.uv.y, _Width) + step(1 - _Width, i.uv.y)) * _Color;

				//if (i.uv.x < _Width || i.uv.x > 1 - _Width || i.uv.y < _Width || i.uv.y > 1 - _Width) 
				//{
				//	col = _Color;
				//}

				return  col;
			}

			ENDCG
		}
    }
}

