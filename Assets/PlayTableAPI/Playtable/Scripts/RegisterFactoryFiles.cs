using Playmove;
using System.Collections.Generic;
using UnityEngine;

public class RegisterFactoryFiles : MonoBehaviour
{
    [SerializeField]
    private FactoryConfiguration _factoryConfiguration;

    private static bool _routineAlreadyUsed = false;

    private void Start()
    {
        if (_routineAlreadyUsed) return;
        if (PlaytableWin32.Instance.GameId == 0)
            PlaytableWin32.Instance.onGameValidate.AddListener(Initialize);
        else
            Initialize();
        _routineAlreadyUsed = true;
    }

    private void Initialize()
    {
        Debug.LogWarning("Registring default files");
        RegisterGroup(_factoryConfiguration.Groups);
    }

    private void RegisterGroup(List<PYGroupFilesManager.GameFilesGroup> groups, int index = 0)
    {
        if (groups.Count == 0 || index >= groups.Count)
        {
            Debug.LogWarning("No files to register");
            return;
        }

        groups[index].FactoryConfiguration = true;
        //groups[index].Localization = PlaytableWin32.Instance.Language;
        for (int i = 0; i < groups.Count; i++)
        {
            for (int j = 0; j < groups[i].AppFiles.Count; j++)
            {
                groups[i].AppFiles[j].AplicativoId = PlaytableWin32.Instance.GameId;
                //string physicalName = System.IO.Path.GetFileName(groups[i].AppFiles[j].File.FullPath);
                //groups[i].AppFiles[j].File.FullPath = System.IO.Path.Combine(TagManager.FACTORY_FILES_PATH, physicalName);
            }
        }
        PlaytableWin32.Instance.Data.GroupFilesManager.RegisterFactoryGroup(groups[index].ConvertTo(), (data) =>
        {
            if (!data.Success)
                Debug.LogWarning("Error on register files: " + data.Message);
            else
            {
                index++;
                if (index < groups.Count)
                    RegisterGroup(groups, index);
                else
                    Debug.LogWarning("Register default files finished");
            }
        }
        );
        //PlaytableWin32.Instance.Data.GroupFilesManager.Get(groups[index].Guid, (data) =>
        //{
        //    if (data != null && data.Id > 0)
        //    {
        //        groups[index].Id = data.Id;
        //        PlaytableWin32.Instance.Data.GroupFilesManager.Update(groups[index], (requestData) => RequestCallback(groups, index, requestData));
        //    }
        //    else
        //    {
        //        PlaytableWin32.Instance.Data.GroupFilesManager.Add(groups[index], (requestData) => RequestCallback(groups, index, requestData));
        //    }
        //});
    }

    private void RequestCallback(List<PYGroupFilesManager.GameFilesGroup> groups, int index, SopRequest<Playmove.SopService.GrupoArquivosVm> data)
    {
        if (!data.Success)
            Debug.LogWarning("Error on register files: " + data.Message);

        index++;
        if (index < groups.Count)
            RegisterGroup(groups, index);
        else
            Debug.LogWarning("Register default files finished");
    }
}