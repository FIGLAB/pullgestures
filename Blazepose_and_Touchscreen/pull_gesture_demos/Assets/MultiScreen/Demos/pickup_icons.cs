using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickup_icons: MonoBehaviour
{
    public ScreenTouch_flat screenBot;
    public HandData hand;
    public GameObject pycons;
    public GameObject icon_block;
    public Material newmat;
    public Material origmat;
    public bool moving = false;
    public bool finish = false;
    Transform origT;
    Vector3 prevPos;

    // Use this for initialization
    void Start()
    {
        origT = this.transform;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(screenBot.num_touches);
        if (screenBot.num_touches == 2)
        {
            moving = true;
            icon_block.GetComponent<Renderer>().material = newmat;
            //this.transform.rotation = Quaternion.Euler(45, 0, 0);
        }
        if (moving)
        {
            this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, Quaternion.Euler(45, 0, 0), 5.0f);
            this.transform.localScale = Vector3.Lerp(this.transform.localScale, origT.localScale + new Vector3(0.01f, 0.01f, 0.01f), 0.05f);
            this.transform.position = hand.indexPos;
            if (hand.isPinched == false)
            {
                moving = false;
                finish = true;
            }
        }
        if (finish)
        {
            this.transform.position = this.transform.position - new Vector3(0.0f, 0.005f, 0.0f);
            if (this.transform.position.y < 0)
            {
                finish = false;
                //screenBot.GetComponent<Renderer>().material = newmat;
                this.transform.position = new Vector3(this.transform.position.x, 0.006f, this.transform.position.z);
                this.transform.rotation = Quaternion.Euler(17, 0, 0);
            }
        }
    }
}
