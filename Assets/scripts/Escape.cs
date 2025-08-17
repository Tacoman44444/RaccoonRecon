using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Escape : MonoBehaviour
{

    [SerializeField] private Transform player;
    [SerializeField] private Transform hostage;
    void Update()
    {
        if (Vector3.Distance(transform.position, player.position) < 1.0f && Vector3.Distance(transform.position, hostage.position) < 3.0f)
        {
            Debug.Log("ESCAPED");
            GameResult.PlayerWon = true;
            SceneManager.LoadScene("GameOverScene");
        }
    }
}
