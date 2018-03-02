using Playmove.SopService;
using Playmove.SopService.Models.Arquivos;
using System;
using System.Collections.Generic;

namespace Playmove
{
    public class PYGroupFilesManager : PYCrudBase<GrupoArquivosVm, PYGroupFilesManager.GameFilesGroup>
    {
        [Serializable]
        public class GameFilesGroup : PYDatabaseItem<GrupoArquivosVm>
        {
            public GameFilesGroup()
            {
                _dataVm = new GrupoArquivosVm();
                AppFiles = new List<PYFileApplicationManager.FileApplication>();
            }

            public GameFilesGroup(GrupoArquivosVm group) : this()
            {
                _dataVm = group ?? new GrupoArquivosVm();
                Id = _dataVm.Id;
                Name = _dataVm.Nome;
                CreateDate = _dataVm.DataCriacao;
                UpdateDate = _dataVm.DataAtualizacao;
                Trash = _dataVm.Lixeira;

                Localization = _dataVm.Localizacao;
                FactoryConfiguration = _dataVm.ConfiguracaoDeFabrica;
                Visible = _dataVm.Visivel;
                ThemeImgId = _dataVm.GrupoImgId;
                ThemeAudioId = _dataVm.GrupoAudioId;

                Guid = _dataVm.Guid;

                AppFiles = new List<PYFileApplicationManager.FileApplication>();
                foreach (ArquivoAplicativoVm item in _dataVm.ArquivosAplicativo)
                    AppFiles.Add(new PYFileApplicationManager.FileApplication(item));
            }

            private string _localization = null;

            public string Localization
            {
                get
                {
                    if (_localization == null)
                        _localization = PlaytableWin32.Instance.Language;
                    return _localization;
                }
                set { _localization = value; }
            }

            public bool FactoryConfiguration;
            public bool Visible;
            public long? ThemeImgId;
            public long? ThemeAudioId;
            public string Guid;
            public List<PYFileApplicationManager.FileApplication> AppFiles;

            public override GrupoArquivosVm ConvertTo()
            {
                _dataVm.Id = Id;
                _dataVm.Nome = Name;
                _dataVm.DataCriacao = CreateDate;
                _dataVm.DataAtualizacao = UpdateDate;
                _dataVm.Lixeira = Trash;

                _dataVm.Localizacao = Localization;
                _dataVm.ConfiguracaoDeFabrica = FactoryConfiguration;
                _dataVm.Visivel = Visible;
                _dataVm.GrupoImgId = ThemeImgId;
                _dataVm.GrupoAudioId = ThemeAudioId;

                _dataVm.Guid = Guid;

                _dataVm.ArquivosAplicativo = new List<ArquivoAplicativoVm>();
                foreach (PYFileApplicationManager.FileApplication item in AppFiles)
                    _dataVm.ArquivosAplicativo.Add(item.ConvertTo());

                return _dataVm;
            }
        }

        public PYGroupFilesManager() : base("GrupoArquivos")
        {
        }

        public override void Add(GameFilesGroup data, Action<SopRequest<GrupoArquivosVm>> callback)
        {
            AddRequest(data.ConvertTo(), callback);
        }

        public override void Delete(long id, Action<SopRequest<GrupoArquivosVm>> callback)
        {
            DeleteRequest(id, callback);
        }

        public override void Update(GameFilesGroup data, Action<SopRequest<GrupoArquivosVm>> callback)
        {
            UpdateRequest(data.ConvertTo(), callback);
        }

        public void GetAll(long gameId, Action<List<GameFilesGroup>> callback)
        {
            SopServiceFilter filters = new SopServiceFilter(SopServiceFilter.EFilters.aplicativoId, gameId.ToString());
            GetAll(callback, filters.Parameters);
        }

        public void GetAll(long gameId, bool defaultFactory, Action<List<GameFilesGroup>> callback)
        {
            SopServiceFilter filters = new SopServiceFilter(SopServiceFilter.EFilters.aplicativoId, gameId.ToString())
                .AddFilter(SopServiceFilter.EFilters.configuracaoFabrica, defaultFactory.ToString());
            GetAll(callback, filters.Parameters);
        }

        public void RemoveFromTrash(long id, Action<SopRequest<GrupoArquivosVm>> callback)
        {
            _service.Post(ACTION + "RemoveFromTrash", id, callback);
        }

        public void Get(string guid, Action<GameFilesGroup> callback)
        {
            SopServiceFilter filter = new SopServiceFilter(SopServiceFilter.EFilters.guid, guid);
            Get(callback, filter.Parameters);
        }

        public void RegisterFactoryGroup(GrupoArquivosVm group, Action<SopRequest<GrupoArquivosVm>> callback)
        {
            _service.Post(ACTION + "RegisterFactoryGroup", group, callback);
        }

        public void GetFactoryFiles(Action<List<GameFilesGroup>> callback)
        {
            SopServiceFilter filter = new SopServiceFilter(SopServiceFilter.EFilters.aplicativoId, PlaytableWin32.Instance.GameId.ToString())
                .AddFilter(SopServiceFilter.EFilters.localizacao, PlaytableWin32.Instance.Language);
            GetAll(callback, filter.Parameters);
        }

        public void SetVisibility(long id, bool visibility, Action<GrupoArquivosVm> callback)
        {
            SopServiceFilter filter = new SopServiceFilter(SopServiceFilter.EFilters.id, id.ToString())
                .AddFilter(SopServiceFilter.EFilters.visibilidade, visibility.ToString());
            string path = CreatePath("SetVisibility", filter.Parameters);
            _service.Get(path, callback);
        }

        public void SetFactoryVisibility(long gameId, Action<List<GrupoArquivosVm>> callback)
        {
            SopServiceFilter filter = new SopServiceFilter(SopServiceFilter.EFilters.aplicativoId, gameId.ToString());
            string path = CreatePath("SetFactoryVisibility", filter.Parameters);
            _service.Get(path, callback);
        }

        protected override void GetCallback(GrupoArquivosVm request, Action<GameFilesGroup> callback)
        {
            callback(new GameFilesGroup(request));
        }

        protected override void GetAllCallback(List<GrupoArquivosVm> requests, Action<List<GameFilesGroup>> listCallback)
        {
            List<GameFilesGroup> configs = new List<GameFilesGroup>();
            if (requests != null)
                foreach (GrupoArquivosVm item in requests)
                    configs.Add(new GameFilesGroup(item));

            listCallback(configs);
        }

        public void DeleteWithoutLogicExclusion(long id, Action<SopRequest<GameFilesGroup>> callback)
        {
            _service.Post(ACTION + "DeleteWithoutLogicExclusion", id, callback);
        }
    }
}