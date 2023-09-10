using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllEnemyManager : MonoBehaviour
{
    //public Transform Player;
    [HideInInspector] public Transform Player;

    private void Awake()
    {
        Player = FindObjectOfType<PlayerManager>().GetComponent<Transform>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
