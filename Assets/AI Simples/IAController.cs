using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IAController : MonoBehaviour
{
	private GameObject playerPosition;
	private float timeStartedGuarding;

	public enum States
	{
		Patrol,
		Guard,
		Chase
	}
	public States atualState;
	NavMeshAgent agent;
	public Transform target;

	[Header("State : Patrol")]
	public Transform wayPoint1;
	public Transform wayPoint2;
	float distanceByWayPoints;

	[Header("State : Guard")]
	public float waitTime;

	public float listenRadius;
	//public float waitedTime;
	private void Awake()
	{
		target = wayPoint1;
	}
	private void Start()
	{
		playerPosition = GameObject.FindGameObjectWithTag("Player");
		agent = GetComponent<NavMeshAgent>();

		//Se a entidade precisar começar patrulhando:
		agent.SetDestination(target.position);
		StartPatrolling();
	}
	private void Update()
	{
		ProcessState();
		if (atualState != States.Chase && (CheckIfPlayerIsSeen() || CheckIfPlayerIsHeard()))
		{
			StartChasing();
		}
	}

	/// <summary>
	/// Processa o estado atual no qual a entidade se encontra
	/// </summary>
	private void ProcessState()
	{
		switch (atualState)
		{
			case States.Patrol:
				Patrol();
				break;
			case States.Guard:
				Guard();
				break;
			case States.Chase:
				Chase();
				break;
		}
	}

	private void Guard()
	{
		if (WaitedEnough())
		{
			StartPatrolling();
		}
		//CheckIfPlayerIsSeen();
	}

	//private void Wait()
	//{
	//    atualState = States.Guard;
	//    waitedTime = Time.time;
	//}
	private bool WaitedEnough()
	{
		return Time.time >= timeStartedGuarding + waitTime;
	}

	/// <summary>
	/// Inicia patrulhamento
	/// </summary>
	private void StartPatrolling()
	{
		atualState = States.Patrol;
		agent.isStopped = false;
	}

	private void StartGuarding()
	{
		atualState = States.Guard;
		agent.isStopped = true;
		timeStartedGuarding = Time.time;
	}

	private void Patrol()
	{
		if (PertoDoWaypoint())
		{
			ChangeWayPoints();
		}

		//if (CheckIfPlayerIsSeen())
		//{
		// StartChasing();
		//}
		//Mecanica de avistar o jogador
	}

	private void ChangeWayPoints()
	{
		target = (target == wayPoint1) ? wayPoint2 : wayPoint1;
		agent.SetDestination(target.position);
		StartGuarding();
	}

	/// <summary>
	/// Detecta se a entidade está perto (o bastante) do waypoint
	/// </summary>
	/// <returns></returns>
	private bool PertoDoWaypoint()
	{
		distanceByWayPoints = Vector3.Distance(transform.position, target.transform.position);
		return distanceByWayPoints <= .1f;
	}

	/// <summary>
	/// Checar se o raycast atinge o jogador
	/// </summary>
	private bool CheckIfPlayerIsSeen()
	{
		//return (player.isSeen);
		return false;
	}

	private bool CheckIfPlayerIsHeard()
	{
		float distance = Vector3.Distance(transform.position, playerPosition.transform.position);
		return distance < listenRadius;
	}

	private void StartChasing()
	{
			atualState = States.Chase;
		agent.SetDestination(playerPosition.transform.position);
		agent.isStopped = false;
	}

	private void Chase()
	{
		if (!CheckIfPlayerIsSeen() && !CheckIfPlayerIsHeard())
		{
			agent.SetDestination(target.position);
			StartGuarding();
		}
		else
		{
			agent.SetDestination(playerPosition.transform.position);
			//Check if can fire
			//If yes, fire
		}
	}
}
