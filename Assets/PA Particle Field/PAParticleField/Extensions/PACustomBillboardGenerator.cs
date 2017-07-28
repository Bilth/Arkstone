using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PACustomBillboardGenerator : PABillboardParticle, PAICustomParticleGenerator {

    public List<PACustomParticle> particles;

    [HideInInspector]
    [SerializeField]
    private Mesh particleMesh;

    //uv array for manipulating UVs
    private Vector2[] particleUVs = new Vector2[4];

    protected override void UpdateCache(PAParticleField settings) {}

    protected override int SetParticleCapacity(int count) {
        //ignore the count sent by the field, use our particles array count instead
        return particles.Count;
    }

    void Awake()
    {
        PAParticleField papf = GetComponent<PAParticleField>();
        if (papf && papf.generatorType != PAParticleField.ParticleType.Custom)
        {
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

    [ContextMenu("Apply Particles")]
    public void ApplyParticles() {

        if (particles != null && particles.Count > 0) {

            int verticesPerParticle = 4; // billboards have 4 verts

            SetArraySizes(particles.Count * verticesPerParticle, particles.Count * 3 * 2); // 3 verts per triangle with 2 triangles in the billboard

            for (int particleIndex = 0; particleIndex < particles.Count; particleIndex++) {

                PACustomParticle particle = particles[particleIndex];

                //map the various particle settings to the vertex data

                SetOriginDirection(particleIndex, particle.originDirection);

                SetSize(particleIndex, particle.size);
                SetColor(particleIndex, particle.color);

                SetSpeed(particleIndex, particle.speed);
                SetSpinSpeed(particleIndex, particle.spinSpeed);

                SetUV(particleIndex, particle.uv);

                SetIndex(particleIndex, particleIndex / (float)particles.Count);
            }
        } else {
            SetArraySizes(0, 0);
        }

        UpdateTriangles(0);

        if (particleMesh == null) {
            particleMesh = GetComponent<MeshFilter>().sharedMesh;
        }

        FillMesh(particleMesh);
    }
    
    void SetOriginDirection(int particleIndex, Vector3 direction) {
        // particle origin/direction is mapped to the vertex position
        for (int i = 0; i < 4; i++) {
            int vertIndex = (particleIndex * 4) + i;
            verts[vertIndex] = direction;
        }
    }

    void SetSpeed(int particleIndex, float speed) {
        // speed is mapped to the vertex normal x, as a multiplier of the fields MoveSpeed parameter        
        for (int i = 0; i < 4; i++) {
            int vertIndex = (particleIndex * 4) + i;
            normals[vertIndex].x = speed;
        }
    }

    void SetSpinSpeed(int particleIndex, float spinSpeed) {
        // spin speed it mapped the vertex normal y, as a multiplier of the field SpinSpeed parameter        
        for (int i = 0; i < 4; i++) {
            int vertIndex = (particleIndex * 4) + i;
            normals[vertIndex].y = spinSpeed;
        }
    }

    void SetColor(int particleIndex, Color color) {
        // color is simply mapped to color
        for (int i = 0; i < 4; i++) {
            int vertIndex = (particleIndex * 4) + i;
            colors[vertIndex] = color;
        }
    }

    void SetUV(int particleIndex, Rect uv) {

        particleUVs[0] = new Vector2(uv.x + uv.width, uv.y);
        particleUVs[1] = new Vector2(uv.x + uv.width, uv.y + uv.height);
        particleUVs[2] = new Vector2(uv.x, uv.y + uv.height);
        particleUVs[3] = new Vector2(uv.x, uv.y);

        // uv is simply mapped to UV
        for (int i = 0; i < 4; i++) {
            int vertIndex = (particleIndex*4) + i;
            uv0[vertIndex] = particleUVs[i];
        }
    }

    void SetSize(int particleIndex, float size) {
        // size and per vertex offset from pivot is mapped to uv1, as a multiplier of the fields ParticleSize paramater
        for (int i = 0; i < 4; i++) {
            int vertIndex = (particleIndex * 4) + i;        
			uv1 [vertIndex] = quadOffsets[i] * size;
        }
    }

    void SetIndex(int particleIndex, float normalizedIndex) {
        // index is mapped to vertex normal z, and is expected to be normalized by the total particle count
        for (int i = 0; i < 4; i++) {
            int vertIndex = (particleIndex * 4) + i;
            normals[vertIndex].z = normalizedIndex;
        }
    }
}

public interface PAICustomParticleGenerator {
    void ApplyParticles();
}

[System.Serializable]
public class PACustomParticle {

    public Vector3 originDirection;
    public float size       = 1;
    public Color color      = Color.white;
    public float speed      = 0;
    public float spinSpeed  = 0;
    public Rect uv          = new Rect(0,0,1,1);

    public void SetDefaultValuesIfUninitialized()
    {
        if (originDirection == default(Vector3) &&
            size == default(float) &&
            color == default(Color) &&
            speed == default(float) &&
            spinSpeed == default(float) &&
            uv == default(Rect))
        {
            originDirection = Vector3.zero;
            size = 1;
            color = Color.white;
            speed = 0f;
            spinSpeed = 0f;
            uv = new Rect(0, 0, 1, 1);
        }
    }

    public PACustomParticle() {
        originDirection = new Vector3(0.5f, 0.5f, 0.5f);
        size       = 1;
        color      = Color.white;
        speed      = 0;
        spinSpeed  = 0;
        uv          = new Rect(0,0,1,1);
    }

    public PACustomParticle(Vector3 originDirection, Color color, float size, float speed, float spinSpeed, Rect uv) {
        this.originDirection = originDirection;
        this.size = size;
        this.speed = speed;
        this.spinSpeed = spinSpeed;
        this.color = color;
        this.uv = uv;
    }

    public PACustomParticle(Vector3 originDirection, Color color, float size = 1, float speed = 0, float spinSpeed = 0) {
        this.originDirection = originDirection;
        this.size = size;
        this.speed = speed;
        this.spinSpeed = spinSpeed;
        this.color = color;
        this.uv = new Rect(0, 0, 1, 1);
    }

    public PACustomParticle(Vector3 originDirection) {
        this.originDirection = originDirection;
        this.size = 1;
        this.speed = 0;
        this.spinSpeed = 0;
        this.color = Color.white;
        this.uv = new Rect(0, 0, 1, 1);
    }
}
