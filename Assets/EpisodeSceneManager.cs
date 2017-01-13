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

public class EpisodeSceneManager : MDSBehaviour {

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


        _context = GameObject.FindObjectOfType<EpisodeContext>();
        if (_context == null)
        {
            GameObject goContext = new GameObject("EPISODE CONTEXT");
            _context = goContext.AddComponent<EpisodeContext>();
        }


        // isso irá garantir que o colider do polynav2d esteja atras de todos os demais...
        var allColliders = FindObjectsOfType<Collider2D>();
        var maxColliderZ = allColliders.Max(c => c.transform.position.z);
        Transform poly2DTransform = GetComponentInChildren<PolyNav2D>().transform;
        Vector3 newPolyNav2DPosition = new Vector3 ( poly2DTransform.position.x, poly2DTransform.position.y, maxColliderZ + 1);
        poly2DTransform.position = newPolyNav2DPosition;
    }

    private void Start()
    {
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

        switch(currentAvailableChallengeIndex)
        {
            case 0:
                float t1 = 0f; ;
                float t0 = Time.realtimeSinceStartup;
                _titleGO.GetComponentInChildren<Text>().text = SceneManager.GetActiveScene().GetEpisodeTitle();
                t1 = Time.realtimeSinceStartup;
                //Log("Tempo para decodificar o titulo: " + (t1 - t0).ToString());

                List<IAction> actions = new List<IAction>();
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
        Vector3 newPos = _target.position;
        float x = Mathf.Clamp(_player.position.x, leftX, rightX);
        float y = Mathf.Clamp(_player.position.y, bottomY, topY);

        newPos.x = Mathf.MoveTowards(newPos.x, x, cameraSpeed * Time.deltaTime);
        newPos.y = Mathf.MoveTowards(newPos.y, y, cameraSpeed * Time.deltaTime);

        _target.position = newPos;
    }

	public void SetCameraPosition(Vector3 position)
	{
		_target.position = position;
	}

}
