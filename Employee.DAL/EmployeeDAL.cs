using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Employee.DTO;

namespace Employee.DAL
{
    public class EmployeeDAL : DALBASE
    {

        readonly EmployeeDTO empDTO = new EmployeeDTO();

        public EmployeeDTO SelectEmployee()
        {
            empDTO.empList = new List<EmployeeDTO.EmployeeDetailEntity>();

            using (command = db.GetStoredProcCommand("sp_GetAllEmployee"))
            {
                db.AddInParameter(command, "@Action", DbType.String, "select");
            }

            string Retval = "";
            IDataReader reader = db.ExecuteReader(command);
            try
            {
                if (reader == null)
                {
                    empDTO.Message = "Unsuccessfull";
                    empDTO.Code = (int)ErrorCode.ErrorType.DATANOTFOUND;
                }
                else
                {
                    while (reader.Read())
                    {
                        Retval = reader["Retval"].ToString();

                        if (Retval == "SUCCESS")
                        {
                           empDTO.empList.Add(new EmployeeDTO.EmployeeDetailEntity
                            {
                                id = Convert.ToInt32(reader["id"]),
                                name = reader["name"].ToString(),
                                email = reader["email"].ToString(),
                                gender = reader["gender"].ToString(),
                                age = Convert.ToInt32(reader["age"])
                            });
                            empDTO.Message = "Successfull";
                            empDTO.Code =(int )ErrorCode.ErrorType.SUCCESS;
                        }
                        else
                        {
                            empDTO.Message = "Record Not Found";
                            empDTO.Code =(int )ErrorCode.ErrorType.DATANOTFOUND;
                        }
                    }
                }

            }
            catch (Exception ex) {

                empDTO.Message = "Unsuccessfull";
                empDTO.Code =(int)ErrorCode.ErrorType.ERROR;
                ErrorLog("GetList" , "EmployeeDAL",ex.ToString());

            }

            return empDTO;


        }

        public bool AddEmployee(string name, string email, string gender, int age)
        {

            bool isSuccess = false;

            using (DbCommand command = db.GetStoredProcCommand("sp_GetAllEmployee"))
            {
                db.AddInParameter(command, "@Action", DbType.String, "Insert");
                db.AddInParameter(command, "@name", DbType.String, name);
                db.AddInParameter(command, "@email", DbType.String, email);
                db.AddInParameter(command, "@gender", DbType.String, gender);
                db.AddInParameter(command, "@age", DbType.Int32, age);
                try
                {
                    
                    int result = db.ExecuteNonQuery(command);
                    isSuccess = result > 0; // Assuming a successful insert affects one or more rows

                   
                }
                catch
                {
                    isSuccess = false;
                }
            }

            return isSuccess;
        }
        public bool DeleteEmployee(int id)
        {
            bool isSuccess = false;
            using (DbCommand command = db.GetStoredProcCommand("sp_GetAllEmployee"))
            {
                db.AddInParameter(command, "@Action", DbType.String, "Delete");
                db.AddInParameter(command, "@id", DbType.Int32, id);

                try
                {
                    int result = db.ExecuteNonQuery(command);
                    isSuccess = result > 0;
                }
                catch (Exception ex)
                {
                    isSuccess = false;
                    ErrorLog("DeleteEmployee", "EmployeeDAL", ex.ToString());
                }
            }

            return isSuccess;
        }

        public bool UpdateEmployee(int id, string name, string email, string gender , int age)
        {
            bool isSuccess = false;
            using (DbCommand command = db.GetStoredProcCommand("sp_GetAllEmployee"))
            {
                db.AddInParameter(command, "@Action", DbType.String, "Update");
                db.AddInParameter(command, "@id", DbType.Int32, id);
                db.AddInParameter(command, "@name", DbType.String, name);
                db.AddInParameter(command, "@email", DbType.String, email);
                db.AddInParameter(command, "@gender", DbType.String, gender);
                db.AddInParameter(command, "@age", DbType.Int32, age);


                try
                {
                    int result = db.ExecuteNonQuery(command);
                    isSuccess = result > 0;
                }
                catch (Exception ex)
                {
                    {
                        isSuccess = false;
                        ErrorLog("UpdateEmployee", "LearningDAL", ex.ToString());

                    }
                }
                return isSuccess;
            }
        }

    }
}
