/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class KinectNIControl : MonoBehaviour
{
    public float Speed ;
    public GameObject textout;
    public float threshold;

    // reference to the gesture listener
    private ModelGestureListener gestureListener;


    void Start()
    {
        // hide mouse cursor
        Cursor.visible = false;

        // get the gestures listener
        gestureListener = ModelGestureListener.Instance;
    }

    void Update()
    {
        // dont run Update() if there is no gesture listener
        if (!gestureListener)
            return;
        /*if (gestureListener.IsTurningWheel())
        {
            // rotate the model
            float turnAngle = gestureListener.GetWheelAngle();

            Vector3 newRotation = transform.rotation.eulerAngles;
            newRotation.y += turnAngle;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(newRotation), spinSpeed * Time.deltaTime);
        }*/
        if (gestureListener.IsBowing())
        {
           // Debug.Log("bowing ready!!!!");
            // rotate the model
            float bowSpeed = gestureListener.GetBowFactor();
            //Debug.Log(string.Format("bow speed={0},threshold={1}",bowSpeed,threshold));
            // textout.GetComponent<Text>().text = bowSpeed.ToString();
            if (Mathf.Abs(bowSpeed) >= threshold)
            {
                
                GetComponent<Transform>().position += new Vector3(0, 0, Mathf.Pow(bowSpeed / 20, 2) * Time.deltaTime * Speed * Mathf.Sign(bowSpeed));
            }
        }
        


    }

}
