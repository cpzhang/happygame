using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Bootstrapper : MonoBehaviour
{
    ILRuntime.Runtime.Enviorment.AppDomain appdomain;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("bootstrapper start");
        _ = StartCoroutine(LoadILRuntime());
    }

    IEnumerator LoadILRuntime()
    {
        appdomain = new ILRuntime.Runtime.Enviorment.AppDomain();
#if UNITY_ANDROID
        WWW www = new WWW(Application.streamingAssetsPath + "/HotFix.dll");
#else
    WWW www = new WWW("file:///" + Application.streamingAssetsPath + "/HotFix.dll");
#endif
        while (!www.isDone)
            yield return null;
        if (!string.IsNullOrEmpty(www.error))
            Debug.LogError(www.error);
        byte[] dll = www.bytes;
        www.Dispose();
#if UNITY_ANDROID
        www = new WWW(Application.streamingAssetsPath + "/HotFix.pdb");
#else
    www = new WWW("file:///" + Application.streamingAssetsPath + "/HotFix.pdb");
#endif
        while (!www.isDone)
            yield return null;
        if (!string.IsNullOrEmpty(www.error))
            Debug.LogError(www.error);
        byte[] pdb = www.bytes;
        using (System.IO.MemoryStream fs = new MemoryStream(dll))
        {
            using (System.IO.MemoryStream p = new MemoryStream(pdb))
            {
                appdomain.LoadAssembly(fs, p, new ILRuntime.Mono.Cecil.Pdb.PdbReaderProvider());
                appdomain.Invoke("HotFix.Bootstrapper", "go", null, null);
            }
        }
    }

}
