using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Time")]
    private float timerSeconds;
    private bool isGameOver = false;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text collectibleCountText;
    private int requiredCollectibleCount;
    public int currentCollectibleCount;
    public static bool isAllCollected;

    public static GameManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        InitElements();
        SetTimer();
        UpDateCollectibleText();
    }

    private void InitElements()
    {
        currentCollectibleCount = 0;
        requiredCollectibleCount = 100;
        isAllCollected = false;
        isGameOver = false;
    }
    private void Update()
    {
        UpdateTimer();
    }

    private void UpdateTimer()
    {
        if (isGameOver) return;
        timerSeconds -= Time.deltaTime;
        var span = new TimeSpan(0, 0, (int)timerSeconds);
        timerText.text = span.ToString(@"mm\:ss");

        if (timerSeconds < 0)
        {
            timerSeconds = 0;
            isGameOver = true;
            isAllCollected = false;
        }
    }
    private void SetTimer()
    {
        timerSeconds = 180;
    }


    public void AddCollectible()
    {
        if (isGameOver) return;
        currentCollectibleCount++;
        UpDateCollectibleText();
        if (currentCollectibleCount == requiredCollectibleCount)
        {
            isAllCollected = true;
        }
    }

    private void UpDateCollectibleText()
    {
        collectibleCountText.text = $"{currentCollectibleCount}/{requiredCollectibleCount}";
    }

    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;
        isAllCollected = false;
        SceneFader.instance.LoadScene("Result");
    }


}
