using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace AdOneSDK.CrossAdv
{
    public class CrossAdvVideo : MonoBehaviour
    {
        public VideoPlayer player;
        public VideoSource source;

        private void Start()
        {
            CrossAdv.ShowVideo(this);
        }
    }
}
