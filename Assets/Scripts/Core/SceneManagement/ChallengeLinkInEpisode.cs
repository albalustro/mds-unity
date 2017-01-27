using System.Collections;
using System.Collections.Generic;
using FullInspector;
using MDS.Core.SceneManagement;
using MDS.Interactable;
using UnityEngine;

public class ChallengeLinkInEpisode : MDSBehaviour {

    private const string StartChallengePlayerPositionName = "PLAYER Position";
    private const string StartChallengeMediatorPosition = "MEDIATOR Position";

    [SerializeField, InspectorRange(1,5)]
    private int _challengeIndex = 1;
    public int ChallengeIndex { get { return _challengeIndex-1; } }

    [SerializeField, InspectorCollectionRotorzFlags(DisableReordering =true, HideAddButton =true,HideRemoveButtons =true)]
    [InspectorComment("Esse campo está bloqueado para adicionar/remover elementos")]
    private Dictionary<ChallengeStatusInEpisode, Sprite> _statusSprite;

    private GameObject _particles;
    private Transform _startChallengePlayerPosition;
    private Transform _startChallengeMediatorPosition;
    private SpriteRenderer _spriteRenderer;
    private OpenChallengeInteractable _interactable;

    public ChallengeStatusInEpisode Status
    {
        get
        {
            return EpisodeContext.Instance.GetChallengeStatus(ChallengeIndex);
        }
        set
        {
            EpisodeContext.Instance.SetChallengeStatus(ChallengeIndex, value);
            if(_spriteRenderer != null)
                Refresh();
        }
    }

    private void Reset()
    {
        var sr = GetComponent<SpriteRenderer>();
        _statusSprite = new Dictionary<MDS.Core.SceneManagement.ChallengeStatusInEpisode, Sprite>();
        _statusSprite.Add(ChallengeStatusInEpisode.Unavailable, sr.sprite);
        _statusSprite.Add(ChallengeStatusInEpisode.Available, sr.sprite);
        _statusSprite.Add(ChallengeStatusInEpisode.Done, sr.sprite);

        char lastLetter = name[name.Length-1];
        if (char.IsNumber(lastLetter))
        {
            _challengeIndex = int.Parse(lastLetter.ToString());
        }
    }

    protected override void Awake()
    {
        base.Awake();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _interactable = GetComponent<OpenChallengeInteractable>();
        _particles = GetComponentInChildren<ParticleSystem>().gameObject;
        _startChallengeMediatorPosition = transform.Find(StartChallengeMediatorPosition);
        _startChallengePlayerPosition = transform.Find(StartChallengePlayerPositionName);

    }

	private void Start()
    {
        Refresh();

        if (Status == ChallengeStatusInEpisode.Available)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            var m = GameObject.FindGameObjectWithTag("Mediator");


            p.transform.position = _startChallengePlayerPosition.position;
            m.transform.position = _startChallengeMediatorPosition.position;
        }
    }

    public void Refresh()
    {
        var status = Status;
        _spriteRenderer.sprite = _statusSprite[status];
        _particles.SetActive(status == ChallengeStatusInEpisode.Available);
        _interactable.enabled = (status == ChallengeStatusInEpisode.Available);
    }



#if UNITY_EDITOR
    [ShowInInspector]
    private bool debugging;

    public void OnDrawGizmos()
    {
        if(debugging)
            DrawGizmos();
    }
    public void OnDrawGizmosSelected()
    {
        DrawGizmos();
    }

    private void DrawGizmos()
    {
        if(Application.isPlaying) return;

        if(_startChallengeMediatorPosition == null)
            _startChallengeMediatorPosition = transform.Find(StartChallengeMediatorPosition);
        if(_startChallengePlayerPosition == null)
            _startChallengePlayerPosition = transform.Find(StartChallengePlayerPositionName);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(_startChallengePlayerPosition.position, 0.15f);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(_startChallengeMediatorPosition.position, 0.15f);
    }

#endif
}
