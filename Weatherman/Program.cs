using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Weatherman
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("What is your name?");
            var user = Console.ReadLine();
            Console.WriteLine("What is your zip code?");
            var zip = Console.ReadLine();

            var url = $"http://api.openweathermap.org/data/2.5/weather?zip={zip},us&appid=5b6bda62c86b15026c21f468a4192fce";

            var request = WebRequest.Create(url);

            var response = request.GetResponse();

            var rawResponse = String.Empty;
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                rawResponse = reader.ReadToEnd();
                
            }

            var theweather = JsonConvert.DeserializeObject<RootObject>(rawResponse);
            Console.WriteLine(theweather.main.temp);
            Console.ReadLine();
            Console.WriteLine(theweather.weather.First().description);
            Console.ReadLine();

            Program.addtodatabase(user, theweather);

           
        }
        
        public static void addtodatabase(string user, RootObject theweather)
        {
            const string connectionString =
           @"Server=localhost\SQLEXPRESS;Database=Weatherman;Trusted_Connection=True;";

            using (var connection = new SqlConnection(connectionString))
            { 
                
                var text = (@"INSERT INTO dbo.weatherdata (temperature, [current conditions], [user name])" +
                            "Values (@temp, @cc, @u)");

                var cmd = new SqlCommand(text, connection);

                cmd.Parameters.AddWithValue("@temp", theweather.main.temp);
                cmd.Parameters.AddWithValue("@cc", theweather.weather[0].description);
                cmd.Parameters.AddWithValue("@u", user);
                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
                
            }
        }
    }
}
