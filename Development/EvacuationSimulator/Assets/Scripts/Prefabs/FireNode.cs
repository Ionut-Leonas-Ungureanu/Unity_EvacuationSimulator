using UnityEngine;

public class FireNode : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        var radius = gameObject.GetComponent<SphereCollider>().radius;
        var newPosition = gameObject.transform.position + Vector3.forward * radius;
        if(Physics.CheckSphere(newPosition, radius))
        {
            Instantiate(gameObject, newPosition, gameObject.transform.rotation);
        }
        newPosition = gameObject.transform.position + Vector3.back * gameObject.GetComponent<SphereCollider>().radius;
        if (Physics.CheckSphere(newPosition, radius))
        {
            Instantiate(gameObject, newPosition, gameObject.transform.rotation);
        }
        newPosition = gameObject.transform.position + Vector3.left * gameObject.GetComponent<SphereCollider>().radius;
        if (Physics.CheckSphere(newPosition, radius))
        {
            Instantiate(gameObject, newPosition, gameObject.transform.rotation);
        }
        newPosition = gameObject.transform.position + Vector3.right * gameObject.GetComponent<SphereCollider>().radius;
        if (Physics.CheckSphere(newPosition, radius))
        {
            Instantiate(gameObject, newPosition, gameObject.transform.rotation);
        }
        newPosition = gameObject.transform.position + Vector3.up * gameObject.GetComponent<SphereCollider>().radius;
        if (Physics.CheckSphere(newPosition, radius))
        {
            Instantiate(gameObject, newPosition, gameObject.transform.rotation);
        }
        newPosition = gameObject.transform.position + Vector3.down * gameObject.GetComponent<SphereCollider>().radius;
        if (Physics.CheckSphere(newPosition, radius))
        {
            Instantiate(gameObject, newPosition, gameObject.transform.rotation);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name == "Terrain")
        {
            Destroy(gameObject);
        }
    }

}
