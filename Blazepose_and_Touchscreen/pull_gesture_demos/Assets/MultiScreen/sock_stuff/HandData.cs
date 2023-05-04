using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandData : MonoBehaviour
{
    public SocketClient_Cam sockData;
    // queried variables
    public bool isPinched;
    public Vector3 indexPos;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (sockData.handpoints[21, 0] > 0.0f) { isPinched = true; }
        else { isPinched = false; }
        indexPos = new Vector3(sockData.handpoints[8, 0], sockData.handpoints[8, 2], sockData.handpoints[8, 1]);
    }
}
