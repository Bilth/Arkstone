using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{

    public static GameManager instance; // Instance, can easily get a reference to GameManager without having to do GameManager.find

    public MatchSettings matchSettings;

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Confined; // keep confined in the game window

        if (instance != null)
        {
            Debug.LogError("More than one GameManager in scene.");
        }
        else
        {
            instance = this;
        }
    }

    #region Player Tracking // SHORTCUT Collapsible

    private const string PLAYER_ID_PREFIX = "Player ";

    private static Dictionary<string, PlayerManager> _players = new Dictionary<string, PlayerManager>();

    public static void RegisterPlayer(string pNetID, PlayerManager pPlayer)
    {
        string tPlayerID = PLAYER_ID_PREFIX + pNetID;
        _players.Add(tPlayerID, pPlayer);
        pPlayer.transform.name = tPlayerID;
    }

    public static void UnRegisterPlayer(string pPlayerID)
    {
        _players.Remove(pPlayerID);
    }

    public static PlayerManager GetPlayer(string pPlayerID)
    {
        return _players[pPlayerID];
    }

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(200, 200, 200, 500));
        GUILayout.BeginVertical();

        foreach (string key in _players.Keys)
        {
            GUILayout.Label(key + " - " + _players[key].transform.name);
        }

        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    /* void RegisterPlayer()
     {
         string tID = "Player " + GetComponent<NetworkIdentity>().netId;
         transform.name = tID;
     }*/

    #endregion


}
