using Playmove;
using System;

[Serializable]
public class Student : IStudent
{
    public string Name;
    public int[] Score { get; set; }

    /// <summary>
    /// Inicializar um aluno novo com os dados necessários para guardar o placar deste jogador
    /// </summary>
    /// <param name="name"></param>
    /// <param name="classId"></param>
    /// <param name="score"></param>
    /// <param name="difficulty"></param>
    public Student(string name, long classId, int score, TagManager.GameDifficulty difficulty)
    {
        Name = name;
        Score = new int[3];
        Score[(int)difficulty] = score;
    }

    /// <summary>
    /// Atualiza a instancia desse jogador
    /// </summary>
    /// <param name="score"></param>
    /// <param name="difficulty"></param>
    public void Update(int score, TagManager.GameDifficulty difficulty)
    {
        Score[(int)difficulty] = score;
    }
}