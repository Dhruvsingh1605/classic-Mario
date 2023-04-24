using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Question_Block : MonoBehaviour
{
    public float bounceHeight = 0.5f;
    public float bouncespeed = 4f;
    public Sprite EmptyBlockSprite;
    public float coinMovespeed = 8f;
    public float coinMoveHeight = 1f;
    public float coinFallDistance = 1f;

    private Vector2 OriginalPosition;
    private bool CanBounce = true;
    // Start is called before the first frame update
    void Start()
    {
        OriginalPosition = transform.localPosition;
    }
    public void QuestionBlockBounce()
    {
        if(CanBounce)
        {
            CanBounce = false;
            StartCoroutine(Bounce());
        }
    }

    void ChangeSprite()
    {
        GetComponent<Animator>().enabled = false;
        GetComponent<SpriteRenderer>().sprite = EmptyBlockSprite;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void PresentCoin()
    {
        GameObject spinningCoin = (GameObject)Instantiate (Resources.Load("Prefabs/Spinning_Coin",typeof(GameObject)));

        spinningCoin.transform.SetParent(this.transform.parent);
        spinningCoin.transform.localPosition = new Vector2 (OriginalPosition.x,OriginalPosition.y+1);

        StartCoroutine(MoveCoin (spinningCoin));
    }

    IEnumerator Bounce()
    {
        ChangeSprite();
        PresentCoin();

        while (true)
        {
            transform.localPosition = new Vector2(transform.localPosition.x, transform.localPosition.y + bouncespeed * Time.deltaTime);
            if(transform.localPosition.y >= OriginalPosition.y + bounceHeight)
                break;
            yield return null;
        }
    
    while (true)
    {
        transform.localPosition = new Vector2(transform.localPosition.x, transform.localPosition.y - bouncespeed * Time.deltaTime);

        if(transform.localPosition.y <= OriginalPosition.y){
            transform.localPosition = OriginalPosition;
            break;
        }
        yield return null;
    }
    }

    IEnumerator MoveCoin (GameObject coin){
            while(true)
            {
                coin.transform.localPosition = new Vector2(coin.transform.localPosition.x,coin.transform.localPosition.y+coinMovespeed+Time.deltaTime);
                if(coin.transform.localPosition.y >= OriginalPosition.y + coinMoveHeight + 1)
                    break;

                yield return null;
            }

            while(true)
            {
                coin.transform.localPosition = new Vector2(coin.transform.localPosition.x,coin.transform.localPosition.y-coinMovespeed*Time.deltaTime);

                if(coin.transform.localPosition.y <= OriginalPosition.y + coinFallDistance + 1)
                {
                    Destroy(coin.gameObject);
                    break;
                }
                yield return null;
            }
    }
}
