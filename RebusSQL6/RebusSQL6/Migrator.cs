using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//
// this migrator will bring a database up to date by adding new tables, columns and indices
//
// it does NOT:   modify a column, i.e. change from one type to another, or modify a property such as the size
//                drop tables or columns
//                modify an index (only checks if the index name matches)
//
//
// update: if a "Char" type column size changes, the migrator will now perform an ALTER TABLE [table] ALTER COLUMN [column] CHAR([newSize])
//

namespace RebusDatabaseMigrator
{
    public enum DatabaseType
    {
        Access = 0//,
        //SQLServer = 1
    }

    public enum ColumnType
    {
        Unknown = -1,
        AutoIncrement = 0,
        VarChar = 1,
        Integer16 = 2,
        Integer32 = 3,
        Integer64 = 4,
        Memo = 5,
        Float = 6,
        Bit = 7,
        Boolean = 8,
        DateTime = 9,
        Single = 10,
        Double = 11,
        Char = 12,
        Currency = 13, 
        Text = 14,
        Integer = 15
    }

    public enum TableActionType
    {
        Migrate = 0,
        Create = 1,
        Drop = 2
    }

    public partial class DatabaseConnectionInfo
    {
        public DatabaseType Type { get; set; }
        public string Name { get; set; }
        public string Server { get; set; }
        public string UserID { get; set; }
        public string Password { get; set; }

        public DatabaseConnectionInfo(DatabaseType peType, string psName, string psServer = "", string psUserID = "", string psPswd = "")
        {
            Type = peType;
            Name = psName;
            Server = psServer;
            UserID = psUserID;
            Password = psPswd;
        }
    }



    public partial class Migrator
    {
        private DatabaseConnectionInfo Database;                    // for connecting to the database to check against
        private List<MigrationTable> TablesAsTheyShouldBe;          // this is what the structure should look like

        private struct ColumnSizeChange
        {
            public string Name;
            public int NewSize;
            public ColumnType Type;
        }



        // constructor
        public Migrator(DatabaseConnectionInfo poDbConnInfo, List<Table> poTables)
        {
            Database = poDbConnInfo;
            TablesAsTheyShouldBe = new List<MigrationTable>(0);
            for (int xii = 0; xii < poTables.Count; xii++)
            {
                MigrationTable xoTbl = new MigrationTable();
                xoTbl.Name = poTables[xii].Name;
                xoTbl.Columns = poTables[xii].Columns;
                xoTbl.Indices = poTables[xii].Indices;
                TablesAsTheyShouldBe.Add(xoTbl);
            }
        }



        public List<string> Migrate()
        {
            string xsErrMsg = "";
            List<string> xsErrMsgs = new List<string>(0);

            System.Data.OleDb.OleDbCommand xoCmd = new OleDbCommand();
            System.Data.OleDb.OleDbConnection xoConn = new OleDbConnection();

            if (OpenDatabase(out xsErrMsg, out xoCmd, out xoConn))
            {
                List<MigrationTable> xoCurrTables = GetDatabaseTables(xoConn, out xsErrMsg);
                xoCmd.Connection = xoConn;
                if (xsErrMsg.Length == 0)
                {
                    MarkTableActions(xoCurrTables);
                    //////DropTables(xoCmd, ref xsErrMsgs);
                    CreateTables(xoCmd, ref xsErrMsgs);
                    MigrateTables(xoCmd, xoCurrTables, ref xsErrMsgs);
                }
                else
                {
                    xsErrMsgs.Add(xsErrMsg);
                }
            }
            else
            {
                xsErrMsgs.Add(xsErrMsg);
            }

            // cleanup
            try
            {
                xoCmd.Dispose();
            }
            catch { }
            try
            {
                xoConn.Dispose();
            }
            catch { }
            xoCmd = null;
            xoConn = null;

            return (xsErrMsgs);
        }

        private void MigrateTables(System.Data.OleDb.OleDbCommand poCmd, List<MigrationTable> poTables, ref List<string> psErrMsgs)
        {
            for (int xii = 0; xii < TablesAsTheyShouldBe.Count; xii++)
            {
                if (TablesAsTheyShouldBe[xii].Action == TableActionType.Migrate)
                {
                    //
                    // create any new index/indices
                    //
                    List<string> xsNewIdxs = GetNewIndices(TablesAsTheyShouldBe[xii], poTables);
                    if (xsNewIdxs.Count > 0)
                    {
                        for (int xiIdx = 0; xiIdx < xsNewIdxs.Count; xiIdx++)
                        {
                            string xsErr = "";
                            List<string> xsColms = new List<string>(0);
                            for (int xi = 0; xi < TablesAsTheyShouldBe[xii].Indices.Count; xi++)
                            {
                                if (xsNewIdxs[xiIdx].Trim().ToLower() == TablesAsTheyShouldBe[xii].Indices[xi].Name.Trim().ToLower())
                                {
                                    xsColms = TablesAsTheyShouldBe[xii].Indices[xi].Columns;
                                    break;
                                }
                            }
                            if (!CreateIndex(poCmd, xsNewIdxs[xiIdx], TablesAsTheyShouldBe[xii].Name, xsColms, out xsErr))
                            {
                                psErrMsgs.Add(xsErr);
                            }
                        }
                    }

                    //
                    // create any new column(s)
                    //
                    List<string> xsNewColms = GetNewColumns(TablesAsTheyShouldBe[xii], poTables);
                    if (xsNewColms.Count > 0)
                    {
                        for (int xiNewColm = 0; xiNewColm < xsNewColms.Count; xiNewColm++)
                        {
                            string xsColm = xsNewColms[xiNewColm].Trim().ToLower();
                            Column xoColm = new Column();
                            for (int xiCol = 0; xiCol < TablesAsTheyShouldBe[xii].Columns.Count; xiCol ++)
                            {
                                if (xsColm == TablesAsTheyShouldBe[xii].Columns[xiCol].Name.Trim().ToLower())
                                {
                                    xoColm = TablesAsTheyShouldBe[xii].Columns[xiCol];
                                    break;
                                }
                            }

                            string xsSQL = "ALTER TABLE " + TablesAsTheyShouldBe[xii].Name + " ADD COLUMN " + xsColm + " ";
                            xsSQL += ColumnTypeForSQL(xoColm);

                            try
                            {
                                poCmd.CommandText = xsSQL;
                                poCmd.CommandType = System.Data.CommandType.Text;
                                poCmd.ExecuteNonQuery();
                            }
                            catch (Exception xoExc)
                            {
                                psErrMsgs.Add(xoExc.Message + "  (" + xsSQL + ")");
                            }
                        }
                    }

                    //
                    // resize any column(s) with new size(s)
                    //
                    List<ColumnSizeChange> xrNewSizes = GetSizeChangeColumns(TablesAsTheyShouldBe[xii], poTables);
                    if (xrNewSizes.Count > 0)
                    {
                        for (int xiNewSize = 0; xiNewSize < xrNewSizes.Count; xiNewSize++)
                        {
                            string xsColm = xrNewSizes[xiNewSize].Name.Trim().ToLower();
                            Column xoColm = new Column();
                            xoColm.Name = xsColm;
                            xoColm.Type = xrNewSizes[xiNewSize].Type;
                            xoColm.Size = xrNewSizes[xiNewSize].NewSize;
                            xoColm.PrimaryKey = false;

                            string xsSQL = "ALTER TABLE " + TablesAsTheyShouldBe[xii].Name + " ALTER COLUMN " + xsColm + " ";
                            xsSQL += ColumnTypeForSQL(xoColm);

                            try
                            {
                                poCmd.CommandText = xsSQL;
                                poCmd.CommandType = System.Data.CommandType.Text;
                                poCmd.ExecuteNonQuery();
                            }
                            catch (Exception xoExc)
                            {
                                psErrMsgs.Add(xoExc.Message + "  (" + xsSQL + ")");
                            }
                        }
                    }
                }
            }
        }

        private List<string> GetNewColumns(MigrationTable poTbl, List<MigrationTable> poTables)
        {
            List<String> xsNewColumns = new List<string>(0);

            string xsTbl = poTbl.Name.Trim().ToLower();
            for (int xii = 0; xii < poTables.Count; xii++)
            {
                if (poTables[xii].Name.Trim().ToLower() == xsTbl)
                {
                    for (int xiColm = 0; xiColm < poTbl.Columns.Count; xiColm++)
                    {
                        string xsColm = poTbl.Columns[xiColm].Name.Trim().ToLower();
                        bool xbFound = false;
                        for (int xi = 0; xi < poTables[xii].Columns.Count; xi++)
                        {
                            if (poTables[xii].Columns[xi].Name.ToLower() == xsColm)
                            {
                                xbFound = true;
                                break;
                            }
                        }
                        if (!xbFound)
                        {
                            // column is in "as they should be", but not currently a column for the table in the actual database
                            xsNewColumns.Add(xsColm);
                        }
                    }

                    break;
                }
            }

            return (xsNewColumns);
        }

        private List<ColumnSizeChange> GetSizeChangeColumns(MigrationTable poTbl, List<MigrationTable> poTables)
        {
            List<ColumnSizeChange> xrSizeChangeColumns = new List<ColumnSizeChange>(0);

            string xsTbl = poTbl.Name.Trim().ToLower();
            for (int xii = 0; xii < poTables.Count; xii++)
            {
                if (poTables[xii].Name.Trim().ToLower() == xsTbl)
                {
                    for (int xiColm = 0; xiColm < poTbl.Columns.Count; xiColm++)
                    {
                        string xsColm = poTbl.Columns[xiColm].Name.Trim().ToLower();
                        for (int xi = 0; xi < poTables[xii].Columns.Count; xi++)
                        {
                            if (poTables[xii].Columns[xi].Name.ToLower() == xsColm)
                            {
                                if (poTbl.Columns[xiColm].Type == ColumnType.Char && poTbl.Columns[xiColm].Type == poTables[xii].Columns[xi].Type)
                                {
                                    if (poTbl.Columns[xiColm].Size != poTables[xii].Columns[xi].Size)
                                    {
                                        ColumnSizeChange xo = new ColumnSizeChange();
                                        xo.Name = poTbl.Columns[xiColm].Name;
                                        xo.NewSize = poTbl.Columns[xiColm].Size;
                                        xo.Type = poTbl.Columns[xiColm].Type;
                                        xrSizeChangeColumns.Add(xo);
                                    }
                                }
                                break;
                            }
                        }
                    }

                    break;
                }
            }

            return (xrSizeChangeColumns);
        }

        private List<string> GetNewIndices(MigrationTable poTbl, List<MigrationTable> poTables)
        {
            List<String> xsNewIdxs = new List<string>(0);

            string xsTbl = poTbl.Name.Trim().ToLower();
            for (int xii = 0; xii < poTables.Count; xii++)
            {
                if (poTables[xii].Name.Trim().ToLower() == xsTbl)
                {
                    for (int xiIdx = 0; xiIdx < poTbl.Indices.Count; xiIdx++)
                    {
                        string xsIdx = poTbl.Indices[xiIdx].Name.Trim().ToLower();
                        bool xbFound = false;
                        for (int xi = 0; xi < poTables[xii].Indices.Count; xi++)
                        {
                            if (poTables[xii].Indices[xi].Name.ToLower() == xsIdx)
                            {
                                xbFound = true;
                                break;
                            }
                        }
                        if (!xbFound)
                        {
                            // index is in "as they should be", but not currently an index for the table in the actual database
                            xsNewIdxs.Add(xsIdx);
                        }
                    }

                    break;
                }
            }

            return (xsNewIdxs);
        }

        private void CreateTables(System.Data.OleDb.OleDbCommand poCmd, ref List<string> psErrMsgs)
        {
            for (int xii = 0; xii < TablesAsTheyShouldBe.Count; xii++)
            {
                if (TablesAsTheyShouldBe[xii].Action == TableActionType.Create)
                {
                    //
                    // create the table
                    //
                    string xsSQL = "CREATE TABLE [" + TablesAsTheyShouldBe[xii].Name + "] (";
                    for (int xiColm = 0; xiColm < TablesAsTheyShouldBe[xii].Columns.Count; xiColm++)
                    {
                        if (xiColm > 0) xsSQL += ", ";
                        xsSQL += "[" + TablesAsTheyShouldBe[xii].Columns[xiColm].Name.Trim() + "] " + ColumnTypeForSQL(TablesAsTheyShouldBe[xii].Columns[xiColm]);
                    }
                    xsSQL += ")";
                    try
                    {
                        poCmd.CommandText = xsSQL;
                        poCmd.CommandType = System.Data.CommandType.Text;
                        poCmd.ExecuteNonQuery();
                    }
                    catch (Exception xoExc)
                    {
                        psErrMsgs.Add(xoExc.Message + "  (" + xsSQL + ")");
                    }

                    //
                    // create any indices for the table
                    //
                    for (int xiIdx = 0; xiIdx < TablesAsTheyShouldBe[xii].Indices.Count; xiIdx++)
                    {
                        string xsErr = "";
                        if (!CreateIndex(poCmd, TablesAsTheyShouldBe[xii].Indices[xiIdx].Name, TablesAsTheyShouldBe[xii].Name, TablesAsTheyShouldBe[xii].Indices[xiIdx].Columns, out xsErr))
                        {
                            psErrMsgs.Add(xsErr);
                        }
                    }
                }
            }
        }

        private bool CreateIndex(System.Data.OleDb.OleDbCommand poCmd, string psIdxName, string psTableName, List<string> psColumns, out string psErrMsg)
        {
            string xsErrMsg = "", xsSQL = "";

            xsSQL = "CREATE INDEX [" + psIdxName.Trim() + "] ON [" + psTableName.Trim() + "] (";
            for (int xiColm = 0; xiColm < psColumns.Count; xiColm++)
            {
                if (xiColm > 0) xsSQL += ", ";
                xsSQL += "[" + psColumns[xiColm].Trim() + "]";
            }
            xsSQL += ")";
                    
            try
            {
                poCmd.CommandText = xsSQL;
                poCmd.CommandType = System.Data.CommandType.Text;
                poCmd.ExecuteNonQuery();
            }
            catch (Exception xoExc)
            {
                xsErrMsg = xoExc.Message + "  (" + xsSQL + ")";
            }

            psErrMsg = xsErrMsg;
            return (xsErrMsg.Length == 0);
        }

        private string ColumnTypeForSQL(Column poColm)
        {
            string xsSQL = "";
            switch (poColm.Type)
            {
                case ColumnType.AutoIncrement:
                    xsSQL += "AUTOINCREMENT";
                    //if (poColm.PrimaryKey) xsSQL += " PRIMARY KEY";
                    break;
                case ColumnType.Boolean:
                    xsSQL += "LOGICAL";
                    break;
                case ColumnType.Char:
                case ColumnType.VarChar:
                case ColumnType.Text:
                    if (poColm.Size > 0)
                    {
                        xsSQL += "CHAR(" + poColm.Size.ToString() + ")";
                    }
                    else
                    {
                        xsSQL += "CHAR";
                    }
                    break;
                case ColumnType.Memo:
                    xsSQL += "MEMO";
                    break;
                case ColumnType.Integer:
                case ColumnType.Integer16:
                case ColumnType.Integer32:
                    xsSQL += "INTEGER";
                    break;
                case ColumnType.Integer64:
                    xsSQL += "LONG";
                    break;
                case ColumnType.DateTime:
                    xsSQL += "DATETIME";
                    break;
                case ColumnType.Float:
                case ColumnType.Single:
                    xsSQL += "SINGLE";
                    break;
                case ColumnType.Bit:
                    xsSQL += "LOGICAL";
                    break;
                case ColumnType.Double:
                    xsSQL += "DOUBLE";
                    break;
                case ColumnType.Currency:
                    xsSQL += "CURRENCY";
                    break;
                default:
                    break;
            }

            if (poColm.PrimaryKey) xsSQL += " PRIMARY KEY";
            return (xsSQL.Trim());
        }

        //private void DropTables(System.Data.OleDb.OleDbCommand poCmd, ref List<string> psErrMsgs)
        //{
        //    for (int xii = 0; xii < TablesAsTheyShouldBe.Count; xii++)
        //    {
        //        if (TablesAsTheyShouldBe[xii].Action == TableActionType.Drop)
        //        {
        //            string xsSQL = "DROP TABLE" + TablesAsTheyShouldBe[xii].Name;
        //            try
        //            {
        //                poCmd.CommandText = xsSQL;
        //                poCmd.CommandType = System.Data.CommandType.Text;
        //                poCmd.ExecuteNonQuery();
        //            }
        //            catch (Exception xoExc)
        //            {
        //                psErrMsgs.Add(xoExc.Message + "  (" + xsSQL + ")");
        //            }
        //        }
        //    }
        //}

        private void MarkTableActions(List<MigrationTable> poCurrDbTables)
        {
            //
            // compare table names from database with the table names we should have
            // if in passed list and not in migrator list, DROP (not currently active)
            // if not in passed list but in migrator list, ADD
            // otherwise, just flag as migrate
            //
            // result:  passed in tables are flagged in either as Migrate or Create
            //          current tables flagged as either Migrate or Drop

            // default - assume in both sets, so only a migration
            for (int xii = 0; xii < TablesAsTheyShouldBe.Count; xii++) TablesAsTheyShouldBe[xii].Action = TableActionType.Migrate;

            // mark these, and flag as DROP if the don't appear in "as they should be now" list
            for (int xii = 0; xii < TablesAsTheyShouldBe.Count; xii++) TablesAsTheyShouldBe[xii].Action = TableActionType.Migrate;

            // first, pass through the "as they should be now" list, and if not in the current database, they are to be created
            for (int xiToBe = 0; xiToBe < TablesAsTheyShouldBe.Count; xiToBe++)
            {
                string xsTbl = TablesAsTheyShouldBe[xiToBe].Name.Trim().ToLower();
                bool xbFound = false;
                for (int xiCurr = 0; xiCurr < poCurrDbTables.Count; xiCurr++)
                {
                    if (poCurrDbTables[xiCurr].Name.Trim().ToLower() == xsTbl)
                    {
                        xbFound = true;
                        break;
                    }
                }
                if (!xbFound) TablesAsTheyShouldBe[xiToBe].Action = TableActionType.Create;
            }

            // finally, pass through the current list, and if not in the "as they should be now" list, the table should be dropped
            for (int xiCurr = 0; xiCurr < poCurrDbTables.Count; xiCurr++)
            {
                string xsTbl = poCurrDbTables[xiCurr].Name.Trim().ToLower();
                bool xbFound = false;
                for (int xiToBe = 0; xiToBe < TablesAsTheyShouldBe.Count; xiToBe++)
                {
                    if (TablesAsTheyShouldBe[xiToBe].Name.Trim().ToLower() == xsTbl)
                    {
                        xbFound = true;
                        break;
                    }
                }
                if (!xbFound) poCurrDbTables[xiCurr].Action = TableActionType.Drop;
            }
        }

        private List<string> GetTableIndices(OleDbConnection poConn, string psTableName, out string psErrMsg)
        {
            string xsErrMsg = "";
            List<string> xsIndices = new List<string>(0);

            try
            {
                DataTable xoData = poConn.GetSchema("Indexes", new[] { null, null, null, null, psTableName });
                int xiIndexNameColm = -1;
                for (int xiCol = 0; xiCol < xoData.Columns.Count; xiCol++)
                {
                    string xsColmName = xoData.Columns[xiCol].ColumnName;
                    if (xiIndexNameColm < 0)
                    {
                        if (xsColmName.Trim().ToUpper() == "INDEX_NAME")
                        {
                            xiIndexNameColm = xiCol;
                            break;
                        }
                    }
                }

                if (xiIndexNameColm >= 0)
                {
                    for (int xii = 0; xii < xoData.Rows.Count; xii++)
                    {
                        string xsIndexName = xoData.Rows[xii][xiIndexNameColm].ToString().Trim();
                        xsIndices.Add(xsIndexName);
                    }
                }
                 xoData.Dispose();
            }
            catch (Exception xoExc)
            {
                xsErrMsg = xoExc.Message;
            }

            psErrMsg = xsErrMsg;

            return (xsIndices);
        }

        private List<MigrationTable> GetDatabaseTables(OleDbConnection poConn, out string psErrMsg)
        {
            string xsErrMsg = "";
            List<MigrationTable> xoTables = new List<MigrationTable>(0);

            try
            {
                DataTable xoData = poConn.GetSchema("TABLES");
                int xiTblTypeColm = -1, xiTblNameColm = -1;
                for (int xiCol = 0; xiCol < xoData.Columns.Count; xiCol++)
                {
                    string xsColmName = xoData.Columns[xiCol].ColumnName;
                    if (xiTblTypeColm < 0) if (xsColmName.Trim().ToUpper() == "TABLE_TYPE") xiTblTypeColm = xiCol;
                    if (xiTblNameColm < 0) if (xsColmName.Trim().ToUpper() == "TABLE_NAME") xiTblNameColm = xiCol;
                }

                for (int xii = 1; xii <= xoData.Rows.Count; xii++)
                {
                    string xsTblNm = "";
                    bool xbIsTbl = false;
                    string xsLine = "";
                    for (int xi2 = 0; xi2 < xoData.Columns.Count; xi2++)
                    {
                        var xv = xoData.Rows[xii - 1][xi2];
                        if (xi2 > 0) xsLine += "\t";
                        xsLine += xv.ToString();
                        if (xi2 == xiTblTypeColm)
                        {
                            string xs = xv.ToString().Trim();
                            if (xs == "TABLE" || xs == "BASE TABLE") xbIsTbl = true;
                        }
                        else
                        {
                            if (xi2 == xiTblNameColm)
                            {
                                xsTblNm = xv.ToString().Trim();
                            }
                        }
                    }
                    if (xbIsTbl)
                    {
                        MigrationTable xoTbl = new MigrationTable();
                        xoTbl.Columns = new List<Column>(0);
                        xoTbl.Name = xsTblNm;

                        //
                        // get columns in table
                        //
                        try
                        {
                            DataTable xoColms = poConn.GetSchema("Columns", new[] { null, null, xsTblNm });
                            int xiColm = -1, xiMaxColm = -1, xiDataTypeColm = -1;
                            for (int xiCol = 0; xiCol < xoColms.Columns.Count; xiCol++)
                            {
                                string xsColm = xoColms.Columns[xiCol].Caption.ToUpper();
                                if (xsColm == "COLUMN_NAME")
                                {
                                    xiColm = xiCol;
                                    //break;
                                }
                                else
                                {
                                    if (xsColm == "CHARACTER_MAXIMUM_LENGTH")
                                    {
                                        xiMaxColm = xiCol;
                                    }
                                    else
                                    {
                                        if (xsColm == "DATA_TYPE") xiDataTypeColm = xiCol;
                                    }
                                }
                            }
                            for (int xiRow = 0; xiRow < xoColms.Rows.Count; xiRow++)
                            {
                                Column xoCol = new Column();
                                xoCol.Name = xoColms.Rows[xiRow][xiColm].ToString();
                                ColumnType xiDataType = ColumnType.Unknown;
                                int xiSize = 0;
                                try
                                {
                                    int xi = Convert.ToInt32(xoColms.Rows[xiRow][xiDataTypeColm]);
                                    if (xi == 130)
                                    {
                                        xiDataType = ColumnType.Char;
                                        xiSize = Convert.ToInt32(xoColms.Rows[xiRow][xiMaxColm]);
                                    }
                                }
                                catch { }
                                xoCol.Type = xiDataType;
                                xoCol.Size = xiSize;
                                xoTbl.Columns.Add(xoCol);
                            }
                            xoColms.Dispose();
                        }
                        catch (Exception xoExc)
                        {
                            xsErrMsg = xoExc.Message;
                            break;
                        }

                        //
                        // get any indices for the table
                        //
                        string xsErr = "";
                        List<string> xsIdxs = GetTableIndices(poConn, xoTbl.Name, out xsErr);
                        if (xsErr.Length == 0)
                        {
                            for (int xiIdx = 0; xiIdx < xsIdxs.Count; xiIdx++)
                            {
                                xoTbl.Indices.Add(new Index(xsIdxs[xiIdx], ""));
                            }
                        }

                        xoTables.Add(xoTbl);
                    }
                }
                xoData.Dispose();
            }
            catch (Exception xoExc)
            {
                xsErrMsg = xoExc.Message;
            }

            psErrMsg = xsErrMsg;
            return (xoTables);
        }

        private bool OpenDatabase(out string psErrMsg, out System.Data.OleDb.OleDbCommand poCmd, out System.Data.OleDb.OleDbConnection poConn)
        {
            string xsErrMsg = "", xsConn = "";
            System.Data.OleDb.OleDbCommand xoCmd = new OleDbCommand();
            System.Data.OleDb.OleDbConnection xoConn = new OleDbConnection();

            switch (Database.Type)
            {
                case DatabaseType.Access:
                    for (int xii = 1; xii <= 2; xii++)
                    {
                        try
                        {
                            if (xii == 1) xsConn = "Provider=Microsoft.Jet.OLEDB.4.0;"; else xsConn = "Provider=Microsoft.ACE.OLEDB.12.0;";
                            xsConn += "Data Source=" + Database.Name + ";";
                            xsConn += "User ID=" + Database.UserID.Trim() + ";";
                            xsConn += "Password=" + Database.Password.Trim();
                        }
                        catch (Exception xoExc)
                        {
                            xsErrMsg = xoExc.Message;
                        }
                        if (xsErrMsg.Length == 0)
                        {
                            try
                            {
                                xoConn.ConnectionString = xsConn;
                                xoConn.Open();
                                xoCmd.Connection = xoConn;
                            }
                            catch (Exception xoExc)
                            {
                                if (xii > 1)
                                {
                                    xsErrMsg = xoExc.Message + "  (" + xsConn + ")";
                                }
                            }
                        }
                        else
                        {
                            break;
                        }
                        if (xii == 1 && xsErrMsg.Length == 0) break;
                    }
                    break;
                //case DatabaseType.SQLServer:
                //    xsErrMsg = "not yet suppored";
                //    break;
                default:
                    break;
            }

            if (xsErrMsg.Length > 0)
            {
                try
                {
                    xoCmd.Dispose();
                }
                catch { }
                try
                {
                    xoConn.Dispose();
                }
                catch { }
                xoCmd = null;
                xoConn = null;
            }
            poCmd = xoCmd;
            poConn = xoConn;
            psErrMsg = xsErrMsg;
            return (xsErrMsg.Length == 0);
        }
    }

    internal class MigrationTable : Table
    {
        public TableActionType Action { get; set; }


        public MigrationTable()
        {
            Action = TableActionType.Migrate;
        }
    }

    public partial class Table
    {
        public string Name { get; set; }
        public List<Column> Columns { get; set; }
        public List<Index> Indices { get; set;
        }

        public Table()
        {
            Initialize();
        }

        public Table(string psName)
        {
            Initialize();
            Name = psName;
        }

        public void Initialize()
        {
            Name = "";
            Columns = new List<Column>(0);
            Indices = new List<Index>(0);
        }
    }

    public partial class Column
    {
        public string Name { get; set; }
        public ColumnType Type { get; set; }
        public bool PrimaryKey { get; set; }
        public int Size { get; set; }

        public Column()
        {
            Initialize();
        }

        public Column(string psName, ColumnType piType)
        {
            Initialize();
            Name = psName;
            Type = piType;
        }

        public Column(string psName, ColumnType piType, int piSize)
        {
            Initialize();
            Name = psName;
            Type = piType;
            Size = piSize;
        }

        public Column(string psName, ColumnType piType, bool pbPrimaryKey)
        {
            Initialize();
            Name = psName;
            Type = piType;
            PrimaryKey = pbPrimaryKey;
        }

        public Column(string psName, ColumnType piType, int piSize, bool pbPrimaryKey)
        {
            Initialize();
            Name = psName;
            Type = piType;
            PrimaryKey = pbPrimaryKey;
            Size = piSize;
        }

        public void Initialize()
        {
            Name = "";
            Type = ColumnType.Unknown;
            PrimaryKey = false;
            Size = 0;
        }
    }

    public partial class Index
    {
        public string Name { get; set; }
        public List<string> Columns;

        public Index(string psName, List<string> psFlds)
        {
            Name = psName;
            Columns = psFlds;
        }

        public Index(string psName, string psFld)
        {
            Name = psName;
            Columns = new List<string>(0);
            Columns.Add(psFld);
        }
    }
}
