using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AISideswiper : MonoBehaviour
{
    [SerializeField] private float moveSpeed;

    void Update()
    {
        Move();
    }

    void Move()
    {
        transform.position += (moveSpeed * transform.up * Time.deltaTime);
    }
}
