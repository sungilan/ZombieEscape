using System.Collections.Generic;
using UnityEngine;

public class ItemDB
{
    private Dictionary<int, ItemEntity> _items = new();

    public ItemDB()
    {
        var res = Resources.Load<ItemData>("DB/ItemSO");
        var itemSo = Object.Instantiate(res);
        var entities = itemSo.Entities;

        if (entities == null || entities.Count <= 0) return;

        var entityCount = entities.Count;
        for (int i = 0; i < entityCount; i++)
        {
            var item = entities[i];

            if (_items.ContainsKey(item._id))
                _items[item._id] = item;
            else
                _items.Add(item._id, item);
        }
    }
    public ItemEntity Get(int id)
    {
        if (_items.ContainsKey(id))
            return _items[id];

        return null;
    }
    //public IEnumerator DbEnumerator()
    //{
    //    return _items.GetEnumerator();
    //}
}

