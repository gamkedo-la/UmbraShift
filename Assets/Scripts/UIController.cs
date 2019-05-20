using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Button player1;
    public Button player2;
    public Button endTurn;

    public Text currentSelectedCharacterName;
    public Image currentSelectedCharacterAvatar;

    public void SetCurrentCharacterName(string characterName)
    {
        currentSelectedCharacterName.text = characterName;
    }

    public void SetCurrentCharacterAvatar(Sprite avatar)
    {
        currentSelectedCharacterAvatar.sprite = avatar;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
