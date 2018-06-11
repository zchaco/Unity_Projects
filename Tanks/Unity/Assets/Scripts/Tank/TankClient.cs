using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class TankClient : MonoBehaviour {

    Thread mThread;
    public string connectionIP = "127.0.0.1";
    public int connectionPort = 25001;
    IPAddress localAdd;
    TcpListener listener;
    TcpClient client;
    Vector3 pos = Vector3.zero;

    bool running;
	// Use this for initialization
	void Start () {
        ThreadStart ts = new ThreadStart(GetInfo);
        mThread = new Thread(ts);
        mThread.Start();

	}
	
	// Update is called once per frame
	void Update () {
        transform.position = pos;
	}

    public static string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        Debug.Log(host.HostName);
        foreach(var ip in host.AddressList)
        {
            if(ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        throw new System.Exception("No Network adapters with IPV4 address in system!");
    }

    void GetInfo()
    {
        localAdd = IPAddress.Parse(connectionIP);
        listener = new TcpListener(IPAddress.Any, connectionPort);
        listener.Start();

        client = listener.AcceptTcpClient();

        running = true;
        while (running)
        {
            Connection();
        }
        listener.Stop();
    }

    void Connection()
    {
        NetworkStream nwStream = client.GetStream();
        byte[] buffer = new byte[client.ReceiveBufferSize];

        int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);
        string dataReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead);

        if (dataReceived != null)
        {
            if (dataReceived == "stop")
            {
                running = false;
            }
            else
            {
                pos = 10f * StringToVector3(dataReceived);
                print("moved");
                nwStream.Write(buffer, 0, bytesRead);
            }
        }
    }

    public static Vector3 StringToVector3(string sVector)
    {
        //Remove parenthesis
        if(sVector.StartsWith("(") && sVector.EndsWith(")"))
        {
            sVector = sVector.Substring(1, sVector.Length - 2);
        }

        string[] sArray = sVector.Split(',');

        //store as a vector3
        Vector3 result = new Vector3(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]),
            float.Parse(sArray[2])
            );

        return result;   
    }

}   
