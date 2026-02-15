using System;
using UnityEngine;

public class CheckCollision : MonoBehaviour
{
    public static event Action OnCollision;

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("LOL");
        if (collision.gameObject.tag == "Ship")
        {
            OnCollision?.Invoke();
        }
    }
}
