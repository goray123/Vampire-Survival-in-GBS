using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int id; //����Ÿ��
    public int prefabId; //������ƮŸ��
    public float damage;
    public int count; //���� or �����
    public float speed; //ȸ���ӵ� or ����ӵ�
    public int level;
    public ItemData.ItemType type;

    private float timer;
    private Player player;

    private void Awake()
    {
        player = GameManager.instance.player;
    }

    public void Init(ItemData data, int level)
    {
        name = "Weapon " + data.Type + data.itemId;
        if (!(data.Type == ItemData.ItemType.Range && data.itemId == 1))
            transform.parent = player.transform;
        transform.localPosition = Vector3.zero;

        id = data.itemId;
        damage = data.baseDamage;
        count = data.baseCount;
        this.level = level;
        type = data.Type;

        for (int index = 0; index < GameManager.instance.pool.prefabs.Length; index++) {
            if (data.projectile == GameManager.instance.pool.prefabs[index]) {
                prefabId = index;
                break;
            }
        } //editor �������� ���߱� ���ؼ�

        switch (type) {
            case ItemData.ItemType.Melee:
                switch (id) {
                    case 0: //���̾Ƹ�� ��
                        speed = -150; //ȸ���ӵ�
                        Batch();
                        break;
                    case 1: //�ڱ���
                        speed = 0.25f; //ƽ
                        Batch();
                        break;
                }
                break;
            case ItemData.ItemType.Range:
                switch (id) {
                    case 0: //��
                        speed = 1.25f; //����ӵ�
                        break;
                    case 1: //����
                        speed = 5f; //����ӵ�
                        break;
                }
                break;
            default:
                speed = 1.25f; //����ӵ�
                break;
        }

        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }

    private void Batch() //����ȸ�����Ⱑ �������ɶ����� ����Ǵ� ���� ��ġ �Լ�
    {
        if (type == ItemData.ItemType.Melee) {
            Transform bullet;
            switch (id) {
                case 0: //���̾Ƹ�� ��
                    for (int index = 0; index < count; index++) {
                        if (index < transform.childCount) {
                            bullet = transform.GetChild(index); //�ִ� ����� ��Ȱ��
                        }
                        else {
                            bullet = GameManager.instance.pool.Get(prefabId).transform; //���� ����� �����
                            bullet.parent = transform;
                        }

                        bullet.localPosition = Vector3.zero;
                        bullet.localRotation = Quaternion.identity;

                        Vector3 rotVec = Vector3.forward * 360 * index / count;
                        bullet.Rotate(rotVec);
                        bullet.Translate(bullet.up * 2.2f, Space.World); //������ ���� �ʿ�(2.2f)
                        bullet.GetComponent<Bullet>().Init(damage, -999, Vector3.zero); //-999 : ���Ѱ���
                        bullet.GetComponent<Bullet>().Rotate();
                    }
                    break;
                case 1: //�ڱ���
                    if (level == 0) {
                        bullet = GameManager.instance.pool.Get(prefabId).transform; //���� ����� �����
                        bullet.parent = transform;
                    }
                    else {
                        bullet = transform.GetChild(0);
                    }

                    bullet.localPosition = Vector3.zero;
                    bullet.localRotation = Quaternion.identity;
                    bullet.localScale = Vector3.one * (2 + count * 0.2f);

                    bullet.GetComponent<Bullet>().Init(damage, -999, Vector3.zero); //-999 : ���Ѱ���
                    break;
            }
        }
    }

    private void Update()
    {
        if (!GameManager.instance.isLive) return;

        switch(type) {
            case ItemData.ItemType.Melee: {
                    if (id == 0)
                        transform.Rotate(Vector3.forward * speed * Time.deltaTime);
                    break;
                }
            case ItemData.ItemType.Range: {
                    timer += Time.deltaTime;
                    if (timer > speed) {
                        timer -= speed;
                        Fire(id);
                    }
                    break;
                }
        }
    }

    public void LevelUp(float damage, int count)
    {
        this.damage = damage;
        this.count += count;
        level++;

        if (type == ItemData.ItemType.Melee && id == 0)
            Batch();
        if (type == ItemData.ItemType.Melee && id == 1)
            Batch();

        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);

    }

    private void Fire(int id)
    {
        if (id == 0 && player.scanner.nearestTarget) {
            Vector3 targetPos = player.scanner.nearestTarget.position;
            Vector3 dir = targetPos - transform.position;
            dir = dir.normalized;

            Transform bullet = GameManager.instance.pool.Get(prefabId).transform;
            bullet.parent = transform;
            bullet.position = transform.position;
            bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);
            bullet.GetComponent<Bullet>().Init(damage, count, dir);

            AudioManager.instance.PlaySfx(AudioManager.Sfx.Range);
        }
        else if (id == 1) {
            for (int i = 0; i < count; i++) {
                Vector3 targetPos = new Vector3(player.transform.position.x + Random.Range(-7f, 7f), player.transform.position.y + Random.Range(-7f, 7f), 0);
                Transform bullet = GameManager.instance.pool.Get(prefabId).transform;
                bullet.parent = transform;
                bullet.position = new Vector3(targetPos.x, targetPos.y + 5f, targetPos.z);
                bullet.rotation = Quaternion.FromToRotation(Vector3.up, new Vector2(1, 1));
                bullet.GetComponent<Bullet>().HCl(damage);
            }
        }
    }
}
