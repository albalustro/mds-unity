using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Playmove
{
    public class SopRequest<T>
    {
        public T Model;
        public bool Success;

        /// <summary>
        /// Error Message
        /// </summary>
        public string Message;
        /// <summary>
        /// ModelState with erros
        /// </summary>
        public Dictionary<string, string[]> ModelState = new Dictionary<string, string[]>();

        public SopRequest() { }
        public SopRequest(bool success, string text)
        {
            Success = success;
            Message = text;
        }


        /// <summary>
        /// Get all sentences of erros to show
        /// </summary>
        /// <returns></returns>
        public string[] GetErros()
        {
            string[] erros = null;
            if (ModelState != null)
            {
                var modelErros = ModelState.Select(s => string.Join(". ", s.Value));
                erros = new string[modelErros.Count()];
                for (int i = 0; i < modelErros.Count(); i++)
                    erros[i] = modelErros.ElementAt(i);
            }
            return erros;
        }
    }
}