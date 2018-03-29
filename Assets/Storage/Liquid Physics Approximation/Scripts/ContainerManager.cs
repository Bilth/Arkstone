using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif 
using System;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
[DisallowMultipleComponent]

[RequireComponent (typeof (MeshFilter))]
[RequireComponent (typeof (MeshRenderer))]
[RequireComponent (typeof (SphereCollider))]

public class ContainerManager : MonoBehaviour {
	public bool UsePhysicsAnimation = true;
	public bool UseStaticAnimation;

	[Serializable] public class LiquidBehaviorClass {
		[Range(0, 1)] public float Amount = .5f;

		public Vector3 Pivot = new Vector3(0, 0, 0);

		[Serializable] public class OnPhysicsAnimationClass {
			[Range(1, 5)] public float DensityMoving = 5;
			[Range(1, 5)] public float DensityDirection = 2;
			[Range(.1f, 10)] public float Weight = 5;
		}
		public OnPhysicsAnimationClass OnPhysicsAnimation;

		[Serializable] public class OnStaticAnimationClass {
			[Range(.1f, .5f)] public float SliceHeight = .2f;
			[Range(-10, 10)] public float SliceSpeed = .3f;
		}
		public OnStaticAnimationClass OnStaticAnimation;

		public bool AddBubbles = false;
	}
	public LiquidBehaviorClass LiquidBehavior;

	[Serializable] public class ColorsClass {
		[Serializable] public class GlassClass {
			public enum GlassSideClass {OneSide, DoubleSide};

			public GlassSideClass GlassSide;
			public Color Color = new Color(1, 1, 1, 1);
			[Range(0, 1)] public float Trasparency = 1;
			[Range(0, 1)] public float Opacity = .8f;
			[Range(.5f, 5)] public float FresnelOpacity = 1.5f;
			[Range(.1f, 1)] public float Reflectivity = .5f;
			public bool AddTranslucency;
		}
		public GlassClass Glass;

		[Serializable] public class LiquidClass {
			public Texture2D Gradient;
			public Color Color = new Color(1, 1, 1, 1);
			[Range(0, 1)] public float Trasparency = 1;
		}
		public LiquidClass Liquid;

		[Serializable] public class BubblesClass {
			[Range(0, 1)] public float Trasparency = 1;
		}
		public BubblesClass Bubbles;
	}
	public ColorsClass Colors;

	[Serializable] public class PatternsClass {
		[Serializable] public class GlassMainClass {
			public Texture2D Pattern;
			public int Tiling = 1;
		}
		public GlassMainClass GlassMain;

		[Serializable] public class GlassTranslucencyClass {
			public Texture2D Pattern;
			public int Tiling = 1;
			[Range(-1, 1)] public float SpeedX = .1f;
			[Range(-1, 1)] public float SpeedY = .1f;
		}
		public GlassTranslucencyClass GlassTranslucency;

		[Serializable] public class LiquidClass {
			public Texture2D StaticAnimationSlicePattern;
			[Range(.1f, 10)] public float Tiling = .7f;
		}
		public LiquidClass Liquid;

		[Serializable] public class LiquidBubblesClass {
			public Texture2D Pattern;
			[Range(1, 30)] public int Tiling = 5;
			public Vector2 Speed = new Vector2 (.1f, .1f);
			[Range(0, 1)] public float Density = .5f;
		}
		public LiquidBubblesClass LiquidBubbles;
	}
	public PatternsClass Patterns;

	private GameObject VelocityDirObj;
	private GameObject BubblesObj;
	private Material BubblesObjMaterial;
	private bool SwitchStateDisableFlag;
	private Material[] ContainerMaterials = new Material[4];
	private Mesh InitMesh;
	private Vector3 InitWorldScale;
	private Vector3 PreviousVelocity;
	private Vector3 Velocity;
	private float RotationDirX;
	private float RotationDirZ;
	private float RotationDirW;
	private Vector3 PreviousObjPos;
	private float LocalTimeX;
	private float LocalTimeZ;
	private float InitReposDirX;
	private float InitReposDirZ;
	private float X;
	private float Z;
	
	void OnEnable () {
		//Generate container materials
		if (!ContainerMaterials [0])
			ContainerMaterials [0] = new Material (Shader.Find ("Liquid Physics Approximation/Front liquid"));
		if (!ContainerMaterials [1])
			ContainerMaterials [1] = new Material (Shader.Find ("Liquid Physics Approximation/Back liquid"));
		if (!ContainerMaterials [2])
			ContainerMaterials [2] = new Material (Shader.Find ("Liquid Physics Approximation/Front glass"));
		if (!ContainerMaterials [3])
			ContainerMaterials [3] = new Material (Shader.Find ("Liquid Physics Approximation/Back glass"));
		
		//Generate material for bubbles emission gameobject
		if (!BubblesObjMaterial)
			BubblesObjMaterial = new Material (Shader.Find ("Liquid Physics Approximation/Liquid bubbles"));

		//Search trace of required objects for effect, and fill the relative variables if these are already existing on the current Editor instance
		for (int i = 0; i <= transform.childCount; i++) {
			if (transform.GetComponentsInChildren<Transform> () [i].name.Contains ("BubblesObj"))
				BubblesObj = transform.GetComponentsInChildren<Transform> () [i].gameObject;
			
			if (transform.GetComponentsInChildren<Transform> () [i].name.Contains ("VelocityDirObj"))
				VelocityDirObj = transform.GetComponentsInChildren<Transform> () [i].gameObject;
		}
		
		#if UNITY_EDITOR
			//Enable editor update on a specific function
			EditorApplication.update = null;
			if (!EditorApplication.isPlaying)
				EditorApplication.update = EditorUpdate;
		#endif
	}

	void EditorUpdate () {
		try {
			LiquidMove ();
		} catch {}
	}

	void FixedUpdate () {
		LiquidMove ();
	}

	void LiquidMove () {
		if (!VelocityDirObj) {
			VelocityDirObj = new GameObject (transform.name + " VelocityDirObj");

			VelocityDirObj.transform.parent = transform;
		}

		if (!SwitchStateDisableFlag) {
			if (!UseStaticAnimation) {
				UseStaticAnimation = false;

				UsePhysicsAnimation = true;
			} else {
				UseStaticAnimation = true;

				UsePhysicsAnimation = false;

				SwitchStateDisableFlag = true;
			}
		} else {
			if (!UsePhysicsAnimation) {
				UsePhysicsAnimation = false;

				UseStaticAnimation = true;
			} else {
				UsePhysicsAnimation = true;

				UseStaticAnimation = false;

				SwitchStateDisableFlag = false;
			}
		}

		if (Patterns.GlassMain.Tiling < 1)
			Patterns.GlassMain.Tiling = 1;
		if (Patterns.GlassTranslucency.Tiling < 1)
			Patterns.GlassTranslucency.Tiling = 1;
		if (Patterns.LiquidBubbles.Tiling < 1)
			Patterns.LiquidBubbles.Tiling = 1;

		if (GetComponent<MeshFilter> ().sharedMesh != InitMesh) {
			Quaternion LolRotation = transform.rotation;

			transform.rotation = new Quaternion (0, 0, 0, 0);

			InitWorldScale = GetComponent<MeshRenderer> ().bounds.size;

			transform.rotation = LolRotation;

			InitMesh = GetComponent<MeshFilter> ().sharedMesh;
		}

		//Generate gameobject for bubbles emission
		if (!BubblesObj) {
			BubblesObj = new GameObject (transform.name + " BubblesObj");

			BubblesObj.AddComponent<MeshFilter> ();
			BubblesObj.AddComponent<MeshRenderer> ();

			BubblesObj.GetComponent<MeshRenderer> ().enabled = false;

			BubblesObj.GetComponent<MeshFilter> ().sharedMesh = GetComponent<MeshFilter> ().sharedMesh;
			BubblesObj.GetComponent<MeshRenderer> ().sharedMaterial = BubblesObjMaterial;

			BubblesObj.transform.position = transform.position;
			BubblesObj.transform.parent = transform;

			BubblesObj.transform.localScale = new Vector3 (.95f, 1, .95f);
		} else {
			Vector3 MainCamPos = Camera.main.transform.position;
			MainCamPos.y = 0;
			BubblesObj.transform.LookAt (MainCamPos);

			BubblesObj.GetComponent<MeshFilter> ().sharedMesh = GetComponent<MeshFilter> ().sharedMesh;

			BubblesObj.GetComponent<MeshRenderer> ().enabled = LiquidBehavior.AddBubbles;
		}

		//Liquid direction math
		if (LiquidBehavior.Amount > 0 && LiquidBehavior.Amount < 1) {
			if (!UseStaticAnimation) {
				Vector3 VelocityMath = (transform.position - PreviousVelocity) / Time.fixedDeltaTime;

				Velocity = new Vector3 (VelocityMath.x, VelocityMath.y, VelocityMath.z);
				PreviousVelocity = transform.position;

				float MaxAxisVelocity = .7f;
				float LiquidDensityMultiplicator = 10f;

				if (Mathf.Abs (Velocity.x) > MaxAxisVelocity) {
					RotationDirX = Mathf.Lerp (RotationDirX, (-Velocity.x / LiquidBehavior.OnPhysicsAnimation.DensityMoving) / 2, 5 * Time.deltaTime);

					InitReposDirX = 0;

					LocalTimeX = Time.time;
				} else {
					if (Mathf.Abs (Velocity.z) < MaxAxisVelocity) {
						X = (Time.time - LocalTimeX) * LiquidBehavior.OnPhysicsAnimation.Weight;

						if (RotationDirX != 0 && InitReposDirX == 0)
							InitReposDirX = RotationDirX > 0 ? -1 : 1;

						if (LocalTimeX > 0) {
							if (X > LiquidBehavior.OnPhysicsAnimation.Weight * LiquidDensityMultiplicator)
								RotationDirX = Mathf.Lerp (RotationDirX, 0, LiquidBehavior.OnPhysicsAnimation.Weight * Time.deltaTime);
							else
								RotationDirX = Mathf.Lerp (RotationDirX, ((Mathf.Sin (X * 2) / (X * 2 + .1f)) / 2) * InitReposDirX, LiquidBehavior.OnPhysicsAnimation.Weight * Time.deltaTime);
						} else
							RotationDirX = 0;
					}
				}
				if (Mathf.Abs (Velocity.z) > MaxAxisVelocity) {
					RotationDirZ = Mathf.Lerp (RotationDirZ, (-Velocity.z / LiquidBehavior.OnPhysicsAnimation.DensityMoving) / 2, 5 * Time.deltaTime);

					InitReposDirZ = 0;

					LocalTimeZ = Time.time;
				} else {
					if (Mathf.Abs (Velocity.x) < MaxAxisVelocity) {
						Z = (Time.time - LocalTimeZ) * LiquidBehavior.OnPhysicsAnimation.Weight;

						if (RotationDirZ != 0 && InitReposDirZ == 0)
							InitReposDirZ = RotationDirZ > 0 ? -1 : 1;

						if (LocalTimeZ > 0) {
							if (Z > LiquidBehavior.OnPhysicsAnimation.Weight * LiquidDensityMultiplicator)
								RotationDirZ = Mathf.Lerp (RotationDirZ, 0, LiquidBehavior.OnPhysicsAnimation.Weight * Time.deltaTime);
							else
								RotationDirZ = Mathf.Lerp (RotationDirZ, ((Mathf.Sin (Z * 2) / (Z * 2 + .1f)) / 2) * InitReposDirZ, LiquidBehavior.OnPhysicsAnimation.Weight * Time.deltaTime);
						} else
							RotationDirZ = 0;
					}
				}

				if (VelocityDirObj && Velocity != Vector3.zero)
					VelocityDirObj.transform.rotation = Quaternion.LookRotation (Velocity);
			}

			RotationDirW = Mathf.MoveTowards (RotationDirW, (Mathf.Round (((!UseStaticAnimation ? VelocityDirObj.transform.eulerAngles.y : Camera.main.transform.eulerAngles.y) / 57) * 100) / 100) - (!UseStaticAnimation ? 1.75f : 1.55f), 50 * Time.deltaTime);
		} else {
			RotationDirX = 0;
			RotationDirZ = 0;
			RotationDirW = 0;
		}

		//Set gameobject materials
		if (Colors.Glass.GlassSide == ColorsClass.GlassClass.GlassSideClass.OneSide) {
			Material[] LocalContainerMaterial = ContainerMaterials;
		
			Array.Resize (ref LocalContainerMaterial, LocalContainerMaterial.Length - 1);
		
			GetComponent<MeshRenderer> ().sharedMaterials = LocalContainerMaterial;
		} else
			GetComponent<MeshRenderer> ().sharedMaterials = ContainerMaterials;

		//Set shaders properties
		float WorldScale = (GetComponent<MeshRenderer> ().bounds.size.y - Mathf.Lerp (0, Mathf.Lerp ((GetComponent<MeshRenderer> ().bounds.size.x - InitWorldScale.x) + (GetComponent<MeshRenderer> ().bounds.size.y - InitWorldScale.y), 0, LiquidBehavior.Amount * 10), (1 - LiquidBehavior.Amount) * 10)) + .025f;

		float LessOffsetRecalc = 1;

		for (int i = 0; i < 3; i++) {
			if (i < 2) {
				LessOffsetRecalc = -.5f * (WorldScale - (UseStaticAnimation && LiquidBehavior.Amount == 0 ? 1 - ContainerMaterials [i].GetFloat ("_StaticAnimationSliceHeight") : 1));

				ContainerMaterials [i].SetColor ("_Color", Colors.Liquid.Color);
				ContainerMaterials [i].SetFloat ("_Trasparency", Colors.Liquid.Trasparency);
				ContainerMaterials [i].SetTexture ("_Gradient", Colors.Liquid.Gradient);

				ContainerMaterials [i].SetVector ("_Pivot", LiquidBehavior.Pivot);
				ContainerMaterials [i].SetFloat ("_RotationX", RotationDirX);
				ContainerMaterials [i].SetFloat ("_RotationZ", RotationDirZ);
				ContainerMaterials [i].SetFloat ("_RotationW", RotationDirW);

				ContainerMaterials [i].SetFloat ("_Amount", Mathf.Lerp (LessOffsetRecalc, WorldScale + LessOffsetRecalc, LiquidBehavior.Amount));

				ContainerMaterials [i].SetFloat ("_EnableStaticAnimation", UseStaticAnimation ? 1 : 0);
				ContainerMaterials [i].SetFloat ("_Tiling", Patterns.Liquid.Tiling);
				ContainerMaterials [i].SetFloat ("_StaticAnimationSliceHeight", LiquidBehavior.OnStaticAnimation.SliceHeight);
				ContainerMaterials [i].SetFloat ("_StaticAnimationSliceSpeed", LiquidBehavior.OnStaticAnimation.SliceSpeed);

				ContainerMaterials [i].SetFloat ("_SliceAngleCurvature", ((Mathf.Max (Mathf.Abs (RotationDirX), Mathf.Abs (RotationDirZ)) * GetComponent<MeshRenderer> ().bounds.size.y) / LiquidBehavior.OnPhysicsAnimation.DensityDirection) * -1);

				ContainerMaterials [i].SetTexture ("_StaticAnimationSlicePattern", Patterns.Liquid.StaticAnimationSlicePattern);
			} else {
				ContainerMaterials [i].SetColor ("_Color", Colors.Glass.Color);
				ContainerMaterials [i].SetFloat ("_Trasparency", Colors.Glass.Trasparency);
				ContainerMaterials [i].SetFloat ("_Opacity", Colors.Glass.Opacity);
				ContainerMaterials [i].SetFloat ("_FresnelOpacity", Colors.Glass.FresnelOpacity);
				ContainerMaterials [i].SetInt ("_AddedFresnel", !Patterns.GlassMain.Pattern ? 0 : 1);

				ContainerMaterials [i].SetTexture ("_MainPattern", Patterns.GlassMain.Pattern);
				ContainerMaterials [i].SetInt ("_MainPatternTiling", Patterns.GlassMain.Tiling);

				ContainerMaterials [i].SetInt ("_AddTranslucencyPattern", Colors.Glass.AddTranslucency ? 1 : 0);
				ContainerMaterials [i].SetTexture ("_TranslucencyPattern", Patterns.GlassTranslucency.Pattern);
				ContainerMaterials [i].SetInt ("_TranslucencyPatternTiling", Patterns.GlassTranslucency.Tiling);
				ContainerMaterials [i].SetFloat ("_TranslucencyPatternSpeedX", Patterns.GlassTranslucency.SpeedX);
				ContainerMaterials [i].SetFloat ("_TranslucencyPatternSpeedY", Patterns.GlassTranslucency.SpeedY);

				ContainerMaterials [i].SetFloat ("_Reflectivity", Colors.Glass.Reflectivity);
			}
		}

		BubblesObjMaterial.SetVector ("_Pivot", LiquidBehavior.Pivot);
		BubblesObjMaterial.SetFloat ("_RotationX", RotationDirX);
		BubblesObjMaterial.SetFloat ("_RotationZ", RotationDirZ);
		BubblesObjMaterial.SetFloat ("_RotationW", RotationDirW);

		BubblesObjMaterial.SetFloat ("_Amount", Mathf.Lerp (LessOffsetRecalc, WorldScale + LessOffsetRecalc, LiquidBehavior.Amount));

		BubblesObjMaterial.SetTexture ("_Pattern", Patterns.LiquidBubbles.Pattern);
		BubblesObjMaterial.SetColor ("_LiquidColor", Colors.Liquid.Color);
		BubblesObjMaterial.SetFloat ("_PatternTrasparency", Colors.Bubbles.Trasparency);
		BubblesObjMaterial.SetInt ("_PatternTiling", Patterns.LiquidBubbles.Tiling);
		BubblesObjMaterial.SetFloat ("_PatternSpeedX", Patterns.LiquidBubbles.Speed.x);
		BubblesObjMaterial.SetFloat ("_PatternSpeedZ", Patterns.LiquidBubbles.Speed.y);
		BubblesObjMaterial.SetFloat ("_PatternDensity", Patterns.LiquidBubbles.Density);

		BubblesObjMaterial.SetFloat ("_SliceAngleCurvature", ((Mathf.Max (Mathf.Abs (RotationDirX), Mathf.Abs (RotationDirZ)) * GetComponent<MeshRenderer> ().bounds.size.y) / LiquidBehavior.OnPhysicsAnimation.DensityDirection) * -1);
	}
}