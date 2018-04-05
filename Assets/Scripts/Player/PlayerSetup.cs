using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.VR;

[RequireComponent(typeof(PlayerManager))]
public class PlayerSetup : NetworkBehaviour {

	[SerializeField]
	Behaviour[] _componentsToDisable;

    [SerializeField]
    string remoteLayerName = "RemotePlayer";

    [SerializeField]
    string dontDrawLayerName = "DontDraw";

    [SerializeField]
    GameObject playerGraphics;

    [SerializeField]
    GameObject playerUIPrefab;
    private GameObject playerUIInstance;

	private Camera _sceneCamera;

	void Start() {
        //VRSettings.enabled = false;

		if (!isLocalPlayer) {
            DisableComponents();
            AssignRemoteLayer();
        } else {
			_sceneCamera = Camera.main;
			if(_sceneCamera != null) {
				_sceneCamera.gameObject.SetActive(false);
			}

            // Disable Player Graphics for local player and all children
            SetLayerRecursively(playerGraphics, LayerMask.NameToLayer(dontDrawLayerName));

            // Create PlayerUI
            playerUIInstance = Instantiate(playerUIPrefab);
            playerUIInstance.name = playerUIPrefab.name;

        }

        GetComponent<PlayerManager>().Setup();

       //RegisterPlayer();
    }

    // Will Set Layer of Object and all child objects
    void SetLayerRecursively(GameObject pObject, int pNewLayer)
    {
        if(!pObject.layer.Equals("Draw"))
        {
            pObject.layer = pNewLayer;
        }
       
        foreach(Transform iChild in pObject.transform)
        {
            SetLayerRecursively(iChild.gameObject, pNewLayer);
        }
    }

    // OnStartClient is part of the NetworkBehavior class and is triggered when a client joins a server
    public override void OnStartClient()
    {
        base.OnStartClient();

        // Net ID is a part of NetworkBehavior so it will exist
        string tNetID = GetComponent<NetworkIdentity>().netId.ToString();
        PlayerManager _player = GetComponent<PlayerManager>(); //We require this above

        GameManager.RegisterPlayer(tNetID, _player);
    }

    void AssignRemoteLayer()
    {
        // Set layer to remote player
        gameObject.layer = LayerMask.NameToLayer(remoteLayerName);
    }

    void DisableComponents()
    {
        for (int i = 0; i < _componentsToDisable.Length; i++)
        {
            _componentsToDisable[i].enabled = false;
        }
    }

    // Unregister this player when it's killed
	void OnDisable() {
        Destroy(playerUIInstance);

		if (_sceneCamera != null) {
			_sceneCamera.gameObject.SetActive(true);
		}

        GameManager.UnRegisterPlayer(transform.name);
	}
}
