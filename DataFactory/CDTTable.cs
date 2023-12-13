using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
namespace DataFactory
{
    public class CDTTable
    {
        public struct Table4Lookup
        {
          public DataRow drTable;
           public DataTable Table;
            public bool Full;
        }
        static List<Table4Lookup> lstTable =new List<Table4Lookup>();
        public static Table4Lookup FindTable(string TableName)
        {
           Table4Lookup y = lstTable.Find(x => x.Table.TableName == TableName);
            return y;
        }
        public static void AddTable(DataTable Table, DataRow dr, bool isFull)
        {
            if(lstTable.Exists(x=>x.Table.TableName==Table.TableName && !x.Full)){
                Table4Lookup y = lstTable.Find(x => x.Table.TableName == Table.TableName);
                lstTable.Remove(y);
            }
            if (!lstTable.Exists(x => x.Table.TableName == Table.TableName))
            {
                Table4Lookup table4look = new Table4Lookup();
                table4look.drTable = dr;
                table4look.Table = Table;
                table4look.Full = isFull;
                lstTable.Add(table4look);
            }
        }
    }
}
