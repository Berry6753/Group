using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManger : Singleton<GameManger>
{
    public GameObject clearZone;
    public bool isClear;
    public bool isBattle;

    // Start is called before the first frame update
    void Start()
    {
        clearZone.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (isClear)
        {
            clearZone.SetActive(true);
        }
    }
}
