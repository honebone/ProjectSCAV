using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


[CreateAssetMenu(menuName = "Database/ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    [SerializeField] private List<ItemData> _items;

    public ItemData Get(string id)
    {
        ItemData result = _items.Find(item => item.Id == id);
        if (result == null)
            DevLog.Error($"[ItemDatabase] ID‚ªŒ©‚Â‚©‚è‚Ü‚¹‚ñ‚Å‚µ‚½: {id}");
        return result;
    }

    public List<ItemData> GetByTags(List<ItemTag> tags)
    {
        return _items.FindAll(item => item.ItemTags.Any(tag => tags.Contains(tag)));
    }
}