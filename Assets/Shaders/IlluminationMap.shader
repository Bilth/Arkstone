Shader "Dark Forest/IlluminationMap" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
	_EmissionTex("Emission Map (RGB)", 2D) = "white" {}
	_Bump("Bump", 2D) = "bump" {}
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 200

		Cull Off

		CGPROGRAM
#pragma surface surf Lambert

		sampler2D _MainTex;
	sampler2D _EmissionTex;
	sampler2D _Bump;

	struct Input {
		float2 uv_MainTex;
		float2 uv_EmissionTex;
		float2 uv_Bump;
	};

	void surf(Input IN, inout SurfaceOutput o) {
		half4 c = tex2D(_MainTex, IN.uv_MainTex);
		half4 em = tex2D(_EmissionTex, IN.uv_EmissionTex);
		o.Normal = UnpackNormal(tex2D(_Bump, IN.uv_Bump));

		o.Albedo = c.rgb;
		o.Alpha = c.a;
		o.Emission = em.rgb;
	}
	ENDCG
	}
		FallBack "Diffuse"
}