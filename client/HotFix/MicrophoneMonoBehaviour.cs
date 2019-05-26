using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace HotFix
{
    class MicrophoneBehaviour : MonoBehaviour
    {
        void Awake()
        {
            UnityEngine.Debug.Log("microphone awake ...");
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            GameObject.Find("Record").GetComponent<Button>().onClick.AddListener(()=>
            {
                UnityEngine.Debug.Log("record");
                audioSource.clip = Microphone.Start(null, false, 10, 44100);
            });
            GameObject.Find("Play").GetComponent<Button>().onClick.AddListener(() =>
            {
                UnityEngine.Debug.Log("play");
                audioSource.Play();
            });
        }

        void Start()
        {
        }

        void Update()
        {
        }
    }
}
