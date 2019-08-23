using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class ShipController : MonoBehaviour
{
    public int startHp = 3;
    public float fireRate = 0.2f;

    private float rateFromLast = 0.0f;

    private ShipModel model { get; set; }
    public GameView view;
    public Bullet bullet;

    private GameController gameController;
    private CompositeDisposable disposables;

    void Start()
    {
        disposables = new CompositeDisposable();
        model = new ShipModel(startHp);
        model.hp.AsObservable().Subscribe(x => view.lives.text = "Осталось до смерти: " + x).AddTo(disposables);
        Observable.EveryUpdate().Where(_ => Input.GetKey(KeyCode.A)).Subscribe(_ => MoveLeft()).AddTo(disposables);
        Observable.EveryUpdate().Where(_ => Input.GetKey(KeyCode.D)).Subscribe(_ => MoveRight()).AddTo(disposables);
        Observable.EveryUpdate().Where(_ => Input.GetMouseButton(0) && rateFromLast >= fireRate).Subscribe(_ => Fire()).AddTo(disposables);

        gameController = GameObject.Find("GameController").GetComponent<GameController>();
    }

    void OnDestroy()
    {
        disposables.Dispose();
    }

    void MoveRight()
    {
        gameObject.transform.Translate(1.0f, 0, 0);
    }

    void MoveLeft()
    {
        gameObject.transform.Translate(-1.0f, 0, 0);
    }

    public void Fire()
    {
        var bullet = gameController.GetLevelController().GetBehavior().cache.GetObjectByTag("bullet");
        if (bullet != null)
        {
            var ship_p = gameObject.transform.position;

            bullet.transform.position = new Vector3(ship_p.x + 5, ship_p.y + 15, ship_p.z);
            rateFromLast = 0;
        }
    }

    public ShipModel GetModel()
    {
        return model;
    }

    void Update()
    {
        if (rateFromLast < fireRate) rateFromLast += Time.deltaTime;
    }
}
