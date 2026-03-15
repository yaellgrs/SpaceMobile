using System.Collections.Generic;
using UnityEngine;

public enum BossType { Normal, Ressource, Speed };

public class meteorBoss : spaceObject
{
    public enum AttackStatut { Waiting, Launch, Attack}

    public BossType bossType;
    public AttackStatut statut = AttackStatut.Waiting;
    private float statutTimer = 0f;
    private float statutTimerLimit = 0f;

    //attack
    private int wave = 0;
    private float attackTimer = 0f;
    private const float ATTACK_TIMER_LIMITE = 1.5f;

    public spaceObject firstWavePrefab;
    public spaceObject secondWavePrefab;
    public spaceObject thirdWavePrefab;


    public override void Init(bool spawn = true)
    {
        base.Init();
        //keep
        if (Stats.Instance.ReduceLifeBoss)
        {
            life *= 0.6f;
            Stats.Instance.ReduceLifeBoss = false;
        }
        setTimerLimit();
        setAnimation();
        attackTimer = ATTACK_TIMER_LIMITE;
    }
    protected override bool CanBeStellar()
    {
        return true;
    }

    protected override int GetStellarProbability()
    {
        return Stats.Instance.prestigeUnlocked ? Utility.GetStellarBossProbability() : 1000;
    }

    protected override void setFontSize()
    {
        //la barre du bas suffit
        lifeText.gameObject.SetActive(false);   
    }
    protected override void setLife()
    {
        lifeMax *= 15;
    }
    public override void loadSpeed(float factor =1f)
    {
        float baseSpeed = 0.75f;

        spaceObjectSpeed = bossType switch
        {
            BossType.Normal or BossType.Ressource => baseSpeed * 0.75f * factor,
            BossType.Speed => baseSpeed * 2f,
            _ => baseSpeed,
        };
            


    }

    // Update is called once per frame
    protected override void Update()
    {
        if (gameManager.instance.isPaused) { return; }
        base.Update();
        statutTimer += Time.deltaTime;

        if (statut == AttackStatut.Attack)
            attackTimer += Time.deltaTime;

        if(statutTimer > statutTimerLimit)
        {
            setNextStatut();
            statutTimer = 0f;
        }
        Move();
        if (attackTimer > ATTACK_TIMER_LIMITE){
            Attack();
            attackTimer = 0f;
        }
    }

    private void Attack()
    {
        if (wave > 3) return;
        meteorType type;
        if (bossType == BossType.Normal)
        {
            type = wave switch
            {
                1 => meteorType.Big,
                2 => meteorType.Scatter,
                _ => meteorType.Normal,
            };
        }
        else if(bossType == BossType.Ressource)
        {
            type = !Ship.Current.HaveUranium() ? meteorType.Iron : Random.Range(0, 2) == 1 ?
                                meteorType.Iron : meteorType.Uranium;
        }
        else
            type = meteorType.None;

        if(type != meteorType.None) gameManager.instance.SpawnMeteor(type, transform.position, false);
    }

    public override void Move()
    {

        Vector3 shipDir = (spaceShip.instance.transform.position - transform.position).normalized;
        Vector3 perp = new Vector3(-shipDir.y, shipDir.x, 0).normalized;
        Vector3 dir = (shipDir + perp * 6.5f).normalized;
        transform.position += dir * spaceObjectSpeed * Time.deltaTime * Stats.Instance.scale;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        gameObject.transform.rotation = Quaternion.Euler(0, 0, angle - 180);
    }
    protected override void Spawn()
    {
        Vector3 bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 topRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));
        float side = Random.Range(0, 4);

        float x = bottomLeft.x - 0.5f;
        float y = topRight.y * 0.55f;

        transform.position = new Vector3(x, y, 0);
    }

    private void setTimerLimit()
    {
        float[] limits = { 2f, 3f, ATTACK_TIMER_LIMITE*3 };
        statutTimerLimit = limits[(int)statut];  
    }

    private void setAnimation()
    {
        Animator animator = GetComponent<Animator>();
        animator.SetBool("isAttacking", statut == AttackStatut.Attack);
        animator.SetBool("isWaiting", statut == AttackStatut.Waiting);
        animator.SetBool("isLaunch", statut == AttackStatut.Launch);
    }

    private void setNextStatut()
    {
        if (!canAttack()) return;


        if(statut == AttackStatut.Attack) {
            statut = AttackStatut.Waiting;

        }
        else if(wave < 3)
            statut++;

        if (statut == AttackStatut.Launch){
            gameManager.instance.activeWarning(true);
            StartCoroutine(SoundManager.Instance.PlaySoundWithTime(SoundEffectType.BossWarning,  3f));
        }
        else if (statut == AttackStatut.Attack){
            gameManager.instance.activeWarning(false);

            wave++;

        }

        setAnimation();
        setTimerLimit();
    }

    private bool canAttack()
    {
        if (bossType == BossType.Speed) return false;
        else return wave <= 3;
    }

    private void OnDestroy()
    {
        gameManager.instance.meteors.Remove(this);
        gameManager.instance.activeWarning(false);
        if(!Ship.Current.life.EqualZero()){
            gameManager.instance.upStage();
            gameManager.instance.bossStage = false;
            SoundManager.Instance.lauchTransitionMusic(MusicType.Main);
        }
        else
        {
            //SoundManager.Instance.lauchTransitionMusic(MusicType.Main);
        }


        if (gameManager.instance.fragmentBoss) BossFragmentUi.EndFragmentBoss(true);
        if (isStellar) Stats.Instance.prestigeUnlocked = true;
    }
}
