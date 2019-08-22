using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class ShipController : MonoBehaviour
{
    public int startHp = 3;

    private ShipModel model { get; set; }
    public GameView view;

    // Ленивая инициализация, потому что на порядок Start() полагаться нельзя.
    public ShipModel GetModel()
    {
        if (model == null)
        {
            model = new ShipModel(startHp);
            model.hp.AsObservable().Subscribe(x => view.lives.text = "Осталось до смерти: " + x).AddTo(this);
        }
        return model;
    }

}
