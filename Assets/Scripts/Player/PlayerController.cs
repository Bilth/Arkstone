using UnityEngine;
using System.Collections;
using UnityEngine.VR;

[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour {
    
    [SerializeField]
    private bool _useMouse = false;

    private float _lookSensitivity = 10f; //6f

	private PlayerMotor _motor;

	// Use this for initialization
	void Start () {
		_motor = GetComponent<PlayerMotor>();
    }

	// Update is called once per frame
	void Update () {
        float deadzone = 0.1f;
        Vector2 stickInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (stickInput.magnitude < deadzone)
            stickInput = Vector2.zero;
        else
            stickInput = stickInput.normalized * ((stickInput.magnitude - deadzone) / (1 - deadzone));

        //Debug.Log("Stick Input: " + stickInput.x + ", " + stickInput.y);
        
        // Player Lean
        Vector3 tMovX = transform.right * stickInput.x; 	// (X, 0, 0)
        Vector3 tMovY = transform.forward * stickInput.y;   // (0, 0, X)
        Vector3 tLean = (tMovX + tMovY).normalized; // Direction
        tLean.y = 0;

        var tSpeed = stickInput.magnitude * 4f; // Magnitude
        tLean *= tSpeed;

        _motor.Lean(tLean);

        Vector2 cameraInput;

        if(_useMouse) {
            // Mouse
            cameraInput = new Vector2(Input.GetAxisRaw("Mouse Y"), Input.GetAxisRaw("Mouse X"));
        } else
        {
            // Controller
            deadzone = 0.1f;
            cameraInput = new Vector2(Input.GetAxisRaw("Controller Y"), Input.GetAxisRaw("Controller X"));
           // Debug.Log(cameraInput.magnitude + ", " + cameraInput.x + ", " + cameraInput.y);
            if (cameraInput.magnitude < deadzone)
                cameraInput = Vector2.zero;
            else
                cameraInput = cameraInput.normalized * ((cameraInput.magnitude - deadzone) / (1 - deadzone));
        }

		// Apply rotation
		_motor.SetRotateY(cameraInput.y * _lookSensitivity * Time.deltaTime * 90f);

        // Calculate camera rotation as a 3D Vector (turning around)
        float tCameraRotationX = cameraInput.x * _lookSensitivity;
		_motor.setCameraRotation (tCameraRotationX);

        // 
        if (Input.GetButtonDown("Jump"))  {
            if(_motor.CanJump())
            {
                _motor.Jump();
            } else
            {
                _motor.SetGliding();
            }
        }

        // LB
        if (Input.GetButtonDown("Leap"))
        {
            _motor.Leap();
        }

            if (Input.GetButtonDown("Recall"))
        {
            Debug.Log("RECALL!");

            _motor.Teleport(20, 0, 8);

            if(UnityEngine.XR.XRSettings.enabled && UnityEngine.XR.XRDevice.isPresent)
            {
                UnityEngine.XR.XRDevice.SetTrackingSpaceType(UnityEngine.XR.TrackingSpaceType.RoomScale);
                Debug.Log("Resetting Seated Zero Pose.");
                //SteamVR.instance.hmd.ResetSeatedZeroPose();
                Valve.VR.OpenVR.System.ResetSeatedZeroPose();
            }
        }
    }
}
