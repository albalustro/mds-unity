using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MDS.Utilities
{
    public static class Extensions
    {


        public static T GetRandom<T> (this IEnumerable<T> sequence)
        {
            int index = Random.Range(0, sequence.Count());
            return sequence.ElementAt(index);
        }

       
    }
}
