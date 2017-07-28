// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

float3 UnpackVector( float v ) {		
     return frac(v / float3(16777216, 65536, 256)) * 2 - 1;
}

void OrthoNormalize(inout float3 forward, inout float3 up, inout float3 right){
	forward = normalize(forward);
	right = normalize(cross(forward,up));
	up = cross(right,forward);
}

float3x3 LookMatrix(float3 forward, float3 up){	
	float3 right = float3(1,0,0);
	OrthoNormalize(forward, up, right);
	
	float3x3 rotMat = float3x3(right.x, right.y, right.z,
		up.x, up.y, up.z,
		-forward.x, -forward.y, -forward.z);

	return rotMat;
}

float3 PAGetMeshNormal(appdata_full v){
	return UnpackVector(v.texcoord1.x);
}

struct MeshParticleInput{
	float3 vertexOffset;
	float3 normal;
	float4 tangent;
	float2 texcoord;
	fixed4 color;
	float3 spinAxis;
	float spinSpeed;
};

MeshParticleInput GetMeshParticleInput(appdata_full v){
	MeshParticleInput o;
	o.vertexOffset = v.vertex.xyz;
	o.normal = PAGetMeshNormal(v);
	o.tangent = v.tangent;
	o.texcoord = v.texcoord;
	o.texcoord.x += _UOffset;
	o.color = v.color;
	o.spinSpeed = v.normal.g;
	o.spinAxis = UnpackVector(v.normal.z);
	return o;
}

ParticleInput GetParticleInputForMesh(appdata_full v){
	ParticleInput o;
	o.index = 0;
	o.position =  UnpackVector(v.texcoord1.y);
	o.direction = o.position;
	o.speed = v.normal.r;
	o.timeOffset = 0;
	return o;
}

void RasterizeMesh(MeshParticleInput mesh, ParticleOutput particle, inout appdata_full v){
	
#ifdef DIRECTIONAL_ON
	float3x3 rotMat = LookMatrix(particle.velocity, _UpDirection);	//LookMatrix normalized particle.velocity
	mesh.vertexOffset = mul(mesh.vertexOffset, rotMat);
	mesh.normal = mul(mesh.normal, rotMat);
	mesh.tangent.xyz = mul(mesh.tangent.xyz, rotMat);
#elif SPIN_ON
	float4 q = AxisAngleToQuaternion(mesh.spinAxis, mesh.spinSpeed + particle.time * _SpinSpeed * mesh.spinSpeed);//UnpackVector(v.normal.z)
	mesh.vertexOffset = mesh.vertexOffset + 2.0 * cross(q.xyz, cross(q.xyz, mesh.vertexOffset) + q.w * mesh.vertexOffset);
	mesh.normal = mesh.normal + 2.0 * cross(q.xyz, cross(q.xyz, mesh.normal) + q.w * mesh.normal);	
	mesh.tangent.xyz = mesh.tangent.xyz + 2.0 * cross(q.xyz, cross(q.xyz, mesh.tangent.xyz) + q.w * mesh.tangent.xyz);
#else
	float3x3 rotMat = LookMatrix(-_FaceDirection, _UpDirection);	
	mesh.vertexOffset = mul(mesh.vertexOffset, rotMat);
	mesh.normal = mul(mesh.normal, rotMat);
	mesh.tangent.xyz = mul(mesh.tangent.xyz, rotMat);
#endif

#if (!NO_EDGE_MODE)
	fixed scaleByAlpha = lerp(1, particle.visibility, _EdgeScale) * ceil(particle.visibility);
	mesh.vertexOffset *= scaleByAlpha;
	mesh.color.a *= lerp(1, particle.visibility, _EdgeAlpha);
#endif

	//Finalize values
	v.vertex.xyz = particle.position + mesh.vertexOffset * _ParticleSize.x;
#ifdef WORLDSPACE_ON
	v.normal = mul(unity_WorldToObject, float4(mesh.normal, 0));
#else
	v.normal = mesh.normal;
#endif
	v.tangent = mesh.tangent;
	v.texcoord.xy = mesh.texcoord;
	v.color = mesh.color;
}

float3 PAParticleMeshField(inout appdata_full v){
	ParticleInput pi = GetParticleInputForMesh(v);
	MeshParticleInput mesh = GetMeshParticleInput(v);
	ParticleOutput po = Simulate(pi);
	RasterizeMesh(mesh, po, /*out*/ v);
	return po.position;
}
