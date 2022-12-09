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
        public string click_url;
        public string imp_url;
        public List<string> creative_video;
        public List<string> creative_image;

        [System.NonSerialized]
        public List<string> PathImageLocal = new List<string>();
        [System.NonSerialized]
        public List<string> PathVideoLocal = new List<string>();
        [System.NonSerialized]
        public List<string> UrlVideoValid = new List<string>();
        [System.NonSerialized]
        public List<Sprite> Sprites = new List<Sprite>();
        [System.NonSerialized]
        public List<Texture> Textures = new List<Texture>();

        public override string ToString()
        {
            return JsonUtility.ToJson(this);
        }        
    }
}
