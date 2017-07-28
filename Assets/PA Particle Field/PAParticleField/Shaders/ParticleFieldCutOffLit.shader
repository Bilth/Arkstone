Shader "PA/ParticleField/CutOffLit" {
	Properties {
		_Color ("Color", Color) = (1, 1, 1, 1)
		_MainTex ("Texture Image", 2D) = "white" {}
	}
	SubShader {		
	
		Tags{
			"Queue"="AlphaTest"
			"RenderType"="Transparent"
			"IgnoreProjector"="True"
		}

        Pass {
            Name "ForwardBase"
            Tags {
                "LightMode"="ForwardBase"
            }
            
			ZWrite On
			
			CGPROGRAM
			
			#pragma multi_compile_fwdbase noshadow nometa nolightmap nodynlightmap nodirlightmap exclude_path:deferred exclude_path:prepass
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
#define LIGHTING_ON
            
			#include "UnityCG.cginc"
			#include "AutoLight.cginc"
			#include "ParticleField.cginc"
			#include "ParticleFieldCGLight.cginc"
			
			
			uniform sampler2D _MainTex;			
			fixed4 _Color;			
			fixed _CutOff;
			
			fixed _Softness;
			
			float4 frag(v2fParticleField i) : COLOR
			{

				i.color.rgb *= ForwardBaseLight(i);
				
				fixed4 col = tex2D(_MainTex, i.tex.xy) * i.color * _Color;
				
#ifdef SOFTPARTICLES_ON
				i.color.a *= GetAlphaFromDepth(i.projPos, _Softness);
#endif

				clip (col.a - _CutOff);
				
				UNITY_APPLY_FOG(i.fogCoord, col);

				return col;  
			}
		
			ENDCG
		}
		
		 Pass {
            Name "ForwardAdd"
            Tags {
                "LightMode"="ForwardAdd"
            }

            Blend One One
            ZWrite Off
			
			CGPROGRAM
			
			#pragma multi_compile_fwdadd
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
#define LIGHTING_ON

			#include "UnityCG.cginc"
			#include "AutoLight.cginc"
			#include "ParticleField.cginc"	
			#include "ParticleFieldCGLight.cginc"		
			
			uniform sampler2D _MainTex;			
			fixed4 _Color;			
			fixed _CutOff;
			
			fixed _Softness;
			
			float4 frag(v2fParticleField i) : COLOR
			{
#ifdef SOFTPARTICLES_ON
				i.color.a *= GetAlphaFromDepth(i.projPos, _Softness);
#endif

				i.color.rgb *= ForwardAddLight(i);

				fixed4 final = tex2D(_MainTex, i.tex.xy);
				clip ((final.a * i.color.a) - _CutOff);
				
				final *= i.color * _Color;
				//final.rgb *= final.a;

				UNITY_APPLY_FOG_COLOR(i.fogCoord, final, fixed4(0,0,0,0));
				
				return final;   
			}
		
			ENDCG
		}
	}
	
	Fallback "VertexLit"
}