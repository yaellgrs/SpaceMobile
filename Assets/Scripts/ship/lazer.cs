using UnityEngine;

public class Lazer : MonoBehaviour
{
    float timer;
    float lifeTime = 3f;

    public bool isRocket = false;

    private void Start()
    {
        SoundManager.Instance.PlaySound(SoundEffectType.Lazer);
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

        if (collision.TryGetComponent(out spaceObject meteor))
        {

            if (meteor != null && meteor.spawnTime > 0.1f)
            {

                float criticProb = Stats.Instance.critical_Prob % 1000;
                int LockCritic = (int)Stats.Instance.critical_Prob / 1000;
                bool critic = UnityEngine.Random.Range(0, 1000) <= criticProb;

                BigNumber dmg = new BigNumber(Ship.Current.damage.getTotal(isRocket, critic, LockCritic));
                if (Stats.Instance.boosts.ContainsKey(Boost.Type.damage))
                {
                    if (Stats.Instance.boosts[Boost.Type.damage].time > 0f) 
                        dmg *= Stats.Instance.boosts[Boost.Type.damage].coef;
                }
                meteor.life.Subtract(dmg);

                if (isRocket)
                {
                    if (meteor.life.EqualZero() || meteor.lifeText.text == "0")
                        meteor.isDestroyByRocket = true;
                }
                if (Settings.Instance.displayDamageMarker)
                {
                    MarkerType type = critic ? MarkerType.Critique : MarkerType.Damage;
                    MarkersUI.Instance.ShowMarker(transform.position, "+" + dmg.ToString(), type);
                }
                meteor.UpLife();
                Destroy(gameObject);
            }
        }
    }
}
