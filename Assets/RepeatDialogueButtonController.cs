using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MDS.Utilities;
using UnityEngine.SceneManagement;

public class RepeatDialogueButtonController : MDSBehaviour {

	public Sprite[] botao_npc_down;
	public Sprite[] botao_npc_up;
	private Image image;
	private Button button;
	private SpriteState spriteState;

	protected override void Awake ()
	{
		base.Awake ();
		image = GetComponent<Image> ();
		spriteState = new SpriteState ();
		Scene scene = SceneManager.GetActiveScene ();
		print (scene.GetWorldIndex ());
		switch (scene.GetWorldIndex())
		{
		case 1: 
			image.sprite = botao_npc_up [0];
			spriteState.pressedSprite = botao_npc_down[0];
			button.spriteState = spriteState;
			break;
		case 2:
			image.sprite = botao_npc_up [1];
			spriteState.pressedSprite = botao_npc_down[1];
			button.spriteState = spriteState;
			break;
		case 3: 
			image.sprite = botao_npc_up [2];
			spriteState.pressedSprite = botao_npc_down[2];
			button.spriteState = spriteState;
			break;
		case 4: 
			image.sprite = botao_npc_up [3];
			spriteState.pressedSprite = botao_npc_down[3];
			button.spriteState = spriteState;
			break;
		}
	}
	

}
