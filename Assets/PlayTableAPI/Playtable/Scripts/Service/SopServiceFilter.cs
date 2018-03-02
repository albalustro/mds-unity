using System.Collections.Generic;

public class SopServiceFilter
{
    public enum EFilters
    {
        id,
        aplicativoId,
        alunoId,
        groupId,
        turmaId,
        semTurma,
        configuracaoFabrica,
        arquivosComAluno,
        configuracaoNome,
        nome,
        jogoId,
        guid,
        localizacao,
        lixeira,
        visibilidade
    }

    public Dictionary<string, string> Parameters { get; private set; }

    public SopServiceFilter()
    {
        Parameters = new Dictionary<string, string>();
    }

    public SopServiceFilter(EFilters filter, string value) : this()
    {
        Parameters.Add(filter.ToString(), value);
    }

    public SopServiceFilter AddFilter(EFilters filter, string value)
    {
        Parameters.Add(filter.ToString(), value);
        return this;
    }
}