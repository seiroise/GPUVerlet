Shader "Custom/GPUParticle" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Standard vertex:vert addshadow
		#pragma instancing_options procedural:setup

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		struct ParticleData {
			float2 position;
			float2 radius;
		};

		#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
		StructuredBuffer<ParticleData> _ParticleDataBuffer;
		#endif

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		void vert(inout appdata_full v) {
			#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED

			ParticleData p = _ParticleDataBuffer[unity_InstanceID];

			float4x4 object2world = (float4x4)0.0;

			// スケール行列
			float scl = p.radius.x + p.radius.y;
			object2world._11_22_33_44 = float4(scl, scl, scl, 1.0);

			// 移動行列
			object2world._14_24_34 += float3(p.position, 0.0);

			v.vertex = mul(object2world, v.vertex);
			#endif
		}

		void setup()
		{
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
