Shader "Unlit/InstancingSprite"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags
		{
			"Queue"="Transparent"
			"IgnoreProjector"="True"
			"RenderType"="Transparent"
		}
		Cull Off
		Lighting Off
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		// Blend SrcAlpha One

		Pass
		{
			CGPROGRAM
			// Upgrade NOTE: excluded shader from DX11, OpenGL ES 2.0 because it uses unsized arrays
			#pragma exclude_renderers d3d11 gles
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			
			#include "UnityCG.cginc"

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
			};

			struct ParticleData
			{
				float2 position;
				float2 radius;
			};

			StructuredBuffer<ParticleData> _ParticleDataBuffer;

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v, uint inst : SV_InstanceID)
			{
				v2f o;

				UNITY_SETUP_INSTANCE_ID(v);

				ParticleData p = _ParticleDataBuffer[inst];
				float scl = p.radius.x + p.radius.y;

				// スケーリングと平行移動
				float4x4 mat = (float4x4)0;
				mat._11_22_33_44 = float4(scl, scl, scl, 0);
				mat._14_24 += p.position;
				v.vertex = mul(mat, v.vertex);

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				return col;
			}
			ENDCG
		}
	}
}
