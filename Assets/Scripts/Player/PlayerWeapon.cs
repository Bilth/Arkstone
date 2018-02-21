using UnityEngine;

// Allows it to show up as a variable in the inspect
[System.Serializable]
public class PlayerWeapon : MonoBehaviour {
    public string weaponName = "Wand";
    public int damage = 50;
    public float range = 200f;
}
