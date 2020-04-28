using MantisDBCleaner.Connection;
using MantisDBCleaner.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Text;

namespace MantisDBCleaner.Types
{
    static class DBExtensions
    {
        public static int IndexOfField(this DbDataReader dr, String fieldName)
        {
            fieldName = fieldName.ToLower();

            for (int i = 0; i < dr.FieldCount; i++)
            {
                string s = dr.GetName(i);
                if (s.ToLower().CompareTo(fieldName) == 0)
                    return i;
            }

            return -1;
        }

        public static T Get<T>(this DbDataReader dr, int columnIndex)
        {
            return Get<T>(dr, columnIndex, GetDefaultValue<T>());
        }

        public static T Get<T>(this DbDataReader dr, int columnIndex, T returnOnNull)
        {
            if (dr.IsDBNull(columnIndex))
                return returnOnNull;

            T ret = default(T);
            Object objVal = null;

            // Wir gehen davon aus, dass der Spaltentyp korrekt war
            // Wir konvertieren den Wert der Spalte einfach in den Zieltyp
            try
            {
                object dbVal = dr.GetValue(columnIndex);
                objVal = Convert.ChangeType(dbVal, typeof(T));
            }
            catch (Exception e)
            {
                return returnOnNull;
            }

            try
            {
                ret = (T)objVal;
                return ret;
            }
            catch (Exception e)
            {
                return returnOnNull;
            }
        }

        private static T GetDefaultValue<T>()
        {
            Object def = default(T);

            if (typeof(T) == typeof(String))
                def = String.Empty;
            else if (typeof(T) == typeof(DateTime))
                def = DateTime.MinValue;

            return (T)def;
        }

        public static T Get<T>(this DbDataReader dr, string columnName)
        {
            return Get<T>(dr, columnName, GetDefaultValue<T>());
        }

        public static T Get<T>(this DbDataReader dr, string columnName, T returnOnNull)
        {
            int colIdx = IndexOfField(dr, columnName);

            if (colIdx == -1)
                return returnOnNull;
            else
                return Get<T>(dr, colIdx, returnOnNull);
        }

        public static T ScalarTo<T>(this DbCommand cmd, T returnOnNull)
        {
            object dbResult = cmd.ExecuteScalar();

            if (dbResult == null || dbResult.GetType() == typeof(System.DBNull))
                return returnOnNull;
            else
                return (T)Convert.ChangeType(dbResult, typeof(T));
        }

        public static T ScalarTo<T>(this DbCommand cmd)
        {
            return ScalarTo<T>(cmd, GetDefaultValue<T>());
        }

        /// <summary>
        /// This will add an array of parameters to a SqlCommand. This is used for an IN statement.
        /// Use the returned value for the IN part of your SQL call. (i.e. SELECT * FROM table WHERE field IN @@paramNameRoot)
        /// </summary>
        /// <param name="cmd">The SqlCommand object to add parameters to.</param>
        /// <param name="paramNameRoot">What the parameter should be named followed by a unique value for each value. This parameter name in the CommandText will be replaced.</param>
        /// <param name="values">The array of objects that need to be added as parameters.</param>
        /// <param name="dbType">One of the System.Data.SqlDbType values. If null, determines type based on T.</param>
        /// <param name="size">The maximum size, in bytes, of the data within the column. The default value is inferred from the parameter value.</param>
        public static DbParameter[] AddArrayParameters<T>(this DbCommand cmd, string paramQueryName, IEnumerable<T> values, DbType? dbType = null, int? size = null)
        {
            /* An array cannot be simply added as a parameter to a SqlCommand so we need to loop through things and add it manually. 
             * Each item in the array will end up being it's own SqlParameter so the return value for this must be used as part of the
             * IN statement in the CommandText.
             */
            if (!paramQueryName.StartsWith("@@"))
                throw new ArgumentException("paramQuer yname must start with '@@'");

            string paramNameRoot = paramQueryName.Substring(2);

            var parameters = new List<DbParameter>();
            var parameterNames = new List<string>();
            var paramNbr = 1;
            foreach (var value in values)
            {
                var paramName = string.Format("@{0}{1}", paramNameRoot, paramNbr++);
                parameterNames.Add(paramName);
                DbParameter p = cmd.CreateParameter();
                p.ParameterName = paramName;
                p.Value = value;
                if (dbType.HasValue)
                    p.DbType = dbType.Value;
                if (size.HasValue)
                    p.Size = size.Value;
                cmd.Parameters.Add(p);
                parameters.Add(p);
            }

            cmd.CommandText = cmd.CommandText.Replace(paramQueryName, "(" + string.Join(",", parameterNames) + ")");

            return parameters.ToArray();
        }

        public static List<T> ReadList<T>(this MantisDatabase db, string commandText)
        {
            List<T> list = new List<T>();
            using (var cmd = db.CreateCommand())
            {
                cmd.CommandText = commandText;
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read()) list.Add(dr.Get<T>(0));
                }
            }

            return list;
        }

        public static void DeleteMatching<T>(this MantisDatabase db, string what, string commandText, IEnumerable<T> ids)
        {
            using (var cmd = db.CreateCommand())
            {
                cmd.CommandText = commandText;
                cmd.AddArrayParameters("@@ids", ids);
                int count = cmd.ExecuteNonQuery();
                LogService.Info($"Deleted {count} {what}");
            }
        }

        /// <summary>
        /// Removes all rows form the given table
        /// </summary>        
        public static void DeleteAll(this MantisDatabase db, string what, MantisTable table)
        {
            using(var cmd = db.CreateCommand())
            {
                cmd.CommandText = $"DELETE FROM {db.Table(table)}";
                int count = cmd.ExecuteNonQuery();
                LogService.Info($"Deleted {count} {what}");
            }
        }
    }
}
