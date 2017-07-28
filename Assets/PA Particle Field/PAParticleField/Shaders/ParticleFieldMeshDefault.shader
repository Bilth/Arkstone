Shader "PA/ParticleField/MeshDefault" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_BumpMap ("Normal", 2D) = "bump" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags {"Queue"="Transparent" "RenderType"="Transparent" }
		LOD 200
		
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite On
		
		CGPROGRAM
		#pragma shader_feature _ DIRECTIONAL_ON SPIN_ON
		#pragma shader_feature _ WORLDSPACE_ON
		#pragma shader_feature _ SHAPE_SPHERE SHAPE_CYLINDER
		#pragma multi_compile _ EXCLUSION_ON
		#pragma shader_feature _ TURBULENCE_SIMPLEX2D TURBULENCE_SIMPLEX	

		#include "ParticleField.cginc"
		#include "ParticleMeshField.cginc"
		
		// Physically based Standard lighting model
		#pragma surface surf Standard keepalpha addshadow nometa nolightmap nodynlightmap nodirlightmap exclude_path:deferred exclude_path:prepass vertex:vert
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _BumpMap;

		struct Input {
			float2 uv_MainTex;
			float2 uv_BumpMap;
			fixed4 color : COLOR;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		
		void vert(inout appdata_full v, out Input o){
			UNITY_INITIALIZE_OUTPUT(Input, o);
			PAParticleMeshField(v);
			v.vertex = PAPositionVertexSurf(v.vertex);
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb * IN.color.rgb;
			o.Normal = UnpackNormal(tex2D (_BumpMap, IN.uv_BumpMap));
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a * IN.color.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
