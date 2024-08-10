using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Employee.DAL
{
    public class RequestResponseLog
    {
        public Database db = null;
        public DbCommand command = null;
        public RequestResponseLog()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            db = factory.Create("Constr");
        }

        public void RequestResponseLogs(string MethodName, string ClassName, string Request, string Response, string Exception)
        {
            using (command = db.GetStoredProcCommand("sp_tblRequestRespone"))
            {
                db.AddInParameter(command, "@Action", DbType.String, "insert");
                db.AddInParameter(command, "@MethodName", DbType.String, MethodName);
                db.AddInParameter(command, "@ClassName", DbType.String, ClassName);
                db.AddInParameter(command, "@Request", DbType.String, Request);
                db.AddInParameter(command, "@Response", DbType.String, Response);
                db.AddInParameter(command, "@Exception", DbType.String, Exception);
                try
                {
                    IDataReader reader = db.ExecuteReader(command);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}
