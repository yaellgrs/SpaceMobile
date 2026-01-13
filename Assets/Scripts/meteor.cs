using NUnit.Compatibility;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Timeline;
using UnityEngine.UI;
using UnityEngine.UIElements;
using System.Collections.Generic;
using Image = UnityEngine.UI.Image;

public class spaceObject : MonoBehaviour
{

    public TextMeshProUGUI lifeText;
    public GameObject ironCollectiblePrefab;
    public GameObject uraniumCollectiblePrefab;
    public GameObject diamandCollectiblePrefab;

    public float spaceObjectSpeed;
    public BigNumber lifeMax = new BigNumber(2, 0);
    public BigNumber life;
    Vector3 direction;
    bool isStart = true;
    bool isOmega = false;
    bool isUp = true;
    public float spawnTime = 0f;
    public bool isPause = false;


    public Vector3 baseScale;

    public enum meteorType { Normal, Big, Scatter, Diamand, miniMeteor, Iron, Uranium};
    public meteorType type;

    public bool isDestroyByRocket = false;


    public void Init()
    {
        

        if (type != meteorType.miniMeteor)
        {
            Spawn();
        }

        int x = Stats.Instance.stage - 1;
        float hp = 3 + (x * 1.5f) + Mathf.Pow(x, 1.25f);
        lifeMax = new BigNumber((int)hp);
        //lifeMax = new BigNumber( 2 + (Mathf.Pow(1.2f, Stats.Instance.stage)));


        if(type == meteorType.Big)
        {
            lifeMax.Multiply(5);
            if (!Stats.Instance.popupTutos[PopupTuto.BigMeteor]) Tuto.Instance.LoadPopupTuto(PopupTuto.BigMeteor);
        }
        else if(type == meteorType.Diamand)
        {
            lifeMax.Multiply(2f);
            if (!Stats.Instance.popupTutos[PopupTuto.diamandMeteor]) Tuto.Instance.LoadPopupTuto(PopupTuto.diamandMeteor);
        }
        else if(type == meteorType.miniMeteor)
        {
            lifeMax.Multiply(0.5f);
        }
        else if(type == meteorType.Iron)
        {
            lifeMax.Multiply(2.5f);
            if (!Stats.Instance.popupTutos[PopupTuto.ironMeteor]) Tuto.Instance.LoadPopupTuto(PopupTuto.ironMeteor);
        }
        else if(type == meteorType.Uranium)
        {
            if (!Stats.Instance.popupTutos[PopupTuto.uraniumMeteor]) Tuto.Instance.LoadPopupTuto(PopupTuto.uraniumMeteor);
        }
        else if(type == meteorType.Scatter)
        {
            if (!Stats.Instance.popupTutos[PopupTuto.splitterMeteor]) Tuto.Instance.LoadPopupTuto(PopupTuto.splitterMeteor);
        }

            List<(int stage, float mult)> paliers = new()
        {
            (10, 1.2f),
            (25, 1.5f),
            (50, 1.5f),
            (100, 1.75f),
        };

        foreach(var palier in paliers)
        {
            if(Stats.Instance.stage >= palier.stage)
            {
                lifeMax.Multiply(palier.mult);
            }
        }

        life = new BigNumber(lifeMax.Mantisse, lifeMax.Exp);
        transform.localScale *= gameManager.instance.meteorScale;
        lifeText.text = life.ToString();
        if (Stats.Instance.prestigeUnlocked)
        {
            if (Random.Range(0, 100) <= Stats.Instance.probabilitéOfOmega + 50)
            {
                isOmega = true;
                GetComponentInChildren<ParticleSystem>()?.gameObject.SetActive(true);
            }
            else GetComponentInChildren<ParticleSystem>()?.gameObject.SetActive(false);
        }

        loadSpeed();
        transform.localScale = baseScale;
        transform.localScale *= Stats.Instance.scale;
        setFontSize();
    }

    private void setFontSize()
    {
        if(new BigNumber(10).isBigger(lifeMax))
        {
            lifeText.fontSize = 1250;
        }
        else if (new BigNumber(100).isBigger(lifeMax))
        {
            lifeText.fontSize = 1000;
        }
    }

    public void loadSpeed()
    {
        spaceObjectSpeed = 0.75f;
        if (type == meteorType.Big)
        {
            spaceObjectSpeed *= 0.5f;
        }
        else if(type == meteorType.Diamand)
        {
            spaceObjectSpeed *= 1.5f;
        }
        else if(type== meteorType.miniMeteor)
        {
            spaceObjectSpeed *= 0.35f;
        }
            spaceObjectSpeed *= UpSpeed.Instance.upModeMultiplicator;
        Move();
    }

    private void Update()
    {
        spawnTime += Time.deltaTime;
        UpLife();

    }

    public void Move()
    {

        isPause = gameManager.instance.isPaused;
        if (isPause) return;
        if (isStart)
        {
            direction = spaceShip.instance.transform.position - transform.position;
            isStart = false;
        }
        float angle = Mathf.Atan2(direction.y, direction.x);
        GetComponent<Rigidbody2D>().linearVelocity = direction.normalized * spaceObjectSpeed;
        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        gameObject.transform.rotation = Quaternion.Euler(0, 0, angle-180);
        GetComponentInChildren<Canvas>().transform.rotation = Quaternion.Euler(0, 0,0);
    }
    
    private void Spawn()
    {
        Vector3 bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 topRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));
        float x, y;
        float side = Random.Range(0, 4);

        switch (side)
        {
            case 0: //Haut
                x = Random.Range(bottomLeft.x, topRight.x);
                y = topRight.y + 0.5f;
                break;
            case 1: //Bas
                x = Random.Range(bottomLeft.x, topRight.x);
                y = bottomLeft.y - 0.5f;
                break;
            case 2://Droit
                x = topRight.x + 0.5f;
                y = Random.Range(bottomLeft.y, topRight.y);
                break;
            case 3://Gauche
                x = bottomLeft.x - 0.5f;
                y = Random.Range(bottomLeft.y, topRight.y);
                break;
            default:
                x = 0;
                y = 0;
                break;
        }
        transform.position = new Vector3(x, y, 0);
    }

    public void UpLife()
    {
        lifeText.text = life.ToString();
        if (life.EqualZero() || lifeText.text == "0")
        {
            DieCalcul();
            Destroy(gameObject);
        }
    }

    public virtual void DieCalcul()
    {
        if(MainUi.Instance.questUI.type == QuestUI.questType.MeteorToKill)
        {
            MainUi.Instance.questUI.upQuest();
        }

        if(type != meteorType.miniMeteor && type != meteorType.Diamand)
        {
            gameManager.instance.meteorKilled++;
            MainUi.Instance.upMeteorUI();
        }

        if (type == meteorType.Diamand)
        {
            GameObject obj = Instantiate(diamandCollectiblePrefab);
            Collectible collectible = obj.GetComponent<Collectible>();
            collectible.Init(transform.position);
        }
        else if (type == meteorType.Iron)
        {
            GameObject obj = Instantiate(ironCollectiblePrefab);
            Collectible collectible = obj.GetComponent<Collectible>();
            collectible.Init(transform.position);
        }
        else if (type == meteorType.Uranium)
        {
            GameObject obj = Instantiate(uraniumCollectiblePrefab);
            Collectible collectible = obj.GetComponent<Collectible>();
            collectible.Init(transform.position);
        }
        else
        {
            if (Settings.Instance.displayXpMarker)
            {
                Stats.Instance.AddXP(calculXp());
                PoolManager.Instance.LaunchPrefab(transform.position, calculXp().ToString(), MarkerType.Xp);
            }
        }
        //Death
        if (isOmega)
        {
            Stats.Instance.upPrestigeWaiting(new BigNumber(Stats.Instance.stage), true);
            PoolManager.Instance.LaunchPrefab(transform.position, Stats.Instance.stage.ToString(), MarkerType.Prestige);
        }
        if (type == meteorType.Scatter)
        {
            if(!isDestroyByRocket) gameManager.instance.launchMiniMeteor(transform);
        }

        Song.Instance.playSound(Song.Instance.meteor_sound);
        Data.Instance.meteorKilled += 1;
        gameManager.instance.SmallVibrate();
    }

    public void Pause()
    {
        GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        isPause = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if(collision.gameObject.layer == LayerMask.NameToLayer("spaceShip"))
        {
            if(type != meteorType.Diamand && !(Stats.Instance.life.EqualZero()))
            {
                spaceShip.instance.getDamage(lifeMax);
                gameManager.instance.meteorKilled++;
                MainUi.Instance.upMeteorUI();
                if (Stats.Instance.life.EqualZero())
                {
                    Song.Instance.lauchTransitionMusic(Song.Instance.main_music, Song.Instance.dead_music);
                }
            }

            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("area"))
        {
            spaceObjectSpeed /= Stats.Instance.areaSpeed;
            Pause();
            Move();
        }
    }

    public BigNumber calculXp()
    {
        BigNumber result = new BigNumber(Stats.Instance.stage * 1.25f);
        //xpToLevelUp = 50 * 1.15^level
        //XpByMeteorDestroyed = 0.5 * stage * 1.1^stage
        //  Stats.Instance.xpLevelUp = new BigNumber(50 * Mathf.Pow(1.15f, Stats.Instance.level));
        switch (type)
        {
            case meteorType.Big:
                result *= 5f;
                break;
        }
        if(Stats.Instance.xpBoostTime > 0)
        {
            result *= 2;
        }
        result *= Stats.Instance.XpMultiplicator;
        return result;
    }
}
