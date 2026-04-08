using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Thông số máu")]
    public float health = 50f;

    [Header("Thông số di chuyển")]
    public float moveSpeed = 2f;
    public float chaseSpeed = 4f;

    [Header("Tuần tra đường thẳng")]
    public float patrolDistance = 5f; // Quái đi sang trái/phải bao nhiêu mét
    private Vector2 leftPoint;
    private Vector2 rightPoint;
    private bool movingRight = true;
    private Vector2 startPosition;

    [Header("Phạm vi nhận diện")]
    public float chaseRange = 6f;    // Tầm nhìn thấy người chơi
    public float stopRange = 10f;    // Tầm mất dấu người chơi
    public float attackRange = 1.2f; // Khoảng cách dừng lại để đánh

    [Header("Tấn công")]
    public float attackRate = 1.5f;
    private float nextAttackTime = 0f;

    private Transform player;
    private bool isChasing;
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        startPosition = transform.position;

        // Thiết lập 2 điểm mốc trái và phải dựa trên vị trí đặt quái ban đầu
        leftPoint = new Vector2(startPosition.x - patrolDistance, startPosition.y);
        rightPoint = new Vector2(startPosition.x + patrolDistance, startPosition.y);

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // 1. Kiểm tra trạng thái: Đuổi theo hay Tuần tra
        if (distanceToPlayer < chaseRange)
        {
            isChasing = true;
        }
        else if (distanceToPlayer > stopRange)
        {
            isChasing = false;
        }

        // 2. Thực thi hành động
        if (isChasing)
        {
            if (distanceToPlayer <= attackRange)
            {
                StopMoving();
                TryAttack();
            }
            else
            {
                MoveTowards(player.position, chaseSpeed);
            }
        }
        else
        {
            LinearPatrol();
        }
    }

    void LinearPatrol()
    {
        // Chọn điểm đích là bên phải hoặc bên trái
        Vector2 target = movingRight ? rightPoint : leftPoint;

        MoveTowards(target, moveSpeed);

        // Nếu chạm điểm đích thì đổi hướng cho lần sau
        if (Vector2.Distance(transform.position, target) < 0.2f)
        {
            movingRight = !movingRight;
        }
    }

    void MoveTowards(Vector2 target, float speed)
    {
        float step = speed * Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(target.x, transform.position.y), step);

        // KIỂM TRA QUÁI CÓ ĐANG DI CHUYỂN KHÔNG
        if (anim != null)
        {
            anim.SetBool("isWalking", true); // Bật animation đi bộ
        }

        // Lật mặt quái
        if (target.x > transform.position.x) transform.localScale = new Vector3(1, 1, 1);
        else if (target.x < transform.position.x) transform.localScale = new Vector3(-1, 1, 1);
    }

    void StopMoving()
    {
        if (anim != null)
        {
            anim.SetBool("isWalking", false); // Tắt animation đi bộ, quay về Idle
        }
    }

    void TryAttack()
    {
        if (Time.time >= nextAttackTime)
        {
            if (anim != null) anim.SetTrigger("attack");

            // Nếu bạn dùng va chạm (Collision) trong PlayerHealth thì không cần dòng sát thương ở đây.
            // Nếu không dùng va chạm, hãy mở dòng dưới:
            // player.GetComponent<PlayerHealth>().TakeDamage(10);

            nextAttackTime = Time.time + attackRate;
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        // Nếu có animation trúng đòn (Hurt) thì gọi ở đây:
        // if (anim != null) anim.SetTrigger("hurt");

        if (health <= 0) Die();
    }

    void Die()
    {
        // Nếu có animation chết, hãy đợi nó chạy xong rồi mới Destroy
        Destroy(gameObject);
    }

    // Vẽ đường tuần tra trong Scene để dễ căn chỉnh
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            Gizmos.color = Color.blue;
            Vector2 p1 = new Vector2(transform.position.x - patrolDistance, transform.position.y);
            Vector2 p2 = new Vector2(transform.position.x + patrolDistance, transform.position.y);
            Gizmos.DrawLine(p1, p2);
            Gizmos.DrawSphere(p1, 0.1f);
            Gizmos.DrawSphere(p2, 0.1f);
        }
    }
}