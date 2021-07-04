using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparerNET.Exceptions
{
    public class BadArchiveException:Exception
    {
        public BadArchiveException()
        {

        }

        public BadArchiveException(string message)
            :base(message)
        {

        }

        public BadArchiveException(string message, Exception inner)
            :base(message,inner)
        {

        }
    }
}
