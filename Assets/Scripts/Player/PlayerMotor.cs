using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.VR;
using UnityEngine.Networking;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour {

    private float _GLIDE_CATCH_TIME_MAX = 2f;
    private float _TERMINAL_VELOCITY_HORIZONTAL = 3f;
    private float TIME_CHECK_GROUNDED = .2f;
    private float TIME_AIRBORNE_MAX = .3f;
    private float SPEED_BASE = 800f; //150f
    private float SPEED_BOOST = 100f; //150f
    private float GRAVITY = -9.81f;
    private bool _VR_TURNING = true;

    [SerializeField]
    public Camera _cam;

    private Vector3 _lean = Vector3.zero;
    private Vector3 _velocityPlanar = Vector3.zero;
    private Vector3 _momentum = Vector3.zero;
    private float _momentumMagnitude;
    private Vector3 _rotate = Vector3.zero;
    private float _cameraRotationX = 0f; // X Rotation Only
    private float _cameraRotationXCurrent = 0f;
    private Vector3 _thrusterForce = Vector3.zero;
    private RaycastHit _groundHit;
    private float _distToGround = 0f;

    private LineRenderer _lineRenderer;
    private bool _rayGrounded;
    private Vector3 _lastCollisionLocal = Vector3.zero;

    private AudioSource _audioSource;
    public AudioClip land;

    private float _speed;

    private Rigidbody _body;
    private CharacterController _controller;

    private Vector3 _velocityFinal;

    private bool _isGrounded = false;
    private bool _canAirJump = false;
    private bool _doCatchFall = false;
    private float _courseStrength = 0f;
    private float _courseCharge = 0f;
    private float _timerCheckAirborne = 0f;
    private float _timeAirborne = 0f;
    private float _timerGliding = 0f;
    private float _cooldownJump = 0f;
    private float _gravity = 9.81f;

    [SerializeField]
    private float _cameraRotationLimit = 89f;
    private float _cameraRotationCharge;
    private List<string> _colliders;
    private List<string> _triggers;

    // Use this for initialization
    void Start() {
        _body = GetComponent<Rigidbody>();
        _controller = GetComponent<CharacterController>();
        _audioSource = GetComponent<AudioSource>();
        _velocityFinal = Vector3.zero;

        _colliders = new List<string>();
        _triggers = new List<string>();

        DrawLine(transform.position, transform.position + new Vector3(0.0f, -0.5f, 0.0f), Color.red, 5);

        _isGrounded = false;
        _speed = SPEED_BASE;

        _distToGround = _body.GetComponent<Collider>().bounds.extents.y;
    }

    void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0.2f)
    {
        gameObject.AddComponent<LineRenderer>();
        _lineRenderer = gameObject.GetComponent<LineRenderer>();
        _lineRenderer.material = new Material(Shader.Find("Sprites/Default")); //Particles/Alpha Blended Premultiply
        _lineRenderer.startColor = Color.yellow;
        _lineRenderer.endColor = Color.blue;
        _lineRenderer.startWidth = .5f;
        _lineRenderer.endWidth = 0.0f;
        _lineRenderer.SetPosition(0, start);
        _lineRenderer.SetPosition(1, end);
        _lineRenderer.sortingOrder = 1;
    }

    public void Lean(Vector3 pLean) {
        _lean = pLean;
    }

    public void SetRotateY(float pY) {
        _rotate.y = pY;
    }

    public void setCameraRotation(float pRotation) {
        _cameraRotationX = pRotation;
    }

    public void Teleport(float x, float y, float z)
    {
        Jump();

        Transform tSpawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = tSpawnPoint.position;
        transform.rotation = tSpawnPoint.rotation;
    }

    void Update()
    {
        _courseStrength = Input.GetAxisRaw("Course");

        PerformMovement();

        if (!_controller.isGrounded) // Course-Corrected Air Movement
        {
            float tCorrectionSpeed = 10f;

            if (_lean.x != 0)
            {
                if (_lean.x > 0 && _velocityFinal.x < _lean.x)
                {
                    _velocityFinal.x += _lean.x * Time.deltaTime * tCorrectionSpeed;
                    if (_velocityFinal.x > _lean.x) { _velocityFinal.x = _lean.x; }
                } else if (_lean.x < 0 && _velocityFinal.x > _lean.x)
                {
                    _velocityFinal.x += _lean.x * Time.deltaTime * tCorrectionSpeed;
                    if (_velocityFinal.x < _lean.x) { _velocityFinal.x = _lean.x; }
                }
            }

            if (_lean.z != 0)
            {
                if (_lean.z > 0 && _velocityFinal.z < _lean.z)
                {
                    _velocityFinal.z += _lean.z * Time.deltaTime * tCorrectionSpeed;
                    if (_velocityFinal.z > _lean.z) { _velocityFinal.z = _lean.z; }
                }
                else if (_lean.z < 0 && _velocityFinal.z > _lean.z)
                {
                    _velocityFinal.z += _lean.z * Time.deltaTime * tCorrectionSpeed;
                    if (_velocityFinal.z < _lean.z) { _velocityFinal.z = _lean.z; }
                }
            }

            _velocityFinal.y += Physics.gravity.y * Time.deltaTime;
        }
        else // Smooth Ground Movement
        {
            _velocityFinal.x = _lean.x;
            _velocityFinal.z = _lean.z;
            if (_velocityFinal.y <= 0)
            {
                _velocityFinal.y = Physics.gravity.y * Time.deltaTime;
            }
        }

        if (_courseStrength > 0)
        {
            _courseCharge += Time.deltaTime;
        } else if (_courseCharge > 0)
        {
            // Leap!!
            Vector3 tDash = Camera.main.transform.forward * 10f;
            tDash.y += 15f;
            _velocityFinal = tDash;

            _courseCharge = 0;
        }

        _controller.Move(_velocityFinal * Time.deltaTime);

        PerformRotation();

        // Add downhill speedup

        // Intelligent Ground Snapping - Raycasts from last collision point instead of center point, since last collision is closer to the ground
        //      Allows for a much smaller snapping distance, reducing unwanted snapping during normal landings
        float snapDistance = .1f;
        if (!_controller.isGrounded && _timerGliding == 0 && _velocityFinal.y < 0)
        {
            RaycastHit hitInfo = new RaycastHit();
            if (Physics.Raycast(new Ray(transform.TransformPoint(_lastCollisionLocal), Vector3.down), out hitInfo, snapDistance))
            {
                Vector3 tDiff = hitInfo.point - transform.TransformPoint(_lastCollisionLocal);
                if(tDiff.magnitude > 0f)
                {
                    //Debug.Log("DIFF: " + tDiff + "Magnitude: " + tDiff.magnitude);
                    _controller.Move(tDiff);

                    OnLand();
                }
            }

        }
    }

    void PerformMovement()
    {
        // *** GO
        // >>> Design first area flow, objectives, etc.
        // Better in-air course correction

        // *** WAIT

        // EASY
        // > Faster downhill
        // > Ladder & Rope Climbing
        // > Ceiling grab
        // > Ceiling release
        // > Course cancel
        // > Energy
        // > Impact
        // > Health

        // WAIT
        // > Smooth Rider - Allows for smooth traversal on uneven ground
        // > Disable Character Controller Step
        // > Ducking
        // > Window Hop

        // DONE
        // + Smoother downhill
        // + No momentum
        // + Complete Control
        // + Full Air Control
        // + No Drag
        // + No Run button that needs to be constantly held

        if (!_controller.isGrounded) { _timeAirborne += Time.deltaTime; }
        _cooldownJump -= Time.deltaTime;

        // Glide - Coursing catches fall
        if (_courseStrength > 0)
        {
            _timerGliding += Time.deltaTime;
            if (_timerGliding > _GLIDE_CATCH_TIME_MAX) { _timerGliding = _GLIDE_CATCH_TIME_MAX; }

            if (_velocityFinal.y < 0)
            {
                float tMaxCourseFall = -1f; // 1m/s
                _velocityFinal.y = Mathf.Lerp(_velocityFinal.y, tMaxCourseFall, _timerGliding / _GLIDE_CATCH_TIME_MAX);
            }
        }
        else
        {
            _timerGliding = 0;
        }
    }

    // Run every physics iteration
    void FixedUpdate() {
        /* _courseStrength = Input.GetAxisRaw("Course");

         float tVelocityY = GetComponent<Rigidbody>().velocity.y;

         _speed = SPEED_BASE;

         _lean.y = Physics.gravity.y;

         _controller.Move(_lean * Time.fixedDeltaTime);

         _performMovement();
         _performRotation();*/
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        _lastCollisionLocal = transform.InverseTransformPoint(hit.point);

        // Relative Velocity
        var tRigidbody = hit.gameObject.GetComponent<Rigidbody>();
        if (tRigidbody != null)
        {
            //Vector3 tRelativeVelocity = tRigidbody.velocity
        }

        // Collision Friction
        if (!_controller.isGrounded)
        {
            _velocityFinal.x -= _velocityFinal.x * Time.deltaTime * 5f;
            _velocityFinal.z -= _velocityFinal.z * Time.deltaTime * 5f;
            if (_velocityFinal.y < 0) // If falling, slow fall speed
            {
                _velocityFinal.y -= _velocityFinal.y * Time.deltaTime * 5f;
            }

            // Check Ceiling



            // Debug.Log("FRICTION! " + _velocityFinal.y + ", Delta: " + (_velocityFinal.y * Time.deltaTime * 5f));
        }
        else
        {
            OnLand();
        }

    }

    void OnLand() {
        _timeAirborne = 0;
        _timerGliding = 0;
        _canAirJump = true;
    }

    void OnTriggerEnter(Collider collider)
    {
        if (_triggers == null) return;
        
        string tName = collider.transform.name;
        _triggers.Add(tName);

        if (collider.gameObject.tag == "Platform")
        {
            transform.parent = collider.gameObject.transform;
        }

        if (collider.gameObject.tag == "River")
        {
            Debug.Log("IN RIVER!!");
            //transform.parent = collider.gameObject.transform;
        }

        //_doCatchFall = true;
        //SetGrounded();
    }

    void OnTriggerExit(Collider collider)
    {
        if (_triggers == null) return;
        
        string tName = collider.transform.name;
        _triggers.Remove(tName);

        if (collider.gameObject.transform == transform.parent)
        {
            transform.parent = null;
        }

        if (collider.gameObject.tag == "River")
        {
            Debug.Log("OUT RIVER!!");
            //transform.parent = collider.gameObject.transform;
        }

        if (_triggers.Count == 0)
        {
            //_doCatchFall = false;
            //SetGrounded(false);
            
        }
    }

        void OnCollisionEnter(Collision collision)
    {
        if (_colliders == null) return;

        string tName = collision.collider.transform.name;
        int tColliderStart = _colliders.Count;

        _colliders.Add(tName);

        // While Airborne
        //if (_timeAirborne > .1f)
        {
            // Check Feet
            //if(Physics.Raycast(_body.GetComponent<Transform>().position, new Vector3(0, -1, 0), _distToGround + .2f))
            {
              /*  _isGrounded = true;
                _timeAirborne = 0f;

                Debug.Log("Velocity Y: " + collision.relativeVelocity.magnitude);

                // If falling fast, play a noise (dependent on velocity)
                if (tColliderStart == 0 && collision.relativeVelocity.magnitude > 15)
                {
                    _audioSource.time = .9f;
                    _audioSource.PlayOneShot(land, .1f);
                }*/
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (_colliders == null) return;

        string tName = collision.collider.transform.name;
        _colliders.Remove(tName);

        Debug.Log("Collision Exit: " + tName + ", " + _colliders.Count);
    }

    void UpdateLine(Color pColor, Vector3 tPosStart, Vector3 tPosEnd)
    {
        _lineRenderer.startColor = Color.red;
        _lineRenderer.endColor = Color.red;
        _lineRenderer.SetPosition(0, tPosStart);
        _lineRenderer.SetPosition(1, tPosEnd);
    }

	void PerformRotation() {

        // VR Chunked Rotation
        if(UnityEngine.XR.XRSettings.enabled)
        {
            _cameraRotationCharge += _rotate.y;
            if (Mathf.Abs(_cameraRotationCharge) < 22.5f)
            {
                return;
            }

            _rotate.y += _cameraRotationCharge;
            _cameraRotationCharge = 0f;
        }

        _controller.transform.Rotate(_rotate);
		if (_cam != null) {
            // Set our rotation and clamp it
            _cameraRotationXCurrent -= _cameraRotationX;
            _cameraRotationXCurrent = Mathf.Clamp(_cameraRotationXCurrent, - _cameraRotationLimit, _cameraRotationLimit);

            // Apply our rotation to the transform of the camera
            // Since camera is a child of player, it's always going to have only x rotation, with 0 y and z. No quaternians needed
            _cam.transform.localEulerAngles = new Vector3(_cameraRotationXCurrent, 0f, 0f);
		}
	}

    public float fallingForce()
    {
        return 0;
    }

    public void CoolJump()
    {
        _cooldownJump = .3f;
    }

    public void Jump()
    {
        if(_timeAirborne > TIME_AIRBORNE_MAX && !_controller.isGrounded) { _canAirJump = false; }

        _velocityFinal.y = 9f;
        CoolJump();
    }

    public void JumpPhysics()
    {
        NullifyVelocityY();

        Vector3 tJumpForce = Vector3.up;
        tJumpForce *= 200f; // 650f
        _body.AddForce(tJumpForce, ForceMode.Impulse);
        CoolJump();
    }

    // Get a force vector for our thrusters
    public void applyThruster(Vector3 pForce)
    {
        Debug.Log("Apply Thruster Force: " + pForce);
        _thrusterForce = pForce;
    }

    public bool CanJump()
    {
        //Debug.Log("Can Jump? " + _timeAirborne + ", " + _controller.isGrounded + ", " + _cooldownJump);
        return _cooldownJump < 0f && (_timeAirborne < TIME_AIRBORNE_MAX || _controller.isGrounded || _canAirJump);
    }
    
    public Vector3 velocity
    {
        get { return _body.velocity; }
    }

    public bool isGrounded
    {
        get { return _controller.isGrounded; }
    }

    public float speed
    {
        get { return _speed; }
        set { _speed = value; }
    }

    /*** HELPER FUNCTIONS ***/

    public void NullifyVelocity()
    {
        NullifyVelocityX();
        NullifyVelocityY();
        NullifyVelocityZ();
    }

    public void NullifyVelocityX()
    {
        float tReversingForce = -GetComponent<Rigidbody>().velocity.x;
        _body.AddForce(new Vector3(tReversingForce, 0, 0), ForceMode.VelocityChange);
    }

    public void NullifyVelocityY()
    {
        float tReversingForce = -GetComponent<Rigidbody>().velocity.y;
        _body.AddForce(new Vector3(0, tReversingForce, 0), ForceMode.VelocityChange);
    }

    public void NullifyVelocityZ()
    {
        float tReversingForce = -GetComponent<Rigidbody>().velocity.z;
        _body.AddForce(new Vector3(0, 0, tReversingForce), ForceMode.VelocityChange);
    }
}
