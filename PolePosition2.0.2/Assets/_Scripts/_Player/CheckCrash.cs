using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckCrash : MonoBehaviour
{
    public GameObject manager;
    private PolePositionManager component;
    private List<PlayerInfo> players;

    // Start is called before the first frame update
    void Start()
    {
        component = manager.GetComponent<PolePositionManager>();
        players = component.playersList;
    }

    // Update is called once per frame
    void Update()
    {
        checkUp();
    }

    private void checkUp()
    {
        foreach(PlayerInfo jugador in players)
        {
            Vector3 dir = jugador.gameObject.transform.up;
            if(Mathf.Abs(Vector3.Angle(dir, Vector3.up)) > 90)
            {
                Debug.Log($"Has volcado {dir}");
            }
        }
    }
}
