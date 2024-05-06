using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class AIGunController : MonoBehaviour
{
    [SerializeField] private Gun gun;   // 현재 총
    [SerializeField] private GameObject enemy;

    [Header("착용하고 있는 몬스터")]
    [SerializeField]
    private GameObject Monster;

    private Monster monsterSightScript;

    private float currentFireRate;  // 연사 속도 계산

    private AudioSource audioSource;    // 효과음

    private RaycastHit hit; // 레이저 충돌 정보

    public Transform laserStartPos;
    private float laserDuration = 0.05f;
    private LineRenderer laserLine;
    private Vector3 targetPos;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        laserLine = GetComponent<LineRenderer>();

        monsterSightScript = Monster.GetComponent<Monster>();
    }

    private void Update()
    {
        GunFireRateCalc();
        //Shoot();

        if(monsterSightScript.monsterSight.target != null)
        {
            targetPos = monsterSightScript.monsterSight.target.position;
        }

        Debug.DrawRay(enemy.transform.position, (targetPos - enemy.transform.position + new Vector3(0,1,0)).normalized, Color.red);
    }

    // 연사속도 재계산
    private void GunFireRateCalc()
    {
        if (currentFireRate > 0)
        {
            currentFireRate -= Time.deltaTime;
        }
    }

    // 발사 후 계산
    public void Shoot()
    {
        if(currentFireRate <= 0)
        {
            gun.muzzleFlash.Play();
            PlaySE(gun.fire_Sound);
            currentFireRate = gun.fireRate; //연사속도 재계산
            laserLine.SetPosition(0, laserStartPos.position);
            Hit();
        }
    }

    private void Hit()
    {
        float forwardX = Random.Range(-gun.accuracy, gun.accuracy);
        float forwardY = Random.Range(-gun.accuracy, gun.accuracy);
        if (Physics.Raycast(enemy.transform.position, (targetPos - enemy.transform.position + new Vector3(0, 1, 0)).normalized +
            new Vector3(forwardX, forwardY, 0f), out hit, gun.range))
        {
            laserLine.SetPosition(1, hit.point);
            Debug.Log(hit.transform.name);
            if(hit.transform.CompareTag("Player"))
            { 
                //player 체력 감소
            }
        }
        else
        {
            laserLine.SetPosition(1, (enemy.transform.forward + new Vector3(forwardX, forwardY, 0)) * gun.range);
        }
        StartCoroutine(ShootLaser());
    }

    private IEnumerator ShootLaser()
    {
        laserLine.enabled = true;
        yield return new WaitForSeconds(laserDuration);
        laserLine.enabled = false;
    }

    private void PlaySE(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }
}
