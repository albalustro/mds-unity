using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MDS.Utilities;
using UnityEngine.SceneManagement;
using MDS.DialogueSystem;

public class RepeatDialogueButtonController : MonoBehaviour {

	public Sprite[] botao_npc_down;
	public Sprite[] botao_npc_up;
	private Image image;
	private Button button;
	private SpriteState spriteState;
	
	void Start ()
	{
		Scene scene = SceneManager.GetActiveScene ();
		image = GetComponent<Image> ();
		if (scene.IsEpisode () || scene.IsChallenge ())
		{
			spriteState = new SpriteState ();
			button = GetComponent<Button> ();
			ChangeInteractable (false);
			switch (scene.GetWorldIndex ())
			{
				case 1: 
					image.sprite = botao_npc_up [0];
					spriteState.pressedSprite = botao_npc_down [0];
					button.spriteState = spriteState;
					break;
				case 2:
					image.sprite = botao_npc_up [1];
					spriteState.pressedSprite = botao_npc_down [1];
					button.spriteState = spriteState;
					break;
				case 3: 
					image.sprite = botao_npc_up [2];
					spriteState.pressedSprite = botao_npc_down [2];
					button.spriteState = spriteState;
					break;
				case 4: 
					image.sprite = botao_npc_up [3];
					spriteState.pressedSprite = botao_npc_down [3];
					button.spriteState = spriteState;
					break;
			}

			button.onClick.AddListener (() => RepeatDialogue ());
		}
		else 
		{
			image.enabled = false;	
		}
	}

	public void RepeatDialogue ()
	{
		//chama método no Diaglogue System para repetir a ultima slug tocada
		DialogueSystem.Instance.RepeatLastSlugPlayed();
	}

	public void ChangeInteractable(bool interact)
	{
		//Botão só pode estar clicável se o diálogo estiver fechado
		button.interactable = interact;
	}
}
