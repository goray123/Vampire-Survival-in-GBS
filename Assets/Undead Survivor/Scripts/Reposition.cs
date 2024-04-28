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
        Vector3 playerDir = GameManager.instance.player.inputVec;
        float dirX = playerPos.x - myPos.x;
        float dirY = playerPos.y - myPos.y;
        float diffX = Mathf.Abs(dirX);
        float diffY = Mathf.Abs(dirY);
        dirX = dirX > 0 ? 1 : -1;
        dirY = dirY > 0 ? 1 : -1;

        switch (transform.tag) {
            case "Ground":
                if (diffX > diffY) {
                    transform.Translate(Vector3.right * dirX * 60); //타일 팔레트 2개 넘어야 하므로 60이다.
                }
                else if (diffX < diffY) {
                    transform.Translate(Vector3.up * dirY * 60); //타일 팔레트 2개 넘어야 하므로 60이다.
                }
                break;
            case "Enemy":
                if (cl.enabled) {
                    transform.Translate(playerDir * 30 + new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0));
                }
                break;
        }
    }
}
