using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Net;
using System.IO;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace Beauty
{
    class ClassMySQL
    {
        public string ConnString = "host=localhost;user=root;database=beauty;sslmode=none";

        public async Task AsyncRequest(DataTable Table)
        {
            // получение json
            String answer;
            WebRequest connect = WebRequest.Create("http://localhost/beauty/getproducts.php");
            WebResponse res = await connect.GetResponseAsync();
            using (Stream stream = res.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                    answer = await reader.ReadToEndAsync();
            }
            //  десериализация
            Table = JsonConvert.DeserializeObject<DataTable>(answer);
        }
        public DataTable QueryToDB(string Query)
        {
            MySqlConnection Connect = new MySqlConnection(ConnString);
            Connect.Open();
            MySqlDataAdapter Adapter = new MySqlDataAdapter(Query, Connect);
            DataTable DataTab = new DataTable();
            Adapter.Fill(DataTab);
            Connect.Close();
            return DataTab;
        }

        public void EditDB(DataTable prod)
        {
            MySqlConnection con = new MySqlConnection(ConnString);
            con.Open();
            MySqlDataAdapter Adapter = new MySqlDataAdapter();
            Adapter.SelectCommand = new MySqlCommand("SELECT * from product", con);
            MySqlCommandBuilder Build = new MySqlCommandBuilder(Adapter);
            Adapter.UpdateCommand = Build.GetUpdateCommand();
            Adapter.Update(prod);
            con.Close();
        }
        
        public void AddToDB(DataTable prod)
        {
            MySqlConnection con = new MySqlConnection(ConnString);
            con.Open();
            MySqlDataAdapter Adapter = new MySqlDataAdapter();
            Adapter.SelectCommand = new MySqlCommand("SELECT * from product", con);
            MySqlCommandBuilder Build = new MySqlCommandBuilder(Adapter);
            Adapter.InsertCommand = Build.GetInsertCommand();
            Adapter.Update(prod);
            con.Close();
        }

        public void DeleteFromDB(int id)
        {
            string Query = "DELETE FROM product WHERE id = @id";
            MySqlConnection Connect = new MySqlConnection(ConnString);
            Connect.Open();
            MySqlCommand cmdProduct = new MySqlCommand(Query, Connect);
            cmdProduct.Parameters.AddWithValue("@id", id);
            cmdProduct.ExecuteNonQuery();
            Connect.Close();
        }
    }   
}
