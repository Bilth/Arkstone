using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(PAParticleField))]
[CanEditMultipleObjects]
public class PAParticleFieldInspector : Editor {

	//Max particle count = 65000/4
	const int MAX_PARTICLE_COUNT = 16250;	
	const float WARNING_THRESHOLD = 0.3f;

	private float defaultLabelWidth;
	private float defaultFieldWidth;

	private bool meshParticleIsDirty = false;

	SerializedProperty seedProp;

	SerializedProperty particleTypeProp;

	SerializedProperty generatorProp;
	SerializedProperty inputMeshProp;

	SerializedProperty countProp;
    SerializedProperty countMaskProp;
	SerializedProperty fieldSizeProp;
	SerializedProperty simulationSpaceProp;

	SerializedProperty shapeProp;
	SerializedProperty edgeModeProp;
	SerializedProperty edgeThresholdProp;

	SerializedProperty exclusionZoneProp;
	SerializedProperty exclusionOverrideProp;

	SerializedProperty forceProp;
	SerializedProperty particleSizeProp;
	SerializedProperty speedProp;
	SerializedProperty speedMaskProp;
	SerializedProperty colorProp;
	SerializedProperty stretchedBillboardProp;
	SerializedProperty scaleSpeedMultiplierProp;
	SerializedProperty spinProp;
	SerializedProperty spinSpeedProp;
    SerializedProperty customRotationAxisProp;
    SerializedProperty rotationAxisProp;
	SerializedProperty customFacingDirectionProp;
	SerializedProperty facingDirectionProp;
	SerializedProperty customUpProp;
	SerializedProperty upProp;

	SerializedProperty softParticlesProp;
	SerializedProperty nearFadeDistanceProp;
	SerializedProperty nearFadeOffsetProp;
	SerializedProperty softnessProp;

    SerializedProperty turbulenceTypeProp;
    SerializedProperty turbulenceFrequencyProp;
    SerializedProperty turbulenceAmplitudeProp;
    SerializedProperty turbulenceScaleProp;
    SerializedProperty turbulenceOffsetProp;

	SerializedProperty colorVariationProp;

	SerializedProperty materialTypeProp;
	SerializedProperty materialProp;

	SerializedProperty textureTypeProp;
	SerializedProperty textureProp;
	SerializedProperty pivotProp;
	SerializedProperty spriteColumnsProp;
	SerializedProperty spriteRowsProp;
	SerializedProperty framerateProp;
	SerializedProperty cutOffProp;

	GUIStyle style{
		get{
#if UNITY_4_3
			return EditorStyles.textField;
#else
			return EditorStyles.textArea;
#endif
		}
	}

	int spacing{
		get{
			#if UNITY_4_3
			return 2;
			#else
			return (int)EditorGUIUtility.standardVerticalSpacing;
			#endif
		}
	}

	[MenuItem("GameObject/Create Other/PA Particle Field")]
	static void CreateField(){
		PAParticleField.Create ("New PAParticleField");
	}

	void OnEnable(){
		mHandlePositions = new Vector3[6];

		seedProp = serializedObject.FindProperty("mSeed");

		particleTypeProp = serializedObject.FindProperty ("mGeneratorType");

		inputMeshProp = serializedObject.FindProperty ("mInputMesh");

		countProp = serializedObject.FindProperty ("mParticleCount");
        countMaskProp = serializedObject.FindProperty("mParticleCountMask");

		fieldSizeProp = serializedObject.FindProperty ("mFieldSize");
		simulationSpaceProp = serializedObject.FindProperty ("mSimulationSpace");

		shapeProp = serializedObject.FindProperty ("mShape");
		edgeModeProp = serializedObject.FindProperty ("mEdgeMode");
		edgeThresholdProp = serializedObject.FindProperty("mEdgeThreshold");

		exclusionZoneProp = serializedObject.FindProperty ("mUseExclusionZones");
		exclusionOverrideProp = serializedObject.FindProperty ("mExclusionAnchorOverride");

		forceProp = serializedObject.FindProperty ("mForce");
		particleSizeProp = serializedObject.FindProperty ("mParticleSize");
		speedProp = serializedObject.FindProperty ("mSpeed");
		speedMaskProp = serializedObject.FindProperty ("mSpeedMask");
		colorProp = serializedObject.FindProperty ("mColor");
		
		stretchedBillboardProp = serializedObject.FindProperty ("mStretchedBillboard");
		scaleSpeedMultiplierProp = serializedObject.FindProperty ("mSpeedScaleMultiplier");
		spinProp = serializedObject.FindProperty("mSpin");
		spinSpeedProp = serializedObject.FindProperty("mSpinSpeed");
		customFacingDirectionProp = serializedObject.FindProperty("mCustomFacingDirection");
		facingDirectionProp = serializedObject.FindProperty("mFacingDirection");
		customUpProp = serializedObject.FindProperty("mCustomUpDirection");
		upProp = serializedObject.FindProperty("mUpDirection");
        customRotationAxisProp = serializedObject.FindProperty("mCustomRotationAxis");
        rotationAxisProp = serializedObject.FindProperty("mRotationAxis");

		softParticlesProp = serializedObject.FindProperty("mSoftParticles");
		nearFadeDistanceProp = serializedObject.FindProperty ("mNearFadeDistance");
		nearFadeOffsetProp = serializedObject.FindProperty ("mNearFadeOffset");
		softnessProp = serializedObject.FindProperty("mSoftness");

        turbulenceTypeProp = serializedObject.FindProperty("mTurbulenceType");
        turbulenceFrequencyProp = serializedObject.FindProperty("mTurbulenceFrequency");
        turbulenceAmplitudeProp = serializedObject.FindProperty("mTurbulenceAmplitude");
        turbulenceScaleProp = serializedObject.FindProperty("mTurbulenceScale");
        turbulenceOffsetProp = serializedObject.FindProperty("mTurbulenceOffsetSpeed");
		
		colorVariationProp = serializedObject.FindProperty ("mColorVariation");

		materialTypeProp = serializedObject.FindProperty("mMaterialType");

		materialProp = serializedObject.FindProperty("material");

		textureTypeProp = serializedObject.FindProperty ("mTextureType");
		textureProp = serializedObject.FindProperty ("mTexture");
		pivotProp = serializedObject.FindProperty("mPivotOffset");
		spriteColumnsProp = serializedObject.FindProperty ("mSpriteColumns");
		spriteRowsProp = serializedObject.FindProperty ("mSpriteRows");
		framerateProp = serializedObject.FindProperty ("mFramerate");
		cutOffProp = serializedObject.FindProperty("mCutOff");
	}

	void OnDisable(){
		if (meshParticleIsDirty == true) {
			MarkMeshAsDirty (serializedObject);
			meshParticleIsDirty = false;
			serializedObject.Update ();
		}
	}

	public override void OnInspectorGUI ()
	{
		defaultLabelWidth = EditorGUIUtility.labelWidth;
		defaultFieldWidth = EditorGUIUtility.fieldWidth;

		EditorGUIUtility.fieldWidth = 5f;

		serializedObject.Update ();

		PAParticleField pf = target as PAParticleField;

		EditorGUILayout.Space ();

		DrawFieldProperties (pf);

		DrawExclusionZoneProperties (pf);

		DrawGlobalParticleProperties (pf);

        DrawTurbulenceProperites(pf);

		DrawPerParticleProperties (pf);

		DrawSpriteProperties (pf);

		EditorGUIUtility.fieldWidth = defaultFieldWidth;
		EditorGUIUtility.labelWidth = defaultLabelWidth;

		if (softParticlesProp.enumValueIndex == 2) {
			EditorGUILayout.HelpBox("CameraDepth Soft Particles require the camera to generate a depth texture", MessageType.Info);
		}

		if (ShowFillRateWarning(pf)){
			EditorGUILayout.HelpBox("The current settings could cause exessive overdraw and may impact performance.\n\nConsider reducing the size of the particles, increasing the field size or reducing the number of particles.", MessageType.Warning);
		}

		serializedObject.ApplyModifiedProperties ();
	}

    static bool drawFieldProperties = true;

	void DrawFieldProperties(PAParticleField pf){

        DrawControlBox("Field Properties", "field-properties", ref drawFieldProperties,
            () =>
            {
                FlaggedLayoutPropertyField(seedProp, new GUIContent("Random Seed", "The random seed used to generate the mesh"), MeshFlags.Seed);

                //Draw the particle count
                EditorGUILayout.BeginHorizontal();
                FlaggedLayoutPropertyField(countProp, new GUIContent("Particle Count", "The number of particles in the field"), MeshFlags.Count);
                countProp.intValue = Mathf.Clamp(countProp.intValue, 0, pf.GetMaxCount());
                EditorGUIUtility.labelWidth = 1f;
                EditorGUILayout.LabelField(new GUIContent("/" + pf.GetMaxCount()));
                EditorGUIUtility.labelWidth = 0f;
                EditorGUILayout.EndHorizontal();

                bool countMaskNotSupported = particleTypeProp.enumValueIndex == 1; // count mask isnt supported on mesh paticles currently
                FlaggedLayoutPropertyField(countMaskProp, new GUIContent("Particle Count Mask", "Gives the appearance of changing the particle count without regenerating the mesh. Does not reduce memory but also doesn't allocate garbage. (Billboard particles only)"), MeshFlags.None, true, countMaskNotSupported);
                countMaskProp.floatValue = Mathf.Clamp01(countMaskProp.floatValue);

                int particleTypeIndex = particleTypeProp.enumValueIndex; //0 billboard, 1 mesh, 2 custom
                FlaggedLayoutPropertyField(particleTypeProp, new GUIContent("Particle Type", "Defines what type of particle will be drawn"), MeshFlags.All, true);
                //if changing particle type
                if (particleTypeProp.enumValueIndex != particleTypeIndex) {
                    // if its not a custom material (material type 6)
                    // and its not a custom generator (generator type 2
                    // then assign the default material for that particle type
                    if (materialTypeProp.enumValueIndex != 6 && particleTypeProp.enumValueIndex != 2) {
                        UpdateMaterialType(serializedObject, (particleTypeProp.enumValueIndex == 0 ? 0 : 7)); // (0 = billboad transparent, 7 = mesh default)
                    }
                }

                if (particleTypeProp.enumValueIndex == 1) {
                    EditorGUI.indentLevel += 1;
                    FlaggedLayoutPropertyField(inputMeshProp, new GUIContent("Mesh", "The mesh used as a base for the mesh particle system"), MeshFlags.All, true);
                    EditorGUI.indentLevel--;
                }

                FlaggedLayoutPropertyField(simulationSpaceProp, new GUIContent("Simulation Space", "Simulate particles in the transforms local/world space"), MeshFlags.None, true);

                //Draw the field size
                EditorGUILayout.LabelField(new GUIContent("Field Size", "The bounds of the field"));
                FlaggedLayoutPropertyField(fieldSizeProp, new GUIContent(""), MeshFlags.None, true);

                EditorGUILayout.Space();

                FlaggedLayoutPropertyField(shapeProp, new GUIContent("Shape", "The shape the particles form inside the bounds"), MeshFlags.None, true);
                FlaggedLayoutPropertyField(edgeModeProp, new GUIContent("Edge Mode", "Defines the way particles act at the edge of the shape"), MeshFlags.None, true);

                serializedObject.ApplyModifiedProperties();

                //Draw the field size
                EditorGUILayout.LabelField(new GUIContent("Edge Threshold", "Threshold for edge effects (alpha and scale)"));
                if (pf.shape == PAParticleField.Shape.Cube) {
                    FlaggedLayoutPropertyField(edgeThresholdProp, new GUIContent(""), MeshFlags.None, true);
                } else if (pf.shape == PAParticleField.Shape.Sphere) {
                    float width = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth = 13f;
                    float threshold = EditorGUILayout.FloatField("X", pf.edgeThreshold.x);
                    EditorGUIUtility.labelWidth = width;
                    pf.edgeThreshold = new Vector3(threshold, pf.edgeThreshold.y, pf.edgeThreshold.z);
                    serializedObject.Update();
                } else if (pf.shape == PAParticleField.Shape.Cylinder) {
                    Vector2 threshold = EditorGUILayout.Vector2Field("", new Vector2(pf.edgeThreshold.x, pf.edgeThreshold.y));
                    pf.edgeThreshold = new Vector3(threshold.x, threshold.y, pf.edgeThreshold.z);
                    serializedObject.Update();
                }

                Vector3 edgeThreshold = edgeThresholdProp.vector3Value;
                edgeThreshold.x = Mathf.Clamp01(edgeThreshold.x);
                edgeThreshold.y = Mathf.Clamp01(edgeThreshold.y);
                edgeThreshold.z = Mathf.Clamp01(edgeThreshold.z);
                edgeThresholdProp.vector3Value = edgeThreshold;

                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("clearCacheInBuilds"), new GUIContent("Rebuild At Runtime", "If true the particle cache will be cleared, reducing the size of the scene but increasing start up time"));
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();

            }
        );
	}

    static bool drawExclusionZones = true;

	void DrawExclusionZoneProperties(PAParticleField pf){

        DrawControlBox("Exclusion Zones", "exclusion-zones", ref drawExclusionZones,
            () =>
            {
                FlaggedLayoutPropertyField(exclusionZoneProp, new GUIContent("Use Exclusion Zones", "When on exclusion zones in the fields bounds will hide particles inside them"), MeshFlags.None, true);
                FlaggedLayoutPropertyField(exclusionOverrideProp, new GUIContent("Anchor Override", "Used to prioritize and sort exclusion zones"), MeshFlags.None, true);
            }
        );
	}

    static bool drawGlobalProperties = true;

	void DrawGlobalParticleProperties(PAParticleField pf){

        DrawControlBox("Global Particle Properties", "global-properties", ref drawGlobalProperties,
            () =>
            {
                //draw the size
                FlaggedLayoutPropertyField(particleSizeProp, new GUIContent("Particle Size", "The base size applied to every particle"), MeshFlags.None, true);

                //Draw the color
                FlaggedLayoutPropertyField(colorProp, new GUIContent("Color", "The clobal color of the particles"), MeshFlags.None, true, materialTypeProp.enumValueIndex == 6);

                //draw the speed
                FlaggedLayoutPropertyField(speedProp, new GUIContent("Move Speed", "The base speed applied to every particle"), MeshFlags.None, true);

                EditorGUILayout.LabelField(new GUIContent("Speed Mask", "Allows masking or amplifying of speed along the x y z axis"));
                FlaggedLayoutPropertyField(speedMaskProp, new GUIContent(""), MeshFlags.None, true);

                //Draw the global force
                EditorGUILayout.LabelField(new GUIContent("Force", "A directional force aplpied to every particle"));
                FlaggedLayoutPropertyField(forceProp, new GUIContent(""), MeshFlags.None, true);

                EditorGUILayout.Space();

                //Start drawing scale by speed
                EditorGUIUtility.labelWidth = 100f;
                EditorGUILayout.BeginHorizontal();

				bool lastBool = stretchedBillboardProp.boolValue;
                FlaggedLayoutPropertyField(stretchedBillboardProp, new GUIContent("Stretch/Directional", "when true the particle will be scaled/oriented in the direction its moving"), MeshFlags.None, true);

                if (stretchedBillboardProp.boolValue != lastBool && !lastBool) {
                    spinProp.boolValue = false;				
					customFacingDirectionProp.boolValue = false;
					if (particleTypeProp.enumValueIndex == 0){
	                    customUpProp.boolValue = false;
					}
                }

                EditorGUI.BeginDisabledGroup(!stretchedBillboardProp.boolValue);
                FlaggedLayoutPropertyField(scaleSpeedMultiplierProp, new GUIContent("Stretch Multiplier", "Amount to scale by speed/direction"), MeshFlags.None, true, particleTypeProp.enumValueIndex != 0);
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
				lastBool = spinProp.boolValue;
                FlaggedLayoutPropertyField(spinProp, new GUIContent("Spin", "When enabled the particle will spin"), MeshFlags.None, true);

                if (spinProp.boolValue != lastBool && !lastBool) {
                    stretchedBillboardProp.boolValue = false;
                    customUpProp.boolValue = false;
					customFacingDirectionProp.boolValue = false;
                }

                EditorGUI.BeginDisabledGroup(!spinProp.boolValue);
                FlaggedLayoutPropertyField(spinSpeedProp, new GUIContent("Spin Speed", "The maximum speed the particles will spin at"), MeshFlags.None, true);
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.EndHorizontal();

				lastBool = customFacingDirectionProp.boolValue;
                DrawDisableableVector3(customFacingDirectionProp, facingDirectionProp, "Face Direction", "When true the particle will face the specified direction instead of the camera", MeshFlags.None, true);
				if (customFacingDirectionProp.boolValue != lastBool && !lastBool){
					spinProp.boolValue = false;
					stretchedBillboardProp.boolValue = false;
				}
				
                DrawDisableableVector3(customUpProp, upProp, "Up Direction", "When true customize the up direction of the particles", MeshFlags.None, true);
                if (customUpProp.boolValue) {
					if (particleTypeProp.enumValueIndex == 0){
	                    stretchedBillboardProp.boolValue = false;
					}
                    spinProp.boolValue = false;
                }

                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();
                EditorGUIUtility.fieldWidth = defaultFieldWidth;
                EditorGUIUtility.labelWidth = defaultLabelWidth;

                FlaggedLayoutPropertyField(softParticlesProp, new GUIContent("Soft Particles", "When true particles will fade off as they approach solid objects [Requires deferred rendering or camera depth texture]"), MeshFlags.None, true, particleTypeProp.enumValueIndex != 0);
                EditorGUILayout.EndHorizontal();

                EditorGUIUtility.fieldWidth = 20f;
                EditorGUIUtility.labelWidth = 100f;

                EditorGUILayout.BeginHorizontal();
				EditorGUI.BeginDisabledGroup(softParticlesProp.enumValueIndex <= 0);
			FlaggedLayoutPropertyField(nearFadeDistanceProp, new GUIContent("Fade Distance"), MeshFlags.None, true, particleTypeProp.enumValueIndex != 0);
			FlaggedLayoutPropertyField(nearFadeOffsetProp, new GUIContent("Fade Offset"), MeshFlags.None, true, particleTypeProp.enumValueIndex != 0);
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.EndHorizontal();
				EditorGUI.BeginDisabledGroup(softParticlesProp.enumValueIndex <= 1);
			FlaggedLayoutPropertyField(softnessProp, new GUIContent("Softness"), MeshFlags.None, true, particleTypeProp.enumValueIndex != 0);
                EditorGUI.EndDisabledGroup();
            }
        );
	}

	void DrawDisableableVector3(SerializedProperty boolVal, SerializedProperty vectorVal, string name, string tooltip, MeshFlags mesh = MeshFlags.None, bool shader = false){
		EditorGUILayout.BeginHorizontal ();
		FlaggedLayoutPropertyField (boolVal, new GUIContent (name, tooltip), mesh, shader);
		
		EditorGUI.BeginDisabledGroup(!boolVal.boolValue);
		FlaggedLayoutPropertyField (vectorVal, new GUIContent (""), mesh, shader);
		EditorGUI.EndDisabledGroup();
		EditorGUILayout.EndHorizontal ();
	}

    static bool drawTurbulenceProperties = true;

    void DrawTurbulenceProperites(PAParticleField pf) {

        DrawControlBox("Turbulence", "turbulence", ref drawTurbulenceProperties,
            () =>
            {
                int elementIndex = turbulenceTypeProp.enumValueIndex;
#if UNITY_5
                string[] options = new string[] { "None", "Simplex2D (Faster)", "Simplex (Nicer)" };
#else
                string[] options = new string[] { "None", "Sine" };
#endif
                int newIndex = EditorGUILayout.Popup("Type", elementIndex, options);
                if (newIndex != elementIndex) {
                    turbulenceTypeProp.enumValueIndex = newIndex;
                    MarkAsDirty(serializedObject, MeshFlags.None, true);
                }

                EditorGUI.BeginDisabledGroup(turbulenceTypeProp.enumValueIndex == 0);
                FlaggedLayoutPropertyField(turbulenceFrequencyProp, new GUIContent("Frequency", "How 'noisy' the turbulence is"), MeshFlags.None, true);
                FlaggedLayoutPropertyField(turbulenceAmplitudeProp, new GUIContent("Amplitude", "Affects how far the turbulence moves the particle from its base (linear) position"), MeshFlags.None, true);
                FlaggedLayoutPropertyField(turbulenceScaleProp, new GUIContent("Scale", "Affects how far the turbulence moves the particle along each axis"), MeshFlags.None, true);
                turbulenceScaleProp.vector3Value = Vector3.Max(turbulenceScaleProp.vector3Value, Vector3.one * 0.0001f);
                FlaggedLayoutPropertyField(turbulenceOffsetProp, new GUIContent("Offset Speed", "The speed at which the turbulence moves as a whole"), MeshFlags.None, true);
                EditorGUI.EndDisabledGroup();
            }
        );
    }

    static bool showPerParticleProperties = true;

	void DrawPerParticleProperties(PAParticleField pf){

        DrawControlBox("Per Particle Properties", "per-properties", ref showPerParticleProperties,
            () => 
            {
                FlaggedLayoutPropertyField(colorVariationProp, new GUIContent("Color Variation", "Each particle will be assigned a random color from this gradient"), MeshFlags.Color, false);

                DrawPercentVariationField(serializedObject.FindProperty("mMinimumSize"), "Size Variation", "Min Size", "The minimum size as a percentage of the base size (internally a value in range [0,1])", MeshFlags.Surface);
                DrawPercentVariationField(serializedObject.FindProperty("mMinimumSpeed"), "Speed Variation", "Min Speed", "the minimum speed as a percantage of the base speed  (internally a value in range [0,1])", MeshFlags.Speed);

                EditorGUI.BeginDisabledGroup(!pf.spin);
                DrawPercentVariationField(serializedObject.FindProperty("mMinSpinSpeed"), "Spin Variation", "Min Speed", "The minumum spin speed as a percentage of the base spin speed (internally a value in range [-1,1]", MeshFlags.Speed);
                EditorGUI.EndDisabledGroup();

                if (particleTypeProp.enumValueIndex != 0) {
                    DrawDisableableVector3(customRotationAxisProp, rotationAxisProp, "Custom Spin Axis", "For mesh particles, when false a random axis will be used", MeshFlags.Speed, false);
                }
            }
        );
	}

	float DrawPercentVariationField(SerializedProperty property, string label, string name, string tooltip, MeshFlags affectsMesh = MeshFlags.None, bool affectsShader = false){ //TODO this isnt multi enabled

		float lw = EditorGUIUtility.labelWidth;
		float fw = EditorGUIUtility.fieldWidth;

        float normalizedValue = property.floatValue;

		EditorGUILayout.BeginHorizontal ();
		EditorGUIUtility.labelWidth = 60f;
		EditorGUILayout.LabelField (label);
		EditorGUIUtility.labelWidth = 70f;
		EditorGUIUtility.fieldWidth = 30f;
        GUI.changed = false;
		float minimum = EditorGUILayout.FloatField (new GUIContent (name, tooltip), normalizedValue * 100f);
		normalizedValue = minimum / 100f;

        if (GUI.changed){
            property.floatValue = normalizedValue;
            MarkAsDirty(property.serializedObject, affectsMesh, affectsShader);
        }

		EditorGUIUtility.labelWidth = 2f;
		EditorGUIUtility.fieldWidth = 2f;
		EditorGUILayout.LabelField ("%");
		EditorGUIUtility.labelWidth = lw;		
		EditorGUIUtility.fieldWidth = fw;
		EditorGUILayout.EndHorizontal ();

		return normalizedValue;
	}

    static bool drawSpriteProperties = true;

	void DrawSpriteProperties(PAParticleField pf){

        DrawControlBox("Sprite Properties", "sprite-properties", ref drawSpriteProperties,
            () =>
            {
                int materialTypeIndex = materialTypeProp.enumValueIndex;
                FlaggedLayoutPropertyField(materialTypeProp, new GUIContent("Material", "The material/shader used to draw the particles"), MeshFlags.None, true);
                serializedObject.ApplyModifiedProperties();

                if (materialTypeIndex == 6 && materialTypeProp.enumValueIndex != 6) {
                    pf.material = null;
                } else if (materialTypeIndex != materialTypeProp.enumValueIndex) {
                    UpdateMaterialType(serializedObject, materialTypeProp.enumValueIndex);
                }

                FlaggedLayoutPropertyField(textureTypeProp, new GUIContent("Texture Type"), MeshFlags.Surface, true);

                if (pf.materialType == PAParticleField.MaterialType.Custom) {
                    FlaggedLayoutPropertyField(materialProp, new GUIContent("Material", "Custom material to apply to the particles"), MeshFlags.None, true);
                } else {
                    FlaggedLayoutPropertyField(textureProp, new GUIContent("Texture", "The texture applied tot he particles, can be a grid of sprites"), MeshFlags.None, true);
                }

                if (particleTypeProp.enumValueIndex != 1) {
                    FlaggedLayoutPropertyField(pivotProp, new GUIContent("Pivot Offset"), MeshFlags.Surface, false);
                }

                EditorGUILayout.Space();

                EditorGUIUtility.labelWidth = 80f;

                Rect mainTextureRect = EditorGUILayout.GetControlRect(false, 86f);
                Rect leftRect = mainTextureRect;
                leftRect.width *= 0.5f;
                leftRect.height = EditorGUIUtility.singleLineHeight;

                EditorGUI.LabelField(leftRect, "Grid Properties");

                leftRect.y += EditorGUIUtility.singleLineHeight;

                EditorGUI.BeginDisabledGroup(textureTypeProp.enumValueIndex == 0);
                FlaggedPropertyField(leftRect, spriteRowsProp, new GUIContent("Rows"), MeshFlags.Surface, false);

                leftRect.y += EditorGUIUtility.singleLineHeight;
                leftRect.y += spacing;

                FlaggedPropertyField(leftRect, spriteColumnsProp, new GUIContent((textureTypeProp.enumValueIndex == 2 ? "Frames" : "Columns")), MeshFlags.Surface, false);
                EditorGUI.EndDisabledGroup();

                leftRect.y += EditorGUIUtility.singleLineHeight;
                leftRect.y += spacing;

                EditorGUI.BeginDisabledGroup(textureTypeProp.enumValueIndex != 2);
                FlaggedPropertyField(leftRect, framerateProp, new GUIContent("Framerate"), MeshFlags.None, false);
                EditorGUI.EndDisabledGroup();

                leftRect.y += EditorGUIUtility.singleLineHeight;
                leftRect.y += spacing;

                EditorGUI.BeginDisabledGroup(pf.materialType != PAParticleField.MaterialType.CutOff && pf.materialType != PAParticleField.MaterialType.CutOffLit);
                FlaggedPropertyField(leftRect, cutOffProp, new GUIContent("Cut Off"), MeshFlags.None, true);
                cutOffProp.floatValue = Mathf.Clamp01(cutOffProp.floatValue);
                EditorGUI.EndDisabledGroup();

                Rect rightRect = mainTextureRect;
                rightRect.width *= 0.5f;
                rightRect.x += rightRect.width;

                rightRect.width -= spacing * 2;
                rightRect.x += spacing;

                EditorGUI.DrawRect(rightRect, new Color(0.1f, 0.1f, 0.1f, 0.2f));

                if (pf.texture && materialTypeProp.enumValueIndex != 6) {
                    float rightRectAspect = rightRect.width / (float)rightRect.height;
                    float textureAspect = pf.texture.width / (float)pf.texture.height;
                    float height = rightRect.height;
                    float width = rightRect.width;

                    if (rightRectAspect < textureAspect) {
                        rightRect.height = rightRect.width / textureAspect;
                        rightRect.y += height * 0.5f - rightRect.height * 0.5f;
                    } else {
                        rightRect.width = rightRect.height * textureAspect;
                    }
                    rightRect.x += ((width) - (rightRect.width)) * 0.5f;

                    rightRect.width -= spacing * 2;
                    rightRect.x += spacing;

                    rightRect.height -= spacing * 2;
                    rightRect.y += spacing;
                    EditorGUI.DrawPreviewTexture(EditorGUI.IndentedRect(rightRect), pf.texture);
                }
            }
        );
	}

	bool ShowFillRateWarning(PAParticleField pf){
		if (pf.generatorType != PAParticleField.ParticleType.Billboard) {
			return false;
		} else {
			return WARNING_THRESHOLD <((pf.particleCount - 1) * 4 * (pf.particleSize.x * pf.particleSize.y)) / (pf.fieldSize.x * pf.fieldSize.y * pf.fieldSize.z);
		}
	}

	void OnSceneGUI(){
		PAParticleField pf = target as PAParticleField;

		bool local = pf.simulationSpace != PAParticleField.SimulationSpace.World;

		if (pf.shape == PAParticleField.Shape.Cube) {
            Vector3 fieldSize = Vector3.Scale(pf.fieldSize, local ? Vector3.one : pf.transform.lossyScale);
			DrawWireCube (local ? Vector3.zero : pf.transform.position, Vector3.Scale (fieldSize, Vector3.one - pf.edgeThreshold), local ? pf.transform : null, new Color (0.5f, 0.5f, 0.5f, 0.5f));
		}

		DrawBoxGizmo (pf);
	}

	private Vector3[] mHandlePositions = null;

	void DrawBoxGizmo (PAParticleField field)
	{   		
		bool local = field.simulationSpace != PAParticleField.SimulationSpace.World;
		Vector3 Pos = field.transform.position;

		Quaternion fieldRot = field.transform.rotation;
		if (!local) {
			field.transform.rotation = Quaternion.identity;
		}

		Vector3 fieldSize = Vector3.Scale (field.fieldSize, local ? Vector3.one : field.transform.lossyScale);
		DrawWireCube (local ? Vector3.zero : field.transform.position, fieldSize, local ? field.transform : null, Color.white);

		// Calculate the handle positions
		mHandlePositions [0] = field.transform.TransformPoint (field.fieldSize.x * 0.5f, 0, 0);
		mHandlePositions [1] = field.transform.TransformPoint (field.fieldSize.x * -0.5f, 0, 0);
		mHandlePositions [2] = field.transform.TransformPoint (0, field.fieldSize.y * 0.5f, 0);
		mHandlePositions [3] = field.transform.TransformPoint (0, field.fieldSize.y * -0.5f, 0);
		mHandlePositions [4] = field.transform.TransformPoint (0, 0, field.fieldSize.z * 0.5f);
		mHandlePositions [5] = field.transform.TransformPoint (0, 0, field.fieldSize.z * -0.5f);
		
		float handleSize = 0.05f;
		
		Vector3[] NewHandlePositions = new Vector3[6];

#if UNITY_5_6_OR_NEWER
        Handles.CapFunction handle = Handles.DotHandleCap;
#else
        Handles.DrawCapFunction handle = Handles.DotCap;
#endif

        NewHandlePositions[0] = Handles.Slider (mHandlePositions [0], field.transform.right, HandleUtility.GetHandleSize (mHandlePositions [0]) * handleSize, handle, 0.1f);
		NewHandlePositions [1] = Handles.Slider (mHandlePositions [1], -field.transform.right, HandleUtility.GetHandleSize (mHandlePositions [1]) * handleSize, handle, 0.1f);
		NewHandlePositions [2] = Handles.Slider (mHandlePositions [2], field.transform.up, HandleUtility.GetHandleSize (mHandlePositions [2]) * handleSize, handle, 0.1f);
		NewHandlePositions [3] = Handles.Slider (mHandlePositions [3], -field.transform.up, HandleUtility.GetHandleSize (mHandlePositions [3]) * handleSize, handle, 0.1f);
		NewHandlePositions [4] = Handles.Slider (mHandlePositions [4], field.transform.forward, HandleUtility.GetHandleSize (mHandlePositions [4]) * handleSize, handle, 0.1f);
		NewHandlePositions [5] = Handles.Slider (mHandlePositions [5], -field.transform.forward, HandleUtility.GetHandleSize (mHandlePositions [5]) * handleSize, handle, 0.1f);

        bool changed = NewHandlePositions[0] != mHandlePositions[0] ||
                        NewHandlePositions[1] != mHandlePositions[1] ||
                        NewHandlePositions[2] != mHandlePositions[1] ||
                        NewHandlePositions[3] != mHandlePositions[1] ||
                        NewHandlePositions[4] != mHandlePositions[1] ||
                        NewHandlePositions[5] != mHandlePositions[1];

        if (changed) {

            Undo.RecordObject(field, "PAParticleField");
            Undo.RecordObject(field.transform, "PAParticleField");

            Vector3 Change;
            Vector3 size = field.fieldSize;

            Change = NewHandlePositions[0] - mHandlePositions[0];
            if (Change.sqrMagnitude != 0.0f) {
                field.transform.position = Pos + Change * 0.5f;
                size.x = (field.transform.position - NewHandlePositions[0]).magnitude * 2.0f / field.transform.localScale.x;
            }
            Change = NewHandlePositions[1] - mHandlePositions[1];
            if (Change.sqrMagnitude != 0.0f) {
                field.transform.position = Pos + Change * 0.5f;
                size.x = (field.transform.position - NewHandlePositions[1]).magnitude * 2.0f / field.transform.localScale.x;
            }
            Change = NewHandlePositions[2] - mHandlePositions[2];
            if (Change.sqrMagnitude != 0.0f) {
                field.transform.position = Pos + Change * 0.5f;
                size.y = (field.transform.position - NewHandlePositions[2]).magnitude * 2.0f / field.transform.localScale.y;
            }
            Change = NewHandlePositions[3] - mHandlePositions[3];
            if (Change.sqrMagnitude != 0.0f) {
                field.transform.position = Pos + Change * 0.5f;
                size.y = (field.transform.position - NewHandlePositions[3]).magnitude * 2.0f / field.transform.localScale.y;
            }
            Change = NewHandlePositions[4] - mHandlePositions[4];
            if (Change.sqrMagnitude != 0.0f) {
                field.transform.position = Pos + Change * 0.5f;
                size.z = (field.transform.position - NewHandlePositions[4]).magnitude * 2.0f / field.transform.localScale.z;
            }
            Change = NewHandlePositions[5] - mHandlePositions[5];
            if (Change.sqrMagnitude != 0.0f) {
                field.transform.position = Pos + Change * 0.5f;
                size.z = (field.transform.position - NewHandlePositions[5]).magnitude * 2.0f / field.transform.localScale.z;
            }

            if (field.fieldSize != size) {
                field.fieldSize = size;
            }
        }

        if (!local) {
            field.transform.rotation = fieldRot;
        }        
	}

	public static void DrawWireCube(Vector3 position, Vector3 size, Transform space, Color color)
	{
		Vector3 half = size * 0.5f;

		Vector3 a, b, c, d, e, f, g, h;
		a = position + new Vector3 (-half.x, -half.y, half.z);
		b = position + new Vector3 (half.x, -half.y, half.z);
		c = position + new Vector3 (half.x, half.y, half.z);
		d = position + new Vector3 (-half.x, half.y, half.z);

		e = position + new Vector3 (-half.x, -half.y, -half.z);
		f = position + new Vector3 (half.x, -half.y, -half.z);
		g = position + new Vector3 (-half.x, half.y, -half.z);
		h = position + new Vector3 (half.x, half.y, -half.z);

		if (space != null) {
			a = space.TransformPoint(a);
			b = space.TransformPoint(b);
			c = space.TransformPoint(c);
			d = space.TransformPoint(d);
			e = space.TransformPoint(e);
			f = space.TransformPoint(f);
			g = space.TransformPoint(g);
			h = space.TransformPoint(h);
		}

		Color col = Handles.color;

		Handles.color = color;

		// draw front
		Handles.DrawLine(a, b);
		Handles.DrawLine(a, d);
		Handles.DrawLine(c, b);
		Handles.DrawLine(c, d);
		// draw back
		Handles.DrawLine(e, f);
		Handles.DrawLine(e, g);
		Handles.DrawLine(h, f);
		Handles.DrawLine(h, g);
		// draw corners
		Handles.DrawLine(e, a);
		Handles.DrawLine(f, b);
		Handles.DrawLine(g, d);
		Handles.DrawLine(h, c);

		Handles.color = col;
	}

    delegate void GUIDelegate();

    void FlaggedGUI(GUIDelegate gui, MeshFlags affectsMesh = MeshFlags.None, bool affectsShader = false) {
        GUI.changed = false;
        gui();
        if (GUI.changed) {
            MarkAsDirty(serializedObject, affectsMesh, affectsShader);
        }
    }

	void FlaggedLayoutPropertyField(SerializedProperty property, GUIContent label, MeshFlags affectsMesh = MeshFlags.None, bool affectsShader = false, bool disabled = false){
		EditorGUI.BeginDisabledGroup(disabled);
		GUI.changed = false;
		EditorGUILayout.PropertyField (property, label);
		if (GUI.changed) {
			MarkAsDirty(property.serializedObject, affectsMesh, affectsShader);
		}
		EditorGUI.EndDisabledGroup ();
	}

	void FlaggedPropertyField(Rect propRect, SerializedProperty property, GUIContent label, MeshFlags affectsMesh = MeshFlags.None, bool affectsShader = false){
		GUI.changed = false;
		EditorGUI.PropertyField (propRect, property, label);
		if (GUI.changed) {
			MarkAsDirty(property.serializedObject, affectsMesh, affectsShader);
		}
	}

	void MarkAsDirty(SerializedObject obj, MeshFlags mesh, bool shader){
		for (int i = 0; i < obj.targetObjects.Length; i++) {
			PAParticleField field = obj.targetObjects[i] as PAParticleField;
			if (field){
				if (mesh != MeshFlags.None){
					field.meshIsDirtyMask = mesh;
				}
				if (shader){field.shaderIsDirty = true;}
			}
		}
	}

	void MarkMeshAsDirty(SerializedObject obj){
		for (int i = 0; i < obj.targetObjects.Length; i++) {
			PAParticleField field = obj.targetObjects[i] as PAParticleField;
			if (field){
				field.meshIsDirtyMask = MeshFlags.All;
			}
		}
	}

	void UpdateMaterialType(SerializedObject obj, int enumIndex){
		obj.ApplyModifiedProperties ();
		for (int i = 0; i < obj.targetObjects.Length; i++) {
			PAParticleField field = obj.targetObjects[i] as PAParticleField;
			if (field){
				field.materialType = PAParticleField.MaterialType.Custom;
				field.materialType = (PAParticleField.MaterialType)enumIndex;
			}
		}
	}

	void UpdateGeneratorType(SerializedObject obj, int enumIndex){
		for (int i = 0; i < obj.targetObjects.Length; i++) {
			PAParticleField field = obj.targetObjects[i] as PAParticleField;
			if (field){
				field.generatorType = (PAParticleField.ParticleType)enumIndex;
			}
		}
	}

	void DrawHelpButton(string topic, Rect lastRect){

		lastRect.height = EditorGUIUtility.singleLineHeight-2;
		lastRect.y += 2;
		lastRect.x += lastRect.width - 20 - 2;
		lastRect.width = 20f;
		
		if (GUI.Button (lastRect, "?")) {
			Application.OpenURL("http://www.popupasylum.co.uk/paparticlefield/documentation#" + topic);
		}
	}

    void DrawControlBox(string title, string docLink, ref bool open, GUIDelegate contents) {

        EditorGUIUtility.labelWidth = defaultLabelWidth;
        EditorGUIUtility.fieldWidth = defaultFieldWidth;
        GUI.color = new Color(0.95f, 0.95f, 0.95f, 1f);
        EditorGUILayout.LabelField(title, EditorStyles.toolbarButton);
        GUI.color = Color.white;

        Rect lastRect = GUILayoutUtility.GetLastRect ();
        DrawHelpButton(docLink, lastRect);

        Rect toggleRect = new Rect(lastRect);
        toggleRect.x += 16f;
        toggleRect.y += 2f;
        
        open = EditorGUI.Foldout(toggleRect, open, new GUIContent(""));

        if (open) {
            EditorGUILayout.BeginVertical(style);
            EditorGUILayout.Space();

            contents();

            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
        } else {
            EditorGUILayout.GetControlRect(false, 1f);
        }

        EditorGUIUtility.labelWidth = defaultLabelWidth;
        EditorGUIUtility.fieldWidth = defaultFieldWidth;
    }
}
