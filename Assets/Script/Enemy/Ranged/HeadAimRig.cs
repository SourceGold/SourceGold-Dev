using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class HeadAim : MonoBehaviour
{
    private PlayerManager _playerManager;
    private Transform _playerBot;
    private MultiAimConstraint _multiAimConstraint;
    private RigBuilder _rigBuilder;

    private void Awake()
    {
        _playerManager = FindObjectOfType<PlayerManager>();
        _playerBot = _playerManager.transform.Find("Player Bot");
        _multiAimConstraint = GetComponent<MultiAimConstraint>();
        _rigBuilder = GetComponentInParent<RigBuilder>();
    }

    // Start is called before the first frame update
    void Start()
    {
        var data = _multiAimConstraint.data.sourceObjects;
        data.SetTransform(0, _playerBot.Find("Follow Target"));
        _multiAimConstraint.data.sourceObjects = data;
        _rigBuilder.Build();
    }

    // Update is called once per frame
    void Update()
    {
    }
}
