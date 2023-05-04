using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class eject_USB : MonoBehaviour
{
    public ScreenTouch_Stand screenTop;
    public HandData hand;
    public GameObject usbScreen;
    public GameObject whiteBox;
    public bool moving = false;
    public bool finish = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (screenTop.num_touches == 2)
        {
            moving = true;
            usbScreen.SetActive(true);
        }
        if (moving)
        {
            this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, Quaternion.Euler(90, 0, 0), 5.0f);
            this.transform.position = new Vector3(0.0f, 0.084f, hand.indexPos.z);
            if (hand.isPinched == false)
            {
                moving = false;
                finish = true;
            }
        }
        if (finish)
        {
            usbScreen.SetActive(false);
            if (this.transform.position.z < -0.09)
            {
                whiteBox.SetActive(true);
            }
            else
            {
                this.transform.position = this.transform.position + new Vector3(0.0f, 0.005f, 0.0f);
            }
            if (this.transform.position.z > 0.0319) { finish = false; }
        }

    }
}
