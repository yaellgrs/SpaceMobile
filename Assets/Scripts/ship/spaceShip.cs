using Unity.VisualScripting;
using UnityEngine;

public class spaceShip : MonoBehaviour
{

    static public spaceShip instance;
    public GameObject area;

    public float shootTimer = 0.1f;
    public float shieldRegen = 0f;

    Vector3 initAreaScale;
    Vector3 initScale;

    private float anim = 0f;
    private bool animUp = false;
    public float animSpeed = 0.0025f;

    private Animator animator;
    public RepelerLink repelerLink;
    private spaceObject repelerTarget;
    private float targetTimer = 0f;
    private const float RELOAD_TARGET_TIME = 1f;

    private bool isPause = false;



    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator  = GetComponent<Animator>();
        animator.speed = 0.75f;
        initAreaScale = area.transform.localScale;
        initScale = area.transform.localScale;
        LoadAnimation();
    }

    public void LoadAnimation()
    {
        //animator.SetBool("isFire", Stats.Instance.currentSpaceShipType == SpaceShipType.Fire);
    }

    public void SetPause(bool pause)
    {
        if (animator == null)return;
        animator.speed = pause ? 0f : 1f;

        Debug.Log("set pause : " + pause);
        isPause = pause;
    }

    // Update is called once per frame
    void Update()
    {
        Animation();
        if (!Ship.Current.shield.isBigger(getMaxShield())) //regen shield
        {
            if (!gameManager.instance.isPaused)
            {
                shieldRegen += Time.deltaTime;
            }
            MainUi.Instance.upShieldRegenUI();
            if (shieldRegen > Stats.Instance.shield_Regen_Time && Ship.Current.life.isBigger(new BigNumber(0)))
            {
                BigNumber x = new BigNumber(getMaxShield());
                x.Subtract(Ship.Current.shield);
                if (x.isBigger(Ship.Current.regenShield))
                {
                    Ship.Current.shield.Add(Ship.Current.regenShield);
                }
                else
                {
                    Ship.Current.shield.Add(x);
                }
                MainUi.Instance.upShieldBar();
                shieldRegen = 0;
            }
        }
        if ((MainUi.Instance.healthBar.resolvedStyle.width / MainUi.Instance.healthBar.resolvedStyle.maxWidth.value) * 100f < 1f && Ship.Current.life.Mantisse <= 0) 
        {
            gameManager.instance.RestartStage();
            Handheld.Vibrate();
            if (Ship.Current.stage % Stats.BOSS_STAGE_GAP == 0)
            {
                Ship.Current.isDead = true;
                ResurectionUI.Instance.loadResurection();
            }
        }
        UpdateRepeler();
    }

    private void UpdateRepeler()
    {
        if (!Utility.HaveTheShipUpgrade(UpgradesShipElement.UpgradeType.Magnectic)) return;
        targetTimer += Time.deltaTime;

        if (gameManager.instance.meteors.Count <= 0)
        {
            repelerLink.pointB = transform;
        }
        else if (repelerTarget != null && !Utility.isInScreen(repelerTarget.transform.position, 0.1f))
        {
            repelerTarget.loadSpeed();
            repelerLink.pointB = transform;
        }
        else if (targetTimer > RELOAD_TARGET_TIME)
        {
            targetTimer = 0f;
            if (repelerTarget != null) repelerTarget.loadSpeed();
            spaceObject target = Utility.FindNearestMeteor(transform.position);
            Vector3 viewportPos = Camera.main.WorldToViewportPoint(target.transform.position);
            if(Utility.isInScreen(target.transform.position, 0.1f))
            {
                repelerTarget = target;
                target.loadSpeed(Stats.Instance.shipUpgradesReward[UpgradesShipElement.UpgradeType.Magnectic]);

                repelerLink.pointB = repelerTarget.transform;
            }
        }
    }

    private void Animation()
    {
        if (isPause) return;
        float speed = animSpeed * UpSpeed.Instance.upModeMultiplicator * 100 * Time.deltaTime;
        if (animUp)
        {
            Vector3 pos = transform.position;
            pos.y += speed;
            anim += speed;
            transform.position = pos;
        }
        else
        {
            Vector3 pos = transform.position;
            pos.y -= speed;
            anim += speed;
            transform.position = pos;
        }
        if(anim >= 0.075f)
        {
            anim = 0f;
            animUp = !animUp;
        }
    }

    public void getDamage(BigNumber amount, bool boss = false)
    {
        if (boss)
        {
            Ship.Current.shield.Set(0);
            Ship.Current.life.Set(0);
            return;
        }

        if (Ship.Current.shield.isBigger(amount))
        {
            Ship.Current.shield -= amount;
            if(new BigNumber(0).isBigger(Ship.Current.shield)) Ship.Current.shield.Set(0);
        }
        else
        {
            BigNumber x = new BigNumber(amount);
            x -= Ship.Current.shield;
            if (Ship.Current.shield.Mantisse != 0)
            {
                Ship.Current.shield.Set(0);
            }

            Ship.Current.life -= x;
            if (Ship.Current.life.Mantisse < 0) Ship.Current.life.Set(0);
        }

        MainUi.Instance.upShieldBar();
    }

    public void setAreaScale()
    {
        if (Ship.Current.HaveUranium())
        {
            area.gameObject.SetActive(true);
            area.transform.localScale = new Vector3(0.5386925f, 0.4774579f, 1f);
            area.transform.localScale *= Stats.Instance.scale * Stats.Instance.areaSize;
        }
        else
        {
            area.gameObject.SetActive(false);
        }
    }
    public void setScale()
    {
        transform.localScale = new Vector3(0.75f, 0.75f, 1f);
        transform.localScale *= Stats.Instance.scale;
    }

    public BigNumber getMaxLife()
    {
        BigNumber life = new BigNumber(Ship.Current.lifeMax.getTotal());
        if (Stats.Instance.pvShieldBoostTime > 0) {
            life.Multiply(2);
        }
        return life;
    }

    public BigNumber getMaxShield()
    {
        BigNumber shield = new BigNumber(Ship.Current.shieldMax.getTotal());
        if (Stats.Instance.pvShieldBoostTime > 0)
        {
            shield.Multiply(2);
        }
        return shield;
    }
}
