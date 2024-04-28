using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int id; //����Ÿ��
    public int prefabId; //������ƮŸ��
    public float damage; 
    public int count; //���� or �����
    public float speed; //ȸ���ӵ� or ����ӵ�

    private float timer;
    private Player player;

    private void Awake()
    {
        player = GetComponentInParent<Player>();
    }

    public void Init()
    {
        switch (id) {
            case 0: //����ȸ������
                speed = -150;
                Batch();
                break;
            default:
                speed = 0.3f;
                break;
        }
    }

    private void Batch() //����ȸ�����Ⱑ �������ɶ����� ����Ǵ� ���� ��ġ �Լ�
    {
        for (int index = 0; index < count; index++) {
            Transform bullet;
            
            if (index < transform.childCount) {
                bullet = transform.GetChild(index);
            }
            else {
                bullet = GameManager.instance.pool.Get(prefabId).transform;
                bullet.parent = transform;
            }
            
            bullet.localPosition = Vector3.zero;
            bullet.localRotation = Quaternion.identity;

            Vector3 rotVec = Vector3.forward * 360 * index / count;
            bullet.Rotate(rotVec);
            bullet.Translate(bullet.up * 2.2f, Space.World); //������ ���� �ʿ�(2.2f)
            bullet.GetComponent<Bullet>().Init(damage, -999, Vector3.zero); //-999 : ���Ѱ���
        }
    }

    private void Start()
    {
        Init();
    }

    private void Update()
    {
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
    }
}