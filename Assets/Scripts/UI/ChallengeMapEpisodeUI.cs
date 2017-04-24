using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChallengeMapEpisodeUI : MonoBehaviour
{

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

    void Start()
    {
        key = transform.Find("chave_base").gameObject;
        letter = transform.Find("cartinha").gameObject;
        locker = transform.Find("cadeado").gameObject;
        episodeIndexImage = transform.Find("EpisodeIndex").GetComponent<RawImage>();
        episodeImage = GetComponent<RawImage>();
        crystals = new GameObject[5];
        for(int i = 0; i < 5; i++)
        {
            crystals[i] = transform.Find("chave_base/MiniPedras/MiniPedra" + (i + 1).ToString()).gameObject;
        }



        episode = int.Parse(gameObject.name.Substring(gameObject.name.Length - 2, 1)) - 1;
        world = int.Parse(transform.parent.name.Substring(transform.parent.name.Length - 2, 1)) - 1;

        var libStatus = UserProfile.Instance.conceptMap.worlds[world]
                    .episodes[episode].liberationStatus;

        letter.SetActive(libStatus == EpisodeLiberationTypes.ALLOW_BY_TEACHER);
        locker.SetActive(libStatus == EpisodeLiberationTypes.BLOCK_BY_CONCEPT || libStatus == EpisodeLiberationTypes.BLOCK_BY_TEACHER);

        if(libStatus == EpisodeLiberationTypes.BLOCK_BY_CONCEPT || libStatus == EpisodeLiberationTypes.BLOCK_BY_TEACHER)
        {
            episodeImage.texture = episodeBlockedTexture;
            episodeIndexImage.texture = episodeIndexBlockedTexture;
            key.SetActive(false);
            return;
        }

        if(UserProfile.Instance.conceptMap.worlds[world].episodes[episode].CheckEpisodeComplete())
        {
            episodeImage.texture = episodeCompleteTexture;
            episodeIndexImage.texture = episodeIndexCompleteTexture;
        }

        for(int i = 0; i < 5; i++)
        {

            bool active = UserProfile.Instance.conceptMap
                    .worlds[world]
                    .episodes[episode]
                    .challenges[i].concept == ConceptTypes.CONCEPT_GREEN;
            crystals[i].SetActive(active);
        }

        

    }


}
