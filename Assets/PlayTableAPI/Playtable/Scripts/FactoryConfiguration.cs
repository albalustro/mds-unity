using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Playmove;

[CreateAssetMenu(fileName = "FactoryFiles", menuName = "SOP/FactoryFiles", order = 1)]
public class FactoryConfiguration : ScriptableObject
{
    public List<PYGroupFilesManager.GameFilesGroup> Groups = new List<PYGroupFilesManager.GameFilesGroup>();
    public List<PYFileApplicationManager.FileApplication> FilesWithoutGroup = new List<PYFileApplicationManager.FileApplication>();
}
