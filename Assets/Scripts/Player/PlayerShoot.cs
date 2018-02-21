using UnityEngine;
using UnityEngine.Networking;

public class PlayerShoot : NetworkBehaviour
{

    private const string PLAYER_TAG = "Player";

    [SerializeField]
    private PlayerWeapon weapon;

    [SerializeField]
    private GameObject bulletPrefab;

    [SerializeField]
    private Transform bulletSpawn;

    [SerializeField]
    private GameObject weaponGraphic;

    [SerializeField]
    private string weaponLayerName = "Weapon";

    [SerializeField]
    private Camera _cam;

    [SerializeField] //Controls what we hit
    private LayerMask _layerMask;

    private float _fireballCharge = 0f;
    
	void Start () {
	    if(_cam == null)
        {
            Debug.LogError("Player Shoot: No camera referenced!");
            this.enabled = false;
        }

        weaponGraphic.layer = LayerMask.NameToLayer(weaponLayerName);
	}
	
	void Update () {
	    if(Input.GetAxisRaw("Fireball") > 0) //Input.GetButtonDown("Fire1") || 
        {
            _fireballCharge += Time.deltaTime;
        } else
        {
            if(_fireballCharge > .2f)
            {
                CmdFire();
                _fireballCharge = 0f;
            }
        }
	}

    // Command the server to spawn a bullet for all clients
    [Command]
    void CmdFire()
    {
        // Create the Bullet from the Bullet Prefab
        var tBullet = (GameObject)Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);

        // Add Velocity to Bullet
        tBullet.GetComponent<Rigidbody>().velocity = tBullet.transform.forward * 50;
        //tBullet.GetComponent<Rigidbody>().AddForce(new Vector3(0, -Physics.gravity.y, 0), ForceMode.Impulse);

        // Spawn the bullet on the Clients
        NetworkServer.Spawn(tBullet);

        // Destroy the bullet after 2 seconds
        Destroy(tBullet, 3f);
    }
    
    [Client]
    private void Shoot() // OLD UNUSED
    {
        // Will store information on what we hit
        RaycastHit tHit;
        if (Physics.Raycast(_cam.transform.position, _cam.transform.forward, out tHit, weapon.range, _layerMask))
        {
            // We hit something
            Debug.Log("We hit " + tHit.collider.name + ", [" + tHit.transform.name + "]");

            // Could check if layer == RemotePlayer
            if (tHit.collider.tag == PLAYER_TAG)
            {
                CmdPlayerShot(tHit.collider.name, weapon.damage);
            }
        }
    }

    // Commands only called on the server
    [Command]
    void CmdPlayerShot(string pPlayerID, int pDamage)
    {
        Debug.Log(pPlayerID + " has been shot.");
        
        PlayerManager tPlayer = GameManager.GetPlayer(pPlayerID);
        tPlayer.RpcTakeDamage(pDamage);
        // GameObject.Find(pID);  Find is VERY slow. Use a dictionary instead
    }
}
