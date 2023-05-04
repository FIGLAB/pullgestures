using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move_webpage : MonoBehaviour {
    public GameObject taskbar_top;
    public GameObject taskbar_bot;
    public Material roboMat;
    public Material desktopMat;
    public GameObject topScreen;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("s"))
        {
            taskbar_top.SetActive(true);
            taskbar_bot.SetActive(false);
            topScreen.GetComponent<Renderer>().material = desktopMat;
            this.GetComponent<Renderer>().material = roboMat;
        }
        if (Input.GetKeyDown("d"))
        {
            taskbar_top.SetActive(false);
            taskbar_bot.SetActive(true);
            topScreen.GetComponent<Renderer>().material = roboMat;
            this.GetComponent<Renderer>().material = desktopMat;
        }
    }
}

