using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(requiredComponent: typeof(Rigidbody2D))]

public class TapController : MonoBehaviour
{
    public delegate void PlayerDelegate();
    public static event PlayerDelegate OnPlayerDied;
    public static event PlayerDelegate OnPlayerScored;

    public float tapForce = 10;
    public float tiltSmooth = 5;
    public Vector3 startPos;

    public int curHealth;
    public int maxHealth = 3;

    public float knockDur = 0.02f;
    public float knockPowX = 100;
    public float knockPowY = 50;

    public AudioSource tapAudio;
    public AudioSource scoreAudio;
    public AudioSource deadAudio;
    public AudioSource hurtAudio;
    public AudioSource healthAudio;
    public AudioSource superhealthAudio;

    Rigidbody2D rigidbody;
    Quaternion downRotation;
    Quaternion forwardRotation;

    GameManager game;

    private void Start()
    {
        curHealth = maxHealth;

        rigidbody = GetComponent<Rigidbody2D>();
        downRotation = Quaternion.Euler(0, 0, -90);
        forwardRotation = Quaternion.Euler(0, 0, 35);
        game = GameManager.Instance;
        rigidbody.simulated = false;
        
    }

    private void OnEnable()
    {
        GameManager.OnGameStarted += OnGameStarted;
        GameManager.OnGameOverConfirmed += OnGameOverConfirmed;
    }

    private void OnDisable()
    {
        GameManager.OnGameStarted -= OnGameStarted;
        GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;
    }

    void OnGameStarted()
    {
        rigidbody.velocity = Vector3.zero;
        rigidbody.simulated = true;
    }

    void OnGameOverConfirmed()
    {
        transform.localPosition = startPos;
        transform.rotation = Quaternion.identity;

    }

    private void Update()
    {
        if (game.GameOver)
        {
            return;

        }

        if(Input.GetMouseButtonDown(0))
        {
            tapAudio.Play();
            //Time.timeScale += 1;
            transform.rotation = forwardRotation;
            rigidbody.velocity = Vector3.zero;
            rigidbody.AddForce(Vector2.up * tapForce, ForceMode2D.Force);
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, downRotation, tiltSmooth * Time.deltaTime);
   
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.tag == "ScoreZone") //Score
        {
            //register score event
            OnPlayerScored(); // event sent to GameManager;
            //play sound
            scoreAudio.Play();

        }

        if (collision.gameObject.tag == "HealthZone") // Cherry
        {

            if (curHealth < maxHealth)
            {
                curHealth += 1;
                healthAudio.Play(); //play sound
                collision.gameObject.SetActive(false); //hide after pickup

                StartCoroutine(Unhide(5f)); //unhide after 5 seconds
            }
           
            

        }

        if (collision.gameObject.tag == "SuperHealthZone") //Supercherry
        {

            if(curHealth < maxHealth)
            {
                curHealth = maxHealth;
                //play sound
                superhealthAudio.Play();
                collision.gameObject.SetActive(false); //hide after pickup

                StartCoroutine(Unhide(5f)); // Unhide after 5 seconds
            }
  
        }

        IEnumerator Unhide(float time)
        {
            yield return new WaitForSeconds(time);
            collision.gameObject.SetActive(true);
        }



        if (collision.gameObject.tag == "DeadZoneTop") //Top Pipe
        {
            curHealth -= 1;
            // play sound and animation

            GetComponent<Animation>().Play("knockback");
            hurtAudio.Play();

            //knockback

            StartCoroutine(Knockback(knockDur, -knockPowX, -knockPowY));
            



            if (curHealth <= 0)
            {
                rigidbody.simulated = false;
                //register a dead event
                OnPlayerDied(); // event sent to GameManager
                                //play sound
                deadAudio.Play();

        
            }
            
        }

        if (collision.gameObject.tag == "DeadZoneBottom") //BottomPipe
        {
            curHealth -= 1;
            // play sound and animation

            GetComponent<Animation>().Play("knockback");
            hurtAudio.Play();

            //knockback

            StartCoroutine(Knockback(knockDur, -knockPowX, knockPowY*2.5f));




            if (curHealth <= 0)
            {
                rigidbody.simulated = false;
                //register a dead event
                OnPlayerDied(); // event sent to GameManager
                //play sound
                deadAudio.Play();


            }

        }

        if (collision.gameObject.tag == "GroundZone") //Ground
        {

            curHealth = 0;
            GetComponent<Animation>().Play("knockback");

            rigidbody.simulated = false;
            //register a dead event
            OnPlayerDied(); // event sent to GameManager
            //play sound
            deadAudio.Play();

        }

    }

    public IEnumerator Knockback(float knockDur, float knockPowX, float knockPowY)
    {
        float timer = 0;

        while(knockDur > timer)
        {
            timer += Time.deltaTime;

            rigidbody.AddForce(new Vector2(knockPowX, knockPowY));

        }

        yield return 0;
    }
}
