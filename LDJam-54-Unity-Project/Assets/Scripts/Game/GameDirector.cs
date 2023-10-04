using Elanetic.Tools;
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

    public float commonRelicWeight { get; set; } = 0.1f;
    public float uncommonRelicWeight { get; set; } = 0.05f;
    public float rareRelicWeight { get; set; } = 0.0175f;
    public float legendaryRelicWeight { get; set; } = 0.005f;

    public int commonRelicHealAmount { get; set; } = 1;
    public int uncommonRelicHealAmount { get; set; } = 2;
    public int rareRelicHealAmount { get; set; } = 5;
    public int legendaryRelicHealAmount { get; set; } = 10;

    public int minEnemies { get; set; } = 5;
    public int bonusEnemies { get; set; } = 10;
    public int enemyHealth { get; set; } = 5;

    public float waveTime { get; set; } = 120.0f;
    public float currentWaveTime => m_CurrentWaveTime;

    //stats 
    public int commonRelicsPickedUp { get; set; } = 0;
    public int uncommonRelicsPickedUp { get; set; } = 0;
    public int rareRelicsPickedUp { get; set; } = 0;
    public int legendaryRelicsPickedUp { get; set; } = 0;
    public int slimesKilled { get; set; } = 0;
    public int shrubsKilled { get; set; } = 0;
    public int potgoblinsKilled { get; set; } = 0;
    public float timePlayed { get; set; } = 0.0f;

    private float m_CurrentWaveTime;
    private List<Enemy> m_Enemies = new List<Enemy>();
    private List<Action> m_DeathListeners = new List<Action>();
    public Human human { get; private set; }

    void Awake()
    {
        m_PlayerCamera = FindObjectOfType<Camera>();
        human = FindObjectOfType<Human>();
        human.onDeath += OnPlayerDeath;
        human.relicPickerUpper.onRelicPickup += OnRelicPickup;

        isPaused = false;
    }

    public void Start()
    {
        AdvanceWave();

    }

    void Update()
    {
        timePlayed += Time.deltaTime;

        if(m_CurrentWaveTime > 0.0f && human.isAlive)
        {
            m_CurrentWaveTime -= Time.deltaTime;
            if(m_CurrentWaveTime <= 0)
            {
                EndWave();
            }
            else
            {
                if(m_Enemies.Count <= minEnemies)
                {
                    for(int i = 0; i < bonusEnemies; i++)
                    {
                        if(currentStage == 1)
                        {

                            Slime slime = SpawnEnemy<Slime>();
                            slime.SetHealth(enemyHealth);
                        }
                        else if(currentStage <= 3) 
                        {
                            int rand = Random.Range(0, 2);
                            if(rand == 0)
                            {
                                Slime slime = SpawnEnemy<Slime>();
                                slime.SetHealth(enemyHealth);
                            }
                            else
                            {
                                Shrubbery shrub = SpawnEnemy<Shrubbery>();
                                shrub.SetHealth(enemyHealth);
                            }

                        }
                        else
                        {
                            int rand = Random.Range(0, 3);
                            if(rand == 0)
                            {
                                Slime slime = SpawnEnemy<Slime>();
                                slime.SetHealth(enemyHealth);
                            }
                            else if(rand == 1)
                            {
                                PotGoblin gob = SpawnEnemy<PotGoblin>();
                                gob.SetHealth(enemyHealth);
                            }
                            else
                            {
                                Shrubbery shrub = SpawnEnemy<Shrubbery>();
                                shrub.SetHealth(enemyHealth);
                            }
                        }
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

    List<int> abilitiesLeft = new List<int>() { 0, 1, 2, 3 };

    public void AdvanceWave()
    {
        currentStage++;

        bonusEnemies += 5;

        minEnemies += 2;

        enemyHealth = ((currentStage / 2) + 1) * 5;

        m_CurrentWaveTime = waveTime;

        if(currentStage > 2)
        {
            if(currentStage % 2 == 1)
            {
                if(abilitiesLeft.Count > 0)
                {
                    int ab = Random.Range(0, abilitiesLeft.Count);

                    int chosenAbility = abilitiesLeft[ab];
                    abilitiesLeft.RemoveAt(ab);

                    switch(chosenAbility)
                    {
                        case 0:
                            human.cleaveTrigger.gameObject.SetActive(true);
                            break;
                        case 1:
                            human.earthTrigger.gameObject.SetActive(true);
                            break;
                        case 2:
                            human.poisonTrigger.gameObject.SetActive(true);
                            break;
                        case 3:
                            human.smiteTrigger.gameObject.SetActive(true);
                            break;
                        default:
                            break;
                    }

                }
            }
        }

        onWaveStart?.Invoke();

        for(int i = 0; i < 10; i++)
        {
            Slime slime = SpawnEnemy<Slime>();
            slime.SetHealth(5);
        }
    }

    private void EndWave()
    {
        m_CurrentWaveTime = 0.0f;

        for(int i = 0; i < m_Enemies.Count; i++)
        {
            m_Enemies[i].onDeath -= m_DeathListeners[i];
            m_Enemies[i].Die();
        }

        m_Enemies.Clear();
        m_DeathListeners.Clear();

        /*
        RewardScreen rewardsScreen = FindObjectOfType<RewardScreen>(true);
        rewardsScreen.gameObject.SetActive(true);
        rewardsScreen.onFinishReward += OnFinishReward;
        */

        onWaveComplete?.Invoke();

        AdvanceWave();
    }

    private void OnFinishReward()
    {
        RewardScreen rewardsScreen = FindObjectOfType<RewardScreen>(true);
        rewardsScreen.onFinishReward -= OnFinishReward;

        AdvanceWave();

    }

    private void OnEnemyDeath(Enemy enemy)
    {
        //WARNING enemy parameter will be null due to Unity null overrid

        //Player killed enemy
        for(int i = 0; i < m_Enemies.Count; i++)
        {
            if(ReferenceEquals(m_Enemies[i], enemy))
            {
                m_Enemies.RemoveAt(i);
                m_DeathListeners.RemoveAt(i);
                break;
            }
        }

        Relic relic = RollForRelic();
        if (relic != null)
        {
            GameObject relicObject = new GameObject("Relic Pickup");
            relicObject.transform.position = enemy.transform.position;
            RelicPickup relicPU = relicObject.AddComponent<RelicPickup>();
            relicPU.SetRelic(relic);
        }

        if(enemy is Slime)
        {
            slimesKilled++;
        }
        else if(enemy is Shrubbery)
        {
            shrubsKilled++;
        }
        else if(enemy is PotGoblin)
        {
            potgoblinsKilled++;
        }
    }

    private Relic RollForRelic()
    {
        float remaining = 1.0f - (commonRelicWeight + uncommonRelicWeight + rareRelicWeight + legendaryRelicWeight);

        int index = RandomWeighted.Get(remaining, commonRelicWeight, uncommonRelicWeight, rareRelicWeight, legendaryRelicWeight);
        switch(index)
        {
            case 0:
                return null;
            case 1:
                return new Relic(RelicRarity.Common);
            case 2:
                return new Relic(RelicRarity.Uncommon);
            case 3:
                return new Relic(RelicRarity.Rare);
            case 4:
                return new Relic(RelicRarity.Legendary);
        }
        return null;
    }

    public int humanMaxHealth { get; set; } = 100;

    private void OnRelicPickup(Relic relic)
    {
        int newHealth = human.health;
        switch(relic.rarity)
        {
            case RelicRarity.Common:
                newHealth += commonRelicHealAmount;
                commonRelicsPickedUp++;
                break;
            case RelicRarity.Uncommon:
                newHealth += uncommonRelicHealAmount;
                uncommonRelicsPickedUp++;
                break;
            case RelicRarity.Rare:
                newHealth += rareRelicHealAmount;
                rareRelicsPickedUp++;
                break;
            case RelicRarity.Legendary:
                newHealth += legendaryRelicHealAmount;
                legendaryRelicsPickedUp++;
                break;
            default:
                break;
        }

        if(newHealth > humanMaxHealth)
        {
            newHealth = humanMaxHealth;
        }

        human.SetHealth(newHealth);

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


        float spawnSize = 0.05f;
        Rect spawnLocation = new Rect();
        spawnLocation.size = new Vector2(spawnSize, spawnSize);

        Vector2 spawnDistance = new Vector2(12, 6);

        spawnLocation.center = new Vector2(Random.Range(-spawnDistance.x, spawnDistance.x), Random.Range(-spawnDistance.y, spawnDistance.y));

        int breakoutSafety = 20;

        while(!IsEnoughSpaceToSpawn(spawnLocation.center) || cameraRect.Overlaps(spawnLocation))
        {

            if(breakoutSafety == 0)
            {
                spawnLocation.center = new Vector2(Random.Range(-spawnDistance.x, spawnDistance.x), Random.Range(-spawnDistance.y, spawnDistance.y));
                int nestBreakoutSafety = 20;
                while(!IsEnoughSpaceToSpawn(spawnLocation.center))
                {
                    if(nestBreakoutSafety == 0)
                    {
                        Debug.LogError("Unable to find spawn location.");
                        return Vector2.zero;
                    }
                    spawnLocation.center = new Vector2(Random.Range(-spawnDistance.x, spawnDistance.x), Random.Range(-spawnDistance.y, spawnDistance.y));
                    nestBreakoutSafety--;
                }
                break;
            }

            spawnLocation.center = new Vector2(Random.Range(-spawnDistance.x, spawnDistance.x), Random.Range(-spawnDistance.y, spawnDistance.y));

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

        Action a = () =>
        {
            OnEnemyDeath(enemy);
        };
        enemy.onDeath += a;
        m_DeathListeners.Add(a);

        m_Enemies.Add(enemy);

        return enemy;

    }

    Collider2D[] overlapResults = new Collider2D[10];

    private bool IsEnoughSpaceToSpawn(Vector2 position)
    {
        int layerMask = 1 | (1 << 11) | (1 << 6);

        if(Vector2.Distance(position, human.transform.position) < 3.5f) return false;

        int overlapCount = Physics2D.OverlapBoxNonAlloc(position, new Vector2(0.8f, 0.8f), 0.0f, overlapResults, layerMask);
        while(overlapCount == overlapResults.Length)
        {
            overlapResults = new Collider2D[overlapResults.Length * 2];
            overlapCount = Physics2D.OverlapBoxNonAlloc(position, new Vector2(0.8f, 0.8f), 0.0f, overlapResults, layerMask);
        }

        for(int i = 0; i < overlapCount; i++)
        {
            Collider2D collider = overlapResults[i];
            if(collider.gameObject.layer == 11) continue;
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
