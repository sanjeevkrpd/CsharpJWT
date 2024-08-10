using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace Employee.DAL
{
    public class DALBASE
    {
        public Database db = null;
        public DbCommand command = null;


        public DALBASE()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            db = factory.Create("ConStr");
        }
        public void ErrorLog(string MethodName, string ClassName, string Message)
        {
            using (command = db.GetStoredProcCommand("SP_ErrorLog"))
            {
                db.AddInParameter(command, "@MethodName", DbType.String, MethodName);
                db.AddInParameter(command, "@ClassName", DbType.String, ClassName);
                db.AddInParameter(command, "@Exception", DbType.String, Message);
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
        private bool disposed = false;

        public void Disposed()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                }
                if (db != null)
                    db = null;
                disposed = true;
            }
        }

        ~DALBASE()
        {
            Dispose(false);
        }
    }
}
