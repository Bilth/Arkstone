using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(PAExclusionZone))]
[CanEditMultipleObjects]
public class PAExclusionInspector : Editor {
	
	[MenuItem("GameObject/Create Other/PA Exclusion Zone")]
	static void CreateExclusionZone(){
		Selection.activeGameObject = PAExclusionZone.Create ("New PAExclusionZone").gameObject;
	}

	private Vector3[] mHandlePositions = null;
		
	void OnEnable ()
	{
		mHandlePositions = new Vector3[6];
	}
		
	void OnDisable ()
	{
		mHandlePositions = null;
	}

	void OnSceneGUI(){
		PAExclusionZone zone = target as PAExclusionZone;
		if(zone){
			DrawBoxGizmo(zone);

			PAParticleFieldInspector.DrawWireCube (Vector3.zero, zone.edgeThreshold, zone.transform, new Color(1,1,1,0.5f));
		}
	}
		
	void DrawBoxGizmo (PAExclusionZone zone)
	{   
		Vector3 Pos = zone.transform.position;  
		// Calculate the handle positions
		mHandlePositions [0] = zone.transform.TransformPoint (0.5f, 0, 0);
		mHandlePositions [1] = zone.transform.TransformPoint (-0.5f, 0, 0);
		mHandlePositions [2] = zone.transform.TransformPoint (0, 0.5f, 0);
		mHandlePositions [3] = zone.transform.TransformPoint (0, -0.5f, 0);
		mHandlePositions [4] = zone.transform.TransformPoint (0, 0, 0.5f);
		mHandlePositions [5] = zone.transform.TransformPoint (0, 0, -0.5f);		

		PAParticleFieldInspector.DrawWireCube (Vector3.zero, Vector3.one, zone.transform, Color.white);

		Undo.RecordObject (zone.transform, "PAExclusionZone");

		float handleSize = 0.025f;
		
		Vector3[] NewHandlePositions = new Vector3[6];

#if UNITY_5_6_OR_NEWER
        Handles.CapFunction handle = Handles.DotHandleCap;
#else
        Handles.DrawCapFunction handle = Handles.DotCap;
#endif

        NewHandlePositions [0] = Handles.Slider (mHandlePositions [0], zone.transform.right, HandleUtility.GetHandleSize (mHandlePositions [0]) * handleSize, handle, 0.1f);
		NewHandlePositions [1] = Handles.Slider (mHandlePositions [1], -zone.transform.right, HandleUtility.GetHandleSize (mHandlePositions [1]) * handleSize, handle, 0.1f);
		NewHandlePositions [2] = Handles.Slider (mHandlePositions [2], zone.transform.up, HandleUtility.GetHandleSize (mHandlePositions [2]) * handleSize, handle, 0.1f);
		NewHandlePositions [3] = Handles.Slider (mHandlePositions [3], -zone.transform.up, HandleUtility.GetHandleSize (mHandlePositions [3]) * handleSize, handle, 0.1f);
		NewHandlePositions [4] = Handles.Slider (mHandlePositions [4], zone.transform.forward, HandleUtility.GetHandleSize (mHandlePositions [4]) * handleSize, handle, 0.1f);
		NewHandlePositions [5] = Handles.Slider (mHandlePositions [5], -zone.transform.forward, HandleUtility.GetHandleSize (mHandlePositions [5]) * handleSize, handle, 0.1f);
		
		Vector3 Change;
		Vector3 Scale = zone.transform.localScale;
		
		Change = NewHandlePositions [0] - mHandlePositions [0];
		if (Change.sqrMagnitude != 0.0f) {
			zone.transform.position = Pos + Change * 0.5f;
			Scale.x = (zone.transform.position - NewHandlePositions [0]).magnitude * 2.0f;
		}
		Change = NewHandlePositions [1] - mHandlePositions [1];
		if (Change.sqrMagnitude != 0.0f) {
			zone.transform.position = Pos + Change * 0.5f;
			Scale.x = (zone.transform.position - NewHandlePositions [1]).magnitude * 2.0f;
		}
		Change = NewHandlePositions [2] - mHandlePositions [2];
		if (Change.sqrMagnitude != 0.0f) {
			zone.transform.position = Pos + Change * 0.5f;
			Scale.y = (zone.transform.position - NewHandlePositions [2]).magnitude * 2.0f;
		}
		Change = NewHandlePositions [3] - mHandlePositions [3];
		if (Change.sqrMagnitude != 0.0f) {
			zone.transform.position = Pos + Change * 0.5f;
			Scale.y = (zone.transform.position - NewHandlePositions [3]).magnitude * 2.0f;
		}
		Change = NewHandlePositions [4] - mHandlePositions [4];
		if (Change.sqrMagnitude != 0.0f) {
			zone.transform.position = Pos + Change * 0.5f;
			Scale.z = (zone.transform.position - NewHandlePositions [4]).magnitude * 2.0f;
		}
		Change = NewHandlePositions [5] - mHandlePositions [5];
		if (Change.sqrMagnitude != 0.0f) {
			zone.transform.position = Pos + Change * 0.5f;
			Scale.z = (zone.transform.position - NewHandlePositions [5]).magnitude * 2.0f;
		}
		
		if (zone.transform.localScale != Scale) {
			zone.transform.localScale = Scale;
		}
	}
}
