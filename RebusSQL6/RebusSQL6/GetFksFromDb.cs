using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace RebusData6
{
    public partial class DatabaseForeignKeys
    {
        public string Message { get; internal set; }
        public List<ForeignKey> ForeignKeys { get; set; }
        public List<string> DatabaseTables { get; set; }

        struct TableColumnDataType
        {
            public string Column;
            public string DataType;
        }


        public DatabaseForeignKeys()
        {
            Message = "";
            ForeignKeys = new List<ForeignKey>(0);
            DatabaseTables = new List<string>(0);
        }

        public bool GetForeignKeysFromRubyStyleSqlServerDatabase(string psSrvr, string psDB, string psUser = "", string psPswd = "", string psConnStr = "")
        {
            Message = "";

            string xsDatabase = psDB.Trim();

            string xsConnStr = psConnStr.Trim();
            if (psSrvr.Trim().Length > 0 && xsDatabase.Length > 0)
            {
                //
                // build the connection string
                //
                if (xsConnStr.Length > 0 && xsConnStr.Substring(xsConnStr.Length - 1, 1) != ";") xsConnStr += ";";
                xsConnStr = "Server=" + psSrvr.Trim() + ";Database=" + xsDatabase;
                if (xsConnStr.Substring(xsConnStr.Length - 1, 1) != ";") xsConnStr += ";";
                if (psUser.Trim().Length > 0)
                {
                    xsConnStr += "User ID=" + psUser.Trim() + ";Password=" + psPswd.Trim();
                }
                else
                {
                    string xsTrusted = "Trusted_Connection=";
                    if (xsConnStr.ToLower().IndexOf(xsTrusted.ToLower()) < 0) xsConnStr += xsTrusted + "True";
                }
            }
            if (xsDatabase.Length == 0)
            {
                int xi = xsConnStr.ToLower().IndexOf("database=");
                if (xi >= 0)
                {
                    xsDatabase = xsConnStr.Substring(xi + 9).Trim();
                    xi = xsDatabase.IndexOf(";");
                    if (xi >= 0)
                    {
                        if (xi == 0)
                        {
                            xsDatabase = "";
                        }
                        else
                        {
                            xsDatabase = xsDatabase.Substring(0, xi).Trim();
                        }
                    }
                }
            }

            //
            // get the table names in the database
            //
            DatabaseTables = new List<string>(0);
            SqlConnection xoConn = null;
            DataTable xoTable = null;

            try
            {
                if (xsDatabase.Length == 0) throw new Exception("No database specified.");
                xoConn = new SqlConnection(xsConnStr);
                xoConn.Open();
                xoTable = xoConn.GetSchema("TABLES");
                string xsLine = "";
                int xiTblTypeColm = -1, xiTblNameColm = -1;

                for (int xiCol = 0; xiCol < xoTable.Columns.Count; xiCol++)
                {
                    if (xiCol > 0) xsLine += "\t";
                    string xsColmName = xoTable.Columns[xiCol].ColumnName;
                    xsLine += xsColmName;
                    if (xiTblTypeColm < 0) if (xsColmName.Trim().ToUpper() == "TABLE_TYPE") xiTblTypeColm = xiCol;
                    if (xiTblNameColm < 0) if (xsColmName.Trim().ToUpper() == "TABLE_NAME") xiTblNameColm = xiCol;
                }

                for (int xii = 0; xii < xoTable.Rows.Count; xii++)
                {
                    string xsTblNm = "";
                    bool xbIsTbl = false;
                    for (int xi2 = 0; xi2 < xoTable.Columns.Count; xi2++)
                    {
                        object xo = xoTable.Rows[xii][xi2];
                        if (xi2 == xiTblTypeColm)
                        {
                            string xs = xo.ToString().Trim();
                            if (xs == "TABLE" || xs == "BASE TABLE") xbIsTbl = true;
                        }
                        else
                        {
                            if (xi2 == xiTblNameColm)
                            {
                                xsTblNm = xo.ToString().Trim();
                            }
                        }
                    }
                    if (xbIsTbl)
                    {
                        DatabaseTables.Add(xsTblNm);
                    }
                }
            }
            catch (Exception xoExc)
            {
                Message = xoExc.Message + "  (" + xsConnStr + ")";
            }

            //
            // build the foreign keys list
            //
            if (Message.Length == 0)
            {
                //
                // pass through each table, and for all *_id fields, try to see if matchup to *s table (i.e. {table}.user_id column, look for Users.id)
                //
                DatabaseTables.Sort();
                ForeignKeys = new List<ForeignKey>(0);
                try
                {
                    foreach (string xsTable in DatabaseTables)
                    {
                        //
                        // get the columns in the table
                        //
                        List<TableColumnDataType> xoColumns = GetTableColumns(xoConn, xsDatabase, xsTable);
                        if (xoColumns.Count > 0)
                        {
                            for (int xii = 0; xii < xoColumns.Count; xii++)
                            {
                                string xsColm = xoColumns[xii].Column;
                                string xsType = xoColumns[xii].DataType;
                                int xi = xsColm.ToUpper().IndexOf("_ID");
                                if (xi > 0 && xsType.ToUpper() == "INT")
                                {
                                    string xsFkTable = xsColm.Substring(0, xi);
                                    if (xsFkTable.Substring(xsFkTable.Length - 1, 1).ToLower() == "y")
                                    {
                                        xsFkTable = xsFkTable.Substring(0, xsFkTable.Length - 1) + "ies";
                                    }
                                    else
                                    {
                                        if (xsFkTable.Substring(xsFkTable.Length - 1, 1).ToLower() == "s")
                                        {
                                            xsFkTable = xsFkTable.Substring(0, xsFkTable.Length - 0) + "es";
                                        }
                                        else
                                        {
                                            xsFkTable = xsFkTable.Substring(0, xsFkTable.Length - 0) + "s";
                                        }
                                    }
                                    for (int xiTbl = 0; xiTbl < DatabaseTables.Count; xiTbl++)
                                    {
                                        if (xsFkTable.ToLower() == DatabaseTables[xiTbl].ToLower())
                                        {
                                            //
                                            // match!  (if matching table has an "id" field of type "int")
                                            //
                                            List<TableColumnDataType> xoColumns2 = GetTableColumns(xoConn, xsDatabase, xsFkTable);
                                            foreach (TableColumnDataType xoCol in xoColumns2)
                                            {
                                                if (xoCol.Column.ToUpper() == "ID")
                                                {
                                                    if (xoCol.DataType.ToUpper() == "INT")
                                                    {
                                                        ForeignKeys.Add(new ForeignKey(xsTable, xsColm, xsFkTable, xoCol.Column));
                                                    }
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception xoExc)
                {
                    Message = xoExc.Message + "  (" + xsConnStr + ")";
                }
            }

            //
            // cleanup
            //
            if (xoTable != null)
            {
                try { xoTable.Dispose(); }
                catch { }
                xoTable = null;
            }

            if (xoConn != null)
            {
                try { xoConn.Close(); }
                catch { }
                try { xoConn.Dispose(); }
                catch { }
                xoConn = null;
            }

            return (Message.Length == 0);
        }

        private List<TableColumnDataType> GetTableColumns(SqlConnection poAlreadyOpenConn, string psDatabaseName, string psTable)
        {
            List<TableColumnDataType> xoColumns = new List<TableColumnDataType>(0);
            DataTable xoTable = null;

            xoTable = poAlreadyOpenConn.GetSchema("Columns", new[] { psDatabaseName, null, psTable });
            int xiColmNameIdx = -1, xiDataTypeIdx = -1;
            for (int xii = 0; xii < xoTable.Columns.Count; xii++)
            {
                string xsColm = xoTable.Columns[xii].ColumnName.ToString().Trim().ToUpper();
                if (xsColm == "COLUMN_NAME")
                {
                    xiColmNameIdx = xii;
                }
                else
                {
                    if (xsColm == "DATA_TYPE")
                    {
                        xiDataTypeIdx = xii;
                    }
                }
            }
            if (xiColmNameIdx >= 0 && xiDataTypeIdx >= 0)
            {
                for (int xii = 0; xii < xoTable.Rows.Count; xii++)
                {
                    TableColumnDataType xoColm = new TableColumnDataType();
                    xoColm.Column = xoTable.Rows[xii][xiColmNameIdx].ToString();
                    xoColm.DataType = xoTable.Rows[xii][xiDataTypeIdx].ToString();
                    xoColumns.Add(xoColm);
                }
            }

            //
            // cleanup
            //
            if (xoTable != null)
            {
                try { xoTable.Dispose(); }
                catch { }
                xoTable = null;
            }

            // wrapup
            return (xoColumns);
        }
    }

    public class ForeignKeyField
    {
        public string Table { get; set; }
        public string Column { get; set; }

        public ForeignKeyField()
        {
            Table = "";
            Column = "";
        }

        public ForeignKeyField(string psTable, string psColumn)
        {
            Table = psTable;
            Column = psColumn;
        }
    }

    public class ForeignKey
    {
        public ForeignKeyField TableA { get; set; }
        public ForeignKeyField TableB { get; set; }

        public ForeignKey()
        {
            TableA = new ForeignKeyField();
            TableB = new ForeignKeyField();
        }

        public ForeignKey(string psTableA, string psColumnA, string psTableB, string psColumnB)
        {
            TableA = new ForeignKeyField(psTableA, psColumnA);
            TableB = new ForeignKeyField(psTableB, psColumnB);
        }

        public ForeignKey(ForeignKeyField poTableA, ForeignKeyField poTableB)
        {
            TableA = new ForeignKeyField(poTableA.Table, poTableA.Column);
            TableB = new ForeignKeyField(poTableB.Table, poTableB.Column);
        }
    }
}
