using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System.Linq;
using UnityEngine.SceneManagement;

public class LevelBehavior : MonoBehaviour
{
    #region PUBLIC_VARS

    // Превьюшка в главном меню(панель выбора левела)
    public Sprite preview;

    // Задник(дабы левела отличались более визуально, можно грузить разные задники)
    public Sprite background;
    public Sprite background2;

    public float SpawnTimeOut = 1;

    public int needToGo = 100;
    private IReactiveProperty<double> way = new ReactiveProperty<double>(0);

    public Image image { get; private set; }
    public Image image2 { get; private set; }

    [SerializeField]
    public ObjectCache cache;

    #endregion

    #region PRIVATE_VARS

    private bool isEnabled = false;

    // Общий пулл всех закэшированных объектов.
    private ReactiveCollection<GameObject> cachedObjects { get; set; } = new ReactiveCollection<GameObject>();

    private LevelModel level_m;
    private ShipModel ship_m;

    #endregion

    #region BASE_INTERFACE

    /// <summary>
    /// Запуск "прокрутки" уровня и спауна объектов
    /// </summary>
    /// <param name="model_l">Модель уровня</param>
    /// <param name="model_s">Модель корабля</param>
    public void InitBehavior(LevelModel model_l, ShipModel model_s)
    {
        var cacheObj = Instantiate(cache.gameObject);
        cache = cacheObj.GetComponent<ObjectCache>();

        level_m = model_l;
        ship_m = model_s;

        image.sprite = background;
        image2.sprite = background2;

        isEnabled = true;

        Observable.Timer(System.TimeSpan.FromSeconds(SpawnTimeOut)).Repeat().Subscribe(_ => Spawn());
    }

    /// <summary>
    /// Функция, возвращающая подписанные "цели" для отображения.
    /// Каждый дочерний класс может имплементировать по своему усмотрению(в зависимости от поставленных целей, конечно же).
    /// Базовая цель определена как "Пролететь ... ед. пути"
    /// </summary>
    /// <returns>Список ГеймОбъектов-целей</returns>
    public virtual List<GameObject> GetApplyingableObjectives(GameObject objectivePrefab)
    {
        List<GameObject> objects = new List<GameObject>();

        var item = Instantiate(objectivePrefab) as GameObject;
        way.AsObservable().Subscribe(x => item.GetComponentInChildren<Text>().text = "Пройдено " + x + " из " + needToGo).AddTo(gameObject);
        level_m.isPassed = way.AsObservable().Select(x => x >= needToGo).ToReactiveProperty();
        level_m.isPassed.Where(x => x == true).Subscribe(_ =>
        {
            isEnabled = false;
            SceneManager.LoadScene("MainScene");
        });

        objects.Add(item);

        return objects;
    }

    public void SetImages(Image img1, Image img2)
    {
        image = img1;
        image2 = img2;
    }

    #endregion

    protected virtual void Spawn()
    {
        var obj = cache.GetObjectByTag("asteroid");
        if(obj != null) obj.transform.position = new Vector3(135, 630, -55);
    }

    void Update()
    {
        if (isEnabled)
        {
            way.Value += 0.1d;
            image.transform.Translate(0, -1, 0);
            image2.transform.Translate(0, -1, 0);

            if (image.transform.position.y <= -900)
            {
                image.transform.position = new Vector3(image.transform.position.x, 1500, image.transform.position.z);
            }

            if (image2.transform.position.y <= -900)
            {
                image2.transform.position = new Vector3(image2.transform.position.x, 1500, image2.transform.position.z);
            }
        }
    }

}
