using UnityEngine;

public class JumpPad : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Player player = other.gameObject.GetComponent<Player>();
        if (player != null)
        {
            player.ApplyForce(transform.forward * 20.0f);
        }
    }
}
