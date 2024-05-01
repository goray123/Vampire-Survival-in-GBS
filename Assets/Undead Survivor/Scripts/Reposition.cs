using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Reposition : MonoBehaviour
{
    private Collider2D cl;

    private void Awake() 
    {
        cl = GetComponent<Collider2D>();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Area"))
            return;

        Vector3 playerPos = GameManager.instance.player.transform.position;
        Vector3 myPos = transform.position;
        

        switch (transform.tag) {
            case "Ground":
                float diffX = playerPos.x - myPos.x;
                float diffY = playerPos.y - myPos.y;
                float dirX = diffX < 0 ? -1 : 1;
                float dirY = diffY < 0 ? -1 : 1;
                diffX = Mathf.Abs(diffX);
                diffY = Mathf.Abs(diffY);

                if (diffX > diffY) {
                    transform.Translate(Vector3.right * dirX * 60); //타일 팔레트 2개 넘어야 하므로 60이다.
                }
                else if (diffX < diffY) {
                    transform.Translate(Vector3.up * dirY * 60); //타일 팔레트 2개 넘어야 하므로 60이다.
                }
                break;
            case "Enemy":
                if (cl.enabled) {
                    Vector3 dist = playerPos - myPos;
                    Vector3 ran = new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0);
                    transform.Translate(dist * 2);
                }
                break;
        }
    }
}
