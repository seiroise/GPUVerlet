﻿Shader "Unlit/Watercolor"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_GradientTex ("Gradient Texture", 2D) = ""{}
		_NoiseTex("Noise Texture", 2D) = ""{}
		_RemapTex("Remap Texture", 2D) = ""{}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			sampler2D _GradientTex;
			sampler2D _NoiseTex;
			sampler2D _RemapTex;

			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// グラデーションにノイズをブレンドする
				fixed grad = tex2D(_GradientTex, i.uv);
				fixed noise = tex2D(_NoiseTex, i.uv);

				fixed c = clamp(grad * noise + grad, 0.0, 1.0);

				// ブレンドした値をremapテクスチャにマッピングする
				fixed4 col  = tex2D(_RemapTex, c);

				return col;
			}
			ENDCG
		}
	}
}
