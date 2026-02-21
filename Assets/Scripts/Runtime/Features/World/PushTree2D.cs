using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PushTree2D : MonoBehaviour
{
    [field: SerializeField] Rigidbody2D thisRigidbody { get; set; }
    [field: SerializeField] BoxCollider2D thisBoxCollider { get; set; }
    
    void Awake()
    {
        // make one if we miss one, remove if wanna set up manually always
        if (thisBoxCollider == null)
        {
            thisBoxCollider = gameObject.AddComponent<BoxCollider2D>();
            // optional defaults
            thisBoxCollider.size = new Vector2(1f, 1f);
            thisBoxCollider.offset = Vector2.zero;
        }
    }
    
    private void Reset()
    {
        thisRigidbody = GetComponent<Rigidbody2D>();
        thisBoxCollider = GetComponent<BoxCollider2D>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {   
        if (thisRigidbody.bodyType == RigidbodyType2D.Dynamic)
            return;
        if (!other.gameObject.CompareTag("Player"))
            return;
        if (other.relativeVelocity.magnitude < 0.1f)
            return;
        thisRigidbody.bodyType = RigidbodyType2D.Dynamic;
    }
}
