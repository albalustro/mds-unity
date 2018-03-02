using UnityEditor;

namespace Playmove
{
    public class PYAudioPreprocess : AssetPostprocessor
    {
        void OnPreprocessAudio()
        {
            AudioImporter audio = (AudioImporter)assetImporter;
            audio.loadInBackground = true;
            audio.preloadAudioData = true;
        }
    }
}