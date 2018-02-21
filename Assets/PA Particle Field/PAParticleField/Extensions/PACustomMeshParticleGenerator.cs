using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PACustomMeshParticleGenerator : PAMeshParticle, PAICustomParticleGenerator {

    public List<PACustomMeshParticle> particles;

    void Awake()
    {
        PAParticleField papf = GetComponent<PAParticleField>();
        if (papf && papf.generatorType != PAParticleField.ParticleType.Custom){
            papf.generatorType = PAParticleField.ParticleType.Custom;
        }
    }

    void OnValidate()
    {
        PAParticleField papf = GetComponent<PAParticleField>();
        if (papf && papf.generatorType != PAParticleField.ParticleType.Custom)
        {
            papf.generatorType = PAParticleField.ParticleType.Custom;
        }
        particles.ForEach(obj => obj.SetDefaultValuesIfUninitialized());
    }

    protected override void UpdateCache(PAParticleField settings) {
        //base.UpdateCache(settings);
    }

    protected override int SetParticleCapacity(int count) {
        return particles.Count;
    }

    [ContextMenu("Apply Particles")]
    public void ApplyParticles() {
        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;

        if (mesh != null && particles != null && particles.Count > 0) {

            List<Vector3> mVerts = new List<Vector3>();
            List<Vector3> mNorms = new List<Vector3>();
            List<Vector4> mTangs = new List<Vector4>();
            List<Vector2> mUV0s = new List<Vector2>();
            List<Vector2> mUV1s = new List<Vector2>();
            List<Color> mColors = new List<Color>();

            List<int> mTriangles = new List<int>();

            for (int particleIndex = 0; particleIndex < particles.Count; particleIndex++) {
                PACustomMeshParticle particle = particles[particleIndex];

                if (particle.mesh == null)
                {
                    continue;
                }

                for (int i = 0; i < particle.mesh.triangles.Length; i++) {
                    mTriangles.Add(particle.mesh.triangles[i] + mVerts.Count);
                }

                for (int i = 0; i < particle.mesh.vertexCount; i++) {
                    mVerts.Add(particle.mesh.vertices[i] * particle.size); //per vertex pivot offset mapped to vertex, premultiplied by per particle size

                    if (particle.mesh.tangents != null && particle.mesh.tangents.Length > 0) {
                        mTangs.Add(particle.mesh.tangents[i]); //per vertex tangents are mapped to tangents
                    } else {
                        mTangs.Add(new Vector4(0, 1, 0, 1));
                    }
                    
                    Vector2 uv = Vector2.Scale(particle.mesh.uv[i], particle.uv.size) + particle.uv.position;
                    mUV0s.Add(uv); //per vertex UV is mapped into uv

                    Vector2 uv1PackedData = Vector2.zero;
                    uv1PackedData.x = PAMeshParticle.Vector3ToFloat(particle.mesh.normals[i]); // per vertex normal is packed into uv1.x
                    uv1PackedData.y = PAMeshParticle.Vector3ToFloat(particle.originDirection); //per particle direction is packed into uv1.y
                    mUV1s.Add(uv1PackedData);

                    Color baseColor = Color.white;
                    if (particle.mesh.colors != null && particle.mesh.colors.Length > 0) {
                        baseColor = particle.mesh.colors[i];
                    }
                    mColors.Add(baseColor * particle.color); //per particle/vertex color is mapped into color

                    Vector3 normalPackedData = Vector3.zero;
                    normalPackedData.x = particle.speed; //per particle speed is mapped to normal.x
                    normalPackedData.y = particle.spinSpeed; //per particle spin speed is mapped to normal.y
                    normalPackedData.z = PAMeshParticle.Vector3ToFloat(particle.spinAxis); //per particle spin axis is packed into normal.z
                    mNorms.Add(normalPackedData);
                }
            }

            
            verts = mVerts.ToArray();
            normals = mNorms.ToArray();
            tangents = mTangs.ToArray();
            uv0 = mUV0s.ToArray();
            uv1 = mUV1s.ToArray();
            colors = mColors.ToArray();
            triangles = mTriangles.ToArray();

            FillMesh(mesh);
        }
    }
}

[System.Serializable]
public class PACustomMeshParticle : PACustomParticle {

    public Vector3 spinAxis;
    public Mesh mesh;

}
