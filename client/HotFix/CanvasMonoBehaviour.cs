using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace HotFix
{
    class CanvasBehaviour : MonoBehaviour
    {
        Button _button;
        void Awake()
        {
            UnityEngine.Debug.Log("microphone awake ...");
            GameObject.DontDestroyOnLoad(this);
            GameObject.Find("Log").AddComponent<LogBehaviour>();
            GameObject.Find("Audio").AddComponent<MicrophoneBehaviour>();
        }

        void Start()
        {
        }

        void Update()
        {
        }
    }
}
