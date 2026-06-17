using UnityEngine;

// Stage 1: MoveTowards 직선 추적. Stage 2에서 NavMeshAgent로 교체 예정.
public class ZombieAI : MonoBehaviour
{
    [SerializeField] private float speed = 2.5f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private int attackDamage = 15;

    private Transform player;
    private PlayerHealth playerHealth;
    private float lastAttackTime;

    private void Awake()
    {
        SetupVisual();
    }

    private void Start()
    {
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerHealth = playerObj.GetComponent<PlayerHealth>();
        }
    }

    private void SetupVisual()
    {
        if (transform.Find("Head") != null) return;

        var bodyRenderer = GetComponent<Renderer>();
        if (bodyRenderer != null)
            bodyRenderer.material.color = new Color(0.2f, 0.55f, 0.2f);

        var head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        head.name = "Head";
        head.transform.SetParent(transform, false);
        head.transform.localPosition = new Vector3(0f, 0.85f, 0f);
        head.transform.localScale = Vector3.one * 0.35f;

        var headCol = head.GetComponent<Collider>();
        if (headCol != null) Destroy(headCol);

        var headRenderer = head.GetComponent<Renderer>();
        if (headRenderer != null)
            headRenderer.material.color = new Color(0.85f, 0.25f, 0.15f);
    }

    private void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance < attackRange)
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                lastAttackTime = Time.time;
                playerHealth?.TakeDamage(attackDamage);
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
            transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
        }
    }
}