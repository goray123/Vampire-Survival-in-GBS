using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage;
    public int per; //관통
    private bool isRotate = false;

    private Rigidbody2D rigid;
    private SpriteRenderer sr;
    private Collider2D cl;

    public Sprite[] sprites;
    public GameObject effect;

    private void Awake()
    {   
        rigid = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        cl = GetComponent<Collider2D>();
    }

    public void Init(float damage, int per, Vector3 dir)
    {
        this.damage = damage;
        this.per = per;

        if (per >= 0)
            rigid.velocity = dir * 15f; //15f : 탄속, 변경필요
    }

    public void HCl(float damage)
    {
        this.damage = damage;
        this.per = -999;
        cl.enabled = false;
        sr.sprite = sprites[0];
        sr.color = new Color(1, 1, 1, 1);
        transform.localScale = Vector3.one;
        Invoke("HCl_Animation", 1f);
    }

    private void HCl_Animation()
    {
        rigid.gravityScale = 0;
        rigid.velocity = Vector3.zero;
        sr.sprite = sprites[1];
        sr.color = new Color(0.5564f, 0.8066f, 0.3728f, 0.7176f);
        transform.localScale = Vector3.one * 5;
        cl.enabled = true;
        Instantiate(effect, transform);
        //Invoke("HCl_isGone", 3f);
    }

    private void HCl_isGone()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy") || per == -999) 
            return;

        per--;
        if (per < 0) {
            rigid.velocity = Vector2.zero;
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Area") || per == -999)
            return;

        gameObject.SetActive(false);
    }

    public void Rotate()
    {
        isRotate = true;
    }

    private void FixedUpdate()
    {
        if (!isRotate) return;
        
        transform.Rotate(Vector3.forward * -15);
    }
}
