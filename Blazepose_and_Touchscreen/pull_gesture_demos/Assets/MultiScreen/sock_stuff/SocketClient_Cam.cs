using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

public class SocketClient_Cam : MonoBehaviour
{
    private const string HostName = "172.26.110.197";
    private int socketPort = 5005;
    private Socket sclient;

    private byte[] sendBytes = Encoding.ASCII.GetBytes("gimme");
    private byte[] bytes;
    private int receiveBytes;
    public float[,] handpoints = new float[22, 3];

    // Start is called before the first frame update
    void Start()
    {
        sclient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPAddress ipAddress = Dns.GetHostEntry(HostName).AddressList[0];
        //sclient = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        /* Find correct AddressList index
         * IPHostEntry hostInfo = Dns.GetHostEntry(HostName);
         * for (int index = 0; index < hostInfo.AddressList.Length; index++)
         * {
         *    Debug.Log(hostInfo.AddressList[index]);
         * } */

        IPEndPoint ipe = new IPEndPoint(ipAddress, socketPort);
        try
        {
            sclient.Connect(ipe);
        }
        catch (ArgumentNullException ae)
        {
            Debug.Log("ArgumentNullException : " + ae.ToString());
        }
        catch (SocketException se)
        {
            Debug.Log("SocketException : " + se.ToString());
        }
        catch (Exception e)
        {
            Debug.Log("Unexpected exception : " + e.ToString());
        }
        Debug.Log("Is connected: " + sclient.Connected);

    }

    // Update is called once per frame
    void Update()
    {
        // Sends a message to the host to which you have connected.
        sclient.Send(sendBytes);
        bytes = new byte[66 * 4];
        // bytes = new byte[63 * 4];
        receiveBytes = sclient.Receive(bytes);
        for (var index = 0; index < 66; index++)
        // for (var index = 0; index < 63; index++)
        {
            handpoints[(int)(index / 3), index % 3] = BitConverter.ToSingle(bytes, index * sizeof(float));
        }
        Debug.Log(handpoints[21, 0]);
    }
}
