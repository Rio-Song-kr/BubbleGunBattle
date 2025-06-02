using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    private List<Item> _items = new List<Item>();

    public void AddItem(Item item)
    {
        _items.Add(item);
    }

    public void RemoveItem(Item item)
    {
        _items.Remove(item);
    }

    public bool CanCreate(Vector3 newPosition, float distance)
    {
        foreach (var item in _items)
        {
            if (item == null) return true;

            if (Vector3.Distance(item.transform.position, newPosition) < distance)
            {
                return false;
            }
        }

        return true;
    }
}