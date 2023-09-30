 using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Random = UnityEngine.Random;

public class GameDirector
{

    public int currentStage { get; private set; }
    public bool isPlaying { get; private set; }

    public Action onPause;

    public GameDirector()
    {
        m_PlayerCamera = GameObject.FindObjectOfType<Camera>();
        AdvanceStage();
    }

    public void Start()
    {

    }

    public void AdvanceStage()
    {
        currentStage++;

        
        for(int i = 0; i < 10; i++)
        {
            Slime slime = SpawnEnemy<Slime>();
            slime.SetHealth(5);
        }
    }

    private void EndStage()
    {

    }

    private Camera m_PlayerCamera;

    private Vector2 ChooseSpawnLocation()
    {
        Rect cameraRect = new Rect();
        cameraRect.height = m_PlayerCamera.orthographicSize * 2.0f;
        cameraRect.width = m_PlayerCamera.aspect * cameraRect.height;
        cameraRect.center = m_PlayerCamera.transform.position;


        float spawnSize = 0.75f;
        Rect spawnLocation = new Rect();
        spawnLocation.size = new Vector2(spawnSize, spawnSize);

        float spawnDistance = 5.0f;

        spawnLocation.center = new Vector2(Random.Range(-spawnDistance, spawnDistance), Random.Range(-spawnDistance, spawnDistance));

        int breakoutSafety = 20;

        while(!IsEnoughSpaceToSpawn(spawnLocation.center) || cameraRect.Overlaps(spawnLocation))
        {

            if(breakoutSafety == 0)
            {
                spawnLocation.center = new Vector2(Random.Range(-spawnDistance, spawnDistance), Random.Range(-spawnDistance, spawnDistance));
                int nestBreakoutSafety = 20;
                while(!IsEnoughSpaceToSpawn(spawnLocation.center))
                {
                    if(nestBreakoutSafety == 0)
                    {
                        Debug.LogError("Unable to find spawn location.");
                        return Vector2.zero;
                    }
                    spawnLocation.center = new Vector2(Random.Range(-spawnDistance, spawnDistance), Random.Range(-spawnDistance, spawnDistance));
                    nestBreakoutSafety--;
                }
                break;
            }

            spawnLocation.center = new Vector2(Random.Range(-spawnDistance, spawnDistance), Random.Range(-spawnDistance, spawnDistance));

            breakoutSafety--;
        }

        return spawnLocation.center;
    }


    private T SpawnEnemy<T>() where T : Enemy
    {
        //Choose spawn location
        Vector2 spawnLocation = ChooseSpawnLocation();


        GameObject enemyObject = new GameObject(typeof(T).Name);
        enemyObject.transform.position = spawnLocation;

        T enemy = enemyObject.AddComponent<T>();

        return enemy;

    }

    Collider2D[] overlapResults = new Collider2D[10];

    private bool IsEnoughSpaceToSpawn(Vector2 position)
    {
        int overlapCount = Physics2D.OverlapBoxNonAlloc(position, new Vector2(0.8f, 0.8f), 0.0f, overlapResults);
        while(overlapCount == overlapResults.Length)
        {
            overlapResults = new Collider2D[overlapResults.Length * 2];
            overlapCount = Physics2D.OverlapBoxNonAlloc(position, new Vector2(0.8f, 0.8f), 0.0f, overlapResults);
        }

        for(int i = 0; i < overlapCount; i++)
        {
            Collider2D collider = overlapResults[i];
            if(collider.gameObject.layer !=  8 && collider.gameObject.layer != 9)
            {
                return false;
            }
        }

        return true;
    }

}
