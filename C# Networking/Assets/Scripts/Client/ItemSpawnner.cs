using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawnner : MonoBehaviour
{
    public int spawnerID;
    public bool HasItem;
    public MeshRenderer itemModel;

    public float ItemRotationSpeed = 50f;
    public float ItemBobSpeed = 2f;
    private Vector3 basePosition;


    private void Update()
    {
        if (HasItem)
        {
            transform.Rotate(Vector3.up, ItemRotationSpeed * Time.deltaTime, Space.World);
            transform.position = basePosition + new Vector3(0f, 0.25f * Mathf.Sin(Time.time * ItemBobSpeed), 0);
        }
    }


    public void initislize(int _spawnerID, bool _Hasitem)
    {
        spawnerID = _spawnerID;
        HasItem = _Hasitem;
        itemModel.enabled = _Hasitem;

        basePosition = transform.position;
    }


    public void ItemSpawned()
    {
        HasItem = true;
        itemModel.enabled = true;
    }

    public void ItemPickedUp()
    {
        print("item picked up called");
        HasItem = false;
        itemModel.enabled = false;
    }

}
