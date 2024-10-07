using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Limb : MonoBehaviour
{
    [SerializeField] Limb[] childLimbs;

    [SerializeField] GameObject limbPerfab;
    [SerializeField] GameObject woundHole;

    [SerializeField] GameObject bloodPrefab;
    private bool limbShot;
    
    // Start is called before the first frame update
    void Start()
    {
        if (woundHole != null)
        {

        woundHole.SetActive(false); 
        
        }

        if (bloodPrefab != null)
        {
            bloodPrefab.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetHit()
    {
        
            if (childLimbs.Length >= 0)
            {
                foreach (Limb limb in childLimbs)
                {
                    if (limb != null)
                    {
                        limb.GetHit();
                    }
                }
            }

            

            if (woundHole != null)
            {
                woundHole.SetActive(true);

                if (bloodPrefab != null)
                {
                    bloodPrefab.SetActive(true);
                }
            }

            if (limbPerfab != null )
            {
                Instantiate(limbPerfab, transform.position, transform.rotation);
                

                

            }

            

            transform.localScale = Vector3.zero;
            
            


            Destroy(this);

    }

    

    
}
