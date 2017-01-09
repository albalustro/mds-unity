using System;
using System.Collections;
using System.Collections.Generic;
using MDS.Core;
using MDS.Core.SceneManagement;

namespace MDS.Actions
{
    public class FinishChallenge : BaseAction
    {
        public override IEnumerator Execute()
        {
            if(byPass) yield break;
            yield return base.Execute();
            EpisodeContext.Instance.SetCurrentChallengeDone();
            SceneLoader.Instance.GoBackAfterChallenge();
        }
    }


    public class SetRedConcept: BaseAction
    {
        public override IEnumerator Execute()
        {
            yield return base.Execute();
            Challenge.ChallengeConcept = ConceptTypes.CONCEPT_RED;
        }
    }
}
