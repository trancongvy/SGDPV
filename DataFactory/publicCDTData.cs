using System;
using System.Collections.Generic;
using System.Text;
using CDTControl;
namespace DataFactory
{
    public class publicCDTData
    {
        static List<DataSingle> lCDTData=new List<DataSingle>();
        public static DataSingle findCDTData(string tableName, string condition, string DynCondition)
        {
            foreach (DataSingle c in lCDTData)
            {
                if (c._tableName == tableName)
                {

                }
                if (c._tableName == tableName && condition == c.Condition && c.DynCondition == DynCondition)
                    return c;
            }
                return null;
            
        }
        public static void AddCDTData(DataSingle c)
        {
            if (!lCDTData.Exists(data => data._tableName == c._tableName && data.Condition == c.Condition && c.DynCondition == data.DynCondition))
            {
                lCDTData.Add(c);
            }
        }
        
    }
    
}
