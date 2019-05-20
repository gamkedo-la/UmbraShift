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
    void Start()
    {
        foreach (var character in characters)
        {
            GameObject tempCharacterUIButton = (GameObject) GameObject.Instantiate(buttonPrefab);
            
            Image tempImage = tempCharacterUIButton.GetComponentInChildren<Image>();
            
            if (tempImage != null)
            {
                tempImage.sprite = character.avatar;
                characterImages.Add(tempImage);
            }

            Button tempButton = tempCharacterUIButton.GetComponent<Button>();

            if (tempButton != null)
            {
                tempButton.transform.SetParent(characterPanelTransform);
                characterButtons.Add(tempButton);
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
