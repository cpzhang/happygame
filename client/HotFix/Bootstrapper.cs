using System;
using System.Collections.Generic;

namespace HotFix
{
    public class Bootstrapper
    {
        static List<IMessageSink> sinks_ = new List<IMessageSink>();
        static TcpComponent tcp_ = new TcpComponent();
        // static method
        public static void go()
        {
            //UnityEngine.GameObject.FindGameObjectWithTag("Canvas").AddComponent<CanvasBehaviour>();
            UnityEngine.Debug.Log("hotfix start ...");
            sinks_.Add(new LoginComponent());
            //
            //tcp_.ConnectTaskAsync();
        }
    }
}
