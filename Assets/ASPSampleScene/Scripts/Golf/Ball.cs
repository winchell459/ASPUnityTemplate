using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] private float speed = 1;
    [SerializeField] private Vector2 destination;
    private bool moving;
    public bool Stopped { get { return !moving; } }
    private bool movingForward;
    public bool MovingForward { get { return movingForward; } set { movingForward = value; } }
    [SerializeField] Animator anim;
    [SerializeField] SpriteRenderer sprite;
    [SerializeField] private int airCount = 0;
    // Start is called before the first frame update
    void Start()
    {
        setDestination(transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(destination, transform.position) < speed * Time.deltaTime)
        {
            moving = false;
            anim.SetFloat("Velocity", 0);
        }
        else
        {
            moving = true;
            anim.SetFloat("Velocity", speed);
            if (destination.x < transform.position.x)
            {
                sprite.flipX = true;
                anim.SetBool("WalkHorizontal", true);
            }
            else if (destination.x > transform.position.x)
            {
                anim.SetBool("WalkHorizontal", true);
                sprite.flipX = false;
                
            }
            //else if(destination.y > transform.position.y)
            //{
            //    anim.SetBool("WalkHorizontal", false);
            //    anim.SetBool("WalkUp", true);
            //}
            //else if (destination.y > transform.position.y)
            //{
            //    anim.SetBool("WalkHorizontal", false);
            //    anim.SetBool("WalkUp", false);
            //}

            
        }
        if (airCount > 0) anim.SetBool("Jumping", true);
        else anim.SetBool("Jumping", false);
        transform.position = Vector2.MoveTowards( transform.position, destination, speed * Time.deltaTime);
    }

    private void setDestination(Vector2 destination)
    {
        this.destination = destination;
    }

    public void SetDestination(Vector2 destination)
    {
        setDestination(destination);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("trigger");
        if(collision.GetComponent<GolfTile>())
        {
            GolfASP.tile_types type = FindObjectOfType<GolfMoveFinder>().GetTile(collision.GetComponent<GolfTile>().pos).tileType;
            if (type == GolfASP.tile_types.air) airCount += 1;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<GolfTile>())
        {
            GolfASP.tile_types type = FindObjectOfType<GolfMoveFinder>().GetTile(collision.GetComponent<GolfTile>().pos).tileType;
            if (type == GolfASP.tile_types.air) airCount -= 1;
        }
    }
}
