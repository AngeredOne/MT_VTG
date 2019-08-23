using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UniRx;

// TODO UI MainMenu; UI Game(+Relevant object position), Firing & Destroying Asteroids
// REFACTORING
// HATE FRONT END.

public class GameController : MonoBehaviour
{
    public MenuView mainMenuView;
    public ShipController ship;
    public List<LevelController> levels = new List<LevelController>();

    private IReactiveProperty<int> choisedLevelInd = new ReactiveProperty<int>(0);
    private LevelController loadedLevel = null;
    private CompositeDisposable disposables;

    public string saveName = "mysave";

    static GameController inst;

    void Awake()
    {
        if (inst != null)
        {
            Destroy(gameObject);
        }
        else
        {
            inst = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void Init()
    {
        var mainMenuViewInstance = Instantiate(mainMenuView.gameObject).GetComponent<MenuView>();
        disposables = new CompositeDisposable();

        mainMenuViewInstance.playButton.OnClickAsObservable().Subscribe(_ =>
        {
            SceneManager.sceneLoaded += OnLevelLoaded;
            SceneManager.LoadScene("GameScene");
            disposables.Dispose();
        }
        ).AddTo(disposables);

        choisedLevelInd.AsObservable().Subscribe(x =>
        {
            mainMenuViewInstance.preview.sprite = levels[x].GetBehavior().preview;
            mainMenuViewInstance.levelStateText.text = levels[x].GetModel().state.ToString();
        }).AddTo(disposables);

        Observable.EveryUpdate().Where(_ => (levels.Count > choisedLevelInd.Value + 1) && (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))).Subscribe(_ =>
        {
            choisedLevelInd.Value++;
        }).AddTo(disposables);

        Observable.EveryUpdate().Where(_ => (choisedLevelInd.Value - 1 > -1) && (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))).Subscribe(_ =>
        {
            choisedLevelInd.Value--;
        }).AddTo(disposables);
    }

    public void Save()
    {
        GameSave save = new GameSave();

        foreach (var level in levels)
        {
            save.levels.Add(level.GetModel());
        }

        BinaryFormatter bf = new BinaryFormatter();

        Directory.CreateDirectory(Application.persistentDataPath + "/Saves/");
        FileStream file = File.Create(Application.persistentDataPath + "/Saves/" + saveName + ".cube");

        bf.Serialize(file, save);
        file.Close();
    }

    void Start()
    {
        Init();

        if (File.Exists(Application.persistentDataPath + "/Saves/ " + saveName + ".cube"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/Saves/ " + saveName + ".cube", FileMode.Open);
            GameSave save = (GameSave)bf.Deserialize(file);

            for (int i = 0; i < save.levels.Count; ++i)
            {
                levels[i].SetModel(save.levels[i]);
            }

            file.Close();
        }
    }

    void OnLevelLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GameScene")
        {
            LevelController syncLC = levels[choisedLevelInd.Value];

            var viewObject = GameObject.Find("GameViewObject");
            // Через указанный LevelController создается его gameObject, после чего можно будет связаться с поведением уровня.
            var levelObject = Instantiate(syncLC.gameObject);
            // Через указанный ShipController создается его gameObject. Это нужно только для разных кораблей(чтобы ни 1 статичный вечно тусил на сцене).
            var shipObject = Instantiate(ship.gameObject, new Vector3(0, -32, -10), new Quaternion());

            // Что это такое? Это синхронизация пулла уровней, который настраивается в редакторе и относится к EditorRuntime,
            // и объекта уровня, создаваемоего при старте игровой сцены. В дальнейшем уровень модифицирует модель, которая может отображаться в главной сцене\сохраняться и т.д.
            var lc = levelObject.GetComponent<LevelController>();
            loadedLevel = lc;
            lc.SetModel(syncLC.GetModel());

            // Binding //

            shipObject.GetComponent<ShipController>().view = viewObject.GetComponent<GameView>();
            lc.view = viewObject.GetComponent<GameView>();

            lc.GetModel().isPassed.Where(x => x == true).Subscribe(_ => Save());

            // Устанавливаем скроллящиеся картинки
            var scrollImages = viewObject.GetComponentsInChildren<Image>();
            lc.GetBehavior().SetImages(scrollImages[0], scrollImages[1]);

            levelObject.GetComponentInChildren<LevelController>().StartLevel(ship.GetModel());
        }
        else
        {
            inst.Init();
        }
    }

    public LevelController GetLevelController()
    {
        return loadedLevel;
    }
}
