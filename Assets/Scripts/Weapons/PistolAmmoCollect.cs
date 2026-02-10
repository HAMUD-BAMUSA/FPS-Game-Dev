using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolAmmoCollect : MonoBehaviour
{
    [SerializeField] AudioSource ammoCollect;

    // NEW: time before ammo can be collected again
    [SerializeField] float respawnTime = 10f;

    private BoxCollider boxCollider;
    private MeshRenderer meshRenderer;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        this.gameObject.GetComponent<BoxCollider>().enabled = false; // prevent collecting more than 10
        ammoCollect.Play();
        GlobalAmmo.handgunAmmoCount += 20;

        // NEW: hide the ammo pickup instead of destroying it
        meshRenderer.enabled = false;

        // NEW: start respawn timer
        StartCoroutine(RespawnAmmo());
        //Destroy(gameObject);
    }

    // NEW: respawn ammo after a few seconds so it can be collected again
    IEnumerator RespawnAmmo()
    {
        yield return new WaitForSeconds(respawnTime);

        // re-enable visuals and collider
        meshRenderer.enabled = true;
        boxCollider.enabled = true;
    }
}
