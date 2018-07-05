Shader "Unlit/Particle_Offseted"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		[Enum(UnityEngine.Rendering.BlendMode)]_BlendSrc ("Blend Src", Float) = 0
		[Enum(UnityEngine.Rendering.BlendMode)]_BlendDst ("Blend Dst", Float) = 0
		
		[HideInInspector]_Offset("Offset", Int) = 0
	}
	SubShader
	{
		Tags
		{
			"Queue"="Transparent"
			"IgnoreProjection"="True"
			"RenderType"="Transparent"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		// Blend SrcAlpha OneMinusSrcAlpha
		Blend [_BlendSrc] [_BlendDst]

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			
			#include "UnityCG.cginc"
			#include "./Verlet.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				half4 color : COLOR;
			};

			StructuredBuffer<ParticleData> _ParticleDataBuffer;

			sampler2D _MainTex;
			float4 _MainTex_ST;

			int _Offset;
			
			v2f vert (appdata v, uint inst : SV_InstanceID)
			{
				v2f o;

				UNITY_SETUP_INSTANCE_ID(v);

				ParticleData p = _ParticleDataBuffer[inst + _Offset];
				float scl = p.size;

				// スケーリングと平行移動
				float4x4 mat = (float4x4)0;
				mat._11_22_33_44 = float4(scl, scl, scl, 0);
				mat._14_24 += p.position;
				v.vertex = mul(mat, v.vertex);

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.color = p.color;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv) * i.color;
				return col;
			}
			ENDCG
		}
	}
}
