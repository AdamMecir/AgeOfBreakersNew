using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameStatus : MonoBehaviour
{ 
    [Range(0.1f, 10f)][SerializeField] float gameSpeed = 1f;
    [SerializeField] int pointsPerBlockDestroyed = 2;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] int currentScore = 0;
    [SerializeField] bool isAutoPlayEnabled;
    [SerializeField] public bool ObjectPoolEnable;
    [SerializeField] public bool DontPoolAgain;



    private void Awake()
    {
        int gameStatusCount = FindObjectsOfType<GameStatus>().Length;
        if(gameStatusCount > 1)
        {
            gameObject.SetActive(false);
            
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        scoreText.text = currentScore.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        Time.timeScale = gameSpeed;
    }
    public void ResetGame()
    {

        Destroy(gameObject);
    }
    public void AddToScore()
    {
        currentScore += pointsPerBlockDestroyed;
        scoreText.text = currentScore.ToString();
    }
    public bool IsAutoPlayEnabled()
    {
        return isAutoPlayEnabled;
    }
    public void EnableObjectPool()
    {
        ObjectPoolEnable = true;
    }
    public void EnableAutoPlay()
    {
        isAutoPlayEnabled = true;
    }
}
