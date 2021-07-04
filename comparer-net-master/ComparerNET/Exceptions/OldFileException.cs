using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparerNET.Exceptions
{
    public class OldFileException: Exception
    {
        public OldFileException()
        {

        }

        public OldFileException(string message)
            :base(message)
        {

        }

        public OldFileException(string message, Exception inner)
            :base(message,inner)
        {
            
        }
    }
}
