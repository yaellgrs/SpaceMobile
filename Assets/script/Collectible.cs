using NUnit.Framework.Constraints;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    private BigNumber reward = new BigNumber(10);
    public float time = 5f;

    private bool collected = false;

    public enum CollectibleType { iron, uranium, Diamand };

    public CollectibleType type;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Init(Vector3 position)
    {
        transform.position = position;
        switch (type)
        {
            case CollectibleType.iron:
                reward = new BigNumber(OfflineUI.calculOfflineIronEarn(30, false));
                break;
            case CollectibleType.uranium:
                reward = new BigNumber(OfflineUI.calculOfflineUraniumEarn(30, false));
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!collected)
        {
            time -= Time.deltaTime;
            if (time > 0)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Vector3 pos = Input.mousePosition;
                    pos.z = Camera.main.WorldToScreenPoint(transform.position).z;
                    Vector3 worldPos = Camera.main.ScreenToWorldPoint(pos);
                    checkCollected(worldPos);
                }
                else if (Input.touchCount > 0)
                {
                    Vector3 pos = Input.touches[0].position;
                    pos.z = Camera.main.WorldToScreenPoint(transform.position).z;
                    Vector3 worldPos = Camera.main.ScreenToWorldPoint(pos);
                    checkCollected(worldPos);
                }
            }
            else
            {
                collected = true;
                Collect();
            }
        }
        else
        {
            transform.localScale *= 0.995f;
            if(transform.position.y < -4.5f)
            {

                switch (type)
                {
                    case CollectibleType.iron:
                        Stats.Instance.upIron(reward, true);
                        if (MainUi.Instance.questUI.type == QuestUI.questType.ironMeteor) MainUi.Instance.questUI.upQuest(reward);
                        break;
                    case CollectibleType.uranium:
                        Stats.Instance.upUranium(reward, true);
                        if (MainUi.Instance.questUI.type == QuestUI.questType.uraniumMeteor) MainUi.Instance.questUI.upQuest(reward);
                        break;
                    case CollectibleType.Diamand:
                        Stats.Instance.upDiamand(1, true);
                        break;
                }

                Destroy(gameObject);
            }
        }

        

    }

    private void checkCollected(Vector3 pos)
    {
;
        float dist = Vector3.Distance(pos, transform.position);

        if (dist <= 0.5f)
        {
            collected = true;
            Collect();
        }
    }

    private void Collect()
    {

        Vector3 bottomCenterScreen = new Vector3(Screen.width / 2f, 0, Camera.main.WorldToScreenPoint(transform.position).z);
        Vector3 bottomCenterWorld = Camera.main.ScreenToWorldPoint(bottomCenterScreen);

        Vector3 direction = bottomCenterWorld - transform.position;

        GetComponent<Rigidbody2D>().AddForce(direction*60);
        
        switch (type)
        {
            case CollectibleType.iron:
                PoolManager.Instance.LaunchPrefab(transform.position, reward.ToString(), PoolManager.markerType.Iron);
                break;
            case CollectibleType.uranium:
                PoolManager.Instance.LaunchPrefab(transform.position, reward.ToString(), PoolManager.markerType.Uranium);
                break;
            case CollectibleType.Diamand:
                PoolManager.Instance.LaunchPrefab(transform.position, "", PoolManager.markerType.Diamand);
                break;
        }

    }


}
