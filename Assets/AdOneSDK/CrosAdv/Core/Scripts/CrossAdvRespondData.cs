using System.Collections.Generic;
using UnityEngine;

namespace AdOneSDK.CrossAdv
{
    [System.Serializable]
    public class CrossAdvRespondData
    {
        public int id;
        public string app_key;
        public string platform;
        public string title;
        public string description;
        public string button_text;
        public List<string> creative_video;
        public List<string> creative_image;

        public override string ToString()
        {
            return JsonUtility.ToJson(this);
        }
    }
}
