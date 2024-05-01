using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int id; //무기타입
    public int prefabId; //오브젝트타입
    public float damage; 
    public int count; //개수 or 관통력
    public float speed; //회전속도 or 연사속도

    private float timer;
    private Player player;

    private void Awake()
    {
        player = GameManager.instance.player;
    }

    public void Init(ItemData data)
    {   
        name = "Weapon " + data.itemId;
        transform.parent = player.transform;
        transform.localPosition = Vector3.zero;

        id = data.itemId;
        damage = data.baseDamage;
        count = data.baseCount;

        for (int index = 0; index < GameManager.instance.pool.prefabs.Length; index++) {
            if (data.projectile == GameManager.instance.pool.prefabs[index]) {
                prefabId = index;
                break;
            }
        } //editor 의존도를 낮추기 위해서

        switch (id) {
            case 0: //근접회전무기
                speed = -150;
                Batch();
                break;
            default:
                speed = 0.66f; //기본 연사속도
                break;
        }

        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }

    private void Batch() //근접회전무기가 레벨업될때마다 실행되는 무기 배치 함수
    {
        for (int index = 0; index < count; index++) {
            Transform bullet;
            
            if (index < transform.childCount) {
                bullet = transform.GetChild(index); //있는 무기는 재활용
            }
            else {
                bullet = GameManager.instance.pool.Get(prefabId).transform; //없는 무기는 재생성
                bullet.parent = transform;
            }
            
            bullet.localPosition = Vector3.zero;
            bullet.localRotation = Quaternion.identity;

            Vector3 rotVec = Vector3.forward * 360 * index / count;
            bullet.Rotate(rotVec);
            bullet.Translate(bullet.up * 2.2f, Space.World); //반지름 설정 필요(2.2f)
            bullet.GetComponent<Bullet>().Init(damage, -999, Vector3.zero); //-999 : 무한관통
        }
    }

    private void Update()
    {
        if (!GameManager.instance.isLive) return;

        switch (id) {
            case 0:
                transform.Rotate(Vector3.forward * speed * Time.deltaTime);
                break;
            default:
                timer += Time.deltaTime;
                if (timer > speed) {
                    timer -= speed;
                    Fire();
                }
                break;
        }
    }

    public void LevelUp(float damage, int count)
    {
        this.damage = damage;
        this.count += count;

        if (id == 0)
            Batch();

        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);

    }

    private void Fire()
    {
        if (!player.scanner.nearestTarget) //null is false
            return;

        Vector3 targetPos = player.scanner.nearestTarget.position;
        Vector3 dir = targetPos - transform.position;
        dir = dir.normalized;

        Transform bullet = GameManager.instance.pool.Get(prefabId).transform;
        bullet.position = transform.position;
        bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);
        bullet.GetComponent<Bullet>().Init(damage, count, dir);

        AudioManager.instance.PlaySfx(AudioManager.Sfx.Range);
    }
}
