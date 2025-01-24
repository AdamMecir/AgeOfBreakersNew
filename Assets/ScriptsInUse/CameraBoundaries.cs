using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EdgeCollider2D))] // Tento atribút vyžaduje, aby bol na tomto objekte pridaný komponent EdgeCollider2D
public class CameraBoundaries : MonoBehaviour
{
    private EdgeCollider2D edgeCollider; // Odkaz na komponent EdgeCollider2D

    void Start()
    {
        // Získame komponent EdgeCollider2D z tohto objektu
        edgeCollider = GetComponent<EdgeCollider2D>();

        // Nastavíme hrany (okraje) okolo kamery
        SetBoundaryEdges();
    }


    private void SetBoundaryEdges()
    {
        // Získame hlavnú kameru v scéne
        Camera cam = Camera.main;

        // Vypočítame výšku kamery na základe ortografickej veľkosti
        float height = cam.orthographicSize * 2;
        // Vypočítame šírku kamery na základe pomeru strán a výšky
        float width = height * cam.aspect;

        // Definujeme pozície jednotlivých rohov kamery v 2D priestore
        Vector2 bottomLeft = new Vector2(-width / 2, -cam.orthographicSize); // Dolný ľavý roh
        Vector2 topLeft = new Vector2(-width / 2, cam.orthographicSize);     // Horný ľavý roh
        Vector2 topRight = new Vector2(width / 2, cam.orthographicSize);     // Horný pravý roh
        Vector2 bottomRight = new Vector2(width / 2, -cam.orthographicSize); // Dolný pravý roh

        // Vytvoríme pole bodov pre kolidér, ktorý definuje okraje
        Vector2[] edgePoints = new Vector2[5];

        // Nastavíme body hrán, ktoré tvoria uzavretý rám okolo kamery
        edgePoints[0] = bottomLeft;    // Začneme dolným ľavým rohom
        edgePoints[1] = topLeft;       // Horný ľavý roh
        edgePoints[2] = topRight;      // Horný pravý roh
        edgePoints[3] = bottomRight;   // Dolný pravý roh
        edgePoints[4] = bottomLeft;    // Uzavrieme slučku, vrátime sa do dolného ľavého rohu

        // Nastavíme body EdgeCollideru, ktoré určia hrany kolíznej zóny
        edgeCollider.points = edgePoints;
    }
}
