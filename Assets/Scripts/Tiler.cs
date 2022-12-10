using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tiler : MonoBehaviour
{
    private const int MaxDepth = 3;
    private const int MinDepth = 0;

    private Camera cameraMain;
    private Camera cameraTile1;
    private Camera cameraTile2;
    private Camera cameraTile3;
    private GameObject tileMain;
    private GameObject tile1;
    private GameObject tile2;
    private GameObject tile3;
    private int depth;

    void Start()
    {
        cameraMain = GameObject.Find("CameraMain").GetComponent<Camera>();
        cameraTile1 = GameObject.Find("CameraTile1").GetComponent<Camera>();
        cameraTile2 = GameObject.Find("CameraTile2").GetComponent<Camera>();
        cameraTile3 = GameObject.Find("CameraTile3").GetComponent<Camera>();
        
        tileMain = GameObject.Find("TileMain");
        tile1 = GameObject.Find("Tile1");
        tile2 = GameObject.Find("Tile2");
        tile3 = GameObject.Find("Tile3");
        
        depth = 0;

        cameraMain.enabled = true;
        cameraTile1.enabled = false;
        cameraTile2.enabled = false;
        cameraTile3.enabled = false;

        // tileMain.setLayer("TileMain");
        // tile1.setLayer("Tile1");
        // tile2.setLayer("Tile2");
        // tile3.setLayer("Tile3");

        tileMain.layer = LayerMask.NameToLayer("TileMain");
        tile1.layer = LayerMask.NameToLayer("Tile1");
        tile2.layer = LayerMask.NameToLayer("Tile2");
        tile3.layer = LayerMask.NameToLayer("Tile3");
        
        // Only show the default and corresponding tile layers for each camera
        cameraMain.cullingMask = 1 << LayerMask.NameToLayer("TileMain");
        cameraTile1.cullingMask = 1 << LayerMask.NameToLayer("Tile1");
        cameraTile2.cullingMask = 1 << LayerMask.NameToLayer("Tile2");
        cameraTile3.cullingMask = 1 << LayerMask.NameToLayer("Tile3");
    }

    void Flashback()
    {
        IncreaseDepth();
        // for each tile
        //   rewind
        // copy tile[depth-1] into tile[depth]
        return;
    }

    void Collapse()
    {
        DecreaseDepth();
        return;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            IncreaseDepth();
        }
        else if(Input.GetKeyDown(KeyCode.Backspace))
        {
            DecreaseDepth();
        }
    }

    private void IncreaseDepth()
    {
        if(depth < MaxDepth)
        {
            depth++;
            UpdateCameras();
            UpdateTiles();
            Debug.Log("Increased depth, new depth: " + depth);
        }
        else
        {
            Debug.Log("Failed to increase depth, maximum depth reached!");
        }
    }

    private void DecreaseDepth()
    {
        if(depth > MinDepth)
        {
            depth--;
            UpdateCameras();
            UpdateTiles();
            Debug.Log("Decreased depth, new depth: " + depth);
        }
        else
        {
            Debug.Log("Failed to decrease depth, minimum depth reached!");
        }
    }

    private void UpdateCameras()
    {
        return;
    }

    private void UpdateTiles()
    {
        return;
    }
}
