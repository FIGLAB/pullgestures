using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class font_grow : MonoBehaviour {
    private bool shrinking = false;
    private bool growing = false;
    public GameObject indicator;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("s")) { growing = true; }
        if (growing)
        {
            indicator.transform.localPosition = indicator.transform.localPosition - new Vector3(0.0f, 0.0f, 0.0053f);
            this.GetComponent<TextMesh>().fontSize += 1;
            if (this.GetComponent<TextMesh>().fontSize == 24) { growing = false; }
        }
        if (Input.GetKeyDown("d")) { growing = false; shrinking = true; }
        if (shrinking)
        {
            indicator.transform.localPosition = indicator.transform.localPosition + new Vector3(0.0f, 0.0f, 0.0053f);
            this.GetComponent<TextMesh>().fontSize -= 1;
            if (this.GetComponent<TextMesh>().fontSize == 12){ shrinking = false; }
        }
    }
}

