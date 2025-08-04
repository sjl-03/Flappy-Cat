using UnityEngine;

public class CatScript : MonoBehaviour
{
    public LogicScript logic;
    public Rigidbody2D myRigidbody;
    public float flatStrength = 10f;
    public bool catIsAlive = true; 
    private float deadZone = -20;
    private bool flap = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            flap = true;
        }
        else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            flap = true;
        }
        if (flap && catIsAlive)
            {
                myRigidbody.linearVelocity = Vector2.up * flatStrength; // new Vector2(0, 1)
                flap = false;
            }

        if (transform.position.y < deadZone)
        {
            logic.gameOver();
            catIsAlive = false;
        }

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        logic.gameOver();
        catIsAlive = false;
    }
}
