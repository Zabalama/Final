using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    
    private Rigidbody2D myRigidbody;

    private Animator myAnimator;

    public Text scoreText;
    public Text livesCount;

    private int score;
    private int lives;



    [SerializeField]
    private float movementSpeed;

    private bool attack;

    private bool slide;
    private bool facingRight;
    [SerializeField]
    private Transform[] groundPoints;

    [SerializeField]    
    private float groundRadius;

    [SerializeField]
    private LayerMask whatIsGround;

    private bool isGrounded;
    private bool jump;

    [SerializeField]
    private bool airControl;
    
    [SerializeField]
    private float jumpForce;
    [SerializeField]
    private GameObject starPrefab;
    
    
        void Start()
    {
       score = 0;
       lives = 3; 
       SetScoreText();
       SetLivesCount(); 

        facingRight = true;
        myRigidbody = GetComponent<Rigidbody2D>();

        myAnimator = GetComponent<Animator>();

        
    }

    void Update()
    {
        HandleInput();

        float horizontal = Input.GetAxis("Horizontal");
        HandleMovement(horizontal);
        Flip(horizontal);

        if (Input.GetKey("escape"))
         Application.Quit();
       
    }


    void FixedUpdate()
    {
        isGrounded = IsGrounded();

        HandleAttacks();

        Resetvalues();

        HandleLayers();

    }

    private void HandleMovement(float horizontal)
    {
        if(!myAnimator.GetBool("slide") && !this.myAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
        {
            myRigidbody.velocity = new Vector2 (horizontal * movementSpeed,myRigidbody.velocity.y);
        }

         if (isGrounded && jump)
        {
            isGrounded = false;
            myRigidbody.AddForce(new Vector2(0,jumpForce));
            myAnimator.SetTrigger("jump");
        }
      
        if (slide && !this.myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Slide"))
        {
            myAnimator.SetBool("slide", true);
        }
        else if (!this.myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Slide"))
        {
            myAnimator.SetBool("slide", false);
        }

        if (myRigidbody.velocity.y < 0)
        {
            myAnimator.SetBool("land", true);
        }
        
       

        myAnimator.SetFloat("speed", Mathf.Abs (horizontal));
    }

    private void HandleAttacks()
    {
        if (attack && !this.myAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
        {
            myAnimator.SetTrigger("attack");
            myRigidbody.velocity = Vector2.zero;
        }
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            attack = true;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jump = true;
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            slide = true;
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            myAnimator.SetTrigger("throw");
            ThrowStar(0);
        }
        
        
    }
    private void Flip(float horizontal)
    {
        if (horizontal > 0 && !facingRight || horizontal < 0 && facingRight)
        {
            facingRight = !facingRight;

            Vector3 theScale = transform.localScale;

            theScale.x *= -1;

            transform.localScale = theScale;
        }
    }

       private void Resetvalues()
    {
        attack = false;

        slide = false;

        jump = false;
    }

   
       private bool IsGrounded()
    {
        if (myRigidbody.velocity.y <= 0)
        {
            foreach (Transform point in groundPoints)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(point.position, groundRadius, whatIsGround);

                for (int i = 0; i < colliders.Length; i++)
                {
                    if (colliders[i].gameObject != gameObject)
                    {
                        myAnimator.ResetTrigger("jump");
                        myAnimator.SetBool("land", false);
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private void HandleLayers()
    {
        if (!isGrounded)
        {
            myAnimator.SetLayerWeight(1, 1);
        }
        else
        {
            myAnimator.SetLayerWeight(1,0);
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Pickup"))
        {
            other.gameObject.SetActive(false);
        
        score = score + 1;
        SetScoreText();
        }
    

        if (other.gameObject.CompareTag("Enemy"))
        {
            other.gameObject.SetActive(false);
            lives = lives - 1;
            SetLivesCount();
        }
    }

   
    void SetScoreText()
    {
         scoreText.text = "Score: " + score.ToString ();
        
        if(score >= 4)
        GameController.instance.Win();
        
    }
    void SetLivesCount()
    {
         livesCount.text = "Lives: " + lives.ToString ();

        if(lives <= 0)
        {
        GameController.instance.GameOver();
        Destroy(gameObject);
        }
    }
    
    public void ThrowStar(int value)
    {
        if (facingRight)
        {
           GameObject tmp = (GameObject)Instantiate(starPrefab, transform.position,Quaternion.Euler(new Vector3(0,0,-90)));
           tmp.GetComponent<Star>().Initialize(Vector2.right);
        }
        else
        {
            GameObject tmp = (GameObject)Instantiate(starPrefab, transform.position,Quaternion.Euler(new Vector3(0,0,90)));
            tmp.GetComponent<Star>().Initialize(Vector2.left);
        }
    }
    
}
