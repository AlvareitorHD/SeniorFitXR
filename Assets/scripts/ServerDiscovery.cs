using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class ServerDiscovery : MonoBehaviour
{
    public static ServerDiscovery Instance;
    public string serverUrl;

    private Thread listenerThread;
    private bool running;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            StartListening();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void StartListening()
    {
        running = true;
        listenerThread = new Thread(Listen);
        listenerThread.IsBackground = true;
        listenerThread.Start();
    }

    void Listen()
    {
        UdpClient client = new UdpClient(41234);
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);

        while (running)
        {
            try
            {
                byte[] data = client.Receive(ref endPoint);
                string message = Encoding.UTF8.GetString(data);

                Debug.Log("Broadcast recibido: " + message);

                if (message.Contains("ip"))
                {
                    var parsed = JsonUtility.FromJson<ServerMessage>(message);
                    serverUrl = $"https://{parsed.ip}:{parsed.port}";
                    Debug.Log("Servidor detectado en: " + serverUrl);
                    running = false; // Una vez encontrado, para de escuchar
                }
            }
            catch (SocketException ex)
            {
                Debug.LogError("Error UDP: " + ex.Message);
            }
        }

        client.Close();
    }

    void OnDestroy()
    {
        running = false;
        listenerThread?.Abort();
    }

    [System.Serializable]
    public class ServerMessage
    {
        public string ip;
        public int port;
    }
}
