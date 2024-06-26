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
    private bool isHitting = false;
    private float hittingTime = 0.25f;

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
        if (!isLive || anime.GetCurrentAnimatorStateInfo(0).IsName("Hit")) //hit일 때 넉백을 주기 위함.
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
        if (!collision.CompareTag("Bullet") || !isLive) return;

        Debug.Log(collision.name);

        health -= collision.GetComponent<Bullet>().damage;
        StartCoroutine(KnockBack());

        if (health > 0) {
            anime.SetTrigger("Hit");
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Hit);
        }
        else {
            isLive = false;
            cl.enabled = false;
            rigid.simulated = false;
            sr.sortingOrder = 0;
            anime.SetBool("Dead", true);
            GameManager.instance.kill++;
            GameManager.instance.GetExp();

            if (GameManager.instance.isLive) //Enemy Cleaner가 죽일 때는 소리가 나지 않도록
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Dead);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag("Bullet") || !isLive) return;
        if (isHitting) return;

        Debug.Log(collision.name);

        health -= collision.GetComponent<Bullet>().damage;
        StartCoroutine(KnockBack());

        if (health > 0) {
            anime.SetTrigger("Hit");
            isHitting = true;
            Invoke("ChangeHit", hittingTime);
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Hit);
        }
        else {
            isLive = false;
            cl.enabled = false;
            rigid.simulated = false;
            sr.sortingOrder = 0;
            anime.SetBool("Dead", true);
            GameManager.instance.kill++;
            GameManager.instance.GetExp();

            if (GameManager.instance.isLive) //Enemy Cleaner가 죽일 때는 소리가 나지 않도록
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Dead);
        }
    }

    private void ChangeHit()
    {
        isHitting = false;
    }

    private IEnumerator KnockBack()
    {   
        yield return wait;
        Vector3 playerPos = GameManager.instance.player.transform.position;
        Vector3 dirVec = transform.position - playerPos;
        rigid.AddForce(dirVec.normalized * 3f, ForceMode2D.Impulse); //넉백 크기 : 3
    }

    public void Dead() //Animation Event로 사용됨.
    {
        gameObject.SetActive(false);
    }
}
