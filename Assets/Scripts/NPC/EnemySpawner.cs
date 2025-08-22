using UnityEngine;

public class EnemySpawnerInteractable : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform spawnPoint;

    private bool _playerIsNear = false;

    void Update()
    {
        if (_playerIsNear && Input.GetKeyDown(KeyCode.E))
        {
            SpawnAndStartBattle();
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _playerIsNear = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _playerIsNear = false;
        }
    }

    void SpawnAndStartBattle()
    {
        GameObject enemyObj = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
        Enemy enemy = enemyObj.GetComponent<Enemy>();
        if (enemy != null)
        {
            EnemyManager.Instance.RegisterEnemy(enemy);

            BattleManager.Instance.enemy = enemy;
            BattleManager.Instance.player.SetMyTurn(true); 
        }
    }
}