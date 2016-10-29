using UnityEngine;
using System.Collections;

public class ResetGlobal : MonoBehaviour {

	void Start () {
        DSGlobal.game = "0";
        DSGlobal.world = "0";
        DSGlobal.episode = "0";
        DSGlobal.minigame = "";
        DSGlobal.slug = "";
        DSGlobal.id = 0;
        DSGlobal.challenge = 0;
        DSGlobal.isActive = false;
    }
}
