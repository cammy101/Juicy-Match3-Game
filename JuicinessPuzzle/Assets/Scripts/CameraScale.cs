﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScale : MonoBehaviour
{

    private Board board;

    public float cameraOffset;
    public float aspectRatio = 0.625f;
    public float padding = 2;
    public float yOffSET = 1;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();

        if (board != null)
        {
            Reposition(board.width -1, board.height -1);
        }
    }

    void Reposition(float x, float y)
    {
        Vector3 tempPos = new Vector3(x/2, y/2 + yOffSET, cameraOffset);
        transform.position = tempPos;
        if (board.width >= board.height)
        {
            Camera.main.orthographicSize = (board.width / 2 + padding) / aspectRatio;
        }
        else
        {
            Camera.main.orthographicSize = board.height / 2 + padding;
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
