Shader "Unlit/Edge"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		[Enum(UnityEngine.Rendering.BlendMode)]_BlendSrc ("Blend Src", Float) = 0
		[Enum(UnityEngine.Rendering.BlendMode)]_BlendDst ("Blend Dst", Float) = 0
	}
	SubShader
	{
		Tags
		{
			"Queue"="Transparent-10"
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
			StructuredBuffer<EdgeData> _EdgeDataBuffer;

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v, uint inst : SV_InstanceID)
			{
				v2f o;

				UNITY_SETUP_INSTANCE_ID(v);

				EdgeData e = _EdgeDataBuffer[inst];
				ParticleData pa = _ParticleDataBuffer[e.aID];
				ParticleData pb = _ParticleDataBuffer[e.bID];

				float2 delta = pa.position - pb.position;
				float len = length(delta);

				// 拡縮
				float2x2 scl = (float2x2)0;
				scl._11_22 = float2(len, e.width);
				v.vertex.xy = mul(scl, v.vertex.xy);

				// 回転
				float2x2 rot = (float2x2)0;
				float2 nDelta = normalize(delta);
				float rad = atan2(nDelta.y, nDelta.x);
				float tc = cos(rad);
				float ts = sin(rad);
				rot._11 = tc;
				rot._12 = -ts;
				rot._21 = ts;
				rot._22 = tc;
				v.vertex.xy = mul(rot, v.vertex.xy);

				// 移動
				float2 mid = (pa.position + pb.position) * 0.5;
				v.vertex.xy += mid;

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.color = e.color;
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
