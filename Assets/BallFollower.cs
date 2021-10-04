using UnityEngine;

public class BallFollower : MonoBehaviour
{
    [SerializeField] Transform target;

    private void Update()
    {
        transform.position = target.position;
    }
}
