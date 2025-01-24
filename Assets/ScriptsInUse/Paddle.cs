using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paddle : MonoBehaviour
{
    [SerializeField] float screenWidthInUnits = 16f;
    [SerializeField] float minX = 1f;
    [SerializeField] float maxX = 15f;
    [SerializeField] float paddleSpeed = 10f; // Rýchlosť pohybu pádla

    private bool isPaused = true; // Premenná, ktorá sleduje, či je hra pozastavená (kurzor viditeľný)
                                  //cached references
    GameStatus theGameStatus;
    Ball theBall;
    void Start()
    {
        // Na začiatku je kurzor voľný (nie je zamknutý v strede obrazovky) a je viditeľný
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        theGameStatus = FindObjectOfType<GameStatus>();
        theBall = FindObjectOfType<Ball>();
    }
    private float GetXPos()
    {
        if (theGameStatus.IsAutoPlayEnabled())
        {
            return theBall.transform.position.x;
        }
        else
        {
            return Input.mousePosition.x / Screen.width * screenWidthInUnits;
        }
    }
    void Update()
    {
        Vector2 paddlePos = new Vector2(transform.position.x, transform.position.y);
        paddlePos.x = Mathf.Clamp(GetXPos(), minX, maxX);
        transform.position = paddlePos;
        // Ak je hra pozastavená, neumožníme pádlu pohyb, dokým hráč neklikne myšou
        if (isPaused)
        {
            // Ak hráč klikne ľavým tlačidlom myši (Mouse Button 0)
            if (Input.GetMouseButtonDown(0))
            {
                // Skryjeme kurzor a zamkneme ho do stredu obrazovky
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                isPaused = false; // Hra pokračuje
            }
        }
        else
        {
            // Ak je kurzor zamknutý a neviditeľný, umožníme pádlu pohyb
            if (Cursor.lockState == CursorLockMode.Locked && !Cursor.visible)
            {
                // Získame horizontálny pohyb myši a upravíme ho podľa rýchlosti pádla
                float deltaX = Input.GetAxis("Mouse X") * paddleSpeed * Time.deltaTime;
                //Debug.Log(Input.GetAxis("Mouse X"));
                // Vypočítame novú pozíciu X pre pádlo a obmedzíme ju na minX a maxX
                float newXPos = Mathf.Clamp(transform.position.x + deltaX, minX, maxX);
                Debug.Log(newXPos);
                // Nastavíme novú pozíciu pádla
                paddlePos = new Vector2(newXPos, transform.position.y);
                transform.position = paddlePos;
            }

            // Ak hráč stlačí klávesu Escape, zastaví hru (kurzor sa znovu zobrazí)
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                // Uvoľníme kurzor a spravíme ho viditeľným
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                isPaused = true; // Hra je znovu pozastavená
            }
        }
    }
}
