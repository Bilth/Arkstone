using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class EnemySpawner : NetworkBehaviour
{

    public GameObject enemyPrefab;
    public int numberOfEnemies;

    private float _timer;

    public override void OnStartServer()
    {
        _timer = Time.time + 3f;

        for (int i = 0; i < numberOfEnemies; i++)
        {
            SpawnEnemy();
        }
    }

    public void Update()
    {
        /*if (_timer < Time.time)
        {
            _timer = Time.time + 5f;

            SpawnEnemy();
        }*/
    }

    public void SpawnEnemy()
    {

        Vector3 spawnPosition = new Vector3(-20f, -1f, 20f); // new Vector3(Random.Range(-8f, 8f), 0f, Random.Range(-8f, 8f));
        Quaternion spawnRotation = Quaternion.Euler(0f, Random.Range(0f, 180f), 0f);

        GameObject enemy = (GameObject)Instantiate(enemyPrefab, spawnPosition, spawnRotation);
        NetworkServer.Spawn(enemy);
    }
}
