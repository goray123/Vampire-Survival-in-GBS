using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gear : MonoBehaviour
{
    public ItemData.ItemType type;
    public float rate;

    public void Init(ItemData data)
    {
        name = "Gear " + data.itemId;
        transform.parent = GameManager.instance.player.transform;
        transform.localPosition = Vector3.zero;

        type = data.Type;
        rate = data.damages[0];
        ApplyGear();
    }

    public void LevelUp(float rate)
    {
        this.rate = rate;
        ApplyGear();
    }

    public void ApplyGear()
    {
        switch(type) {
            case ItemData.ItemType.Glove:
                RateUp();
                break;
            case ItemData.ItemType.Shoe:
                SpeedUp();
                break;
        }
    }

    private void RateUp()
    {
        Weapon[] weapons = transform.parent.GetComponentsInChildren<Weapon>();

        foreach (Weapon weapon in weapons) {
            switch(weapon.id) {
                case 0:
                    weapon.speed = 150 + (150*rate);
                    break;
                case 1:
                    weapon.speed = 0.3f * (1f-rate);
                    break;
            }
        }
    }

    private void SpeedUp()
    {
        float speed = 3f;
        GameManager.instance.player.speed = speed + speed * rate;
    }
}
