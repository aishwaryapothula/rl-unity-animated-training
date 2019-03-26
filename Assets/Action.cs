using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action : MonoBehaviour
{

    public Animator ar;
    // Start is called before the first frame update
    void Start()
    { 
    ar = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp("1"))
        {
            ar.Play("stance");
        }

        if (Input.GetKeyUp("2"))
        {
            ar.Play("box");
        }

        if (Input.GetKeyUp("3"))
        {
            ar.Play("step");
        }

        if (Input.GetKeyUp("4"))
        {
            ar.Play("win");
        }
    }
}
