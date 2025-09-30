
global using DB = TestVisualCode.SqliteDbEf;
global using YC = TestVisualCode.YC;
global using OC = TestVisualCode.OtherContextDb;
global using T = TestVisualCode.ToolsEf;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using TagLib;
using TestVisualCode;
using Microsoft.VisualBasic;



internal class Program
{


  internal static async Task Main(string[] args)
  {
    // await Manager.PrintMessage();
 

    ManagerEf.GetDifferentTracksId();
  }

}






