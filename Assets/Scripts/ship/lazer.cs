using UnityEngine;

public class Lazer : MonoBehaviour
{


    float timer;
    float lifeTime = 3f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public BigNumber degats = new BigNumber(1, 0);

    private AudioSource lazerSound;

    public bool isRocket = false;


    private void Start()
    {
        lazerSound = GetComponent<AudioSource>();
        lazerSound.time = 0.025f;
        if (Settings.Instance.activeSound)
            GetComponent<AudioSource>().volume = (Settings.Instance.sound_general_value / 100) * (Settings.Instance.sound_effect_value / 100);
        else
            GetComponent<AudioSource>().volume = 0;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if(timer > lifeTime)
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.gameObject.layer == LayerMask.NameToLayer("spaceObject"))
        {
            spaceObject meteor = collision.gameObject.GetComponent<spaceObject>();

            if (meteor != null && meteor.spawnTime > 0.1f)
            {
                BigNumber dmg = new BigNumber(degats.Mantisse, degats.Exp);
                dmg.Multiply(Stats.Instance.prest_damage_multiplicator);
                dmg.Multiply(Stats.Instance.damage_Multiplicator_Lvl);
                dmg.Multiply(Stats.Instance.perm_Damage_Multiplicator_Lvl);
                if (Stats.Instance.damageBoostTime > 0) dmg.Multiply(2);


                bool critic = false;
                if (UnityEngine.Random.Range(0, 1000) <= Stats.Instance.critical_Prob)
                {
                    dmg.Multiply(5);
                    critic = true;
                }

                meteor.life.Subtract(dmg);

                if (isRocket)
                {
                    if (meteor.life.EqualZero() || meteor.lifeText.text == "0")
                    {
                        meteor.isDestroyByRocket = true;
                    }
                }


                if (Settings.Instance.displayDamageMarker)
                {
                    if (critic)
                    {
                        PoolManager.Instance.LaunchPrefab(transform.position, dmg.ToString(), PoolManager.markerType.Critique);
                    }
                    else
                    {
                        PoolManager.Instance.LaunchPrefab(transform.position, dmg.ToString(), PoolManager.markerType.Damage);
                    }
                        
                }
                Destroy(gameObject);
            }

        }
    }
}
