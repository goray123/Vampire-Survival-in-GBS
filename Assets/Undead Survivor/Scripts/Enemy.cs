using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed;
    public float health;
    public float maxHealth;
    public RuntimeAnimatorController[] animeCon;
    private bool isLive;

    public Rigidbody2D target;

    private Rigidbody2D rigid;
    private Animator anime;
    private SpriteRenderer sr;
    private Collider2D cl;

    private WaitForFixedUpdate wait;

    private void Awake() 
    {
        rigid = GetComponent<Rigidbody2D>();
        anime = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        cl = GetComponent<Collider2D>();

        wait = new WaitForFixedUpdate();
    }

    private void OnEnable()
    {
        target = GameManager.instance.player.GetComponent<Rigidbody2D>();
        isLive = true;
        cl.enabled = true;
        rigid.simulated = true;
        sr.sortingOrder = 3;
        anime.SetBool("Dead", false);
        health = maxHealth;
    }

    public void Init(SpawnData data)
    {
        anime.runtimeAnimatorController = animeCon[data.spriteType];
        speed = data.speed;
        maxHealth = data.health;
        health = maxHealth;
    }

    private void FixedUpdate() 
    {
        if (!GameManager.instance.isLive) return;
        if (!isLive || anime.GetCurrentAnimatorStateInfo(0).IsName("Hit")) 
            return;

        Vector2 dirVec = target.position - rigid.position;
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
        rigid.velocity = Vector2.zero;
    }

    private void LateUpdate() 
    {
        if (!GameManager.instance.isLive) return;
        if (isLive)
            sr.flipX = target.position.x < rigid.position.x;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Bullet") || !isLive)
            return;

        health -= collision.GetComponent<Bullet>().damage;
        StartCoroutine(KnockBack());

        if (health > 0) {
            anime.SetTrigger("Hit");
        }
        else {
            isLive = false;
            cl.enabled = false;
            rigid.simulated = false;
            sr.sortingOrder = 0;
            anime.SetBool("Dead", true);
            GameManager.instance.kill++;
            GameManager.instance.GetExp();
        }
    }

    private IEnumerator KnockBack()
    {   
        yield return wait;
        Vector3 playerPos = GameManager.instance.player.transform.position;
        Vector3 dirVec = transform.position - playerPos;
        rigid.AddForce(dirVec.normalized * 3f, ForceMode2D.Impulse); //³Ë¹é Å©±â : 3
    }

    public void Dead()
    {
        gameObject.SetActive(false);
    }
}
