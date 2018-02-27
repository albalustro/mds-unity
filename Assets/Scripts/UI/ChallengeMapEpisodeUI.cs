using System.Collections;
using System.Collections.Generic;
using MDS.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChallengeMapEpisodeUI : MonoBehaviour
{

    private Texture episodeDefaultTexture;
    private Texture episodeIndexDefaultTexture;

    public Texture episodeCompleteTexture;
    public Texture episodeIndexCompleteTexture;

    public Texture episodeBlockedTexture;
    public Texture episodeIndexBlockedTexture;

    private GameObject letter;
    private GameObject locker;
    private RawImage episodeIndexImage;
    private RawImage episodeImage;
    private GameObject[] crystals;
    private GameObject key;
    private int world;
    private int episode;

#if PLAY_MOVE
    private void Awake()
    {
        episodeDefaultTexture = GetComponent<RawImage>().texture;
        episodeIndexDefaultTexture = transform.Find("EpisodeIndex").GetComponent<RawImage>().texture;
    }
#endif

    void RestoreToDefault()
    {
        key = transform.Find("chave_base").gameObject;
        key.SetActive(true);
        letter = transform.Find("cartinha").gameObject;
        locker = transform.Find("cadeado").gameObject;
        episodeIndexImage = transform.Find("EpisodeIndex").GetComponent<RawImage>();
        episodeImage = GetComponent<RawImage>();
#if PLAY_MOVE
        episodeIndexImage.texture = episodeIndexDefaultTexture;
        episodeImage.texture = episodeDefaultTexture;
#endif

        crystals = new GameObject[5];
        for (int i = 0; i < 5; i++)
        {
            crystals[i] = transform.Find("chave_base/MiniPedras/MiniPedra" + (i + 1).ToString()).gameObject;
        }

        episode = int.Parse(gameObject.name.Substring(gameObject.name.Length - 2, 1)) - 1;
        world = int.Parse(transform.parent.name.Substring(transform.parent.name.Length - 2, 1)) - 1;
    }

    void OnEnable()
    {
#if !PLAY_MOVE
        ConceptMap activeMap = UserProfile.Instance.conceptMap;
#else
        ConceptMap activeMap;
        if (SceneManager.GetActiveScene().IsLogin())
        {
            activeMap = FindObjectOfType<PlayMoveChallengeMapUI>().activeMap;
        }
        else
        {
            activeMap = UserProfile.Instance.conceptMap;
        }
#endif
        RestoreToDefault();
        var libStatus = activeMap.worlds[world].episodes[episode].liberationStatus;

#if !PLAY_MOVE
        letter.SetActive(libStatus == EpisodeLiberationTypes.ALLOW_BY_TEACHER);
#else
        letter.SetActive(false);
#endif
        locker.SetActive(libStatus == EpisodeLiberationTypes.BLOCK_BY_CONCEPT || libStatus == EpisodeLiberationTypes.BLOCK_BY_TEACHER);

        var episodeComplete = activeMap.worlds[world].episodes[episode].CheckEpisodeComplete();

        if ((libStatus == EpisodeLiberationTypes.BLOCK_BY_CONCEPT || 
           libStatus == EpisodeLiberationTypes.BLOCK_BY_TEACHER) && !episodeComplete)
        {
            episodeImage.texture = episodeBlockedTexture;
            episodeIndexImage.texture = episodeIndexBlockedTexture;
            key.SetActive(false);
            return;
        }

        if(episodeComplete)
        {
            episodeImage.texture = episodeCompleteTexture;
            episodeIndexImage.texture = episodeIndexCompleteTexture;
        }

        for(int i = 0; i < 5; i++)
        {
            bool active = activeMap
                    .worlds[world]
                    .episodes[episode]
                    .challenges[i].concept == ConceptTypes.CONCEPT_GREEN;
            crystals[i].SetActive(active);
        }
    }
}
