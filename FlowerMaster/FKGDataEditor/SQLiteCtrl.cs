using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;

namespace FKGDataEditor
{
    public class SQLiteCtrl
    {
        #region Define

        /// <summary>
        /// 資料庫檔案路徑
        /// <para>PATH_DB_FILE = "./GirlsDB.sqlite"</para>
        /// </summary>
        private const String PATH_DB_FILE = "./GirlsDB.sqlite";

        /// <summary>
        /// 資料表名稱:基本資料
        /// <para>TABLE_NAME_BASIC_INFO = "BasicInformation"</para>
        /// </summary>
        private const String TABLE_NAME_BASIC_INFO = "BasicInformation";

        //---欄位名稱---
        private const String COLUMN_NAME_ID = "ID";

        private const String COLUMN_NAME_IMG_BASE64 = "ImgBase64";
        private const String COLUMN_NAME_NAMES = "Names";
        private const String COLUMN_NAME_NAMES_JPN = "NamesJPN";
        private const String COLUMN_NAME_NAMES_CHT = "NamesCHT";
        private const String COLUMN_NAME_NAMES_CHS = "NamesCHS";
        private const String COLUMN_NAME_NAMES_ENU = "NamesENU";
        private const String COLUMN_NAME_FKG_ID = "FKGID";
        private const String COLUMN_NAME_RARE = "Rare";
        private const String COLUMN_NAME_TYPE = "Type";
        private const String COLUMN_NAME_NATIONALITY = "Nationality";
        private const String COLUMN_NAME_NOTE = "Note";

        #endregion Define

        private static readonly Lazy<SQLiteCtrl> _lazyInstance = new Lazy<SQLiteCtrl>(() => new SQLiteCtrl(), true);

        public static SQLiteCtrl Data
        {
            get
            {
                return _lazyInstance.Value;
            }
        }

        #region Private Member

        private SQLiteConnection _dbConnection;
        private bool _isConnected = false;

        #endregion Private Member

        public SQLiteCtrl()
        {
        }

        #region Public Method

        public void Init()
        {
            if (_isConnected)
            {
                return;
            }

            //Create file
            if (!File.Exists(PATH_DB_FILE))
            {
                try
                {
                    SQLiteConnection.CreateFile(PATH_DB_FILE);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(String.Format("[SQLiteCtrl] Create file fail:{0}", e.Message));
                }
            }

            //Open DB
            try
            {
                _dbConnection = new SQLiteConnection(String.Format("DATA SOURCE={0}; VERSION=3;", PATH_DB_FILE));
                _dbConnection.Open();
            }
            catch (Exception e)
            {
                Debug.WriteLine(String.Format("[SQLiteCtrl] Open database fail:{0}", e.Message));
            }

            //Create table
            CreateBasicInfoTable();

            //Add other column into basic info table.
            AddBasicInfoColumn();

            _isConnected = true;
        }

        public void InsertData(GirlInfo data)
        {
            /*
             * INSERT OR REPLACE INTO BasicInformation
             * (
             * ID,
             * ImgBase64,
             * Names,
             * NamesJPN,
             * NamesCHT,
             * NamesCHS,
             * NamesENU,
             * Rare,
             * Type,
             * Nationality
             * ) VALUES (
             * 1,
             * xxxxxxxxxxxxxxx...,
             * names,
             * jpn names,
             * cht names,
             * chs names,
             * enu names,
             * 2,
             * 1,
             * 1,
             * 123456
             * )
             */

            String cmdText = "INSERT OR REPLACE INTO " + TABLE_NAME_BASIC_INFO
            + "("
            + COLUMN_NAME_ID + ", "
            + COLUMN_NAME_IMG_BASE64 + ", "
            + COLUMN_NAME_NAMES + ", "
            + COLUMN_NAME_NAMES_JPN + ", "
            + COLUMN_NAME_NAMES_CHT + ", "
            + COLUMN_NAME_NAMES_CHS + ", "
            + COLUMN_NAME_NAMES_ENU + ", "
            + COLUMN_NAME_RARE + ", "
            + COLUMN_NAME_TYPE + ", "
            + COLUMN_NAME_NATIONALITY + ", "
            + COLUMN_NAME_NOTE + ", "
            + COLUMN_NAME_FKG_ID
            + ") VALUES ("
            + data.ID + ", "
            + String.Format("\"{0}\", ", data.ImgBase64)
            + String.Format("\"{0}\", ", data.Names)
            + String.Format("\"{0}\", ", data.NamesJPN)
            + String.Format("\"{0}\", ", data.NamesCHT)
            + String.Format("\"{0}\", ", data.NamesCHS)
            + String.Format("\"{0}\", ", data.NamesENU)
            + (int)data.Rare + ", "
            + (int)data.Type + ", "
            + (int)data.Nationality + ", "
            + String.Format("\"{0}\"", data.Note) + ", "
            + data.FKGID
            + ")";

            using (SQLiteCommand cmd = new SQLiteCommand(cmdText, _dbConnection))
            {
                cmd.ExecuteNonQuery();
            }
        }

        public List<GirlInfo> LoadData()
        {
            List<GirlInfo> ret = new List<GirlInfo>();
            String cmdText = "SELECT * FROM " + TABLE_NAME_BASIC_INFO;

            using (SQLiteCommand cmd = new SQLiteCommand(cmdText, _dbConnection))
            {
                SQLiteDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    GirlInfo info = ParseData(rdr);

                    ret.Add(info);
                }
            }
            return ret;
        }

        public List<GirlInfo> SearchData(String keyword, GirlInfoEnum.Types type, GirlInfoEnum.Nationalities nationality, int rare)
        {
            List<GirlInfo> ret = new List<GirlInfo>();
            String condition = "";

            if (keyword != "")
            {
                condition += "( ";
                condition += COLUMN_NAME_NAMES_JPN + String.Format(" LIKE '%{0}%' OR ", keyword);
                condition += COLUMN_NAME_NAMES_CHT + String.Format(" LIKE '%{0}%' OR ", keyword);
                condition += COLUMN_NAME_NAMES_CHS + String.Format(" LIKE '%{0}%' OR ", keyword);
                condition += COLUMN_NAME_NAMES_ENU + String.Format(" LIKE '%{0}%' OR ", keyword);
                condition += COLUMN_NAME_NOTE + String.Format(" LIKE '%{0}%'", keyword);
                condition += " )";
            }

            if (type != GirlInfoEnum.Types.NotCare)
            {
                int itype = (int)type;
                if (condition != "")
                {
                    condition += " AND ";
                }
                condition += String.Format("{0} == {1}", COLUMN_NAME_TYPE, itype);
            }

            if (nationality != GirlInfoEnum.Nationalities.NotCare)
            {
                int inational = (int)nationality;
                if (condition != "")
                {
                    condition += " AND ";
                }
                condition += String.Format("{0} == {1}", COLUMN_NAME_NATIONALITY, inational);
            }

            if (rare > 0)
            {
                if (condition != "")
                {
                    condition += " AND ";
                }
                condition += String.Format("{0} == {1}", COLUMN_NAME_RARE, rare);
            }

            if (condition != "")
            {
                condition = " WHERE " + condition;
            }

            String cmdText = "SELECT * FROM " + TABLE_NAME_BASIC_INFO + condition;
            using (SQLiteCommand cmd = new SQLiteCommand(cmdText, _dbConnection))
            {
                try
                {
                    SQLiteDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        GirlInfo info = ParseData(rdr);
                        ret.Add(info);
                    }
                }
                catch (Exception e)
                {
                }
            }
            return ret;
        }

        #endregion Public Method

        #region Private Method

        private void CreateBasicInfoTable()
        {
            /*
             * CREATE TABLE IF NOT EXISTS `BasicInformation`
             *  (
             * `ID`	INTEGER,
             * `ImgBase64`	TEXT,
             * `Names`	TEXT,
             * `NamesJPN`	TEXT,
             * `NamesCHT`	TEXT,
             * `NamesCHS`	TEXT,
             * `NamesENU`	TEXT,
             * `Rare`	INTEGER,
             * `Type`	INTEGER,
             * `Nationality`	INTEGER,
             * PRIMARY KEY(ID)
             * );
             */
            String cmdText = "CREATE TABLE IF NOT EXISTS " + TABLE_NAME_BASIC_INFO
                + " ( "
                + COLUMN_NAME_ID + " INTEGER,"
                + COLUMN_NAME_IMG_BASE64 + " TEXT,"
                + COLUMN_NAME_NAMES + " TEXT,"
                + COLUMN_NAME_NAMES_JPN + " TEXT,"
                + COLUMN_NAME_NAMES_CHT + " TEXT,"
                + COLUMN_NAME_NAMES_CHS + " TEXT,"
                + COLUMN_NAME_NAMES_ENU + " TEXT,"
                + COLUMN_NAME_RARE + " INTEGER,"
                + COLUMN_NAME_TYPE + " INTEGER,"
                + COLUMN_NAME_NATIONALITY + " INTEGER,"
                + " PRIMARY KEY(" + COLUMN_NAME_ID + ")"
                + " );";

            using (SQLiteCommand cmd = new SQLiteCommand(cmdText, _dbConnection))
            {
                cmd.ExecuteNonQuery();
            }
        }

        private void AddBasicInfoColumn()
        {
            String cmdHeader = "ALTER TABLE " + TABLE_NAME_BASIC_INFO + " ADD COLUMN ";
            String[] addColumns = {
                COLUMN_NAME_NOTE + " TEXT",
                COLUMN_NAME_FKG_ID + " INTEGER"
            };

            foreach (String cmdCol in addColumns)
            {
                String cmdText = cmdHeader + cmdCol;
                using (SQLiteCommand cmd = new SQLiteCommand(cmdText, _dbConnection))
                {
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                    }
                }
            }
        }

        private static GirlInfo ParseData(SQLiteDataReader rdr)
        {
            GirlInfo info = new GirlInfo();

            info.ID = Convert.ToInt32(rdr[COLUMN_NAME_ID]);
            info.ImgBase64 = Convert.ToString(rdr[COLUMN_NAME_IMG_BASE64]);
            //info.ImageSrc = GirlInfo.Base642Image(info.ImgBase64);
            info.Names = Convert.ToString(rdr[COLUMN_NAME_NAMES]);
            info.NamesJPN = Convert.ToString(rdr[COLUMN_NAME_NAMES_JPN]);
            info.NamesCHT = Convert.ToString(rdr[COLUMN_NAME_NAMES_CHT]);
            info.NamesCHS = Convert.ToString(rdr[COLUMN_NAME_NAMES_CHS]);
            info.NamesENU = Convert.ToString(rdr[COLUMN_NAME_NAMES_ENU]);
            info.Rare = Convert.ToInt32(rdr[COLUMN_NAME_RARE]);
            info.Type = (GirlInfoEnum.Types)Convert.ToInt32(rdr[COLUMN_NAME_TYPE]);
            info.Nationality = (GirlInfoEnum.Nationalities)Convert.ToInt32(rdr[COLUMN_NAME_NATIONALITY]);
            info.Note = Convert.ToString(rdr[COLUMN_NAME_NOTE]);
            int variable = 0;
            int.TryParse(Convert.ToString(rdr[COLUMN_NAME_FKG_ID]), out variable);
            info.FKGID = variable;
            return info;
        }

        #endregion Private Method
    }
}