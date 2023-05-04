using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class clipboard : MonoBehaviour {
    public GameObject highlight1;
    public GameObject highlight2;

    // Use this for initialization
    void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("s")) { highlight1.SetActive(true); }
        if (Input.GetKeyDown("d")) { highlight1.SetActive(false); highlight2.SetActive(true); }
        if (Input.GetKeyDown("f")) { highlight1.SetActive(false); highlight2.SetActive(false); }
    }
}
