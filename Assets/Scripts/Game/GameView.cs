using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class GameView : MonoBehaviour
{
    public GameObject listObject;
    public GameObject objectivePrefab;


    public Text lives;
    private ReactiveCollection<GameObject> objectives = new ReactiveCollection<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        lives = gameObject.GetComponentInChildren<Text>();

        objectives.ObserveAdd().Subscribe(x =>
        {
            x.Value.transform.SetParent(listObject.transform, false);
        });
    }

    public void BindLevelBehavior(LevelBehavior behavior)
    {
        foreach (var level_objective in behavior.GetApplyingableObjectives(objectivePrefab))
        {
            objectives.Add(level_objective);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
