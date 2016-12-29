using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EpisodeSceneManager : MDSBehaviour {

    

    public float rightX;
    public float leftX;

    private Transform _player;
    private Transform _target;

    protected override void Awake()
    {
        base.Awake();

        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _target = Camera.main.transform.root;
    }

    void Update()
    {
        Vector3 newPos = _target.position;
        float x = Mathf.Clamp(_player.position.x, leftX, rightX);

        newPos.x = Mathf.Lerp(newPos.x, x, Time.deltaTime);

        _target.position = newPos;
    }

}
