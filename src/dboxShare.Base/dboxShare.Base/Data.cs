using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using MySql.Data;
using MySql.Data.MySqlClient;


namespace dboxShare.Base
{


    public static class Data
    {


        /// <summary>
        /// 数据库连接
        /// </summary>
        public static dynamic DBConnection(string ConnectionString)
        {
            try
            {
                if (Common.StringCheck(ConnectionString, "multipleactiveresultsets") == true)
                {
                    return new SqlConnection(ConnectionString);
                }
                else
                {
                    return new MySqlConnection(ConnectionString);
                }
            }
            catch (Exception)
            {

            }

            return null;
        }


        /// <summary>
        /// 执行 SQL 查询
        /// </summary>
        public static void SqlQuery(string Sql, ref object Conn)
        {
            string ConnType = Data.ConnectionType(Conn.GetType());
            dynamic Cmd = null;

            try
            {
                if (ConnType == "SqlConnection")
                {
                    Cmd = new SqlCommand(Data.SqlConvert(ConnType, Sql), (SqlConnection)Conn);
                }
                else if (ConnType == "MySqlConnection")
                {
                    Cmd = new MySqlCommand(Data.SqlConvert(ConnType, Sql), (MySqlConnection)Conn);
                }

                Cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {

            }
            finally
            {
                if (Common.IsNothing(Cmd) == false)
                {
                    Cmd.Dispose();
                }
            }
        }


        /// <summary>
        /// 执行 SQL 插入记录返回自动编号
        /// </summary>
        public static int SqlInsert(string Sql, ref object Conn)
        {
            string ConnType = Data.ConnectionType(Conn.GetType());
            dynamic Cmd = null;

            try
            {
                if (ConnType == "SqlConnection")
                {
                    Cmd = new SqlCommand(Data.SqlConvert(ConnType, Sql), (SqlConnection)Conn);

                    Cmd.ExecuteNonQuery();

                    Cmd.CommandText = "Select Scope_Identity()";
                }
                else if (ConnType == "MySqlConnection")
                {
                    Cmd = new MySqlCommand(Data.SqlConvert(ConnType, Sql), (MySqlConnection)Conn);

                    Cmd.ExecuteNonQuery();

                    Cmd.CommandText = "Select Last_Insert_Id()";
                }

                object Scalar = Cmd.ExecuteScalar();

                if (Common.IsNothing(Scalar) == true)
                {
                    return 0;
                }
                else
                {
                    return Scalar.TypeInt();
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                if (Common.IsNothing(Cmd) == false)
                {
                    Cmd.Dispose();
                }
            }

            return 0;
        }


        /// <summary>
        /// 执行 SQL 查询返回 Scalar
        /// </summary>
        public static int SqlScalar(string Sql, ref object Conn)
        {
            string ConnType = Data.ConnectionType(Conn.GetType());
            dynamic Cmd = null;

            try
            {
                if (ConnType == "SqlConnection")
                {
                    Cmd = new SqlCommand(Data.SqlConvert(ConnType, Sql), (SqlConnection)Conn);
                }
                else if (ConnType == "MySqlConnection")
                {
                    Cmd = new MySqlCommand(Data.SqlConvert(ConnType, Sql), (MySqlConnection)Conn);
                }

                object Scalar = Cmd.ExecuteScalar();

                if (Common.IsNothing(Scalar) == true)
                {
                    return 0;
                }
                else
                {
                    return Scalar.TypeInt();
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                if (Common.IsNothing(Cmd) == false)
                {
                    Cmd.Dispose();
                }
            }

            return 0;
        }


        /// <summary>
        /// 读取数据记录返回 Hashtable
        /// </summary>
        public static void SqlDataToTable(string Sql, ref object Conn, ref Hashtable Hashtable)
        {
            string ConnType = Data.ConnectionType(Conn.GetType());
            dynamic Cmd = null;

            try
            {
                if (ConnType == "SqlConnection")
                {
                    Cmd = new SqlCommand(Data.SqlConvert(ConnType, Sql), (SqlConnection)Conn);
                }
                else if (ConnType == "MySqlConnection")
                {
                    Cmd = new MySqlCommand(Data.SqlConvert(ConnType, Sql), (MySqlConnection)Conn);
                }

                using (var Reader = Cmd.ExecuteReader())
                {
                    if (Reader.Read() == true)
                    {
                        Hashtable.Add("Exists", true);

                        string Fields = Data.SelectField(Sql);

                        if (Fields == "*")
                        {
                            for (int i = 0; i <= Reader.FieldCount - 1; i++)
                            {
                                string Field = Reader.GetName(i).Substring(Reader.GetName(i).IndexOf(".") + 1);
                                string Value = Reader.GetValue(Reader.GetOrdinal(Field)).ToString();

                                Hashtable.Add(Field, Value == "null" ? "" : Value);
                            }
                        }
                        else
                        {
                            string[] Items = Fields.Split(',');

                            for (int i = 0; i < Items.Length; i++)
                            {
                                string Field = Items[i].Substring(Items[i].IndexOf(".") + 1);
                                string Value = Reader.GetValue(Reader.GetOrdinal(Field.Trim())).ToString();

                                Hashtable.Add(Field.Trim(), Value == "null" ? "" : Value);
                            }
                        }
                    }
                    else
                    {
                        Hashtable.Add("Exists", false);
                    }
                }
            }
            catch (Exception)
            {
                Hashtable["Exists"] = false;
            }
            finally
            {
                if (Common.IsNothing(Cmd) == false)
                {
                    Cmd.Dispose();
                }
            }
        }


        /// <summary>
        /// 读取数据记录返回 JSON 格式数据
        /// </summary>
        public static string SqlDataToJson(string Sql, ref object Conn)
        {
            ArrayList Json = new ArrayList();
            string ConnType = Data.ConnectionType(Conn.GetType());
            dynamic Cmd = null;

            try
            {
                if (ConnType == "SqlConnection")
                {
                    Cmd = new SqlCommand(Data.SqlConvert(ConnType, Sql), (SqlConnection)Conn);
                }
                else if (ConnType == "MySqlConnection")
                {
                    Cmd = new MySqlCommand(Data.SqlConvert(ConnType, Sql), (MySqlConnection)Conn);
                }

                using (var Reader = Cmd.ExecuteReader())
                {
                    if (Reader.Read() == true)
                    {
                        for (int i = 0; i <= Reader.FieldCount - 1; i++)
                        {
                            string Field = Reader.GetName(i).Substring(Reader.GetName(i).IndexOf(".") + 1);
                            string Value = Reader.GetValue(Reader.GetOrdinal(Field)).ToString();

                            Json.Add("'" + Field.ToLower() + "':'" + (Value == "null" ? "" : Common.JsonEscape(Value)) + "'");
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                if (Common.IsNothing(Cmd) == false)
                {
                    Cmd.Dispose();
                }
            }

            return "{" + string.Join(",", Json.ToArray()) + "}";
        }


        /// <summary>
        /// 读取列表数据返回 ArrayList
        /// </summary>
        public static void SqlListToArray(string Field, string Sql, ref object Conn, ref ArrayList List)
        {
            string ConnType = Data.ConnectionType(Conn.GetType());
            dynamic Cmd = null;

            try
            {
                if (ConnType == "SqlConnection")
                {
                    Cmd = new SqlCommand(Data.SqlConvert(ConnType, Sql), (SqlConnection)Conn);
                }
                else if (ConnType == "MySqlConnection")
                {
                    Cmd = new MySqlCommand(Data.SqlConvert(ConnType, Sql), (MySqlConnection)Conn);
                }

                using (var Reader = Cmd.ExecuteReader())
                {
                    while (Reader.Read() == true)
                    {
                        List.Add(Reader.GetValue(Reader.GetOrdinal(Field)).ToString());
                    }
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                if (Common.IsNothing(Cmd) == false)
                {
                    Cmd.Dispose();
                }
            }
        }


        /// <summary>
        /// 读取列表数据返回 List<Hashtable>
        /// </summary>
        public static void SqlListToTable(string Sql, ref object Conn, ref List<Hashtable> DataList)
        {
            string ConnType = Data.ConnectionType(Conn.GetType());
            dynamic Cmd = null;

            try
            {
                string Fields = Data.SelectField(Sql);

                if (ConnType == "SqlConnection")
                {
                    Cmd = new SqlCommand(Data.SqlConvert(ConnType, Sql), (SqlConnection)Conn);
                }
                else if (ConnType == "MySqlConnection")
                {
                    Cmd = new MySqlCommand(Data.SqlConvert(ConnType, Sql), (MySqlConnection)Conn);
                }

                using (var Reader = Cmd.ExecuteReader())
                {
                    while (Reader.Read() == true)
                    {
                        Hashtable ItemTable = new Hashtable();

                        if (Fields == "*")
                        {
                            for (int i = 0; i <= Reader.FieldCount - 1; i++)
                            {
                                string Field = Reader.GetName(i).Substring(Reader.GetName(i).IndexOf(".") + 1);
                                string Value = Reader.GetValue(Reader.GetOrdinal(Field)).ToString();

                                ItemTable.Add(Field, Value == "null" ? "" : Value);
                            }
                        }
                        else
                        {
                            string[] Items = Fields.Split(',');

                            for (int i = 0; i < Items.Length; i++)
                            {
                                string Field = Items[i].Substring(Items[i].IndexOf(".") + 1);
                                string Value = Reader.GetValue(Reader.GetOrdinal(Field.Trim())).ToString();

                                ItemTable.Add(Field.Trim(), Value == "null" ? "" : Value);
                            }
                        }

                        DataList.Add(ItemTable);
                    }
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                if (Common.IsNothing(Cmd) == false)
                {
                    Cmd.Dispose();
                }
            }
        }


        /// <summary>
        /// 读取列表数据返回 JSON 格式数据
        /// </summary>
        public static string SqlListToJson(string Sql, ref object Conn)
        {
            ArrayList Json = new ArrayList();
            string ConnType = Data.ConnectionType(Conn.GetType());
            dynamic Cmd = null;

            try
            {
                string Fields = Data.SelectField(Sql);

                if (ConnType == "SqlConnection")
                {
                    Cmd = new SqlCommand(Data.SqlConvert(ConnType, Sql), (SqlConnection)Conn);
                }
                else if (ConnType == "MySqlConnection")
                {
                    Cmd = new MySqlCommand(Data.SqlConvert(ConnType, Sql), (MySqlConnection)Conn);
                }

                using (var Reader = Cmd.ExecuteReader())
                {
                    while (Reader.Read() == true)
                    {
                        ArrayList JsonField = new ArrayList();

                        if (Fields == "*")
                        {
                            for (int i = 0; i <= Reader.FieldCount - 1; i++)
                            {
                                string Field = Reader.GetName(i).Substring(Reader.GetName(i).IndexOf(".") + 1);
                                string Value = Reader.GetValue(Reader.GetOrdinal(Field)).ToString();

                                JsonField.Add("'" + Field.ToLower() + "':'" + (Value == "null" ? "" : Common.JsonEscape(Value)) + "'");
                            }
                        }
                        else
                        {
                            string[] Items = Fields.Split(',');

                            for (int i = 0; i < Items.Length; i++)
                            {
                                string Field = Items[i].Substring(Items[i].IndexOf(".") + 1);
                                string Value = Reader.GetValue(Reader.GetOrdinal(Field.Trim())).ToString();

                                JsonField.Add("'" + Field.ToLower().Trim() + "':'" + (Value == "null" ? "" : Common.JsonEscape(Value)) + "'");
                            }
                        }

                        Json.Add("{" + string.Join(",", JsonField.ToArray()) + "}");
                    }
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                if (Common.IsNothing(Cmd) == false)
                {
                    Cmd.Dispose();
                }
            }

            return "[" + string.Join(",", Json.ToArray()) + "]";
        }


        /// <summary>
        /// 读取数据分页返回 List<Hashtable>
        /// </summary>
        public static void SqlPageToTable(string Table, string ReadFields, string SortFields, string Query, int Quantity, int Page, ref object Conn, ref List<Hashtable> DataList)
        {
            string ConnType = Data.ConnectionType(Conn.GetType());
            dynamic Cmd = null;

            try
            {
                string[] SortItems = { };
                string[] ReadItems = { };
                string OrderFields = "";
                string Sql = "";

                if (ConnType == "SqlConnection")
                {
                    SortItems = SortFields.Split(',');

                    ReadItems = ReadFields.Split(',');

                    for (int i = 0; i < SortItems.Length; i++)
                    {
                        if (Page == 1)
                        {
                            OrderFields += "" + SortItems[i].Trim() + ", ";
                        }
                        else
                        {
                            OrderFields += "RowQuery." + SortItems[i].Trim() + ", ";
                        }
                    }

                    OrderFields = OrderFields.Substring(0, OrderFields.Length - 2);

                    if (Page > 1)
                    {
                        Query = Query.Replace("" + Table + ".", "RowQuery.");
                    }

                    if (Page == 1)
                    {
                        Sql = "Select Top " + Quantity + " " + ReadFields + " From " + Table + " Where " + Query + " Order By " + OrderFields + "";
                    }
                    else
                    {
                        Sql = "Select " + ReadFields + " From (" +
                              "Select " + ReadFields + ", Row_Number() Over(" +
                              "Order By " + OrderFields + "" +
                              ") As RowNumber From " + Table + " As RowQuery Where " + Query + "" +
                              ") As RowRange Where RowNumber Between " + (((Page - 1) * Quantity) + 1) + " And " + (Page * Quantity) + "";
                    }

                    Cmd = new SqlCommand(Data.SqlConvert(ConnType, Sql), (SqlConnection)Conn);
                }
                else if (ConnType == "MySqlConnection")
                {
                    SortItems = SortFields.Split(',');

                    ReadItems = ReadFields.Split(',');

                    for (int i = 0; i < SortItems.Length; i++)
                    {
                        OrderFields += "" + SortItems[i].Trim() + ", ";
                    }

                    OrderFields = OrderFields.Substring(0, OrderFields.Length - 2);

                    if (Page == 1)
                    {
                        Sql = "Select " + ReadFields + " From " + Table + " Where " + Query + " Order By " + OrderFields + " Limit " + Quantity + "";
                    }
                    else
                    {
                        Sql = "Select " + ReadFields + " From " + Table + " Where Exists (" +
                              "Select " + ReadItems[0].Trim() + " From (" +
                              "Select " + ReadItems[0].Trim() + " From " + Table + " Where " + Query + " Order By " + OrderFields + " Limit " + ((Page - 1) * Quantity) + ", " + Quantity + "" +
                              ") As T Where " + ReadItems[0].Trim() + " = " + Table + "." + ReadItems[0].Trim() + "" +
                              ") Order By " + OrderFields + "";
                    }

                    Cmd = new MySqlCommand(Data.SqlConvert(ConnType, Sql), (MySqlConnection)Conn);
                }

                using (var Reader = Cmd.ExecuteReader())
                {
                    while (Reader.Read() == true)
                    {
                        Hashtable ItemTable = new Hashtable();

                        for (int i = 0; i < ReadItems.Length; i++)
                        {
                            string Field = ReadItems[i].Substring(ReadItems[i].IndexOf(".") + 1);
                            string Value = Reader.GetValue(Reader.GetOrdinal(Field.Trim())).ToString();

                            ItemTable.Add(Field.Trim(), Value == "null" ? "" : Value);
                        }

                        DataList.Add(ItemTable);
                    }
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                if (Common.IsNothing(Cmd) == false)
                {
                    Cmd.Dispose();
                }
            }
        }


        /// <summary>
        /// 读取数据分页返回 JSON 格式数据(通用)
        /// </summary>
        public static string SqlPageToJson(string Table, string ReadFields, string SortFields, string Query, int Quantity, int Page, ref object Conn)
        {
            ArrayList Json = new ArrayList();
            string ConnType = Data.ConnectionType(Conn.GetType());
            dynamic Cmd = null;

            try
            {
                string[] ReadItems = null;
                string[] SortItems = null;
                string OrderFields = "";
                string Sql = "";

                if (ConnType == "SqlConnection")
                {
                    SortItems = SortFields.Split(',');

                    ReadItems = ReadFields.Split(',');

                    for (int i = 0; i < SortItems.Length; i++)
                    {
                        if (Page == 1)
                        {
                            OrderFields += "" + SortItems[i].Trim() + ", ";
                        }
                        else
                        {
                            OrderFields += "RowQuery." + SortItems[i].Trim() + ", ";
                        }
                    }

                    OrderFields = OrderFields.Substring(0, OrderFields.Length - 2);

                    if (Page > 1)
                    {
                        Query = Query.Replace("" + Table + ".", "RowQuery.");
                    }

                    if (Page == 1)
                    {
                        Sql = "Select Top " + Quantity + " " + ReadFields + " From " + Table + " Where " + Query + " Order By " + OrderFields + "";
                    }
                    else
                    {
                        Sql = "Select " + ReadFields + " From (" +
                              "Select " + ReadFields + ", Row_Number() Over(" +
                              "Order By " + OrderFields + "" +
                              ") As RowNumber From " + Table + " As RowQuery Where " + Query + "" +
                              ") As RowRange Where RowNumber Between " + (((Page - 1) * Quantity) + 1) + " And " + (Page * Quantity) + "";
                    }

                    Cmd = new SqlCommand(Data.SqlConvert(ConnType, Sql), (SqlConnection)Conn);
                }
                else if (ConnType == "MySqlConnection")
                {
                    SortItems = SortFields.Split(',');

                    ReadItems = ReadFields.Split(',');

                    for (int i = 0; i < SortItems.Length; i++)
                    {
                        OrderFields += "" + SortItems[i].Trim() + ", ";
                    }

                    OrderFields = OrderFields.Substring(0, OrderFields.Length - 2);

                    if (Page == 1)
                    {
                        Sql = "Select " + ReadFields + " From " + Table + " Where " + Query + " Order By " + OrderFields + " Limit " + Quantity + "";
                    }
                    else
                    {
                        Sql = "Select " + ReadFields + " From " + Table + " Where Exists (" +
                              "Select " + ReadItems[0].Trim() + " From (" +
                              "Select " + ReadItems[0].Trim() + " From " + Table + " Where " + Query + " Order By " + OrderFields + " Limit " + ((Page - 1) * Quantity) + ", " + Quantity + "" +
                              ") As T Where " + ReadItems[0].Trim() + " = " + Table + "." + ReadItems[0].Trim() + "" +
                              ") Order By " + OrderFields + "";
                    }

                    Cmd = new MySqlCommand(Data.SqlConvert(ConnType, Sql), (MySqlConnection)Conn);
                }

                using (var Reader = Cmd.ExecuteReader())
                {
                    while (Reader.Read() == true)
                    {
                        ArrayList JsonField = new ArrayList();

                        for (int i = 0; i < ReadItems.Length; i++)
                        {
                            string Field = ReadItems[i].Substring(ReadItems[i].IndexOf(".") + 1);
                            string Value = Reader.GetValue(Reader.GetOrdinal(Field.Trim())).ToString();

                            JsonField.Add("'" + Field.ToLower().Trim() + "':'" + (Value == "null" ? "" : Common.JsonEscape(Value)) + "'");
                        }

                        Json.Add("{" + string.Join(",", JsonField.ToArray()) + "}");
                    }
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                if (Common.IsNothing(Cmd) == false)
                {
                    Cmd.Dispose();
                }
            }

            return "[" + string.Join(",", Json.ToArray()) + "]";
        }


        /// <summary>
        /// 读取数据分页返回 JSON 格式数据(带 Join 语句)
        /// </summary>
        public static string SqlPageToJson(string Table, string ReadFields, string SortFields, string Join, string Query, int Quantity, int Page, ref object Conn)
        {
            ArrayList Json = new ArrayList();
            string ConnType = Data.ConnectionType(Conn.GetType());
            dynamic Cmd = null;

            try
            {
                string[] ReadItems = null;
                string[] SortItems = null;
                string OrderFields = "";
                string Sql = "";

                if (ConnType == "SqlConnection")
                {
                    SortItems = SortFields.Split(',');

                    ReadItems = ReadFields.Split(',');

                    for (int i = 0; i < SortItems.Length; i++)
                    {
                        if (Page == 1)
                        {
                            OrderFields += "" + SortItems[i].Trim() + ", ";
                        }
                        else
                        {
                            OrderFields += "RowQuery." + SortItems[i].Trim() + ", ";
                        }
                    }

                    OrderFields = OrderFields.Substring(0, OrderFields.Length - 2);

                    if (Page > 1)
                    {
                        Join = Join.Replace("" + Table + ".", "RowQuery.");
                        Query = Query.Replace("" + Table + ".", "RowQuery.");
                    }

                    if (Page == 1)
                    {
                        Sql = "Select Top " + Quantity + " " + ReadFields + " From " + Table + " " + Join + " Where " + Query + " Order By " + OrderFields + "";
                    }
                    else
                    {
                        Sql = "Select " + ReadFields + " From (" +
                              "Select " + ReadFields + ", Row_Number() Over(" +
                              "Order By " + OrderFields + "" +
                              ") As RowNumber From " + Table + " As RowQuery " + Join + " Where " + Query + "" +
                              ") As RowRange Where RowNumber Between " + (((Page - 1) * Quantity) + 1) + " And " + (Page * Quantity) + "";
                    }

                    Cmd = new SqlCommand(Sql, (SqlConnection)Conn);
                }
                else if (ConnType == "MySqlConnection")
                {
                    SortItems = SortFields.Split(',');

                    ReadItems = ReadFields.Split(',');

                    for (int i = 0; i < SortItems.Length; i++)
                    {
                        OrderFields += "" + SortItems[i].Trim() + ", ";
                    }

                    OrderFields = OrderFields.Substring(0, OrderFields.Length - 2);

                    if (Page == 1)
                    {
                        Sql = "Select " + ReadFields + " From " + Table + " " + Join + " Where " + Query + " Order By " + OrderFields + " Limit " + Quantity + "";
                    }
                    else
                    {
                        Sql = "Select " + ReadFields + " From " + Table + " " + Join + " Where Exists (" +
                              "Select " + ReadItems[0].Trim() + " From (" +
                              "Select " + ReadItems[0].Trim() + " From " + Table + " Where " + Query + " Order By " + OrderFields + " Limit " + ((Page - 1) * Quantity) + ", " + Quantity + "" +
                              ") As T Where " + ReadItems[0].Trim() + " = " + Table + "." + ReadItems[0].Trim() + "" +
                              ") Order By " + OrderFields + "";
                    }

                    Cmd = new MySqlCommand(Sql, (MySqlConnection)Conn);
                }

                using (var Reader = Cmd.ExecuteReader())
                {
                    while (Reader.Read() == true)
                    {
                        ArrayList JsonField = new ArrayList();

                        for (int i = 0; i < ReadItems.Length; i++)
                        {
                            string Field = ReadItems[i].Substring(ReadItems[i].IndexOf(".") + 1);
                            string Value = Reader.GetValue(Reader.GetOrdinal(Field.Trim())).ToString();

                            JsonField.Add("'" + Field.ToLower().Trim() + "':'" + (Value == "null" ? "" : Common.JsonEscape(Value)) + "'");
                        }

                        Json.Add("{" + string.Join(",", JsonField.ToArray()) + "}");
                    }
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                if (Common.IsNothing(Cmd) == false)
                {
                    Cmd.Dispose();
                }
            }

            return "[" + string.Join(",", Json.ToArray()) + "]";
        }


        /// <summary>
        /// 获取数据库连接类型
        /// </summary>
        public static string ConnectionType(object Type)
        {
            if (Type.TypeString() == "System.Data.SqlClient.SqlConnection")
            {
                return "SqlConnection";
            }
            else if (Type.TypeString() == "MySql.Data.MySqlClient.MySqlConnection")
            {
                return "MySqlConnection";
            }

            return "";
        }


        /// <summary>
        /// 获取选择字段
        /// </summary>
        public static string SelectField(string Sql)
        {
            Match Match = Regex.Match(Sql, @"Select\s+(?:Top\s+\(?[\d]+\)?\s+)?([\s\w\*\.\,]+)\s+From\s+", RegexOptions.IgnoreCase);

            return Match.Groups[1].Value;
        }


        /// <summary>
        /// SQL 语句转换(SQL Server 与 MySQL 互换)
        /// </summary>
        public static string SqlConvert(string Type, string Sql)
        {
            if (Type == "SqlConnection")
            {
                Match Match = Regex.Match(Sql, @"\sLimit\s+([\d]+)", RegexOptions.IgnoreCase);

                if (string.IsNullOrEmpty(Match.Groups[0].Value) == false)
                {
                    Sql = Regex.Replace(Sql.Replace(Match.Groups[0].Value, ""), "^Select ", "Select Top " + Match.Groups[1].Value + " ", RegexOptions.IgnoreCase);
                }

                Sql = Regex.Replace(Sql, @"\sLength\(([\w]+)\)\s", " Len($1) ", RegexOptions.IgnoreCase);
            }
            else if (Type == "MySqlConnection")
            {
                Match Match = Regex.Match(Sql, @"\sTop\s+\(?([\d]+)\)?", RegexOptions.IgnoreCase);

                if (string.IsNullOrEmpty(Match.Groups[0].Value) == false)
                {
                    Sql = Sql.Replace(Match.Groups[0].Value, "") + " Limit " + Match.Groups[1].Value;
                }

                Sql = Regex.Replace(Sql, @"\sLen\(([\w]+)\)\s", " Length($1) ", RegexOptions.IgnoreCase);
            }

            Sql = Sql.Replace("NULL", "'null'");
            Sql = Sql.Replace("''", "'null'");

            return Sql;
        }


    }


}
