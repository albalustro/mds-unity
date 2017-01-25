using System.Collections;
using System.Collections.Generic;
using FullInspector;
using MDS.Player;
using UnityEngine;

namespace MDS.Actions
{
    public class PlayerAvatarAction : BaseAction
    {

        [SerializeField, ShowInInspector]
        private bool _restoreChosenAvatar;

        [SerializeField, InspectorHideIf("_restoreChosenAvatar")]
        private PlayerAvatar? _setAvatar;

        [SerializeField, InspectorHideIf("_restoreChosenAvatar")]
        private bool _setMounted;

        [SerializeField, InspectorHideIf("_restoreChosenAvatar")]
        private bool _setIdleBack;

     

        public override IEnumerator Execute()
        {
            if(byPass) yield break;
            yield return base.Execute();

            var pController = GameObject.FindObjectOfType<PlayerController>();

            if(_restoreChosenAvatar)
                pController.RestoreOriginalAvatar();
            else
                pController.SetAvatar(_setAvatar, _setMounted, _setIdleBack);


        }

    }
}