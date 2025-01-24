using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InicializeGameScene : MonoBehaviour
{
    [SerializeField] private GameManager gamemanager;
    [SerializeField] private string nextGameScene;

    [SerializeField] private int WarriorCount;
    [SerializeField] private int ArcherCount;
    [SerializeField] private int TankCount;
    [SerializeField] private int CoinCount;
    [SerializeField] private int Seed;
    // Start is called before the first frame update
    void Start()
    {
        if (gamemanager == null)
        {
            gamemanager = FindObjectOfType<GameManager>();
            
            gamemanager.SetNextGameScene(nextGameScene);
            gamemanager.SetArcherCount(ArcherCount);
            gamemanager.SetWarriorCount(WarriorCount);
            gamemanager.SetTankCount(TankCount);
            gamemanager.SetCoinCount(CoinCount);
            gamemanager.SetSeed(Seed);

            gamemanager.InitializeGameLogic();
            gamemanager.HowManyBlocksShouldBe(CounterOfBlocks());
        }
    }
    int CounterOfBlocks()
    {
        int count = WarriorCount + ArcherCount + TankCount + CoinCount;
        return count;
    }

}
