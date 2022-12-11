using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tiler : MonoBehaviour
{
    private const int MaxXY = 1;
    private const int MaxLayerMask = 31;
    private const int MaxTiles = 12;
    private const int MaxDepth = MaxTiles - 1;
    private const int MinDepth = 0;

    private LevelManager lm;
    private GameObject cameraMain;
    private GameObject[] cameras = new GameObject[MaxTiles];
    private GameObject[] tiles = new GameObject[MaxTiles];
    private int depth;

    // Sets up tiler, intended to be called immediately after instantiation
    public void Initialize(GameObject level)
    {
        // Get a reference to the level manager
        lm = transform.parent.GetComponent<LevelManager>();

        // Get a reference to the main camera
        cameraMain = GameObject.Find("CameraMain");

        // Initialize the tiles
        for(int i = 0; i < MaxTiles; i++)
        {
            cameras[i] = NewCamera(i);
            if(i == 0)
            {
                // Take over ownership of the level & track it as the first tile
                tiles[0] = level;
            }
            else
            {
                // Initialize remaining tiles as copies of the current level
                tiles[i] = Instantiate(level);
            }
            InitializeTile(i);
        }

        // Only show the the first tile at the start
        depth = 0;
        cameras[0].GetComponent<Camera>().enabled = true;
        ActivateTile(0);

        //Set first active player
        lm.ChangeActivePlayer(GetActivePlayer());
    }

    // Rewinds tiles back TIME time, makes new tile, and sets as active
    public void Flashback(float time)
    {
        // for each tile
        //   rewind
        // copy tile[depth-1] into tile[depth]
        IncreaseDepth();
        lm.ChangeActivePlayer(GetActivePlayer());
        return;
    }

    // Forwards time, if applicable, to last active tile's time and sets as active
    public void Collapse()
    {
        DecreaseDepth();
        // for each tile
        //   forward
        lm.ChangeActivePlayer(GetActivePlayer());
        return;
    }

    private void IncreaseDepth()
    {
        if(depth < MaxDepth)
        {
            depth++;
            ActivateTile(depth);
            AddCamera();
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
            DeactivateTile(depth);
            depth--;
            RemoveCamera();
            Debug.Log("Decreased depth, new depth: " + depth);
        }
        else
        {
            Debug.Log("Failed to decrease depth, minimum depth reached!");
        }
    }

    private void AddCamera()
    {
        Camera cam = cameras[depth].GetComponent<Camera>();
        Camera prevCam = cameras[depth - 1].GetComponent<Camera>();
        if(depth % 2 == 1)
        {
            prevCam.rect = new Rect(
                prevCam.rect.x, 
                prevCam.rect.y, 
                prevCam.rect.width / 2, 
                prevCam.rect.height);
            cam.rect = new Rect(
                prevCam.rect.x + (MaxXY - prevCam.rect.x) / 2, 
                prevCam.rect.y,
                prevCam.rect.width,
                prevCam.rect.height);
            Debug.Log("Halving prevCam width, moving cam right");
        }
        else
        {
            prevCam.rect = new Rect(
                prevCam.rect.x, 
                prevCam.rect.y, 
                prevCam.rect.width, 
                prevCam.rect.height / 2);
            cam.rect = new Rect(
                prevCam.rect.x,
                prevCam.rect.y + (MaxXY - prevCam.rect.y) / 2,
                prevCam.rect.width,
                prevCam.rect.height);
            Debug.Log("Halving prevCam height, moving cam up");
        }
        cam.enabled = true;
        Debug.Log("prevCam: " + prevCam.rect.ToString() + ", cam: " + cam.rect.ToString());
    }

    private void RemoveCamera()
    {
        Camera cam = cameras[depth].GetComponent<Camera>();
        cameras[depth + 1].GetComponent<Camera>().enabled = false;
        if(depth % 2 == 1)
        {
            cam.rect = new Rect(
                cam.rect.x, 
                cam.rect.y, 
                cam.rect.width,
                cam.rect.height * 2);
            Debug.Log("Doubling cam height");
        }
        else
        {
            cam.rect = new Rect(
                cam.rect.x, 
                cam.rect.y, 
                cam.rect.width * 2, 
                cam.rect.height);
            Debug.Log("Doubling cam width");
        }
        Debug.Log("cam: " + cam.rect.ToString());
    }

    private LayerMask GetLayerMask(int ndx)
    {
        return (LayerMask)(MaxLayerMask - ndx);
    }

    private Color RandomColor()
    {
        return new Color(
            Random.Range(0.0f, 1.0f),
            Random.Range(0.0f, 1.0f),
            Random.Range(0.0f, 1.0f),
            1);
    }

    private GameObject NewCamera(int ndx)
    {
        GameObject obj = new GameObject("Cam" + ndx);
        obj.transform.parent = cameraMain.transform;
        obj.transform.position = cameraMain.transform.position;
        Camera cam = obj.AddComponent<Camera>() as Camera;
        cam.orthographic = true;
        cam.cullingMask = 1 << GetLayerMask(ndx) | 1;
        cam.enabled = false;
        cam.backgroundColor = RandomColor();
        return obj;
    }

    private void InitializeTile(int ndx)
    {
        GameObject obj = tiles[ndx];
        obj.name = "Tile" + ndx;
        obj.layer = GetLayerMask(ndx);
        foreach (Transform child in obj.transform)
        {
            child.gameObject.layer = GetLayerMask(ndx);
        }
        DeactivateTile(ndx);
    }

    private void ActivateTile(int ndx)
    {
        GameObject obj = tiles[ndx];
        foreach(Transform child in obj.transform)
        {
            child.gameObject.SetActive(true);
        }
        var enemies = tiles[ndx].GetComponentsInChildren<Enemy>();
        foreach(Enemy enemy in enemies)
        {
            enemy.SetTarget(GetPlayer(ndx));
        }
    }

    private void DeactivateTile(int ndx)
    {
        GameObject obj = tiles[ndx];
        foreach(Transform child in obj.transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    private GameObject GetActivePlayer()
    {
        return GetPlayer(depth);
    }

    private GameObject GetPlayer(int ndx)
    {
        return tiles[ndx].transform.Find("Player").gameObject;
    }
}
