using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instances;

    public GameObject startMenu;
    public InputField UserName;

    private void Awake()
    {
        if(Instances == null)
        {
            Instances = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ConnectToServer()
    {
        startMenu.SetActive(false);
        UserName.interactable = false;
        Client.Instances.ConnectToServer();
    }

}
