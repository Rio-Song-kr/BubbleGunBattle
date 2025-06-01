using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    private List<GameObject> _items = new List<GameObject>();

    public void AddItem(GameObject item)
    {
        _items.Add(item);
    }

    public void RemoveItem(GameObject item)
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