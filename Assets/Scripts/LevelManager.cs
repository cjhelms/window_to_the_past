using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] Tiler tiler;
    [SerializeField] PlayerController playerController;
    [SerializeField] GameObject[] levels;
    int curLevel;

    // Intended to be called only once at the very beginning of the game
    void Start()
    {
        // Make the persistent player controller, will persist for rest of the game
        playerController = Instantiate(playerController, new Vector3(0, 0, 0), Quaternion.identity);
        playerController.transform.SetParent(gameObject.transform);
        
        // Set level to first level
        curLevel = -1;
        NextLevel();
    }

    // Sets up the next level of the game
    public void NextLevel() {
        curLevel++;

        // Make a new tiler for every level, the old one will be garbage collected
        tiler = Instantiate(tiler, new Vector3(0, 0, 0), Quaternion.identity);
        tiler.transform.SetParent(gameObject.transform);
        
        //Instantiate the prefab first!
        GameObject level = Instantiate(levels[curLevel], new Vector3(0, 0, 0), Quaternion.identity);
        tiler.Initialize(level);
    }

    // Notify the player controller that the active player has changed & who is now active
    public void ChangeActivePlayer (GameObject newActivePlayer)
    { 
        playerController.ChangeActivePlayer(newActivePlayer); 
    }
}
