using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
    public GameObject CollectionEffect;
    private bool isCollected;
    private long Column;
    private Vector3 startPos;

    public void Init(long column)
    {
        this.Column = column;
    }

    void Start()
    {
        startPos = this.transform.position;
    }

    void Update()
    {
        this.transform.position = startPos + Vector3.up * Mathf.Sin(Time.time) * .07f;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
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
        Managers.Helicopter.AddFuel(.33f);
    }

    public void Reset()
    {
        if (GameState.Player.GetFirePosOfHighestRegion() > Column)
        {
            return;
        }

        this.isCollected = false;
        this.gameObject.SetActive(true);
    }
}