using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD : MonoBehaviour
{
    GameDirector m_GameDirector;

    const float m_NewWaveDisplayTime = 5.0f;

    private Image m_NewWaveImage;
    private TextMeshProUGUI m_NewWaveText;
    private TextMeshProUGUI m_WaveText;
    private float m_NewWaveDiplayTimer = 0.0f;

    private Image m_HealthBar;
    private TextMeshProUGUI m_HealthText;

    private TextMeshProUGUI m_WaveTimer;

    private void Awake()
    {
        m_GameDirector = FindObjectOfType<GameDirector>();
        m_GameDirector.onWaveStart += OnWaveStart;

        m_NewWaveImage = transform.Find("New Wave").GetComponent<Image>();
        m_NewWaveText = m_NewWaveImage.transform.GetComponentInChildren<TextMeshProUGUI>();

        m_WaveTimer = transform.Find("Time Elapsed").GetComponent<TextMeshProUGUI>();
        m_WaveText = transform.Find("Wave Text").GetComponent<TextMeshProUGUI>();

        m_HealthBar = transform.Find("Health Unfilled").Find("Health").GetComponent<Image>();
        m_HealthText = transform.Find("Hitpoints").GetComponent<TextMeshProUGUI>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(m_NewWaveImage.gameObject.activeSelf)
        {
            m_NewWaveDiplayTimer += Time.deltaTime;
            if(m_NewWaveDiplayTimer >= m_NewWaveDisplayTime)
            {
                m_NewWaveImage.gameObject.SetActive(false);
            }
        }
        float minuteFloat = Mathf.Floor(m_GameDirector.currentWaveTime / 60.0f);
        int minute = (int)minuteFloat;
        int second = Mathf.FloorToInt(m_GameDirector.currentWaveTime - (minuteFloat * 60.0f));

        if(second < 10)
        {
            m_WaveTimer.text = "Time: " + minute.ToString() + ":0" + second.ToString();
        }
        else
        {
            m_WaveTimer.text = "Time: " + minute.ToString() + ":" + second.ToString();
        }


        m_HealthText.text = "HP: " + m_GameDirector.human.health.ToString() + "/" + m_GameDirector.humanMaxHealth.ToString();

        float perc = (float)m_GameDirector.human.health / (float)m_GameDirector.humanMaxHealth;
        m_HealthBar.rectTransform.sizeDelta = new Vector2(700.0f * perc, m_HealthBar.rectTransform.sizeDelta.y);

    }

    private void OnWaveStart()
    {
        m_NewWaveImage.gameObject.SetActive(true);
        m_NewWaveDiplayTimer = 0.0f;
        m_NewWaveText.text = "Wave " + m_GameDirector.currentStage;
        m_WaveText.text = "Wave: " + m_GameDirector.currentStage;
    }
}
