using Playmove.SopService;
using System;
using System.Collections.Generic;

namespace Playmove
{
    /// <summary>
    /// Gerenciador de Series, a serie é o agrupador de turmas e mantenedor do historico
    /// </summary>
    public class PYGradeManager : PYCrudBase<SerieVm, PYGradeManager.Grade>
    {
        [Serializable]
        public class Grade : PYDatabaseItem<SerieVm>
        {
            public Grade()
            {
                _dataVm = new SerieVm();
            }

            public Grade(string name) : this()
            {
                Name = name;
            }

            public Grade(SerieVm serie) : this()
            {
                _dataVm = serie;
                Id = _dataVm.Id;
                Name = _dataVm.Nome;
                Localization = _dataVm.Localizacao;
                CreateDate = _dataVm.DataCriacao;
                UpdateDate = _dataVm.DataAtualizacao;
                Trash = _dataVm.Lixeira;

                Order = _dataVm.Ordem;
                HistoricId = _dataVm.HistoricoId;
                Excluded = _dataVm.Excluido;
                ExclusionDate = _dataVm.DataExclusao;
            }

            public string Localization;
            public int Order;
            public long HistoricId;
            public bool Excluded;
            public DateTime? ExclusionDate;

            public override SerieVm ConvertTo()
            {
                _dataVm.Id = Id;
                _dataVm.Nome = Name;
                _dataVm.Localizacao = Localization;
                _dataVm.DataCriacao = CreateDate;
                _dataVm.DataAtualizacao = UpdateDate;
                _dataVm.Lixeira = Trash;

                _dataVm.Ordem = Order;
                _dataVm.HistoricoId = HistoricId;
                _dataVm.Excluido = Excluded;
                _dataVm.DataExclusao = ExclusionDate;

                return _dataVm;
            }
        }

        public PYGradeManager() : base("Series")
        {
        }

        public override void Add(Grade data, Action<SopRequest<SerieVm>> callback)
        {
            AddRequest(data.ConvertTo(), callback);
        }

        public override void Delete(long id, Action<SopRequest<SerieVm>> callback)
        {
            DeleteRequest(id, callback);
        }

        public override void Update(Grade data, Action<SopRequest<SerieVm>> callback)
        {
            UpdateRequest(data.ConvertTo(), callback);
        }

        protected override void GetCallback(SerieVm request, Action<Grade> callback)
        {
            callback(new Grade(request));
        }

        protected override void GetAllCallback(List<SerieVm> requests, Action<List<Grade>> listCallback)
        {
            List<Grade> grades = new List<Grade>();
            foreach (SerieVm s in requests)
                grades.Add(new Grade(s));
            listCallback(grades);
        }

        public void GetAll(string localization, Action<List<Grade>> callback)
        {
            SopServiceFilter filter = new SopServiceFilter(SopServiceFilter.EFilters.localizacao, localization);
            GetAll(callback, filter.Parameters);
        }
    }
}