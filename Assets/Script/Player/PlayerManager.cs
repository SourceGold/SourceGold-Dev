using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : CharacterManager
{
    [HideInInspector] public Transform MainCamera;

    private void Awake()
    {
        MainCamera = FindObjectOfType<CameraManager>().GetComponent<Transform>();
    }

    // Start is called before the first frame update
    void Start()
    {
       
        //Debug.Log(MainCamera);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
