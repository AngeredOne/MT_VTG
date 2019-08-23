using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CachableObject : MonoBehaviour
{
    [HideInInspector]
    public void Reset()
    {
        gameObject.transform.position = new Vector3(1000, 0, -100);
        gameObject.SetActive(false);
    }
}
