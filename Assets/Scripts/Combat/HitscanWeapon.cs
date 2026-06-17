using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class HitscanWeapon : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform muzzle;
    [SerializeField] private Transform playerCamera;
    [SerializeField] private InputActionAsset inputActions;

    [Header("Stats")]
    [SerializeField] private float range = 100f;
    [SerializeField] private float damage = 20f;
    [SerializeField] private float cooldown = 0.2f;
    [SerializeField] private LayerMask layerMask = -1;

    [Header("VFX")]
    [SerializeField] private Color tracerColor = new Color(1f, 0.85f, 0.2f, 1f);
    [SerializeField] private float tracerDuration = 0.08f;
    [SerializeField] private float muzzleFlashDuration = 0.06f;

    private InputAction attackAction;
    private float lastFireTime;
    private PlayerHealth playerHealth;
    private LineRenderer tracerLine;
    private GameObject muzzleFlash;
    private Coroutine tracerRoutine;
    private Material tracerMaterial;
    private Material muzzleFlashMaterial;
    private Material hitSparkMaterial;

    private void Awake()
    {
        if (playerCamera == null)
        {
            var cam = GetComponentInChildren<Camera>();
            if (cam != null) playerCamera = cam.transform;
        }

        var asset = inputActions != null ? inputActions : Resources.Load<InputActionAsset>("InputSystem_Actions");
        attackAction = asset.FindActionMap("Player").FindAction("Attack");
        playerHealth = GetComponentInParent<PlayerHealth>();
        CacheMaterials();
        SetupVfx();
    }

    private void CacheMaterials()
    {
        tracerMaterial = new Material(Shader.Find("Sprites/Default"));
        tracerMaterial.color = tracerColor;

        muzzleFlashMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        muzzleFlashMaterial.color = new Color(1f, 0.9f, 0.3f, 1f);
        muzzleFlashMaterial.EnableKeyword("_EMISSION");
        muzzleFlashMaterial.SetColor("_EmissionColor", new Color(2f, 1.6f, 0.4f));

        hitSparkMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        hitSparkMaterial.color = Color.red;
    }

    private void SetupVfx()
    {
        var tracerGo = new GameObject("ShotTracer");
        tracerGo.transform.SetParent(transform, false);
        tracerLine = tracerGo.AddComponent<LineRenderer>();
        tracerLine.positionCount = 2;
        tracerLine.startWidth = 0.06f;
        tracerLine.endWidth = 0.02f;
        tracerLine.material = tracerMaterial;
        tracerLine.startColor = tracerColor;
        tracerLine.endColor = tracerColor;
        tracerLine.enabled = false;
        tracerLine.useWorldSpace = true;

        muzzleFlash = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        muzzleFlash.name = "MuzzleFlash";
        muzzleFlash.transform.SetParent(muzzle != null ? muzzle : transform, false);
        muzzleFlash.transform.localPosition = Vector3.zero;
        muzzleFlash.transform.localScale = Vector3.one * 0.15f;
        var col = muzzleFlash.GetComponent<Collider>();
        if (col != null) Destroy(col);
        var renderer = muzzleFlash.GetComponent<Renderer>();
        if (renderer != null) renderer.material = muzzleFlashMaterial;
        muzzleFlash.SetActive(false);
    }

    private void OnEnable()
    {
        attackAction?.Enable();
    }

    private void OnDisable()
    {
        attackAction?.Disable();
    }

    private void Update()
    {
        if (attackAction == null || muzzle == null || playerCamera == null) return;
        if (playerHealth != null && playerHealth.IsDead) return;
        if (Time.time < lastFireTime + cooldown) return;

        if (attackAction.IsPressed())
            Fire();
    }

    private void Fire()
    {
        lastFireTime = Time.time;

        Vector3 origin = playerCamera.position;
        Vector3 direction = playerCamera.forward;
        Vector3 end = origin + direction * range;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, range, layerMask))
        {
            end = hit.point;
            var damageable = hit.collider.GetComponentInParent<IDamageable>();
            damageable?.TakeDamage(damage);
            SpawnHitSpark(hit.point);
        }

        ShowShotFx(muzzle.position, end);
    }

    private void ShowShotFx(Vector3 start, Vector3 end)
    {
        if (tracerRoutine != null)
            StopCoroutine(tracerRoutine);

        tracerLine.enabled = true;
        tracerLine.SetPosition(0, start);
        tracerLine.SetPosition(1, end);
        tracerRoutine = StartCoroutine(HideTracerAfterDelay());

        if (muzzleFlash != null)
            StartCoroutine(FlashMuzzle());
    }

    private IEnumerator HideTracerAfterDelay()
    {
        yield return new WaitForSeconds(tracerDuration);
        tracerLine.enabled = false;
        tracerRoutine = null;
    }

    private IEnumerator FlashMuzzle()
    {
        muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(muzzleFlashDuration);
        muzzleFlash.SetActive(false);
    }

    private void SpawnHitSpark(Vector3 point)
    {
        var spark = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        spark.name = "HitSpark";
        spark.transform.position = point;
        spark.transform.localScale = Vector3.one * 0.08f;
        var col = spark.GetComponent<Collider>();
        if (col != null) Destroy(col);
        var renderer = spark.GetComponent<Renderer>();
        if (renderer != null) renderer.material = hitSparkMaterial;
        Destroy(spark, 0.05f);
    }
}