using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdOneDemoCameraFollow : MonoBehaviour
{
    public Transform tr_Target;
    public float dampen = 10f;
    // Update is called once per frame
    void Update()
    {
        this.transform.position = Vector3.Lerp(transform.position, tr_Target.position, dampen * Time.deltaTime);
    }
}
