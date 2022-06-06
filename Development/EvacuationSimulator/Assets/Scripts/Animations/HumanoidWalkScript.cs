using Assets.Scripts.Utils.AStar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidWalkScript : MonoBehaviour
{
    private Animator _animator;
    private CharacterController _controller;
    private CapsuleCollider _collider;

    private List<Node> path = null;
    private uint frameCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _controller = GetComponent<CharacterController>();
        _collider = GetComponent<CapsuleCollider>();
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        frameCount++;

        var direction = Input.GetAxis("Horizontal");
        var speed = Input.GetAxis("Vertical");
        _animator.SetFloat("Direction", direction);
        _animator.SetFloat("Speed", speed);

        //if (!executed)
        //    StartCoroutine(Move());

        //var center = new Vector3(0, 0.95f, 0);
        //var radius = 0.29f;
        //var height = 1.7f;
        //float distanceToPoints = height / 2 - radius / 2;
        //var startCapsule = transform.position + center + Vector3.up * distanceToPoints;
        //var endCapsule = transform.position + center - Vector3.up * distanceToPoints;
        //var colliders = Physics.OverlapCapsule(startCapsule, endCapsule, radius);
        //var x = colliders;

        //var directionHorizontal = Quaternion.AngleAxis(ang, transform.up) * transform.forward;
        //ang = (ang == 100) ? -100 : (ang + 2);
        //var directionDown = Quaternion.AngleAxis(0, transform.up) * Quaternion.AngleAxis(30, transform.right) * transform.forward;

        //var center = new Vector3(0, 0.86f, 0);
        //var radius = 0.25f;
        //var height = 1.83f;
        
        //float distanceToPoints = height / 2 - radius/2 ;
        //var startCapsule = transform.position + center + Vector3.up * distanceToPoints;
        //var endCapsule = transform.position + center - Vector3.up * (distanceToPoints + 0.25f);

        //Vector3 startCapsulePos = transform.position + _collider.center; /*this line gives you the capsule start position*/
        //Vector3 finalCapsulePos = transform.position - new Vector3(0f, _collider.height / 2f) + _collider.center; /*this line gives you the capsule end position*/
        ////var colliders = Physics.OverlapCapsule(startCapsule, endCapsule, radius);
        ////var x = colliders;

        //Debug.DrawRay(endCapsule, transform.forward * 2, Color.blue, 4);
        //Debug.DrawRay(transform.position + center - new Vector3(0, 0.1f, 0), directionHorizontal * 2, Color.blue, 4);
        //Physics.SphereCast(endCapsule, 0.15f, transform.forward, out var hit, 5, ~(1 << 10));
        //if (hit.collider != null)
        //    Debug.Log($"Hit at {hit.distance} on {hit.collider.gameObject.name}");
        //Debug.DrawRay(endCapsule, transform.forward * 10, Color.green, 3);

        //if (Input.GetKey(KeyCode.UpArrow))
        //{
        //    _animator.SetFloat("Speed", 2f);
        //}

        //if (Input.GetKey(KeyCode.DownArrow))
        //{
        //    _animator.SetFloat("Speed", 0f);
        //}

        if(frameCount % 1000 == 0)
        {
            path = Navigator.FindPath(transform.position, new Vector3(20.5f, 0.6f, 198.2f));
            Debug.Log($"-> {path.Count}");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log($"Exit collision with {other.gameObject.name} - {other.tag}");
    }

    private void OnTriggerEnter(Collider other)
    {
                Debug.Log($"Collision with {other.gameObject.name} - {other.tag}");
        
    }

    private void OnDrawGizmos()
    {
        if (path == null)
        {
            return;
        }

        foreach (var node in path)
        {
            if (node != null)
            {
                if (node.Walkable)
                {
                    Gizmos.color = Color.blue;
                }
                else
                {
                    Gizmos.color = Color.black;
                }
                Gizmos.DrawCube(node.Position, Vector3.one * (0.2f * 2 - 0.1f));
            }
        }
    }

    IEnumerator Move()
    {
        yield return new WaitForSeconds(5);

        //float distanceToPoints = _collider.height / 2 - _collider.radius / 2;
        //var startCapsule = transform.position + _collider.center + Vector3.up * distanceToPoints;
        //var endCapsule = transform.position + _collider.center - Vector3.up * distanceToPoints;
        var directionHorizontal = Quaternion.AngleAxis(0, transform.up) * transform.forward;
        var directionDown = Quaternion.AngleAxis(0, transform.up) * Quaternion.AngleAxis(30, transform.right) * transform.forward;

        var center = new Vector3(0, 0.95f, 0);
        var radius = 0.29f;
        var height = 1.7f;
        float distanceToPoints = height / 2 - radius;
        var startCapsule = transform.position + center + Vector3.up * distanceToPoints;
        var endCapsule = transform.position + center - Vector3.up * distanceToPoints;
        //var colliders = Physics.OverlapCapsule(startCapsule, endCapsule, radius);
        //var x = colliders;

        //Debug.DrawRay(startCapsule, directionHorizontal * 10, Color.red, 10);
        //Debug.DrawRay(endCapsule, directionHorizontal * 10, Color.red, 10);
        //Debug.DrawRay(transform.position, directionHorizontal * 10, Color.blue, 10);
        //Debug.DrawRay(transform.position + _collider.center, directionHorizontal * 10, Color.red, 10);
        //Debug.DrawRay(startCapsule, directionDown * 10, Color.green, 10);
    }


}
