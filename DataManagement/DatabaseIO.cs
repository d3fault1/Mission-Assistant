using Dapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionAssistant
{
    static class DatabaseIO
    {
        //public static void Init()
        //{
        //    SqlMapper.SetTypeMap(typeof(PerformanceData), new CustomPropertyTypeMap(typeof(PerformanceData), (type, columnName) => type.GetProperties().FirstOrDefault(prop => prop.GetCustomAttributes(false).OfType<ColumnAttribute>().Any(attr => attr.Name == columnName))));
        //    SqlMapper.SetTypeMap(typeof(FuelStartData), new CustomPropertyTypeMap(typeof(FuelStartData), (type, columnName) => type.GetProperties().FirstOrDefault(prop => prop.GetCustomAttributes(false).OfType<ColumnAttribute>().Any(attr => attr.Name == columnName))));
        //    SqlMapper.SetTypeMap(typeof(FuelReduceData), new CustomPropertyTypeMap(typeof(FuelReduceData), (type, columnName) => type.GetProperties().FirstOrDefault(prop => prop.GetCustomAttributes(false).OfType<ColumnAttribute>().Any(attr => attr.Name == columnName))));
        //}

        public static void Fetch(string aircraft, out dynamic pdata, out dynamic fsdata, out dynamic frdata, out dynamic sudata)
        {
            using (IDbConnection cnn = new SQLiteConnection(@"Data Source=.\test.db;Version=3"))
            {
                pdata = cnn.Query($"SELECT * FROM 'Performance Data' WHERE Aircraft='{aircraft}' ORDER BY ALT", new DynamicParameters()).ToList();
                fsdata = cnn.Query($"SELECT Label, Value FROM 'Fuel Data' WHERE Aircraft='{aircraft}' AND Type='Starting' ORDER BY Label", new DynamicParameters()).ToList();
                frdata = cnn.Query($"SELECT Label, Value FROM 'Fuel Data' WHERE Aircraft='{aircraft}' AND Type='Reduction' ORDER BY Label", new DynamicParameters()).ToList();
                sudata = cnn.Query($"SELECT Value, Unit FROM 'Speed and Unit Data' WHERE Aircraft='{aircraft}' ORDER BY SpeedID", new DynamicParameters()).ToList();
            }
        }
    }
}
