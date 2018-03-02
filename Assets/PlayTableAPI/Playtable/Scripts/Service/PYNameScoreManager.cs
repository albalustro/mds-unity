using Playmove.SopService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Playmove
{
    /// <summary>
    /// Classe pronta para vincular um placar genérico para serem guardados em banco, ainda em planejamento e testes.
    /// </summary>
    public class PYNameScoreManager : PYCrudBase<SopService.AlunoPlacarJogo, PYNameScoreManager.StudentScore>
    {
        [Serializable]
        public class StudentScore
        {
            public long StudentId;
            public ScoreData Score;

            [Serializable]
            public class ScoreData
            {
            }

            public StudentScore()
            {
            }

            public StudentScore(SopService.AlunoPlacarJogo dto)
            {
                StudentId = dto.AlunoId;
                MemoryStream memStream = new MemoryStream();
                BinaryFormatter binaryForm = new BinaryFormatter();
                memStream.Write(dto.Placar, 0, dto.Placar.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                Score = binaryForm.Deserialize(memStream) as ScoreData;
            }

            public SopService.AlunoPlacarJogo ConvertTo()
            {
                SopService.AlunoPlacarJogo data = new SopService.AlunoPlacarJogo();
                data.AlunoId = StudentId;
                data.JogoId = PlaytableWin32.Instance.GameId;
                BinaryFormatter b = new BinaryFormatter();
                MemoryStream m = new MemoryStream();
                b.Serialize(m, Score);
                data.Placar = m.GetBuffer();
                return data;
            }
        }

        public List<StudentScore> StudentScores = new List<StudentScore>();

        public PYNameScoreManager() : base("AlunoDadosJogo")
        {
            SopServiceFilter filter = new SopServiceFilter(SopServiceFilter.EFilters.aplicativoId, PlaytableWin32.Instance.GameId.ToString());
            GetAll(GetAllScoreByGame, filter.Parameters);
        }

        public StudentScore GetScore(int studentId)
        {
            return StudentScores.Find((s) => s.StudentId == studentId);
        }

        private void GetAllScoreByGame(List<StudentScore> scores)
        {
            StudentScores = scores;
        }

        public override void Add(StudentScore data, Action<SopRequest<AlunoPlacarJogo>> callback)
        {
            AddRequest(data.ConvertTo(), callback);
        }

        public override void Delete(long id, Action<SopRequest<AlunoPlacarJogo>> callback)
        {
            DeleteRequest(id, callback);
        }

        public override void Update(StudentScore data, Action<SopRequest<AlunoPlacarJogo>> callback)
        {
            UpdateRequest(data.ConvertTo(), callback);
        }

        protected override void GetCallback(AlunoPlacarJogo request, Action<StudentScore> callback)
        {
            callback(new StudentScore(request));
        }

        protected override void GetAllCallback(List<AlunoPlacarJogo> requests, Action<List<StudentScore>> listCallback)
        {
            List<StudentScore> scores = new List<StudentScore>();
            foreach (SopService.AlunoPlacarJogo score in requests)
                scores.Add(new StudentScore(score));
            listCallback(scores);
        }
    }
}