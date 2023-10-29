using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlManager : MonoBehaviour
{
    // Start is called before the first frame update

    public InputMap InputMap;

    private void Awake()
    {
        InputMap = new InputMap();
        InputMap.Player.Enable();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
