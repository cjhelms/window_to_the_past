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

    private GameObject cameraMain;
    private GameObject[] cameras = new GameObject[MaxTiles];
    private GameObject[] tiles = new GameObject[MaxTiles];
    private int depth;

    // TODO: Debugging, must be deleted when integrated with level manager
    GameObject level;
    void Start()
    {
        level = Instantiate(Resources.Load("Level_1")) as GameObject;
        Initialize(ref level);
    }

    void Initialize(ref GameObject level)
    {
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
    }

    void Flashback()
    {
        IncreaseDepth();
        // for each tile
        //   rewind
        // copy tile[depth-1] into tile[depth]
        // transform.parent.ChangeActivePlayer(tiles[depth].transform.Find("Player").gameObject);
        return;
    }

    void Collapse()
    {
        DecreaseDepth();
        // transform.parent.ChangeActivePlayer(tiles[depth].transform.Find("Player").gameObject);
        return;
    }

    // TODO: Delete this, only for debugging
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
            prevCam.rect = new Rect(prevCam.rect.x, prevCam.rect.y, prevCam.rect.width / 2, 
                prevCam.rect.height);
            cam.rect = new Rect(prevCam.rect.x + (MaxXY - prevCam.rect.x) / 2, prevCam.rect.y,
                prevCam.rect.width, prevCam.rect.height);
            Debug.Log("Halving prevCam width, moving cam right");
        }
        else
        {
            prevCam.rect = new Rect(prevCam.rect.x, prevCam.rect.y, prevCam.rect.width, 
                prevCam.rect.height / 2);
            cam.rect = new Rect(prevCam.rect.x, prevCam.rect.y + (MaxXY - prevCam.rect.y) / 2,
                prevCam.rect.width, prevCam.rect.height);
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
            cam.rect = new Rect(cam.rect.x, cam.rect.y, cam.rect.width, cam.rect.height * 2);
            Debug.Log("Doubling cam height");
        }
        else
        {
            cam.rect = new Rect(cam.rect.x, cam.rect.y, cam.rect.width * 2, cam.rect.height);
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
        return new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f),
            Random.Range(0.0f, 1.0f), 1);
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
        DeactivateTile(ndx);
    }

    private void ActivateTile(int ndx){
        GameObject obj = tiles[ndx];
        foreach(Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    private void DeactivateTile(int ndx){
        GameObject obj = tiles[ndx];
        foreach(Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }
}
