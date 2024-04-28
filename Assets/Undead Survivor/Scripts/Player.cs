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
        inputVec = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    private void FixedUpdate()
    {
        Vector2 nextVec = inputVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
    }

    private void LateUpdate()
    {
        anime.SetFloat("Velocity", inputVec.magnitude); //magnitude : Àý´ñ°ª°ú °°Àº ´À³¦
        if (inputVec.x != 0) {
            sr.flipX = inputVec.x < 0;
        }
    }
}
