using System.Collections;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Block : MonoBehaviour
{
    [SerializeField] private AudioClip breakSound;
    [SerializeField] private GameManager gamemanager;
    [SerializeField] int maxHits;
    [SerializeField] private ParticleSystem particlePrefab;
    [SerializeField] private ParticleSystem particleCoinPrefab;

    private SpriteRenderer spriteBackgroundRenderer;
    int timesHit;

    GameStatus theGameStatus;
    private void Start()
    {
        theGameStatus = FindObjectOfType<GameStatus>();
        Transform spriteBackgroundTransform = transform.Find("spritebackground");
        if (spriteBackgroundTransform != null)
        {
            spriteBackgroundRenderer = spriteBackgroundTransform.GetComponent<SpriteRenderer>();
            if (spriteBackgroundRenderer != null && gameObject.CompareTag("CoinBlock"))
            {
                spriteBackgroundRenderer.color = Color.red;
            }
            else
            {
            }
        }
        else
        {
        }

        if (gamemanager == null)
        {
            gamemanager = FindObjectOfType<GameManager>();
        }



        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
             BlockDestroyObjectPooling(collision);
        
        
        
    }

    
    private void BlockDestroyObjectPooling(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            if (gameObject.CompareTag("CoinBlock"))
            {
                timesHit++;
                UpdateCoinBlockColor();

                if (timesHit >= maxHits - 1)
                {
                    gameObject.tag = "OnOneHitBlock";
                }
            }
            else
            {
                AudioSource.PlayClipAtPoint(breakSound, Camera.main.transform.position);
                if(gameObject.tag == "OnOneHitBlock")
                {
                    if(theGameStatus.ObjectPoolEnable == true)
                    {
                        TriggerSparklesCoinVFX();
                        spriteBackgroundRenderer.color = Color.red;
                        gameObject.tag = "CoinBlock";
                    }
                    else
                    {
                        ParticleSystem newParticle = Instantiate(particleCoinPrefab);
                        newParticle.transform.position = transform.position;
                    }
                    
                }
                else
                {
                    if (theGameStatus.ObjectPoolEnable == true)
                    {
                        TriggerSparklesVFX();
                    }
                    else
                    {
                        ParticleSystem newParticle = Instantiate(particlePrefab);
                        newParticle.transform.position = transform.position;
                    }
                }

                gamemanager.BlockDestroyed();
                FindObjectOfType<GameStatus>().AddToScore();
                if (theGameStatus.ObjectPoolEnable == true)
                {
                    gameObject.SetActive(false);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    private void UpdateCoinBlockColor()
    {
        if (spriteBackgroundRenderer != null)
        {
            float colorProgress = (float)timesHit / (maxHits - 1);
            spriteBackgroundRenderer.color = Color.Lerp(Color.red, Color.green, colorProgress);
        }
    }

    private void TriggerSparklesVFX()
    {
        if (gamemanager != null)
        {
            ParticleSystem particle = gamemanager.GetParticle();
            if (particle != null)
            {
                particle.transform.position = transform.position;
                particle.Play();

                StartCoroutine(ReturnParticleToPool(particle, particle.main.duration));
            }
        }
    }

    private void TriggerSparklesCoinVFX()
    {
        if (gamemanager != null)
        {
            ParticleSystem particle = gamemanager.GetCoinParticle();
            if (particle != null)
            {
                particle.transform.position = transform.position;
                particle.Play();

                StartCoroutine(ReturnParticleCoinToPool(particle, particle.main.duration));
            }
        }
    }
    private IEnumerator ReturnParticleToPool(ParticleSystem particle, float delay)
    {
        yield return new WaitForSeconds(delay);
        gamemanager.ReturnParticle(particle);
    }
    private IEnumerator ReturnParticleCoinToPool(ParticleSystem particle, float delay)
    {
        yield return new WaitForSeconds(delay);
        gamemanager.ReturnParticleCoin(particle);
    }
}
