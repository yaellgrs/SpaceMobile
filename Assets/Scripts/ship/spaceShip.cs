using Unity.VisualScripting;
using UnityEngine;

public class spaceShip : MonoBehaviour
{

    static public spaceShip instance;
    public GameObject area;

    public BigNumber damage = new BigNumber(1, 0);
    public float shootTimer = 0.1f;
    public float shieldRegen = 0f;

    Vector3 initAreaScale;
    Vector3 initScale;

    private float anim = 0f;
    private bool animUp = false;
    public float animSpeed = 0.0025f;

    private Animator animator;

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
        animator.SetBool("isFire", Stats.Instance.currentSpaceShipType == SpaceShipType.Fire);
    }

    // Update is called once per frame
    void Update()
    {
        Animation();
        if (!Stats.Instance.shield.isBigger(getMaxShield()))
        {
            if (!gameManager.instance.isPaused)
            {
                shieldRegen += Time.deltaTime;
            }
            MainUi.Instance.upShieldRegenUI();
            if (shieldRegen > Stats.Instance.shield_Regen_Time && Stats.Instance.life.isBigger(new BigNumber(0)))
            {
                BigNumber x = new BigNumber(getMaxShield());
                x.Subtract(Stats.Instance.shield);
                if (x.isBigger(Stats.Instance.regenShield))
                {
                    Stats.Instance.shield.Add(Stats.Instance.regenShield);
                }
                else
                {
                    Stats.Instance.shield.Add(x);
                }
                MainUi.Instance.upShieldBar();
                shieldRegen = 0;
            }

        }
        if ((MainUi.Instance.healthBar.resolvedStyle.width / MainUi.Instance.healthBar.resolvedStyle.maxWidth.value) * 100f < 1f && new BigNumber(1, 0).isBigger(Stats.Instance.life)) 
        {
            gameManager.instance.RestartStage();
            Handheld.Vibrate();
            if (Stats.Instance.stage % Stats.BOSS_STAGE_GAP == 0)
            {
                Stats.Instance.isDead = true;
                ResurectionUI.Instance.loadResurection();
            }
        }
    }

    private void Animation()
    {
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

    public void getDamage(BigNumber amount)
    {
        if (Stats.Instance.shield.isBigger(amount))
        {
            Stats.Instance.shield -= amount;
            if(new BigNumber(0).isBigger(Stats.Instance.shield)) Stats.Instance.shield.Set(0);
        }
        else
        {
            BigNumber x = new BigNumber(amount);
            x -= Stats.Instance.shield;
            if (Stats.Instance.shield.Mantisse != 0)
            {
                Stats.Instance.shield.Set(0);
            }

            Stats.Instance.life -= x;
            if (new BigNumber(0).isBigger(Stats.Instance.life)) Stats.Instance.life.Set(0);
        }

        MainUi.Instance.upShieldBar();
    }

    public void setAreaScale()
    {
        if (XpUI.rewardUnlocked(XpUI.BonusLevel.UnlockUranium))
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
        BigNumber life = new BigNumber(Stats.Instance.lifeMax);
        life.Multiply(Stats.Instance.life_Multiplicator_Lvl);
        if (Stats.Instance.pvShieldBoostTime > 0) {
            life.Multiply(2);
        }
        return life;
    }

    public BigNumber getMaxShield()
    {
        BigNumber shield = new BigNumber(Stats.Instance.shieldMax);
        shield.Multiply(Stats.Instance.shield_Multiplicator_Lvl);
        if (Stats.Instance.pvShieldBoostTime > 0)
        {
            shield.Multiply(2);
        }
        return shield;
    }
}
