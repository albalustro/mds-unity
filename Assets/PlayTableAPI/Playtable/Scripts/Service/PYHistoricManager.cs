using Playmove.SopService;
using System;
using System.Collections.Generic;

namespace Playmove
{
    public class PYHistoricManager : PYCrudBase<HistoricoVm, PYHistoricManager.Historic>
    {
        [Serializable]
        public class Historic : PYDatabaseItem<HistoricoVm>
        {
            public Historic()
            {
                _dataVm = new HistoricoVm();
            }

            public Historic(HistoricoVm historic) : this()
            {
                _dataVm = historic;
                Id = _dataVm.Id;
                Name = _dataVm.Nome;
                CreateDate = _dataVm.DataCriacao;

                Revision = _dataVm.Revisao;
                Description = _dataVm.Descricao;
                AutomaticAdvance = _dataVm.AvancoAutomatico;
                Months = _dataVm.Meses;
                Excluded = _dataVm.Excluido;
                ExclusionDate = _dataVm.DataExclusao;
            }

            #region NotImplemented Property

            public override DateTime UpdateDate
            {
                get { throw new NotImplementedException("This property should not be used in type PYHistoricManager.Historic"); }
                set { throw new NotImplementedException("This property should not be used in type PYHistoricManager.Historic"); }
            }

            public override bool Trash
            {
                get { throw new NotImplementedException("This property should not be used in type PYHistoricManager.Historic"); }
                set { throw new NotImplementedException("This property should not be used in type PYHistoricManager.Historic"); }
            }

            #endregion NotImplemented Property

            public long Revision;
            public string Description;
            public bool AutomaticAdvance;
            public int Months;
            public bool Excluded;
            public DateTime? ExclusionDate;

            public override HistoricoVm ConvertTo()
            {
                _dataVm.Id = Id;
                _dataVm.Nome = Name;
                _dataVm.DataCriacao = CreateDate;

                _dataVm.Revisao = Revision;
                _dataVm.Descricao = Description;
                _dataVm.AvancoAutomatico = AutomaticAdvance;
                _dataVm.Meses = Months;
                _dataVm.Excluido = Excluded;
                _dataVm.DataExclusao = ExclusionDate;

                return _dataVm;
            }
        }

        public PYHistoricManager() : base("Historicos")
        {
        }

        public override void Add(Historic data, Action<SopRequest<HistoricoVm>> callback)
        {
            AddRequest(data.ConvertTo(), callback);
        }

        public override void Delete(long id, Action<SopRequest<HistoricoVm>> callback)
        {
            DeleteRequest(id, callback);
        }

        public override void Update(Historic data, Action<SopRequest<HistoricoVm>> callback)
        {
            UpdateRequest(data.ConvertTo(), callback);
        }

        protected override void GetCallback(HistoricoVm request, Action<Historic> callback)
        {
            callback(new Historic(request));
        }

        protected override void GetAllCallback(List<HistoricoVm> requests, Action<List<Historic>> listCallback)
        {
            List<Historic> historics = new List<Historic>();
            foreach (HistoricoVm historico in requests)
                historics.Add(new Historic(historico));
            listCallback(historics);
        }
    }
}