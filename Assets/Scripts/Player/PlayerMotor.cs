using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.VR;
using UnityEngine.Networking;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour {


    private float _TERMINAL_VELOCITY_HORIZONTAL = 3f;
    private float TIME_CHECK_GROUNDED = .2f;
    private float TIME_AIRBORNE_MAX = .5f;
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

    private AudioSource _audioSource;
    public AudioClip land;

    private float _speed;

    private Rigidbody _body;

    private bool _isGrounded = false;
    private float _courseStrength = 0f;
    private float _timerCheckAirborne = 0f;
    private float _timeAirborne = 0f;
    private float _cooldownJump = 0f;
    private float _gravity = 9.81f;

    [SerializeField]
    private float _cameraRotationLimit = 89f;
    private float _cameraRotationCharge;
    private List<string> _colliders;

	// Use this for initialization
	void Start () {
		_body = GetComponent<Rigidbody>();
        _audioSource = GetComponent<AudioSource>();

        _colliders = new List<string>();

        /* _lineRenderer = gameObject.AddComponent<LineRenderer>();
         _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
         _lineRenderer.startWidth = .2f;
         _lineRenderer.endWidth = .1f;
         _lineRenderer.widthMultiplier = 0.2f;
         _lineRenderer.positionCount = 2;
         _lineRenderer.sortingOrder = 1;
         _lineRenderer.SetPosition(0, transform.position);
         _lineRenderer.SetPosition(1, new Vector3(transform.position.x - 1.0f, transform.position.y));
         _lineRenderer.startColor = Color.yellow;
         _lineRenderer.endColor = Color.red;*/

        //DrawLine(transform.position, transform.position + new Vector3(0.0f, -1.0f), Color.red, 5);
        DrawLineHere(transform.position, transform.position + new Vector3(0.0f, -1.0f), Color.red, 5);

        //_velocity.y = 0;
        _isGrounded = false;
        _speed = SPEED_BASE;

        _distToGround = _body.GetComponent<Collider>().bounds.extents.y;
    }

    void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0.2f)
    {
        GameObject myLine = new GameObject();
        myLine.transform.position = Vector3.zero; // start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        lr.SetColors(color, color);
        lr.SetWidth(0.1f, 0.2f);
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        //GameObject.Destroy(myLine, duration);
    }


    void DrawLineHere(Vector3 start, Vector3 end, Color color, float duration = 0.2f)
    {
        gameObject.AddComponent<LineRenderer>();
        _lineRenderer = gameObject.GetComponent<LineRenderer>();
        _lineRenderer.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        _lineRenderer.startColor = Color.yellow;
        _lineRenderer.endColor = Color.red;
        _lineRenderer.startWidth = .1f;
        _lineRenderer.endWidth = .5f;
        _lineRenderer.SetPosition(0, start);
        _lineRenderer.SetPosition(1, end);
        //GameObject.Destroy(myLine, duration);
    }

    // Gets a movement vector
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

	// Run every physics iteration
	void FixedUpdate () {
        _courseStrength = Input.GetAxisRaw("Course");

        float tVelocityY = GetComponent<Rigidbody>().velocity.y;

        // Float
        if (_courseStrength > 0 && tVelocityY < 0) {
            float tReversingForce = -tVelocityY * .2f * _courseStrength; //ITEM VARIABLE (.1, .2, .3, .4)
            _body.AddForce(new Vector3(0, tReversingForce, 0), ForceMode.VelocityChange);
        }

        // Lift
        /*if(_courseStrength > 0)
        {
            _body.AddForce(new Vector3(0, _gravity * _courseStrength + , 0), ForceMode.VelocityChange);
        }*/

        /*if (_courseStrength > 0)
        {
            _body.AddForce(new Vector3(0, 0, 0));
        }*/

        //_speed = SPEED_BASE + _courseStrength * SPEED_BASE;

        //if (_isGrounded) {
        _speed = SPEED_BASE;// + (SPEED_BOOST * _courseStrength); //ITEM VARIABLE
       // }

        _performMovement ();
		_performRotation ();
	}

    void OnCollisionEnter(Collision collision)
    {
        if (_colliders == null) return;

        string tName = collision.collider.transform.name;
        int tColliderStart = _colliders.Count;
        Debug.Log("Collision with " + tName);
        _colliders.Add(tName);

        // While Airborne
        //if (_timeAirborne > .1f)
        {
            // Check Feet
            //if(Physics.Raycast(_body.GetComponent<Transform>().position, new Vector3(0, -1, 0), _distToGround + .2f))
            {
                _isGrounded = true;
                _timeAirborne = 0f;

                Debug.Log("Velocity Y: " + collision.relativeVelocity.magnitude);

                // If falling fast, play a noise (dependent on velocity)
                if (tColliderStart == 0 && collision.relativeVelocity.magnitude > 15)
                {
                    _audioSource.time = .9f;
                    _audioSource.PlayOneShot(land, .1f);
                }
            }
        }

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

        if(_colliders.Count == 0)
        {
            _isGrounded = false;
        }

        Debug.Log("Exit: " + tName + ", " + _colliders.Count);
        

        /*if (_isGrounded)
        {
            CalculateGrounded();
        }*/
        //ContactPoint contact = collision.contacts[0];

        //Debug.Log("Collision: " + _body.velocity);
    }

    // Perform movement based on velocity variable
    void _performMovement() {

        // Debug.Log("RB Velocity Y: " + GetComponent<Rigidbody>().velocity.y);

        // Check Airborne Status
        /*_timerCheckAirborne -= Time.deltaTime;
        //if (_timerCheckAirborne < 0f) { CalculateGrounded(); _timerCheckAirborne += TIME_CHECK_GROUNDED; }
        if (isGrounded) {
            _timeAirborne = 0f;
        } else {
            _timeAirborne += Time.deltaTime;
        }*/

        if(!_isGrounded)
        {
            _timeAirborne += Time.deltaTime;
        }
        

        _cooldownJump -= Time.deltaTime;

        // Anti-Gravity
        // Vector3 _velocityModified = _velocity;
        // if(_velocity.y > 0) { _velocityModified.y *= _courseStrength; }
        //_velocity = _velocityModified;
        //float tLiftingForce = -GetComponent<Rigidbody>().velocity.y * _courseStrength;
        //Debug.Log("VY: " + GetComponent<Rigidbody>().velocity.y + ", Lifting Force: " + tLiftingForce);
        //_body.AddForce(new Vector3(0, tLiftingForce * _body.mass), ForceMode.Impulse);

        //if(_velocity.y >)

        /*if(_isGrounded)
        {
            _velocity.y = 0;
        } else
        {
            _velocity.y += GRAVITY * Time.deltaTime;
        }*/

        //Debug.Log("VY: " + _velocity.y);

        // MOVE-BASED VELOCITY (NO PHYSICS)
        /*if (_velocity != Vector3.zero) {
			// Performs collision checks when trying to move and easier to control than addForce
			_body.MovePosition(_body.position + _velocity * Time.deltaTime);
            Debug.Log("Z: " + Mathf.Round(GetComponent<Rigidbody>().velocity.z * 10000000000));
        }*/

        // ACHIEVE THRUST SPEED QUICKLY

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

        if (Physics.Raycast(transform.position, Vector3.down, out _groundHit, 2.0f)) {
            Debug.DrawRay(transform.position, Vector3.down, Color.red);
            //_lineRenderer.startColor = Color.yellow;
            //_lineRenderer.endColor = Color.green;
            _lineRenderer.SetPosition(0, transform.position);
            _lineRenderer.SetPosition(1, transform.position + new Vector3(0, -1.0f));
        } else
        {
            Debug.DrawRay(transform.position, Vector3.down, Color.blue);
            //_lineRenderer.startColor = Color.yellow;
            //_lineRenderer.endColor = Color.red;
            _lineRenderer.SetPosition(0, transform.position);
            _lineRenderer.SetPosition(1, transform.position + new Vector3(0, -1.0f));
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

        // LIMIT THRUST SPEED

        float _LEAN_ACCELERATION = 200f;
        //if (_lean != Vector3.zero)
        {
            //_lean.x += tHit.normal.x;
            //_lean.z += tHit.normal.z;
            //_lean.y = .2f;
            /* float tX = _lean.x;
            float tY = 0;
            float tZ = _lean.z;

           _velocityPlanar.x = _body.velocity.x;
            _velocityPlanar.z = _body.velocity.z;
            _velocityPlanar.y = 0;

            Debug.Log("LEAN: " + tX + ", " + tY + ", " + tZ + ", " + _velocityPlanar.magnitude);*/

            /*if (!isGrounded && (Input.GetAxisRaw("Course") > 0))
            {
                Debug.Log("NULLIFY FOR FLOAT");
                NullifyVelocityX();
                NullifyVelocityZ();
            }*/

            //if (_isGrounded)

            /*
             * NIGHT 
             * Light 000810FF 1.14
             * Scene 000000*/
            
            // Existing Momentum (Velocity
            _momentum = _body.velocity.normalized; // Use real player velocity
            _momentum.y = 0; // Ignore vertical velocity in calculations
            _momentumMagnitude = _body.velocity.magnitude;

            // *** GROUND SLOPE ***
            /*if (_isGrounded)
            {
                _lean.x += tHit.normal.x;
                _lean.z += tHit.normal.z;

            }*/

            //_momentum *= (_TERMINAL_VELOCITY_HORIZONTAL * _lean.magnitude); // Move based on lean


            //_body.velocity = _velocityWork;

            // *** MOMENTUM ***
            _momentum *= _momentumMagnitude;
            
            _momentum.y = _body.velocity.y; //Preserve Y
            _body.velocity = _momentum;

            // *** DRAG ***
            if(_timeAirborne < .5f)
            {
                //1.5f, .5f
                // As lean approaches 80, slow it down
                float tDrag = -.2f;// -.3f; // Of Magnitude, 0f for Riding Crystal
                float tSpeed = 1.3f;// + (_isGrounded ? (Input.GetAxisRaw("Course") * .25f) : 0); //1.5f, 1f: 7f for Riding Crystal
                float tGroundDrag = (tDrag * _momentumMagnitude) + tSpeed;
                if(tGroundDrag < 0) { tGroundDrag = 0; }
                //Debug.Log("Drag: " + tGroundDrag + ", Momentum: " + _momentumMagnitude);
                _body.velocity += (_lean * tGroundDrag);
            } else if (_colliders.Count == 0)
            {
                _body.velocity += (_lean * .1f); // Slight Air Movement
            }

            // *** CLIMBING ***

            // START HERE - Current is too fast top speed but slowing it down causes really fast warping possibly
            // because character is flipping negative and continues exponentially. Check if negative and ignore?
            // Also, factor in sprint/run button so you can accurately gauge a fast run
            /*
             * if(_isGrounded)
            {
                // As lean approaches 80, slow it down
                float tGroundDrag = (-.5f * _momentumMagnitude) + 4f; //0.25
                Debug.Log("Drag: " + tGroundDrag + ", Momentum: " + _momentumMagnitude);
                //if(_lean * tGroundDrag)
                _body.velocity += (_lean * 1f);
                _lean.z *= -1f;
                _lean.x *= -1f;
                _lean.y = 0;
                _body.velocity -= (_lean * tGroundDrag);
            }*/
 
            /*if (isGrounded)
            {
                if (_velocityWork.magnitude * _TERMINAL_VELOCITY_HORIZONTAL * _lean.magnitude > _TERMINAL_VELOCITY_HORIZONTAL)
                {
                   // Debug.Log("Velocity below threshold: " + _velocityWork.magnitude + " of " + _TERMINAL_VELOCITY_HORIZONTAL);
                    
                        _velocityWork *= (_TERMINAL_VELOCITY_HORIZONTAL * _lean.magnitude);
                        _velocityWork.y = _body.velocity.y; //Preserve Y
                        _body.velocity = _velocityWork;

                        Debug.Log("Set velocity to: " + _velocityWork);

                        //_body.
                        //_body.AddForce(_lean * _LEAN_ACCELERATION, ForceMode.Force);
                        //_body.velocity = _TERMINAL_VELOCITY_HORIZONTAL * (_body.velocity.normalized);
                
                } else
                {
                    _velocityWork.y = _body.velocity.y; //Preserve Y
                    _body.velocity = _velocityWork * _TERMINAL_VELOCITY_HORIZONTAL;// _lean * _TERMINAL_VELOCITY_HORIZONTAL;
                    //var tDiffX
                    //_body.AddForce(_lean * 5f, ForceMode.Force);
                }

                Debug.Log("Velocity: " + _body.velocity.magnitude);
            }*/

        }

        /*if(_thrusterForce != Vector3.zero)
        {
            //Force Mode acceleration takes mass out of the equation
            _body.AddForce(_thrusterForce, ForceMode.Acceleration);

            _thrusterForce = Vector3.zero;
        }*/
    }

	void _performRotation() {
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
        

        /*_cameraRotationCharge += (_rotate.y);
        Debug.Log("Rotation Charge Pre: " + _cameraRotationCharge);
        if (Mathf.Abs(_cameraRotationCharge) < 22.5f) {
            Debug.Log("Returning");
            return;
        }

        if(_cameraRotationCharge > 0)
        {
            _rotate.y = 22.5f;
            _cameraRotationCharge -= _rotate.y;
        } else
        {
            _rotate.y = -22.5f;
            _cameraRotationCharge -= _rotate.y;
        }

        Debug.Log("Rotation Charge Pos: " + _cameraRotationCharge);*/

        //_rotate.y += (_cameraRotationCharge < 0 ? -22.5f : 22.5f);// _cameraRotationCharge;
        //_cameraRotationCharge -= _rotate.y;

        _body.MoveRotation (_body.rotation * Quaternion.Euler(_rotate));
		if (_cam != null) {
            // Set our rotation and clamp it
            _cameraRotationXCurrent -= _cameraRotationX;
            _cameraRotationXCurrent = Mathf.Clamp(_cameraRotationXCurrent, - _cameraRotationLimit, _cameraRotationLimit);
           // Debug.Log("Camera Rotation X: " + _cameraRotationXCurrent);

            // Apply our rotation to the transform of the camera
            // Since camera is a child of player, it's always going to have only x rotation, with 0 y and z. No quaternians needed
            _cam.transform.localEulerAngles = new Vector3(_cameraRotationXCurrent, 0f, 0f);
			//_cam.transform.Rotate(-_cameraRotation);
		} else
        {
            Debug.Log("Camera is null");
        }
	}

    public float fallingForce()
    {
        return 0;
    }

    private void CalculateGrounded() {
        //if (_body.velocity.y > 0) { _isGrounded = false; return; }

        bool tGrounded = false;

        if (_body != null)
        {
            tGrounded = Physics.Raycast(_body.GetComponent<Transform>().position, new Vector3(0, -1, 0), _distToGround + .2f);
        }

        if(tGrounded)
        {
            //NullifyVelocity();
            if(!_isGrounded)
            {
                _isGrounded = true;
                _audioSource.time = .9f;
                _audioSource.PlayOneShot(land, .1f);
            }
            
        } else
        {
            _isGrounded = false;
        }
        /*if(_isGrounded)
        {
            _velocity.y = GRAVITY;
        }*/
    }

    public void CoolJump()
    {
        _cooldownJump = .2f;
    }

    public void Jump()
    {
        NullifyVelocityY();

        Vector3 tJumpForce = Vector3.up;
        //tJumpForce.x = _lean.x * .3f;// * .5f + _groundHit.normal.x;
        //tJumpForce.z = _lean.z;// * .5f + _groundHit.normal.z;
        tJumpForce *= 850f; // 650f
        _body.AddForce(tJumpForce, ForceMode.Impulse);
        CoolJump();
        //isGrounded = false;
        //_timeAirborne = 0f;
    }

    // Get a force vector for our thrusters
    public void applyThruster(Vector3 pForce)
    {
        Debug.Log("Apply Thruster Force: " + pForce);
        _thrusterForce = pForce;
    }

    public bool CanJump()
    {
        return (_isGrounded || _timeAirborne < .2f) && _cooldownJump < 0f;
        // Check if player just landed and automated check hasn't updated
        //CalculateGrounded();
        return Physics.Raycast(_body.GetComponent<Transform>().position, new Vector3(0, -1, 0), _distToGround + .2f);

        //return true;

        Debug.Log("Can Jump? " + _timeAirborne + ", " + _isGrounded + ", " + _cooldownJump);
        return _cooldownJump < 0f && (_timeAirborne < TIME_AIRBORNE_MAX || _isGrounded);
    }
    
    public Vector3 velocity
    {
        get { return _body.velocity; }
    }

    public bool isGrounded
    {
        get { return _isGrounded; }
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
