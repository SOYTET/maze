using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RaycastWall : MonoBehaviour
{
    public float range = 40f;
    public float demage = 10f;

    public float ImpactForce = 30f;
    public float FireRate = 15f;

    public Camera fpsCamera;
    public ParticleSystem particleFlashShooting;
    public GameObject ImpactEffect;


    private float nextFire = 0;
    // Update is called once per frame
    private void Start()
    {
        particleFlashShooting.Stop();
    }
    void Update()
    {
        if (Input.GetMouseButton(0) && Time.time >= nextFire)
        {
            nextFire = Time.time + 1f / FireRate;
            shoot();    
        }
    }
    void shoot () {
        WFX_LightFlicker.isShoot = true;
        particleFlashShooting.Play();
        if (Physics.Raycast(fpsCamera.transform.position, fpsCamera.transform.forward, out RaycastHit hitInfo, range))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hitInfo.distance, Color.green);
            Debug.Log("hit something" + hitInfo.transform.position + hitInfo.transform + hitInfo.distance);

            TargetShooting Target = hitInfo.transform.GetComponent<TargetShooting>();
            if(Target != null)
            {
                Target.TakeDemage(demage);
            }
            if(hitInfo.rigidbody != null)
            {
                hitInfo.rigidbody.AddForce(-hitInfo.normal * ImpactForce);
            }
            GameObject BulletEffect =  Instantiate(ImpactEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
            Destroy(BulletEffect, 3f);
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 20f, Color.red);
            Debug.Log("nothing hit");
        }
    }

}
