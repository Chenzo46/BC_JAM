using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomDoor : MonoBehaviour
{
    [SerializeField] private int roomEnterIndex = 0;
    [SerializeField] private int currentRoom = 0;
    [SerializeField] private Transform enterLocation;
    [SerializeField] private float verticalSpeed = 0;
    [SerializeField] private bool upVertical = false;
    private Transform playerTrans;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("player"))
        {
            GameStateManager.Singleton.currentRoom = roomEnterIndex;
            GameStateManager.Singleton.previousRoom = currentRoom;
            SceneTransitioner.Singleton.transitionScene(roomEnterIndex);
            playerTrans = collision.transform;
            if (upVertical)
            {
                playerTrans.GetComponent<Rigidbody2D>().gravityScale = 0;
                playerTrans.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            }

        }

    }

    private void FixedUpdate()
    {
        if(playerTrans != null && upVertical)
        {
            playerTrans.position += Vector3.up * verticalSpeed * Time.fixedDeltaTime;
        }
    }

    public Transform getEnterLocation() { return enterLocation; }

    public int getFromRoom()
    {
        return roomEnterIndex;
    }
}
