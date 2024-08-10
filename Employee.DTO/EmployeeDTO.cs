using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Employee.DTO
{
    public class EmployeeDTO
    {

        public string Message { get; set; }
        public int Code { get; set; }


        public class EmployeeDetailEntity
        {
            public int id { get; set; }
            public string name { get; set; }
            public string email { get; set; }
            public string gender { get; set; }
            public int age { get; set; }
        }

       public List<EmployeeDTO.EmployeeDetailEntity> empList { get; set; }
    }
}