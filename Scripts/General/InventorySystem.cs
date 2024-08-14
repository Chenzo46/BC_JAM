using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    public List<ScriptableItem> inventory {get; private set;} =  new List<ScriptableItem>();
    public static InventorySystem Singleton;

    private void Awake() {
        if(Singleton == null){
            Singleton = this;
        }
        // We don't attempt to destroy because this is connected to the same gobj as the GSM
    }
    public void collectItem(ScriptableItem inventoryItem){
        GameStateManager.Singleton.addCondition($"has_{inventoryItem.itemName}", true);
        inventory.Add(inventoryItem);
        InventoryUI.Singleton.UpdateInventoryUI(inventory);
    }

    public void removeItem(string itemName){
        ScriptableItem itemToRemove = null;
        foreach(ScriptableItem it in inventory){
            if(it.itemName.Equals(itemName)){
                itemToRemove = it;
            }
        }
        try{
            inventory.Remove(itemToRemove);
            InventoryUI.Singleton.UpdateInventoryUI(inventory);
        }
        catch (System.Exception e){
            Debug.LogException(new System.Exception($"The item called '{itemName}' is not in the inventory\nDetailed error message: {e.Message}"));
        }
    }   
}
