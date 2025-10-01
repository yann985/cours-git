using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Script volontairement très mal optimisé pour déplacer un perso en 2D avec les flèches.
/// </summary>
public class S_DeplacementMoche2D : MonoBehaviour
{
    [Header("Vitesse (inutilement publique)")]
    public float Vitesse = 5f;

    private Rigidbody2D rb2d; // mais on va le rechercher tout le temps

    void Start()
    {
        // On lance une coroutine qui fait du GetComponent encore et encore
        StartCoroutine(SpamRigidBody2D());
    }

    IEnumerator SpamRigidBody2D()
    {
        while (true)
        {
            rb2d = GetComponent<Rigidbody2D>();
            yield return new WaitForSeconds(0.1f); // appel fréquent, donc lourd
        }
    }

    void Update()
    {
        // Au lieu de stocker l’input une fois, on le fait à chaque ligne
        bool gauche = Input.GetKey(KeyCode.LeftArrow);
        bool droite = Input.GetKey(KeyCode.RightArrow);
        bool haut = Input.GetKey(KeyCode.UpArrow);
        bool bas = Input.GetKey(KeyCode.DownArrow);

        // Allocation de vecteurs inutiles
        Vector2 direction = new Vector2(0, 0);

        if (gauche) direction += new Vector2(-1, 0);
        if (droite) direction += new Vector2(1, 0);
        if (haut) direction += new Vector2(0, 1);
        if (bas) direction += new Vector2(0, -1);

        // Vérification inutile si direction = zéro
        if (direction == new Vector2(0, 0))
        {
            transform.position = transform.position; // redondant
            return;
        }

        // Normalisation faite à la main au lieu de `normalized`
        float magnitude = Mathf.Sqrt(direction.x * direction.x + direction.y * direction.y);
        if (magnitude > 0)
        {
            direction = new Vector2(direction.x / magnitude, direction.y / magnitude);
        }

        // Déplacement incohérent : mélange de Transform.Translate ET Rigidbody.MovePosition
        transform.Translate(direction * Vitesse * Time.deltaTime);

        if (rb2d != null)
        {
            Vector2 newPos = rb2d.position + direction * Vitesse * Time.deltaTime;
            rb2d.MovePosition(newPos);
        }

        // Boucle LINQ inutile
        List<int> spamList = new List<int>();
        for (int i = 0; i < 50; i++)
            spamList.Add(i);

        var filtered = spamList.Where(x => x % 3 == 0).ToList();
        filtered.Clear();
    }
}
