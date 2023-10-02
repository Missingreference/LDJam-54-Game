 using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Random = UnityEngine.Random;

public class GameDirector : MonoBehaviour
{

    static public bool isPaused { get; private set; } = false;

    public int currentStage { get; private set; }
    public bool isPlaying { get; private set; }

    public Action onPause;
    public Action onWaveStart;
    public Action onWaveComplete;
    public Action onPlayerDeath;
    public Action<Relic> onRelicPickup;

    public float waveTime { get; set; } = 120.0f;
    public float currentWaveTime => m_CurrentWaveTime;

    private float m_CurrentWaveTime;
    private List<Enemy> m_Enemies = new List<Enemy>();
    public Human human { get; private set; }

    void Awake()
    {
        m_PlayerCamera = FindObjectOfType<Camera>();
        human = FindObjectOfType<Human>();
        human.onDeath += OnPlayerDeath;
    }

    public void Start()
    {
        AdvanceWave();
    }

    void Update()
    {
        if(m_CurrentWaveTime > 0.0f)
        {
            m_CurrentWaveTime -= Time.deltaTime;
            if(m_CurrentWaveTime <= 0)
            {
                EndWave();
            }
            else
            {
                if(m_Enemies.Count <= 5)
                {
                    for(int i = 0; i < 10; i++)
                    {
                        Slime slime = SpawnEnemy<Slime>();
                        m_Enemies.Add(slime);
                        slime.onDeath += () =>
                        {
                            OnEnemyDeath(slime);
                        };
                        slime.SetHealth(5);
                    }
                }
            }
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }

#if DEBUG
        if(Input.GetKeyDown(KeyCode.K))
        {
            EndWave();
        }
#endif
    }

    public void AdvanceWave()
    {
        currentStage++;
        m_CurrentWaveTime = waveTime;

        onWaveStart?.Invoke();

        for(int i = 0; i < 10; i++)
        {
            Slime slime = SpawnEnemy<Slime>();
            m_Enemies.Add(slime);
            slime.onDeath += () =>
            {
                OnEnemyDeath(slime);
            };
            slime.SetHealth(5);
        }
    }

    private void EndWave()
    {
        m_CurrentWaveTime = 0.0f;

        for(int i = 0; i < m_Enemies.Count; i++)
        {
            m_Enemies[i].onDeath = null;
            m_Enemies[i].Die();
        }

        m_Enemies.Clear();

        RewardScreen rewardsScreen = FindObjectOfType<RewardScreen>(true);
        rewardsScreen.gameObject.SetActive(true);
        rewardsScreen.onFinishReward += OnFinishReward;

        onWaveComplete?.Invoke();
    }

    private void OnFinishReward()
    {
        RewardScreen rewardsScreen = FindObjectOfType<RewardScreen>(true);
        rewardsScreen.onFinishReward -= OnFinishReward;

        AdvanceWave();

    }

    private void OnEnemyDeath(Enemy enemy)
    {
        //Player killed enemy
        m_Enemies.Remove(enemy);
        Relic relic = RollForRelic();
        if (relic != null)
        {
            
        }
    }

    private Relic RollForRelic()
    {
        return null;
    }

    private void OnPlayerDeath()
    {
        GameOver gameOver = FindObjectOfType<GameOver>(true);
        gameOver.gameObject.SetActive(true);
        onPlayerDeath?.Invoke();
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

    public void Pause()
    {
        if(isPaused || !human.isAlive) return;

        isPaused = true;
        Time.timeScale = 0.0f;

        PauseMenu pauseMenu = FindObjectOfType<PauseMenu>(true);
        pauseMenu.gameObject.SetActive(true);
        onPause?.Invoke();
    }

    public void Unpause()
    {
        if(!isPaused) return;
        isPaused = false;
        Time.timeScale = 1.0f;
    }

}
