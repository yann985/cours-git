using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script volontairement très mal optimisé pour faire sauter un perso en 2D avec la touche espace.
/// </summary>
public class S_SautMoche2D : MonoBehaviour
{
    [Header("Force de saut (inutilement publique)")]
    public float ForceSaut = 10f;

    [Header("Gravité supplémentaire inutile")]
    public float GraviteBidon = 2f;

    private Rigidbody2D rb2d; // mais on va quand même GetComponent tout le temps
    private bool isGrounded = true; // géré de manière ridicule

    void Start()
    {
        // Lance une coroutine qui spam GetComponent (mauvaise pratique)
        StartCoroutine(RigidbodySpam());
    }

    IEnumerator RigidbodySpam()
    {
        while (true)
        {
            rb2d = GetComponent<Rigidbody2D>(); // super lent si souvent répété
            yield return new WaitForSeconds(0.2f);
        }
    }

    void Update()
    {
        // Vérifie deux fois la touche espace (inutile et contradictoire)
        if (Input.GetKey(KeyCode.Space) && Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            if (rb2d != null)
            {
                // On reset la vitesse Y n'importe comment
                rb2d.velocity = new Vector2(rb2d.velocity.x, 0);

                // On applique plein de petites forces au lieu d'une seule
                for (int i = 0; i < 20; i++)
                {
                    rb2d.AddForce(Vector2.up * (ForceSaut / 20f), ForceMode2D.Impulse);
                }
            }

            // Lance une coroutine débile pour "désactiver" le grounded
            StartCoroutine(FauxGrounded());
        }

        // Ajoute une gravité supplémentaire n'importe comment chaque frame
        if (rb2d != null)
        {
            rb2d.AddForce(Vector2.down * GraviteBidon);
        }
    }

    IEnumerator FauxGrounded()
    {
        isGrounded = false;
        // On attend 1 seconde pile au lieu de vérifier le sol
        yield return new WaitForSeconds(1f);
        isGrounded = true;
    }
}
