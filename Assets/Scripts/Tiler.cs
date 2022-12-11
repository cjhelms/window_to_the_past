using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Tiler : MonoBehaviour
{
    enum State
    {
        Uninitialized,
        Idle,
        WaitForRewind,
        WaitForReplay
    }    

    const int MaxTiles = 4;
    const int MaxXY = 1;
    const int MaxLayerMask = 31;
    const int MaxDepth = MaxTiles - 1;
    const int MinDepth = 0;

    State state;    
    GameObject[] cameras;
    GameObject[] tiles;
    int depth;
    
    public void Initialize(GameObject level)
    {
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
        depth = 0;
        ActivateTile(depth);
        PushCamera();
        state = State.Idle;
    }

    public void HandleFlashbackRequest(float time)
    {
        Assert.IsTrue(state == State.Idle || state == State.WaitForReplay);
        if(depth < MaxDepth)
        {
            SendMessage("Flashback");
            state = State.WaitForRewind;
        }
        else
        {
            Debug.Log("Maximum depth reached, cannot flashback!");
        }
    }

    public void RewindComplete()
    {
        Assert.AreNotEqual(state, State.Uninitialized); 
        if(state == State.WaitForRewind)
        {
            PushTile();
            state = State.WaitForReplay;
        }
    }

    public void ReplayComplete()
    {
        Assert.AreEqual(state, State.WaitForReplay);
        Assert.IsTrue(depth > 0);
        PopTile();
    }

    void start()
    {
        cameras = new GameObject[MaxTiles];
        tiles = new GameObject[MaxTiles];
        state = State.Uninitialized;
    }

    void PushTile()
    {
        depth++;
        tiles[depth] = Instantiate(tiles[depth - 1]);
        ActivateTile(depth);
        PushCamera();
    }

    void PopTile()
    {
        PopCamera();
        DeactivateTile(depth);
        depth--;
    }

    void PushCamera()
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

    void PopCamera()
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

    LayerMask GetLayerMask(int ndx)
    {
        return (LayerMask)(MaxLayerMask - ndx);
    }

    Color RandomColor()
    {
        return new Color(
            Random.Range(0.0f, 1.0f),
            Random.Range(0.0f, 1.0f),
            Random.Range(0.0f, 1.0f),
            1);
    }

    GameObject NewCamera(int ndx)
    {
        GameObject obj = new GameObject("Cam" + ndx);
        obj.transform.parent = Camera.main.transform;
        obj.transform.position = Camera.main.transform.position;
        Camera cam = obj.AddComponent<Camera>() as Camera;
        cam.orthographic = true;
        cam.cullingMask = 1 << GetLayerMask(ndx) | 1;
        cam.enabled = false;
        cam.backgroundColor = RandomColor();
        return obj;
    }

    void InitializeTile(int ndx)
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

    void ActivateTile(int ndx)
    {
        GameObject obj = tiles[ndx];
        obj.SetActive(true);
        foreach(Transform child in obj.transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    void DeactivateTile(int ndx)
    {
        GameObject obj = tiles[ndx];
        obj.SetActive(false);
        foreach(Transform child in obj.transform)
        {
            child.gameObject.SetActive(false);
        }
    }
}
