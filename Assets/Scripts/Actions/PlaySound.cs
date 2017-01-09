using System.Collections;
using System.Collections.Generic;
using MDS.Actions;
using MDS.Core;
using UnityEngine;

public class PlaySound : BaseAction
{

	public AudioClip soundFXClip;
	public AudioClip themeClip;
	public AudioClip voiceOverClip;
    
    public override IEnumerator Execute()
    {
        yield return base.Execute();

		if (soundFXClip != null)
			AudioController.Instance.PlaySoundFX (soundFXClip);

		if (soundFXClip != null)
			AudioController.Instance.PlayTheme (themeClip);

		if (soundFXClip != null)
			AudioController.Instance.PlayVoiceOver (voiceOverClip);
    }

}