using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class ControladorIA : MonoBehaviour
{
    public enum States
    {
        Esperar,
        Patrulhar,
        Procurar,
        Perseguir
    }
    public States atualState;

    [Header("State : Esperar")]
    public float waitTime = 2.0f;
    private float waitingTime;

    [Header("State : Patrulhar")]
    public Transform wayPoint1, wayPoint2;
    public Transform atualWayPoint;
    float distanceByWayPoints;

    [Header("State : Perseguir")]
    private GameObject player;
    public float fieldVision = 5f;
    float distanceByPlayer;

    [Header("State : Procurar")]
    public float timePercistance = 6.0f;
    private float timeNotVision;


    //Movement by AI
    private AICharacterControl aiCharacterControll;
    private Transform target;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        atualWayPoint = wayPoint1;
        aiCharacterControll = GetComponent<AICharacterControl>();
    }

    private void Start()
    {
        Esperar();
    }

    private void Update()
    {
        CheckingState();
    }

    private void Esperar()
    {
        atualState = States.Esperar;
        waitingTime = Time.time;
    }

    private void CheckingState()
    {
        if (atualState != States.Perseguir && PossuiVisaoDoJogador())
        {
            Perseguir();
        }


        switch (atualState)
        {
            case States.Esperar:
                if(WaitedEnought())
                {
                    Patrulhar();
                }
                else
                {
                    target = transform;
                }
                break;
            case States.Patrulhar:
                if(PertoWayPointAtual())
                {
                    AlternateWayPoint();
                }
                break;
            case States.Perseguir:
                if (!PossuiVisaoDoJogador())
                {
                    Search();
                }
                else
                {
                    target = player.transform;
                }
                break;
            case States.Procurar:
                if (NotVisionTimeEnough())
                {
                    Esperar();
                }
                break;
                    
        }
        aiCharacterControll.target = target;
    }

    private bool NotVisionTimeEnough()
    {
        return timeNotVision + timePercistance <= Time.time;
    }

    private void Search()
    {
        atualState = States.Procurar;
        timeNotVision = Time.time;
        target = null;
    }

    private void Perseguir()
    {
        atualState = States.Perseguir;
    }

    private bool PossuiVisaoDoJogador()
    {

        distanceByPlayer = Vector3.Distance(transform.position, player.transform.position);
        return distanceByPlayer <= fieldVision;
    }

    private bool PertoWayPointAtual()
    {
        distanceByWayPoints = Vector3.Distance(transform.position, atualWayPoint.position);
        return distanceByWayPoints <= 0.1f;
    }

    private void AlternateWayPoint()
    {
        atualWayPoint = (atualWayPoint == wayPoint1) ? wayPoint2 : wayPoint1;
        target = atualWayPoint;
    }

    private void Patrulhar()
    {
        atualState = States.Patrulhar;
        AlternateWayPoint();
    }

    private bool WaitedEnought()
    {
        return waitingTime + waitTime <= Time.time;
    }
}
