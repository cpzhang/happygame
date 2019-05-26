using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HotFix
{
    interface IMessageSink
    {
        void OnMessage();
    }
    class LoginComponent: IMessageSink
    {
        public void AWake()
        {
            var b = GameObject.Find("NEGUI/UILogin/LoginBtn");
            Debug.Log(b.name);
            UIEventListener.Get(b).onClick = delegate (GameObject o)
            {
                UnityEngine.Debug.Log("clicked");
            };
        }
        void IMessageSink.OnMessage()
        {
            
        }
    }
}
