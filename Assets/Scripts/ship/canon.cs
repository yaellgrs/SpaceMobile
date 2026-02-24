using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UIElements;

public class canon : MonoBehaviour
{
    static public canon instance;

    public Camera mainCam;
    public Lazer lazer;
    public Lazer rocket;

    float speed = 100f;
    public float degats = 2f;
    float shootTimer = 0.1f;
    float timer;

    public bool canFire = true;
    public float autoTimer = 0;


    private bool isPause = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        shootTimer = spaceShip.instance.shootTimer;
        timer = shootTimer;
    }
    // Update is called once per frame
    void Update()
    {
        if (!isPause)
        {
            timer += Time.deltaTime;

            moveCanon(); // appel shoot 
            if (XpUI.rewardUnlocked(XpUI.BonusLevel.UnlockUranium))
                autoShoot();
        }  
    }

    public void moveCanon()
    {
        if (canFire)
        {
            canFire = false;
            Vector3 touchPosition = Input.mousePosition;

            Vector3 pos = mainCam.ScreenToWorldPoint(touchPosition);
            Vector3 direction = pos - transform.position;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90);
            if (timer >= shootTimer)
            {
                shoot(angle, direction, 0);
                timer = 0f;
            }
        }
    }

    private void shoot(float angle, Vector3 direction, int type)
    {
        Lazer projectil;
        float rocketSpeed = 1f;
        if (type == 0)
        {
            projectil = Instantiate(lazer);
        }
        else
        {
            projectil = Instantiate(rocket);
            projectil.isRocket = true;
            rocketSpeed = 0.5f;
        }
        projectil.transform.rotation = Quaternion.Euler(0, 0, angle - 90);
        angle = angle * Mathf.Deg2Rad;
        projectil.transform.position = transform.position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0)*0.3f;
        Vector2 force = direction * speed * rocketSpeed;
        force.Normalize();
        projectil.GetComponent<Rigidbody2D>().AddForce(force*250);
    }

    public void setPause(bool pause)
    {
        isPause = pause;
    }

    public void autoShoot()
    {
        autoTimer += Time.deltaTime;
        if (gameManager.instance.meteors.Count > 0)
        {
            spaceObject meteor = Utility.FindMeteor();
            if (meteor != null)
            {
                if (autoTimer > Stats.Instance.speedAuto)
                {
                    autoTimer = 0f;
                    Vector3 position = meteor.gameObject.transform.position;
                    Vector3 vect = position - transform.position;
                    float angle = Mathf.Atan2(vect.y, vect.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.Euler(0, 0, angle - 90);
                    shoot(angle, vect, 0);
                }
            }
        }
    }

    public void rocketShoot()
    {
        spaceObject meteor = Utility.FindMeteor(true);
        Vector3 position = meteor.gameObject.transform.position;
        Vector3 vect = position - transform.position;
        float angle = Mathf.Atan2(vect.y, vect.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90);
        Song.Instance.playSound(Song.Instance.rocket_sound);
        shoot(angle, vect, 1);
    }
}
