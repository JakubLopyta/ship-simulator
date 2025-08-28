using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class ChunkHandler : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] GameObject WaterPrefab;

    [SerializeField] float chunkSize = 100f;
    [SerializeField] int renderDistance = 3;

    private Dictionary<Vector2Int, GameObject> activeChunks = new Dictionary<Vector2Int, GameObject>();
    private Vector2Int lastPlayerChunkCoord;


    void Start()
    {
        updateChunks();
    }

    private void Update()
    {
        Vector2Int currentPlayerChunkCoord = GetPlayerChunkCoordinate();
        if (currentPlayerChunkCoord != lastPlayerChunkCoord)
        {
            updateChunks();
        }
    }

    void updateChunks()
    {

        lastPlayerChunkCoord = GetPlayerChunkCoordinate();
        List<Vector2Int> chunksToRemove = new List<Vector2Int>(activeChunks.Keys);



        for (int x = -renderDistance; x <= renderDistance; x++)
        {
            for (int z = -renderDistance; z <= renderDistance; z++)
            {

                Vector2Int chunkCoord = new Vector2Int(lastPlayerChunkCoord.x + x, lastPlayerChunkCoord.y + z);
                if (activeChunks.ContainsKey(chunkCoord))
                {
                    chunksToRemove.Remove(chunkCoord);
                }
                else
                {
                    CreateChunk(chunkCoord);
                }
            }
        }
        foreach (Vector2Int chunkCoord in chunksToRemove)
        {
            DestroyChunk(chunkCoord);
        }
    }

    private Vector2Int GetPlayerChunkCoordinate()
    {
        int currentChunkX = Mathf.RoundToInt(player.transform.position.x / chunkSize);
        int currentChunkZ = Mathf.RoundToInt(player.transform.position.z / chunkSize);
        return new Vector2Int(currentChunkX, currentChunkZ);
    }

    void CreateChunk(Vector2Int chunkCoord)
    {
        Vector3 position = new Vector3(chunkCoord.x * chunkSize, 0, chunkCoord.y * chunkSize);
        GameObject newChunk = Instantiate(WaterPrefab, position, Quaternion.identity);

        newChunk.transform.SetParent(this.transform);
        activeChunks.Add(chunkCoord, newChunk);
    }

    void DestroyChunk(Vector2Int chunkCoord)
    {
        if (activeChunks.TryGetValue(chunkCoord, out GameObject chunkToDestroy))
        {
            Destroy(chunkToDestroy);
            activeChunks.Remove(chunkCoord);
        }
    }
}
