// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

//// PAPF Static Branches

#ifndef TURBULENCE_ANY
#if defined(TURBULENCE_SIMPLEX2D) || defined(TURBULENCE_SIMPLEX) || defined(TURBULENCE_SIN)
#define TURBULENCE_ANY
#endif
#endif

#ifndef LOW_INSTRUCTION_COUNT
#if defined(SHADER_TARGET) && SHADER_TARGET < 30
#define LOW_INSTRUCTION_COUNT
#endif
#endif

#ifndef LOW_INSTRUCTION_COUNT
#if defined(TURBULENCE_ANY) && defined(SHADER_API_MOBILE) && defined(LIGHTING_ON)
#define LOW_INSTRUCTION_COUNT
#endif
#endif

#ifndef LOW_INSTRUCTION_COUNT
#if defined(TURBULENCE_ANY) && defined(SHADER_API_MOBILE) && defined(DIRECTIONAL_ON)
#define LOW_INSTRUCTION_COUNT
#endif
#endif

#ifndef LOW_INSTRUCTION_COUNT
#if defined(TURBULENCE_ANY) && defined(SHADER_API_MOBILE) && defined(SPIN_ON)
#define LOW_INSTRUCTION_COUNT
#endif
#endif

//// Includes

#include "UnityCG.cginc"
#include "AutoLight.cginc"

//// Uniform Parameters

float _Zero;

//Floats as Bools
fixed _Editor;
fixed _UserFacing;
fixed _EdgeAlpha;
fixed _EdgeScale;

//Particle params
float2 _ParticleSize;
float3 _FieldSize;
float3 _Speed;
float _SpinSpeed;
float3 _Force;
float _SpeedScale;
float _UOffset;
fixed3 _EdgeThreshold;
fixed3 _InverseEdgeThreshold;
float3 _FaceDirection;
float3 _UpDirection;

float _ParticleCount;
fixed _CountMask;

float _TotalTime;
float3 _DeltaSpeed;
float3 _DeltaForce;
float3 _DeltaPosition;

//Soft particle params
float _NearFadeDistance;
float _NearFadeOffset;

//Exclusion Zones
#ifdef EXCLUSION_ON
float4x4 _ExclusionMatrix0;
float4x4 _ExclusionMatrix1;
float4x4 _ExclusionMatrix2;
fixed3 _ExclusionThreshold0;
fixed3 _ExclusionThreshold1;
fixed3 _ExclusionThreshold2;
fixed3 _InverseExclusionThreshold0;
fixed3 _InverseExclusionThreshold1;
fixed3 _InverseExclusionThreshold2;
#endif

//Turbulence
float3 _TurbulenceOffset;
float _TurbulenceFrequency;
float _TurbulenceAmplitude;
float3 _TurbulenceDeltaOffset;
float3 _TurbulenceScale;

////////////////// START SIMPLEX NOISE SECTION /////////////////////
// Simplex noise was adapted from Ian McEwan and Ashima Arts 
// webgl-noise https://github.com/ashima/webgl-noise
// This section of software (just this section) is licenced under the MIT 
// licence, therefore the licence is included here. (I would have kept this
// in a seperate file to prevent confusion but I had to incorporate it in
// order to better support relative paths)
// 
// Copyright (C) 2011 by Ashima Arts (Simplex noise)
// Copyright (C) 2011-2016 by Stefan Gustavson (Classic noise and others)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.

float3 mod(float3 x, float3 y){return modf(x, y);}
float2 mod289(float2 x) {return x - floor(x / 289.0) * 289.0;}
float3 mod289(float3 x) {return x - floor(x * (1.0 / 289.0)) * 289.0;}
float4 mod289(float4 x) {return x - floor(x * (1.0 / 289.0)) * 289.0;}
float3 permute(float3 x) {return mod289((x * 34.0 + 1.0) * x);}
float4 permute(float4 x) {return mod289(((x*34.0)+1.0)*x);}
float3 taylorInvSqrt(float3 r){return 1.79284291400159 - 0.85373472095314 * r;}
float4 taylorInvSqrt(float4 r){return 1.79284291400159 - 0.85373472095314 * r;}

float3 snoised3D(float3 v)
{ 
	const float2  C = float2(1.0/6.0, 1.0/3.0) ;
	const float4  D = float4(0.0, 0.5, 1.0, 2.0);

	// First corner
	float3 i  = floor(v + dot(v, C.yyy) );
	float3 x0 =   v - i + dot(i, C.xxx) ;

	// Other corners
	float3 g = step(x0.yzx, x0.xyz);
	float3 l = 1.0 - g;
	float3 i1 = min( g.xyz, l.zxy );
	float3 i2 = max( g.xyz, l.zxy );

	float3 x1 = x0 - i1 + C.xxx;
	float3 x2 = x0 - i2 + C.yyy; // 2.0*C.x = 1/3 = C.y
	float3 x3 = x0 - D.yyy;      // -1.0+3.0*C.x = -0.5 = -D.y

	// Permutations
	i = mod289(i); 
	float4 p = permute( permute( permute( 
             i.z + float4(0.0, i1.z, i2.z, 1.0 ))
           + i.y + float4(0.0, i1.y, i2.y, 1.0 )) 
           + i.x + float4(0.0, i1.x, i2.x, 1.0 ));

	// Gradients: 7x7 points over a square, mapped onto an octahedron.
	// The ring size 17*17 = 289 is close to a multiple of 49 (49*6 = 294)
	float n_ = 0.142857142857; // 1.0/7.0
	float3  ns = n_ * D.wyz - D.xzx;

	float4 j = p - 49.0 * floor(p * ns.z * ns.z);  //  mod(p,7*7)

	float4 x_ = floor(j * ns.z);
	float4 y_ = floor(j - 7.0 * x_ );    // mod(j,N)

	float4 x = x_ *ns.x + ns.yyyy;
	float4 y = y_ *ns.x + ns.yyyy;
	float4 h = 1.0 - abs(x) - abs(y);

	float4 b0 = float4( x.xy, y.xy );
	float4 b1 = float4( x.zw, y.zw );

	float4 s0 = floor(b0)*2.0 + 1.0;
	float4 s1 = floor(b1)*2.0 + 1.0;
	float4 sh = -step(h, 0.0);

	float4 a0 = b0.xzyw + s0.xzyw*sh.xxyy ;
	float4 a1 = b1.xzyw + s1.xzyw*sh.zzww ;

	float3 p0 = float3(a0.xy,h.x);
	float3 p1 = float3(a0.zw,h.y);
	float3 p2 = float3(a1.xy,h.z);
	float3 p3 = float3(a1.zw,h.w);

	//Normalise gradients
	float4 norm = taylorInvSqrt(float4(dot(p0,p0), dot(p1,p1), dot(p2, p2), dot(p3,p3)));
	p0 *= norm.x;
	p1 *= norm.y;
	p2 *= norm.z;
	p3 *= norm.w;

	// Mix final noise value
	float4 m = max(0.6 - float4(dot(x0,x0), dot(x1,x1), dot(x2,x2), dot(x3,x3)), 0.0);
	float4 m2 = m * m;
    float4 m3 = m2 * m;
    float4 m4 = m2 * m2;
    float3 grad =
      -6.0 * m3.x * x0 * dot(x0, p0) + m4.x * p0 +
      -6.0 * m3.y * x1 * dot(x1, p1) + m4.y * p1 +
      -6.0 * m3.z * x2 * dot(x2, p2) + m4.z * p2 +
      -6.0 * m3.w * x3 * dot(x3, p3) + m4.w * p3;
    return 42.0 * grad;
}

float3 snoised2D(float3 v)
{
	float d = dot(v, float3(0,0,0.5));
	v.xy += d;
    
	const float4 C = float4( 0.211324865405187,  // (3.0-sqrt(3.0))/6.0
                             0.366025403784439,  // 0.5*(sqrt(3.0)-1.0)
                            -0.577350269189626,  // -1.0 + 2.0 * C.x
                             0.024390243902439); // 1.0 / 41.0
    // First corner
    float2 i  = floor(v.xy + dot(v.xy, C.yy));
    float2 x0 = v -   i + dot(i, C.xx);

    // Other corners
    float2 i1;
    i1.x = step(x0.y, x0.x);
    i1.y = 1.0 - i1.x;

    // x1 = x0 - i1  + 1.0 * C.xx;
    // x2 = x0 - 1.0 + 2.0 * C.xx;
    float2 x1 = x0 + C.xx - i1;
    float2 x2 = x0 + C.zz;

    // Permutations
    i = mod289(i); // Avoid truncation effects in permutation
    float3 p = permute(permute(i.y + float3(0.0, i1.y, 1.0)) + i.x + float3(0.0, i1.x, 1.0));

    float3 m = max(0.5 - float3(dot(x0, x0), dot(x1, x1), dot(x2, x2)), 0.0);
    float3 m2 = m * m;
    float3 m3 = m2 * m;
    float3 m4 = m2 * m2;

    // Gradients: 41 points uniformly over a line, mapped onto a diamond.
    // The ring size 17*17 = 289 is close to a multiple of 41 (41*7 = 287)

    float3 x = 2.0 * frac(p * C.www) - 1.0;
    float3 h = abs(x) - 0.5;
    float3 ox = floor(x + 0.5);
    float3 a0 = x - ox;

    // Normalise gradients
    float3 norm = taylorInvSqrt(a0 * a0 + h * h);
    float2 g0 = float2(a0.x, h.x) * norm.x;
    float2 g1 = float2(a0.y, h.y) * norm.y;
    float2 g2 = float2(a0.z, h.z) * norm.z;

    // Compute gradient of noise function at P
    float2 grad =
      -6.0 * m3.x * x0 * dot(x0, g0) + m4.x * g0 +
      -6.0 * m3.y * x1 * dot(x1, g1) + m4.y * g1 +
      -6.0 * m3.z * x2 * dot(x2, g2) + m4.z * g2;
    grad *= 130;

	return float3(grad, sin((grad.x + grad.y) * 0.5)*2);
}
////////////////// END SIMPLEX NOISE SECTION /////////////////////

float4 AxisAngleToQuaternion(float3 axis, float angle)
{ 
	float4 qr;
	qr.xyz = axis * sin(angle);
	qr.w = cos(angle);
	return qr;
}

float3 Noise(float3 input){

#ifdef TURBULENCE_ANY
	#if defined(LOW_INSTRUCTION_COUNT) || defined(TURBULENCE_SIN)
		//on low instruction counts fallback to some Sin based pseudo noise
		return sin((input.yzx * float3(12, 19, 7) + input.zxy)+float3(1.23,4.56,7.89))*2.5;
	#endif
	#if defined(TURBULENCE_SIMPLEX2D) || defined(SHADER_API_MOBILE)
		return snoised2D(input);
	#else	
		return snoised3D(input);
	#endif
#endif
	return 0;
}

float3 ApplyTurbulence(float3 input, float3 deltaInput, inout float3 position, inout float3 speed)
{
	float3 positionOffset = (Noise(input * _TurbulenceFrequency) * _TurbulenceScale);
	position += positionOffset;

#if DIRECTIONAL_ON
	float3 deltaOffset = (Noise(deltaInput * _TurbulenceFrequency) * _TurbulenceScale);
	speed += positionOffset- deltaOffset;
#endif

	return positionOffset;
}

float GetExclusionVisibility(float3 pos){

#ifdef EXCLUSION_ON
	float finalEAlpha = 1;

	float4 worldPos = float4(pos, 1);
	
	float3 exclusionPos = abs(mul(_ExclusionMatrix0, worldPos));
	float ealpha = 1;
	exclusionPos -= _ExclusionThreshold0;
	exclusionPos = 1-(exclusionPos * _InverseExclusionThreshold0);
	ealpha = min(exclusionPos.x, min(exclusionPos.y, exclusionPos.z));
	
	finalEAlpha = saturate(ealpha);

	exclusionPos = abs(mul(_ExclusionMatrix1, worldPos));
	ealpha = 1;	
	exclusionPos -= _ExclusionThreshold1;
	exclusionPos = 1-(exclusionPos * _InverseExclusionThreshold1);
	ealpha = min(exclusionPos.x, min(exclusionPos.y, exclusionPos.z));
	
	finalEAlpha = max(saturate(ealpha), finalEAlpha);
	
	exclusionPos = abs(mul(_ExclusionMatrix2, worldPos));
	ealpha = 1;
	exclusionPos -= _ExclusionThreshold2;
	exclusionPos = 1-(exclusionPos * _InverseExclusionThreshold2);
	ealpha = min(exclusionPos.x, min(exclusionPos.y, exclusionPos.z));
	
	return 1 - max(saturate(ealpha), finalEAlpha);
#else
	return 1;
#endif
}

// returns a value between 0 and 1 indicating the positions value in the edge threshold
float GetEdgeVisibility(float3 localParticlePosition){

	//Set the vertex alpha based on distance from the center
	float3 absPos = abs(localParticlePosition)*2;
	float edgeVisibility = 1;
	
#ifdef SHAPE_SPHERE
	edgeVisibility = 1-(length(absPos) - _EdgeThreshold.x) * _InverseEdgeThreshold.x;
#elif SHAPE_CYLINDER
	edgeVisibility = 1-(length(absPos.xz) - _EdgeThreshold.x) * _InverseEdgeThreshold.x;
	edgeVisibility = min(1 - (absPos.y - _EdgeThreshold.y) * _InverseEdgeThreshold.y, edgeVisibility);
#else
	absPos = (1 - (absPos - _EdgeThreshold) * _InverseEdgeThreshold);
	edgeVisibility = min(absPos.x, min(absPos.y, absPos.z));
#endif

	return saturate(edgeVisibility);
}

struct v2fParticleField {
	float4 pos : SV_POSITION;
	fixed4 color : COLOR;
	float4 worldPos : TEXCOORD0;
	float2 tex : TEXCOORD1;

	UNITY_FOG_COORDS(2)	
	
	#ifdef LIGHTING_ON
	LIGHTING_COORDS(3, 4)
	#endif

	#ifdef SOFTPARTICLES_ON
	float4 projPos : TEXCOORD5;
	#endif	
};

struct ParticleInput{
	float index;
	float3 position;
	float3 direction;
	float speed;
	float timeOffset;
};

struct ParticleOutput{
	float3 position;
	float3 velocity;
	float visibility;
	float time;
	float linearDistanceTravelled;
};

struct BillboardParticleInput{
	float2 vertexOffset;
	float2 texcoord;
	fixed4 color;
	float spinSpeed;
};

ParticleInput GetParticleInputForBillboard(appdata_full v){
	ParticleInput o;
	o.index = v.normal.z;
	o.position = -v.vertex.xyz;
	o.direction = -v.vertex.xyz;
	o.speed = v.normal.r;
	o.timeOffset = 0;
	return o;
}

BillboardParticleInput GetBillboardParticleInput(appdata_full v){

	BillboardParticleInput o;
	o.vertexOffset = float2(v.texcoord1.x,v.texcoord1.y);
	o.texcoord = float2(v.texcoord.x + _UOffset, v.texcoord.y);
	o.color = v.color;
	o.spinSpeed = v.normal.g;
	return o;
}

ParticleOutput Simulate(ParticleInput input){

	ParticleOutput o;

	float simulationTimeDelta = lerp(1, _Time.y, _Editor);
	float totalTime = lerp(_TotalTime, _Time.y, _Editor) + input.timeOffset;

	//Get the position of the field
	float3 fieldPosition = mul (unity_ObjectToWorld, float4(0, 0, 0, 1)).xyz;
	//Get the position of the particle
	float3 particlePosition = input.position;
	
#if (!NO_LINEAR_MOVEMENT)
	//get the particles speed in units per second
	float3 linearVelocityInput = normalize(input.direction) * input.speed;
    float3 linearVelocityInputAndForce = (linearVelocityInput * _DeltaSpeed) + _DeltaForce;
    
	//get the vector the particle has moved in by multiplying by time delta (which is 1 at runtime because the speed values are premultiplied)
	float3 particlePositionOffset = ((linearVelocityInput * _Speed.xyz) + _Force) * simulationTimeDelta + _DeltaSpeed * input.timeOffset * + _DeltaForce * input.timeOffset;
	//add to the position
	particlePosition += particlePositionOffset;
#else
	float3 linearVelocityInput = 0;
	float3 linearVelocityInputAndForce = 0;
#endif

	o.linearDistanceTravelled = length(particlePosition);

#ifdef WORLDSPACE_ON
	// in world space, offset the position by the field position
	float3 adjPos = (fieldPosition/ _FieldSize.xyz);
	particlePosition += adjPos;	
#else
	// in local space add a constant precalculated offset for the "Local with Deltas" type (the offset is always zero in "Local Space")
	particlePosition += _DeltaPosition;
#endif

	//Wrap the vertex position in the bounds
	particlePosition = (0.5-frac(particlePosition)); 

#if (!NO_EDGE_MODE)
	//Get how close the particle is to the edge, for alpha/scaling effects
	float visibility = GetEdgeVisibility(particlePosition);
#else
	float visibility = 1;
#endif
	//check if the particle is masked
	visibility *= floor(input.index + _CountMask);

	//Scale the position from field space to local/worldSpace
	particlePosition = particlePosition * _FieldSize.xyz;
	
#ifdef WORLDSPACE_ON
	//Center the field on the objects pivot
	particlePosition += fieldPosition;
#endif

	//calculate inputs for noise based on the particles position and approximately where the particle would be on the next frame, ignoring force and turbulence
	float3 turbulenceOffset = (_TurbulenceOffset * simulationTimeDelta) + _TurbulenceDeltaOffset * input.timeOffset;
	float3 noiseInput = particlePosition - turbulenceOffset;
	float3 deltaNoiseInput = particlePosition - linearVelocityInputAndForce - (turbulenceOffset + _TurbulenceDeltaOffset/5);
	
	//update the particles position and velocity with turbulence
	ApplyTurbulence(noiseInput, deltaNoiseInput, /*out*/ particlePosition, /*out*/ linearVelocityInputAndForce);

#ifdef EXCLUSION_ON	
	visibility *= GetExclusionVisibility(particlePosition);
#endif

	//Set the final particle output
	o.position = particlePosition;
	o.velocity = linearVelocityInputAndForce;
	o.visibility = visibility + _Zero; // Adding uniform value fixed dx9 ceil bug
	o.time = simulationTimeDelta;

	return o;
}

void RasterizeBillboard(BillboardParticleInput billboard, ParticleOutput particle, inout appdata_full v){

	// Get particle facing direction
#ifdef WORLDSPACE_ON
	float3 facing = (_WorldSpaceCameraPos - particle.position);
#else
	float3 camPos = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos, 1));
	float3 facing = (camPos - particle.position);
#endif
	facing = lerp(facing, _FaceDirection, _UserFacing);

	//Get particle direction (velocity scaled particle up)
	float3 direction = _UpDirection;
#ifdef DIRECTIONAL_ON
	direction = particle.velocity; //TODO the direction here may be to be scaled or could be prescaled?
#elif SPIN_ON
	float4 q = AxisAngleToQuaternion(normalize(facing), particle.time * _SpinSpeed * billboard.spinSpeed); //TODO this doesnt include time offset
	//direction = normalize(direction);
	direction = normalize(direction + 2.0 * cross(q.xyz, cross(q.xyz, direction) + q.w * direction));
#endif

	float3 up = direction;
	float3 right = normalize(cross(up, facing));
	up = normalize(cross(facing, right));
	up *= max(length(direction) * _SpeedScale, 1);
	
	//float minSize = min(_ParticleSize.x, _ParticleSize.y);
	//float rightSize = minSize/_ParticleSize.x;
	//float upSize = minSize/_ParticleSize.y; 

	float3 vertexOffset = right * _ParticleSize.x * billboard.vertexOffset.x + up * _ParticleSize.y * billboard.vertexOffset.y;

	
#if (!NO_EDGE_MODE)
	//Scale by the alpha if _EdgeScale is enabled, scale particles to zero that are not visible
	fixed scaleByAlpha = lerp(1, particle.visibility, _EdgeScale) * ceil(particle.visibility);
	vertexOffset *= scaleByAlpha;
	
	//Apply alpha if _EdgeAlpha is 1
	billboard.color.a *= particle.visibility * _EdgeAlpha + (1 - _EdgeAlpha);
#endif

	//Finalize values
	v.vertex.xyz = particle.position + vertexOffset;
	v.normal.xyz = facing;
	v.tangent.xyz = direction;
	v.tangent.w = 1;
	v.texcoord.x = billboard.texcoord;
	v.color = billboard.color;
	
	//Fade the particle if it is too close to the camera
#if (!NO_NEAR_CLIP)
	//get the distance to the camera
	#if WORLDSPACE_ON
	float distanceToCamera = length(_WorldSpaceCameraPos - particle.position);
	#else	
	float distanceToCamera = length(camPos - particle.position);
	#endif
	//subract the near clip plane to get the distance to the plane
	distanceToCamera -= _ProjectionParams.y;
	v.color.a *= saturate((distanceToCamera-_NearFadeOffset)/_NearFadeDistance);
#endif
}

float3 PAParticleField(inout appdata_full v){
	ParticleInput pi = GetParticleInputForBillboard(v);
	BillboardParticleInput bi = GetBillboardParticleInput(v);
	ParticleOutput po = Simulate(pi);
	RasterizeBillboard(bi, po, /*out*/ v);
	return po.position;
}

float4 PAPositionVertex(float3 position){
#ifdef WORLDSPACE_ON
	//Position the vertex in view
	return mul(UNITY_MATRIX_VP, float4(position, 1));
#else
	//Position the vertex in view
	return UnityObjectToClipPos(float4(position, 1));
#endif
}

float4 PAPositionVertexSurf(float3 position){
#ifdef WORLDSPACE_ON
	return mul(unity_WorldToObject, float4(position, 1));
#else
	return float4(position, 1);
#endif
}

v2fParticleField vertParticleFieldCube(appdata_full v) 
{
	v2fParticleField o;	
	UNITY_INITIALIZE_OUTPUT(v2fParticleField, o);
	
	PAParticleField(v);

	o.color = v.color;
	o.worldPos = float4(v.vertex.xyz, 1);
	o.tex = v.texcoord;

#ifndef WORLDSPACE_ON
#ifdef LIGHTING_ON
	TRANSFER_VERTEX_TO_FRAGMENT(o);
#endif
#endif
	v.vertex = mul(unity_WorldToObject, o.worldPos);
	
#ifdef WORLDSPACE_ON
#ifdef LIGHTING_ON
	TRANSFER_VERTEX_TO_FRAGMENT(o);
#endif
#endif
	o.pos = PAPositionVertex(o.worldPos);
	
#ifdef SOFTPARTICLES_ON
	o.projPos = ComputeScreenPos (o.pos);
#if UNITY_VERSION >= 550
	COMPUTE_EYEDEPTH(o.projPos.z);
#endif
#endif	

	UNITY_TRANSFER_FOG(o, o.pos);

	return o;
}

#ifdef SOFTPARTICLES_ON
sampler2D _CameraDepthTexture;

float GetAlphaFromDepth(float4 projPos, fixed softness) {
#if UNITY_VERSION >= 550
	float sceneZ = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(projPos)));
#else
	float sceneZ = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(projPos))));
#endif
	float partZ = projPos.z;
	return saturate(softness * (sceneZ - partZ));
}
#endif