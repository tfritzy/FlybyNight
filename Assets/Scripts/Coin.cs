using System.Collections.Generic;
using UnityEngine;


public class Coin : MonoBehaviour
{
    public GameObject CollectionEffect;
    protected bool isCollected;
    protected long Column;

    public void Init(long column)
    {
        this.Column = column;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Collider enter");
        if (isCollected)
        {
            return;
        }

        if (other.CompareTag(Constants.Tags.Helicopter))
        {
            Collect();
            Color color = GridManager.GetColorForColumn(Managers.Helicopter.Distance);
            var collectionEffect = GameObject.Instantiate(CollectionEffect, this.transform.position, new Quaternion());
            foreach (ParticleSystem ps in collectionEffect.GetComponentsInChildren<ParticleSystem>())
            {
                var main = ps.main;
                main.startColor = Color.white;
            }

            collectionEffect.SetActive(true);
            collectionEffect.transform.SetParent(null);
            isCollected = true;
            this.gameObject.SetActive(false);
            Destroy(collectionEffect, 10f);
        }
    }


    public void Collect()
    {
        GameState.Player.GemCount += 1;
    }

    public void Reset()
    {
        this.isCollected = false;
        this.gameObject.SetActive(true);
    }
}