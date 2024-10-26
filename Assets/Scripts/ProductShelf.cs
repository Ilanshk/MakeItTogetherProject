using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductShelf : MonoBehaviour {
    private List<string> productsOnShelf;
    private Rigidbody productRigidBody;
    private Transform productGrabPointTransform;
   [SerializeField] private Transform productOnTopOfShelf;

    public void Awake() {
        productRigidBody = GetComponentInChildren<Rigidbody>();
        Debug.Log(productRigidBody);
    }
    

    public void GrabProduct(Transform productGrabPointTransform,Transform product) {
        
            this.productGrabPointTransform = productGrabPointTransform;
            productRigidBody.useGravity = false;
            this.productOnTopOfShelf = productGrabPointTransform;

    }

    private void FixedUpdate()
    {
        if (productGrabPointTransform != null) {
            productRigidBody.MovePosition(productGrabPointTransform.position);
        
        }
    }
    
    public Transform GetProductOnTopOfShelf() { return productOnTopOfShelf; }
}