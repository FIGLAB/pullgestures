using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class open_drawer : MonoBehaviour {
    private bool moving;
    private bool finish;
    private Vector3 origPos;
    public Material openFolder;
    public Material closeFolder;
    public GameObject folder;

	// Use this for initialization
	void Start () {
        origPos = new Vector3(-0.0399f, 0.0015f, 0.0281f);
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("s"))
        {
            moving = true;
            folder.GetComponent<Renderer>().material = openFolder;
        }
        if (moving)
        {
            this.transform.position -= new Vector3(0.0f, 0.0f, 0.001f);
            if (this.transform.position.z < -0.06f)
            {
                moving = false;
            }
        }
        if (Input.GetKeyDown("d")) { finish = true; }
        if (finish)
        {
            this.transform.position = this.transform.position + new Vector3(0.0f, 0.0f, 0.0025f);
            if (this.transform.position.z > 0.0329f)
            {
                finish = false;
                this.transform.position = origPos;
                folder.GetComponent<Renderer>().material = closeFolder;
            }
        }
    }
}
