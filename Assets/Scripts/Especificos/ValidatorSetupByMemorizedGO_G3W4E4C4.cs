using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MDS.Core;
using MDS.Gameplay.Selectable;
using MDS.Validators;
using MDS.Validators.Interfaces;
using UnityEngine;

public class ValidatorSetupByMemorizedGO_G3W4E4C4 : MDSBehaviour
{

    private Validator _validator;
	
    [SerializeField]
    private  Dictionary<string, List<string>> _map;
    //            selectable_label -> proximos labels (que devem ser colocados no validator) 


	void Start () {

        _validator = Challenge.GetCurrentValidador() as Validator;
        _validator.Rules = new List<ValidationRule>();

        SelectableGroup sgroup = FindObjectOfType<SelectableGroup>();

        // pegando os labels para conferir se o mapa nao tem erro de digitacao..
        List<string> validLabels = new List<string>();
        foreach(var item in sgroup._selectables)
        {
            validLabels.AddRange(item.Labels);
        }



        GameObject go = MemorizeMe.MemorizedGameObject;

        if (go==null)
        {
            LogError("Tentando usar um objeto memorizado sem haver um..");
            return;
        }


        Selectable selectable = go.GetComponent<Selectable>();

        List<string> values;

        if (_map.TryGetValue(selectable.Labels[0], out values)==false)
        {
            LogError("Mapa faltando label");
            return;
        }

        for(int i = 0; i < values.Count; i++)
        {
            ValidationRule r = new ValidationRule();
            r.OperationLogic = MDS.Validators.Enum.OperationLogic.OR;
            r.ValidatableObject = sgroup;
            r.CorrectAnswer = values[i];
            if(validLabels.Contains(r.CorrectAnswer) == false)
                LogError("Mapa com label inexitente");
            _validator.Rules.Add(r);
        }

        	
	}
	
	
}
