using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AdOneSDK.CrossAdv
{
    [System.Serializable]
    public class CrossAdvRespondModel
    {
        public int status;
        public List<CrossAdvRespondData> data;
        public CrossAdvRespondInfo info;


        public override string ToString()
        {
            return JsonUtility.ToJson(this);
        }
    }
}
