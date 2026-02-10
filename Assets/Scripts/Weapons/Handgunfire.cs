using System.Collections;
using UnityEngine;

public class Handgunfire : MonoBehaviour
{
    [SerializeField] AudioSource gunFire;
    [SerializeField] GameObject handgun;
    [SerializeField] bool canFire = true;
    [SerializeField] GameObject extraCross;

    [SerializeField] AudioSource emptyGunSound;

    // Bullet settings
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform bulletSpawnPoint;
    [SerializeField] float bulletSpeed = 2000f;

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (canFire)
            {
                if (GlobalAmmo.handgunAmmoCount == 0)
                {
                    canFire = false;
                    StartCoroutine(EmptyGun());
                }
                else
                {
                    canFire = false;
                    StartCoroutine(FiringGun());
                }
            }
        }
    }

    IEnumerator EmptyGun()
{
    emptyGunSound.Play();
    yield return new WaitForSeconds(0.6f);
    canFire = true;
}


    IEnumerator FiringGun()
{
    gunFire.Play();
    extraCross.SetActive(true);

    // Ammo usage
    GlobalAmmo.handgunAmmoCount -= 1;

    // Play animation
    handgun.GetComponent<Animator>().Play("HandgunFire");

    // Spawn bullet
    GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
    Rigidbody rb = bullet.GetComponent<Rigidbody>();
    rb.useGravity = false;

    // Raycast from crosshair
    Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
    Vector3 targetPoint;

    if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
    {
        targetPoint = hit.point;
    }
    else
    {
        targetPoint = ray.GetPoint(1000f);
    }

    Vector3 direction = (targetPoint - bulletSpawnPoint.position).normalized;
    rb.velocity = direction * bulletSpeed;

    yield return new WaitForSeconds(0.5f);

    handgun.GetComponent<Animator>().Play("New State");
    extraCross.SetActive(false);

    yield return new WaitForSeconds(0.1f);
    canFire = true;
}
}