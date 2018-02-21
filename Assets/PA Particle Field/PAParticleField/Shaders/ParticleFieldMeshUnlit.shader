// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "PA/ParticleField/MeshUnlit"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags {"Queue"="Transparent" "RenderType"="Transparent" }
		LOD 200
		
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite On

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#pragma target 3.0
			
			#include "UnityCG.cginc"
			#include "ParticleField.cginc"
			#include "ParticleMeshField.cginc"

			#pragma shader_feature _ DIRECTIONAL_ON SPIN_ON
			#pragma shader_feature _ WORLDSPACE_ON
			#pragma shader_feature _ SHAPE_SPHERE SHAPE_CYLINDER
			#pragma multi_compile _ EXCLUSION_ON
			#pragma shader_feature _ TURBULENCE_SIMPLEX2D TURBULENCE_SIMPLEX	

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _Color;
			
			v2fParticleField vert(appdata_full v) 
			{
				v2fParticleField o;	
				UNITY_INITIALIZE_OUTPUT(v2fParticleField, o);
	
				PAParticleMeshField(v);
				o.color = v.color;
				o.worldPos = float4(v.vertex.xyz, 1);
				o.tex = v.texcoord;

				v.vertex = mul(unity_WorldToObject, o.worldPos);

				o.pos = PAPositionVertex(o.worldPos);

				UNITY_TRANSFER_FOG(o, o.pos);

				return o;
			}
			
			fixed4 frag (v2fParticleField i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.tex) * i.color * _Color;
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
