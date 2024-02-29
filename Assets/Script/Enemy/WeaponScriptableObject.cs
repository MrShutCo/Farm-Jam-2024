using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public enum WeaponType { Shotgun, AssaultRifle, Grenade, GatlingGun, Pitchfork }


[CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Weapon", order = 0)]
public class WeaponScriptableObject : ScriptableObject
{
    public WeaponType Type;
    public string Name;
    public GameObject ModelPrefab;
    public GameObject BulletPrefab;
    public Vector2 SpawnPoint;
    public Vector2 SpawnRotation;
    public Vector3 bulletRotation;
    public bool rotateModelToTarget;

    public ShootConfigScriptableObject ShootConfig;
    public TrailConfigScriptableObject TrailConfig;

    private MonoBehaviour ActiveMonoBehaviour;
    private GameObject Model;
    private float LastShootTime;
    private ParticleSystem ShootSystem;
    private ObjectPool<TrailRenderer> TrailPool;
    private ObjectPool<GameObject> BulletPool;

    private GameObject ModelChild;


    public void WeaponModelActive(bool active)
    {
        if (Model != null)
            Model.SetActive(active);
    }


    public void Spawn(Transform Parent, MonoBehaviour ActiveMonoBehaviour)
    {
        this.ActiveMonoBehaviour = ActiveMonoBehaviour;
        LastShootTime = 0;
        TrailPool = new ObjectPool<TrailRenderer>(CreateTrail);

        if (BulletPrefab != null)
            BulletPool = new ObjectPool<GameObject>(() => Instantiate(BulletPrefab));

        Model = Instantiate(ModelPrefab);
        Model.transform.SetParent(Parent, false);
        Model.transform.localPosition = SpawnPoint;
        Model.transform.localRotation = Quaternion.Euler(SpawnRotation);

        ShootSystem = Model.GetComponentInChildren<ParticleSystem>();
        if (Model.transform.childCount > 0)
            ModelChild = Model.transform.GetChild(0).gameObject;
    }

    public void Shoot(Vector2 direction, float fireRateMultiplier = 0)
    {
        RaycastHit2D hit;
        if (Time.time > ShootConfig.FireRate - (ShootConfig.FireRate * fireRateMultiplier) + LastShootTime)
        {
            LastShootTime = Time.time;
            for (int i = 0; i < ShootConfig.BulletsPerShot; i++)
            {
                switch (Type)
                {
                    case WeaponType.Shotgun:
                        GameManager.Instance.onPlayHumanSound?.Invoke(ESoundType.humanShotgun, Model.transform.position);
                        break;
                    case WeaponType.AssaultRifle:
                        //GameManager.Instance.onPlayHumanSound?.Invoke(ESoundType.humanAssaultRifle, Model.transform.position);
                        break;
                    case WeaponType.GatlingGun:
                        //GameManager.Instance.onPlayHumanSound?.Invoke(ESoundType.humanGatlingGun, Model.transform.position);
                        break;
                    case WeaponType.Pitchfork:
                        //GameManager.Instance.onPlayHumanSound?.Invoke(ESoundType.humanPitchfork, Model.transform.position);
                        break;
                }
                ShootSystem.Play();
                Vector2 shootDirection = (Vector2)direction
                 + new Vector2(
                    UnityEngine.Random.Range(
                    -ShootConfig.Spread.x,
                    ShootConfig.Spread.x
                 ),
                    UnityEngine.Random.Range(
                    -ShootConfig.Spread.y,
                    ShootConfig.Spread.y
                 )
                 );
                shootDirection.Normalize();
                hit = Physics2D.Raycast(
                    ShootSystem.transform.position,
                    shootDirection,
                    float.MaxValue,
                    ShootConfig.HitMask
                );
                if (hit)
                {
                    ActiveMonoBehaviour.StartCoroutine(
                        PlayTrail(
                            ShootSystem.transform.position,
                            hit.point,
                            hit
                        )
                    );
                    if (hit.collider.TryGetComponent(out HealthBase health))
                    {
                        health.TakeDamage(ShootConfig.Damage);
                    }
                    if (ShootConfig.KnockbackForce != 0)
                    {
                        if (hit.collider.TryGetComponent(out Rigidbody2D rb))
                        {
                            //Addknockback
                        }
                    }
                }
                else
                {
                    ActiveMonoBehaviour.StartCoroutine(
                        PlayTrail(
                            ShootSystem.transform.position,
                            ShootSystem.transform.position + (Vector3)(shootDirection * TrailConfig.MissDistance),
                            new RaycastHit2D()
                        )
                    );
                }
            }
        }
    }
    public void Flip(Vector2 direction)
    {
        if (direction.x > 0)
        {
            // Rotate up or down towards the direction along the z axis
            if (ModelChild != null)
                ModelChild.transform.localScale = new Vector3(1, 1, 1);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.Euler(0, 0, angle);
            Model.transform.rotation = rotation;
        }
        else if (direction.x < 0)
        {
            // Flip to the left and rotate up and down towards the direction
            if (ModelChild != null)
                ModelChild.transform.localScale = new Vector3(1, -1, 1);
            Model.transform.localScale = new Vector3(1, 1, 1);
            float angle = Mathf.Atan2(-direction.y, direction.x) * Mathf.Rad2Deg;
            Model.transform.rotation = Quaternion.Euler(0, 0, -angle);
        }

    }

    private IEnumerator PlayTrail(Vector3 start, Vector3 end, RaycastHit2D hit)
    {
        TrailRenderer instance = TrailPool.Get();
        GameObject bullet = null;
        if (BulletPrefab != null)
        {
            bullet = BulletPool.Get();
            bullet.SetActive(true);
            bullet.transform.position = start;
            bullet.transform.rotation = Quaternion.LookRotation(Vector3.forward, end - start);
            bullet.transform.Rotate(bulletRotation);
        }

        instance.gameObject.SetActive(true);
        instance.transform.position = start;
        yield return null;

        instance.emitting = true;

        float distance = Vector3.Distance(start, end);
        float remainingDistance = distance;
        while (remainingDistance > 0)
        {
            instance.transform.position = Vector3.Lerp(
                start,
                end,
                Mathf.Clamp01(1 - (remainingDistance / distance))
            );

            if (bullet != null)
                bullet.transform.position = instance.transform.position;

            remainingDistance -= TrailConfig.SimulationSpeed * Time.deltaTime;

            yield return null;
        }

        instance.transform.position = end;
        if (bullet != null)
            bullet.transform.position = end;

        yield return new WaitForSeconds(TrailConfig.Duration);
        yield return null;
        instance.emitting = false;
        instance.gameObject.SetActive(false);
        TrailPool.Release(instance);
        if (bullet != null)
        {
            bullet.SetActive(false);
            BulletPool.Release(bullet);
        }
    }

    private TrailRenderer CreateTrail()
    {
        GameObject instance = new GameObject("BulletTrail");
        TrailRenderer trail = instance.AddComponent<TrailRenderer>();
        trail.colorGradient = TrailConfig.Color;
        trail.material = TrailConfig.Material;
        trail.widthCurve = TrailConfig.WidthCurve;
        trail.time = TrailConfig.Duration;
        trail.minVertexDistance = TrailConfig.MinVertexDistance;
        trail.emitting = false;
        trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        return trail;
    }

}
