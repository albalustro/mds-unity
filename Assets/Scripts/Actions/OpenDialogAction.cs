using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FullInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using MDS.Utilities;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MDS.Actions
{
    [Serializable]
    public class OpenDialogAction : BaseAction
    {


#if UNITY_EDITOR

        public class slugSelector
        {
            public bool useThis;

            [InspectorDisabled]
            public string slug;
        }

        public class slugSelectorComparer : IEqualityComparer<slugSelector>
        {
            public bool Equals(slugSelector x, slugSelector y)
            {
                return x.slug.Equals(y.slug);
            }

            public int GetHashCode(slugSelector obj)
            {
                return obj.slug.GetHashCode();
            }
        }

        [InspectorButton, InspectorOrder(0)]
        public void GetSlugs()
        {
            Scene curScene = SceneManager.GetActiveScene();

            sceneSlugs = new List<slugSelector>();
            string sceneName = curScene.name;

            string dialogPath = "Assets/DialogueSystem/Resources/SO/" + sceneName.Substring(0, 4) + ".asset";

            DialogueList list = AssetDatabase.LoadAssetAtPath<DialogueList>(dialogPath);


            string game = sceneName.Substring(1, 1);
            string world = sceneName.Substring(3, 1);
            string episode = sceneName.Substring(5, 1);
            string challenge = "";
            if(curScene.IsChallenge())
            {
                challenge = sceneName.Substring(7, 1);
            }
            else if (curScene.IsEpisode())
            {
            // nao precisa fazer nada    
            }

            sceneSlugs = list.dialogueList.Where(i => i.episode == episode && i.minigame == challenge)
                    .Select((s) => new slugSelector() { slug = s.slug })
                    .Distinct(new slugSelectorComparer())
                    .ToList();
        }

        [InspectorButton, InspectorOrder(1)]
        public void SetupSelected()
        {
            if(sceneSlugs == null || sceneSlugs.Count == 0) return;

            List<Slug> slugList = new List<Slug>();

            foreach(var s in sceneSlugs.Where(ss => ss.useThis))
            {
                slugList.Add((Slug)Enum.Parse(typeof(Slug), s.slug));
            }

            slugs = slugList.ToArray();

            sceneSlugs = null;
        }


        [ShowInInspector, InspectorOrder(3)]
        private List<slugSelector> sceneSlugs;

#endif


        public Slug[] slugs { get; set; }

        public override void Initialize(MonoBehaviour coroutineHolder)
        {
            base.Initialize(coroutineHolder);
            // Esse tipo de actions SEMPRE deverá aguardar que seja encerrado..
            // por isso deverá se comportar como se o wait finish marcado
            waitFinish = true;
        }

        public override IEnumerator Execute()
        {
            if(byPass) yield break;
            yield return base.Execute();
            DialogueSystem.Instance.ShowDialogueMessage(slugs);
            yield return new WaitWhile(DialogueSystem.Instance.IsDialogueOpen);
        }
    }
}
