using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;
using System;

[Serializable]
public class PoolInitilizer
{
    [SerializeField]
    public CachableObject c_obj;

    [SerializeField]
    public int count;
}

[Serializable]
public class ObjectCache : MonoBehaviour
{
    [SerializeField]
    public List<PoolInitilizer> initializers = new List<PoolInitilizer>();
    public ReactiveCollection<CachableObject> objects = new ReactiveCollection<CachableObject>();

    void Start()
    {
        objects.DefaultIfEmpty(null);

        //objects.ObserveAdd().Subscribe(obj => );
        objects.ObserveRemove().Subscribe(obj =>
            Destroy(obj.Value.gameObject)
        );

        foreach(var initilizer in initializers)
        {
            for(int i = 0; i < initilizer.count; ++i)
            {
                var obj = Instantiate(initilizer.c_obj.gameObject);
                obj.SetActive(false);

                initilizer.c_obj.Reset();
                objects.Add(obj.GetComponent<CachableObject>());
            }
        }
    }

    public CachableObject GetObjectByTag(string tag)
    {
        var obj = objects.FirstOrDefault(x => x.tag == tag && x.gameObject.activeSelf == false);
        if(obj != null)
        {
            obj.gameObject.SetActive(true);    
        }
        return obj;
    }
}
