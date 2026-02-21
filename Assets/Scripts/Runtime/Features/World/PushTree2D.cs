using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PushTree2D : MonoBehaviour
{
    [field: SerializeField] Rigidbody2D thisRigidbody { get; set; }

    private void Reset()
    {
        thisRigidbody = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
            thisRigidbody.bodyType = RigidbodyType2D.Dynamic;
    }
}
