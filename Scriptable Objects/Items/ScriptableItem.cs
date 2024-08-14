using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Scriptable Objects/New Item", fileName = "Item")]
public class ScriptableItem : ScriptableObject
{
    public string itemName;
    public Sprite inventorySprite;
}
