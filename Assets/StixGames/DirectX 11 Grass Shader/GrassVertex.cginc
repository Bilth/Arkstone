// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

#ifndef GRASS_VERTEX
#define GRASS_VERTEX

appdata vert(appdata v)
{
	#ifdef GRASS_OBJECT_MODE
		v.objectSpacePos = v.vertex.xyz;
	#endif

	v.vertex = mul(unity_ObjectToWorld, v.vertex);
	v.uv = TRANSFORM_TEX(v.uv, _Density);

	//v.color doesn't have to be changed.

	#ifdef GRASS_FOLLOW_SURFACE_NORMAL
		v.normal = UnityObjectToWorldNormal(v.normal);
	#endif

	//Camera, or rather renderer pos
	v.cameraPos = getCameraPos();

	return v;
}
#endif