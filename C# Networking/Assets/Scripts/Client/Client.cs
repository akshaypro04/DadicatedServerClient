using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;

public class Client : MonoBehaviour
{
    public static Client Instances;
    public static int dataBufferSize = 4096;

    public string IP = "127.0.0.1";
    public int Port = 2033;
    public int MyID;
    public TCP tcp;
    public UDP udp;

    private bool IsConnected = false;
    private delegate void PacketHandler(Packet _packet);
    private static Dictionary<int, PacketHandler> packetHandlers;


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


    private void OnApplicationQuit()
    {
        Disconnect();
    }


    public void ConnectToServer()                                                       // Connect to server btn
    {
        tcp = new TCP();
        udp = new UDP();

        InitializeClientData();
        IsConnected = true;
        tcp.Connect();
    }

    public class TCP
    {
        public TcpClient socket;

        private NetworkStream stream;
        private Packet packet;
        private byte[] receviceBuffer;

        public void Connect()
        {
            socket = new TcpClient                                                                  // like set property of TCPClient
            {
                ReceiveBufferSize = dataBufferSize,
                SendBufferSize = dataBufferSize,
            };

            receviceBuffer = new byte[dataBufferSize];
            socket.BeginConnect(Instances.IP, Instances.Port, ConnectCallBack, socket);                                   // only ones
        }

        private void ConnectCallBack(IAsyncResult result)                                          // client connect Server Now Callback
        {
            socket.EndConnect(result);

            if (!socket.Connected)
            {
                return;
            }

            stream = socket.GetStream();
            packet = new Packet();

            stream.BeginRead(receviceBuffer, 0, dataBufferSize, ReceviceCallBack, null);
        }

        private void ReceviceCallBack(IAsyncResult result)
        {
            try
            {
                int _byteLength = stream.EndRead(result);
                if (_byteLength <= 0)
                {
                    Instances.Disconnect();                                                                                 // only close socket
                    return;
                }

                byte[] _data = new byte[_byteLength];
                Array.Copy(receviceBuffer, _data, _byteLength);

                packet.Reset(HandleData(_data));
                stream.BeginRead(receviceBuffer, 0, dataBufferSize, ReceviceCallBack, null);                                 // loop throw
            }
            catch (Exception e)
            {
                DisConnect();
                Console.WriteLine($"Error Receiving Data :{ e }");
            }
        }

        private bool HandleData(byte[] data)
        {
            int _packetLenght = 0;
            packet.SetBytes(data);

            if(packet.UnreadLength() >= 4)
            {
                _packetLenght = packet.ReadInt();
                if(_packetLenght <= 0)
                {
                    return true;
                }
            }

            while (_packetLenght > 0 && _packetLenght <= packet.UnreadLength())
            {
                byte[] _packetBytes = packet.ReadBytes(_packetLenght);
                ThreadMananager.ExecuteOnMainThread(() =>
                {
                    using (Packet packet = new Packet(_packetBytes))
                    {
                        int _packetID = packet.ReadInt();
                        packetHandlers[_packetID](packet);
                    }
                });

                _packetLenght = 0;
                if(packet.UnreadLength() >= 4)
                {
                    _packetLenght = packet.ReadInt();
                    if(_packetLenght <= 0)
                    {
                        return true;
                    }
                }
            }

            if(_packetLenght <= 1)
            {
                return true;
            }

            return false;
        }

        public void SendDataTCP(Packet _packet)
        {
            try
            {
                if(socket != null)
                {
                    stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
                }
            }
            catch(Exception e)
            {
                print("Error sending data to server :" + e);
            }
        }

        private void DisConnect()
        {
            Instances.Disconnect();

            stream = null;
            packet = null;
            receviceBuffer = null;
            socket = null;
        }

    }


    public class UDP
    {
        public UdpClient socket;
        public IPEndPoint endPoint;

        public UDP()
        {
            endPoint = new IPEndPoint(IPAddress.Parse(Instances.IP), Instances.Port);
        }

        public void Connect(int _localPort)                                         // client Connected via IPEndPoint from ClientHandler Welcome Scripts
        {
            socket = new UdpClient(_localPort);
            socket.Connect(endPoint);
            socket.BeginReceive(ReceivceCallBacks, null);
        
            using(Packet _packet = new Packet())
            {
                SendDataUDP(_packet);
            }
        }                                   

        public void SendDataUDP(Packet _packet)
        {
            try
            {
                _packet.InsertInt(Instances.MyID);
                if (socket != null)
                {
                    socket.BeginSend(_packet.ToArray(), _packet.Length(), null, null);
                }
            }
            catch (Exception e)
            {
                print("Error sending Data to server via UDP: " + e);
            }
        }

        private void ReceivceCallBacks(IAsyncResult result)
        {
            try
            {
                byte[] _data = socket.EndReceive(result, ref endPoint);
                socket.BeginReceive(ReceivceCallBacks, null);
            
                if(_data.Length < 4)
                {
                    Instances.Disconnect();
                    return;
                }

                HandleData(_data);
            
            }
            catch(Exception e)
            {
                Disconnect();
            }
        }

        private void HandleData(byte[] _data)
        {
            using(Packet _packet = new Packet(_data))
            {
                int _packetLength = _packet.ReadInt();
                _data = _packet.ReadBytes(_packetLength);
            }

            ThreadMananager.ExecuteOnMainThread(() =>
            {
                using(Packet _packet = new Packet(_data))
                {
                    int _packetid = _packet.ReadInt();
                    packetHandlers[_packetid](_packet);
                }
            });
        }

        private void Disconnect()
        {
            Instances.Disconnect();

            endPoint = null;
            socket = null;
        }

    }

    public void InitializeClientData()
    {
        packetHandlers = new Dictionary<int, PacketHandler>()
        {
            {(int)ServerPackets.welcome,ClientHandle.welcome },
            {(int)ServerPackets.spawnPlayer,ClientHandle.SpawnPlayer },
            {(int)ServerPackets.playerPosition,ClientHandle.PlayerPostion },
            {(int)ServerPackets.playerRotation,ClientHandle.PlayerRotation },
            {(int)ServerPackets.playerDisconnect,ClientHandle.PlayerDisconnect },
            {(int)ServerPackets.playerHealth,ClientHandle.playerHealth },
            {(int)ServerPackets.playerRespawned,ClientHandle.PlayerReSpawned },
            {(int)ServerPackets.createItemSpawnner,ClientHandle.CreateItemSpawnner },
            {(int)ServerPackets.ItemSpawned,ClientHandle.ItemSpawned  },
            {(int)ServerPackets.itemPicked,ClientHandle.ItemPickedUp  },
            {(int)ServerPackets.SpawnProjectile,ClientHandle.SpawnProjectile  },
            {(int)ServerPackets.projectilePosition,ClientHandle.ProjectilePosition  },
            {(int)ServerPackets.projectileExploded,ClientHandle.ProjectileExploded  },
            {(int)ServerPackets.SpawnEnemy,ClientHandle.SpawnEnemy  },
            {(int)ServerPackets.enemyPosition,ClientHandle.EnemyPosition  },
            {(int)ServerPackets.enemyHealth,ClientHandle.EnemyHealth  }
        };
        print("Initialized Packet....");
    }

    void Disconnect()
    {
        if (IsConnected)
        {
            IsConnected = false;
            tcp.socket.Close();
            udp.socket.Close();

            print("DisConnect From Server");
        }

    }
}