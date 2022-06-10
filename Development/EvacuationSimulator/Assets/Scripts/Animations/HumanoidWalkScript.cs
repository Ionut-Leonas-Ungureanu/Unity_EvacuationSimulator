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
        ang = (ang == 100) ? -100 : (ang + 10);
        Debug.DrawRay(transform.position, directionHorizontal * 2, Color.blue, 1);

        var center = new Vector3(0, 0.86f, 0);
        var radius = 0.25f;
        var height = 1.83f;

        float distanceToPoints = height / 2 - radius/2 ;
        var startCapsule = transform.position + center + Vector3.up * distanceToPoints;
        var endCapsule = transform.position + center - Vector3.up * (distanceToPoints);

        Debug.DrawRay(endCapsule, transform.forward * 2, Color.blue, 1);
        Debug.DrawRay(startCapsule, transform.forward * 2, Color.red, 1);
        //Physics.SphereCast(endCapsule, 0.15f, transform.forward, out var hit, 5, ~(1 << 10));
        if (Physics.Raycast(endCapsule, transform.forward, out var hit, 1f))
            Debug.Log($"Hit at {hit.distance} on {hit.collider.gameObject.name}");
    }
}
