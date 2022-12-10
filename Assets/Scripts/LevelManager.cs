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
        playerController = Instantiate(playerController, new Vector3(0, 0, 0), Quaternion.identity);
        playerController.transform.SetParent(gameObject.transform);

        tiler = Instantiate(tiler, new Vector3(0, 0, 0), Quaternion.identity);
        tiler.transform.SetParent(gameObject.transform);
        tiler.Initialize(ref levels[0]);
    }

    public void ChangeActivePlayer (GameObject newActivePlayer)
    { playerController.ChangeActivePlayer(newActivePlayer); }
}
