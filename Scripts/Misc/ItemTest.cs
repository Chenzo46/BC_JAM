using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTest : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("player"))
        {
            gameObject.SetActive(false);
            GameStateManager.Singleton.setCondition("catnip", true);
        }
    }
}
