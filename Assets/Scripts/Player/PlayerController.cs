using UnityEngine;
using System.Collections;
using UnityEngine.VR;

[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour {

    private float _RUSHING_MODIFIER = 3f;

    [SerializeField]
    private bool _useMouse = false;

    private float _lookSensitivity = 10f; //6f
    private float _thrusterForce = 30f; //700f
    private float _cameraOffsetY = 0f;
    private float _courseStrength = 0f;
    private bool _rushing = false;
    private float _jumpStrength = 0f;
    private bool _jumpCharging = false;

	private PlayerMotor _motor;

	// Use this for initialization
	void Start () {
		_motor = GetComponent<PlayerMotor>();

        //Time.timeScale = 2.0f;
    }

	// Update is called once per frame
	void Update () {
        float deadzone = 0.1f;
        Vector2 stickInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (stickInput.magnitude < deadzone)
            stickInput = Vector2.zero;
        else
            stickInput = stickInput.normalized * ((stickInput.magnitude - deadzone) / (1 - deadzone));
        
        // Player Lean
        Vector3 tMovX = transform.right * stickInput.x; 	// (X, 0, 0)
        Vector3 tMovY = transform.forward * stickInput.y;   // (0, 0, X)
        Vector3 tLean = (tMovX + tMovY); //.normalized

        //Debug.Log("LEAN: " + tLean);
        tLean.y = 0;

        // Apply Lean
        if(_motor.isGrounded)
        {
            tLean *= 5f;
        } else
        {
            tLean *= 5f;
        }
        
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

        //Debug.Log("Falling Force: " + _motor.fallingForce());
        //cameraInput.y += _motor.fallingForce();

        // Only do y axis because left/right mouse will affect camera, not player
        //Vector3 tRot = new Vector3 (0f, cameraInput.y, 0f) * _lookSensitivity;

		// Apply rotation
		_motor.SetRotateY(cameraInput.y * _lookSensitivity); //tRot

        // Calculate camera rotation as a 3D Vector (turning around)
        float tCameraRotationX = cameraInput.x * _lookSensitivity;
		_motor.setCameraRotation (tCameraRotationX);

        if (Input.GetButtonDown("Jump") && _motor.CanJump())  {
            _motor.Jump();
        }

        if(Input.GetButtonDown("Recall"))
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


        /*if (Input.GetButtonDown("Jump"))
        {
            _jumpStrength = 0f;
            _jumpCharging = true;
        }

        if(_jumpCharging)
        {
            _jumpStrength += Time.deltaTime;
        }

        // Jump
        if (Input.GetButtonUp("Jump") && _motor.CanJump()) {
            _jumpCharging = false;
            _motor.Jump(Vector3.up * _thrusterForce, _jumpStrength);
        }*/
    }
}
