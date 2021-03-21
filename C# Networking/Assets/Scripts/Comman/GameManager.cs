using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public static Dictionary<int, PlayerManager> players = new Dictionary<int, PlayerManager>();
    public static Dictionary<int, ItemSpawnner> itemSpawnner = new Dictionary<int, ItemSpawnner>();
    public static Dictionary<int, ProjectileManager> projectiles = new Dictionary<int, ProjectileManager>();
    public static Dictionary<int, EnemyManager> enemies = new Dictionary<int, EnemyManager>();

    public GameObject LocalPlayerPrefab;
    public GameObject playerPrefab;
    public GameObject ItemSpawnnerPerfabs;
    public GameObject projectileprefab;
    public GameObject enemiesPrefabs;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SpawnPlayer(int id, string userName, Vector3 position, Quaternion Rotation)             ///  server side looping
    {
        GameObject _player;

        print("the player id : " + id);
        
        if(id == Client.Instances.MyID)
        {
            _player = Instantiate(LocalPlayerPrefab, position, Rotation);
        }
        else
        {
            _player = Instantiate(playerPrefab, position, Rotation);
        }

        _player.GetComponent<PlayerManager>().Initialize(id, userName);
        players.Add(id, _player.GetComponent<PlayerManager>());
    }


    public void CreateItemSpawnner(int _SpawnnerID, Vector3 _position, bool _hasItem)
    {
        GameObject _spawner = Instantiate(ItemSpawnnerPerfabs, _position, ItemSpawnnerPerfabs.transform.rotation);
        _spawner.GetComponent<ItemSpawnner>().initislize(_SpawnnerID, _hasItem);
        itemSpawnner.Add(_SpawnnerID, _spawner.GetComponent<ItemSpawnner>());
    }

    public void SpawnProjectile(int _id, Vector3 _position)
    {
        GameObject _projectile = Instantiate(projectileprefab, _position, Quaternion.identity);
        _projectile.GetComponent<ProjectileManager>().Initialize(_id);
        projectiles.Add(_id, _projectile.GetComponent<ProjectileManager>());
    }

    public void SpawnEnemy(int _id, Vector3 _Pos)
    {
        GameObject _enemy = Instantiate(enemiesPrefabs, _Pos, Quaternion.identity);
        _enemy.GetComponent<EnemyManager>().Initialize(_id);
        enemies.Add(_id, _enemy.GetComponent<EnemyManager>());
    }









}
