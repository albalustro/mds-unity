using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MDS.Gameplay.Selectable;
using MDS.Validators;

/// <summary>
/// Usado na cena G3W1E1C1 para atribuir em tempo de execução o CorrectAnswer do ValidationRules
/// Pegando o label do Selectable ao ser clicado. Funciona 1 pra 1 (uma Label pra um Rules)
/// </summary>
public class SelectableToValidationRule_G3W1E1C1 : MDSBehaviour {

	Selectable selectable;
	public Validator validator;

	void Start () {
		selectable = GetComponent<Selectable> ();
	}
	
	void OnMouseUp()
	{
		validator.Rules [0].CorrectAnswer = selectable.Labels [0];
	}
}
