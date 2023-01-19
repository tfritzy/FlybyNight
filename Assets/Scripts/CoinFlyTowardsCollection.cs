using UnityEngine;

public class CoinFlyTowardsCollection : MonoBehaviour
{
    void FixedUpdate()
    {
        FlyTowardsTarget();
    }

    private void FlyTowardsTarget()
    {
        Vector3 delta = Managers.CollectionTargetPos.position - this.transform.position;

        this.transform.position += delta.normalized * 15f;

        if (delta.magnitude < 10f)
        {
            Collect();
        }
    }

    private void Collect()
    {
        GameState.Player.GemCount += 1;
        Destroy(this.gameObject);
    }
}