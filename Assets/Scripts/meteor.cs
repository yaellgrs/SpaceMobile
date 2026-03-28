using GoogleMobileAds.Api;
using NUnit.Compatibility;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Timeline;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class spaceObject : MonoBehaviour
{
    public TextMeshProUGUI lifeText;
    public GameObject ironCollectiblePrefab;
    public GameObject uraniumCollectiblePrefab;
    public GameObject diamandCollectiblePrefab;

    public ParticleSystem starParticle;
    public ParticleSystem meteorParticle;

    private static List<(int stage, float mult)> lifePaliers = new()
    {
        (10, 1.2f),
        (25, 1.5f),
        (50, 1.5f),
        (100, 1.75f),
    };
    private static List<(int stage, float mult)> speedPaliers = new()
    {
        (10, 1.05f),
        (25, 1.1f),
        (50, 1.1f),
        (100, 1.15f),
    };

    public float spaceObjectSpeed;
    public BigNumber lifeMax = new BigNumber(2, 0);
    public BigNumber life;
    Vector3 direction;
    protected bool isStart = true;
    protected bool isStellar = false;
    protected bool isUp = true;
    public float spawnTime = 0f;
    public bool isPause = false;


    public Vector3 baseScale;
    public int level = 1;

    public enum meteorType { Normal, Big, Scatter, Diamand, miniMeteor, Iron, Uranium, None};
    public meteorType type;

    public bool isDestroyByRocket = false;


    public virtual void Init(bool spawn = true)
    {
        if (type != meteorType.miniMeteor && spawn)
            Spawn();
        int x = level - 1;

        //float hp = 3 + (x * 1.5f) + Mathf.Pow(x, 1.25f);
        float hp = 10 + Mathf.Pow(x, 1.75f);
        lifeMax = new BigNumber((int)hp);

        setLife();
        lifeMax *= getStageModifier(lifePaliers);

        life = new BigNumber(lifeMax);

        transform.localScale *= Stats.Instance.scale;
        lifeText.text = life.ToString();

        if (CanBeStellar())
        {
            if (Random.Range(0, 1000) <= GetStellarProbability())
            {
                isStellar = true;
                starParticle.gameObject.SetActive(true);
                Debug.Log("is stellar");
            }
            else starParticle.gameObject.SetActive(false);
        }
        else starParticle.gameObject.SetActive(false);

        loadSpeed();
        spaceObjectSpeed *= getStageModifier(speedPaliers);
        transform.localScale = baseScale;
        transform.localScale *= Stats.Instance.scale;
        setFontSize();
    }

    protected virtual bool CanBeStellar()
    {
        return Stats.Instance.prestigeUnlocked && Ship.Current.stage >= Consts.MINIMUM_STAR_PARTICULE_STAGE;
    }

    protected virtual int GetStellarProbability()
    {
        return Utility.GetStellarMeteorProbability();
    }

    protected virtual void setLife()
    {
        if (type == meteorType.Big)
        {
            lifeMax.Multiply(2.5f);
            if (!Stats.Instance.popupTutos[PopupTuto.BigMeteor]) Tuto.Instance.LoadPopupTuto(PopupTuto.BigMeteor);
        }
        else if (type == meteorType.Diamand)
        {
            lifeMax.Multiply(1.5f);
            if (!Stats.Instance.popupTutos[PopupTuto.diamandMeteor]) Tuto.Instance.LoadPopupTuto(PopupTuto.diamandMeteor);
        }
        else if (type == meteorType.miniMeteor)
        {
            lifeMax.Multiply(0.5f);
        }
        else if (type == meteorType.Iron)
        {
            lifeMax.Multiply(1.5f);
            if (!Stats.Instance.popupTutos[PopupTuto.ironMeteor]) Tuto.Instance.LoadPopupTuto(PopupTuto.ironMeteor);
        }
        else if (type == meteorType.Uranium)
        {
            lifeMax.Multiply(1.5f); 
            if (!Stats.Instance.popupTutos[PopupTuto.uraniumMeteor]) Tuto.Instance.LoadPopupTuto(PopupTuto.uraniumMeteor);
        }
        else if (type == meteorType.Scatter)
        {
            if (!Stats.Instance.popupTutos[PopupTuto.splitterMeteor]) Tuto.Instance.LoadPopupTuto(PopupTuto.splitterMeteor);
        }


        float factor = Random.Range(0.75f, 1.25f);
        lifeMax *= factor;
    }

    protected float getStageModifier(List<(int stage, float mult)> paliers)
    {
        float mult = 1f;

        foreach (var palier in paliers)
        {
            if (Ship.Current.stage >= palier.stage)
                mult *= palier.mult;
        }

        return mult;
    }


    protected virtual void setFontSize()
    {
        if(new BigNumber(10).isBigger(lifeMax))
            lifeText.fontSize = 1250;
        else if (new BigNumber(100).isBigger(lifeMax))
            lifeText.fontSize = 1000;
    }

    public virtual void loadSpeed(float factor = 1f)
    {
        spaceObjectSpeed = 0.75f;
        if (type == meteorType.Big)
            spaceObjectSpeed *= 0.5f;
        else if(type == meteorType.Diamand)
            spaceObjectSpeed *= 1.25f;
        else if(type== meteorType.miniMeteor)
            spaceObjectSpeed *= 0.5f;
        else if(type == meteorType.Uranium || type == meteorType.Iron)
            spaceObjectSpeed *= 1.15f;


        spaceObjectSpeed *= UpSpeed.Instance.upModeMultiplicator * factor;

        List<(int stage, float mult)> paliers = new()
        {
            (10, 1.2f),
            (25, 1.5f),
            (50, 1.5f),
            (100, 1.75f),
        };

        Move();
    }

    protected virtual void Update() 
    {
        spawnTime += Time.deltaTime;
    }


    public virtual void Move()
    {
        isPause = gameManager.instance.isPaused;
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
    
    protected virtual void Spawn()
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
        if (Datas.Instance.current.meteorKilled.ContainsKey(type)) Datas.Instance.current.meteorKilled[type] += 1;
        else Datas.Instance.current.meteorKilled[type] = new BigNumber(1);

        if (QuestManager.Instance.type == QuestType.KillMeteor)
        {
            QuestManager.Instance.upQuest();
        }

        if(type != meteorType.miniMeteor && type != meteorType.Diamand && !gameManager.instance.bossStage)
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
                Ship.Current.AddXP(calculXp());
                MarkersUI.Instance.ShowMarker(transform.position, calculXp().ToString(), MarkerType.Xp);
            }

        }
        //Death
        if (isStellar)
        {
            
            Stats.Instance.AddPrestigeWainting(GetStarParticle());

            ShowStartParticleReward();
            //MarkersUI.Instance.ShowMarker(transform.position, Ship.Current.stage.ToString(), MarkerType.Prestige);
        }
        if (type == meteorType.Scatter)
        {
            if(!isDestroyByRocket) gameManager.instance.launchMiniMeteor(transform);
        }

        SoundManager.Instance.PlaySound(SoundEffectType.MeteorExplosion);
        gameManager.instance.SmallVibrate();
        meteorParticle.transform.SetParent(null);
        meteorParticle.transform.position = transform.position; 
        meteorParticle.Play();
    }

    public virtual BigNumber GetStarParticle()
    {
        BigNumber reward = new BigNumber(Ship.Current.stage) * Random.Range(0.25f, 0.5f);
        return reward;
    }

    private void ShowStartParticleReward()
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(1.15f * (Screen.width / 2f), 1f * (Screen.height / 3f), 10));
        MarkerType type = MarkerType.Prestige;
        MarkersUI.Instance.ShowMarker(worldPos, "Star Particle : " + GetStarParticle().ToString(), type, 0.1f, 0.985f, 1f);
    }

    public void SetPause(bool pause)
    {
        if (pause) GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        else Move();
        isPause = pause;
        GetComponent<Animator>().speed = pause ? 0f : 1f;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if(collision.gameObject.layer == LayerMask.NameToLayer("spaceShip"))
        {
            if(type != meteorType.Diamand && !(Ship.Current.life.EqualZero()))
            {
                Datas.Instance.current.missMeteor++;
                spaceShip.instance.getDamage(lifeMax, this is meteorBoss);
                MainUi.Instance.upMeteorUI();
                if (Ship.Current.life.EqualZero() && gameManager.instance.bossStage)
                {
                    SoundManager.Instance.lauchTransitionMusic(MusicType.Dead);
                    Datas.Instance.current.dead++;
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
            Move();
        }
    }

    public BigNumber calculXp()
    {
        BigNumber result = new BigNumber(Ship.Current.stage * 1.25f);
        //xpToLevelUp = 50 * 1.15^level
        //XpByMeteorDestroyed = 0.5 * stage * 1.1^stage
        //  Ship.Current.BN_xpMax = new BigNumber(50 * Mathf.Pow(1.15f, Ship.Current.level));
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
    }
}
