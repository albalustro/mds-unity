using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

/// <summary>
/// Script criado para controlar todo gerenciamento de jogadores, possui todos os métodos necessários para manipulação por todo o jogo
/// Com os métodos estáticos essa classe possui toda interação necessária para manipular e responder os dados referentes ao jogadores
/// </summary>
namespace Playmove
{
    /// <summary>
    /// Responsible to handle the game score as save the students names
    /// </summary>
    //[Obsolete("Utilize agora PYNamesScoreManager")]
    public class PYScoreData
    {
        /// <summary>
        /// Lista de alunos com seus placares carregada assim que o jogo inicia
        /// </summary>
        public List<Student> Students { get; private set; }

        public static PYScoreData Instance { get { return PlaytableWin32.Instance.Data.ScoreManager; } }

        public void Load()
        {
            if (File.Exists(string.Format("{0}/data.bin", Application.persistentDataPath)))
            {
                try
                {
                    using (Stream stream = File.Open(string.Format("{0}/data.bin", Application.persistentDataPath), FileMode.Open))
                    {
                        BinaryFormatter bin = new BinaryFormatter();
                        Students = (List<Student>)bin.Deserialize(stream);
                    }
                }
                catch (Exception)
                {
                    Students = new List<Student>();
                    Save();
                    Debug.LogWarning("Students rebuild!");
                }
            }
            else
            {
                Students = new List<Student>();
            }
        }

        /// <summary>
        /// Responsavel em gravar o placar e salvar o nome
        /// Esse método pode/deve ser alterado conforme a necessidade do jogo
        /// Porém alguns passos nele são obrigatórios, tal como a chamada para gravar o nome e a turma em banco como o Save(), que irá persistir os dados
        /// </summary>
        /// <param name="name"></param>
        /// <param name="classId"></param>
        /// <param name="score"></param>
        /// <param name="difficulty"></param>
        public void RegisterStudent(string name, long classId = 0, int score = 0, TagManager.GameDifficulty difficulty = TagManager.GameDifficulty.Easy)
        {
            Student student = Students.FindLast(x => x.Name == name);
            PlaytableWin32.Instance.Data.StudentsManager.SaveName(name, classId); // OBRIGATORIO Chamada para gravar em banco o nome e a turma do aluno
            if (student == null)
            {
                student = new Student(name, classId, score, difficulty); // Criar um novo aluno caso não existe
                Students.Add(student);
            }
            else
            {
                student.Update(score, difficulty); // Atualiza o placar do aluno
            }
            Save(); // OBRIGATORIO Persiste as alterações
        }

        public void RegisterStudent(PYStudentManager.Student name, int score, TagManager.GameDifficulty difficulty)
        {
            RegisterStudent(name.Name, name.ClassId ?? 0, score, difficulty);
        }

        /// <summary>
        /// Deleta todos os registros salvos na mesa e na memória
        /// </summary>
        public void DeleteAll()
        {
            Students = new List<Student>();
            Save();
        }

        /// <summary>
        /// Busca um placar
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Student GetStudentByName(string name)
        {
            foreach (Student student in Students)
            {
                if (student.Name == name)
                    return student;
            }
            return null;
        }

        /// <summary>
        /// Persiste os dados na pplaytable
        /// </summary>
        private void Save()
        {
            BinaryFormatter b = new BinaryFormatter();
            MemoryStream m = new MemoryStream();

            b.Serialize(m, Students);
            using (Stream stream = File.Open(string.Format("{0}/data.bin", Application.persistentDataPath), FileMode.Create))
            {
                BinaryFormatter bin = new BinaryFormatter();
                bin.Serialize(stream, Students);
            }
        }
    }
}