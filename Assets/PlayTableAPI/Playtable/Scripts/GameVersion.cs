namespace Playmove
{
    public class GameVersion
    {
        /// Alteração do jogo por completa, inicia-se em 0 e passa para 1 após primeiro lançamento, porém só se altera quando realizar alteração de larga escala
        public int Major;
        /// Alteração básica no jogo, só é alterada quando realizado alterações simples na estrutura, como atualização de software, adição de temas e modos de jogo
        public int Minor;
        /// Número que representa o lançamento para atualização comercial, sempre que feito uma versão de release nova esse número se incrementa sozinho
        public int Release;
        /// Número de teste, sempre que for realizado uma build de teste esse número cresce automaticamente.
        public int Test;

        public GameVersion() { }
        public GameVersion(int major, int minor, int release, int test)
        {
            Major = major;
            Minor = minor;
            Release = release;
            Test = test;
        }
        public GameVersion(string number)
        {
            Set(number);
        }

        /// <summary>
        /// Os dois primeiros numeros representam alterações estruturais, \nportanto sempre que alterado scripts de dependencia do livro o primeiro ou segundo número deve mudar.
        /// No caso de livro conter o primeiro numero menor que do contador deve FALHAR
        /// No caso de livro conter o segundo numero menor que do contador deve FALHAR
        /// </summary>
        /// <param name="gameBookVersion"></param>
        /// <param name="bookVersion"></param>
        /// <returns></returns>
        public static bool operator >=(GameVersion bookVersion, GameVersion gameBookVersion)
        {
            return bookVersion.Major > gameBookVersion.Major || (bookVersion.Major >= gameBookVersion.Major && bookVersion.Minor >= gameBookVersion.Minor);
        }

        /// <summary>
        /// Os dois primeiros numeros representam alterações estruturais, \nportanto sempre que alterado scripts de dependencia do livro o primeiro ou segundo número deve mudar.
        /// No caso de livro conter o primeiro numero menor que do contador deve FALHAR
        /// No caso de livro conter o segundo numero menor que do contador deve FALHAR
        /// </summary>
        /// <param name="gameBookVersion"></param>
        /// <param name="bookVersion"></param>
        /// <returns></returns>
        public static bool operator <=(GameVersion bookVersion, GameVersion gameBookVersion)
        {
            return bookVersion.Major < gameBookVersion.Major && (bookVersion.Major <= gameBookVersion.Major || bookVersion.Minor <= gameBookVersion.Minor);
        }

        public override string ToString()
        {
            return string.Format("{0}.{1}.{2}.{3}", Major, Minor, Release, Test);
        }

        public void Set(int major, int minor, int release, int test)
        {
            Major = major;
            Minor = minor;
            Release = release;
            Test = test;
        }
        public void Set(int[] numbers)
        {
            if (numbers != null)
            {
                Major = numbers[0];
                if (numbers.Length > 1)
                {
                    Minor = numbers[1];
                    if (numbers.Length > 2)
                    {
                        Release = numbers[2];
                        if (numbers.Length > 3)
                        {
                            Test = numbers[3];
                        }
                    }
                }
            }
        }
        public void Set(string[] numbers)
        {
            int[] num = new int[numbers.Length];
            for (int i = 0; i < numbers.Length; i++)
            {
                int parsed;
                int.TryParse(numbers[i], out parsed);
                num[i] = parsed;
            }
            Set(num);
        }
        public void Set(string number)
        {
            if (string.IsNullOrEmpty(number)) return;
            Set(number.Split('.'));
        }
    }
}