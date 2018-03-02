using Playmove;
using Playtable.Models.Playtable;
using System;
using System.Collections.Generic;

public class PYPlaytableConfigurationManager
{
    protected string ACTION = "Configuracoes/";

    protected SopServiceMethods _service = new SopServiceMethods();

    public void Get<T>(ConfiguracaoNome configuration, Action<T> callback)
    {
        SopServiceFilter filters = new SopServiceFilter(SopServiceFilter.EFilters.configuracaoNome, configuration.ToString());
        string path = CreatePath("Get", filters.Parameters);
        _service.Get(path, callback);
    }

    public void GetUpdateAutomaticClassrooms(Action<ConfiguracaoVm> value)
    {
        Get(ConfiguracaoNome.atualizacaoAutomaticaDeTurmas, value);
    }

    public void GetShowClassroomInGame(Action<ConfiguracaoVm> value)
    {
        Get(ConfiguracaoNome.turmasHabilitadas, value);
    }

    public void SetConfiguration(ConfiguracaoNome configuration, ValorTipo type, string value, Action<bool> callback)
    {
        ConfiguracaoVm vm = new ConfiguracaoVm()
        {
            Nome = configuration.ToString(),
            Tipo = type,
            Valor = value
        };
        _service.Post(ACTION + "SetConfiguration", vm, (x) => callback(x.Success));
    }

    public void SetUpdateAutomaticClassrooms(bool value, Action<bool> callback)
    {
        SetConfiguration(ConfiguracaoNome.atualizacaoAutomaticaDeTurmas, ValorTipo.Bool, value.ToString(), callback);
    }

    public void SetClassroomsInGame(bool value, Action<bool> callback)
    {
        SetConfiguration(ConfiguracaoNome.turmasHabilitadas, ValorTipo.Bool, value.ToString(), callback);
    }

    private string CreatePath(string method, Dictionary<string, string> filters)
    {
        string path = string.Format("{0}{1}?", ACTION, method);

        foreach (string key in filters.Keys)
        {
            path += string.Format("{0}={1}&", key, filters[key]);
        }

        path = path.Remove(path.Length - 1);
        return path;
    }
}