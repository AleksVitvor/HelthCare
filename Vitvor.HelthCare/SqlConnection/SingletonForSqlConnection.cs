using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vitvor.HelthCare
{
    class SingletonForSqlConnection
    {
        private static SingletonForSqlConnection instance;
        public static SqlConnection SqlConnection { get; private set; }
        private SingletonForSqlConnection()
        {
            SqlConnection = new SqlConnection("Data Source=ALEKS;Initial Catalog=CourseProject;Integrated Security=True");
            SqlConnection.Open();
        }
        public static SingletonForSqlConnection getInstance()
        {
            if (instance == null)
                instance = new SingletonForSqlConnection();
            return instance;
        }
        public static void Close()
        {
            if (SqlConnection.State == System.Data.ConnectionState.Open)
                SqlConnection.Close();
        }
    }
}
