using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deplacement : MonoBehaviour
{
    
    // Variables de vitesse
    public float vitesse = 2.0f;
    public float vitesseMarche = 2.0f;
    public float vitesseCourse = 10.0f;
    public float vitesseArriere = 1.0f;
    public float vitesseLaterale = 2.0f;

    // Variables collider et rigidBody
    public float hauteurSaut = 20.0f;
    private bool estAuSol = true;

    // Variables pour les animations
    private bool estEnMouvement = false;
    private bool estMarcheAvant = false;
    private bool estMarcheAvantCourse = false;
    private bool estMarcheArriere = false;
    private bool estDroite = false;
    private bool estGauche = false;
    private bool estEnSaut = false;
    //declaration
    private Animator animator;
    [Header("Tools Audio")]
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip walkSound;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        float deplacementHorizontal = Input.GetAxis("Horizontal") * vitesse * Time.deltaTime;
        float deplacementVertical = Input.GetAxis("Vertical") * vitesse * Time.deltaTime;
        // pour identifier les animations différentes selon les états move or not
        estEnMouvement = deplacementHorizontal != 0 || deplacementVertical != 0;
        if (estEnMouvement == true)
        {
            estEnMouvement = true;
            Debug.Log("je bouge !!!" + estEnMouvement);
        }
        else
        {
            estEnMouvement = false;
            animator.SetBool("Walk", false);
            animator.SetBool("Run", false);
            animator.SetBool("Back", false);
            //Debug.Log("je me cure le nez...." + estEnMouvement);
        }
        // marche course et marche-arrière
        if (deplacementVertical > 0 && !Input.GetKey(KeyCode.LeftControl))
        {
            audioSource.clip = walkSound;
            audioSource.play();
            estMarcheAvant = true;
            estMarcheAvantCourse = false;
            estMarcheArriere = false;
            deplacementVertical = Input.GetAxis("Vertical") * vitesseMarche * Time.deltaTime;
            Debug.Log("Déplacement en avant et je marche");
            animator.SetBool("Run", false);
            animator.SetBool("Walk", true);
            
        }
        if (deplacementVertical > 0 && Input.GetKey(KeyCode.LeftControl))
        {
            estMarcheAvant = false;
            estMarcheAvantCourse = true;
            estMarcheArriere = false;
            deplacementVertical = Input.GetAxis("Vertical") * vitesseCourse * Time.deltaTime;
            Debug.Log("Déplacement en avant et je cours !!! yahooooo");
            animator.SetBool("Run", true);
            
        }
        if (deplacementVertical < 0)
        {
            estMarcheAvant = false;
            estMarcheAvantCourse = false;
            estMarcheArriere = true;
            deplacementVertical = Input.GetAxis("Vertical") * vitesseArriere * Time.deltaTime;
            Debug.Log("Déplacement en arrière");
            animator.SetBool("Back", true);
        }
        //gauche droite...
        if (deplacementHorizontal > 0)
        {
            if (estMarcheAvant)
            {
                vitesseLaterale = vitesseMarche;
                transform.Rotate(Time.deltaTime * vitesseLaterale, 0, 0, Space.Self);
            }
            else if (estMarcheAvantCourse)
            {
                vitesseLaterale = vitesseCourse;
                transform.Rotate(Time.deltaTime * vitesseLaterale, 0, 0, Space.Self);
            }
            else if (estMarcheArriere)
            {
                vitesseLaterale = vitesseArriere;
                transform.Rotate(Time.deltaTime * vitesseLaterale, 0, 0, Space.Self);
            }
            deplacementHorizontal = Input.GetAxis("Horizontal") * vitesseLaterale * Time.deltaTime;
            Debug.Log("Déplacement à droite" + vitesseLaterale);
        }
        else if (deplacementHorizontal < 0)
        {
            if (estMarcheAvant)
            {
                vitesseLaterale = vitesseMarche;
            }
            else if (estMarcheAvantCourse)
            {
                vitesseLaterale = vitesseCourse;
            }
            else if (estMarcheArriere)
            {
                vitesseLaterale = vitesseArriere;
            }
            deplacementHorizontal = Input.GetAxis("Horizontal") * vitesseLaterale * Time.deltaTime;
            Debug.Log("Déplacement à gauche" + vitesseLaterale);
        }
        // jump necessite collider et rigid body (avec contrainte sur le rigidbody freeze rotation X et Z pour eviter de basculer)
        if (Input.GetButtonDown("Jump") && estAuSol)
        {
            GetComponent<Rigidbody>().AddForce(new Vector3(0, hauteurSaut, 0), ForceMode.Impulse);
            estAuSol = false;
            estEnSaut = true;
            Debug.Log("Je saute !!! youpi");
        }

        transform.Translate(deplacementHorizontal, 0, deplacementVertical);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Sol"))
        {
            estAuSol = true;
            estEnSaut = false;

        }
    }
}