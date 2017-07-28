using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class PlayerManager : NetworkBehaviour {

    [SyncVar]
    private bool _isDead = false;
    public bool isDead
    {
        get { return _isDead; }
        protected set { _isDead = value; } //Only the PlayerManager or classes that derive from it can change this variable, protected
    }

    public bool destroyOnDeath;

   // SHORTCUT Can't do it this way, because we can't set it as a SyncVar
   // public bool isDead { get; protected set; }

    [SerializeField]
    private int _healthMax = 100;

    public RectTransform healthBar;

    [SerializeField]
    private Behaviour[] disableOnDeath;
    private bool[] wasEnabled;

    [SyncVar(hook ="OnChangeHealth")]
    private int _healthCurrent;

    // For enemy units
    public void Start ()
    {
        isDead = false;

        _healthCurrent = _healthMax;
    }
    
	public void Setup () { // Called when player setup is ready, not on awake
        wasEnabled = new bool[disableOnDeath.Length];
        for (int i = 0; i < wasEnabled.Length; i++) // SHORTCUT type for then tab twice to do this by default
        {
            if(disableOnDeath[i] != null)
            {
                wasEnabled[i] = disableOnDeath[i].enabled;
            }
            
        }

        SetDefaults();
	}

    public void SetDefaults()
    {
        isDead = false;

        _healthCurrent = _healthMax;

        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            if(disableOnDeath[i] != null)
            {
                disableOnDeath[i].enabled = wasEnabled[i];
            }
            
        }

        // Unitys colliders can be enabled/disabled but derived from Component not Behavior, so we can't store this in disable array

        Collider tCollider = GetComponent<Collider>();
        if(tCollider != null)
        {
            tCollider.enabled = true;
        }
    }

    /* void Update() // Test method for killing self
     {
         if(!isLocalPlayer)
         {
             return;
         }

         if(Input.GetKeyDown(KeyCode.K))
         {
             RpcTakeDamage(200);
         }
     }*/
     
    void OnChangeHealth(int _healthCurrent)
    {
        healthBar.sizeDelta = new Vector2(_healthCurrent, healthBar.sizeDelta.y);
    }

    // Ctrl RR replace all
    // RPC Call - Used to ensure a method is called on all clients connected to the server
    [ClientRpc] 
    public void RpcTakeDamage(int pAmount)
    {
        if (isDead) return;

        _healthCurrent -= pAmount;
        if(_healthCurrent <= 0)
        {
            _healthCurrent = 0;
        }

        Debug.Log(transform.name + " now has " + _healthCurrent + " health.");

        if(_healthCurrent <= 0)
        {
            Die();
        }
    }

    public void TakeDamage(int pAmount)
    {
        if (!isServer) return;
        if (isDead) return;

        _healthCurrent -= pAmount;
        if (_healthCurrent <= 0)
        {
            _healthCurrent = 0;
        }

        Debug.Log(transform.name + " now has " + _healthCurrent + " health.");

        if (_healthCurrent <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        // Disable Components
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }

        // Disable Collider
        Collider tCollider = GetComponent<Collider>();
        if (tCollider != null)
        {
            tCollider.enabled = false;
        }

        Debug.Log(transform.name + " is DEAD!");

        if(destroyOnDeath)
        {
            Destroy(gameObject);
        } else
        {
            // Call Respawn Method
            StartCoroutine(Respawn()); //StartCoroutine needed for Wait
        }
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnTime);

        SetDefaults();
        Transform tSpawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = tSpawnPoint.position;
        transform.rotation = tSpawnPoint.rotation;

        Debug.Log(transform.name + " respawned.");
    }
}
