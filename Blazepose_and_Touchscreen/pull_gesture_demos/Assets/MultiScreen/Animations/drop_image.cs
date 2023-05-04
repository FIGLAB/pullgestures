using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class drop_image : MonoBehaviour {
    public GameObject robotpic;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("s")) { robotpic.SetActive(true); }
        if (Input.GetKeyDown("d")) { robotpic.SetActive(false); }
    }
}
