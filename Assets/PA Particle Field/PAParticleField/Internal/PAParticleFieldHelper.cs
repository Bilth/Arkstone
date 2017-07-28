using UnityEngine;
using System.Collections;

public class PAPFHelper {

    static bool hasInitialized = false;

    public static int _IsUnserialized;

     //Floats as Bools
    public static int _Editor;
    public static int _UserFacing;
    public static int _EdgeAlpha;
    public static int _EdgeScale;
      
     //Particle params
    public static int _ParticleSize;
    public static int _FieldSize;
    public static int _Speed;
    public static int _SpinSpeed;
    public static int _Force;
    public static int _SpeedScale;
    public static int _UOffset;
    public static int _EdgeThreshold;
    public static int _InverseEdgeThreshold;
    public static int _FaceDirection;
    public static int _UpDirection;

    public static int _ParticleCount;
    public static int _CountMask;
      
    public static int _TotalTime;
    public static int _DeltaSpeed;
    public static int _DeltaForce;
    public static int _DeltaPosition;
      
     //Soft particle params
    public static int _NearFadeDistance;
    public static int _NearFadeOffset;
      
    public static int[] _ExclusionMatrix;
    public static int[] _ExclusionThreshold;
    public static int[] _InverseExclusionThreshold;

    //Turbulence
    public static int _TurbulenceOffset;
    public static int _TurbulenceFrequency;
    public static int _TurbulenceScale;
    public static int _TurbulenceDeltaOffset;

    public static int _MainTex;
    public static int _Color;
    public static int _CutOff;
    public static int _Softness;

    public static void GetPropertyIDs() {
        if (hasInitialized) { return; }

        _IsUnserialized = Shader.PropertyToID("_IsUnserialized");

         //Floats as Bools
        _Editor                 = Shader.PropertyToID("_Editor");
        _UserFacing             = Shader.PropertyToID("_UserFacing");
        _EdgeAlpha              = Shader.PropertyToID("_EdgeAlpha");
        _EdgeScale              = Shader.PropertyToID("_EdgeScale");
                                                                         
        //Particle params                                   
        _ParticleSize           = Shader.PropertyToID("_ParticleSize");
        _FieldSize              = Shader.PropertyToID("_FieldSize");
        _Speed                  = Shader.PropertyToID("_Speed");
        _SpinSpeed              = Shader.PropertyToID("_SpinSpeed");
        _Force                  = Shader.PropertyToID("_Force");
        _SpeedScale             = Shader.PropertyToID("_SpeedScale");
        _UOffset                = Shader.PropertyToID("_UOffset");
        _EdgeThreshold          = Shader.PropertyToID("_EdgeThreshold");
        _InverseEdgeThreshold   = Shader.PropertyToID("_InverseEdgeThreshold");
        _FaceDirection          = Shader.PropertyToID("_FaceDirection");
        _UpDirection            = Shader.PropertyToID("_UpDirection");

        _ParticleCount          = Shader.PropertyToID("_ParticleCount");
        _CountMask              = Shader.PropertyToID("_CountMask");

        _TotalTime              = Shader.PropertyToID("_TotalTime");
        _DeltaSpeed             = Shader.PropertyToID("_DeltaSpeed");
        _DeltaForce             = Shader.PropertyToID("_DeltaForce");
        _DeltaPosition          = Shader.PropertyToID("_DeltaPosition");
                                                                         
        //Soft particle params                         
        _NearFadeDistance       = Shader.PropertyToID("_NearFadeDistance");
        _NearFadeOffset         = Shader.PropertyToID("_NearFadeOffset");

        _ExclusionMatrix = new int[3];
        _ExclusionThreshold = new int[3];
        _InverseExclusionThreshold = new int[3];
        _ExclusionMatrix[0]       = Shader.PropertyToID("_ExclusionMatrix0");
        _ExclusionMatrix[1]       = Shader.PropertyToID("_ExclusionMatrix1");
        _ExclusionMatrix[2]       = Shader.PropertyToID("_ExclusionMatrix2");
        _ExclusionThreshold[0]    = Shader.PropertyToID("_ExclusionThreshold0");
        _ExclusionThreshold[1]    = Shader.PropertyToID("_ExclusionThreshold1");
        _ExclusionThreshold[2]    = Shader.PropertyToID("_ExclusionThreshold2");
        _InverseExclusionThreshold[0] = Shader.PropertyToID("_InverseExclusionThreshold0");
        _InverseExclusionThreshold[1] = Shader.PropertyToID("_InverseExclusionThreshold1");
        _InverseExclusionThreshold[2] = Shader.PropertyToID("_InverseExclusionThreshold2");

        //Turbulence                                             
        _TurbulenceOffset       = Shader.PropertyToID("_TurbulenceOffset");
        _TurbulenceFrequency    = Shader.PropertyToID("_TurbulenceFrequency");
        _TurbulenceScale        = Shader.PropertyToID("_TurbulenceScale");
        _TurbulenceDeltaOffset  = Shader.PropertyToID("_TurbulenceDeltaOffset");

        _MainTex                = Shader.PropertyToID("_MainTex");
        _Color                  = Shader.PropertyToID("_Color");
        _CutOff                 = Shader.PropertyToID("_CutOff");
        _Softness               = Shader.PropertyToID("_Softness");

        hasInitialized = true;
    }
}
