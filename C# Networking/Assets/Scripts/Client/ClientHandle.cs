using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class ClientHandle : MonoBehaviour
{
    public static void welcome(Packet _Packet)
    {
        string _msg = _Packet.ReadString();
        int _myID = _Packet.ReadInt();

        print("Message from Server " + _msg);
        Client.Instances.MyID = _myID;
        ClientSend.welcomeReceived();


        Client.Instances.udp.Connect(((IPEndPoint)Client.Instances.tcp.socket.Client.LocalEndPoint).Port);

    }

    public static void SpawnPlayer(Packet _packet)
    {
        int _id = _packet.ReadInt();
        string _username = _packet.ReadString();
        Vector3 _position = _packet.ReadVectors();
        Quaternion _rotation = _packet.ReadQuaternion();

        print("the server call spawn system :" + _id);

        GameManager.Instance.SpawnPlayer(_id, _username, _position, _rotation);
    }

    public static void PlayerPostion(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Vector3 _position = _packet.ReadVectors();
        
        if(GameManager.players.TryGetValue(_id, out PlayerManager player))
        {
            player.transform.position = _position;
        }
    }

    public static void PlayerRotation(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Quaternion _Rotation = _packet.ReadQuaternion();

        if(GameManager.players.TryGetValue(_id, out PlayerManager player))
        {
            player.transform.rotation = _Rotation;
        }

    }

    public static void PlayerDisconnect(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Destroy(GameManager.players[_id].gameObject);
        GameManager.players.Remove(_id);
    }

    public static void playerHealth(Packet _packet)
    {
        int id = _packet.ReadInt();
        float health = _packet.ReadFloat();
        GameManager.players[id].SetHealth(health);
    }


    public static void PlayerReSpawned(Packet _packet)
    {
        int id = _packet.ReadInt();
        GameManager.players[id].ReSpawn();
    }

    public static void CreateItemSpawnner(Packet _packet)
    {
        int SpawnnerID = _packet.ReadInt();
        Vector3 SpawnnerPos = _packet.ReadVectors();
        bool HasItem = _packet.ReadBool();

        GameManager.Instance.CreateItemSpawnner(SpawnnerID, SpawnnerPos, HasItem); 
    }

    public static void ItemSpawned(Packet _packet)
    {
        int _spawnerId = _packet.ReadInt();
        GameManager.itemSpawnner[_spawnerId].ItemSpawned();
    }

    public static void ItemPickedUp(Packet _Packet)
    {
        int _spawnerid = _Packet.ReadInt();
        int _ByPlayer = _Packet.ReadInt();

        print("item picked up " + _ByPlayer + " and " + _spawnerid);

        GameManager.itemSpawnner[_spawnerid].ItemPickedUp();
        GameManager.players[_ByPlayer].ItemCount++;
    }

    public static void SpawnProjectile(Packet _packet)
    {
        int _projectileID = _packet.ReadInt();
        Vector3 _position = _packet.ReadVectors();
        int _thrownByPlayer = _packet.ReadInt();

        GameManager.Instance.SpawnProjectile(_projectileID, _position);
        GameManager.players[_thrownByPlayer].ItemCount--;
    }


    public static void ProjectilePosition(Packet _packet)
    {
        int _projectileID = _packet.ReadInt();
        Vector3 _position = _packet.ReadVectors();

        if(GameManager.projectiles.TryGetValue(_projectileID, out ProjectileManager projectileManager))
        {
            projectileManager.transform.position = _position;
        }
    }

    public static void ProjectileExploded(Packet _packet)
    {
        int _projectileID = _packet.ReadInt();
        Vector3 _position = _packet.ReadVectors();

        GameManager.projectiles[_projectileID].Explode(_position);
    }

    public static void SpawnEnemy(Packet _packet)
    {
        int enemyId = _packet.ReadInt();
        Vector3 _pos = _packet.ReadVectors();

        GameManager.Instance.SpawnEnemy(enemyId, _pos);
    }

    public static void EnemyPosition(Packet _packet)
    {
        int enemyId = _packet.ReadInt();
        Vector3 _pos = _packet.ReadVectors();

        if (GameManager.enemies.TryGetValue(enemyId, out EnemyManager enemy))
        {
            enemy.transform.position = _pos;
        }
    }

    public static void EnemyHealth(Packet _packet)
    {
        int enemyId = _packet.ReadInt();
        float _health = _packet.ReadFloat();

        GameManager.enemies[enemyId].SetHealth(_health);
    }




}
 