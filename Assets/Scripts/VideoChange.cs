using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoChange : MonoBehaviour {

	// Use this for initialization
	void Start () {
        print("video change start");
        int a = Random.Range(0, UsWorkflowMaincontrol.clips.Count);
        GetComponentInChildren<VideoPlayer>().clip = UsWorkflowMaincontrol.clips[a];
    }
   void OnEnabled()
    {
        print("video change enabled");
       
    }
    // Update is called once per frame
    void Update () {
		
	}
}
