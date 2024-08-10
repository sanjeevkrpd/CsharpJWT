using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Employee.DAL
{
    public class ErrorCode
    {
        public enum ErrorType
        {
            NOTEXISTS = 101,
            SUCCESS = 201,
            ERROR = 301,
            LOGINSUCCESS = 401,
            EXISTS = 501,
            DATANOTFOUND = 601,
        }
    }
}
