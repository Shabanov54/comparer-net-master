using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparerNET.Exceptions
{
    public class OldStandartFileException:Exception
    {
        public OldStandartFileException()
        {

        }

        public OldStandartFileException(string message)
            :base(message)
        {

        }

        public OldStandartFileException(string message, Exception inner)
            :base(message,inner)
        {

        }
    }
}
