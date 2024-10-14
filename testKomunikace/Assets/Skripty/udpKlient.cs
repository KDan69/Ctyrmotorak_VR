using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Newtonsoft.Json.Linq;

public class udpKlient : MonoBehaviour
{
    public string ipAddress = "127.0.0.1";  
    public int port = 4000; 
    public float interval = 0.1f;
    public GameObject koule;

    private UdpClient udpClient;
    private IPEndPoint serverEndPoint;
    private Thread receiveThread;
    private bool isRunning = false;

    float posX = 0;
    float posY = 0;
    float posZ = 0;

    void Start()
    {


        udpClient = new UdpClient();
        serverEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
        StartUdpCommunication();
    }

    public float smoothing = 5f; 

    void Update()
    {
        if (koule.transform != null)
        {
            koule.transform.position = Vector3.Lerp(koule.transform.position, new Vector3(posX/10, (posY/10)+13, posZ/10), Time.deltaTime * smoothing);
        }
    }

    private void StartUdpCommunication()
    {
        isRunning = true;
        receiveThread = new Thread(new ThreadStart(SendAndReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    private void SendAndReceiveData()
    {
        while (isRunning)
        {
            try
            {
                SendMessageToServer();
                string receivedData = ReceiveMessageFromServer();
                if (!string.IsNullOrEmpty(receivedData))
                {
                    ProcessReceivedData(receivedData);
                }
            }
            catch (SocketException e)
            {
                Debug.LogError("Socket Exception: " + e.Message);
            }
            Thread.Sleep((int)(interval * 1000));
        }
    }

    private void SendMessageToServer()
    {
        string message = "{\"get\": \"data\"}";
        byte[] data = Encoding.UTF8.GetBytes(message);
        udpClient.Send(data, data.Length, serverEndPoint);
    }

    private string ReceiveMessageFromServer()
    {
        string receivedData = "";
        if (udpClient.Available > 0)
        {
            byte[] receivedBytes = udpClient.Receive(ref serverEndPoint);
            receivedData = Encoding.UTF8.GetString(receivedBytes);
        }
        return receivedData;
    }

     private void ProcessReceivedData(string receivedData)
    {
        JObject jsonData = JObject.Parse(receivedData);
        JToken posData = jsonData["data"]?["pos"];
        if (posData != null)
        {
            posX = posData["x"]?.Value<float>() ?? 0;
            posY = posData["y"]?.Value<float>() ?? 0;
            posZ = posData["z"]?.Value<float>() ?? 0;
            Debug.Log($"Pozice: x={posX}, y={posY}, z={posZ}");
        }
        else
        {
            Debug.LogWarning("Chyba prijateho JSONu.");
        }
    }

    private void OnApplicationQuit()
    {
        isRunning = false;
        if (receiveThread != null && receiveThread.IsAlive)
        {
            receiveThread.Join(); 
        }
        udpClient.Close();
    }
}
