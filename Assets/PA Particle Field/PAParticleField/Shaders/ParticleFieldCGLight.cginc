#include "UnityCG.cginc"
#include "AutoLight.cginc"

uniform float4 _LightColor0;

float3 ForwardBaseLight(v2fParticleField i){
    return _LightColor0.rgb + UNITY_LIGHTMODEL_AMBIENT.rgb;
}

float3 ForwardAddLight(v2fParticleField i){
	return LIGHT_ATTENUATION(i) * _LightColor0.rgb;
}