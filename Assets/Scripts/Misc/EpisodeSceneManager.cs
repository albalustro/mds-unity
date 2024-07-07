using System.Collections;
using System.Collections.Generic;
using MDS.Core.Interfaces;
using MDS.Core.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using FullInspector;
using MDS.Actions;
using Newtonsoft.Json;
using UnityEngine.UI;
using MDS.Utilities;
using System;

public class EpisodeSceneManager : MDSBehaviour {

    public bool StaticCamera { get; set; }

    [Serializable]
    public class ParallaxItem
    {
        [FullInspector.InspectorOrder(0)]
        public Transform target;
        [InspectorShowIf("ShowCaptureButtons"), FullInspector.InspectorOrder(1)]
        public Vector3 rightPosition;
        [InspectorShowIf("ShowCaptureButtons"), FullInspector.InspectorOrder(3)]
        public Vector3 leftPostion;

        [InspectorShowIf("ShowCaptureButtons"), FullInspector.InspectorOrder(2), InspectorButton]
        void CaptureRightPosition()
        {
            rightPosition = target.position;
        }

        [InspectorShowIf("ShowCaptureButtons"), FullInspector.InspectorOrder(4), InspectorButton]
        void CaptureLeftPosition()
        {
            leftPostion = target.position;
        }


        private bool ShowCaptureButtons()
        {
            return target != null;
        }

        internal void SetTargetPosition(float t)
        {
            target.position = Vector3.Lerp(rightPosition, leftPostion, t);
        }
    }

    [SerializeField, InspectorCategory("Parallax")]
    private List<ParallaxItem> _parallaxItens;

    [InspectorCategory("General")]
    public float cameraSpeed = 2f;

    [InspectorCategory("General")]
    public float rightX;
    [InspectorCategory("General")]
    public float leftX;

    [InspectorCategory("General")]
    public float topY;
    [InspectorCategory("General")]
    public float bottomY;

	//[InspectorCategory("General")]
	//public AudioClip episodeTheme;

	[InspectorCategory("One Time Actions")]
	[SerializeField, InspectorCollectionShowItemDropdown(IsCollapsedByDefault = true)]
	private IAction[] BeforeTitleActions;

    [InspectorCategory("One Time Actions")]
    [SerializeField, InspectorCollectionShowItemDropdown(IsCollapsedByDefault = true)]
    private IAction[] StartEpisodeActions;

    [InspectorCategory("One Time Actions")]
    [SerializeField, InspectorCollectionShowItemDropdown(IsCollapsedByDefault = true)]
    private IAction[] ComingFromChallenge1Actions;

    [InspectorCategory("One Time Actions")]
    [SerializeField, InspectorCollectionShowItemDropdown(IsCollapsedByDefault = true)]
    private IAction[] ComingFromChallenge2Actions;

    [InspectorCategory("One Time Actions")]
    [SerializeField, InspectorCollectionShowItemDropdown(IsCollapsedByDefault = true)]
    private IAction[] ComingFromChallenge3Actions;

    [InspectorCategory("One Time Actions")]
    [SerializeField, InspectorCollectionShowItemDropdown(IsCollapsedByDefault = true)]
    private IAction[] ComingFromChallenge4Actions;

    [InspectorCategory("One Time Actions")]
    [SerializeField, InspectorCollectionShowItemDropdown(IsCollapsedByDefault = true)]
    private IAction[] ComingFromChallenge5Actions;


    [InspectorComment(CommentType.Info, "As actions abaixo são executadas de forma acumulativa. Por exemplo, quando voltar vindo do challenge 3, as actions 1, 2 e 3 são executadas antes da 'ComingFromChallenge3")]

    [InspectorCategory("Persistent Actions")]
    [SerializeField, InspectorCollectionShowItemDropdown(IsCollapsedByDefault = true)]
    private IAction[] ExecuteAfterChallenge1Actions;

    [InspectorCategory("Persistent Actions")]
    [SerializeField, InspectorCollectionShowItemDropdown(IsCollapsedByDefault = true)]
    private IAction[] ExecuteAfterChallenge2Actions;

    [InspectorCategory("Persistent Actions")]
    [SerializeField, InspectorCollectionShowItemDropdown(IsCollapsedByDefault = true)]
    private IAction[] ExecuteAfterChallenge3Actions;

    [InspectorCategory("Persistent Actions")]
    [SerializeField, InspectorCollectionShowItemDropdown(IsCollapsedByDefault = true)]
    private IAction[] ExecuteAfterChallenge4Actions;


    private GameObject _titleGO;
    private Transform _player;
    private Transform _target;
    private EpisodeContext _context;

    protected override void Awake()
    {
        base.Awake();

        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _target = Camera.main.transform.root;
        _titleGO = GameObject.FindGameObjectWithTag("EpisodeTitle");
        var cg = _titleGO.GetComponent<CanvasGroup>();
        if(cg != null)
            cg.alpha = 0;
        _titleGO.SetActive(false);


        // Para o context existir, deve estar voltando de um desafio.
        // Caso não exista, acabou de entrar no episodio e precisa criar o contexto.
        _context = GameObject.FindObjectOfType<EpisodeContext>();
        if (_context == null)
        {
            GameObject goContext = new GameObject("EPISODE CONTEXT");
            _context = goContext.AddComponent<EpisodeContext>();
        }


        // isso irá garantir que o colider do polynav2d esteja atras de todos os demais...
        var allColliders = FindObjectsOfType<Collider2D>();
        var maxColliderZ = allColliders.Max(c => c.transform.position.z);


		PolyNav2D[] poly2DTransform = GetComponentsInChildren<PolyNav2D>(true);
		for (int i = 0; i < poly2DTransform.Length; i++) {
			Vector3 newPolyNav2DPosition = new Vector3 ( poly2DTransform[i].transform.position.x, poly2DTransform[i].transform.position.y, maxColliderZ + 1);
			poly2DTransform[i].transform.position = newPolyNav2DPosition;    
		}

		GameObject mediator = GameObject.FindGameObjectWithTag ("Mediator");

		if(mediator.GetComponent<FlipMediadorByPlayerPosition>() == null)
			mediator.AddComponent<FlipMediadorByPlayerPosition> ();
        
    }

	private IEnumerator Start()
    {
		//AudioController.Instance.PlayTheme (episodeTheme);

        // iniciado com 5 apenas para a sequencia do switch ficar 'bonitinha'
        // se nenhum dos challenges estiver com status available é de se supor que
        // todos estao como done (mas nao vamos verificar). Portanto, esse Start
        // esta sendo executado pelo retorno do ultimo challenge, index 5.
        int currentAvailableChallengeIndex = 5;
        for(int i = 0; i < 5; i++)
        {
            if (_context.GetChallengeStatus(i) == ChallengeStatusInEpisode.Available)
            {
                currentAvailableChallengeIndex = i;
                break;
            }
        }

		yield return null;

		SetCameraStartPosition ();

        switch(currentAvailableChallengeIndex)
        {
		case 0:
			float t1 = 0f;
			float t0 = Time.realtimeSinceStartup;
			_titleGO.GetComponentInChildren<Text> ().text = SceneManager.GetActiveScene ().GetEpisodeTitle ();
			t1 = Time.realtimeSinceStartup;
                //Log("Tempo para decodificar o titulo: " + (t1 - t0).ToString());

			List<IAction> actions = new List<IAction> ();

			if (BeforeTitleActions != null)
				actions.AddRange (BeforeTitleActions);
			
                actions.Add(new EnableDisableAction(EnableDisableAction.EAction.Enable, new[] { _titleGO }));
                actions.Add(new FadeInOutAction(true, 2f, new[] { _titleGO }, true, 1f));
                actions.Add(new FadeInOutAction(false, 2f, new[] { _titleGO }, true, 2f));
                actions.Add(new EnableDisableAction(EnableDisableAction.EAction.Disable, new[] { _titleGO }));
                actions.AddRange(StartEpisodeActions);
                ExecuteActions(actions.ToArray());
                break;

            case 1:
                ExecuteActions(ExecuteAfterChallenge1Actions);
                ExecuteActions(ComingFromChallenge1Actions);
                break;

            case 2:
                ExecuteActions(ExecuteAfterChallenge1Actions);
                ExecuteActions(ExecuteAfterChallenge2Actions);
                ExecuteActions(ComingFromChallenge2Actions);
                break;

            case 3:
                ExecuteActions(ExecuteAfterChallenge1Actions);
                ExecuteActions(ExecuteAfterChallenge2Actions);
                ExecuteActions(ExecuteAfterChallenge3Actions);
                ExecuteActions(ComingFromChallenge3Actions);
                break;

            case 4:
                ExecuteActions(ExecuteAfterChallenge1Actions);
                ExecuteActions(ExecuteAfterChallenge2Actions);
                ExecuteActions(ExecuteAfterChallenge3Actions);
                ExecuteActions(ExecuteAfterChallenge4Actions);
                ExecuteActions(ComingFromChallenge4Actions);
                break;

            case 5:
                ExecuteActions(ExecuteAfterChallenge1Actions);
                ExecuteActions(ExecuteAfterChallenge2Actions);
                ExecuteActions(ExecuteAfterChallenge3Actions);
                ExecuteActions(ExecuteAfterChallenge4Actions);
                ExecuteActions(ComingFromChallenge5Actions);
                break;
        }

        
    }

    void Update()
    {
        if(StaticCamera) return;

        Vector3 newPos = _target.position;
        float x = Mathf.Clamp(_player.position.x, leftX, rightX);
        float y = Mathf.Clamp(_player.position.y, bottomY, topY);

        newPos.x = Mathf.MoveTowards(newPos.x, x, cameraSpeed * Time.deltaTime);
        newPos.y = Mathf.MoveTowards(newPos.y, y, cameraSpeed * Time.deltaTime);

        _target.position = newPos;


        float t = (newPos.x - rightX) / (leftX - rightX);


        foreach(var item in _parallaxItens)
        {
            item.SetTargetPosition(t);    
        }


    }

	private void SetCameraStartPosition()
	{
		Vector3 newPos = _target.position;
		float x = Mathf.Clamp(_player.position.x, leftX, rightX);
		float y = Mathf.Clamp(_player.position.y, bottomY, topY);

		newPos.x = x;
		newPos.y = y;
		_target.position = newPos;

	}

	public void SetCameraPosition(Vector3 position)
	{
		_target.position = position;
	}

}
