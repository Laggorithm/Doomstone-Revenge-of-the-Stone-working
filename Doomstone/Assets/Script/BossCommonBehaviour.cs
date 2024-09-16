using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossCommonBehaviour : MonoBehaviour
{
    private List<int> AttackList = new List<int>();
    public Player player;
    public static int Hp = 5;
    public float moveSpeed = 5f;
    public float minTravelDistance = 5f;
    public float directionChangeInterval = 3f;
    public float normalJumpForce = 10f;
    public float doubleJumpFactor = 2f;
    public float projectileSpeed = 10f;
    public float projectileLaunchDelay = 0.5f;
    public float projectileLifetime = 3f;
    private float currentDistanceTraveled;
    public int MaxProjectiles;
    public int Projectilecd = 3;
    private AudioSource audioSource;
    public AudioSource BreakAudio;
    private Rigidbody2D rb;

    private Vector2 moveDirection;
    
    
    private bool isGrounded = true;
    private bool isHandlingAttack = false;
    public bool isPhaseTwo = false;
    private bool hasLaunchedProjectiles = false;

    public GameObject extraLeftProjectilePrefab;
    public GameObject extraRightProjectilePrefab;
    public Transform leftShootingPoint;
    public Transform rightShootingPoint;
    public Transform extraLeftShootingPoint;
    public Transform extraRightShootingPoint;
    public GameObject leftProjectilePrefab;
    public GameObject rightProjectilePrefab;
    public ParticleSystem Particles;

    public Sprite BossPhaseOne;
    public Sprite BossPhaseTwo;
    public Sprite BossPhaseDead;

    void Start()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "BossRoomTwo")
        {
            Hp = 30;
        }
        if (currentScene.name == "BossRoomOne")
        {
            Hp = 10;
        }

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        InitializePhaseOne();
        PrintAttackList();

        SetRandomDirection();
        StartCoroutine(ChangeDirectionPeriodically());
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        StartCoroutine(ShootExtraLeftProjectile());
        StartCoroutine(ShootExtraRightProjectile());

        spriteRenderer.sprite = BossPhaseOne;
    }

    void Update()
    {
        MoveBoss();
        if (!isHandlingAttack && isGrounded)
        {
            StartCoroutine(HandleJump());
        }

        if (Hp <= 3 && !isPhaseTwo)
        {
            EnterPhaseTwo();
            Scene currentScene = SceneManager.GetActiveScene();
            if (currentScene.name == "BossRoomTwo")
            {
                Hp = 20;
            }
            
        }

        if (Hp <= 0)
        {
            Particles.Play();
            GoDead();
            Player.Dmg = 2;
            
        }

        if (isGrounded && !hasLaunchedProjectiles)
        {
            StartCoroutine(LaunchProjectiles());
        }

        // Make extra shooting points face the player
        RotateShootingPoints();

         
            
         
    }

    void InitializePhaseOne()
    {
        Particles = GetComponent<ParticleSystem>();
        Particles.Play();
         
        
        AttackList.Clear();

        for (int i = 0; i < 4; i++)
        {
            AttackList.Add(0);
        }

        for (int i = 0; i < 6; i++)
        {
            AttackList.Add(1);
        }

        ShuffleList(AttackList);
    }

    void EnterPhaseTwo()
    {
        BreakAudio = GetComponent<AudioSource>();
        BreakAudio.Play();
        Particles = GetComponent<ParticleSystem>();
        Particles.Play();
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = BossPhaseTwo;
        isPhaseTwo = true;
        Hp = 10;

        AttackList.Clear();

        for (int i = 0; i < 6; i++)
        {
            AttackList.Add(0);
        }

        for (int i = 0; i < 9; i++)
        {
            AttackList.Add(1);
        }

        for (int i = 0; i < 5; i++)
        {
            AttackList.Add(2);
        }

        ShuffleList(AttackList);
        PrintAttackList();
    }
    
    void GoDead()
    {

        Particles = GetComponent<ParticleSystem>();
        
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        spriteRenderer.sprite = BossPhaseDead;
        
        extraLeftShootingPoint.gameObject.SetActive(false); 
        extraRightShootingPoint.gameObject.SetActive(false);

        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        moveSpeed = 0;
        Particles.Stop();

        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "BossRoomTwo")
        {

            Application.Quit();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif  
        }
    }

    void ShuffleList(List<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(0, list.Count);
            int temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    void PrintAttackList()
    {
        foreach (int attack in AttackList)
        {
            Debug.Log("Attack ID: " + attack);
        }
    }

    void MoveBoss()
    {
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
        currentDistanceTraveled += moveSpeed * Time.deltaTime;

        if (currentDistanceTraveled >= minTravelDistance)
        {
            SetRandomDirection();
            currentDistanceTraveled = 0f;
        }
    }

    void SetRandomDirection()
    {
        float xDirection = Random.value > 0.5f ? 1f : -1f;
        moveDirection = new Vector2(xDirection, 0f);
    }

    IEnumerator ChangeDirectionPeriodically()
    {
        while (true)
        {
            yield return new WaitForSeconds(directionChangeInterval);
            SetRandomDirection();
        }
    }

    IEnumerator HandleJump()
    {
        isHandlingAttack = true;

        while (AttackList.Count > 0 && isGrounded)
        {
            int attackId = AttackList[0];
            AttackList.RemoveAt(0);

            if (attackId != 0)
            {
                float jumpForce = attackId == 2 ? normalJumpForce * doubleJumpFactor : normalJumpForce;
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                isGrounded = false;
                audioSource = GetComponent<AudioSource>();
                audioSource.Play();
            }

            yield return new WaitForSeconds(3);
        }

        isHandlingAttack = false;
    }

    IEnumerator LaunchProjectiles()
    {
        hasLaunchedProjectiles = true;

        if (leftProjectilePrefab != null && leftShootingPoint != null)
        {
            GameObject leftProjectile = Instantiate(leftProjectilePrefab, leftShootingPoint.position, Quaternion.identity);
            leftProjectile.GetComponent<Rigidbody2D>().velocity = new Vector2(-projectileSpeed, 0f);
            Destroy(leftProjectile, projectileLifetime);
        }

        if (rightProjectilePrefab != null && rightShootingPoint != null)
        {
            GameObject rightProjectile = Instantiate(rightProjectilePrefab, rightShootingPoint.position, Quaternion.identity);
            rightProjectile.GetComponent<Rigidbody2D>().velocity = new Vector2(projectileSpeed, 0f);
            Destroy(rightProjectile, projectileLifetime);
        }

        

        yield return null;
    }

    IEnumerator ShootExtraLeftProjectile()
    {
        while (true)
        {
            if (extraLeftProjectilePrefab != null && extraLeftShootingPoint != null && isGrounded)
            {
                GameObject extraLeftProjectile = Instantiate(extraLeftProjectilePrefab, extraLeftShootingPoint.position, Quaternion.identity);
                Rigidbody2D rbProjectile = extraLeftProjectile.GetComponent<Rigidbody2D>();

                // Add homing behavior
                HomingProjectile homingScript = extraLeftProjectile.AddComponent<HomingProjectile>();
                homingScript.target = player.transform;  // Set the player as the target
                homingScript.speed = projectileSpeed;    // Set the speed of the projectile
                homingScript.rotationSpeed = 200f;       // Adjust rotation speed for homing behavior

                Destroy(extraLeftProjectile, projectileLifetime);
            }
            yield return new WaitForSeconds(Projectilecd);
        }
    }

    IEnumerator ShootExtraRightProjectile()
    {
        while (true)
        {
            if (extraRightProjectilePrefab != null && extraRightShootingPoint != null && isGrounded)
            {
                GameObject extraRightProjectile = Instantiate(extraRightProjectilePrefab, extraRightShootingPoint.position, Quaternion.identity);
                Rigidbody2D rbProjectile = extraRightProjectile.GetComponent<Rigidbody2D>();

                // Add homing behavior
                HomingProjectile homingScript = extraRightProjectile.AddComponent<HomingProjectile>();
                homingScript.target = player.transform;  // Set the player as the target
                homingScript.speed = projectileSpeed;    // Set the speed of the projectile
                homingScript.rotationSpeed = 200f;       // Adjust rotation speed for homing behavior

                Destroy(extraRightProjectile, projectileLifetime);
            }
            yield return new WaitForSeconds(Projectilecd);
        }
    }

    void RotateShootingPoints()
    {
        if (player != null)
        {
            Vector2 directionToPlayer = (player.transform.position - extraLeftShootingPoint.position).normalized;
            extraLeftShootingPoint.right = -directionToPlayer;

            directionToPlayer = (player.transform.position - extraRightShootingPoint.position).normalized;
            extraRightShootingPoint.right = directionToPlayer;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = true;
            hasLaunchedProjectiles = false;
            
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
