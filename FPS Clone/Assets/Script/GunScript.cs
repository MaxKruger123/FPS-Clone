using TMPro;
using System.Collections;
using UnityEngine.VFX;
using UnityEngine;

public class GunScript : MonoBehaviour
{
    public float damage = 10f;
    public float headShotDamage = 10f;
    public float range = 100f;
    public VisualEffect muzzleFlashVFX;
    public ParticleSystem impact;
    public Camera fpsCam;

    public int maxAmmo = 10;         // Max bullets in the current magazine
    private int currentAmmo;         // Current bullets in the magazine
    public int stashedAmmo = 50;     // Total stashed ammo available

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

    public float bulletForce;

    public Vector3 thisGun;

    public TextMeshProUGUI currentWeaponAmmo;
    public TextMeshProUGUI stashedWeaponAmmo;

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

        targetPosition = initialLocalPosition;
        currentPosition = initialLocalPosition;
        transform.localPosition = initialLocalPosition;
    }

    void OnEnable()
    {
        isReloading = false;
        animator.SetBool("isReloading", false);
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

        if (currentAmmo <= 0 && stashedAmmo > 0)
        {
            StartCoroutine(Reload());
            return;
        }

        if (isSemiAutomatic)
        {
            if (Input.GetButtonDown("Fire1") && Time.time >= nextTimeToFire)
            {
                nextTimeToFire = Time.time + fireRate;
                Shoot();
            }
        }
        else
        {
            if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
            {
                nextTimeToFire = Time.time + fireRate;
                Shoot();
            }
        }

        // Reload when 'R' is pressed and ammo in the magazine is not full
        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo && stashedAmmo > 0)
        {
            StartCoroutine(Reload());
            return;
        }

        currentWeaponAmmo.text = currentAmmo.ToString();
        stashedWeaponAmmo.text = stashedAmmo.ToString();

        if (currentAmmo <= 0 && stashedAmmo <= 0)
        {
            
            currentWeaponAmmo.text = "0";
            stashedWeaponAmmo.text = "0";
        }
        
    }

    void back()
    {
        targetPosition = Vector3.Lerp(targetPosition, initialLocalPosition, Time.deltaTime * returnAmount);
        currentPosition = Vector3.Lerp(currentPosition, targetPosition, Time.fixedDeltaTime * snappiness);
        transform.localPosition = currentPosition;
    }

    public void Recoil()
    {
        targetPosition -= new Vector3(0, 0, kickBackZ);
        targetRotation += new Vector3(recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));
    }

    void Shoot()
    {
        if (currentAmmo >= 0)
        {
            source.PlayOneShot(clip);
            currentAmmo--;

            if (muzzleFlashVFX != null)
            {
                muzzleFlashVFX.Play();
            }

            RaycastHit hit;
            Recoil();

            if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
            {
                Target target = hit.transform.GetComponentInParent<Target>();
                Zombie zombie = hit.transform.GetComponentInParent<Zombie>();

                if (hit.transform.gameObject.tag == "Zombie")
                {
                    ParticleSystem impactGO = Instantiate(impact, hit.point, Quaternion.LookRotation(hit.normal));
                    hs = false;
                    target.TakeDamage(damage);

                    if (zombie != null && target.currentHealth <= 0)
                    {
                        Vector3 forceDirection = zombie.transform.position - Camera.main.transform.position;
                        forceDirection.y = 1;
                        forceDirection.Normalize();
                        Vector3 force = bulletForce * forceDirection;
                        zombie.TriggerRagdoll(force, hit.point);
                    }

                    Destroy(impactGO.gameObject, 2f);
                }
                else if (hit.transform.gameObject.tag == "Zombie_Head")
                {
                    ParticleSystem impactGO = Instantiate(impact, hit.point, Quaternion.LookRotation(hit.normal));
                    hs = true;
                    target.TakeDamage(headShotDamage);

                    if (zombie != null && target.currentHealth <= 0)
                    {
                        Vector3 forceDirection = zombie.transform.position - Camera.main.transform.position;
                        forceDirection.y = 1;
                        forceDirection.Normalize();
                        Vector3 force = bulletForce * forceDirection;
                        zombie.TriggerRagdoll(force, hit.point);
                    }

                    Destroy(impactGO.gameObject, 2f);
                }
            }
        }
        
    }

    IEnumerator Reload()
    {
        isReloading = true;
        animator.SetBool("isReloading", true);
        Debug.Log("Reloading...");
        yield return new WaitForSeconds(reloadTime);

        // Calculate the number of bullets needed to fill the magazine
        int bulletsToReload = maxAmmo - currentAmmo;

        // Check if there are enough bullets in stashedAmmo
        if (stashedAmmo >= bulletsToReload)
        {
            currentAmmo += bulletsToReload;  // Reload full magazine
            stashedAmmo -= bulletsToReload;  // Decrease stashed ammo by that amount
        }
        else
        {
            currentAmmo += stashedAmmo;      // Reload the remaining stashed ammo
            stashedAmmo = 0;                 // No stashed ammo left
        }

        isReloading = false;
        animator.SetBool("isReloading", false);
    }
}
