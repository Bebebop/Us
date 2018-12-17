
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Collections;
using System.Timers;
using System.Collections.Generic;
public class UsWorkflowMaincontrol : MonoBehaviour
{
    public int minLength;
    public double timeWait;
    private float timeTracked;
    private float timeInterval;
    private float timeCache;
    public string filenameInput;
    public GameObject text1;
    public GameObject text2;
    public GameObject text3;
    public GameObject text4;
    public GameObject quad;
    public static List<VideoClip> clips = new List<VideoClip>();
    private List<string> names = new List<string>();
    public bool displayForeground = true;
    private Texture2D foregroundTex;
    private Rect foregroundRect;
    private KinectManager manager;
    private int i;
    private int fileRenamer;
    private int folderCount;
    private string filepathInput;
    private string pathCache;
    private bool greenToCapture;
    public static bool clipsLoaded=false;
    private static System.Timers.Timer captureTimer = new System.Timers.Timer();
    private string rootPath;
    private bool addReady=false;
    private string appPath;
    
    private string filepathCache {
        set { }
    get {
            pathCache = filepathInput + folderCount+"/";
            return pathCache; }
    }
    public string outputPath
    {
        set { }
        get
        {
            string outputCache = filepathInput + "output/";
                return outputCache;
        }
    }
    private bool trackStatus
    {
        get
            { if (manager.GetUsersCount() > 0)
                return true;
            else
                return false; }
    }
    private bool mountStatus
    {
        get
        {
            return SteamVR_Controller.Input(0).GetPress(Valve.VR.EVRButtonId.k_EButton_ProximitySensor);
        }
    }
    private bool trackBuffer;
    private bool mountBuffer;

     void SaveTextureToFile(Texture2D texture, string fileName, string filePath)
    {   
        i++;
        string num = i.ToString().PadLeft(4, '0');
        //UnityEngine.Debug.Log(filePath + fileName + i + ".png");
        if(Directory.Exists(filePath))File.WriteAllBytes(filePath + fileName + num + ".png", texture.EncodeToPNG());
    }
    void CleanAndSave(object o)
    {//boxblur=2:1:ar=5 ,negate
        int folderCountCache = folderCount;
        Process p = new Process();
        p.StartInfo.FileName = appPath+"/ffmpeg/bin/ffmpeg.exe";
        p.StartInfo.Arguments = " -i " + filepathCache + filenameInput + "%04d.png -auto-alt-ref 0 -c:v libvpx -vf rotate=PI,boxblur=1:1:ar=5 " + filepathInput + "output/"+ folderCountCache+".webm";
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.RedirectStandardError = true;
        p.StartInfo.CreateNoWindow = true;
        p.ErrorDataReceived += new DataReceivedEventHandler(Output);
        p.Start();
        folderCount++;
        Directory.CreateDirectory(filepathCache);
        p.WaitForExit();
        p.Close();
        p.Dispose();
        print("准备剪切");
        string fileFrom = filepathInput + "output/" + folderCountCache + ".webm";
        string fileTo = rootPath + "/Resources/Clips/" + fileRenamer + ".webm";
        File.Move(fileFrom,fileTo);
        fileRenamer++;
        addReady = true;

    }
    public void GetClips(object o)
    {
        string path = Application.dataPath + "/Resources/Clips/";
        if (Directory.Exists(path))
        {
            //获取文件信息
            DirectoryInfo direction = new DirectoryInfo(path);
            FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);
                for (int i = 0; i < files.Length; i++)
                {
                    if (files[i].Name.EndsWith(".meta"))
                    {
                        continue;
                    }
                    print(files[i].Extension);
                    string name = Path.GetFileNameWithoutExtension(files[i].ToString());
                    if (names.Exists(x => x == name)) continue;
                    print(name);
                    clips.Add((VideoClip)Resources.Load(path + name));
                    names.Add(name);
                }
            print(string.Format("Length={0}",clips.Count));
        }
        clipsLoaded = true;
    }
    void TimerStart()
    {
        captureTimer.Enabled= true;
        captureTimer.Elapsed += new ElapsedEventHandler(Time_Elapsed);
        captureTimer.Interval = timeWait;
        captureTimer.AutoReset = true;
        
    }
    void Time_Elapsed(object o, ElapsedEventArgs e)
    {
        if (!greenToCapture) greenToCapture = true;

    }
    public static void DelectDir(string srcPath)
    {
        try
        {
            DirectoryInfo dir = new DirectoryInfo(srcPath);
            FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();
            foreach (FileSystemInfo i in fileinfo)
            {
                if (i is DirectoryInfo)
                {
                    DirectoryInfo subdir = new DirectoryInfo(i.FullName);
                    subdir.Delete(true);
                }
                else
                {
                    File.Delete(i.FullName);
                }
            }
        }
        catch (Exception e)
        {
            throw;
        }
    }
    private void Output(object sendProcess, DataReceivedEventArgs output)
    {
        if (!String.IsNullOrEmpty(output.Data))
        {
            UnityEngine.Debug.Log(output);
        } }

   void Start()
    {
        manager = KinectManager.Instance;
        rootPath = Application.dataPath;
        filepathInput = Application.dataPath + "/temp/";
        print(rootPath);
        GetClips(null);
        appPath = Application.dataPath;
        for (int i =1;true; i++)
        {
            if (!Directory.Exists(string.Format("{0}{1}/",filepathInput,i)))
            {
                folderCount=i;
                print(folderCount);
                break;
            }
        }
        for(int i = 1; true; i++)
        {
            if (!File.Exists(string.Format("{0}/Resources/Clips/{1}.webm", rootPath, i)))
            {
                fileRenamer = i;
                break;
            }
        }
        //Instantiate(quad, new Vector3(0, 0, 3), new Quaternion(0, 0, 0, 0));
        timeCache = 0;
        timeTracked = 0;
        timeInterval = 0;
        
        trackBuffer = trackStatus;
        mountBuffer = mountStatus;
        greenToCapture = false;
        TimerStart();
        captureTimer.Stop();
        if (Directory.Exists(filepathCache) == false)
            Directory.CreateDirectory(filepathCache);
        if (Directory.Exists(outputPath) == false)
            Directory.CreateDirectory(outputPath);
        if (manager && manager.IsInitialized())
        {
            Rect cameraRect = Camera.main.pixelRect;
            float rectHeight = cameraRect.height;
            float rectWidth = cameraRect.width;

            KinectInterop.SensorData sensorData = manager.GetSensorData();

            if (sensorData != null && sensorData.sensorInterface != null)
            {
                if (rectWidth > rectHeight)
                    rectWidth = rectHeight * sensorData.depthImageWidth / sensorData.depthImageHeight;
                else
                    rectHeight = rectWidth * sensorData.depthImageHeight / sensorData.depthImageWidth;

                foregroundRect = new Rect((cameraRect.width - rectWidth) / 2, cameraRect.height - (cameraRect.height - rectHeight) / 2, rectWidth, -rectHeight);
            }
        }
    }


    void Update()
    {
        print("green to cp:"+greenToCapture);
        timeInterval = Time.time - timeTracked;
        timeTracked = Time.time - timeCache;
        //text1.GetComponent<Text>().text = timeInterval.ToString();
        //text2.GetComponent<Text>().text = timeTracked.ToString();
        //text3.GetComponent<Text>().text = "Trackstatus="+trackStatus.ToString();
        //text4.GetComponent<Text>().text = "Trackbuffer=" + trackBuffer.ToString();
        if (greenToCapture)
        {
            SaveTextureToFile(foregroundTex, filenameInput, filepathCache);
            greenToCapture = false;
        }
        if (addReady)
        {
            addReady = false;
            GetClips(null);
        }
            if (manager && manager.IsInitialized())
        {
            foregroundTex = manager.GetUsersLblTex();
            // if (manager.GetUsersCount() > 0 && frameInterval > frameWait)
            //{   //timer.

            // SaveTextureToFile(foregroundTex, filenameInput, filepathCache);

            // }
        }
        if (trackStatus)
        {
            if (mountBuffer != mountStatus)
            {
                mountBuffer = mountStatus;
                if (mountStatus)
                {
                    i = 0;
                    captureTimer.Start();
                    UnityEngine.Debug.Log("录制开始");
                    timeCache = Time.time;
                }
                else
                {
                    captureTimer.Stop();
                    UnityEngine.Debug.Log("录制结束");
                    addReady = true;
                    if (timeTracked > minLength)
                    {
                        UnityEngine.Debug.Log("1");
                        ThreadPool.QueueUserWorkItem(new WaitCallback(CleanAndSave));
                        print("保存录制");
                        UnityEngine.Debug.Log("2");
                    }
                    else
                    {
                        DelectDir(filepathCache);
                    }
                    timeTracked = 0;
                }
            }
        }
        trackBuffer = trackStatus;
       

    }
    void OnDestroy()
    {
            DelectDir(filepathCache);
    }
    void OnGUI()
    {
        if (displayForeground && foregroundTex)
        {
            // get the foreground rectangle (use the portrait background, if available)
            PortraitBackground portraitBack = PortraitBackground.Instance;
            if (portraitBack && portraitBack.enabled)
            {
                foregroundRect = portraitBack.GetBackgroundRect();

                foregroundRect.y += foregroundRect.height;  // invert y
                foregroundRect.height = -foregroundRect.height;
            }

            GUI.DrawTexture(foregroundRect, foregroundTex);
        }
    }

}
