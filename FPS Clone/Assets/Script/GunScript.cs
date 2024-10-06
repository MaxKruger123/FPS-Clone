using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour
{
    public float damage = 10f;
    public float headShotDamage = 10f;
    public float range = 100f;
    public ParticleSystem muzzleFlash;
    public ParticleSystem impact;
    public Camera fpsCam;
    public int maxAmmo = 10;
    private int currentAmmo;
    public float reloadTime = 1f;
    private bool isReloading = false;
    public Animator animator;

    public float fireRate = 0.5f;
    private float nextTimeToFire = 0f;
    public bool isSemiAutomatic = true;

    public AudioSource source;
    public AudioClip clip;
    public bool hs;

    public float kickBackZ;
    public float recoilX;
    public float recoilY;
    public float recoilZ;
    public float snappiness, returnAmount;

    public GameObject pointLight;

    // Expose this variable so you can manually set the starting position in the Inspector
    public Vector3 initialLocalPosition;

    Vector3 targetRotation, currentRotation, targetPosition, currentPosition;

    private void Start()
    {
        currentAmmo = maxAmmo;
        animator = GameObject.Find("WeaponHolder").GetComponent<Animator>();

        // If you haven’t set a value in the Inspector, fall back to the current local position
        if (initialLocalPosition == Vector3.zero)
        {
            initialLocalPosition = transform.localPosition;
        }

        // Set the initial local position for the gun
        targetPosition = initialLocalPosition;
        currentPosition = initialLocalPosition;
        transform.localPosition = initialLocalPosition;



    }

    void OnEnable()
    {
        isReloading = false;
        animator.SetBool("isReloading", false);

        // Reset the gun's position when it’s enabled
        targetPosition = initialLocalPosition;
        currentPosition = initialLocalPosition;
        transform.localPosition = initialLocalPosition;

    }

    void Update()
    {
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, Time.deltaTime * returnAmount);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, Time.fixedDeltaTime * snappiness);
        transform.localRotation = Quaternion.Euler(currentRotation);
        back();

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

    void back()
    {
        targetPosition = Vector3.Lerp(targetPosition, initialLocalPosition, Time.deltaTime * returnAmount);
        currentPosition =  Vector3.Lerp(currentPosition,targetPosition, Time.fixedDeltaTime * snappiness);
        transform.localPosition = currentPosition;
    }

    public void Recoil()
    {
        targetPosition -= new Vector3(0, 0, kickBackZ);
        targetRotation += new Vector3(recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));
    }

    void Shoot()
    {
        source.PlayOneShot(clip);
        // Decrease ammo
        currentAmmo--;

        // Play muzzle flash effect
        muzzleFlash.Play();

        // Create a RaycastHit variable to store information about the object hit by the ray
        RaycastHit hit;

        //Add Random Reocil/Gun Kickback
        Recoil();

        // Cast a ray from the camera's position, in the direction the camera is looking
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            

            // Check if the object hit has a 'Target' component and apply damage if so
            Target target = hit.transform.GetComponentInParent<Target>();
            

            if (hit.transform.gameObject.tag == "Zombie")
            {
                // Instantiate the impact effect at the hit point, facing the hit surface's normal
                ParticleSystem impactGO = Instantiate(impact, hit.point, Quaternion.LookRotation(hit.normal));
                hs = false;
                target.TakeDamage(damage);
                // Destroy the impact effect after 2 seconds
                Destroy(impactGO.gameObject, 2f);
                
            } else if (hit.transform.gameObject.tag == "Zombie_Head")
            {
                ParticleSystem impactGO = Instantiate(impact, hit.point, Quaternion.LookRotation(hit.normal));
                hs = true; 
                target.TakeDamage(headShotDamage);
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
