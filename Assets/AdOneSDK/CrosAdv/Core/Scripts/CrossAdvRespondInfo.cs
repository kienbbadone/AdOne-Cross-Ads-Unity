using UnityEngine;

namespace AdOneSDK.CrossAdv
{
    [System.Serializable]
    public class CrossAdvRespondInfo
    {
        public string bundle;
        public string platform;

        public override string ToString()
        {
            return JsonUtility.ToJson(this);
        }
    }
}
