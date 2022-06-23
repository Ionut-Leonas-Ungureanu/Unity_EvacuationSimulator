using Assets.Scripts.Utils.AStar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidWalkScript : MonoBehaviour
{
    private Animator _animator;
    private float ang;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var direction = Input.GetAxis("Horizontal");
        var speed = Input.GetAxis("Vertical");
        _animator.SetFloat("Direction", direction);
        _animator.SetFloat("Speed", speed);

        var directionHorizontal = Quaternion.AngleAxis(ang, transform.up) * transform.forward;
        ang = (ang == 110) ? -110 : (ang + 10);

        var center = new Vector3(0, 0.86f, 0);
        var radius = 0.25f;
        var height = 1.83f;

        float distanceToPoints = height / 2 - radius/2 ;
        var startCapsule = transform.position + center + Vector3.up * distanceToPoints;
        var endCapsule = transform.position + center - Vector3.up * (distanceToPoints);

        
        
        //Physics.SphereCast(endCapsule, 0.15f, transform.forward, out var hit, 5, ~(1 << 10));
        

        var center1 = new Vector3(0, 0.95f, 0);
        var height1 = 1.7f;
        var _radiusCapsuleCollider = 0.29f;
        var distanceToPoints1 = height1 / 2 - _radiusCapsuleCollider;
        var _startCapsuleColliderConstant = center1 + Vector3.up * distanceToPoints1;
        var _endCapsuleColliderConstant = center1 - Vector3.up * distanceToPoints1;
        var startCapsule1 = transform.position + _startCapsuleColliderConstant;
        var endCapsule1 = transform.position + _endCapsuleColliderConstant;

        var _raysStartingPoint = new Vector3(0, 0.86f, 0);
        var _raysMiddlePoint = _raysStartingPoint - new Vector3(0, 0.1f, 0);
        var middlePoint = transform.position + _raysMiddlePoint;

        if (Physics.Raycast(middlePoint, transform.forward, out var hit, 1f))
            Debug.Log($"Hit at {hit.distance} on {hit.collider.gameObject.name}");

        Debug.DrawRay(middlePoint, directionHorizontal * 1, Color.blue, 1);

        Debug.DrawRay(endCapsule1, transform.forward * 2, Color.blue, 1);
        Debug.DrawRay(startCapsule1, transform.forward * 2, Color.red, 1);

        var colliders = Physics.OverlapCapsule(startCapsule1, endCapsule1, _radiusCapsuleCollider);

        // Get collision
        foreach (var collider in colliders)
        {
            if (collider.name != gameObject.name)
            {
                Debug.Log($"hit {collider.name}");
            }
        }
    }
}
