using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System.Linq;
using System;

public enum LevelState
{
    Closed = 1,
    Available = 2,
    Passed = 4
}

public class LevelController : MonoBehaviour
{
    private LevelModel model_l;

    public GameView view;

    /// <summary>
    /// Т.к. по заветам MVC контроллер не должен имплементировать поведение, а model должен отвечать только за описание модели
    /// Я ввожу объект "поведение". Через него можно будет реализовывать разные по поведению уровни, а view будет связан с отображением этого поведения
    /// К примеру, список целей и состояние их выполнения. Так же, поведение будет связано с моделью для управления состоянием модели.
    /// В итоге просто собираем нужный объект и радуемся жизни(квадратной жизни квадртаного корабля в квардартном космосе(нет, я же "займу" модели из тутора)).
    ///  Если не будет лень(или зудящего ощущения потерянного времени) - то сделаю по такому же принципу корабль, а эту строку сотру. Надеюсь.
    /// </summary>

    public LevelBehavior behavior;

    public LevelModel GetModel()
    {
        if (model_l == null)
        {
            model_l = new LevelModel();

            model_l.isPassed.Where(x => x == true).Subscribe(_ =>
            {
                model_l.state = LevelState.Passed;
                // Call save mb.
            });
        }

        return model_l;
    }

    public void SetModel(LevelModel model)
    {
        model_l = model;
    }

    public LevelBehavior GetBehavior()
    {
        if (behavior == null)
        {
            Debug.Log("Не установлено поведение уровня!");
        }

        return behavior;
    }

    public void StartLevel(ShipModel shipModel)
    {
        behavior = gameObject.GetComponent<LevelBehavior>();

        GetBehavior().InitBehavior(GetModel(), shipModel);
        view.BindLevelBehavior(GetBehavior());
    }
}
