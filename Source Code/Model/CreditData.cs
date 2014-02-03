using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnigmaBreaker.Model
{
    [Serializable]
    public struct CreditData
    {
        public string[] Contributer;
        public string[] Role;

        public int Count;

        public CreditData(int count)
        {
            Contributer = new string[count];
            Role = new string[count];

            Count = count;
        }
    }
}
