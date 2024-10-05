using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour
{
    public float damage = 10f;
    public float range = 100f;
    public ParticleSystem muzzleFlash;
    public ParticleSystem impact;
    public Camera fpsCam;
    public int maxAmmo = 10;
    private int currentAmmo;
    public float reloadTime = 1f;
    private bool isReloading = false;
    public Animator animator;

    public float fireRate = 0.5f;  // Time between shots in seconds
    private float nextTimeToFire = 0f;
    public bool isSemiAutomatic = true;  // Set to true for semi-automatic, false for automatic

    private void Start()
    {
        currentAmmo = maxAmmo;
        animator = GameObject.Find("WeaponHolder").GetComponent<Animator>();
    }

    void OnEnable()
    {
        isReloading = false;
        animator.SetBool("isReloading", false);
    }

    void Update()
    {
        if (isReloading)
        {
            return;
        }

        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        // Check for semi-automatic or automatic firing
        if (isSemiAutomatic)
        {
            // Semi-automatic: Fires one shot per click
            if (Input.GetButtonDown("Fire1") && Time.time >= nextTimeToFire)
            {
                nextTimeToFire = Time.time + fireRate;
                Shoot();
            }
        }
        else
        {
            // Automatic: Continuously fires while holding down the fire button
            if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
            {
                nextTimeToFire = Time.time + fireRate;
                Shoot();
            }
        }

        // Check for reload input
        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo)
        {

            StartCoroutine(Reload());
            return;
        }
    }

    void Shoot()
    {
        // Decrease ammo
        currentAmmo--;

        // Play muzzle flash effect
        muzzleFlash.Play();

        // Create a RaycastHit variable to store information about the object hit by the ray
        RaycastHit hit;

        // Cast a ray from the camera's position, in the direction the camera is looking
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name);  // Log the name of the object hit

            // Check if the object hit has a 'Target' component and apply damage if so
            Target target = hit.transform.GetComponentInParent<Target>();
            if (target != null)
            {
                target.TakeDamage(damage);  // Apply damage to the target
            }

            if (hit.transform.gameObject.tag == "Zombie")
            {
                // Instantiate the impact effect at the hit point, facing the hit surface's normal
                ParticleSystem impactGO = Instantiate(impact, hit.point, Quaternion.LookRotation(hit.normal));

                // Destroy the impact effect after 2 seconds
                Destroy(impactGO.gameObject, 2f);
            }
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;
        animator.SetBool("isReloading", true);
        Debug.Log("Reloading...");
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        isReloading = false;
        animator.SetBool("isReloading", false);
    }
}
