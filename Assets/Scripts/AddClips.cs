using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.Video;

public class AddClips : MonoBehaviour {

    public static  List<VideoClip> clips = new List<VideoClip>();
    private string clipsPath;
    private List<string> names = new List<string>();

    void Start()
    {
       // UsWorkflowMaincontrol flow = new UsWorkflowMaincontrol();
        //clipsPath = flow.outputPath;
        //Thread getClipsThread = new Thread(getClips);
        //getClipsThread.Start();

    }

    public void getClips()
    {
        string path = /*Application.dataPath + "/Resources/Car/"*/clipsPath;
        if (Directory.Exists(path))
        {
            //获取文件信息
            DirectoryInfo direction = new DirectoryInfo(path);

            FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);

            print(files.Length);
            while (isActiveAndEnabled)
            {
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
                    clips.Add((VideoClip)Resources.Load(clipsPath + name));
                    names.Add(name);
                }
                Thread.Sleep(5000);
            }
        }
    }
}
