using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] Tiler tiler;
    [SerializeField] PlayerController playerController;
    [SerializeField] GameObject[] levels;

    void Start()
    {
        Instantiate(tiler, new Vector3(0, 0, 0), Quaternion.identity).transform.SetParent(gameObject.transform);
        Instantiate(playerController, new Vector3(0, 0, 0), Quaternion.identity).transform.SetParent(gameObject.transform);

        tiler.Initialize(ref levels[0]);
    }

    public void ChangeActivePlayer (GameObject newActivePlayer)
    { playerController.ChangeActivePlayer(newActivePlayer); }
}
