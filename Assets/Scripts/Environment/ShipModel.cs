/// <summary>
/// Базовый интерфейс поведения корабля.
/// Заставляет имплементировать базовые поля и метод получения дамага.
/// В теории, можно создать спецификацию этого интерфейса, описав тяжелый корабль.
/// Вьюшка работает только с базовым представлением корабля(пока).
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UniRx;

public class ShipModel
{
    public IReactiveProperty<int> hp { get; set; }

    public IReactiveProperty<double> way { get; set; }
    public IReadOnlyReactiveProperty<bool> isDead { get; set; }

    public void Damage()
    {
        hp.Value--;
    }
    public void Go(float add)
    {
        way.Value += add;
    }

    public ShipModel(int startHp)
    {
        hp = new ReactiveProperty<int>(startHp);
        way = new ReactiveProperty<double>(0.0);
        isDead = hp.Select(x => x < 1).ToReactiveProperty();
    }
}