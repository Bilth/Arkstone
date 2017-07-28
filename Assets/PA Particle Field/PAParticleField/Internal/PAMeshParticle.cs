using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PAMeshParticle : PAParticleMeshGenerator {

    /* mesh particle data layout
     *   pivot/direction = v.uv1.y (packed)
     *   vertex position = v.vertex
     *   vertex normal = v.uv1.x (packed)
     *   uvs = v.uv0
     *   color = v.color
     *   move speed = v.normal.x
     *   spin speed = v.normal.y
     *   spin axis = v.normal.z (packed)
     *   particle index = ???;//floor(v.uv1.r)/_ParticleCount
     */

	const int MAX_VERT_COUNT = 65536;

	[HideInInspector]
	public Mesh inputMesh;

	public override int GetMaximumParticleCount (){
		if (inputMesh) {
			return (int)(MAX_VERT_COUNT / (float)inputMesh.vertexCount);
		} else {
			return 16250;
		}
	}

    public override float GetParticleBaseSize()
    {
        if (inputMesh)
        {
            return Mathf.Max(inputMesh.bounds.size.x, Mathf.Max(inputMesh.bounds.size.y, inputMesh.bounds.size.z));
        }
        else
        {
            return base.GetParticleBaseSize();
        }
    }

	public override void UpdateMesh (Mesh mesh, PAParticleField settings)
	{
		inputMesh = settings.inputMesh;
		if (inputMesh) {
			base.UpdateMesh (mesh, settings);
		}
	}

	protected override int SetParticleCapacity (int count)
	{
		count = GetClampedParticleCount (count);

		int startAt = count;

		//if we're increasing start at the current count
		if (count * inputMesh.vertexCount > verts.Length) {
			startAt = verts.Length / inputMesh.vertexCount;
		}

		int vertCapactity = count * inputMesh.vertexCount;
		int triCapacity = count * inputMesh.triangles.Length;
		SetArraySizes (vertCapactity, triCapacity);

		return startAt;
	}

	protected override void UpdateDirection (PAParticleField settings, int startAt)
	{
		int count = GetClampedParticleCount (settings.particleCount);

		SkipRandomCalls (6, startAt);

		for (int i = startAt; i < count; i++) {

#if UNITY_EDITOR
            //UnityEditor.EditorUtility.DisplayProgressBar("Building Mesh", "Updating directions", i / (float)count);
#endif	

			float randomDirection = Vector3ToFloat(new Vector3(GetRandomAndIncrement(-1f, 1f), GetRandomAndIncrement(-1f,1f), GetRandomAndIncrement(-1f,1f)));

			for (int j = 0; j < inputMesh.vertexCount; j++) {
				int vertIndex = i * inputMesh.vertexCount + j;	
				if (vertIndex >= normals.Length){
					Debug.Log("An error has occurred in PAMeshParticle", gameObject);
				}
				uv1[vertIndex].y = randomDirection;
			}
		}

#if UNITY_EDITOR
        UnityEditor.EditorUtility.ClearProgressBar();
#endif
	}

	protected override void UpdateColor (PAParticleField settings, int startAt)
	{
		int count = GetClampedParticleCount (settings.particleCount);

		SkipRandomCalls (1, startAt);

		for (int i = startAt; i < count; i++) {

#if UNITY_EDITOR
            //UnityEditor.EditorUtility.DisplayProgressBar("Building Mesh", "Updating colors", i / (float)count);
#endif	

			Color randomColor = settings.colorVariation.Evaluate(GetRandomAndIncrement(0f, 1f));

			for (int j = 0; j < inputMesh.vertexCount; j++) {

				int vertIndex = i * inputMesh.vertexCount + j;

				if (inputMesh.colors.Length>0){
					colors[vertIndex] = inputMesh.colors[j] * randomColor;
				}else{
					colors[vertIndex] = randomColor;
				}
			}
		}

#if UNITY_EDITOR
        UnityEditor.EditorUtility.ClearProgressBar();
#endif
	}

	protected override void UpdateSurface (PAParticleField settings, int startAt = 0)
	{
		int count = GetClampedParticleCount (settings.particleCount);//TODO this should be pre-clamped

        float columns = (settings.textureType != PAParticleField.TextureType.Simple ? settings.spriteColumns : 1f);
        float rows = (settings.textureType != PAParticleField.TextureType.Simple ? settings.spriteRows : 1f);
        Vector2 uv0Scale = new Vector2(1f / columns, 1f / rows);

		SkipRandomCalls (3, startAt);

		for (int i = startAt; i < count; i++) {

#if UNITY_EDITOR
            //UnityEditor.EditorUtility.DisplayProgressBar("Building Mesh", "Updating surfaces", i / (float)count);
#endif	

			float size = GetRandomAndIncrement(settings.minimumSize, 1f);

            Vector2 randomUVOffset = new Vector2((int)GetRandomAndIncrement(0f, columns), (int)GetRandomAndIncrement(0f, rows));

			for (int j = 0; j < inputMesh.vertexCount; j++) {
				int vertIndex = i * inputMesh.vertexCount + j;
				//fill positions
				verts [vertIndex] = inputMesh.vertices [j] * size;

				//encode normal into uv1.x
				if (inputMesh.normals.Length > 0){
					uv1[vertIndex].x = Vector3ToFloat(inputMesh.normals[j].normalized * 0.9f);//scale normal to prevent precision errors
				}

				//fill tangents
				if (inputMesh.tangents.Length > 0){
					tangents[vertIndex] = inputMesh.tangents[j];
				}

				//Fill UV1
				if (inputMesh.uv.Length > 0){
                    uv0[vertIndex] = Vector2.Scale(inputMesh.uv[j] + randomUVOffset, uv0Scale); 
				}
			}
		}

#if UNITY_EDITOR
        UnityEditor.EditorUtility.ClearProgressBar();
#endif
	}

	protected override void UpdateTriangles (int startAt)
	{
        int count = triangles.Length / inputMesh.triangles.Length;// GetClampedParticleCount(settings.particleCount);

		for (int i = startAt; i < count; i++) {
			for (int j = 0; j < inputMesh.triangles.Length; j++) {
				int triIndex = i * inputMesh.triangles.Length + j;
				triangles [triIndex] = inputMesh.triangles [j] + inputMesh.vertexCount * i;
			}
		}
	}

	protected override void UpdateSpeed (PAParticleField settings, int startAt)
	{
		int count = GetClampedParticleCount (settings.particleCount);
	
		SkipRandomCalls (2, startAt);

		for (int i = startAt; i < count; i++) {

#if UNITY_EDITOR
            //UnityEditor.EditorUtility.DisplayProgressBar("Building Mesh", "Updating speeds", i / (float)count);
#endif	

            Vector2 speedData = new Vector3(GetRandomAndIncrement(settings.minimumSpeed, 1f), GetRandomAndIncrement(settings.minSpinSpeed, 1f));

            Vector3 rotationAxis = settings.rotationAxis;
            if (!settings.customRotationAxis) {
                rotationAxis = new Vector3(GetRandomAndIncrement(-1f, 1f), GetRandomAndIncrement(-1f, 1f), GetRandomAndIncrement(-1f, 1f)).normalized;
            }

            float packedAxis = Vector3ToFloat(rotationAxis);

			for (int j = 0; j < inputMesh.vertexCount; j++) {
				int vertIndex = i * inputMesh.vertexCount + j;	

				normals[vertIndex].x = speedData.x;
                normals[vertIndex].y = speedData.y;
                normals[vertIndex].z = packedAxis;
			}
		}

#if UNITY_EDITOR
        UnityEditor.EditorUtility.ClearProgressBar();
#endif
	}

	public static float Vector3ToFloat( Vector3 c ) {
		c = (c + Vector3.one) * 0.5f;
		return Vector3.Dot(new Vector3(Mathf.Round(c.x * 255),Mathf.Round(c.y * 255),Mathf.Round(c.z * 255)), new Vector3(65536, 256, 1));
	}
}
