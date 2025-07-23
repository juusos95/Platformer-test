using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    public bool grounded;
    [SerializeField] LayerMask groundLayer;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == groundLayer)
        {

        }
    }
}
