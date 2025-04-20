using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class animalMovementScript : MonoBehaviour
{
    private Vector2 moveDirection = Vector2.zero;
    private float changeDirectionTime = 5.0f;
    private float timer = 0.0f;
    [SerializeField] float speed;
    private float speedDampingFactor = 100.0f;

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("Fence"))
        {
            moveDirection = -moveDirection;
        }
        if (collision.gameObject.CompareTag("Raccoon"))
        {

        }
    }

    // Start is called before the first frame update
    void Start()
    {

        speed /= speedDampingFactor;
        ChangeDirection();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(moveDirection.x * speed, moveDirection.y * speed, 0.0f);
        timer += Time.deltaTime;
        if (timer > changeDirectionTime)
        {
            timer = 0.0f;
            ChangeDirection();
        }
        

    }

    void ChangeDirection()
    {
        float randomValue = Random.Range(1, 4);
        if (randomValue == 1)
        {
            moveDirection = Random.insideUnitCircle.normalized;
        } else
        {
            moveDirection = Vector2.zero;
        }
        
    }


}
