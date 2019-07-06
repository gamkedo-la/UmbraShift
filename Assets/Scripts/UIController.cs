using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    
    public Button endTurn;

    public Text currentSelectedCharacterName;
    public Image currentSelectedCharacterAvatar;
    public Transform characterPanelTransform;
    public GameObject buttonPrefab;
    
    public List<Image> characterImages = new List<Image>();
    public List<BaseCharacterClass> characters = new List<BaseCharacterClass>();
    public List<Button> characterButtons = new List<Button>();

	public void SetCurrentCharacterName(string characterName)
    {
        currentSelectedCharacterName.text = characterName;
    }

    public void SetCurrentCharacterAvatar(Sprite avatar)
    {
        currentSelectedCharacterAvatar.sprite = avatar;
    }

    public void SetActiveCharacter(BaseCharacterClass BCC)
    {
        SetCurrentCharacterName(BCC.name);
    }
    
    
    // Start is called before the first frame update
    private void Start()
    {
        foreach (var character in characters)
        {
            GameObject tempCharacterUIButton = (GameObject) GameObject.Instantiate(buttonPrefab, characterPanelTransform);
            
            Image tempImage = tempCharacterUIButton.GetComponentInChildren<Image>();
            
            if (tempImage != null)
            {
                tempImage.sprite = character.avatar;
                characterImages.Add(tempImage);
            }

            Button tempButton = tempCharacterUIButton.GetComponent<Button>();
            PlayerController tempPC = character.GetComponent<PlayerController>();
            
            Debug.Log($"tempPC is {tempPC} and tempButton is {tempButton}");

            if (tempButton != null && tempPC != null)
            {
                tempButton.onClick.AddListener(() =>
                {
                    InputManager.instance.SetCurrentPlayerController(tempPC);
					SoundManager.instance.playSound( "mousedown" );
                });
                characterButtons.Add(tempButton);
            }

        }

        Button endTurnButton = Instantiate(endTurn, characterPanelTransform);

        endTurnButton.onClick.AddListener(() =>
        {
            TurnManager.instance.SwitchTurn();
			SoundManager.instance.playSound( "mousedown" );
		} );
    }
}
