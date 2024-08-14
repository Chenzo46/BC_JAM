using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject inventoryUIVisual;
    private List<GameObject> visualReferences = new List<GameObject>();
    private List<ScriptableItem> prevVisRefs = new List<ScriptableItem>();
    public static InventoryUI Singleton;
    private void Awake() {
        Singleton = this;
    }
    private void Start() {
        UpdateInventoryUI(InventorySystem.Singleton.inventory);    
    }

    public void UpdateInventoryUI(List<ScriptableItem> inventoryList){
        clearInventoryUI();

        List<ScriptableItem> accounted = new List<ScriptableItem>();
        foreach(ScriptableItem it in inventoryList){
            GameObject nVisual = Instantiate(inventoryUIVisual, transform, false);
            visualReferences.Add(nVisual);
            nVisual.GetComponent<Image>().sprite = it.inventorySprite;

            accounted.Add(it);

            if(!prevVisRefs.Contains(it)){
                nVisual.transform.localScale = new Vector3(0,0,1);
                nVisual.transform.DOScale(1, 0.5f).SetEase(Ease.OutBounce);
                prevVisRefs.Add(it);
            }
        }

        ScriptableItem temp = null;
        foreach(ScriptableItem it in prevVisRefs){
            if(prevVisRefs.Contains(it) && !accounted.Contains(it)){
                temp = it;
            }
        }
        if(temp != null){
            prevVisRefs.Remove(temp);
        }
    }

    private void clearInventoryUI(){
        foreach(GameObject gobj in visualReferences){
            Destroy(gobj);
        }
        visualReferences.Clear();
    }
}
