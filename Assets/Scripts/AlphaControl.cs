using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlphaControl : MonoBehaviour {
    private float fatherPosition;
    private float progress;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        fatherPosition = GetComponentInParent<Transform>().position.z;
        if (fatherPosition > 3)
        {
            progress = (GetComponent<Transform>().position.z - 3)/4.45f ;
            GetComponent<Renderer>().material.SetFloat("_AlphaScale", progress);
        }
        else if(fatherPosition<3)
        {
            progress = (3 - GetComponent<Transform>().position.z)/1.4f;
            GetComponent<Renderer>().material.SetFloat("_AlphaScale", progress);
        }
        
    }
}
