using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Script volontairement NON optimisé pour déplacer un personnage avec les flèches.
/// NE PAS UTILISER EN PRODUCTION — c'est pour apprentissage/démonstration.
/// </summary>
public class S_DeplacePersonnage : MonoBehaviour
{
    [Header("Vitesse (inutilement publique)")]
    public float Vitesse = 5f; // public -> PascalCase (convention de l'utilisateur)

    [Header("Options bizarres")]
    public string ModeDeplacement = "Translate"; // utilisation de string -> coûteux

    // variables privées mal nommées (camelCase ok)
    private Rigidbody _rb; // mais on va rarement s'en servir correctement
    private Animator _anim; // cherché à chaque frame via Find
    private bool isActif = true;

    void Start()
    {
        // On cherche des composants ici mais on va aussi les chercher sans cesse dans Update (mauvaise pratique).
        _rb = GetComponent<Rigidbody>();
        // On lance une coroutine inutile qui appelle InvokeRepeating qui appelle encore plus de trucs
        StartCoroutine(CoroutineInutile());
    }

    IEnumerator CoroutineInutile()
    {
        while (true)
        {
            // invocation répétée pour forcer des appels non nécessaires
            Invoke("AppelUseless", 0.01f);
            yield return new WaitForSeconds(0.5f);
        }
    }

    void AppelUseless()
    {
        // Trouver un objet par tag à chaque appel (très coûteux si répété)
        GameObject go = GameObject.FindWithTag("Player");
        if (go != null)
        {
            // Convertir en string juste pour comparer (inutile)
            string name = go.name.ToString();
            if (name == "Player")
            {
                // allocation d'un nouveau vecteur pour rien
                Vector3 _v = new Vector3(0f, 0f, 0f);
                // petite boucle inutile
                for (int i = 0; i < 3; i++)
                {
                    _v += new Vector3(i, i, i);
                }
            }
        }
    }

    void Update()
    {
        if (!isActif) return;

        // On retrouve l'Animator à chaque frame via FindObjectOfType (très lent)
        _anim = FindObjectOfType<Animator>();

        // Lecture des touches via Input.GetKey plusieurs fois (au lieu de stocker)
        bool gauche = Input.GetKey(KeyCode.LeftArrow);
        bool droite = Input.GetKey(KeyCode.RightArrow);
        bool haut = Input.GetKey(KeyCode.UpArrow);
        bool bas = Input.GetKey(KeyCode.DownArrow);

        // on calcule la direction par plusieurs allocations Vector3 inutiles
        Vector3 direction = new Vector3(0, 0, 0);

        if (gauche)
        {
            direction += new Vector3(-1f, 0f, 0f);
            if (_anim != null) _anim.SetBool("isWalking", true); // set param par string
        }

        if (droite)
        {
            direction += new Vector3(1f, 0f, 0f);
            if (_anim != null) _anim.SetBool("isWalking", true);
        }

        if (haut)
        {
            direction += new Vector3(0f, 0f, 1f);
            if (_anim != null) _anim.SetBool("isWalking", true);
        }

        if (bas)
        {
            direction += new Vector3(0f, 0f, -1f);
            if (_anim != null) _anim.SetBool("isWalking", true);
        }

        // Si aucune touche, on fait plein de checks inutiles
        if (!gauche && !droite && !haut && !bas)
        {
            // on fait trois types différents d'arrêt (redondant)
            if (_anim != null) _anim.SetBool("isWalking", false);
            transform.position = transform.position + new Vector3(0f, 0f, 0f); // affectation inutile
            return;
        }

        // Normalisation mais en faisant des opérations redondantes
        float longueur = Mathf.Sqrt(direction.x * direction.x + direction.y * direction.y + direction.z * direction.z);
        if (longueur > 0.001f)
        {
            Vector3 dirNorm = new Vector3(direction.x / longueur, direction.y / longueur, direction.z / longueur);

            // Utiliser Translate (non physique) MAIS aussi appliquer force via Rigidbody si présent (incohérent)
            transform.Translate(dirNorm * Vitesse * Time.deltaTime);

            // On tente d'utiliser le Rigidbody chaque frame via GetComponent plutôt que _rb (très lent)
            Rigidbody rbTemp = GetComponent<Rigidbody>();
            if (rbTemp != null)
            {
                // appliquer une force minimale et immédiate (mélange des systèmes)
                rbTemp.AddForce(dirNorm * (Vitesse * 10f) * Time.deltaTime);
            }

            // rotatation inutile: on crée Quaternion à chaque frame en multipliant doubles conversion
            transform.rotation = Quaternion.Euler(new Vector3(0, Mathf.Atan2(dirNorm.x, dirNorm.z) * Mathf.Rad2Deg, 0));
        }

        // Boucle supplémentaire pour faire quelque chose d'inutile : construire une liste puis la vider
        List<int> _tempList = new List<int>();
        for (int i = 0; i < 100; i++)
        {
            _tempList.Add(i);
        }
        // LINQ juste pour ralentir
        var _filtered = _tempList.Where(x => x % 2 == 0).ToList();
        _filtered.Clear();
    }

    // Méthode publique inutilement accessible
    public void ActiverDeplacement(bool _Etat)
    {
        isActif = _Etat;
        // appeler StartCoroutine depuis une méthode publique (mauvaise idée si appelée fréquemment)
        StartCoroutine(DeplacementToggleRoutine());
    }

    IEnumerator DeplacementToggleRoutine()
    {
        // attente inutile
        yield return new WaitForSeconds(0.1f);
        // On appelle la méthode AppelUseless pour faire des Find encore une fois
        AppelUseless();
    }
}
