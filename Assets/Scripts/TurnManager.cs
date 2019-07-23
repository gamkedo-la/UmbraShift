using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager instance;
	public UIController uiController;
	List<PlayerController> playerControllers = new List<PlayerController>();
    List<EnemyController> enemyControllers = new List<EnemyController>();

    public bool playersTurn = true;
    public bool isCombatModeActive = false;
	private GameObject activeCharacter;
	private PlayerController activePlayerController;
	private PlayerController mainCharacterController;
	private NewCameraController newCameraController;
	public GameObject ActiveCharacter { get { return activeCharacter; } }
	public PlayerController ActivePlayerController { get { return activePlayerController; } }
	public PlayerController MainCharacterController { get { return mainCharacterController; } }
	public NewCameraController NewCameraController { get { return newCameraController; } }

	private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("singleton already existed but attempted re-assignment");
        }
        else
        {
            instance = this;
        }
    }

    public void Start()
    {
		activePlayerController = null;
		activeCharacter = null;
		DebugManager.instance.OverwriteDebugText($"Combat mode is {isCombatModeActive}");
		newCameraController = FindObjectOfType<NewCameraController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Enter pressed");
            SwitchTurn();
        }
    }

	public void SetActiveCharacter(GameObject character)
	{
		if (activePlayerController) { activePlayerController.DeactivateCharacter(); }
		activeCharacter = character;
		if (newCameraController && newCameraController.CAMERA_ACTIVE==false) { newCameraController.InitialzieCameraHome(activeCharacter);}
		else if (newCameraController) { newCameraController.AssignCameraHome(activeCharacter); }
		if (character.GetComponent<PlayerController>())
		{
			activePlayerController = character.GetComponent<PlayerController>();
			activePlayerController.SetAsActivePlayerController();
			Debug.Log($"Set new manager from {activePlayerController.name}");
			BaseCharacterClass BCC = activeCharacter.GetComponent<BaseCharacterClass>();
			if (BCC != null)
			{
				Debug.Log(BCC.avatar);
//				uiController.SetCurrentCharacterName(BCC.characterName);
				uiController.SetCurrentCharacterAvatar(BCC.avatar);
			}
		}
		else { activePlayerController = null; }
	}



	public void ClearActiveCharacters()
	{
		activeCharacter = null;
		activePlayerController = null;
	}

    public void SwitchTurn()
    {

        playersTurn = !playersTurn;

        foreach (PlayerController eachPC in playerControllers)
        {
            if (playersTurn)
            {
                BaseCharacterClass tempBase = eachPC.GetComponent<BaseCharacterClass>();
                if (tempBase != null)
                {
                    tempBase.ActionPointRefill();
                }
            }
        }

        foreach (EnemyController eachEnemy in enemyControllers)
        {
            if (!playersTurn)
            {
                BaseCharacterClass tempBase = eachEnemy.GetComponent<BaseCharacterClass>();
                if (tempBase != null)
                {
                    tempBase.ActionPointRefill();
                }
            }
        }
    }

    public void CombatMode( bool forceEndCombat = false )
    {
		if (forceEndCombat)
			isCombatModeActive = false;
		else
			isCombatModeActive = !isCombatModeActive;

        DebugManager.instance?.OverwriteDebugText($"Combat mode is {isCombatModeActive}");
        Debug.Log($"CombatModeActive = {isCombatModeActive}");
    }

    public void PlayerControllerReportingForDuty(PlayerController playerController)
    {
        playerControllers.Add(playerController);
		if (playerController.isMainCharacter) { mainCharacterController = playerController; }
        foreach (var enemyController in enemyControllers)
        {
            enemyController.CheckForNewPlayerControllers();
        }
    }

	public void PlayerControllerReportingOffDuty( PlayerController playerController )
	{
		playerControllers.Remove( playerController );
		foreach ( var enemyController in enemyControllers )
		{
			enemyController.CheckForNewPlayerControllers( );
		}
	}

	public void EnemyControllerReportingForDuty(EnemyController enemyController)
    {
        enemyControllers.Add(enemyController);
        enemyController.CheckForNewPlayerControllers();
    }

	public void EnemyControllerReportingOffDuty( EnemyController enemyController )
	{
		enemyControllers.Remove( enemyController );

		if ( enemyControllers.Count == 0 )
			CombatMode( true );
	}

	public List<PlayerController> GetCharacterControllers()
    {
        List<PlayerController> characters = new List<PlayerController>();
        foreach (PlayerController eachPC in playerControllers)
        {
            if (eachPC.CompareTag("Player"))
            {
                characters.Add(eachPC);
            }
        }
        return characters;
    }
}
