using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] private Transform _OrientationTransform;

    private RigidBody _PlayerRigidbody;

    private void Awake() 
    {
        _PlayerRigidbody = GetComponent<Rigidbody>();
    }

}
