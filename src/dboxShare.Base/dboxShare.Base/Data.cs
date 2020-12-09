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

            return default(dynamic);
        }


        /// <summary>
        /// 执行sql查询
        /// </summary>
        public static void SqlQuery(string Sql, ref object Conn)
        {
            string ConnType = Data.ConnectionType(Conn.GetType());
            dynamic Cmd = default(dynamic);

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

                Cmd.Dispose();
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
        /// 执行sql插入记录返回自动编号
        /// </summary>
        public static int SqlInsert(string Sql, ref object Conn)
        {
            string ConnType = Data.ConnectionType(Conn.GetType());
            dynamic Cmd = default(dynamic);
            object Scalar = null;

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

                Scalar = Cmd.ExecuteScalar();

                Cmd.Dispose();

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
        /// 执行sql查询返回scalar
        /// </summary>
        public static int SqlScalar(string Sql, ref object Conn)
        {
            string ConnType = Data.ConnectionType(Conn.GetType());
            dynamic Cmd = default(dynamic);
            object Scalar = null;

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

                Scalar = Cmd.ExecuteScalar();

                Cmd.Dispose();

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
        /// 读取数据记录返回hashtable
        /// </summary>
        public static void SqlDataToTable(string Sql, ref object Conn, ref Hashtable Hashtable)
        {
            string ConnType = Data.ConnectionType(Conn.GetType());
            dynamic Cmd = default(dynamic);
            dynamic Reader = default(dynamic);
            string Field = "";
            string Fields = "";
            string[] Items = {};
            string Value = "";
            int Index = 0;

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

                Reader = Cmd.ExecuteReader();

                if (Reader.Read() == true)
                {
                    Hashtable.Add("Exist", true);

                    Fields = Data.SelectField(Sql);

                    if (Fields == "*")
                    {
                        for (Index = 0; Index <= Reader.FieldCount - 1; Index++)
                        {
                            Field = Reader.GetName(Index).Substring(Reader.GetName(Index).IndexOf(".") + 1);
                            Value = Reader.GetValue(Reader.GetOrdinal(Field)).ToString();

                            Hashtable.Add(Field, Value == "null" ? "" : Value);
                        }
                    }
                    else
                    {
                        Items = Fields.Split(',');

                        for (Index = 0; Index < Items.Length; Index++)
                        {
                            Field = Items[Index].Substring(Items[Index].IndexOf(".") + 1);
                            Value = Reader.GetValue(Reader.GetOrdinal(Field.Trim())).ToString();

                            Hashtable.Add(Field.Trim(), Value == "null" ? "" : Value);
                        }
                    }
                }
                else
                {
                    Hashtable.Add("Exist", false);
                }

                Reader.Close();

                Reader.Dispose();

                Cmd.Dispose();
            }
            catch (Exception)
            {
                if (Hashtable["Exist"] == null)
                {
                    Hashtable.Add("Exist", false);
                }
                else
                {
                    Hashtable["Exist"] = false;
                }
            }
            finally
            {
                if (Common.IsNothing(Reader) == false)
                {
                    Reader.Close();
                    Reader.Dispose();
                }

                if (Common.IsNothing(Cmd) == false)
                {
                    Cmd.Dispose();
                }
            }
        }


        /// <summary>
        /// 读取数据记录返回json格式数据
        /// </summary>
        public static string SqlDataToJson(string Sql, ref object Conn)
        {
            ArrayList JsonList = new ArrayList();
            string ConnType = Data.ConnectionType(Conn.GetType());
            dynamic Cmd = default(dynamic);
            dynamic Reader = default(dynamic);
            string Field = "";
            string Value = "";
            int Index = 0;

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

                Reader = Cmd.ExecuteReader();

                if (Reader.Read() == true)
                {
                    for (Index = 0; Index <= Reader.FieldCount - 1; Index++)
                    {
                        Field = Reader.GetName(Index).Substring(Reader.GetName(Index).IndexOf(".") + 1);
                        Value = Reader.GetValue(Reader.GetOrdinal(Field)).ToString();

                        JsonList.Add("'" + Field.ToLower() + "':'" + (Value == "null" ? "" : Common.JsonEscape(Value)) + "'");
                    }
                }

                Reader.Close();

                Reader.Dispose();

                Cmd.Dispose();

                return "{" + string.Join(",", JsonList.ToArray()) + "}";
            }
            catch (Exception)
            {

            }
            finally
            {
                if (Common.IsNothing(Reader) == false)
                {
                    Reader.Close();
                    Reader.Dispose();
                }

                if (Common.IsNothing(Cmd) == false)
                {
                    Cmd.Dispose();
                }
            }

            return "";
        }


        /// <summary>
        /// 读取列表数据返回arraylist
        /// </summary>
        public static void SqlListToArray(string Field, string Sql, ref object Conn, ref ArrayList List)
        {
            string ConnType = Data.ConnectionType(Conn.GetType());
            dynamic Cmd = default(dynamic);
            dynamic Reader = default(dynamic);

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

                Reader = Cmd.ExecuteReader();

                while (Reader.Read() == true)
                {
                    List.Add(Reader.GetValue(Reader.GetOrdinal(Field)).ToString());
                }

                Reader.Close();

                Reader.Dispose();

                Cmd.Dispose();
            }
            catch (Exception)
            {

            }
            finally
            {
                if (Common.IsNothing(Reader) == false)
                {
                    Reader.Close();
                    Reader.Dispose();
                }

                if (Common.IsNothing(Cmd) == false)
                {
                    Cmd.Dispose();
                }
            }
        }


        /// <summary>
        /// 读取列表数据返回list<hashtable>
        /// </summary>
        public static void SqlListToTable(string Sql, ref object Conn, ref List<Hashtable> DataList)
        {
            string ConnType = Data.ConnectionType(Conn.GetType());
            dynamic Cmd = default(dynamic);
            dynamic Reader = default(dynamic);
            string Field = "";
            string Fields = "";
            string[] Items = {};
            string Value = "";
            int Index = 0;

            try
            {
                Fields = Data.SelectField(Sql);

                if (ConnType == "SqlConnection")
                {
                    Cmd = new SqlCommand(Data.SqlConvert(ConnType, Sql), (SqlConnection)Conn);
                }
                else if (ConnType == "MySqlConnection")
                {
                    Cmd = new MySqlCommand(Data.SqlConvert(ConnType, Sql), (MySqlConnection)Conn);
                }

                Reader = Cmd.ExecuteReader();

                while (Reader.Read() == true)
                {
                    Hashtable ItemTable = new Hashtable();

                    if (Fields == "*")
                    {
                        for (Index = 0; Index <= Reader.FieldCount - 1; Index++)
                        {
                            Field = Reader.GetName(Index).Substring(Reader.GetName(Index).IndexOf(".") + 1);
                            Value = Reader.GetValue(Reader.GetOrdinal(Field)).ToString();

                            ItemTable.Add(Field, Value == "null" ? "" : Value);
                        }
                    }
                    else
                    {
                        Items = Fields.Split(',');

                        for (Index = 0; Index < Items.Length; Index++)
                        {
                            Field = Items[Index].Substring(Items[Index].IndexOf(".") + 1);
                            Value = Reader.GetValue(Reader.GetOrdinal(Field.Trim())).ToString();

                            ItemTable.Add(Field.Trim(), Value == "null" ? "" : Value);
                        }
                    }

                    DataList.Add(ItemTable);
                }

                Reader.Close();

                Reader.Dispose();

                Cmd.Dispose();
            }
            catch (Exception)
            {

            }
            finally
            {
                if (Common.IsNothing(Reader) == false)
                {
                    Reader.Close();
                    Reader.Dispose();
                }

                if (Common.IsNothing(Cmd) == false)
                {
                    Cmd.Dispose();
                }
            }
        }


        /// <summary>
        /// 读取列表数据返回json格式数据
        /// </summary>
        public static string SqlListToJson(string Sql, ref object Conn)
        {
            ArrayList JsonList = new ArrayList();
            string ConnType = Data.ConnectionType(Conn.GetType());
            dynamic Cmd = default(dynamic);
            dynamic Reader = default(dynamic);
            string Field = "";
            string Fields = "";
            string[] Items = {};
            string Value = "";
            int Index = 0;

            try
            {
                Fields = Data.SelectField(Sql);

                if (ConnType == "SqlConnection")
                {
                    Cmd = new SqlCommand(Data.SqlConvert(ConnType, Sql), (SqlConnection)Conn);
                }
                else if (ConnType == "MySqlConnection")
                {
                    Cmd = new MySqlCommand(Data.SqlConvert(ConnType, Sql), (MySqlConnection)Conn);
                }

                Reader = Cmd.ExecuteReader();

                while (Reader.Read() == true)
                {
                    ArrayList JsonField = new ArrayList();

                    if (Fields == "*")
                    {
                        for (Index = 0; Index <= Reader.FieldCount - 1; Index++)
                        {
                            Field = Reader.GetName(Index).Substring(Reader.GetName(Index).IndexOf(".") + 1);
                            Value = Reader.GetValue(Reader.GetOrdinal(Field)).ToString();

                            JsonField.Add("'" + Field.ToLower() + "':'" + (Value == "null" ? "" : Common.JsonEscape(Value)) + "'");
                        }
                    }
                    else
                    {
                        Items = Fields.Split(',');

                        for (Index = 0; Index < Items.Length; Index++)
                        {
                            Field = Items[Index].Substring(Items[Index].IndexOf(".") + 1);
                            Value = Reader.GetValue(Reader.GetOrdinal(Field.Trim())).ToString();

                            JsonField.Add("'" + Field.ToLower().Trim() + "':'" + (Value == "null" ? "" : Common.JsonEscape(Value)) + "'");
                        }
                    }

                    JsonList.Add("{" + string.Join(",", JsonField.ToArray()) + "}");
                }

                Reader.Close();

                Reader.Dispose();

                Cmd.Dispose();

                return "[" + string.Join(",", JsonList.ToArray()) + "]";
            }
            catch (Exception)
            {

            }
            finally
            {
                if (Common.IsNothing(Reader) == false)
                {
                    Reader.Close();
                    Reader.Dispose();
                }

                if (Common.IsNothing(Cmd) == false)
                {
                    Cmd.Dispose();
                }
            }

            return "";
        }


        /// <summary>
        /// 读取数据分页返回list<hashtable>
        /// </summary>
        public static void SqlPageToTable(string Table, string ReadFields, string SortFields, string Query, int Quantity, int Page, ref object Conn, ref List<Hashtable> DataList)
        {
            string ConnType = Data.ConnectionType(Conn.GetType());
            dynamic Cmd = default(dynamic);
            dynamic Reader = default(dynamic);
            string[] ReadItems = null;
            string[] SortItems = null;
            string OrderFields = "";
            string Field = "";
            string Value = "";
            string Sql = "";
            int Index = 0;

            try
            {
                if (ConnType == "SqlConnection")
                {
                    SortItems = SortFields.Split(',');

                    ReadItems = ReadFields.Split(',');

                    for (Index = 0; Index < SortItems.Length; Index++)
                    {
                        if (Page == 1)
                        {
                            OrderFields += "" + SortItems[Index].Trim() + ", ";
                        }
                        else
                        {
                            OrderFields += "RowQuery." + SortItems[Index].Trim() + ", ";
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

                    for (Index = 0; Index < SortItems.Length; Index++)
                    {
                        OrderFields += "" + SortItems[Index].Trim() + ", ";
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

                Reader = Cmd.ExecuteReader();

                while (Reader.Read() == true)
                {
                    Hashtable ItemTable = new Hashtable();

                    for (Index = 0; Index < ReadItems.Length; Index++)
                    {
                        Field = ReadItems[Index].Substring(ReadItems[Index].IndexOf(".") + 1);
                        Value = Reader.GetValue(Reader.GetOrdinal(Field.Trim())).ToString();

                        ItemTable.Add(Field.Trim(), Value == "null" ? "" : Value);
                    }

                    DataList.Add(ItemTable);
                }

                Reader.Close();

                Reader.Dispose();

                Cmd.Dispose();
            }
            catch (Exception)
            {

            }
            finally
            {
                if (Common.IsNothing(Reader) == false)
                {
                    Reader.Close();
                    Reader.Dispose();
                }

                if (Common.IsNothing(Cmd) == false)
                {
                    Cmd.Dispose();
                }
            }
        }


        /// <summary>
        /// 读取数据分页返回json格式数据(通用)
        /// </summary>
        public static string SqlPageToJson(string Table, string ReadFields, string SortFields, string Query, int Quantity, int Page, ref object Conn)
        {
            ArrayList JsonList = new ArrayList();
            string ConnType = Data.ConnectionType(Conn.GetType());
            dynamic Cmd = default(dynamic);
            dynamic Reader = default(dynamic);
            string[] ReadItems = null;
            string[] SortItems = null;
            string OrderFields = "";
            string Field = "";
            string Value = "";
            string Sql = "";
            int Index = 0;

            try
            {
                if (ConnType == "SqlConnection")
                {
                    SortItems = SortFields.Split(',');

                    ReadItems = ReadFields.Split(',');

                    for (Index = 0; Index < SortItems.Length; Index++)
                    {
                        if (Page == 1)
                        {
                            OrderFields += "" + SortItems[Index].Trim() + ", ";
                        }
                        else
                        {
                            OrderFields += "RowQuery." + SortItems[Index].Trim() + ", ";
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

                    for (Index = 0; Index < SortItems.Length; Index++)
                    {
                        OrderFields += "" + SortItems[Index].Trim() + ", ";
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

                Reader = Cmd.ExecuteReader();

                while (Reader.Read() == true)
                {
                    ArrayList JsonField = new ArrayList();

                    for (Index = 0; Index < ReadItems.Length; Index++)
                    {
                        Field = ReadItems[Index].Substring(ReadItems[Index].IndexOf(".") + 1);
                        Value = Reader.GetValue(Reader.GetOrdinal(Field.Trim())).ToString();

                        JsonField.Add("'" + Field.ToLower().Trim() + "':'" + (Value == "null" ? "" : Common.JsonEscape(Value)) + "'");
                    }

                    JsonList.Add("{" + string.Join(",", JsonField.ToArray()) + "}");
                }

                Reader.Close();

                Reader.Dispose();

                Cmd.Dispose();

                return "[" + string.Join(",", JsonList.ToArray()) + "]";
            }
            catch (Exception)
            {

            }
            finally
            {
                if (Common.IsNothing(Reader) == false)
                {
                    Reader.Close();
                    Reader.Dispose();
                }

                if (Common.IsNothing(Cmd) == false)
                {
                    Cmd.Dispose();
                }
            }

            return "";
        }


        /// <summary>
        /// 读取数据分页返回json格式数据(带join语句)
        /// </summary>
        public static string SqlPageToJson(string Table, string ReadFields, string SortFields, string Join, string Query, int Quantity, int Page, ref object Conn)
        {
            ArrayList JsonList = new ArrayList();
            string ConnType = Data.ConnectionType(Conn.GetType());
            dynamic Cmd = default(dynamic);
            dynamic Reader = default(dynamic);
            string[] ReadItems = null;
            string[] SortItems = null;
            string OrderFields = "";
            string Field = "";
            string Value = "";
            string Sql = "";
            int Index = 0;

            try
            {
                if (ConnType == "SqlConnection")
                {
                    SortItems = SortFields.Split(',');

                    ReadItems = ReadFields.Split(',');

                    for (Index = 0; Index < SortItems.Length; Index++)
                    {
                        if (Page == 1)
                        {
                            OrderFields += "" + SortItems[Index].Trim() + ", ";
                        }
                        else
                        {
                            OrderFields += "RowQuery." + SortItems[Index].Trim() + ", ";
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

                    for (Index = 0; Index < SortItems.Length; Index++)
                    {
                        OrderFields += "" + SortItems[Index].Trim() + ", ";
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

                Reader = Cmd.ExecuteReader();

                while (Reader.Read() == true)
                {
                    ArrayList JsonField = new ArrayList();

                    for (Index = 0; Index < ReadItems.Length; Index++)
                    {
                        Field = ReadItems[Index].Substring(ReadItems[Index].IndexOf(".") + 1);
                        Value = Reader.GetValue(Reader.GetOrdinal(Field.Trim())).ToString();

                        JsonField.Add("'" + Field.ToLower().Trim() + "':'" + (Value == "null" ? "" : Common.JsonEscape(Value)) + "'");
                    }

                    JsonList.Add("{" + string.Join(",", JsonField.ToArray()) + "}");
                }

                Reader.Close();

                Reader.Dispose();

                Cmd.Dispose();

                return "[" + string.Join(",", JsonList.ToArray()) + "]";
            }
            catch (Exception)
            {

            }
            finally
            {
                if (Common.IsNothing(Reader) == false)
                {
                    Reader.Close();
                    Reader.Dispose();
                }

                if (Common.IsNothing(Cmd) == false)
                {
                    Cmd.Dispose();
                }
            }

            return "";
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
            Match Match = default(Match);

            Match = Regex.Match(Sql, @"Select\s+(?:Top\s+\(?[\d]+\)?\s+)?([\s\w\*\.\,]+)\s+From\s+", RegexOptions.IgnoreCase);

            return Match.Groups[1].Value;
        }


        /// <summary>
        /// sql语句转换(sql server与mysql互换)
        /// </summary>
        public static string SqlConvert(string Type, string Sql)
        {
            Match Match = default(Match);

            if (Type == "SqlConnection")
            {
                Match = Regex.Match(Sql, @"\sLimit\s+([\d]+)", RegexOptions.IgnoreCase);

                if (string.IsNullOrEmpty(Match.Groups[0].Value) == false)
                {
                    Sql = Regex.Replace(Sql.Replace(Match.Groups[0].Value, ""), "^Select ", "Select Top " + Match.Groups[1].Value + " ", RegexOptions.IgnoreCase);
                }

                Sql = Regex.Replace(Sql, @"\sLength\(([\w]+)\)\s", " Len($1) ", RegexOptions.IgnoreCase);
            }
            else if (Type == "MySqlConnection")
            {
                Match = Regex.Match(Sql, @"\sTop\s+\(?([\d]+)\)?", RegexOptions.IgnoreCase);

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
