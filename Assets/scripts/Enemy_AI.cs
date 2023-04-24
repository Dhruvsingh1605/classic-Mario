using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_AI : MonoBehaviour
{
    public float gravity;
    public Vector2 velocity;
    public bool isWalkingLeft = true;
    private bool grounded = true;
    public LayerMask Floormask;
    public LayerMask Wallmask;
    private bool shouldDie = false;
    private float DeathTimer = 0;
    public float timeBeforeDestroy = 1.0f;

    private enum EnemyState
    {
        walking,
        falling,
        dead
    }
    
    private EnemyState state = EnemyState.falling;
    // Start is called before the first frame update
    void Start()
    {
        enabled = false;
        fall ();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateEnemyPosition ();
        checkCrushed ();
    }

    public void crush()
    {
        state = EnemyState.dead;

        GetComponent<Animator>().SetBool("isCrushed", true);
        GetComponent<Collider2D>().enabled = false;

        shouldDie = true;
    }

    void checkCrushed()
    {
        if(shouldDie)
        {
            if(DeathTimer<=timeBeforeDestroy)
            {
                DeathTimer += Time.deltaTime;
            }
            else{
                shouldDie = false;
                Destroy (this.gameObject);
            }
        }
    }

    void UpdateEnemyPosition()
    {
        if (state != EnemyState.dead)
        {
            Vector3 pos = transform.localPosition;
            Vector3 scale = transform.localScale;

            if(state == EnemyState.falling)
            {
                pos.y  += velocity.y * Time.deltaTime;
                velocity.y -= gravity   * Time.deltaTime;
            }
            if(state == EnemyState.walking)
            {
                if(isWalkingLeft)
                {
                    pos.x -= velocity.x * Time.deltaTime;
                    scale.x = -1;
                }
                else
                {
                    pos.x += velocity.x * Time.deltaTime;
                    scale.x = 1;
                }
            }
            if(velocity.y <= 0)
                pos = CheckGround(pos);
                
                CheckWalls (pos, scale.x);
            
            transform.localPosition = pos;
            transform.localScale = scale;
        }
    }
    Vector3 CheckGround(Vector3 pos)
    {
        Vector2 OriginLeft = new Vector2(pos.x - 0.5f + 0.2f,pos.y - 0.5f);
        Vector2 OriginMIddle = new Vector2(pos.x,pos.y - 0.5f);
        Vector2 OriginRight = new Vector2(pos.x + 0.5f + 0.2f, pos.y - 0.5f);

        RaycastHit2D groundLeft = Physics2D.Raycast(OriginLeft, Vector2.down, velocity.y * Time.deltaTime,Floormask);
        RaycastHit2D groundMiddle = Physics2D.Raycast(OriginMIddle, Vector2.down, velocity.y * Time.deltaTime, Floormask);
        RaycastHit2D groundRight = Physics2D.Raycast(OriginRight, Vector2.down, velocity.y * Time.deltaTime, Floormask);

        if(groundLeft.collider != null || groundMiddle.collider != null || groundRight.collider != null)
        {
            RaycastHit2D Hitray = groundLeft;

            if(groundLeft)
            {
                Hitray = groundLeft;
            }
            else if(groundMiddle)
            {
                Hitray = groundMiddle;
            }
            else if(groundRight)
            {
                Hitray = groundRight;
            }
            if(Hitray.collider.tag == "Player")
            {
                Application.LoadLevel("GameOver");
            }
            pos.y = Hitray.collider.bounds.center.y + Hitray.collider.bounds.size.y / 2 + 0.5f;

            grounded = true;
            velocity.y = 0;
            state = EnemyState.walking;
        }
        else{
            if(state != EnemyState.falling)
            {
                fall();
            }
        }
        return pos;
    }
    void CheckWalls(Vector3 pos, float direction)
    {
        Vector2 originTop = new Vector2 (pos.x + direction * 0.4f, pos.y + 0.5f - 0.2f);
        Vector2 originMiddle = new Vector2(pos.x + direction * 0.4f, pos.y);
        Vector2 originBottom = new Vector2(pos.x + direction * 0.4f, pos.y - 0.5f + 0.2f);

        RaycastHit2D wallTop = Physics2D.Raycast (originTop, new Vector2 (direction, 0), velocity.x * Time.deltaTime, Wallmask);
        RaycastHit2D wallMiddle = Physics2D.Raycast (originMiddle, new Vector2 (direction, 0), velocity.x * Time.deltaTime, Wallmask);
        RaycastHit2D wallBottom = Physics2D.Raycast (originBottom, new Vector2 (direction, 0), velocity.x * Time.deltaTime, Wallmask);

        if(wallTop.collider != null || wallMiddle.collider != null || wallBottom.collider != null)
        {
            RaycastHit2D Hitray = wallTop;

            if(wallTop)
            {
                Hitray = wallTop;
            }
            else if(wallMiddle)
            {
                Hitray = wallMiddle;
            }
            else if(wallBottom)
            {
                Hitray = wallBottom;
            }
            if(Hitray.collider.tag == "Player")
            {
                Application.LoadLevel("GameOver");
            }
            isWalkingLeft = !isWalkingLeft;
        }
    }

    void fall()
    {
        velocity.y = 0;
        state = EnemyState.falling;
        grounded = false;
    }

    private void OnBecameVisible() {
        enabled = true;
    }
}
