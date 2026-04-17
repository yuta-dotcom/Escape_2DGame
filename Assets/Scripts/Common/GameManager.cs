using UnityEngine;
using TMPro;
using System;

public class GameManager : MonoBehaviour
{
    private bool isGameOver = false;
    [SerializeField] private TMP_Text collectibleCountText;
    private int requiredCollectibleCount;
    private int currentCollectibleCount;
    public static bool IsClear { get; private set; }

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
        SoundManager.instance.PlayBgm("GameBgm");
        InitElements();
        UpDateCollectibleText();
    }

    private void InitElements()
    {
        requiredCollectibleCount = GameObject.FindGameObjectsWithTag("Gem").Length;
        currentCollectibleCount = 0;
        IsClear = false;
        isGameOver = false;
    }

    public void AddCollectible()
    {
        if (isGameOver) return;
        currentCollectibleCount++;
        UpDateCollectibleText();
        if (currentCollectibleCount == requiredCollectibleCount)
        {
            IsClear = true;
        }
    }

    private void UpDateCollectibleText()
    {
        collectibleCountText.text = $"{currentCollectibleCount}/{requiredCollectibleCount}";
    }

    public void GameOver()
    {
        if (isGameOver) return;
        SoundManager.instance.StopBgm("GameBgm");
        isGameOver = true;
        IsClear = false;
        SceneFader.instance.LoadScene("Result");
    }
}
