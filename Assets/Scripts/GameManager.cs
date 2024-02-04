using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public GameState state;
    // Start is called before the first frame update
    void Start()
    {
        state = GameState.Start;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            state = GameState.Placing;
        }
    }
}

public enum GameState
{
    Start,
    Placing
}
