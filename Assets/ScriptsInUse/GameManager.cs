using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using static UnityEngine.GridBrushBase;

public class GameManager : MonoBehaviour
{
    // Singleton instance
    public static GameManager Instance { get; private set; }


    // General Settings
    [Header("Game Settings")]
    [SerializeField] private int totalBreakableBlocks = 0;
    [SerializeField] private string nextSceneName;
    [SerializeField] private SceneLoader sceneLoader;

    // Parent Objects for Units
    [Header("Parent GameObjects")]
    [SerializeField] private GameObject warriorsParent;
    [SerializeField] private GameObject archersParent;
    [SerializeField] private GameObject tanksParent;
    [SerializeField] private GameObject coinsParent;
    [SerializeField] private Transform particleParent;
    [SerializeField] private Transform coinParticleParent;

    // Score and Death Tracking
    [Header("UI Elements")]
    private int playerScore = 0;
    private int playerDeaths = 0;
    private TextMeshProUGUI scoreTextUI;
    private TextMeshProUGUI deathsTextUI;

    // Prefabs and Object Pools
    [Header("Prefab Settings")]
    [SerializeField] private GameObject warriorPrefab;
    [SerializeField] private List<GameObject> warriorObjectPool;

    [SerializeField] private GameObject archerPrefab;
    [SerializeField] private List<GameObject> archerObjectPool;

    [SerializeField] private GameObject tankPrefab;
    [SerializeField] private List<GameObject> tankObjectPool;

    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private List<GameObject> coinObjectPool;

    [SerializeField] private ParticleSystem particlePrefab;


    [SerializeField] private ParticleSystem particleCoinPrefab;


    [SerializeField] private List<ParticleSystem> particlePool;  // Changed from Queue to List
    [SerializeField] private List<ParticleSystem> CoinParticlePool;  // Changed from Queue to List

    [Header("Count of Prefabs in Scene and Seed")]
    [SerializeField] private int WarriorCount;
    [SerializeField] private int ArcherCount;
    [SerializeField] private int TankCount;
    [SerializeField] private int CoinCount;
    [SerializeField] private int Seed;

    GameStatus theGameStatus;


    [SerializeField] private int poolSize = 90;

    public void SetWarriorCount(int count)
    {
        WarriorCount = count;
    }
    public void SetArcherCount(int count)
    {
        ArcherCount = count;
    }
    public void SetTankCount(int count)
    {
        TankCount = count;
    }
    public void SetCoinCount(int count)
    {
        CoinCount = count;
    }
    public void SetSeed(int count)
    {
        Seed = count;
    }
    void Awake()
    {
        // Ensure only one instance exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Preserve the instance across scenes
        }
        else
        {
            Debug.LogWarning("Multiple GameManager instances detected! Destroying duplicate.");
            Destroy(gameObject); // Destroy duplicate
        }
    }


    public void HowManyBlocksShouldBe(int count)
    {
        totalBreakableBlocks = count;

    }

    public void SetNextGameScene(string nextScene)
    {
        nextSceneName = nextScene;
    }

    public void InitializeGameLogic()
    {
        theGameStatus = FindObjectOfType<GameStatus>();
        sceneLoader = FindObjectOfType<SceneLoader>();

        if (theGameStatus.ObjectPoolEnable && !theGameStatus.DontPoolAgain)
        {
            InitializeWarriorPool();
            InitializeArcherPool();
            InitializeTankPool();
            InitializeCoinPool();
            InitializePools();
            theGameStatus.DontPoolAgain = true;

            ReplaceBlocksWithPrefabsWithPool(WarriorCount, ArcherCount, TankCount, CoinCount, Seed);
        }
        else if (theGameStatus.ObjectPoolEnable && theGameStatus.DontPoolAgain)
        {
            ReplaceBlocksWithPrefabsWithPool(WarriorCount, ArcherCount, TankCount, CoinCount, Seed);
        }
        else
        {
            SpawnBlocksDirectly(WarriorCount, ArcherCount, TankCount, CoinCount, Seed);
        }

        DeactivateAllBlankBoxes();
    }




    private void InitializePools()
    {
        particlePool = new List<ParticleSystem>();  // Initialize as List
        CoinParticlePool = new List<ParticleSystem>();  // Initialize as List


        // Ensure parent objects are assigned or create them dynamically
        if (particleParent == null)
        {
            particleParent = new GameObject("ParticleParent").transform;
            particleParent.SetParent(transform); // Make it a child of this GameObject
        }

        if (coinParticleParent == null)
        {
            coinParticleParent = new GameObject("CoinParticleParent").transform;
            coinParticleParent.SetParent(transform); // Make it a child of this GameObject
        }

        // Initialize particle pools as List
        for (int i = 0; i < poolSize; i++)
        {
            ParticleSystem newParticle = Instantiate(particlePrefab, particleParent);
            newParticle.gameObject.SetActive(false);
            particlePool.Add(newParticle);  // Added to List
        }

        for (int i = 0; i < poolSize; i++)
        {
            ParticleSystem newCoinParticle = Instantiate(particleCoinPrefab, coinParticleParent);
            newCoinParticle.gameObject.SetActive(false);
            CoinParticlePool.Add(newCoinParticle);  // Added to List
        }

    }

    public ParticleSystem GetParticle()
    {
        // Find the first inactive particle in the List
        ParticleSystem particle = particlePool.Find(p => !p.gameObject.activeInHierarchy);

        if (particle != null)
        {
            particle.gameObject.SetActive(true); // Activate
            particle.Clear(); // Clear old particles
            particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear); // Reset
            return particle;
        }
        return null;
    }

    public ParticleSystem GetCoinParticle()
    {
        // Find the first inactive coin particle in the List
        ParticleSystem coinParticle = CoinParticlePool.Find(p => !p.gameObject.activeInHierarchy);

        if (coinParticle != null)
        {
            coinParticle.gameObject.SetActive(true); // Activate
            coinParticle.Clear(); // Clear old particles
            coinParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear); // Reset
            return coinParticle;
        }
        return null;
    }


    public void ReturnParticle(ParticleSystem particle)
    {
        particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        particle.Clear();
        particle.gameObject.SetActive(false);
        particle.transform.position = Vector3.zero;
        particlePool.Add(particle);  // Added to List
    }

    public void ReturnParticleCoin(ParticleSystem coinParticle)
    {
        coinParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        coinParticle.Clear();
        coinParticle.gameObject.SetActive(false);
        coinParticle.transform.position = Vector3.zero;
        CoinParticlePool.Add(coinParticle);  // Added to List
    }







    void ReplaceBlocksWithPrefabsWithPool(int warriorCount, int archerCount, int tankCount, int coinCount, int seed)
    {
        GameObject[] blankBlocks = GameObject.FindGameObjectsWithTag("BlankBlock");

        if (blankBlocks.Length == 0)
        {
            Debug.LogWarning("No blank blocks found to replace!");
            return;
        }

        System.Random random = new System.Random(seed);
        List<GameObject> blockList = new List<GameObject>(blankBlocks);


        SpawnPrefabs(warriorCount, warriorPrefab, blockList,warriorObjectPool, warriorsParent, random);
        SpawnPrefabs(archerCount, archerPrefab, blockList,archerObjectPool, archersParent, random);
        SpawnPrefabs(tankCount, tankPrefab, blockList,tankObjectPool, tanksParent, random);
        SpawnPrefabs(coinCount, coinPrefab, blockList,coinObjectPool, coinsParent, random);
    }

    void SpawnPrefabs(int count, GameObject prefab, List<GameObject> blockList, List<GameObject> PrefabList, GameObject parent, System.Random random)
    {
        for (int i = 0; i < count && blockList.Count > 0; i++)
        {
            int randomIndex = random.Next(blockList.Count);
            GameObject block = blockList[randomIndex];

            // Replace block with prefab
            Vector2 position = block.transform.position;

            GameObject newObject = GetObjectFromPool(PrefabList, prefab, position, Quaternion.identity, parent.transform);

            if(newObject == null)
            {
                Debug.Log("Uz neni prefab");
            }
            //GameObject newObject = Instantiate(prefab, position, Quaternion.identity, parent.transform);
            newObject.SetActive(true);

            // Remove used block
            blockList.RemoveAt(randomIndex);
        }
    }
    void SpawnBlocksDirectly(int warriorCount, int archerCount, int tankCount, int coinCount, int seed)
    {
        GameObject[] blankBlocks = GameObject.FindGameObjectsWithTag("BlankBlock");

        if (blankBlocks.Length == 0)
        {
            Debug.LogWarning("No blank blocks found to replace!");
            return;
        }

        System.Random random = new System.Random(seed);
        List<GameObject> blockList = new List<GameObject>(blankBlocks);

        // Directly instantiate prefabs
        SpawnPrefabsDirectly(warriorCount, warriorPrefab, blockList, warriorsParent, random);
        SpawnPrefabsDirectly(archerCount, archerPrefab, blockList, archersParent, random);
        SpawnPrefabsDirectly(tankCount, tankPrefab, blockList, tanksParent, random);
        SpawnPrefabsDirectly(coinCount, coinPrefab, blockList, coinsParent, random);
    }

    void SpawnPrefabsDirectly(int count, GameObject prefab, List<GameObject> blockList, GameObject parent, System.Random random)
    {
        for (int i = 0; i < count && blockList.Count > 0; i++)
        {
            int randomIndex = random.Next(blockList.Count);
            GameObject block = blockList[randomIndex];

            // Replace block with a new prefab instance
            Vector2 position = block.transform.position;
            GameObject newObject = Instantiate(prefab, position, Quaternion.identity, parent.transform);
            newObject.SetActive(true);

            // Remove used block
            blockList.RemoveAt(randomIndex);
        }
    }

    void DeactivateAllBlankBoxes()
    {
        GameObject[] blankBoxes = GameObject.FindGameObjectsWithTag("BlankBlock");

        foreach (GameObject box in blankBoxes)
        {
            box.SetActive(false);
        }
    }

    void InitializeWarriorPool()
    {
        InitializeObjectPool(ref warriorObjectPool, warriorPrefab, warriorsParent, poolSize);
    }

    void InitializeArcherPool()
    {
        InitializeObjectPool(ref archerObjectPool, archerPrefab, archersParent, poolSize);
    }

    void InitializeTankPool()
    {
        InitializeObjectPool(ref tankObjectPool, tankPrefab, tanksParent, poolSize);
    }

    void InitializeCoinPool()
    {
        InitializeObjectPool(ref coinObjectPool, coinPrefab, coinsParent, poolSize);
    }

    void InitializeObjectPool(ref List<GameObject> pool, GameObject prefab, GameObject parent, int poolSize)
    {
        pool = new List<GameObject>(poolSize);

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(prefab, parent.transform);
            obj.SetActive(false);
            pool.Add(obj);
        }
    }



    GameObject GetObjectFromPool(List<GameObject> PrefabList, GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
    {
        foreach (GameObject obj in PrefabList)
        {
            if (!obj.activeInHierarchy)
            {
                obj.transform.position = position;
                obj.transform.rotation = rotation;

                return obj;
            }
        }
        return null;
    }


    public void IncreaseScore()
    {
        playerScore++;
        UpdateScoreText();
    }

    public void IncreaseDeaths()
    {
        playerDeaths++;
        UpdateDeathsText();
    }

    public int GetScore()
    {
        return playerScore;
    }

    public int GetDeaths()
    {
        return playerDeaths;
    }

    private void UpdateScoreText()
    {
        if (scoreTextUI != null)
        {
            scoreTextUI.text = "Score: " + playerScore.ToString();
        }
    }

    private void UpdateDeathsText()
    {
        if (deathsTextUI != null)
        {
            deathsTextUI.text = "Deaths: " + playerDeaths.ToString();
        }
    }




    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
    }

    

    public void BlockDestroyed()
    {
        totalBreakableBlocks--;
        if (totalBreakableBlocks <= 0)
        {
            sceneLoader.LoadNextScene(nextSceneName);
        }
    }

    public void CountBreakableBlocks()
    {
        totalBreakableBlocks++;
    }


}
