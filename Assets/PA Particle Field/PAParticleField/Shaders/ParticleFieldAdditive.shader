Shader "PA/ParticleField/Additive" {
	Properties {
		_Color ("Color", Color) = (1, 1, 1, 1)
		_MainTex ("Texture Image", 2D) = "white" {}
	}
	SubShader {	
		
		Tags{
			"Queue"="Transparent"
			"RenderType"="Transparent"
			"IgnoreProjector"="True"
		}
		
		Pass {

			ZWrite Off
			Blend One One
			
			CGPROGRAM
			
			#pragma exclude_renderers flash

			#pragma vertex vertParticleFieldCube
			#pragma fragment frag 
			#pragma multi_compile_fog
			#pragma target 3.0	
			
#pragma shader_feature _ DIRECTIONAL_ON SPIN_ON
#pragma shader_feature _ SOFTPARTICLES_ON
#pragma shader_feature _ WORLDSPACE_ON
#pragma shader_feature _ SHAPE_SPHERE SHAPE_CYLINDER
#pragma multi_compile _ EXCLUSION_ON
#pragma shader_feature _ TURBULENCE_SIMPLEX2D TURBULENCE_SIMPLEX		
			
			#include "UnityCG.cginc"
			#include "ParticleField.cginc"
			
			uniform sampler2D _MainTex;			
			fixed4 _Color;
			fixed _Softness;
			
			float4 frag(v2fParticleField i) : COLOR
			{
#ifdef SOFTPARTICLES_ON
				i.color.a *= GetAlphaFromDepth(i.projPos, _Softness);
#endif
			
				fixed4 col =  tex2D(_MainTex, i.tex.xy) * i.color * i.color.a * _Color;  
				UNITY_APPLY_FOG_COLOR(i.fogCoord, col, fixed4(0,0,0,0));
				return col;
			}
		
			ENDCG
		}
	}
}