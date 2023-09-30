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
        AdvanceStage();
    }

    public void Start()
    {

    }

    public void AdvanceStage()
    {
        currentStage++;

        Vector2 pos = new Vector2(Random.Range(-5.0f, 5.0f), Random.Range(-5.0f, 5.0f));
        
        for(int i = 0; i < 0; i++)
        {
            GameObject newEnemy = new GameObject("Enemy");
            newEnemy.transform.position = pos;
            newEnemy.AddComponent<Enemy>();
        }
    }

    private void EndStage()
    {

    }


    private void SpawnEnemy()
    {

    }

}
