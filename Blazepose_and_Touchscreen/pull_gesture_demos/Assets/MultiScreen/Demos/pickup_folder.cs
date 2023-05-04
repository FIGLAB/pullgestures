using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class pickup_folder : MonoBehaviour {
    public ScreenTouch_Stand screenTop;
    public HandData hand;
    public bool moving = false;
    public bool finish = false;
    public GameObject folder;
    public Material openFolder;
    public Material closeFolder;
    Vector3 origPos;
    Vector3 origScale;
    Vector3 prevPos;
    private int fcounter = 0;

    // Use this for initialization
    void Start()
    {
        origPos = new Vector3(-0.04f, 0.0015f, 0.06f);
        origScale = folder.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (screenTop.num_touches == 2) {
            moving = true;
            folder.GetComponent<Renderer>().material = openFolder;
            Debug.Log("pinching!");
        }
        if (moving)
        {
            if (folder.transform.localScale.x < origScale.x + 0.01)
            {
                folder.transform.localScale += new Vector3(0.001f, 0.001f, 0.0f);
            }
            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, (float)(hand.indexPos.z + 0.015));
            if (!hand.isPinched) { fcounter += 1; }
            else { fcounter = 0; }
            if (fcounter > 30)
            {
                Debug.Log("FINISH!");
                fcounter = 0;
                moving = false;
                finish = true;
            }
        }
        if (finish)
        {
            this.transform.position = this.transform.position + new Vector3(0.0f, 0.0f, 0.003f);
            if (folder.transform.localScale.x > origScale.x)
            {
                folder.transform.localScale -= new Vector3(0.0005f, 0.0005f, 0.0f);
            }
        }
        if (this.transform.position.z > 0.06f) {
            finish = false;
            this.transform.position = origPos;
            folder.GetComponent<Renderer>().material = closeFolder;
            folder.transform.localScale = origScale;
        }

    }
}
