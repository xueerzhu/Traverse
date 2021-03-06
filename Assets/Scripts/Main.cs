using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.SceneManagement;


public class Main : MonoBehaviour
{
    public Vector3 testAngle;
    [SerializeField] private PlayerController playerA; 
    [SerializeField] private Board board; 
    [SerializeField] private Transform viewTransform;
    [SerializeField] private bool isFlip;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float flipSpeed;
    [SerializeField] private AnimationCurve flipCurve;
    
    private Game.Side boardSide;
    private bool isRotating = false;
    
    void Start()
    {
        // init board
        SetBoardSide(Game.Side.ALPHA);
        
        // init players
        InitPlayerControllers();
    }

    private void InitPlayerControllers()
    {
        playerA.InitPlayer(Game.Side.ALPHA, this);
    }

    public void SetBoardSide(Game.Side side)
    {
        boardSide = side;
    }
    
    // BOARD
    public Square GetSquare(Vector3Int squareLocation)
    {
        return board.GetSquareInternal(squareLocation);
    }

    public void FlipGroundTile(Vector3Int tileLoc)
    {
        StartCoroutine(board.FlipGroundTileInternal(tileLoc));
    }

    // INPUTS
    private void OnMove(InputValue movementValue)
    {
        Vector2 vectorA = movementValue.Get<Vector2>();
        Vector2 vectorB = new Vector2(vectorA.x, -vectorA.y);
        
        if (boardSide == Game.Side.ALPHA)
        {
            playerA.Move(vectorA);
        }
    }

    private void OnFlipBoard(InputValue flipValue)
    {
        float flip = flipValue.Get<float>();

        if (isRotating)
        {
            isRotating = false;
            return;
        }
        
        if (flip == 1f)
        {
            if (boardSide == Game.Side.ALPHA)
            {
                StartCoroutine(FlipBoard(new Vector3(270f, 0f, 0f)));
                SetBoardSide(Game.Side.BETA);
            }
            else
            {
                StartCoroutine(FlipBoard(new Vector3(90f, 0f, 0f)));
                SetBoardSide(Game.Side.ALPHA);
            }
        }
    }
    
    private void OnRotateBoard(InputValue rotateValue)
    {
        float r = rotateValue.Get<float>();

        if (isRotating)
        {
            isRotating = false;
            return;
        }
        
        if (r == 1f)
        {
            StartCoroutine(RotateBoard());
        }
    }

    private void OnRestart(InputValue restartValue)
    {
        float restart = restartValue.Get<float>();

        if (restart == 1f)
        {
            SceneManager.LoadScene("GameScene");
        }
    }
    
    // COROUTINES
    private IEnumerator FlipBoard(Vector3 endValue)
    {
        isRotating = true;
        
        float time = 0;
        Vector3 startEulerAngles = viewTransform.eulerAngles;
        Vector3 deltaEulerAngles = Vector3.zero;
        
        while (time < 0.6f)
        {
            viewTransform.eulerAngles = Game.EaseOut(time / 0.5f) * (new Vector3(360*1,0,0) + endValue);
            // viewTransform.eulerAngles = (new Vector3(360*1,0,0) + endValue) * flipCurve.Evaluate(time / 0.5f);
            time += Time.deltaTime;
            yield return null;
        }
        isRotating = false;
    }
    
    private IEnumerator RotateBoard()
    {
        isRotating = true;
        
        float time = 0;
        Vector3 startEulerAngles = viewTransform.eulerAngles;
        Vector3 deltaEulerAngles = Vector3.zero;
        
        while (isRotating)
        {
            deltaEulerAngles += testAngle * Time.deltaTime * rotationSpeed;
            time += Time.deltaTime;
            viewTransform.eulerAngles = startEulerAngles + deltaEulerAngles;
            yield return null;
        }
        isRotating = false;
    }
}
