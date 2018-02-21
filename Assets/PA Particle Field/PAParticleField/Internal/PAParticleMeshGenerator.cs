using UnityEngine;
using System.Collections.Generic;

public class PAParticleMeshGenerator : MonoBehaviour
#if UNITY_4_6 || UNITY_5
	, ISerializationCallbackReceiver
#endif
{
	/// <summary>
	/// Some updates may cause the vertex data layout to change, if so the mesh will need regenerating
	/// </summary>
	const int DATA_STRUCTURE_VERSION = 2;

	[HideInInspector]
	[SerializeField]
	protected Vector3[] verts;
	[HideInInspector]
	[SerializeField]
	protected Vector3[] normals;
	[HideInInspector]
	[SerializeField]
	protected Vector4[] tangents;
	[HideInInspector]
	[SerializeField]
	protected Vector2[] uv0;
	[HideInInspector]
	[SerializeField]
	protected Vector2[] uv1;
	[HideInInspector]
	[SerializeField]
	protected Color[] colors;
	[HideInInspector]
	[SerializeField]
	protected int[] triangles;

	[SerializeField]
	[HideInInspector]
	int dataStructureVersion = 0;

#region ISerializationCallbackReceiver implementation

	public void OnBeforeSerialize ()
	{
		PAParticleField papf = GetComponent<PAParticleField> ();
		if (papf && papf.clearCacheInBuilds) {
			ClearCache();
		}
	}

	public void OnAfterDeserialize ()
	{
		return;
	}

#endregion
	
	protected void CacheSeed(){
        RandomWrapper.CacheState();
	}
	
	void SetSeed(int seed)
    {
        RandomWrapper.SetState(seed);
	}
	
	protected float GetRandomAndIncrement(float min, float max){
		float result = Random.Range (min, max);
		RandomWrapper.SetState(RandomWrapper.seed + 1);
		return result;
	}
	
	protected void ResetSeed(){
		RandomWrapper.RestoreState();
	}
	
	/// <summary>
	/// Gets the maximum particle count.
	/// </summary>
	/// <returns>The maximum particle count.</returns>
	public virtual int GetMaximumParticleCount(){return 16250;}

    /// <summary>
    /// returns the maximum distance in any direction of a particle whose size is 1 unit
    /// </summary>
    /// <returns></returns>
    public virtual float GetParticleBaseSize() { return 1f; }
	
	protected void SkipRandomCalls(int callsPerParticle, int count){
		for (int i = 0; i < callsPerParticle * count; i++) {
			GetRandomAndIncrement(0f, 0f);
		}
	}
	
	/// <summary>
	/// Populates the mesh.
	/// </summary>
	/// <param name="mesh">Mesh.</param>
	/// <param name="settings">Settings.</param>
	public virtual void UpdateMesh(Mesh mesh, PAParticleField settings){
		UpdateCache (settings);
		FillMesh (mesh);
	}

	protected virtual void UpdateCache(PAParticleField settings){
		CacheSeed ();
		
		int count = GetClampedParticleCount (settings.particleCount);
		int startAt = -1;

		//If PAPF has been updated the data layout may have changed, if so regenerate from scratch
		if (verts == null || verts.Length == 0 || dataStructureVersion != DATA_STRUCTURE_VERSION) {
			verts = new Vector3[0];
			normals = new Vector3[0];
			tangents = new Vector4[0];
			uv0 = new Vector2[0];
			uv1 = new Vector2[0];
			colors = new Color[0];
			triangles = new int[0];
			
			startAt = 0;
		}
		
		if ((settings.meshIsDirtyMask & MeshFlags.Count)!=0 || startAt == 0) {
			startAt = SetParticleCapacity (count);
			
			if (startAt == count){
				return;
			}else{
                UpdateIndicies();
				UpdateTriangles(startAt);
			}
		}
		
		if ((settings.meshIsDirtyMask & MeshFlags.Seed) != 0 || startAt != -1) {
			SetSeed (settings.seed);
			UpdateDirection (settings, Mathf.Max(0,startAt));
		}
		
		if ((settings.meshIsDirtyMask & MeshFlags.Surface) != 0 || startAt != -1) {
			SetSeed (settings.seed);
			UpdateSurface (settings, Mathf.Max(0,startAt));
		}
		
		if ((settings.meshIsDirtyMask & MeshFlags.Speed) != 0 || startAt != -1) {
			SetSeed (settings.seed);
			UpdateSpeed (settings, Mathf.Max(0,startAt));
		}
		
		if ((settings.meshIsDirtyMask & MeshFlags.Color) != 0 || startAt != -1) {
			SetSeed (settings.seed);
			UpdateColor (settings, Mathf.Max(0,startAt));
		}
		
		ResetSeed ();
		
		dataStructureVersion = DATA_STRUCTURE_VERSION;
	}

	protected void FillMesh(Mesh mesh){		
		mesh.Clear ();
		
		mesh.vertices = verts;
		mesh.normals = normals;
		mesh.tangents = tangents;
		mesh.uv = uv0;
		mesh.uv2 = uv1;
		mesh.colors = colors;
		mesh.triangles = triangles;
	}

	public int GetClampedParticleCount(int count){
		int max = GetMaximumParticleCount();
		if (count > max) {
			return max;
		} else {
			return count;
		}
	}
	
	/// <summary>
	/// Sets the particle count.
	/// </summary>
	/// <returns>The particle index to regenerate from</returns>
	/// <param name="count">Count.</param>
	protected virtual int SetParticleCapacity(int count){
		return -1;
	}

	protected void SetArraySizes(int vertCount, int triCount){
		Vector3[] verts = new Vector3[vertCount];
		Vector3[] normals = new Vector3[vertCount];
		Vector4[] tangents = new Vector4[vertCount];
		Vector2[] uv0 = new Vector2[vertCount];
		Vector2[] uv1 = new Vector2[vertCount];
		Color[] colors = new Color[vertCount];
		int[] triangles = new int[triCount];

		int copyLength = Mathf.Min (this.verts.Length, vertCount);

		for (int i = 0; i < copyLength; i++) {
			verts[i] = this.verts[i];
			normals[i]=this.normals[i];
			tangents[i]=this.tangents[i];
			uv0[i]=this.uv0[i];
			uv1[i]=this.uv1[i];
			colors[i]=this.colors[i];
		}

		copyLength = Mathf.Min (this.triangles.Length, triCount);

		for (int i=0; i<copyLength; i++) {
			triangles[i]=this.triangles[i];
		}

		this.verts = verts;
		this.normals = normals;
		this.tangents = tangents;
		this.uv0 = uv0;
		this.uv1 = uv1;
		this.colors = colors;
		this.triangles = triangles;
	}
	
	protected virtual void UpdateDirection(PAParticleField settings, int startAt){
		
	}
	
	protected virtual void UpdateSurface(PAParticleField settings, int startAt){
		
	}
	
	protected virtual void UpdateSpeed(PAParticleField settings, int startAt){
		
	}
	
	protected virtual void UpdateColor(PAParticleField settings, int startAt){
		
	}

    protected virtual void UpdateIndicies() {

    }

	protected virtual void UpdateTriangles(int startAt){

	}

    public void ClearCache()
    {
        verts = new Vector3[0];
        normals = new Vector3[0];
        tangents = new Vector4[0];
        uv0 = new Vector2[0];
        uv1 = new Vector2[0];
        colors = new Color[0];
        triangles = new int[0];
    }

	public void CheckDataStructureVersion(){
		if (dataStructureVersion != DATA_STRUCTURE_VERSION) {
			UpdateCache(GetComponent<PAParticleField>());
		}
	}

    public static class RandomWrapper
    {
        public static void CacheState()
        {
#if UNITY_5_4_OR_NEWER
            cachedState = Random.state;
#else
            cachedState = Random.seed;
#endif
        }

        public static void RestoreState()
        {
#if UNITY_5_4_OR_NEWER
            Random.state = cachedState;
#else
            Random.seed = cachedState;
#endif
        }

        public static void SetState(int seed)
        {
#if UNITY_5_4_OR_NEWER
            Random.InitState(seed);
#else
            Random.seed = seed;
#endif
            m_Seed = seed;
            Random.Range(0f, 0f); //skip the first random value after setting state, it's not random
        }

        public static int seed
        {
            get{
                return m_Seed;
            }
        }
        private static int m_Seed;

#if UNITY_5_4_OR_NEWER
        public static Random.State cachedState;
#else
        public static int cachedState;
#endif
    }
}
