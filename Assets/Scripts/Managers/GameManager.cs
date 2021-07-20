using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	public int m_NumRoundsToWin = 5;            // The number of rounds a single player has to win to win the game.
	public float m_StartDelay = 3f;             // The delay between the start of RoundStarting and RoundPlaying phases.
	public float m_EndDelay = 3f;               // The delay between the end of RoundPlaying and RoundEnding phases.
	public CameraControl m_CameraControl;       // Reference to the CameraControl script for control during different phases.
	public Text m_MessageText;                  // Reference to the overlay Text to display winning text, etc.
		
	private WaitForSeconds m_StartWait;         // Used to have a delay whilst the round starts.
	private WaitForSeconds m_EndWait;           // Used to have a delay whilst the round or game ends.

	public GameObject[] m_TankPrefabs;
	public TankManager[] m_Tanks;
	public List<Transform> wayPointsForAI;

	private void Start()
	{
		// Create the delays so they only have to be made once.
		m_StartWait = new WaitForSeconds (m_StartDelay);
		m_EndWait = new WaitForSeconds (m_EndDelay);

		SpawnAllTanks(); // Spawn tanks and corresponding scripts
		SetCameraTargets();

		// Once the tanks have been created and the camera is using them as targets, start the game.
		StartCoroutine (GameLoop ());
	}


	/// <summary>
	/// Method to spawn tanks and relative scripts for tanks
	/// </summary>
	private void SpawnAllTanks()
	{
		for (int i = 0; i < m_Tanks.Length; i++)
		{
			// ... create them, set their player number and references needed for control.
			m_Tanks[i].m_Instance =
				Instantiate(m_TankPrefabs[i], m_Tanks[i].m_SpawnPoint.position, m_Tanks[i].m_SpawnPoint.rotation) as GameObject;
			m_Tanks[i].m_PlayerNumber = i + 1;

			if (m_Tanks[i].isAI)
				m_Tanks[i].SetupAI(wayPointsForAI);
			else
				m_Tanks[i].SetupPlayerTank(m_CameraControl);
		}
	}


	private void SetCameraTargets()
	{
		// Create a collection of transforms the same size as the number of tanks.
		Transform[] targets = new Transform[m_Tanks.Length];

		// For each of these transforms...
		for (int i = 0; i < targets.Length; i++)
		{
			// ... set it to the appropriate tank transform.
			targets[i] = m_Tanks[i].m_Instance.transform;
		}

		// These are the targets the camera should follow.
		m_CameraControl.m_Targets = targets;
	}


	// This is called from start and will run each phase of the game one after another.
	private IEnumerator GameLoop ()
	{
		GameSettings.Instance.OnBeginRound();

		// Start off by running the 'RoundStarting' coroutine but don't return until it's finished.
		yield return StartCoroutine (RoundStarting ());

		// Once the 'RoundStarting' coroutine is finished, run the 'RoundPlaying' coroutine but don't return until it's finished.
		yield return StartCoroutine (RoundPlaying());

		// Once execution has returned here, run the 'RoundEnding' coroutine, again don't return until it's finished.
		yield return StartCoroutine (RoundEnding());

		// This code is not run until 'RoundEnding' has finished.  At which point, check if a game winner has been found.
		if (GameSettings.Instance.ShouldFinishGame())
		{
			SceneManager.LoadScene (2);
		}
		else
		{
			SceneManager.LoadScene(1, LoadSceneMode.Single);
		}
	}


	private IEnumerator RoundStarting ()
	{
		// As soon as the round starts reset the tanks and make sure they can't move.
		DisableTankControl ();

		// Snap the camera's zoom and position to something appropriate for the reset tanks.
		m_CameraControl.SetStartPositionAndSize ();

		// Increment the round number and display text showing the players what round it is.
		m_MessageText.text = "ROUND " + GameState.Instance.RoundNumber;

		// Wait for the specified length of time until yielding control back to the game loop.
		yield return m_StartWait;
	}


	private IEnumerator RoundPlaying ()
	{
		// As soon as the round begins playing let the players control the tanks.
		EnableTankControl ();

		// Clear the text from the screen.
		m_MessageText.text = string.Empty;

		// While there is not one tank left...
		while (!GameSettings.Instance.ShouldFinishRound())
		{
			// ... return on the next frame.
			yield return null;
		}
	}


	private IEnumerator RoundEnding ()
	{
		// Stop tanks from moving.
		DisableTankControl ();

		var winner = GameSettings.Instance.OnEndRound();

		// Get a message based on the scores and whether or not there is a game winner and display it.
		string message = EndMessage (winner);
		m_MessageText.text = message;

		// Wait for the specified length of time until yielding control back to the game loop.
		yield return m_EndWait;
	}

	// Returns a string message to display at the end of each round.
	private string EndMessage(TankThinker winner)
	{
		return winner != null ? winner.player.PlayerInfo.GetColoredName() + " WINS THE ROUND!" : "DRAW!";
	}

	private void EnableTankControl()
	{
		for (int i = 0; i < m_Tanks.Length; i++)
		{
			m_Tanks[i].EnableControl();
		}
	}


	private void DisableTankControl()
	{
		for (int i = 0; i < m_Tanks.Length; i++)
		{
			m_Tanks[i].DisableControl();
		}
	}
}