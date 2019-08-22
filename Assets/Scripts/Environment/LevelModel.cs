using System.Collections;
using System.Collections.Generic;
using UniRx;
using System;

[Serializable]
public class LevelModel
{
    /// <summary>
    /// Указывает доступность уровня при выборе(является сохраняемым объектом)
    /// </summary>
    /// <value></value>
    public LevelState state { get; set; } = LevelState.Available;

    /// <summary>
    /// Реактивное свойство, указывающее, пройден ли уровень(при запущенном уровне)
    /// Каждый инстанс поведения уровня может имплементировать данное свойство, иначе будет вечный левел(офк можно и так делать, спец. уровень - испытание)
    /// </summary>
    /// <value></value>
    public IReadOnlyReactiveProperty<bool> isPassed { get; set; }

    public LevelModel()
    {
        isPassed = new ReactiveProperty<bool>(false);
    }
}
