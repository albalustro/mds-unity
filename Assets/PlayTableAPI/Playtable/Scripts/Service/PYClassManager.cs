using Playmove.SopService;
using System;
using System.Collections.Generic;

namespace Playmove
{
    /// <summary>
    /// Gerenciador de turmas
    /// </summary>
    public class PYClassManager : PYCrudBase<TurmaVm, PYClassManager.Classroom>
    {
        public PYClassManager() : base("Turmas")
        {
        }

        [Serializable]
        public class Classroom : PYDatabaseItem<TurmaVm>
        {
            public Classroom()
            {
                Id = 0;
                _dataVm = new TurmaVm();
            }

            public Classroom(string name, long grandeId, int order) : this()
            {
                Name = name;
                GradeId = grandeId;
                Order = order;
            }

            public Classroom(TurmaVm turma) : this()
            {
                _dataVm = turma;
                Id = _dataVm.Id;
                Name = _dataVm.Nome;
                CreateDate = _dataVm.DataCriacao;
                UpdateDate = _dataVm.DataAtualizacao;
                Trash = _dataVm.Lixeira;

                Order = _dataVm.Ordem;
                GradeId = _dataVm.SerieId;
                MailNotification = _dataVm.EmailNotficacao;
                Completed = _dataVm.Concluida;
                Excluded = _dataVm.Excluido;
                ExclusionDate = _dataVm.DataExclusao;

                GradeName = turma.SerieNome;
            }

            public int Order; // Ordem de vizualização
            public long? GradeId; // Identificador da Serie
            public string GradeName; // Nome da turma
            public string MailNotification;
            public bool Completed;
            public bool Excluded;
            public DateTime? ExclusionDate;

            public string FullName
            {
                get
                {
                    string fullname = Name;
                    if (!string.IsNullOrEmpty(GradeName))
                        fullname += " (" + GradeName + ")";
                    return fullname;
                }
            }

            public override TurmaVm ConvertTo()
            {
                _dataVm.Id = Id;
                _dataVm.Nome = Name;
                _dataVm.DataCriacao = CreateDate;
                _dataVm.DataAtualizacao = UpdateDate;
                _dataVm.Lixeira = Trash;

                _dataVm.Ordem = Order;
                _dataVm.SerieId = GradeId;
                _dataVm.EmailNotficacao = MailNotification;
                _dataVm.Concluida = Completed;
                _dataVm.Excluido = Excluded;
                _dataVm.DataExclusao = ExclusionDate;

                return _dataVm;
            }

            public void SetGrade(long gradeId)
            {
                GradeId = gradeId;
            }

            public override string ToString()
            {
                return string.Format("SerieId:{0} - Turma:{1}.{2}", GradeId, Id, Name);
            }
        }

        public override void Add(Classroom data, Action<SopRequest<TurmaVm>> callback)
        {
            AddRequest(data.ConvertTo(), callback);
        }

        public override void Delete(long id, Action<SopRequest<TurmaVm>> callback)
        {
            DeleteRequest(id, callback);
        }

        public override void Update(Classroom data, Action<SopRequest<TurmaVm>> callback)
        {
            UpdateRequest(data.ConvertTo(), callback);
        }

        protected override void GetCallback(TurmaVm request, Action<Classroom> callback)
        {
            callback(new Classroom(request));
        }

        protected override void GetAllCallback(List<TurmaVm> requests, Action<List<Classroom>> listCallback)
        {
            List<Classroom> classes = new List<Classroom>();
            for (int i = 0; i < requests.Count; i++)
            {
                classes.Add(new Classroom(requests[i]));
            }
            listCallback(classes);
        }
    }
}