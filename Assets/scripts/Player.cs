using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Vector2 velocity;
    public float Gravity;
    public float bounceVelocity;
    public float jumpVelocity;
    public LayerMask floorMask;
    public LayerMask WallMask;
    private bool jump,walk,right_walk,left_walk;
    // Start is called before the first frame update
    public enum PlayerState
    {
        jumping,
        idle,
        walking,
        bouncing
    }
    private PlayerState playerState = PlayerState.idle;
    private bool grounded = false;
    private bool bounce = false;
    void Start()
    {
        // fall();
    }

    // Update is called once per frame
    void Update()
    {
        CheckPlayerInput();
        UpdatePlayerPostion();
        CharacterAnimationState();
    }
    

    void UpdatePlayerPostion()
    {

        Vector3 pos = transform.localPosition;
        Vector3 scale = transform.localScale;
        if (walk)
        {
            if (left_walk)
            {
                pos.x -= velocity.x * Time.deltaTime;
                scale.x = -1;
            }
            if (right_walk)
            {
                pos.x += velocity.x * Time.deltaTime;
                scale.x = 1;
            }
            pos = CheckWallRays(pos, scale.x);
        }
        if(jump && playerState != PlayerState.jumping)
        {
            playerState = PlayerState.jumping;
            velocity = new Vector2(velocity.x,jumpVelocity);
        }
        if(playerState == PlayerState.jumping)
        {
            pos.y += velocity.y * Time.deltaTime;

            velocity.y -= Gravity * Time.deltaTime;
        }
        if(bounce && playerState != PlayerState.bouncing)
        {
            playerState = PlayerState.bouncing;
            velocity = new Vector2(velocity.x,bounceVelocity);
        }
        if(playerState == PlayerState.bouncing)
        {
            pos.y += velocity.y * Time.deltaTime;
            velocity.y -= Gravity * Time.deltaTime;
        }
    
        if(velocity.y <=0)
        {
            pos = CheckfloorRays(pos);
        }

        if(velocity.y >= 0)
        {
            pos = CheckCeilingRays(pos);
        }

        transform.localPosition = pos;
        transform.localScale = scale;
    }

    void CheckPlayerInput(){
        bool input_left = Input.GetKey(KeyCode.LeftArrow);
        bool input_right = Input.GetKey(KeyCode.RightArrow);
        bool input_space = Input.GetKeyDown(KeyCode.Space);

        walk = input_left || input_right;
        right_walk = input_right && !input_left;
        left_walk = !input_right && input_left;
        jump = input_space;
    }

    void CharacterAnimationState()
    {
        if(grounded && !walk && !bounce)
        {
            GetComponent<Animator>().SetBool("isJumping", false);
            GetComponent<Animator>().SetBool("isRunning", false);
        }

        if(grounded && walk)
        {
            GetComponent<Animator>().SetBool("isJumping", false);
            GetComponent<Animator>().SetBool("isRunning", true);
        }

        if(playerState == PlayerState.jumping)
        {
            GetComponent<Animator>().SetBool("isJumping", true);
            GetComponent<Animator>().SetBool("isRunning", false);
        }
    }

    Vector3 CheckWallRays(Vector3 pos, float direction)
    {
        Vector2 originTop = new Vector2(pos.x + direction * .4f, pos.y + 1f -0.2f);
        Vector2 originMiddle = new Vector2(pos.x + direction * 4f, pos.y);
        Vector2 originBottom = new Vector2 (pos.x + direction * 4f, pos.y - 1f + 0.2f);

        RaycastHit2D walltop = Physics2D.Raycast(originTop, new Vector2(direction,0),velocity.x * Time.deltaTime, WallMask);
        RaycastHit2D wallMiddle = Physics2D.Raycast(originMiddle, new Vector2(direction,0),velocity.x * Time.deltaTime, WallMask);
        RaycastHit2D wallBottm = Physics2D.Raycast(originBottom, new Vector2(direction,0),velocity.x * Time.deltaTime, WallMask);

        if(walltop.collider != null || wallMiddle.collider != null || wallBottm.collider != null)
        {
            pos.x -= velocity.x * Time.deltaTime * direction;
        }
        return pos;
    }
    Vector3 CheckfloorRays(Vector3 pos)
    {
        Vector2 originLeft = new Vector2(pos.x - 0.5f + 0.2f, pos.y - 1f);
        Vector2 originMiddle = new Vector2(pos.x, pos.y -1f);
        Vector2 originright = new Vector2(pos.x + 0.5f - 0.2f, pos.y -1f);

        RaycastHit2D FloorLeft = Physics2D.Raycast(originLeft, Vector2.down, velocity.y * Time.deltaTime,floorMask);
        RaycastHit2D FloorMiddle = Physics2D.Raycast(originMiddle, Vector2.down, velocity.y * Time.deltaTime, floorMask);
        RaycastHit2D FloorRight = Physics2D.Raycast(originright, Vector2.down, velocity.y * Time.deltaTime, floorMask);

        if(FloorLeft.collider != null || FloorMiddle.collider != null || FloorRight.collider != null)
        {
            RaycastHit2D Hitray = FloorRight;
            if(FloorLeft)
            {
                Hitray = FloorLeft;
            }
            else if(FloorMiddle)
            {
                Hitray = FloorMiddle;
            }
            else if(FloorRight)
            {
                Hitray = FloorRight;
            }
            if (Hitray.collider.tag == "Enemy"){
                bounce = true;
                Hitray.collider.GetComponent<Enemy_AI>().crush();
            }

            playerState = PlayerState.idle;

            grounded = true;

            velocity.y = 0;

            pos.y = Hitray.collider.bounds.center.y + Hitray.collider.bounds.size.y / 2 + 1;
        }
        else
        {
            if(playerState != PlayerState.jumping)
            {
                fall();
            }
        }
        return pos;
    }

    Vector3 CheckCeilingRays(Vector3 pos)
    {
        Vector2 originLeft = new Vector2(pos.x - 0.5f + 0.2f, pos.y + 1f);
        Vector2 originMiddle = new Vector2(pos.x, pos.y + 1f);
        Vector2 originright = new Vector2(pos.x + 0.5f - 0.2f, pos.y + 1f);

        RaycastHit2D ceilLeft = Physics2D.Raycast(originLeft, Vector2.up, velocity.y * Time.deltaTime,floorMask);
        RaycastHit2D ceilMiddle = Physics2D.Raycast(originMiddle, Vector2.up, velocity.y * Time.deltaTime, floorMask);
        RaycastHit2D ceilRight = Physics2D.Raycast(originright, Vector2.up, velocity.y * Time.deltaTime, floorMask);

        if(ceilLeft.collider != null || ceilMiddle.collider != null || ceilRight.collider != null)
        {
            RaycastHit2D Hitray = ceilLeft;

            if(ceilLeft)
            {
                Hitray = ceilLeft;
            }
            else if(ceilMiddle)
            {
                Hitray = ceilMiddle;
            }
            else if(ceilRight)
            {
                Hitray = ceilRight;
            }
            if(Hitray.collider.tag == "QuestionBlock")
            {
                Hitray.collider.GetComponent<Question_Block>().QuestionBlockBounce();
            }
            pos.y = Hitray.collider.bounds.center.y - Hitray.collider.bounds.size.y / 2 - 1;

            fall();
        }
        return pos;
    }

    void fall()
    {
        velocity.y = 0;
        playerState = PlayerState.jumping;
        bounce = false;
        grounded = false;
    }
}
