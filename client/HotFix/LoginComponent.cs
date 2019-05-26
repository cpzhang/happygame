using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotFix
{
    interface IMessageSink
    {
        void OnMessage();
    }
    class LoginComponent: IMessageSink
    {
        void IMessageSink.OnMessage()
        {
            
        }
    }
}
