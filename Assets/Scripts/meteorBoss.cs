using System.Collections.Generic;
using UnityEngine;

public class meteorBoss : spaceObject
{
    public enum BossType { Normal };
    public enum AttackStatut { Waiting, Launch, Attack}

    public BossType bossType;
    public AttackStatut statut = AttackStatut.Waiting;
    private float statutTimer = 0f;
    private float statutTimerLimit = 0f;


    public override void Init()
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
    }
    protected override void setLife()
    {
        lifeMax *= 15;
    }
    public override void loadSpeed()
    {
        spaceObjectSpeed = 0.75f;
        spaceObjectSpeed *= 0.75f;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        statutTimer += Time.deltaTime;
        if(statutTimer > statutTimerLimit)
        {
            setNextStatut();
            statutTimer = 0f;
        }
        Move();
    }

    public override void Move()
    {
        isPause = gameManager.instance.isPaused;
        if (isPause) {
            Debug.LogError("boss is paused");
            return; 
        }
        Debug.Log("Moving boss");

        Vector3 shipDir = (spaceShip.instance.transform.position - transform.position).normalized;
        Vector3 perp = new Vector3(-shipDir.y, shipDir.x, 0).normalized;
        Vector3 dir = (shipDir + perp * 5f).normalized;
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
        float[] limits = { 6f, 2f, 4f };
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
        if(statut == AttackStatut.Attack)   
            statut = AttackStatut.Waiting;
        else
            statut++;
        setAnimation();
        setTimerLimit();
    }

    private void OnDestroy()
    {
        gameManager.instance.meteors.Remove(this);

        gameManager.instance.bossStage = false;
    }
}
