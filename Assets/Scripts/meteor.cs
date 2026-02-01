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

    public ParticleSystem starParticle;
    public ParticleSystem meteorParticle;

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

    public enum meteorType { Normal, Big, Scatter, Diamand, miniMeteor, Iron, Uranium, Boss};
    public meteorType type;

    public bool isDestroyByRocket = false;


    public virtual void Init()
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
        else if(type == meteorType.Boss)
        {
           lifeMax *= 15;
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
                lifeMax *= palier.mult;
            }
        }

        life = new BigNumber(lifeMax);
        if (type == meteorType.Boss && Stats.Instance.ReduceLifeBoss) {
            life *= 0.6f;
            Stats.Instance.ReduceLifeBoss = false;  
        }

        transform.localScale *= Stats.Instance.scale;
        lifeText.text = life.ToString();
        if (Stats.Instance.prestigeUnlocked && Stats.Instance.stage >= 10)
        {
            if (Random.Range(0, 1000) <= Stats.Instance.probabilitéOfOmega*10 || true)
            {
                isOmega = true;
                starParticle.gameObject.SetActive(true);
            }
            else starParticle.gameObject.SetActive(false);
        }
        else starParticle.gameObject.SetActive(false);

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
        else if (type == meteorType.Boss)
        {
            spaceObjectSpeed *= 0.75f;
        }
        spaceObjectSpeed *= UpSpeed.Instance.upModeMultiplicator;
        if(type != meteorType.Boss)
        {
            Move();
        }
    }

    private void Update()
    {
        spawnTime += Time.deltaTime;
        UpLife();
        if (type == meteorType.Boss) MoveBoss();
    }

    public void MoveBoss()
    {
        if (isPause) return;

        Vector3 shipDir = (spaceShip.instance.transform.position - transform.position).normalized;
        Vector3 perp = new Vector3(-shipDir.y, shipDir.x, 0).normalized;
        Vector3 dir = ( shipDir + perp * 5f ).normalized;
        transform.position += dir * spaceObjectSpeed * Time.deltaTime * Stats.Instance.scale;


        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        gameObject.transform.rotation = Quaternion.Euler(0, 0, angle - 180);
    }

    public void Move()
    {
        isPause = gameManager.instance.isPaused;
        if (type == meteorType.Boss || isPause) return;
        if (isStart)
        {
            direction = spaceShip.instance.transform.position - transform.position;
            isStart = false;
        }
        float angle = Mathf.Atan2(direction.y, direction.x);
        GetComponent<Rigidbody2D>().linearVelocity = direction.normalized * spaceObjectSpeed * Stats.Instance.scale;
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
        if(type == meteorType.Boss)
        {
            x = bottomLeft.x - 0.5f;
            y = topRight.y * 0.55f;
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
        if(QuestManager.Instance.type == QuestType.KillMeteor)
        {
            QuestManager.Instance.upQuest();
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
            Data.Instance.diamandMeteorKilled += 1;
        }
        else if (type == meteorType.Iron)
        {
            GameObject obj = Instantiate(ironCollectiblePrefab);
            Collectible collectible = obj.GetComponent<Collectible>();
            collectible.Init(transform.position);
            Data.Instance.ironMeteorKilled += 1;
        }
        else if (type == meteorType.Uranium)
        {
            GameObject obj = Instantiate(uraniumCollectiblePrefab);
            Collectible collectible = obj.GetComponent<Collectible>();
            collectible.Init(transform.position);
            Data.Instance.uraniumMeteorKilled += 1;
        }
        else
        {
            if (Settings.Instance.displayXpMarker)
            {
                Stats.Instance.AddXP(calculXp());
                PoolManager.Instance.LaunchPrefab(transform.position, calculXp().ToString(), MarkerType.Xp);
            }
            Data.Instance.basicMeteorKilled += 1;
        }
        //Death
        if (isOmega)
        {
            
            Stats.Instance.AddPrestigeWainting(new BigNumber(Stats.Instance.stage) * 0.5f);
            PoolManager.Instance.LaunchPrefab(transform.position, Stats.Instance.stage.ToString(), MarkerType.Prestige);
            Data.Instance.OmegaMeteorKilled += 1;
        }
        if (type == meteorType.Scatter)
        {
            if(!isDestroyByRocket) gameManager.instance.launchMiniMeteor(transform);
            Data.Instance.splitterMeteorKilled += 1;
        }

        Song.Instance.playSound(Song.Instance.meteor_sound);
        Data.Instance.meteorKilled += 1;
        gameManager.instance.SmallVibrate();
        meteorParticle.transform.SetParent(null);
        meteorParticle.transform.position = transform.position; 
        meteorParticle.Play();
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
        //  Stats.Instance.BN_xpMax = new BigNumber(50 * Mathf.Pow(1.15f, Ship.Current.level));
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


    private void OnDestroy()
    {
        gameManager.instance.meteors.Remove(this);

        if(type == meteorType.Boss) gameManager.instance.bossStage = false;
    }
}
