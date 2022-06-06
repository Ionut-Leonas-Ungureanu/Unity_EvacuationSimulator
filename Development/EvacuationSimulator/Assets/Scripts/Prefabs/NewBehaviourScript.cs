using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update
   

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(transform.position, transform.forward * 1, Color.blue, 1);
        
        Physics.Raycast(transform.position, transform.forward, out var hit, 1, ~(1 << 10));
        
            Debug.Log($"Hit {hit.collider.gameObject.name} at {hit.distance}");
        
    }
}
