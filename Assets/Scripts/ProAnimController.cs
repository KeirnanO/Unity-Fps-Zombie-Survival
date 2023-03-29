using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ProAnimController : MonoBehaviour
{
    public float _speed = 1f;

    private Rigidbody _rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_rigidbody.velocity.magnitude < _speed)
        {
            _rigidbody.AddForce(0, 0, 1 * Time.fixedDeltaTime * 1000f);
        }
    }
}