﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.Translate(0, -1.5f, 0);
        if(gameObject.transform.localPosition.y <= -65)
        {
            gameObject.GetComponent<CachableObject>().Reset();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        var shipCont = collision.gameObject.GetComponent<ShipController>();
        if(shipCont != null)
        {
            shipCont.GetModel().Damage();
        }

        gameObject.GetComponent<CachableObject>().Reset();
    }
}
