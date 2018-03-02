using Playmove.SopService;
using System;
using System.Collections.Generic;

namespace Playmove
{
    public class PYGameServiceManager : PYCrudBase<AplicativoVm, PYGameServiceManager.Game>
    {
        public class Game : PYDatabaseItem<AplicativoVm>
        {
            public Game()
            {
                _dataVm = new AplicativoVm();
            }

            public Game(AplicativoVm jogo) : this()
            {
                _dataVm = jogo ?? new AplicativoVm();
                Id = _dataVm.Id;
                Name = _dataVm.NomeApp;
                ProductGuid = _dataVm.ProdutoGuid;
                Path = _dataVm.Path;
            }

            public string ProductGuid;
            public string Path;

            public override AplicativoVm ConvertTo()
            {
                return _dataVm;
            }
        }

        public PYGameServiceManager() : base("Aplicativos")
        {
        }

        public void Get(string guid, Action<Game> callback)
        {
            string url = string.Format("{0}Get?productGuid={1}", ACTION, guid);
            _service.Get<AplicativoVm>(url, (request) => GetCallback(request, callback));
        }

        public override void Add(Game data, Action<SopRequest<AplicativoVm>> callback)
        {
            throw new NotImplementedException();
        }

        public override void Delete(long id, Action<SopRequest<AplicativoVm>> callback)
        {
            throw new NotImplementedException();
        }

        public override void Update(Game data, Action<SopRequest<AplicativoVm>> callback)
        {
            throw new NotImplementedException();
        }

        public void GetAllBundles(long id, Action<List<Game>> getBundlesCallback)
        {
            SopServiceFilter filter = new SopServiceFilter(SopServiceFilter.EFilters.id, id.ToString());
            string path = CreatePath("GetAllBundles", filter.Parameters);
            _service.Get<List<AplicativoVm>>(path, (apps) => GetAllCallback(apps, getBundlesCallback));
        }

        protected override void GetCallback(AplicativoVm request, Action<Game> callback)
        {
            callback(new Game(request));
        }

        protected override void GetAllCallback(List<AplicativoVm> requests, Action<List<Game>> listCallback)
        {
            List<Game> games = new List<Game>();
            for (int i = 0; i < requests.Count; i++)
            {
                games.Add(new Game(requests[i]));
            }
            listCallback(games);
        }
    }
}