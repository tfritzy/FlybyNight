using System.Collections.Generic;
using UnityEngine;


public class Coin : MonoBehaviour
{
    public GameObject CanvasCoinPrefab;
    public GameObject CollectionEffect;
    protected bool isCollected;
    protected long Column;
    private Rigidbody2D rb;

    public void Init(long column)
    {
        this.Column = column;
        this.rb = this.GetComponent<Rigidbody2D>();
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (isCollected)
        {
            return;
        }

        if (other.CompareTag(Constants.Tags.Helicopter))
        {
            var collectionEffect = GameObject.Instantiate(CollectionEffect, this.transform.position, new Quaternion());
            collectionEffect.SetActive(true);
            collectionEffect.transform.SetParent(null);
            isCollected = true;
            Destroy(this.gameObject);
            Instantiate(
                CanvasCoinPrefab,
                Managers.Camera.WorldToScreenPoint(this.transform.position),
                CanvasCoinPrefab.transform.rotation,
                Managers.Canvas);
            Destroy(collectionEffect, 10f);
        }
    }



    public void Collect()
    {
        GameState.Player.GemCount += 1;
        this.gameObject.SetActive(false);
        Destroy(this.gameObject);
    }

    public void Reset()
    {
        this.isCollected = false;
        this.gameObject.SetActive(true);
    }
}