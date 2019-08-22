using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CachableObject : MonoBehaviour
{
    public bool isLoaded {get; set;} = false;

    public void Reset()
    {
        gameObject.transform.position = new Vector3(1000, 0, -100);
        isLoaded = false;
    }
}
