using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data;
using System.Text.RegularExpressions;

namespace CDTLib
{
    public class Config
    {
        static Hashtable _variables = new Hashtable();

        public static Hashtable Variables
        {
            get { return Config._variables; }
            set { Config._variables = value; }
        }

        public static void InitData(DataTable dtData)
        {
            for (int i = 0; i < dtData.Rows.Count; i++)
            {
                if(dtData.Rows[i] != null)
                    try
                    {
                        _variables.Add(dtData.Rows[i]["_Key"], dtData.Rows[i]["_Value"]);
                    }
                    catch (Exception ex)
                    {
                    }

                   
            }
        }

        public static void NewKeyValue(object key, object value)
        {
            if (_variables.Contains(key))
                _variables.Remove(key);
            _variables.Add(key, value);
        }

        public static object GetValue(string key)
        {
            return (_variables[key]);
        }
        public static List<string> GetFieldList(string Formula)
        {
            string sqlCondition = Formula;

            List<string> fields = new List<string>();
            List<string> variables = new List<string>();
            List<string> constants = new List<string>();

            string pattern = @"(@\w+)|('[^']*')|(\w+)"; // Pattern to match variables, constants, and fields
            Regex regex = new Regex(pattern);
            MatchCollection matches = regex.Matches(sqlCondition);

            foreach (Match match in matches)
            {
                if (match.Groups[1].Success) // Variable
                {
                    variables.Add(match.Value);
                }
                else if (match.Groups[2].Success) // Constant
                {
                    constants.Add(match.Value);
                }
                else if (match.Groups[3].Success) // Field
                {
                    fields.Add(match.Value);
                }
            }
            return fields;
        }
        public static List<string> GetVariableList(string Formula)
        {
            string sqlCondition = Formula;

            List<string> fields = new List<string>();
            List<string> variables = new List<string>();
            List<string> constants = new List<string>();

            string pattern = @"(@\w+)|('[^']*')|(\w+)"; // Pattern to match variables, constants, and fields
            Regex regex = new Regex(pattern);
            MatchCollection matches = regex.Matches(sqlCondition);

            foreach (Match match in matches)
            {
                if (match.Groups[1].Success) // Variable
                {
                    if (!variables.Contains(match.Value))
                        variables.Add(match.Value);
                }
                else if (match.Groups[2].Success) // Constant
                {
                    constants.Add(match.Value);
                }
                else if (match.Groups[3].Success) // Field
                {
                    fields.Add(match.Value);
                }
            }
            return variables;
        }
    }
    public partial class UserConnection
    {
        public string DatabaseName { get; set; }
        public string ComputerName { get; set; }
        public string LicenceKey { get; set; }
        public string StructDb { get; set; }
        public Nullable<System.DateTime> TimeEx { get; set; }
        public int stt { get; set; }
    }
    public partial class ComputerConnection
    {
        public string DatabaseName { get; set; }
        public string ComputerName { get; set; }
        public string CPUID { get; set; }
        public string StructDB { get; set; }
        public string LicenceKey { get; set; }
        public int stt { get; set; }
    }

   
}