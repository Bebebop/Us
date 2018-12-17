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
using System;
using System.IO;
using System.Collections;

public class SimpleBackgroundRemoval : MonoBehaviour 
{
    public float frameWait;
    private float frameNow;
    private float frameInterval;
    public string filepathInput;
    public string filenameInput;
    public GameObject text1;
    public GameObject text2;
    [Tooltip("Whether to display the foreground texture on the screen or not.")]
	public bool displayForeground = true;

	[Tooltip("Please follow these instructions to make this component work.")]
	[Multiline]
	public string instructions = "Set 'Compute user map'-setting of KinectManager-component to 'Cut-Out Texture'.";


	// the foreground texture
	private Texture2D foregroundTex;
	
	// rectangle taken by the foreground texture (in pixels)
	private Rect foregroundRect;

	// the Kinect manager
	private KinectManager manager;
    private int i;
    void SaveTextureToFile(Texture2D texture, string fileName, string filePath)
    {
        i++;
        if (Directory.Exists(filePath ) == false)
            Directory.CreateDirectory(filePath);
        //Debug.Log(filePath + fileName + i + ".png");
            File.WriteAllBytes(filePath + fileName+i+".png", texture.EncodeToPNG());
    }
    void Start () 
	{
		manager = KinectManager.Instance;
        frameNow = 0;
        frameInterval = 0;
        

		if(manager && manager.IsInitialized())
		{
			Rect cameraRect = Camera.main.pixelRect;
			float rectHeight = cameraRect.height;
			float rectWidth = cameraRect.width;

			KinectInterop.SensorData sensorData = manager.GetSensorData();

			if(sensorData != null && sensorData.sensorInterface != null)
			{
				if(rectWidth > rectHeight)
					rectWidth = rectHeight * sensorData.depthImageWidth / sensorData.depthImageHeight;
				else
					rectHeight = rectWidth * sensorData.depthImageHeight / sensorData.depthImageWidth;
				
				foregroundRect = new Rect((cameraRect.width - rectWidth) / 2, cameraRect.height - (cameraRect.height - rectHeight) / 2, rectWidth, -rectHeight);
			}
		}
	}
	
	void Update () 
	{
        frameInterval = Time.time - frameNow;
        text1.GetComponent<Text>().text=frameInterval.ToString();
        text2.GetComponent<Text>().text = frameNow.ToString();
        if (manager && manager.IsInitialized())
		{
			foregroundTex = manager.GetUsersLblTex();
            if (manager.GetUsersCount() > 0&& frameInterval > frameWait)
            {
                frameNow = Time.time;
                SaveTextureToFile(foregroundTex, filenameInput,filepathInput);
                
            }
            }
	}

	void OnGUI()
	{
		if(displayForeground && foregroundTex)
		{
			// get the foreground rectangle (use the portrait background, if available)
			PortraitBackground portraitBack = PortraitBackground.Instance;
			if(portraitBack && portraitBack.enabled)
			{
				foregroundRect = portraitBack.GetBackgroundRect();

				foregroundRect.y += foregroundRect.height;  // invert y
				foregroundRect.height = -foregroundRect.height;
			}
			
			GUI.DrawTexture(foregroundRect, foregroundTex);
		}
	}
	
}
