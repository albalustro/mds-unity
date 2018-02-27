using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MDS.Utilities
{
    public static class Extensions
    {

        public static T GetRandom<T> (this IEnumerable<T> sequence)
        {
            int index = Random.Range(0, sequence.Count());
            return sequence.ElementAt(index);
        }


        public static bool IsChallenge(this Scene scene)
        {
            return scene.name.Length == 8 && scene.name[6] == 'C';
        }

        public static bool IsEpisode(this Scene scene)
        {
            return scene.name.Length == 6 && scene.name[4] == 'E';
        }
       
        public static bool IsMap(this Scene scene)
        {
            return scene.name.ToUpper().Contains("MAP");
        }

		public static bool IsRoom(this Scene scene)
		{
			return scene.name.ToUpper().Contains("ROOM");
		}

        public static bool IsLogin(this Scene scene)
        {
            return scene.name.ToUpper().Contains("LOGIN");
        }

        public static int GetGameIndex(this Scene scene)
        {
            return int.Parse(scene.name.Substring(1, 1));
        }

        /// <summary>
        /// Retorna, convertido em inteiro, o valor que esta no nome 
        /// da cena, exatamente como estiver lá
        /// </summary>
        public static int GetWorldIndex(this Scene sceneName)
        {
            return int.Parse(sceneName.name.Substring(3, 1));
        }

        /// <summary>
        /// Retorna, convertido em inteiro, o valor que esta no nome 
        /// da cena, exatamente como estiver lá
        /// </summary>
        public static int GetEpisodeIndex(this Scene sceneName)
        {
            return int.Parse(sceneName.name.Substring(5, 1));
        }

        /// <summary>
        /// Retorna, convertido em inteiro, o valor que esta no nome 
        /// da cena, exatamente como estiver lá
        /// </summary>
        public static int GetChallengeIndex(this Scene sceneName)
        {
            return int.Parse(sceneName.name.Substring(7, 1));
        }

		public static string GetEpisodeTitle(this Scene scene, int episodeIndex = 0)
        {
			if(scene.IsEpisode() == false && scene.IsChallenge()==false && scene.IsMap() == false) return string.Empty;

            string episodeName = scene.name.Substring(0,6);

			if (scene.IsMap () && episodeIndex > 0)
			{
				episodeName = episodeName.Remove (5, 1);
				episodeName = episodeName + episodeIndex.ToString ();
			}

            string json = "{" +
            "\"G1W1E1\": \"POR UM FIO\"," +
            "\"G1W1E2\": \"QUE BAGUNÇA\"," +
            "\"G1W1E3\": \"O TEMPLO DO CONHECIMENTO\"," +
            "\"G1W1E4\": \"A CAVERNA DOS DESAFIOS\"," +
            "\"G1W1E5\": \"O VALE NO FIM DO MUNDO\"," +
            "\"G1W1E6\": \"O CASTELO VOADOR\"," +
            "\"G1W1E7\": \"CAPTURADOS\"," +
            "\"G1W1E8\": \"O RESGATE DA GRANDE BOLA DE LÃ\"," +
            "\"G1W2E1\": \"O RAPTO DO MENINO ASSOMBRADO\"," +
            "\"G1W2E2\": \"A FLORESTA ESCURA\"," +
            "\"G1W2E3\": \"ACAMPAMENTO ASSOMBRADO\"," +
            "\"G1W2E4\": \"O SEGREDO DA VILA FANTASMA\"," +
            "\"G1W2E5\": \"DEIXE-NOS ENTRAR!\"," +
            "\"G1W2E6\": \"O MONSTRO TERRÍVEL!\"," +
            "\"G1W2E7\": \"OS TRÊS REIS MÁGICOS\"," +
            "\"G1W2E8\": \"O MEDALHÃO DA OBEDIÊNCIA\"," +
            "\"G1W3E1\": \"TEMPESTADE DO SÉCULO\"," +
            "\"G1W3E2\": \"DEPOIS DA TEMPESTADE...\"," +
            "\"G1W3E3\": \"VIAGEM DO GAVIÃO SECULAR\"," +
            "\"G1W3E4\": \"NA CORTE DO PRÍNCIPE TORMENTA\"," +
            "\"G1W3E5\": \"O SOMBRIO CONTRA-ATACA!\"," +
            "\"G1W3E6\": \"O DESFILADEIRO DOS VENTOS UIVANTES\"," +
            "\"G1W3E7\": \"DIAS DE TROVÃO\"," +
            "\"G1W3E8\": \"OPERAÇÃO CASTELO VOADOR\"," +
            "\"G1W4E1\": \"A VILA SECRETA\"," +
            "\"G1W4E2\": \"REFLORESTANDO\"," +
            "\"G1W4E3\": \"GRONCO, O BRONCO\"," +
            "\"G1W4E4\": \"A ÁRVORE MAIS VELHA\"," +
            "\"G1W4E5\": \"UM CASTELO DO BARULHO\"," +
            "\"G1W4E6\": \"A HIDRA DE CINCO CABEÇAS\"," +
            "\"G1W4E7\": \"NO FUNDO DO POÇO\"," +
            "\"G1W4E8\": \"O RUBI DE FOGO\"," +
            "\"G2W1E1\": \"PELOS MEUS BOTÕES!\"," +
            "\"G2W1E2\": \"O BOM TRAPO A CASA TORNA\"," +
            "\"G2W1E3\": \"RESGATE NO FOGUETE ESPACIAL\"," +
            "\"G2W1E4\": \"GATO EM ARMADURA RELUZENTE\"," +
            "\"G2W1E5\": \"UMA ÓTIMA IMPRESSÃO\"," +
            "\"G2W1E6\": \"EM FARRAPOS!\"," +
            "\"G2W1E7\": \"VISITA AO PARQUE DO TERROR\"," +
            "\"G2W1E8\": \"FUGA DO CASTELO SOMBRIO\"," +
            "\"G2W2E1\": \"ESCONDE-ESCONDE\"," +
            "\"G2W2E2\": \"O TRATADO DOS SONHOS\"," +
            "\"G2W2E3\": \"O CORCUNDA DE NOSTRADAMUS\"," +
            "\"G2W2E4\": \"ABOBOMINÁVEL\"," +
            "\"G2W2E5\": \"UM MONTE DE OSSOS\"," +
            "\"G2W2E6\": \"QUE CHEIRO É ESSE?\"," +
            "\"G2W2E7\": \"TROLLANDO\"," +
            "\"G2W2E8\": \"BURACO DOS SONHOS\"," +
            "\"G2W3E1\": \"PIRATAS!\"," +
            "\"G2W3E2\": \"O SEGREDO DA ILHA DAS BERMUDAS\"," +
            "\"G2W3E3\": \"PIRATAS DO CABIDE\"," +
            "\"G2W3E4\": \"CAÇADA IMPLACÁVEL\"," +
            "\"G2W3E5\": \"ABORDAR NAVIO!\"," +
            "\"G2W3E6\": \"INCÓGNITOS\"," +
            "\"G2W3E7\": \"SERENA NAS NUVENS\"," +
            "\"G2W3E8\": \"OS PIRATAS CONTRA-ATACAM\"," +
            "\"G2W4E1\": \"O SUMIÇO DAS FADAS\"," +
            "\"G2W4E2\": \"VOO DAS LIBÉLULAS\"," +
            "\"G2W4E3\": \"A HARPA MÁGICA\"," +
            "\"G2W4E4\": \"DEITADO EM BERÇO ESPLÊNDIDO\"," +
            "\"G2W4E5\": \"NADA PODE DAR ERRADO!\"," +
            "\"G2W4E6\": \"PLANO PERFEITO\"," +
            "\"G2W4E7\": \"PEGUEM AQUELA HARPA!\"," +
            "\"G2W4E8\": \"FESTIVAL DA PRIMAVERA\"," +
            "\"G3W1E1\": \"UMA GRANDE SURPRESA\"," +
            "\"G3W1E2\": \"RESGATE ANIMAL\"," +
            "\"G3W1E3\": \"UM PARAFUSO A MENOS!\"," +
            "\"G3W1E4\": \"NUMA FRIA!\"," +
            "\"G3W1E5\": \"NÃO FALE COM ESTRANHOS\"," +
            "\"G3W1E6\": \"QUEM VIGIA OS VIGILANTES?\"," +
            "\"G3W1E7\": \"PRÓXIMA PARADA: FIM DO MUNDO!\"," +
            "\"G3W1E8\": \"A DERROTA DO SENHOR SOMBRIO\"," +
            "\"G3W2E1\": \"RUTO, O DESTEMIDO\"," +
            "\"G3W2E2\": \"PERDIDOS NA FLORESTA\"," +
            "\"G3W2E3\": \"ESCONDERIJO DO REI GOBLIN\"," +
            "\"G3W2E4\": \"QUEM VIGIA O VIGILANTE\"," +
            "\"G3W2E5\": \"A CAVERNA DOS ECOS SEM FIM\"," +
            "\"G3W2E6\": \"NA TEIA DA ARANHA\"," +
            "\"G3W2E7\": \"CUIDADO COM O BICHO-PAPÃO\"," +
            "\"G3W2E8\": \"A MÁQUINA AMPLIFICADORA DE SUSTOS\"," +
            "\"G3W3E1\": \"E O VENTO LEVOU...\"," +
            "\"G3W3E2\": \"TERRA FIRME... MAIS OU MENOS\"," +
            "\"G3W3E3\": \"BERMUDAS AO VENTO\"," +
            "\"G3W3E4\": \"DE VENTO EM POPA\"," +
            "\"G3W3E5\": \"O LABIRINTO DAS NUVENS\"," +
            "\"G3W3E6\": \"RESGATE EM ALTO AR\"," +
            "\"G3W3E7\": \"RUMO À TEMPESTADE\"," +
            "\"G3W3E8\": \"TEMPO LIMPO\"," +
            "\"G3W4E1\": \"FEDIDO PRA CACHORRO!\"," +
            "\"G3W4E2\": \"LIXÃO\"," +
            "\"G3W4E3\": \"A BRUXA ZABUMBA\"," +
            "\"G3W4E4\": \"NADA COMO A ORIGINAL\"," +
            "\"G3W4E5\": \"PERDIDOS NA SELVA\"," +
            "\"G3W4E6\": \"COMO IMPRESSIONAR UMA DAMA\"," +
            "\"G3W4E7\": \"BRUXA DO BEM\"," +
            "\"G3W4E8\": \"A CASA SUMIU\" }";

            Dictionary<string, string> titles = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

            return titles[episodeName];
        }

        //Playmove
        public static IEnumerable<TSource> Page<TSource>(this IEnumerable<TSource> source, int page, int pageSize)
        {
            return source.Skip((page - 1) * pageSize).Take(pageSize);
        }
    }
}


