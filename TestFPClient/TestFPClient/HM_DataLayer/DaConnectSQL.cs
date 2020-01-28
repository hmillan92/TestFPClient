using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HM_DataLayer
{
    public class DaConnectSQL
    {
        public SqlConnection Con = new SqlConnection();
        public SqlTransaction Tran;
        public DaConnectSQL()
        {
            DASQLConnection();
        }


        public SqlConnection DASQLConnection()
        {
            try
            {
                Con = new SqlConnection(Properties.Settings.Default.ConnStringSQL);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return Con;
        }
        public void Open()
        {
            try
            {
                Con.Open();
            }
            catch (Exception ex)
            {
                string Mensaje;
                Mensaje = ex.Message;
            }
        }
        public void Close()
        {
            try
            {
                Con.Close();
            }
            catch (Exception ex)
            {
            }
        }
        public SqlTransaction BeginTransaction()
        {
            Tran = Con.BeginTransaction();
            return Tran;
        }
        public void CommitTransaction()
        {
            if (Tran != null)
                Tran.Commit();
        }
        public void RollBackTransaction()
        {
            if (Tran != null)
                Tran.Rollback();
        }

    }
}
