using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vitvor.HelthCare.Services
{
    static class LoadAndCloseControl
    {
        public async static  void Load(string type, int id, int idOfMI)
        {
            string insert = $"insert into USERSONLINE(id, userType, dateOfInfo, timeOfEntering, idOfMI) " +
                $"values({id},'{type}', '{DateTime.Now.Date}', '{DateTime.Now.TimeOfDay}', {idOfMI})";
            SqlCommand load = new SqlCommand(insert, SingletonForSqlConnection.SqlConnection);
            await load.ExecuteNonQueryAsync();
        }
        public async static void Load(string type, int id)
        {
            string insert = $"insert into USERSONLINE(id, userType, dateOfInfo, timeOfEntering) " +
                $"values({id},'{type}', '{DateTime.Now.Date}', '{DateTime.Now.TimeOfDay}')";
            SqlCommand load = new SqlCommand(insert, SingletonForSqlConnection.SqlConnection);
            await load.ExecuteNonQueryAsync();
        }
        public async static void Close(string type, int id)
        {
            string update = $"update USERSONLINE set timeOfExit='{DateTime.Now.TimeOfDay}' " +
                $"where id={id} and userType='{type}' and dateOfInfo='{DateTime.Now.Date}'";
            SqlCommand updation = new SqlCommand(update, SingletonForSqlConnection.SqlConnection);
            await updation.ExecuteNonQueryAsync();
        }
    }
}
