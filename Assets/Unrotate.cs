using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unrotate : MonoBehaviour
{
    GameObject mParent;
    // Start is called before the first frame update
    void Start()
    {
        mParent = transform.parent.gameObject;
    }

    // LateUpdate is called after all the transforms just before render
    void LateUpdate()
    {
        transform.rotation = Quaternion.identity; //Undo rotation
    }
}
