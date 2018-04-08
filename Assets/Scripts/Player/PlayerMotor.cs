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
    void Start () {
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

    public void SetGrounded(bool pValue = true)
    {
        if(_rayGrounded && pValue == false) { return; } // Ignore if raycast still grounded 

        _isGrounded = pValue;
        if (_isGrounded)
        {
            Debug.Log("GROUNDED!");

            //NullifyVelocityX();
           // NullifyVelocityZ();

            _timeAirborne = 0f;
            //_audioSource.time = .9f;
            //_audioSource.PlayOneShot(land, .1f);

            var tDampener = GetComponent<ControlDampener>();
            if (tDampener != null)
            {
                tDampener.Clear();
            }
        }
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

    public void Teleport (float x, float y, float z)
    {
        NullifyVelocity();
        Jump();

        Transform tSpawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = tSpawnPoint.position;
        transform.rotation = tSpawnPoint.rotation;
    }

    void Update()
    {
        _courseStrength = Input.GetAxisRaw("Course");

        float tVelocityY = GetComponent<Rigidbody>().velocity.y;

        _speed = SPEED_BASE;
        
        _performCustomMovement();
        
        if (!_controller.isGrounded) // Course-Corrected Air Movement
        {
            float tCorrectionSpeed = 10f;
            
            if(_lean.x != 0)
            {
                if(_lean.x > 0 && _velocityFinal.x < _lean.x)
                {
                    _velocityFinal.x += _lean.x * Time.deltaTime * tCorrectionSpeed;
                    if(_velocityFinal.x > _lean.x) { _velocityFinal.x = _lean.x; }
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
            if(_velocityFinal.y <= 0)
            {
                _velocityFinal.y = Physics.gravity.y * Time.deltaTime;
            }
        }

        if (_courseStrength > 0)
        {
            _courseCharge += Time.deltaTime;
            // Debug.Log("CHARGE: " + _courseCharge);
        }
        else if (_courseCharge > 0)
        {
            /*var tDampener = GetComponent<ControlDampener>();
            if (tDampener != null)
            {
                tDampener.AddDampener(1f);
            }*/

            Debug.Log("DISCHARGE: " + _courseCharge);

            // Discharge Energy
            //_body.AddForce(Camera.main.transform.forward * 200f * _courseCharge, ForceMode.Impulse);

            // Leap!!
            Vector3 tDash = Camera.main.transform.forward * 10f;
            tDash.y += 15f;
            _velocityFinal = tDash;
            //_isGrounded = false;

            _courseCharge = 0;
        }

        // Snap to ground
        float snapDistance = .1f;
        /*Vector3 tVelocitySnap = Vector3.zero;
        if (!_controller.isGrounded && _velocityFinal.y <= 0)
        {
            RaycastHit hitInfo = new RaycastHit();
            if (Physics.Raycast(new Ray(transform.position, Vector3.down), out hitInfo, snapDistance))
            {
                float tDiffY = hitInfo.point.y - transform.position.y;
                if(tDiffY > 0) { tDiffY = 0; }
                float tMoveY = (tDiffY * .2f);
                Debug.Log("Move Y: " + tMoveY);
                tVelocitySnap.y = tMoveY;

                //_controller.Move(hitInfo.point - transform.position);
                //if(_velocityFinal.y > hitInfo.point.y - transform.position.y)
                //{
                //_velocityFinal.y = (hitInfo.point.y - transform.position.y) / Time.deltaTime; // We're doing this since it has to be reversed below
                //}
                //_controller.Move(new Vector3(0, tMoveY, 0));
                //hitInfo.point.y - transform.position.y
            }

        }*/

        _controller.Move(_velocityFinal * Time.deltaTime);

        _performRotation();

        // Add downhill speedup
        
        // Intelligent Ground Snapping - Raycasts from last collision point instead of center point, since last collision is closer to the ground
        //      Allows for a much smaller snapping distance, reducing unwanted snapping during normal landings
        if (!_controller.isGrounded && _velocityFinal.y <= 0)
        {
            RaycastHit hitInfo = new RaycastHit();
            if (Physics.Raycast(new Ray(transform.TransformPoint(_lastCollisionLocal), Vector3.down), out hitInfo, snapDistance))
            {
                Vector3 tDiff = hitInfo.point - transform.TransformPoint(_lastCollisionLocal);
                _controller.Move(tDiff);
            }

        }
    }

    // Run every physics iteration
    void FixedUpdate () {
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

        Debug.Log("Local Collision: " + _lastCollisionLocal);

        // Collision Friction
        if (!_controller.isGrounded)
        {
            _velocityFinal.x -= _velocityFinal.x * Time.deltaTime * 5f;
            _velocityFinal.z -= _velocityFinal.z * Time.deltaTime * 5f;
            _velocityFinal.y -= _velocityFinal.y * Time.deltaTime * 5f;

           // Debug.Log("FRICTION! " + _velocityFinal.y + ", Delta: " + (_velocityFinal.y * Time.deltaTime * 5f));
        }
        else
        {
            _timeAirborne = 0;
            _canAirJump = true;
        }

    }

    void OnTriggerEnter(Collider collider)
    {
        if (_triggers == null) return;
        
        string tName = collider.transform.name;
        _triggers.Add(tName);

        _doCatchFall = true;
        SetGrounded();
    }

    void OnTriggerExit(Collider collider)
    {
        if (_triggers == null) return;
        
        string tName = collider.transform.name;
        _triggers.Remove(tName);

        if (_triggers.Count == 0)
        {
            _doCatchFall = false;
            SetGrounded(false);
            
        }
    }

        void OnCollisionEnter(Collision collision)
    {
        if (_colliders == null) return;

        string tName = collision.collider.transform.name;
        int tColliderStart = _colliders.Count;
        Debug.Log("Collision with " + tName);

        //_velocityFinal.x = 0;
        //_velocityFinal.z = 0;
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

        SetGrounded();

        _timeAirborne = 0f;

        /*if(!_isGrounded)
        {
            CalculateGrounded();
        }*/
        //ContactPoint contact = collision.contacts[0];

        //Debug.Log("Collision: " + _body.velocity);
    }

    void OnCollisionExit(Collision collision)
    {
        if (_colliders == null) return;

        string tName = collision.collider.transform.name;
        _colliders.Remove(tName);

        /*if(_colliders.Count == 0)
        {
            _isGrounded = false;
        }*/

        Debug.Log("Exit: " + tName + ", " + _colliders.Count);
        

        /*if (_isGrounded)
        {
            CalculateGrounded();
        }*/
        //ContactPoint contact = collision.contacts[0];

        //Debug.Log("Collision: " + _body.velocity);
    }

    void _performCustomMovement()
    {
        if (!_controller.isGrounded) { _timeAirborne += Time.deltaTime; }
        _cooldownJump -= Time.deltaTime;

        if (_doCatchFall)
        {
        }

        // Glide - Coursing catches fall
        if (_courseStrength > 0)
        {
            _timerGliding += Time.deltaTime;
            if(_timerGliding > _GLIDE_CATCH_TIME_MAX) { _timerGliding = _GLIDE_CATCH_TIME_MAX; }

            if(_velocityFinal.y < 0)
            {
                float tMaxCourseFall = -1f; // 1m/s
                _velocityFinal.y = Mathf.Lerp(_velocityFinal.y, tMaxCourseFall, _timerGliding / _GLIDE_CATCH_TIME_MAX);
            }
        } else
        {
            _timerGliding = 0;
        }

        return;

        // CHECK GROUNDED
        float rayLength = 1f;
        float standMultiplier = 200f;
        float rayPenetration = 0f;
        float rayOverExtension = .5f;

        Vector3 tPosition = transform.position + new Vector3(0.0f, 0.0f, 0.0f);
        if (Physics.Raycast(tPosition, Vector3.down, out _groundHit, rayLength))
        {
            _isGrounded = true;
            _rayGrounded = true;

            Debug.DrawRay(tPosition, Vector3.down, Color.green);
            _lineRenderer.startColor = Color.green;
            _lineRenderer.endColor = Color.green;
            _lineRenderer.SetPosition(0, tPosition);
            _lineRenderer.SetPosition(1, tPosition + new Vector3(0, -rayLength + rayOverExtension));


            rayPenetration = (rayLength - _groundHit.distance) / rayLength;
            rayPenetration -= rayOverExtension;
            if (rayPenetration < 0) { rayPenetration = 0; }

            // Stand Up!
            var tStandingForce = Vector3.up * rayPenetration * standMultiplier; // 100 = Water Bob
            _body.AddForce(tStandingForce, ForceMode.Force);

            // var tStandingForce = Vector3.up * -Physics.gravity.y * (1 / Time.fixedDeltaTime) * 1.7f;
            /*if(tStandingForce.y > 2)
             {
                 _body.AddForce(tStandingForce, ForceMode.VelocityChange);
             }*/

            //Debug.Log("STANDING FORCE: " + tStandingForce.y + ", Pen: " + rayPenetration);
        }
        else
        {
            SetGrounded(false);
            _rayGrounded = false;

            Debug.DrawRay(tPosition, Vector3.down, Color.red);
            _lineRenderer.startColor = Color.red;
            _lineRenderer.endColor = Color.red;
            _lineRenderer.SetPosition(0, tPosition);
            _lineRenderer.SetPosition(1, tPosition + new Vector3(0, -rayLength + rayOverExtension));
        }

        if (!_isGrounded && _rayGrounded)
        {
            SetGrounded(true);
        }
    }

        // Perform movement based on velocity variable
        void _performMovement() {

        if(!_isGrounded)  { _timeAirborne += Time.deltaTime; }
        _cooldownJump -= Time.deltaTime;

        // Gravity - Falling force
        if(_doCatchFall)  {
            if (_body.velocity.y < 0) { NullifyVelocityY(); }
        } else {
            _body.AddForce(Physics.gravity, ForceMode.Acceleration);
        }

        Vector3 tVelocity = _body.velocity;

        // Glide - Coursing catches fall
        if (_courseStrength > 0 && _body.velocity.y < 0)
        {
            // *** PID *** Gradual catch
            float tMaxCourseFall = -1f; // 1m/s
            if(_body.velocity.y < tMaxCourseFall)
            {
                
                tVelocity.y = tMaxCourseFall;

                _body.velocity = tVelocity;
            }
        }

        // NIGHT IS 131C1C

        // CALCULATE GROUND ANGLE
        Vector3 tNormal = Vector3.up;

        //float tDistToGround = _body.GetComponent<Collider>().bounds.size.y - _body.GetComponent<Collider>().bounds.center.y;

        // Raycast to Floor
        Ray tRay = new Ray(transform.position, Vector3.down);
        /*if (Physics.Raycast(Vector3.zero, Vector3.down, 2f))
        {
            tNormal = tHit.normal;
            Debug.DrawLine(Vector3.zero, Vector3.)
        }*/

        float rayLength = .5f;
        float standMultiplier = 200f;
        float rayPenetration = 0f;
        float rayOverExtension = .5f;

        Vector3 tPosition = transform.position + new Vector3(0.0f, 0.0f, 0.0f);
        if (Physics.Raycast(tPosition, Vector3.down, out _groundHit, rayLength)) {
            _isGrounded = true;
            _rayGrounded = true;

           /* Debug.DrawRay(tPosition, Vector3.down, Color.green);
            _lineRenderer.startColor = Color.green;
            _lineRenderer.endColor = Color.green;
            _lineRenderer.SetPosition(0, tPosition);
            _lineRenderer.SetPosition(1, tPosition + new Vector3(0, -rayLength + rayOverExtension));

            rayPenetration = (rayLength - _groundHit.distance) / rayLength;
            rayPenetration -= rayOverExtension;
            if(rayPenetration < 0) { rayPenetration = 0; }

            // Stand Up!
            var tStandingForce = Vector3.up * rayPenetration * standMultiplier; // 100 = Water Bob
            _body.AddForce(tStandingForce, ForceMode.Force);*/

            // var tStandingForce = Vector3.up * -Physics.gravity.y * (1 / Time.fixedDeltaTime) * 1.7f;
            /*if(tStandingForce.y > 2)
             {
                 _body.AddForce(tStandingForce, ForceMode.VelocityChange);
             }*/

            //Debug.Log("STANDING FORCE: " + tStandingForce.y + ", Pen: " + rayPenetration);
        } else {
            SetGrounded(false);
            _rayGrounded = false;

           /* Debug.DrawRay(tPosition, Vector3.down, Color.red);
            _lineRenderer.startColor = Color.red;
            _lineRenderer.endColor = Color.red;
            _lineRenderer.SetPosition(0, tPosition);
            _lineRenderer.SetPosition(1, tPosition + new Vector3(0, -rayLength + rayOverExtension));*/
        }

        if(!_isGrounded && _rayGrounded)
        {
            SetGrounded(true);
        }

        //Ray tRay = new Ray(transform.position, Vector3.down); // Camera.main.ScreenPointToRay(Input.mousePosition);
        //RaycastHit tHit;

        //if (Physics.Raycast(transform.position, -Vector3.up, out _groundHit, 100.0f)) {
        //Debug.DrawLine(tRay.origin, tHit.point);_groundHit

        //}

        //Debug.Log(tHit.normal);


        /*function Start()
            {
                distToGround = collider.bounds.extents.y - collider.center.y;Debug.Log("Hit");
            }

            function Update()
            {
                // assume a range = distToGround + 0.2
                if (Physics.Raycast(transform.position, -transform.up, hit, distToGround + 0.2))
                {
                    normal = hit.normal;
                }
            }*/

        /*
         * NIGHT 
         * Light 000810FF 1.14 */

        // *** GROUND SLOPE ***
        /*if (_isGrounded)
        {
            _lean.x += tHit.normal.x;
            _lean.z += tHit.normal.z;

        }*/

        // > Faster downhill
        // > Smoother downhill
        // + No momentum
        // + Complete Control
        // + Full Air Control
        // + No Drag
        // + No Run button that needs to be constantly held
        //_lean.y = _body.velocity.y;

        //_body.MovePosition(transform.position + (_lean * Time.fixedDeltaTime));

        //tVelocity = _body.velocity;
        //tVelocity += _lean * Time.fixedDeltaTime;
        //_body.velocity = tVelocity;

        /*var tDampener = GetComponent<ControlDampener>();
        if (tDampener != null)
        {
            if (tDampener.Multiplier != 1.0f)
            {
                // *** PID *** Allows for increasing control 
            }
            else
            {
                // *** PID *** Modify velocity instead of set 
                _body.MovePosition(transform.position + (_lean * Time.fixedDeltaTime));
            }
        }*/

    }

	void _performRotation() {

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
        //_controller.MoveRotation (_body.rotation * Quaternion.Euler(_rotate));
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

        Debug.Log("JUMP!");
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

        //return (_isGrounded || _timeAirborne < .2f) && _cooldownJump < 0f;
        // Check if player just landed and automated check hasn't updated
        //CalculateGrounded();
        //return Physics.Raycast(_body.GetComponent<Transform>().position, new Vector3(0, -1, 0), _distToGround + .2f);

        //return true;

        Debug.Log("Can Jump? " + _timeAirborne + ", " + _controller.isGrounded + ", " + _cooldownJump);
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
