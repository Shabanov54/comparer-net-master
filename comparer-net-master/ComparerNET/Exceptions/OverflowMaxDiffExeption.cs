using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparerNET.Exceptions
{
    public class OverflowMaxDiffExeption:Exception
    {
        public OverflowMaxDiffExeption()
        {

        }

        public OverflowMaxDiffExeption(string message)
            :base(message)
        {

        }

        public OverflowMaxDiffExeption(string message, Exception inner)
            :base(message, inner)
        {

        }
    }
}
