using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace HotFix
{
    class LogBehaviour : MonoBehaviour
    {
        Text _text;
        void Awake()
        {
            _text = gameObject.GetComponent<Text>();
            _text.horizontalOverflow = HorizontalWrapMode.Overflow;
            _text.verticalOverflow = VerticalWrapMode.Overflow;
            Application.logMessageReceived += HandleLog;
        }

        void HandleLog(string logString, string stackTrace, LogType type)
        {
            switch (type)
            {
                case LogType.Error:
                    _text.color = Color.red;
                    break;
                default:
                    _text.color = Color.green;
                    break;
            }
            _text.text += $"\n[{type}]\n" + logString;
        }
    }
}
