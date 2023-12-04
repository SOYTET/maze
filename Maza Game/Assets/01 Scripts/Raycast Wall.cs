using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class RaycastWall : NetworkBehaviour
{
    public float range = 40f;
    public float damage = 10f;

    public float ImpactForce = 30f;
    public float FireRate = 15f;

    public Camera fpsCamera;
    public ParticleSystem particleFlashShooting;
    public ParticleSystem particleFlashShooting2;
    public GameObject ImpactEffect;
    public Animator animator;
    public GameObject shootingAudio;


    public bool isOnline;
    private float nextFire = 0;

    private AudioSource shootingAudioSource;

    private void Start()
    {
        particleFlashShooting.Stop();
        particleFlashShooting2.Stop();
        shootingAudioSource = shootingAudio.GetComponent<AudioSource>();
    }

    void Update()
    {
        if (isOnline)
        {
            if (!IsOwner) return;
            if (Input.GetMouseButton(0) && Time.time >= nextFire)
            {
                nextFire = Time.time + 1f / FireRate;
                ShootingServerRpc();
            }
            else
            {
                StopShooting();
            }
        }
        else
        {
            if (Input.GetMouseButton(0) && Time.time >= nextFire)
            {
                nextFire = Time.time + 1f / FireRate;
                shoot();
            }
            else
            {
                StopShooting();
            }
        }
    }

    void StopShooting()
    {
        if (shootingAudioSource.isPlaying)
        {
            StartCoroutine(StopAudio());
        }
    }

    IEnumerator StopAudio()
    {
        //yield return new WaitForSeconds(shootingAudioSource.clip.length);
        yield return new WaitForSeconds(0.05f);
        shootingAudio.SetActive(false);
    }

    [ServerRpc]
    void ShootingServerRpc()
    {
        shootClientRpc();
    }

    [ClientRpc]
    void shootClientRpc()
    {
        shootingAudio.SetActive(true);
        if (!IsOwner) return;
        Fire();

    }

    void shoot()
    {
        shootingAudio.SetActive(true);
        Fire();
    }

    void Fire()
    {
        if (Physics.Raycast(fpsCamera.transform.position, fpsCamera.transform.forward, out RaycastHit hitInfo, range))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hitInfo.distance, Color.green);
            Debug.Log("Hit something at " + hitInfo.transform.position + " Distance: " + hitInfo.distance);

            TargetShooting Target = hitInfo.transform.GetComponent<TargetShooting>();
            if (Target != null)
            {
                Target.TakeDemage(damage);
            }
            if (hitInfo.rigidbody != null)
            {
                hitInfo.rigidbody.AddForce(-hitInfo.normal * ImpactForce);
            }
            GameObject BulletEffect = Instantiate(ImpactEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
            Destroy(BulletEffect, 1.5f);
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 20f, Color.red);
            Debug.Log("Nothing hit");
        }
    }
}
