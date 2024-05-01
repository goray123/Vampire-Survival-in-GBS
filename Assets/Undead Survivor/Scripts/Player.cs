using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Vector2 inputVec;
    public float speed;

    private Rigidbody2D rigid;
    private SpriteRenderer sr;
    private Animator anime;
    public Scanner scanner;
    

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anime = GetComponent<Animator>();
        scanner = GetComponent<Scanner>();  
    }

    private void Update() 
    {
        if (!GameManager.instance.isLive) return;
        inputVec = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    private void FixedUpdate()
    {
        if (!GameManager.instance.isLive) return;

        Vector2 nextVec = inputVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
    }

    private void LateUpdate()
    {
        if (!GameManager.instance.isLive) return;

        anime.SetFloat("Velocity", inputVec.magnitude); //magnitude : 절댓값과 같은 느낌
        if (inputVec.x != 0)
            sr.flipX = inputVec.x < 0;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!GameManager.instance.isLive) return;

        GameManager.instance.health -= Time.deltaTime * 10; //10 : 초당 데미지, 몬스터 별로 데미지 다르게 해도 괜찮을듯
        if (GameManager.instance.health <= 0) {
            for (int index = 2; index < transform.childCount; index++) {
                transform.GetChild(index).gameObject.SetActive(false);
            }

            anime.SetTrigger("Dead");
            GameManager.instance.GameOver();
        }
    }
}
