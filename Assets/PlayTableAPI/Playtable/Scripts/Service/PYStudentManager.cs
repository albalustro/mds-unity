using Playmove.SopService;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Playmove
{
    [Serializable]
    public class PYStudentManager : PYCrudBase<AlunoVm, PYStudentManager.Student>
    {
        /// <summary>
        /// Essa classe é usada para referencia interna nos componentes da API
        /// Para comunicação com API deve ser traduzido para a classe Playtable.Alunos
        /// </summary>
        [Serializable]
        public class Student : PYDatabaseItem<AlunoVm>
        {
            public Student()
            {
                _dataVm = new AlunoVm();
                Files = new List<PYFileManager.File>();
                CreateDate = UpdateDate = DateTime.Now;
            }

            public Student(string name, long classId) : this()
            {
                Name = name;
                ClassId = classId;
            }

            public Student(AlunoVm studentData) : this()
            {
                _dataVm = studentData;
                Id = _dataVm.Id;
                Name = _dataVm.Nome;
                CreateDate = _dataVm.DataCriacao;
                UpdateDate = _dataVm.DataAtualizacao;
                Trash = _dataVm.Lixeira;

                Guid = _dataVm.AlunoGuid;
                ClassId = _dataVm.TurmaId;
                Mail = _dataVm.Email;
                Excluded = _dataVm.Excluido;
                ExclusionDate = _dataVm.DataExclusao;

                //ClassId = studentData.TurmaId.HasValue ? (int)studentData.TurmaId : 0;
            }

            public string Guid;
            public long? ClassId;
            public string Mail;
            public bool Excluded;
            public DateTime? ExclusionDate;

            public List<PYFileManager.File> Files;

            [Obsolete]
            private List<string> GameNames = new List<string>();

            public void UpdateName(long classId)
            {
                ClassId = classId;
                UpdateDate = DateTime.Now;
            }

            public override AlunoVm ConvertTo()
            {
                _dataVm.Id = Id;
                _dataVm.Nome = Name;
                _dataVm.DataCriacao = CreateDate;
                _dataVm.DataAtualizacao = UpdateDate;
                _dataVm.Lixeira = Trash;

                _dataVm.AlunoGuid = Guid;
                _dataVm.TurmaId = ClassId;
                _dataVm.Email = Mail;
                _dataVm.Excluido = Excluded;
                _dataVm.DataExclusao = ExclusionDate;

                _dataVm.Arquivos = new List<ArquivoVm>();
                foreach (PYFileManager.File file in Files)
                    _dataVm.Arquivos.Add(file.ConvertTo());

                return _dataVm;
            }

            public override string ToString()
            {
                return string.Format("{0} - {1}", Id, Name);
            }
        }

        /// <summary>
        /// Armazena os nomes dos alunos adquiridos da playtable e serão esses os salvos na playtable
        /// </summary>
        private List<Student> _names = new List<Student>();

        public List<Student> Students
        {
            get { return _names; }
            private set { _names = value; }
        }

        public PYStudentManager() : base("Alunos")
        {
        }

        public string[] GetNames()
        {
            return Students.Select(n => n.Name).ToArray();
        }

        public void SaveName(string name = null, long classId = 0)
        {
            Student nameData = new Student();
            if (!string.IsNullOrEmpty(name))
            {
                nameData = Students.Find(n => n.Name == name);
                if (nameData == null)
                {
                    long nextId = Students.Count == 0 ? 0 : Students.LastOrDefault().Id + 1;
                    nameData = new Student(name, classId);
                    Students.Add(nameData);
                    Add(nameData, SaveSuccess);
                }
                else
                {
                    nameData.UpdateName(classId);
                    Update(nameData, SaveSuccess);
                }
            }
        }

        private void SaveSuccess(SopRequest<AlunoVm> success)
        {
            Debug.Log("Save: " + success);
            PlaytableWin32.Instance.Data.StudentsManager.GetAll(GetAllNamesCallback);
        }

        public void LoadStudents()
        {
            PlaytableWin32.Instance.Data.StudentsManager.GetAll(GetAllNamesCallback);
        }

        public Student GetName(string name)
        {
            return Students.Find(nd => nd.Name == name);
        }

        public override void Add(Student data, Action<SopRequest<AlunoVm>> callback)
        {
            AddRequest(data.ConvertTo(), callback);
        }

        public override void Delete(long id, Action<SopRequest<AlunoVm>> callback)
        {
            DeleteRequest(id, callback);
        }

        public override void Update(Student data, Action<SopRequest<AlunoVm>> callback)
        {
            UpdateRequest(data.ConvertTo(), callback);
        }

        public void GetAll(long classId, bool withoutClass, Action<List<Student>> students)
        {
            SopServiceFilter filter = new SopServiceFilter(SopServiceFilter.EFilters.turmaId, classId.ToString())
                .AddFilter(SopServiceFilter.EFilters.semTurma, withoutClass.ToString());
            base.GetAll(students, filter.Parameters);
        }

        public void GetAll(bool withoutClass, Action<List<Student>> students)
        {
            SopServiceFilter filter = new SopServiceFilter(SopServiceFilter.EFilters.semTurma, withoutClass.ToString());
            base.GetAll(students, filter.Parameters);
        }

        /// <summary>
        /// Envia os nomes que estao no arquivo e retorna todos que estao no banco junto com os novos migrados
        /// </summary>
        /// <param name="names"></param>
        /// <param name="callback"></param>
        public void MigrateNames(List<Student> names, Action<SopRequest<List<AlunoVm>>> callback)
        {
            List<AlunoVm> alunos = new List<AlunoVm>();
            foreach (Student name in names)
            {
                alunos.Add(name.ConvertTo());
            }
            _service.Post(ACTION + "MigrateNames", alunos, callback);
        }

        private void GetAllNamesCallback(List<Student> names)
        {
            Students = names;
        }

        private void MigrateNamesCallback(SopRequest<List<AlunoVm>> obj)
        {
            Students = new List<Student>();
            if (obj.Success)
            {
                foreach (AlunoVm aluno in obj.Model)
                {
                    Students.Add(new Student(aluno));
                }
            }
        }

        public void Get(string name, Action<Student> callback)
        {
            SopServiceFilter filter = new SopServiceFilter(SopServiceFilter.EFilters.nome, name);
            Get(callback, filter.Parameters);
        }

        protected override void GetCallback(AlunoVm request, Action<Student> callback)
        {
            callback(new Student(request ?? new AlunoVm()));
        }

        protected override void GetAllCallback(List<AlunoVm> requests, Action<List<Student>> listCallback)
        {
            List<Student> names = new List<Student>();
            foreach (AlunoVm a in requests)
                names.Add(new Student(a));
            listCallback(names);
        }

        public void Restore(long id, Action<SopRequest<AlunoVm>> callback)
        {
            _service.Post(ACTION + "Restore", id, callback);
        }
    }
}