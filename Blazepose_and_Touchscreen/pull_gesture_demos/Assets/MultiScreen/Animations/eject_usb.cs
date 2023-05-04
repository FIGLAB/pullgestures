using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class eject_usb : MonoBehaviour {
    public Material usb;
    public Material nousb;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("s"))
        {
            this.GetComponent<Renderer>().material = nousb;
        }
        if (Input.GetKeyDown("d"))
        {
            this.GetComponent<Renderer>().material = usb;
        }
    }
}
