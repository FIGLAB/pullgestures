using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenTouch_flat : MonoBehaviour {
    public int num_touches = 0;
    public Collider[] colliders = new Collider[10];
    public Vector3[] positions = new Vector3[10];
    private Vector3 temp_position;
    private Touch touch;
    private GameObject temp_block;

    private double midwidth;
    private double midheight;
    private double pixwidth;
    private double pixheight;

    private int cidx;
    private float distance;
    private float bestDistance;
    private Collider[] tempcolliders;
    private Collider bestCollider = null;
    private int pidx;

    // Use this for initialization
    void Start()
    {
        temp_block = GameObject.CreatePrimitive(PrimitiveType.Cube);
        temp_block.transform.localScale = new Vector3(0.004f, 0.004f, 0.004f);
        temp_block.GetComponent<Renderer>().material.color = Color.red;

        midwidth = 0.22272 / 2;
        pixwidth = 0.22272 / Screen.width;
        midheight = 0.0845 - 0.12528 / 2;
        pixheight = 0.12528 / Screen.height;

    }

    // Update is called once per frame
    void Update()
    {
        // Handle screen touches.
        num_touches = Input.touchCount;
        positions = new Vector3[10];
        if (num_touches > 0)
        {
            // TODO: implement bipartite weighted maximum matching for two lists
            tempcolliders = colliders;
            for (int i = 0; i < num_touches; i++)
            {
                touch = Input.GetTouch(i);
                cidx = 0;
                bestDistance = 99999.0f;
                temp_position = new Vector3((float)(-midwidth + pixwidth * touch.position.x), 0.006f, (float)(midheight + pixheight * touch.position.y));

                foreach (Collider c in tempcolliders)
                {
                    distance = Vector3.Distance(temp_position, c.transform.position);
                    if (distance < bestDistance && distance < 0.1f)
                    {
                        bestDistance = distance;
                        bestCollider = c;
                        pidx = cidx;
                    }
                    cidx++;
                }
                if (bestDistance < 9999.0f)
                {
                    tempcolliders[pidx].transform.position = new Vector3(100.0f, 100.0f, 100.0f);
                    positions[pidx] = temp_position;
                    //Debug.Log(bestCollider);
                }
            }

            touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved) {
                temp_block.transform.localPosition = new Vector3((float)(-midwidth + pixwidth * touch.position.x), 0.012f, (float)(midheight + pixheight * touch.position.y));
            } 
        }
    }
}

