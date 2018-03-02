using Playmove;
using Playmove.SopService.Models.Arquivos;
using System;
using System.Collections.Generic;

public class PYFileApplicationManager : PYCrudBase<ArquivoAplicativoVm, PYFileApplicationManager.FileApplication>
{
    public PYFileApplicationManager() : base("ArquivoAplicativos")
    {
    }

    [Serializable]
    public class FileApplication : PYDatabaseItem<ArquivoAplicativoVm>
    {
        public FileApplication()
        {
            _dataVm = new ArquivoAplicativoVm();
        }

        public FileApplication(ArquivoAplicativoVm arquivoAplicativoVm) : this()
        {
            _dataVm = arquivoAplicativoVm;
            Id = _dataVm.Id;
            CreateDate = _dataVm.DataCriacao;
            UpdateDate = _dataVm.DataAtualizacao;
            Trash = _dataVm.Lixeira;

            Grouping = arquivoAplicativoVm.Agrupamento;
            GroupApplicationId = arquivoAplicativoVm.GrupoAplicativoId;
            File = new PYFileManager.File(arquivoAplicativoVm.Arquivo);
            Data = _dataVm.Configuracao;
            AplicativoId = _dataVm.AplicativoId;

            StudentId = _dataVm.AlunoId;

            File = new PYFileManager.File(_dataVm.Arquivo);
            FileId = File.Id;
        }

        public FileApplication(PYFileManager.File file, long appId, string grouping = "", string data = "") : this()
        {
            AplicativoId = appId;
            File = file;
            File.Localization =
            Grouping = grouping;
            Data = data;
        }

        public FileApplication(PYFileManager.File file, long appId, long groupId, string grouping = "", string data = "") : this(file, appId, grouping, data)
        {
            GroupApplicationId = groupId;
        }

        public override string Name
        {
            get { return File.Name; }
            set { File.Name = value; }
        }

        public string Grouping;
        public string Data;
        public long AplicativoId;
        public long? FileId;
        public PYFileManager.File File;
        public long? GroupApplicationId;
        public long? StudentId;

        public override ArquivoAplicativoVm ConvertTo()
        {
            _dataVm.Id = Id;
            _dataVm.DataCriacao = CreateDate;
            _dataVm.DataAtualizacao = UpdateDate;
            _dataVm.Lixeira = Trash;

            _dataVm.Agrupamento = Grouping;
            _dataVm.Configuracao = Data;
            _dataVm.AplicativoId = AplicativoId;
            _dataVm.Arquivo = File.ConvertTo();
            _dataVm.GrupoAplicativoId = GroupApplicationId;

            _dataVm.AlunoId = StudentId;

            return _dataVm;
        }

        public override string ToString()
        {
            return string.Format("Id{0} App{1} Group{2}", Id, AplicativoId, GroupApplicationId);
        }
    }

    public override void Add(FileApplication data, Action<SopRequest<ArquivoAplicativoVm>> callback)
    {
        AddRequest(data.ConvertTo(), callback);
    }

    public override void Delete(long id, Action<SopRequest<ArquivoAplicativoVm>> callback)
    {
        DeleteRequest(id, callback);
    }

    public override void Update(FileApplication data, Action<SopRequest<ArquivoAplicativoVm>> callback)
    {
        UpdateRequest(data.ConvertTo(), callback);
    }

    public void GetAllWithFilter(ArquivoGaleriaFiltroVm filtro, Action<Dictionary<string, List<FileApplication>>> callback)
    {
        Dictionary<string, string> filters = new Dictionary<string, string>();
        filters.Add("AplicativoId", filtro.AplicativoId.ToString());
        filters.Add("HistoricoId", filtro.HistoricoId.ToString());
        filters.Add("TurmaId", filtro.TurmaId.ToString());
        filters.Add("AlunoId", filtro.AlunoId.ToString());
        filters.Add("AgrupamentoId", filtro.AgrupamentoId.ToString());

        string method = CreatePath("GetAllWithFilter", filters);
        _service.Get<Dictionary<string, List<ArquivoAplicativoVm>>>(method, (data) =>
            {
                if (callback != null)
                    callback(TranslateGetAllWithFilter(data));
            });
    }

    public void GetAllDropdowns(ArquivoGaleriaFiltroVm filtro, Action<Dictionary<string, List<object>>> callback)
    {
        Dictionary<string, string> filters = new Dictionary<string, string>();
        filters.Add("AplicativoId", filtro.AplicativoId.ToString());
        filters.Add("HistoricoId", filtro.HistoricoId.ToString());
        filters.Add("TurmaId", filtro.TurmaId.ToString());
        filters.Add("AlunoId", filtro.AlunoId.ToString());
        filters.Add("AgrupamentoId", filtro.AgrupamentoId.ToString());

        string method = CreatePath("GetAllDropdowns", filters);
        _service.Get<Dictionary<string, List<object>>>(method, (data) =>
            {
                if (callback != null)
                    callback(TranslateGetAllDropdowns(data));
            });
    }

    protected Dictionary<string, List<FileApplication>> TranslateGetAllWithFilter(Dictionary<string, List<ArquivoAplicativoVm>> data)
    {
        Dictionary<string, List<FileApplication>> filesData = new Dictionary<string, List<FileApplication>>();
        if (data == null)
            return filesData;

        foreach (var group in data)
        {
            if (!filesData.ContainsKey(group.Key))
                filesData[group.Key] = new List<FileApplication>();

            foreach (var vm in group.Value)
                filesData[group.Key].Add(new FileApplication(vm));
        }

        return filesData;
    }

    protected Dictionary<string, List<object>> TranslateGetAllDropdowns(Dictionary<string, List<object>> data)
    {
        Dictionary<string, List<object>> filesData = new Dictionary<string, List<object>>();
        if (data == null)
            return filesData;

        foreach (var group in data)
        {
            if (!filesData.ContainsKey(group.Key))
                filesData[group.Key] = new List<object>();

            if (group.Key == "Aplicativo")
            {
                foreach (var vm in group.Value)
                    filesData[group.Key].Add(new PYGameServiceManager.Game(Newtonsoft.Json.JsonConvert.DeserializeObject<Playmove.SopService.AplicativoVm>(vm.ToString())));
            }
            else if (group.Key == "Historico")
            {
                foreach (var vm in group.Value)
                    filesData[group.Key].Add(new PYHistoricManager.Historic(Newtonsoft.Json.JsonConvert.DeserializeObject<Playmove.SopService.HistoricoVm>(vm.ToString())));
            }
            else if (group.Key == "Turma")
            {
                foreach (var vm in group.Value)
                    if (vm != null)
                        filesData[group.Key].Add(new PYClassManager.Classroom(Newtonsoft.Json.JsonConvert.DeserializeObject<Playmove.SopService.TurmaVm>(vm.ToString())));
            }
            else if (group.Key == "Aluno")
            {
                foreach (var vm in group.Value)
                    filesData[group.Key].Add(new PYStudentManager.Student(Newtonsoft.Json.JsonConvert.DeserializeObject<Playmove.SopService.AlunoVm>(vm.ToString())));
            }
            else if (group.Key == "Agrupamento")
            {
                foreach (var vm in group.Value)
                    filesData[group.Key].Add(new FileApplication(Newtonsoft.Json.JsonConvert.DeserializeObject<ArquivoAplicativoVm>(vm.ToString())));
            }
        }

        return filesData;
    }

    public void GetAll(long aplicativoId, Action<List<FileApplication>> callback)
    {
        SopServiceFilter filter = new SopServiceFilter(SopServiceFilter.EFilters.aplicativoId, aplicativoId.ToString());
        GetAll(callback, filter.Parameters);
    }

    public void GetAll(long aplicativoId, string language, Action<List<FileApplication>> callback)
    {
        SopServiceFilter filter = new SopServiceFilter(SopServiceFilter.EFilters.aplicativoId, aplicativoId.ToString())
            .AddFilter(SopServiceFilter.EFilters.localizacao, language);
        GetAll(callback, filter.Parameters);
    }

    public void GetAll(long aplicativoId, bool arquivosComAluno, Action<List<FileApplication>> callback)
    {
        SopServiceFilter filter = new SopServiceFilter(SopServiceFilter.EFilters.aplicativoId, aplicativoId.ToString())
            .AddFilter(SopServiceFilter.EFilters.arquivosComAluno, arquivosComAluno.ToString());
        GetAll(callback, filter.Parameters);
    }

    public void GetAllWithStudents(bool filesWithStudents, Action<List<FileApplication>> callback)
    {
        SopServiceFilter filter = new SopServiceFilter(SopServiceFilter.EFilters.arquivosComAluno, filesWithStudents.ToString());
        string path = CreatePath("GetAllWithStudents", filter.Parameters);
        _service.Get<List<ArquivoAplicativoVm>>(path, (requests) => GetAllCallback(requests, callback));
    }

    protected override void GetCallback(ArquivoAplicativoVm request, Action<FileApplication> callback)
    {
        callback(new FileApplication(request));
    }

    protected override void GetAllCallback(List<ArquivoAplicativoVm> requests, Action<List<FileApplication>> listCallback)
    {
        List<FileApplication> files = new List<FileApplication>();
        for (int i = 0; i < requests.Count; i++)
        {
            files.Add(new FileApplication(requests[i]));
        }
        listCallback(files);
    }

    public void DeleteWithoutLogicExclusion(long id, Action<SopRequest<ArquivoAplicativoVm>> callback)
    {
        _service.Post(ACTION + "DeleteWithoutLogicExclusion", id, callback);
    }
}