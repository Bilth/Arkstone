using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// PA Particle Field.
/// Initiates particle mesh creation, creates materials and passes material parameters to control the field behaviour
/// </summary>
[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer))]
public class PAParticleField : MonoBehaviour {

	public enum ParticleType{
		Billboard = 0,
		Mesh = 1,
        Custom = 2
	}

	public enum SimulationSpace{
		World = 0,
		Local = 1,
        LocalWithDelta = 2
	}

	public enum Shape{
		Cube = 0,
		Sphere = 1,
		Cylinder = 2
	}

	public enum EdgeMode{
		Alpha = 0,
		Scale = 1,
		Both = 2
	}

	public enum MaterialType{
		Transparent = 0,
		TransparentLit = 1,
		Additive = 2,
		AdditiveLit = 3,
		CutOff = 4,
		CutOffLit = 5,
		Custom = 6,
		MeshDefault = 7,
        MeshUnlit = 8
	}

	public enum TextureType{
		Simple = 0,
		SpriteGrid = 1,
		AnimatedRows = 2
	}

	public enum SoftParticleType{
		None = 0,
		NearClipOnly = 1,
		NearClipAndCameraDepth = 2
	}

    public enum TurbulenceType {
        None = 0,
#if UNITY_5
        Simplex2D = 1,
        Simplex = 2
#else
        Sine = 1
#endif
    }

	static readonly string[] builtinShaderNames = {
		"PA/ParticleField/Transparent",
		"PA/ParticleField/TransparentLit",
		"PA/ParticleField/Additive",
		"PA/ParticleField/AdditiveLit",
		"PA/ParticleField/CutOff",
		"PA/ParticleField/CutOffLit",
		"DoNotUse",
		"PA/ParticleField/MeshDefault",
		"PA/ParticleField/MeshUnlit"
	};

	//Max particle count = 65000/4
	const int MAX_PARTICLE_COUNT = 16250;

    public bool clearCacheInBuilds = false;

	private bool isOpenGL = false;

	/// <summary>
	/// Gets or sets the random seed used to generate the particle mesh, modifying initiates a mesh regeneration
	/// </summary>
	/// <value>The seed.</value>
	public int seed{
		get{return mSeed;}
		set{
			if (mSeed != value){
				mSeed = value;
				meshIsDirtyMask |= MeshFlags.Seed;
			}
		}
	}

	/// <summary>
	/// Gets or sets the type of the generator, can be either billboard or mesh, modifying initiates a mesh regeneration
	/// </summary>
	/// <value>The type of the generator.</value>
	public ParticleType generatorType{
		get{return mGeneratorType;}
		set{
			if (mGeneratorType != value){
				mGeneratorType = value;
				meshIsDirtyMask |= MeshFlags.Generator;
			}
		}
	}

	/// <summary>
	/// The particle count, number of particles the resulting mesh will have, modifying initiates a mesh regeneration
	/// </summary>
	public int particleCount{
		get{return mParticleCount;}
		set{

			if (meshGenerator){
				value = meshGenerator.GetClampedParticleCount(value);
			}else{
				value = Mathf.Clamp(value, 0, MAX_PARTICLE_COUNT);
			}

			if (mParticleCount != value){
				mParticleCount = value;
				meshIsDirtyMask |= MeshFlags.Count;
			}
		}
	}

    public float particleCountMask {
        get { return mParticleCountMask; }
        set {
            value = Mathf.Clamp01(value);
            if (mParticleCountMask != value) {
                mParticleCountMask = value;
                shaderIsDirty = true;
            }
        }
    }

	/// <summary>
	/// The size in units of the particle field.
	/// </summary>
	public Vector3 fieldSize{
		get{return mFieldSize;}
		set{
			if (mFieldSize != value){
				mFieldSize = value;
				shaderIsDirty = true;
			}
		}
	}

	/// <summary>
	/// Gets or sets the edge threshold, determining how near the edge of the shape the edge mode affects
	/// </summary>
	/// <value>The edge threshold.</value>
	public Vector3 edgeThreshold{
		get{return mEdgeThreshold;}
		set{
			value.x = Mathf.Clamp01(value.x);
			value.y = Mathf.Clamp01(value.y);
			value.z = Mathf.Clamp01(value.z);
			if (mEdgeThreshold != value){
				mEdgeThreshold = value;
				shaderIsDirty = true;
			}
		}
	}

	/// <summary>
	/// The simulation space. When using world space moving the transform will not move/affect the particles. When localspace is used the particles move with the transform.
	/// </summary>
	/// <value>The simulation space.</value>
	public SimulationSpace simulationSpace{
		get{ return mSimulationSpace;}
		set{
			if (mSimulationSpace != value){
				mSimulationSpace = value;
				shaderIsDirty = true;
			}
		}
	}

	/// <summary>
	/// The shape of the field.
	/// </summary>
	/// <value>The shape.</value>
	public Shape shape{
		get{ return mShape;}
		set{
			if (mShape!=value){
				mShape = value;
				shaderIsDirty = true;
			}
		}
	}

	/// <summary>
	/// Gets or sets the edge mode used to make the field shape.
	/// </summary>
	/// <value>The edge mode.</value>
	public EdgeMode edgeMode{
		get{ return mEdgeMode;}
		set{
			if (mEdgeMode!=value){
				mEdgeMode = value;
				shaderIsDirty = true;
			}
		}
	}

	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="PAParticleField"/> uses exclusion zones. Up to 3 exclusion zones can affect a field at a time.
	/// </summary>
	/// <value><c>true</c> if use exclusion zones; otherwise, <c>false</c>.</value>
	public bool useExclusionZones {
		get{
			return mUseExclusionZones;
		}
		set{
			if (mUseExclusionZones != value){
				mUseExclusionZones = value;
				shaderIsDirty = true;
			}
		}
	}

	/// <summary>
	/// Gets or sets the exclusion anchor override, which by default prioritizes exclusion zones are by distance from the center of the field
	/// </summary>
	/// <value>The exclusion anchor override.</value>
	public Transform exclusionAnchorOverride{
		get{
			return mExclusionAnchorOverride;
		}
		set{
			if (mExclusionAnchorOverride != value){
				mExclusionAnchorOverride = value;
			}
		}
	}

	/// <summary>
	/// Gets or sets the global color.
	/// </summary>
	/// <value>The color.</value>
	public Color color{
		get{ return mColor;}
		set{
			if (mColor != value){
				mColor = value;
				shaderIsDirty = true;
			}
		}
	}

	/// <summary>
	/// Quick accesor for the global alpha
	/// </summary>
	/// <value>The alpha.</value>
	public float alpha{
		get{ return color.a;}
		set{
			if (color.a != value){
				Color col = color;
				col.a = value;
				color = col;
			}
		}
	}

	/// <summary>
	/// Gets or sets the overall speed of the paricles individial movement
	/// </summary>
	/// <value>The speed.</value>
	public float speed{
		get{return mSpeed;}
		set{
			if (mSpeed != value){
				mSpeed = value;
				shaderIsDirty = true;
			}
		}
	}

	public Vector3 speedMask{
		get{return mSpeedMask;}
		set{
			if (mSpeedMask != value){
				mSpeedMask = value;
				shaderIsDirty = true;
			}
		}
	}

	/// <summary>
	/// Gets or sets the max size of the particle.
	/// </summary>
	/// <value>The size of the particle.</value>
	public Vector2 particleSize{
		get{return mParticleSize;}
		set{
			if (mParticleSize != value){
				mParticleSize = value;
				shaderIsDirty = true;
			}
		}
	}

	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="PAParticleField"/>'s particles should scale by speed alonf the direction they are travelling. Note that this can cause particles to disappear if the final per particle speed is zero
	/// </summary>
	/// <value><c>true</c> if scale by speed; otherwise, <c>false</c>.</value>
	public bool stretchedBillboard{
		get{return mStretchedBillboard;}
		set{
			if (mStretchedBillboard != value){
				mStretchedBillboard = value;
				if (value){
					mCustomUpDirection = false;
					mSpin = false;
				}
				shaderIsDirty = true;
			}
		}
	}

	/// <summary>
	/// Gets or sets the speed scale multiplier. When scaleBySpeed is true, determines the length of the particle
	/// </summary>
	/// <value>The speed scale multiplier.</value>
	public float speedScaleMultiplier{
		get{return mSpeedScaleMultiplier;}
		set{
			if (mSpeedScaleMultiplier != value){
				mSpeedScaleMultiplier = value;
				shaderIsDirty = true;
			}
		}
	}

	/// <summary>
	/// When true the particles will spin around thier pivot
	/// </summary>
	/// <value><c>true</c> if spin; otherwise, <c>false</c>.</value>
	public bool spin{
		get{return mSpin;}
		set{
			if(mSpin != value){
				mSpin = value;
				if (value){
					mStretchedBillboard = false;
					mCustomUpDirection = false;
				}
				shaderIsDirty = true;
			}
		}
	}

	/// <summary>
	/// Gets or sets the base spin speed.
	/// </summary>
	/// <value>The spin speed.</value>
	public float spinSpeed{
		get{return mSpinSpeed;}
		set{
			if(mSpinSpeed != value){
				mSpinSpeed = value;
				shaderIsDirty = true;
			}
		}
	}

	/// <summary>
	/// Gets or sets the minimum spin speed, as a fraction of the base spinSpeed. Negative values will make some particles spin the other direction. Modifying causes a mesh generation.
	/// </summary>
	/// <value>The minimum spin speed.</value>
	public float minSpinSpeed{
		get{return mMinSpinSpeed;}
		set{
			value = Mathf.Clamp(value, -1f, 1f);
			if (mMinSpinSpeed != value){
				mMinSpinSpeed = value;
				meshIsDirtyMask |= MeshFlags.Speed;
			}
		}
	}

    /// <summary>
    /// For mesh particles, when true the all particles will use the rotationAxis value, when false they use a per particle random direction
    /// </summary>
    public bool customRotationAxis {
        get {
            return mCustomRotationAxis;
        }
        set {
            if (mCustomRotationAxis != value) {
                mCustomRotationAxis = value;
                meshIsDirtyMask |= MeshFlags.Speed;
            }
        }
    }

    /// <summary>
    /// For mesh particles, the axis around which the particle will rotate
    /// </summary>
    public Vector3 rotationAxis {
        get { return mRotationAxis; }
        set {
            if (mRotationAxis != value) {
                mRotationAxis = value;
                meshIsDirtyMask |= MeshFlags.Speed;
            }
        }
    }

	/// <summary>
	/// When false particles will face the camera, otherwise they will face the facingDirection vector
	/// </summary>
	/// <value><c>true</c> if custom facing direction; otherwise, <c>false</c>.</value>
	public bool customFacingDirection {
		get{return mCustomFacingDirection;}
		set{
			if(mCustomFacingDirection != value){
				mCustomFacingDirection = value;
				shaderIsDirty = true;
			}
		}
	}

	/// <summary>
	/// The facing direction, used when customFacingDirection is true. Particles will face this direction.
	/// </summary>
	/// <value>The facing direction.</value>
	public Vector3 facingDirection{
		get{return mFacingDirection;}
		set{
			if (mFacingDirection != value){
				mFacingDirection = value;
				mStretchedBillboard = false;
				shaderIsDirty = true;
			}
		}
	}

	/// <summary>
	/// when false the world or local up direction will be used to orient the particles, Using a custom up is useful for effects like caustics.
	/// </summary>
	/// <value><c>true</c> if custom up direction; otherwise, <c>false</c>.</value>
	public bool customUpDirection {
		get{return mCustomUpDirection;}
		set{
			if(mCustomUpDirection != value){
				mCustomUpDirection = value;
				if (value){
					mSpin = false;
					mStretchedBillboard = false;
				}
				shaderIsDirty = true;
			}
		}
	}

	/// <summary>
	/// Gets or sets up direction used when customUpDirection is true
	/// </summary>
	/// <value>Up direction.</value>
	public Vector3 upDirection{
		get{return mUpDirection;}
		set{
			if (mUpDirection != value){
				mUpDirection = value;
				shaderIsDirty = true;
			}
		}
	}

	/// <summary>
	/// Gets or sets the soft particles type, NearClipAndCameraDepth require Pro in Unity 4.x
	/// </summary>
	/// <value>The soft particles.</value>
	public SoftParticleType softParticles {
		get{return mSoftParticles;}
		set{if(value == SoftParticleType.NearClipAndCameraDepth && int.Parse(Application.unityVersion.Split('.')[0])<5 && !Application.HasProLicense()){ 
				Debug.Log("Soft particles requires Unity Pro"); 
				if(mSoftParticles != SoftParticleType.NearClipOnly){
					mSoftParticles = SoftParticleType.NearClipOnly; 
					shaderIsDirty = true;
				}
			}
			else if(mSoftParticles!=value){
				mSoftParticles = value;
				shaderIsDirty = true;
			}
		}
	}

	/// <summary>
	/// Gets or sets distance from the camera that the particles will start fading at to prevent clipping, if the shader supports it
	/// </summary>
	/// <value>The near fade distance.</value>
	public float nearFadeDistance{
		get{return mNearFadeDistance;}
		set{
			if (mNearFadeDistance != value){
				mNearFadeDistance = value;
				shaderIsDirty = true;
			}
		}
	}

	/// <summary>
	/// Gets or sets the distance from the camera that particles will be completely transparent at if softParticles is on and the shader supports it
	/// </summary>
	/// <value>The near fade offset.</value>
	public float nearFadeOffset{
		get{return mNearFadeOffset;}
		set{
			if (mNearFadeOffset != value){
				mNearFadeOffset = value;
				shaderIsDirty = true;
			}
		}
	}

	/// <summary>
	/// Gets or sets the softness when soft particles is on
	/// </summary>
	/// <value>The softness.</value>
	public float softness{
		get{return mSoftness;}
		set{
			if (mSoftness != value){
				mSoftness = value;
				shaderIsDirty = true;
			}
		}
	}

    public TurbulenceType turbulenceType {
        get { return mTurbulenceType; }
        set {
            if (mTurbulenceType != value) {
                mTurbulenceType = value;
                shaderIsDirty = true;
            }
        }
    }

    public float turbulenceFrequency {
        get { return mTurbulenceFrequency; }
        set {
            if (mTurbulenceFrequency != value) {
                mTurbulenceFrequency = value;
                shaderIsDirty = true;
            }
        }
    }

    public float turbulenceAmplitude {
        get { return mTurbulenceAmplitude; }
        set {
            if (mTurbulenceAmplitude != value) {
                mTurbulenceAmplitude = value;
                shaderIsDirty = true;
            }
        }
    }

    public Vector3 turbulenceScale {
        get { return mTurbulenceScale; }
        set {
            if (mTurbulenceScale != value) {
                mTurbulenceScale = value;
                shaderIsDirty = true;
            }
        }
    }

    public Vector3 turbulenceOffsetSpeed {
        get { return mTurbulenceOffsetSpeed; }
        set {
            if (mTurbulenceOffsetSpeed != value) {
                mTurbulenceOffsetSpeed = value;
                shaderIsDirty = true;
            }
        }
    }

	/// <summary>
	/// Gets or sets the color variation. The color is determined by a random Gradient sample allowing a multitude of defined colors. Modifying causes a mesh regeneration
	/// </summary>
	/// <value>The color variation.</value>
	public Gradient colorVariation
	{
		get{
			if (mColorVariation == null){
				mColorVariation = new Gradient();
			}
			return mColorVariation;
		}
		set{
			if (mColorVariation != value){
				mColorVariation = value;
				meshIsDirtyMask |= MeshFlags.Color;
			}
		}
	}

	/// <summary>
	/// Gets or sets the minimum size value between [0-1], the size of each particle will be randomized between particleSize and (minimumSize * particleSize). Modifying causes a mesh regeneration.
	/// </summary>
	/// <value>The minimum size.</value>
	public float minimumSize{
		get{return mMinimumSize;}
		set{
			value = Mathf.Clamp01(value);
			if (mMinimumSize != value){
				mMinimumSize = value;
				meshIsDirtyMask |= MeshFlags.Surface;
			}
		}
	}

	/// <summary>
	/// Gets or sets the minimum speed value between [0-1], the speed of each particle will be randomized between speed and (minimumSpeed * speed). Modifying causes a mesh regeneration.
	/// </summary>
	/// <value>The minimum speed.</value>
	public float minimumSpeed{
		get{return mMinimumSpeed;}
		set{
			value = Mathf.Clamp01(value);
			if (mMinimumSpeed != value){
				mMinimumSpeed = value;
				meshIsDirtyMask |= MeshFlags.Speed;
			}
		}
	}

	/// <summary>
	/// a constant force applied to all particles
	/// </summary>
	public Vector3 force{
		get{return mForce;}
		set{
			if (mForce != value){
				mForce = value;
				shaderIsDirty = true;
			}
		}
	}

	/// <summary>
	/// Gets or sets the type of the material used to draw the particles.
	/// </summary>
	/// <value>The type of the material.</value>
	public MaterialType materialType{
		get{return mMaterialType;}
		set{
			if (mMaterialType != value){
				material = null;
				mMaterialType = value;
				if (mMaterialType != MaterialType.Custom){
				    shader = Shader.Find(builtinShaderNames[(int)mMaterialType]);
					shaderIsDirty = true;
				}
			}
		}
	}

	/// <summary>
	/// Gets the current shader.
	/// </summary>
	/// <value>The shader.</value>
	public Shader shader{
		get{return mShader;}
		private set{
			if (mShader != value){
				mShader = value;
				shaderIsDirty = true;
			}
		}
	}

	/// <summary>
	/// Gets or sets the texture.
	/// </summary>
	/// <value>The texture.</value>
	public Texture2D texture{
		get{return mTexture;}
		set{
			if (mTexture != value){
				mTexture = value;
				shaderIsDirty = true;
			}
		}
	}

	/// <summary>
	/// Gets or sets the texture type, allowing atlassed sprites and sprite based animation
	/// </summary>
	/// <value>The type of the texture.</value>
	public TextureType textureType{
		get{return mTextureType;}
		set{
			if (mTextureType != value){
				mTextureType = value;
				shaderIsDirty= true;
                meshIsDirtyMask |= MeshFlags.Surface;
			}
		}
	}

	/// <summary>
	/// If the particle texture has multiple sprites arranged in a fixed size grid, this is the number of columns. Modifying casues a mesh regeneration.
	/// </summary>
	public int spriteColumns{
		get{return mSpriteColumns;}
		set{
			value = Mathf.Min(value, 1);
			if (mSpriteColumns != value){
				mSpriteColumns = value;
				meshIsDirtyMask |= MeshFlags.Surface;
			}
		}
	}

	/// <summary>
	/// If the particle texture has multiple sprites arranged in a fixed size grid, this is the number of rows. Modifying causes a mesh regeneration
	/// </summary>
	public int spriteRows{
		get{return mSpriteRows;}
		set{
			value = Mathf.Min(value, 1);
			if (mSpriteRows != value){
				mSpriteRows = value;
				meshIsDirtyMask |= MeshFlags.Surface;
			}
		}
	}

	/// <summary>
	/// If the particle texture has multiple sprites arranged in a fixed size grid, this is the number of rows
	/// </summary>
	public float framerate{
		get{return mFramerate;}
		set{
			if (mFramerate != value){
				mFramerate = value;
			}
		}
	}

	/// <summary>
	/// Gets or sets the alpha cut off, when the blend mode is set to CutOff
	/// </summary>
	/// <value>The cut off.</value>
	public float cutOff {
		get {return mCutOff;}
		set {
			value = Mathf.Clamp01(value);
			if (mCutOff != value){
				mCutOff = value;
				shaderIsDirty = true;
			}
		}
	}

	/// <summary>
	/// Gets or sets the pivot offset. default value is 0,0 modifying this changes how the particle scales and rotates. Modifying causes a mesh regeneration.
	/// </summary>
	/// <value>The pivot offset.</value>
	public Vector2 pivotOffset{
		get{return mPivotOffset;}
		set{
			if (mPivotOffset != value){
				mPivotOffset = value;
				meshIsDirtyMask |= MeshFlags.Surface;
			}
		}
	}

	public Mesh inputMesh{
		get{
			return mInputMesh;
		}
		set{
			mInputMesh = value;
			meshIsDirtyMask |= MeshFlags.All;
		}
	}

    public bool receiveShadows {
        get {return mReceiveShadows;}
        set { mReceiveShadows = value; if (meshRenderer) { meshRenderer.receiveShadows = value; } }
    }

#if UNITY_5
    public UnityEngine.Rendering.ShadowCastingMode castShadows {
        get { return mCastShadows; }
        set { mCastShadows = value; if (meshRenderer) { meshRenderer.shadowCastingMode = value; } }
    }
#else
    public bool castShadows {
        get { return mCastShadows; }
        set { mCastShadows = value; if (meshRenderer) { meshRenderer.castShadows = value; } }
    }
#endif

    [SerializeField]
	int mSeed = 1234;
	[SerializeField]
	ParticleType mGeneratorType = ParticleType.Billboard;
	[SerializeField]
	Mesh mInputMesh;
	[SerializeField]
	int mParticleCount = 1200;
    [SerializeField]
    float mParticleCountMask = 1f;
	[SerializeField]
	Vector3 mFieldSize = new Vector3(10, 10, 10);
	[SerializeField]
	Vector3 mEdgeThreshold = Vector3.one;
	[SerializeField]
	EdgeMode mEdgeMode = EdgeMode.Alpha;
	[SerializeField]
	SimulationSpace mSimulationSpace = SimulationSpace.World;
	[SerializeField]
	Shape mShape;

	[SerializeField]
	bool mUseExclusionZones = false;
	[SerializeField]
	Transform mExclusionAnchorOverride;

	[SerializeField]
	Vector2 mParticleSize = new Vector2(0.1f, 0.1f);
	[SerializeField]
	float mSpeed = 0.1f;
	[SerializeField]
	Vector3 mSpeedMask = Vector3.one;
	[SerializeField]
	Color mColor = Color.white;
	[SerializeField]
	Vector3 mForce = Vector3.zero;
	[SerializeField]
	bool mCustomFacingDirection = false;
	[SerializeField]
	Vector3 mFacingDirection = Vector3.up;
	[SerializeField]
	bool mCustomUpDirection = false;
	[SerializeField]
	Vector3 mUpDirection = new Vector3(0f, 1f, 0f);
	[SerializeField]
	bool mStretchedBillboard = false;
	[SerializeField]
	float mSpeedScaleMultiplier = 10f;
	[SerializeField]
	bool mSpin;
	[SerializeField]
	float mSpinSpeed = 0;
	[SerializeField]
	float mMinSpinSpeed = -1f;
    [SerializeField]
    bool mCustomRotationAxis = false;
    [SerializeField]
    Vector3 mRotationAxis = new Vector3(0f, 1f, 0f);

	[SerializeField]
	SoftParticleType mSoftParticles = SoftParticleType.NearClipOnly;
	[SerializeField]
	float mNearFadeDistance = 1;
	[SerializeField]
	float mNearFadeOffset = 0;
	[SerializeField]
	float mSoftness = 0.5f;

    [SerializeField]
    TurbulenceType mTurbulenceType = TurbulenceType.None;
    [SerializeField]
    float mTurbulenceFrequency = 1f;
    [SerializeField]
    float mTurbulenceAmplitude = 1f;
    [SerializeField]
    Vector3 mTurbulenceScale = Vector3.one;
    [SerializeField]
    Vector3 mTurbulenceOffsetSpeed = new Vector3(0f, 0.25f, 0f);

	[SerializeField]
	Gradient mColorVariation = new Gradient();
	[SerializeField]
	float mMinimumSize = 1f;
	[SerializeField]
	float mMinimumSpeed = 1f;

	[SerializeField]
	MaterialType mMaterialType = MaterialType.Transparent;
	[SerializeField]
	Shader mShader;
	[SerializeField]
	Texture2D mTexture;
	[SerializeField]
	Vector2 mPivotOffset = new Vector2(0.0f, 0.0f);
	[SerializeField]
	TextureType mTextureType;
	[SerializeField]
	int mSpriteColumns = 1;
	[SerializeField]
	int mSpriteRows = 1;
	[SerializeField]
	float mFramerate = 16f;
	[SerializeField]
	float mCutOff = 0.01f;

    [SerializeField]
    bool mReceiveShadows = false;
#if UNITY_5
    [SerializeField]
    UnityEngine.Rendering.ShadowCastingMode mCastShadows = UnityEngine.Rendering.ShadowCastingMode.Off;
#else
    [SerializeField]
    bool mCastShadows = false;
#endif

	Mesh particleMesh;
	MeshFilter meshFilter;
	MeshRenderer meshRenderer;

	PAParticleMeshGenerator meshGenerator{
		get{
			if (m_MeshGenerator == null){
				m_MeshGenerator = GetComponent<PAParticleMeshGenerator>();
				if (m_MeshGenerator == null && generatorType != ParticleType.Custom){
					UpdateGeneratorType(generatorType);
				}
			}
			return m_MeshGenerator;
		}
        set {
            m_MeshGenerator = value;
        }
	}
	[SerializeField]
	PAParticleMeshGenerator m_MeshGenerator;

	[SerializeField]
	public Material material;

    //Rendering material needs to be serialized for shader_features to be tracked in the build
    //but serializing it causes all sorts of issues with prefabs
    Material renderingMaterial;

    //time variables
	float time = 0f;
	Vector3 speedTime = Vector3.zero;
	Vector3 forceTime = Vector3.zero;
	float spinTime = 0f;
    Vector3 turbulenceOffsetTime = Vector3.zero;
    float frameTime = 0f;

    //transform tracking
    Vector3 position;
    Vector3 deltaPosition;
    Vector3 scale;
    
	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="PAParticleField"/> mesh is dirty. If the mesh is dirty when Update() is called the mesh will be regenerated
	/// </summary>
	public MeshFlags meshIsDirtyMask{ get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="PAParticleField"/> shader is dirty.
	/// </summary>
	/// <value><c>true</c> if shader is dirty; otherwise, <c>false</c>.</value>
	public bool shaderIsDirty{ get; set;}

    private bool foundExclusionZones = false;
	private PAExclusionZone[] zones = new PAExclusionZone[3];

    /// <summary>
    /// Gets or adds a component
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    T GetOrAddComponent<T>() where T : Component {
        T result = GetComponent<T>();
        if (!result) {result = gameObject.AddComponent<T>();}
        return result;
    }

	/// <summary>
	/// Creates the mesh objects and components required to display the field
	/// </summary>
	void GetRenderingComponents(){
		//Get or add the mesh filter component
        meshFilter = GetOrAddComponent<MeshFilter>();
        meshFilter.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector; //TODO DontSave but throws cleaning up errors

		//Get or add the mesh renderer component
        meshRenderer = GetOrAddComponent<MeshRenderer>();
		meshRenderer.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector; //TODO DontSave but throws cleaning up errors

        //PAPF doesnt fully support shadows just yet
		meshRenderer.receiveShadows = mReceiveShadows;
#if UNITY_5
        meshRenderer.shadowCastingMode = mCastShadows;
#else
		meshRenderer.castShadows = mCastShadows;
#endif
    }

    void CreateAssetTypes(){
		//Create the mesh to populate with particles 
		if (!particleMesh) {
			particleMesh = new Mesh ();
            particleMesh.name = gameObject.name + "_PAPF";
		} 
		particleMesh.hideFlags = HideFlags.HideAndDontSave | HideFlags.HideInInspector;
		meshFilter.sharedMesh = particleMesh;

		//Find the default shader
		if (!shader) {
			shader = Shader.Find ("PA/ParticleField/Transparent"); 
		}

        renderingMaterial = CreateInstanceMaterial();

        meshRenderer.sharedMaterial = renderingMaterial;
    }

    /// <summary>
    /// Creates a hidden material for rendering this field
    /// </summary>
    /// <param name="shader"></param>
    Material CreateInstanceMaterial() {
        Material result = new Material((materialType == MaterialType.Custom && material) ? material.shader : shader);
        result.name = gameObject.name + " (Instance)" + System.DateTime.Now.Millisecond;
        result.hideFlags = HideFlags.HideInInspector | HideFlags.HideAndDontSave;
        return result;
    }

	/// <summary>
	/// Ensures the current mesh particle generator is the right type with the right inputs
	/// </summary>
	void UpdateGeneratorType(ParticleType newType){
        //If we're not changing to a custom type (switching between mesh and billboard) destroy the old generator
        if (m_MeshGenerator) {
            if (newType != ParticleType.Custom) {
                DestroyImmediate(m_MeshGenerator);
            } else {
                //if we're switching to a custom type, expose the old generator for removal
                m_MeshGenerator.hideFlags = HideFlags.None;
            }
        }

		if (newType == ParticleType.Billboard) {
            m_MeshGenerator = gameObject.AddComponent<PABillboardParticle>();
            m_MeshGenerator.hideFlags = HideFlags.HideInInspector;
		} else if (newType == ParticleType.Mesh) {
			PAMeshParticle m = gameObject.AddComponent<PAMeshParticle>();
			m.inputMesh = inputMesh;
			m_MeshGenerator = m;
		    m_MeshGenerator.hideFlags = HideFlags.HideInInspector;
		}

        if (newType != ParticleType.Custom) {
            if (materialType != MaterialType.Custom) {
                if (newType == ParticleType.Mesh) {
                    materialType = MaterialType.MeshDefault;
                } else {
                    materialType = MaterialType.Transparent;
                }
                CreateAssetTypes();
            }
        }

		mGeneratorType = newType;
		shaderIsDirty = true;
		meshIsDirtyMask = MeshFlags.All;
	}

	/// <summary>
	/// Updates all the values used by the this instances PAParticleField shaders to the local copeis of those values
	/// </summary>
	void SetShaderValues(){

        PAPFHelper.GetPropertyIDs();

        //Ensure the material is up to date and applied
		if (materialType == MaterialType.Custom && material != null)
		{
            if (renderingMaterial.shader != material.shader) {
                renderingMaterial.shader = material.shader;
            }
			renderingMaterial.CopyPropertiesFromMaterial(material);
        } else if (renderingMaterial.shader != shader) { //check correct shader is not already assigned to prevent garbage
            renderingMaterial.shader = shader;
        }

		Vector3 finalSpeed = speed * speedMask;

        //Update speeds and deltas
        Vector3 deltaSpeed = new Vector3(finalSpeed.x / fieldSize.x, finalSpeed.y / fieldSize.y, finalSpeed.z / fieldSize.z);
        Vector3 deltaForce = new Vector3(-force.x / fieldSize.x, -force.y / fieldSize.y, -force.z / fieldSize.z);

        renderingMaterial.SetVector(PAPFHelper._DeltaSpeed, deltaSpeed);
        renderingMaterial.SetVector(PAPFHelper._DeltaForce, deltaForce);
        renderingMaterial.SetVector(PAPFHelper._TurbulenceDeltaOffset, turbulenceOffsetSpeed * (1f/60f));

        Vector3 localFieldSize = Vector3.Scale(fieldSize, transform.lossyScale);
        Vector3 fieldScaledDelta = simulationSpace == SimulationSpace.LocalWithDelta ? new Vector3(deltaPosition.x / localFieldSize.x, deltaPosition.y / localFieldSize.y, deltaPosition.z / localFieldSize.z) : Vector3.zero;
        renderingMaterial.SetVector(PAPFHelper._DeltaPosition, fieldScaledDelta);

        //if not a custom material apply the fields color directly
        if (materialType != MaterialType.Custom)
        {
            renderingMaterial.SetColor(PAPFHelper._Color, color);
        } //if a custom material apply this fields color multiploied over the custom materials _Color prop
        else if (material != null && material.HasProperty(PAPFHelper._Color))
        {
            renderingMaterial.SetColor(PAPFHelper._Color, material.color * color);
        }

        //Update fragment properites if not using a custom material
        if (materialType != MaterialType.Custom)
        {
            renderingMaterial.SetTexture(PAPFHelper._MainTex, texture);
            renderingMaterial.SetFloat(PAPFHelper._CutOff, cutOff);
        }

        //Reset _UOffset if not using animated rows
        if (textureType != TextureType.AnimatedRows) {
            renderingMaterial.SetFloat(PAPFHelper._UOffset, 0f);
        }

        //Update field properties
        renderingMaterial.SetFloat(PAPFHelper._CountMask, particleCountMask);
        renderingMaterial.SetFloat(PAPFHelper._ParticleCount, particleCount);

		renderingMaterial.SetVector(PAPFHelper._FieldSize, Vector3.Scale(fieldSize, (simulationSpace == SimulationSpace.World ? transform.lossyScale : Vector3.one)));
        renderingMaterial.SetVector(PAPFHelper._EdgeThreshold, Vector3.one - edgeThreshold);
        renderingMaterial.SetVector(PAPFHelper._InverseEdgeThreshold, new Vector3(1f / edgeThreshold.x, 1f / edgeThreshold.y, 1f / edgeThreshold.z));
		renderingMaterial.SetVector(PAPFHelper._ParticleSize, particleSize);
		renderingMaterial.SetFloat (PAPFHelper._SpeedScale, stretchedBillboard ? speedScaleMultiplier : 1f);
		renderingMaterial.SetVector(PAPFHelper._FaceDirection, customFacingDirection ? facingDirection.normalized + Vector3.right * 0.001f : Vector3.forward); //hacky fix to prevent parrallel up vectors
		renderingMaterial.SetVector(PAPFHelper._UpDirection,  mCustomUpDirection ? upDirection.normalized : Vector3.up);

        //Update Softness properties
		renderingMaterial.SetFloat (PAPFHelper._NearFadeDistance, softParticles != SoftParticleType.None ? nearFadeDistance : 0f);
		renderingMaterial.SetFloat (PAPFHelper._NearFadeOffset, softParticles != SoftParticleType.None ? nearFadeOffset : 0f);
		renderingMaterial.SetFloat(PAPFHelper._Softness,softness);

        //Update Turbulence
        renderingMaterial.SetFloat(PAPFHelper._TurbulenceFrequency, turbulenceFrequency);
        renderingMaterial.SetVector(PAPFHelper._TurbulenceScale, turbulenceScale * turbulenceAmplitude);

        //Update Float "Keywords"
        SetFloatKeyword(PAPFHelper._EdgeAlpha, edgeMode == EdgeMode.Alpha || edgeMode == EdgeMode.Both);
        SetFloatKeyword(PAPFHelper._EdgeScale, edgeMode == EdgeMode.Scale || edgeMode == EdgeMode.Both);
        SetFloatKeyword(PAPFHelper._UserFacing, customFacingDirection);
        SetFloatKeyword(PAPFHelper._Editor, !Application.isPlaying);

        //Update Keywords
        SetKeyword ("DIRECTIONAL_ON", stretchedBillboard);
		SetKeyword ("WORLDSPACE_ON", simulationSpace == SimulationSpace.World);		
		SetKeyword ("SPIN_ON", spin);
#if UNITY_5
        SetKeyword("TURBULENCE_SIMPLEX2D", turbulenceType == TurbulenceType.Simplex2D);
        SetKeyword ("TURBULENCE_SIMPLEX", turbulenceType == TurbulenceType.Simplex);
#else
		SetKeyword ("TURBULENCE_SIN", turbulenceType != TurbulenceType.None);	
#endif
        SetKeyword ("SHAPE_SPHERE", shape == Shape.Sphere);
		SetKeyword ("SHAPE_CYLINDER", shape == Shape.Cylinder);

        //Update the renderers culling bounds to the field size plus 1 particle padding in all directions
        float maxParticleScale = generatorType == ParticleType.Mesh ? particleSize.x : Mathf.Max(particleSize.x, particleSize.y);
        if (particleMesh && meshGenerator) {
            particleMesh.bounds = new Bounds(Vector3.zero, fieldSize + maxParticleScale * meshGenerator.GetParticleBaseSize() * 2 * Vector3.one);
        }
	}

	void SetKeyword(string keyword, bool enable){
        SetMaterialKeyword(keyword, enable, renderingMaterial);
	}

    static void SetMaterialKeyword(string keyword, bool enable, Material material){
		if (!material) return;

		if (enable){
			material.EnableKeyword(keyword);
		}else{
			material.DisableKeyword(keyword);
		}
    }

    void SetFloatKeyword(string keyword, bool enable) {
        renderingMaterial.SetFloat(keyword, enable ? 1 : 0);
    }

    void SetFloatKeyword(int keywordID, bool enable) {
        renderingMaterial.SetFloat(keywordID, enable ? 1 : 0);
    }

	/// <summary>
	/// Immediately updates the particle fields mesh and shader without checking dirty flags
	/// </summary>
	public void UpdateParticleField(){
		UpdateMesh ();
		UpdateShader ();
	}

	/// <summary>
	/// Immediately updates the particle mesh, regardless of MeshFlags
	/// </summary>
	public void UpdateMesh(){
        if ((meshIsDirtyMask & MeshFlags.Generator) != 0) {
            UpdateGeneratorType(generatorType);
        }
        //The generator checks the mesh flags, if any are dirty it will return a new mesh
        if (meshGenerator != null) {
            meshGenerator.UpdateMesh(particleMesh, this);
        }
		meshIsDirtyMask = MeshFlags.None;
	}

	/// <summary>
	/// Immediately updates the shader, regardless of the dirty shader flag
	/// </summary>
	public void UpdateShader(){
		SetShaderValues();
		shaderIsDirty = false;
	}

	/// <summary>
	/// Updates the animation times for this field, should be called once in Update
	/// </summary>
	void UpdateAnimationValues(){
        if (Application.isPlaying) {
            Simulate(Time.deltaTime);
        }
	}

    public void Simulate(float t, bool restart = false)
    {
        if (restart)
        {
            ResetTimers();
        }
        time += t;
        speedTime += speed * speedMask * t;
        forceTime += force * -t;
        spinTime += spinSpeed * t;
        turbulenceOffsetTime += turbulenceOffsetSpeed * t;
        frameTime += t * framerate;
    }

    /// <summary>
    /// Updates exclusion zone values, should be called once in Update
    /// </summary>
    void UpdateExclusionZoneValues() {
        if (useExclusionZones) {
            foundExclusionZones = PAExclusionZone.GetExclusionZones(ref zones, (exclusionAnchorOverride ? exclusionAnchorOverride.position : transform.position), new Bounds(transform.position, fieldSize), gameObject.layer);
        }
    }

	/// <summary>
	/// Passes the time variables from the field to the material
	/// </summary>
	void SetShaderAnimationValues(){
        //When the application is playing, _Time is not used in the shader and absolute values are used instead
        if (Application.isPlaying) {
            renderingMaterial.SetVector(PAPFHelper._Speed, new Vector3(speedTime.x / fieldSize.x, speedTime.y / fieldSize.y, speedTime.z / fieldSize.z));
            renderingMaterial.SetVector(PAPFHelper._Force, new Vector3(forceTime.x / fieldSize.x, forceTime.y / fieldSize.y, forceTime.z / fieldSize.z));
            renderingMaterial.SetFloat(PAPFHelper._SpinSpeed, (spinTime * 0.5f) * 3.145f / 180f);
            renderingMaterial.SetFloat(PAPFHelper._TotalTime, time);
            renderingMaterial.SetVector(PAPFHelper._TurbulenceOffset, turbulenceOffsetTime);

            if (textureType == TextureType.AnimatedRows) {
                int frame = (int)Mathf.Repeat(frameTime, spriteColumns);
                renderingMaterial.SetFloat(PAPFHelper._UOffset, (float)frame / (float)spriteColumns);
            }
        } else {
            //If the application is not playing, the shader uses _Time and values are sent as deltas
            Vector3 finalSpeed = speed * speedMask;

            //Update speeds and deltas
            Vector3 deltaSpeed = new Vector3(finalSpeed.x / fieldSize.x, finalSpeed.y / fieldSize.y, finalSpeed.z / fieldSize.z);
            Vector3 deltaForce = new Vector3(-force.x / fieldSize.x, -force.y / fieldSize.y, -force.z / fieldSize.z);

            renderingMaterial.SetVector(PAPFHelper._Speed, deltaSpeed);
            renderingMaterial.SetVector(PAPFHelper._Force, deltaForce);
            renderingMaterial.SetFloat(PAPFHelper._SpinSpeed, (spinSpeed * 0.5f) * 3.14159f / 180.0f);
            renderingMaterial.SetFloat(PAPFHelper._TotalTime, Time.timeSinceLevelLoad);
            renderingMaterial.SetVector(PAPFHelper._TurbulenceOffset, turbulenceOffsetSpeed);

            if (textureType == TextureType.AnimatedRows) {
                int frame = (int)Mathf.Repeat((Time.timeSinceLevelLoad * framerate), spriteColumns);
                renderingMaterial.SetFloat(PAPFHelper._UOffset, (float)frame / (float)spriteColumns);
            }
        }
	}

	/// <summary>
	/// Updates the exclusion zones and passes the values to the material
	/// </summary>
	void SetShaderExclusionZoneValues(){

        if (!useExclusionZones || !foundExclusionZones) {
            renderingMaterial.DisableKeyword("EXCLUSION_ON");
            return;
        } else {
            renderingMaterial.EnableKeyword("EXCLUSION_ON");

            //Update the exclusion properties of the shader
            for (int i = 0; i < zones.Length; i++) {
                if (zones[i] != null) {
                    //scale the transform to save on shader instructions
                    zones[i].transform.localScale *= 0.5f;
                    Matrix4x4 exclusionMatrix = simulationSpace == SimulationSpace.World ? zones[i].transform.worldToLocalMatrix : zones[i].transform.worldToLocalMatrix * transform.localToWorldMatrix;
                    zones[i].transform.localScale *= 2f;

                    Vector3 edgeThreshold = Vector3.Min(zones[i].edgeThreshold, Vector3.one * 0.9999f);
                    Vector3 inverseEdgeThreshold = new Vector3(1f / (1f - edgeThreshold.x), 1f / (1f - edgeThreshold.y), 1f / (1f - edgeThreshold.z));

                    renderingMaterial.SetMatrix(PAPFHelper._ExclusionMatrix[i], exclusionMatrix);
                    renderingMaterial.SetVector(PAPFHelper._ExclusionThreshold[i], edgeThreshold);
                    renderingMaterial.SetVector(PAPFHelper._InverseExclusionThreshold[i], inverseEdgeThreshold);
                } else {
                    renderingMaterial.SetMatrix(PAPFHelper._ExclusionMatrix[i], Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one * 100000f));
                    renderingMaterial.SetVector(PAPFHelper._ExclusionThreshold[i], Vector3.zero);
                    renderingMaterial.SetVector(PAPFHelper._InverseExclusionThreshold[i], Vector3.one * Mathf.Infinity);
                }
            }
        }
	}

	void Start(){
		meshIsDirtyMask = MeshFlags.None;
		isOpenGL = SystemInfo.graphicsDeviceVersion.ToLower ().Contains ("opengl");

        GetRenderingComponents();
        CreateAssetTypes();
		UpdateParticleField ();
	}

    void OnDisable() {
        //use the opportunity to reset timers
        ResetTimers();
    }

    /// <summary>
    /// Resets timers, useful for preventing floating point precision errors
    /// </summary>
    public void ResetTimers() {
        time = 0f;
        spinTime = 0f;
        speedTime = Vector3.zero;
        forceTime = Vector3.zero;
        turbulenceOffsetTime = Vector3.zero;
        frameTime = 0f;
    }
    
    void Update() {

        if (transform.position != position) {
            Vector3 localDelta = transform.InverseTransformDirection(transform.position - position);
            deltaPosition += localDelta;
            position = transform.position;
            shaderIsDirty = true;
        }

        if (transform.lossyScale != scale) {
            scale = transform.lossyScale;
            shaderIsDirty = true;
        }

        //increment time values
        UpdateAnimationValues();
        UpdateExclusionZoneValues();

        //Update the mesh if it is dirty
		if (meshIsDirtyMask != 0){
			UpdateMesh();
		}

        //If the material is not custom update it now, otherwise update it in OnWillRenderObject
        if (materialType != MaterialType.Custom) {
#if UNITY_EDITOR
            // If the Unity Editor loses focus it doesnt seem to save material properties
            // contained in .cginc files, this would make the fields disappear since most 
            // of the fields are in ParticleField.cginc, setting shaderIsDirty every frame
            // in the editor catches this but obviously not great for editor performance
            // I havent been able to find a better way yet though
            shaderIsDirty = true;
#endif
            if (shaderIsDirty) {
                UpdateShader();
            }
            SetShaderAnimationValues();
            SetShaderExclusionZoneValues();
        }
    }

	void OnWillRenderObject(){
        //OnWillRenderObject can be called before start!
        if (!renderingMaterial) { return; }
        //If the materialType is custom it could beassigned to multiple fields, the shader values must then be updated per field
		if (materialType == MaterialType.Custom) {
			UpdateShader ();
			SetShaderExclusionZoneValues ();
			SetShaderAnimationValues ();
		}

        //Some festures must be changed based on the camera that is rendering the field
        //In opengl clip safe particles doesnt work on orthographic cameras
		if (isOpenGL) {
            renderingMaterial.SetFloat(PAPFHelper._NearFadeDistance, Camera.current.orthographic ? 0 : mNearFadeDistance);
        }

        //Set soft particles based on the soft particle type and if the camera supports them
        bool shouldUseSoftParticles = softParticles == SoftParticleType.NearClipAndCameraDepth;
        shouldUseSoftParticles &= Camera.current.depthTextureMode != DepthTextureMode.None;
#if UNITY_5
        shouldUseSoftParticles &= !Application.isMobilePlatform;
#endif
        SetKeyword("SOFTPARTICLES_ON", shouldUseSoftParticles);
	}

    void OnDestroy() {
        if (renderingMaterial) {
            DestroyImmediate(renderingMaterial);
        }
        if (particleMesh) {
            DestroyImmediate(particleMesh);
        }
    }

	/// <summary>
	/// Gets the max particle count this field can have.
	/// </summary>
	/// <returns>The max count.</returns>
	public int GetMaxCount(){
		if (meshGenerator != null) {
			return meshGenerator.GetMaximumParticleCount ();
		} else {
			return MAX_PARTICLE_COUNT;
		}
	}

	/// <summary>
	/// Gets the bounds of this field
	/// </summary>
	/// <returns>The bounds.</returns>
	public Bounds GetBounds(){
		if (meshRenderer) {
			return meshRenderer.bounds;
		} else {
			return new Bounds(transform.position, Vector3.Scale(fieldSize,transform.lossyScale));
		}
	}

	/// <summary>
	/// Create a particle field with the specified name.
	/// </summary>
	/// <param name="name">Name.</param>
	public static PAParticleField Create(string name){
		return (new GameObject (name)).AddComponent<PAParticleField> ();
	}

    /// <summary>
    /// An unused serialized material to ensure that all required shaders and shader_features are included in the build
    /// </summary>
    [SerializeField]
    Material temporarySerializableMaterial;

    /// <summary>
    /// Creates a serialized material will keywords for all varients used by this field
    /// </summary>
    void CreateTemporarySerializableMaterial() {

        temporarySerializableMaterial = new Material(mMaterialType != MaterialType.Custom ? shader : material.shader);
        
#if UNITY_5
		//Update Keywords
        SetMaterialKeyword("DIRECTIONAL_ON", stretchedBillboard, temporarySerializableMaterial);
        SetMaterialKeyword("WORLDSPACE_ON", simulationSpace == SimulationSpace.World, temporarySerializableMaterial);
        SetMaterialKeyword("SPIN_ON", spin, temporarySerializableMaterial);
        SetMaterialKeyword("TURBULENCE_SIMPLEX2D", turbulenceType == TurbulenceType.Simplex2D, temporarySerializableMaterial);
        SetMaterialKeyword("TURBULENCE_SIMPLEX", turbulenceType == TurbulenceType.Simplex, temporarySerializableMaterial);
        SetMaterialKeyword("SOFTPARTICLES_ON", softParticles == SoftParticleType.NearClipAndCameraDepth, temporarySerializableMaterial);
		SetMaterialKeyword("SHAPE_SPHERE", shape == Shape.Sphere, temporarySerializableMaterial);
		SetMaterialKeyword("SHAPE_CYLINDER", shape == Shape.Cylinder, temporarySerializableMaterial);
#endif
	}

#if UNITY_EDITOR
    [UnityEditor.Callbacks.PostProcessScene]
    static void InitShaderOptions()
    {
        PAParticleField[] papfs = (PAParticleField[])Resources.FindObjectsOfTypeAll(typeof(PAParticleField));
 
        List<PAParticleField> papfsInScene = new List<PAParticleField>();

        for (int i = 0; i < papfs.Length; i++) {
            PAParticleField papf = papfs[i];
            if (papf.hideFlags == HideFlags.NotEditable || papf.hideFlags == HideFlags.HideAndDontSave) {
                continue;
            }

            string sAssetPath = UnityEditor.AssetDatabase.GetAssetPath(papf.transform.root.gameObject);
            if (!string.IsNullOrEmpty(sAssetPath)) {
                continue;
            }

            papfsInScene.Add(papf);
        }

        for (int i = 0; i < papfsInScene.Count; i++)
        {
            //Update the shader to serialize the shader options
            papfsInScene[i].CreateTemporarySerializableMaterial();
            //clear cache if desired
            if (papfsInScene[i].clearCacheInBuilds && papfsInScene[i].m_MeshGenerator){
                papfsInScene[i].meshGenerator.ClearCache();
            }else if (papfsInScene[i].m_MeshGenerator){
				papfsInScene[i].meshGenerator.CheckDataStructureVersion();
			}
        }
    }
#endif
}

[System.Flags]
public enum MeshFlags{
	None,
	Generator	= 1<<0,
	Count		= 1<<1,
	Color		= 1<<2,
	Speed		= 1<<3,
	Surface		= 1<<4,
	Seed = (MeshFlags.Color | MeshFlags.Speed | MeshFlags.Surface),
	All = (MeshFlags.Seed | MeshFlags.Count | MeshFlags.Generator)
}