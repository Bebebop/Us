using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class CollisionBehavior : MonoBehaviour {
    public GameObject layer;
    private int clipsRange
    {
        set { }
        get
        {
            int range = UsWorkflowMaincontrol.clips.Count + 1;
            return range;
        }
    }
	// Use this for initialization
	void Start () {
        if (UsWorkflowMaincontrol.clipsLoaded)
        {
            int a = Random.Range(1, clipsRange);
            print("随机数为："+a);
            GetComponentInChildren<VideoPlayer>().url = Application.dataPath+"/Resources/Clips/"+a+".webm";
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerExit(Collider collider)
    {
        if (GetComponent<Transform>().position.z < 1.5f) Instantiate(layer, new Vector3(0, 0, 7), new Quaternion(0,0,0,0));
        if (GetComponent<Transform>().position.z >6f) Instantiate(layer, new Vector3(0, 0, 1.56f), new Quaternion(0, 0, 0, 0));
        Destroy(this.gameObject);
    }
}
