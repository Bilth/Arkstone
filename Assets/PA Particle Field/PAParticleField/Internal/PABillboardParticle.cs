using UnityEngine;
using System.Collections;

public class PABillboardParticle : PAParticleMeshGenerator {

    /* Billboard particle data layout
     *   pivot/direction = v.vertex
     *   vertex position = v.uv1
     *   uvs = v.uv0
     *   color = v.color
     *   move speed = v.normal.r
     *   spin speed = v.normal.g
     *   particle index = v.normal.b
     */

	//Max particle count = 65000/4
	const int MAX_PARTICLE_COUNT = 16250;

	protected readonly static Vector2[] quadUVs = new Vector2[]{new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1), new Vector2(0, 0)};
    protected readonly static Vector2[] quadOffsets = new Vector2[] { new Vector2(-0.5f, -0.5f), new Vector2(-0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, -0.5f) };
	
	public override int GetMaximumParticleCount ()
	{
		return MAX_PARTICLE_COUNT;
	}

	protected override int SetParticleCapacity (int count)
	{
		count = GetClampedParticleCount (count);
		
		int startAt = count;
		
		//if we're increasing start at the current count
		if (count * 4 > verts.Length) {
			startAt = verts.Length / 4;
		}

		SetArraySizes (count * 4, count * 6);

		return startAt;
	}

	protected override void UpdateDirection (PAParticleField settings, int startAt)
	{
		SkipRandomCalls (3, startAt); //skippung

		for (int i = startAt; i < settings.particleCount; i++) {	

			Vector3 randomPosition = new Vector3 (GetRandomAndIncrement (-1f, 1f), GetRandomAndIncrement (-1f, 1f), GetRandomAndIncrement (-1f, 1f));

			for (int j = 0; j < 4; j++) {
				int vertIndex = i * 4 + j;
				verts [vertIndex] = randomPosition;
			}
		}
	}

	protected override void UpdateColor (PAParticleField settings, int startAt)
	{
		SkipRandomCalls (1, startAt);

		for (int i = startAt; i < settings.particleCount; i++) {	
			Color randomColor = settings.colorVariation.Evaluate (GetRandomAndIncrement (0f, 1f));
			for (int j = 0; j < 4; j++) {
				int vertIndex = i * 4 + j;				
				colors [vertIndex] = randomColor;
			}
		}
	}

	protected override void UpdateSpeed (PAParticleField settings, int startAt)
	{
		SkipRandomCalls (2, startAt);
		
		for (int i = startAt; i < settings.particleCount; i++) {

			Vector2 speedData = new Vector3(GetRandomAndIncrement(settings.minimumSpeed, 1f), GetRandomAndIncrement(settings.minSpinSpeed, 1f));

			for (int j = 0; j < 4; j++) {
				int vertIndex = i * 4 + j;
                normals[vertIndex].x = speedData.x;
                normals[vertIndex].y = speedData.y;
			}
		}
	}

	protected override void UpdateSurface (PAParticleField settings, int startAt)
	{
		SkipRandomCalls (3, startAt);

		float columns = (settings.textureType != PAParticleField.TextureType.Simple ? settings.spriteColumns : 1f);
		float rows = (settings.textureType != PAParticleField.TextureType.Simple ? settings.spriteRows : 1f);
		Vector2 uv0Scale = new Vector2(1f/columns, 1f/rows);

		for (int i = startAt; i < settings.particleCount; i++) {

            Vector2 randomUVOffset = new Vector2((int)GetRandomAndIncrement(0f, columns), (int)GetRandomAndIncrement(0f, rows));
            float randomScale = GetRandomAndIncrement(settings.minimumSize, 1f);

			for (int j = 0; j < 4; j++) {
				int vertIndex = i * 4 + j;
				uv0 [vertIndex] = Vector2.Scale (quadUVs [j] + randomUVOffset, uv0Scale);
				uv1 [vertIndex] = (quadOffsets[j] * randomScale) + settings.pivotOffset * randomScale;
			}
		}
	}

	protected override void UpdateTriangles (int startAt)
	{
		for (int i = startAt; i < triangles.Length/6; i++) {
			triangles [i * 6 + 0] = i * 4 + 2;
			triangles [i * 6 + 1] = i * 4 + 1;
			triangles [i * 6 + 2] = i * 4 + 0;
			triangles [i * 6 + 3] = i * 4 + 2;
			triangles [i * 6 + 4] = i * 4 + 0;
			triangles [i * 6 + 5] = i * 4 + 3;
		}
	}

    protected override void UpdateIndicies() {
        float count = normals.Length / 4;
        for (int i = 0; i < normals.Length; i+=4) {
            float normalizedIndex = (i / 4) / count;
            normals[i].z = normalizedIndex;
            normals[i+1].z = normalizedIndex;
            normals[i+2].z = normalizedIndex;
            normals[i+3].z = normalizedIndex;
        }
    }
}
