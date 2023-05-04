using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class image_zoom : MonoBehaviour {
    private bool shrinking = false;
    private bool growing = false;
    public GameObject properties;
    private Vector3 origSize;

    // Use this for initialization
    void Start()
    {
        origSize = new Vector3(0.05f, 0.035f, 0.003f);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("s")) { growing = true; }
        if (growing)
        {
            this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y, 0.0451f);
            properties.SetActive(true);
            this.transform.localScale = Vector3.Scale(this.transform.localScale, new Vector3(1.02f, 1.02f, 1.02f));
            if (this.transform.localScale.x > (origSize.x*2.0f)) { growing = false; }
        }
        if (Input.GetKeyDown("d")) { growing = false; shrinking = true; }
        if (shrinking)
        {
            this.transform.localScale = Vector3.Scale(this.transform.localScale, new Vector3(0.85f, 0.85f, 1.0f));
            if (this.transform.localScale.x < (origSize.x*0.3f)) {
                shrinking = false;
                this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y, 0.08f);
                this.transform.localScale = origSize;
                properties.SetActive(false);
            }
        }
    }
}
