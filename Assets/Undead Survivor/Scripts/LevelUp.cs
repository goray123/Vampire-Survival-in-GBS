using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUp : MonoBehaviour
{
    private RectTransform rect;
    private Item[] items;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        items = GetComponentsInChildren<Item>(true);
    }

    public void Show()
    {
        Next();
        rect.localScale = Vector3.one;
        GameManager.instance.Stop();
    }

    public void Hide()
    {
        rect.localScale = Vector3.zero;
        GameManager.instance.Resume();
    }

    public void Select(int index)
    {
        items[index].OnClick();
    }

    private void Next()
    {
        foreach(Item item in items) {
            item.gameObject.SetActive(false);
        }

        int[] ran = new int[3];
        while (true) {
            ran[0] = Random.Range(0,items.Length);
            ran[1] = Random.Range(0,items.Length);
            ran[2] = Random.Range(0,items.Length);

            if (ran[0] != ran[1] && ran[1] != ran[2] && ran[2] != ran[0]) break;
        }

        for (int index = 0; index < ran.Length; index++) {
            Item ranItem = items[ran[index]];

            if (ranItem.level == ranItem.data.damages.Length) {
                items[4].gameObject.SetActive(true);
            }

            ranItem.gameObject.SetActive(true);
        }
    }
}
