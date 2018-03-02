using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Playmove
{

    public abstract class PYImage : PYComponentBundle
    {
        public PYBundleAssetTag AssetTag;

        [Header("PYImage")]
        public bool UseSharedMaterial = false;

        public override void UpdateComponent()
        {
            UpdateData.DefaultComponentValue = Image;
            if (PYBundleManager.Instance.IsReady && UpdateData.UpdateFromBundle)
            {
                Sprite sprite = GetAsset<Sprite>(AssetTag.Tag);
                if (sprite != null)
                {
                    if ((Image == null) && (this is PYImageSprite))
                    {
                        PYImageSprite pySprite = (PYImageSprite)this;
                        pySprite.Sprite = sprite;
                    }
                    else
                        Image = sprite.texture;
                }
            }
        }

        public override void RestoreComponent()
        {
            if (UpdateData.UpdateFromBundle)
            {
#if !UNITY_EDITOR
            if (UpdateData.DefaultComponentValue == null)
                Image = null;
            else
                Image = (Texture2D)UpdateData.DefaultComponentValue;
#endif
            }
        }

        public abstract Texture2D Image { get; set; }
        public abstract Material Material { get; set; }

    }
}