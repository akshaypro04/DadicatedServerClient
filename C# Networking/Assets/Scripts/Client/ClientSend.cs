using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSend : MonoBehaviour
{

    private static void SendTCPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.Instances.tcp.SendDataTCP(_packet);
    }
    private static void SendUDPData(Packet packet)
    {
        packet.WriteLength();
        Client.Instances.udp.SendDataUDP(packet);
    }


    ///////////     SEND DATA FROM CLIENT TO SERVER FORM OF PACKETS     ///////////        

    #region Packets                                                

    public static void welcomeReceived()
    {
        using (Packet _packet = new Packet((int)ClientPackets.welcomeReceived))
        {
            _packet.Write(Client.Instances.MyID);
            _packet.Write(UIManager.Instances.UserName.text);

            SendTCPData(_packet);
        }

    }
    
    public static void PlayerMovement(bool[] _inputs)
    {
        using(Packet _packet = new Packet((int)ClientPackets.playerMovement))
        {
            _packet.Write(_inputs.Length);
            foreach(bool input in _inputs)
            {
                _packet.Write(input);
            }

            _packet.Write(GameManager.players[Client.Instances.MyID].transform.rotation);

            SendUDPData(_packet);
        }
    }


    public static void PlayerShoot(Vector3 _facing)
    {
        using(Packet _packet = new Packet((int)ClientPackets.playerShoot))
        {
            _packet.Write(_facing);
            SendTCPData(_packet);
        }
    }


    public static void PlayerThrowItem(Vector3 _facing)
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerThrowItem))
        {
            print("packet is sending " + _facing);
            _packet.Write(_facing);
            SendTCPData(_packet);
        };
    }



    #endregion
}
