using Employee.DAL;
using Employee.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Employee.BAL
{
    public class EmployeeBAL
    {

        EmployeeDAL EmployeeDAL  = new EmployeeDAL();
        EmployeeDTO EmployeeDTO = new EmployeeDTO();


        public EmployeeDTO SelectEmployee()
        {
            EmployeeDTO  = EmployeeDAL.SelectEmployee();
            return EmployeeDTO;
        }

        public bool AddEmployee(string name , string email , string gender , int age)
        {
            return EmployeeDAL.AddEmployee(name , email , gender , age);

        }

        public bool DeleteEmployee(int id)
        {
            return EmployeeDAL.DeleteEmployee(id);
        }

        public bool UpdateEmployee(int id , string name , string email , string gender , int age)
        {
            return EmployeeDAL.UpdateEmployee(id , name , email , gender , age);
        }
    }
}
