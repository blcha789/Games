using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterShooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform shotPos;
    public AudioSource gunSound;
    public List<AmmoList> ammoList =  new List<AmmoList>();

    public float[] shotgunPattern;

    private int pickedAmmo = 0;
    private int currentAmmoAmount = 0;

    private float nextFire;

    private AmmoBoxSpawner AmmoBoxSpawner;
    private CircleJoystick circleJoystick;

    private void Start()
    {
        AmmoBoxSpawner = GameObject.FindGameObjectWithTag("GameLogic").GetComponent<AmmoBoxSpawner>();
        circleJoystick = GameObject.FindGameObjectWithTag("RotateJoystick").GetComponent<CircleJoystick>();
    }

    private void Update()
    {
        Reload();
        Shoot();
    }

    public void ResetShooting()
    {
        circleJoystick.isPressed = false;
        ResetAmmo();
    }

    private void Shoot()
    {
        if(circleJoystick.isPressed && Time.time > nextFire)
        {
            if (ammoList[pickedAmmo].typeOfFire == TypeOfFire.Normal)
            {
                nextFire = Time.time + ammoList[pickedAmmo].fireRate;
                GameObject bullet = Instantiate(bulletPrefab, shotPos.position, Quaternion.identity);

                bullet.GetComponent<BulletStats>().damage = ammoList[pickedAmmo].damage;
                bullet.GetComponent<MeshRenderer>().material.color = ammoList[pickedAmmo].color;
                bullet.GetComponent<Rigidbody>().AddForce(shotPos.transform.forward * ammoList[pickedAmmo].fireForce);

                currentAmmoAmount--;
                gunSound.Play();
            }
            else if(ammoList[pickedAmmo].typeOfFire == TypeOfFire.Shotgun)
            {
                nextFire = Time.time + ammoList[pickedAmmo].fireRate;
                for (int i = 0; i < 3; i++)
                {
                    GameObject bullet = Instantiate(bulletPrefab, shotPos.position, Quaternion.identity);

                    bullet.GetComponent<BulletStats>().damage = ammoList[pickedAmmo].damage;
                    bullet.GetComponent<MeshRenderer>().material.color = ammoList[pickedAmmo].color;
                    bullet.GetComponent<Rigidbody>().AddForce((shotPos.transform.forward + shotPos.transform.right * shotgunPattern[i]) * ammoList[pickedAmmo].fireForce);
                }
                currentAmmoAmount--;
                gunSound.Play();
            }
        }
    }  

    private void Reload()
    {
        if(pickedAmmo > 0)
        {
            if(currentAmmoAmount <= 0)
            {
                pickedAmmo = 0;
                currentAmmoAmount = ammoList[0].pickAmmoAmount;
            }
        }
    }

    private void ResetAmmo()
    {
        pickedAmmo = 0;
        currentAmmoAmount = 0;
    }

    private void OnTriggerEnter(Collider col)
    {
        if(col.CompareTag("AmmoBox"))
        {
            string[] name = col.name.Split('_');
            pickedAmmo = int.Parse(name[1]);
            currentAmmoAmount = ammoList[pickedAmmo].pickAmmoAmount;
            AmmoBoxSpawner.DecreaseAmmoOnGround();

            Destroy(col.gameObject);
        }
    }
}
