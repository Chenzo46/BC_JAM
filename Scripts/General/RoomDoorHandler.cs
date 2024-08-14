using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomDoorHandler : MonoBehaviour
{
    [SerializeField] private List<m_Door> rooms;

    private Transform plrTransform;

    [System.Serializable]
    private struct m_Door
    {
        public RoomDoor m_RoomDoor;
    }

    private void Start()
    {
        plrTransform = GameObject.FindGameObjectWithTag("player").GetComponent<Transform>();

        foreach (m_Door room in rooms) 
        {
            if(room.m_RoomDoor.getFromRoom() == GameStateManager.Singleton.previousRoom)
            {
                plrTransform.position = room.m_RoomDoor.getEnterLocation().position;
                break;
            }
        }
    }
}
