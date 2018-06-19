using OfficeOpenXml;
using System.Data;
using WebCMS.Models;

namespace WebCMS.Areas.Main.Models
{
    public class MachineValueModel
    {
        public DataTable MachineValue;

        public static DataTable GetNowMachineValue(string vName, string gname, string pid, string ValueStyle)
        {

            short _vid; string _gname = "NULL"; string _pid = "NULL";
            _vid = ListModel.GetVid(vName);

            if (gname.Length > 0) { _gname = "'" + gname + "'"; }
            if (pid.Length > 0)
            {
                if (vName == "DPM")
                {
                    _pid = ListModel.GetDPMPid(pid).ToString();
                }
                else
                {
                    _pid = ListModel.GetPid(pid).ToString();
                }
            }
            if (_pid == "0")
            {
                _pid = "NULL";
            }

            DataSet DeptDS = null;
            if (vName == "DPM")
            {
                DeptDS = DBConnector.executeQuery("Intouch", "EXEC uSP_Select_ValueNow_DPM  @vid=" + _vid + " , @gname  = " + _gname + ",@pid=" + _pid);
            }
            else
            {
                DeptDS = DBConnector.executeQuery("Intouch", "EXEC uSP_Select_ValueNow  @vid=" + _vid + " , @gname  = " + _gname + ",@pid=" + _pid);
            }

            if (DeptDS != null && DeptDS.Tables.Count > 0 && DeptDS.Tables[0].Rows.Count > 0)
            {
                if (_pid == "NULL")
                {
                    if (ValueStyle != string.Empty)
                    {
                        //刪除右(左)側欄位
                        //for (int i = DeptDS.Tables[0].Columns.Count - 1; i >= 0; i--)
                        //{
                        //    if (ValueStyle == "L" && (DeptDS.Tables[0].Columns[i].ColumnName.IndexOf("右側") > 0 || DeptDS.Tables[0].Columns[i].ColumnName.IndexOf("側)") == 0))
                        //    {
                        //        DeptDS.Tables[0].Columns.RemoveAt(i);
                        //    }
                        //    else if (ValueStyle == "R" && (DeptDS.Tables[0].Columns[i].ColumnName.IndexOf("左側") > 0 || DeptDS.Tables[0].Columns[i].ColumnName.IndexOf("側)") == 0))
                        //    {
                        //        DeptDS.Tables[0].Columns.RemoveAt(i);
                        //    }
                        //}

                        //刪除右(左)側記錄
                        //for (int i = DeptDS.Tables[0].Rows.Count - 1; i >= 0; i--)
                        //{

                        //    if (ValueStyle == "L" && DeptDS.Tables[0].Rows[i][4].ToString().IndexOf('R') >= 0)
                        //    {
                        //        DeptDS.Tables[0].Rows.Remove(DeptDS.Tables[0].Rows[i]);
                        //    }
                        //    else if (ValueStyle == "R" && DeptDS.Tables[0].Rows[i][4].ToString().IndexOf("L") >= 0)
                        //    {
                        //        DeptDS.Tables[0].Rows.Remove(DeptDS.Tables[0].Rows[i]);
                        //    }
                        //}
                    }

                    //取得防火回欄位的Index
                    //string FireList = string.Empty;
                    //for (int i = 0; i <= DeptDS.Tables[0].Columns.Count - 1; i++)
                    //{
                    //    if (DeptDS.Tables[0].Columns[i].ColumnName.IndexOf("防回火") == 0)
                    //    {
                    //        FireList += i.ToString() + ",";
                    //    }
                    //}

                    //if (FireList != string.Empty)
                    //{
                    //    FireList = FireList.Substring(0, FireList.Length - 1);
                    //    string[] FireArray;
                    //    FireArray = FireList.Split(',');
                    //    int templength = 0;
                    //    if (FireArray.Length < 4)
                    //    {
                    //        templength = FireArray.Length;
                    //    }
                    //    else
                    //    {
                    //        templength = 4;
                    //    }

                    //    if (string.IsNullOrEmpty(pid))
                    //    {
                    //        for (int i = 0; i <= DeptDS.Tables[0].Rows.Count - 1; i++)
                    //        {
                    //            //暫存單筆所有防回火欄位數據
                    //            int j = 0;
                    //            string[] isNotNullArray = new string[4];
                    //            for (int k = 0; k <= FireArray.Length - 1; k++)
                    //            {
                    //                if (!DeptDS.Tables[0].Rows[i].IsNull(int.Parse(FireArray[k].ToString())))
                    //                {
                    //                    isNotNullArray[j] = DeptDS.Tables[0].Rows[i][int.Parse(FireArray[k].ToString())].ToString();
                    //                    j += 1;
                    //                }
                    //            }



                    //            //回寫4個防回火數據到DataSet
                    //            for (int k = 0; k <= templength - 1; k++)
                    //            {

                    //                if (!string.IsNullOrEmpty(isNotNullArray[k]))
                    //                {
                    //                    DeptDS.Tables[0].Rows[i][int.Parse(FireArray[k + FireArray.Length - templength].ToString())] = double.Parse(isNotNullArray[k]);
                    //                }
                    //                else
                    //                {
                    //                    DeptDS.Tables[0].Rows[i][int.Parse(FireArray[k + FireArray.Length - templength].ToString())] = System.DBNull.Value;
                    //                }
                    //            }
                    //        }

                    //        //保留4個防回火欄位重新命名,其他刪除
                    //        int intFire = templength;
                    //        for (int i = DeptDS.Tables[0].Columns.Count - 1; i >= 0; i--)
                    //        {
                    //            if (DeptDS.Tables[0].Columns[i].ColumnName.IndexOf("防回火") == 0)
                    //            {

                    //                if (intFire > 0)
                    //                {
                    //                    DeptDS.Tables[0].Columns[i].ColumnName = "防回火" + intFire;
                    //                }
                    //                else
                    //                {
                    //                    DeptDS.Tables[0].Columns.RemoveAt(i);
                    //                }
                    //                intFire -= 1;
                    //            }
                    //        }
                    //    }
                    //}

                    //更改抬頭名稱
                    //int indexStr = 0;

                    //for (int i = DeptDS.Tables[0].Columns.Count - 1; i >= 0; i--)
                    //{
                    //    if (ValueStyle == "R")
                    //    {
                    //        indexStr = DeptDS.Tables[0].Columns[i].ColumnName.IndexOf("(右側)");
                    //    }
                    //    else if (ValueStyle == "L")
                    //    {
                    //        indexStr = DeptDS.Tables[0].Columns[i].ColumnName.IndexOf("(左側)");
                    //    }

                    //    if (indexStr > 0)
                    //    {
                    //        DeptDS.Tables[0].Columns[i].ColumnName = DeptDS.Tables[0].Columns[i].ColumnName.Substring(0, indexStr);
                    //    }
                    //}
                }
                //DeptDS.Tables[0].Columns.Remove("location_side");
                DeptDS.Tables[0].Columns.Remove("toolID1");
                DeptDS.Tables[0].Columns.Remove("DataTime");
                return DeptDS.Tables[0];
            }
            else
            {
                return null;
            }
        }

        public static DataTable GetNowMachineValueDASAll(string vName, string gname, string pid, string ValueStyle)
        {
            DataTable dt = null;
            DataTable dt_R = MachineValueModel.GetNowMachineValue(vName, gname, pid, "R");
            DataTable dt_L = MachineValueModel.GetNowMachineValue(vName, gname, pid, "L");
            if (dt_R != null && dt_L != null)
            {
                dt_R.Merge(dt_L, true);
                dt = dt_R;
                DataView dv = new DataView(dt);
                dv.Sort = "gName ASC,toolID ASC";
                dt = dv.ToTable();
                return dt;
            }
            else
            {
                return null;
            }

        }

        public static byte[] GetNowMachineValueFile(string vName, string gname, string pid, string ValueStyle)
        {
            DataTable dt = null;
            if (vName == "DAS" && ValueStyle == string.Empty && pid == string.Empty)
            {
                DataTable dt_R = MachineValueModel.GetNowMachineValue(vName, gname, pid, "R");
                DataTable dt_L = MachineValueModel.GetNowMachineValue(vName, gname, pid, "L");
                if (dt_R != null || dt_L != null)
                {
                    if (dt_R != null && dt_L != null)
                    {
                        dt_R.Merge(dt_L, true);
                        dt = dt_R;
                    }
                    else if (dt_R != null && dt_L == null)
                    {
                        dt = dt_R;
                    }
                    else if (dt_R == null && dt_L != null)
                    {
                        dt = dt_L;
                    }
                    DataView dv = new DataView(dt);
                    dv.Sort = "gName ASC,toolID ASC";
                    dt = dv.ToTable();
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        dt.Columns.Remove("location_side");
                    }
                }
            }
            else
            {
                dt = MachineValueModel.GetNowMachineValue(vName, gname, pid, ValueStyle);
                if (dt != null && dt.Rows.Count > 0)
                {
                    dt.Columns.Remove("location_side");
                }
            }

            ExcelPackage ep = new ExcelPackage();

            if (dt != null && dt.Rows.Count > 0)
            {
                //DataSet ds = null;

                string[,] ds_array = new string[3, 2];


                if (vName == "DAS" && pid == string.Empty)
                {
                    if (gname == string.Empty)
                    {
                        //ds = DBConnector.executeQuery("Intouch", "SELECT 'EPI*MCE*' gname, 'DAS_STY' eqType UNION SELECT 'EPI*MCE*' gname, 'DAS_ESC' eqType UNION SELECT 'ETCH*ME*' gname, '' eqType");
                        ds_array[0, 0] = "EPI*MCE*";
                        ds_array[0, 1] = "DAS_STY";
                        ds_array[1, 0] = "EPI*MCE*";
                        ds_array[1, 1] = "DAS_ESC";
                        ds_array[2, 0] = "ETCH*ME*";
                        ds_array[2, 1] = string.Empty;
                    }
                    if (gname == "EPI*MCE*")
                    {
                        //ds = DBConnector.executeQuery("Intouch", "SELECT 'EPI*MCE*' gname, 'DAS_STY' eqType UNION SELECT 'EPI*MCE*' gname, 'DAS_ESC' eqType ");
                        ds_array[0, 0] = "EPI*MCE*";
                        ds_array[0, 1] = "DAS_STY";
                        ds_array[1, 0] = "EPI*MCE*";
                        ds_array[1, 1] = "DAS_ESC";
                        ds_array[2, 0] = string.Empty;
                        ds_array[2, 1] = string.Empty;
                    }
                    if (gname == "ETCH*ME*")
                    {
                        //ds = DBConnector.executeQuery("Intouch", "SELECT 'ETCH*ME*' gname, '' eqType");
                        ds_array[0, 0] = string.Empty;
                        ds_array[0, 1] = string.Empty;
                        ds_array[1, 0] = string.Empty;
                        ds_array[1, 1] = string.Empty;
                        ds_array[2, 0] = "ETCH*ME*";
                        ds_array[2, 1] = string.Empty;
                    }
                }
                else
                {
                    //ds = DBConnector.executeQuery("Intouch", "SELECT '" + gname + "' gname");
                    ds_array[0, 0] = string.Empty;
                    ds_array[0, 1] = string.Empty;
                    ds_array[1, 0] = string.Empty;
                    ds_array[1, 1] = string.Empty;
                    ds_array[2, 0] = "ETCH*ME*";
                    ds_array[2, 1] = string.Empty;
                }
                //else if ( gname == string.Empty)
                //{
                //    ds = DBConnector.executeQuery("Intouch", "SELECT DISTINCT gname FROM vw_eq_status WHERE vName = '" + vName + "' AND gname IS NOT NULL");
                //}
                //else
                //{
                //    ds = DBConnector.executeQuery("Intouch", "SELECT DISTINCT gname FROM vw_eq_status WHERE vName = '" + vName + "' AND gname ='" + gname  + "'");
                //}

                DataView dv = new DataView(dt);

                for (int i = 0; i <= 3 - 1; i++)
                {
                    if (vName == "DAS" && pid == string.Empty)
                    {
                        if (ds_array[i, 0].ToString() == "EPI*MCE*")
                        {
                            dv.RowFilter = "gname='" + ds_array[i, 0].ToString() + "' and typeName like '" + ds_array[i, 1].ToString() + "%'";
                        }
                        else if (ds_array[i, 0].ToString() == "ETCH*ME*")
                        {
                            dv.RowFilter = "gname='" + ds_array[i, 0].ToString() + "'";
                        }
                        dt = dv.ToTable();
                    }

                    for (int col = dt.Columns.Count - 1; col >= 0; col--)
                    {
                        bool removeColumn = true;
                        foreach (DataRow row in dt.Rows)
                        {
                            if (!row.IsNull(col))
                            {
                                removeColumn = false;
                                break;
                            }
                        }
                        if (removeColumn && col > 5)
                            dt.Columns.RemoveAt(col);
                    }

                    if (dt.Rows.Count > 0 && ds_array[i, 0].ToString() != string.Empty)
                    {
                        ExcelWorksheet sheet1 = null;
                        if (vName == "DAS" && pid == string.Empty)
                        {
                            if (ds_array[i, 0].ToString() == "EPI*MCE*")
                            {
                                ep.Workbook.Worksheets.Add(ds_array[i, 0].ToString().Replace("*", "-") + " " + ds_array[i, 1].ToString());
                                sheet1 = ep.Workbook.Worksheets[ds_array[i, 0].ToString().Replace("*", "-") + " " + ds_array[i, 1].ToString()];
                            }
                            else if (ds_array[i, 0].ToString() == "ETCH*ME*")
                            {
                                ep.Workbook.Worksheets.Add(ds_array[i, 0].ToString().Replace("*", "-"));
                                sheet1 = ep.Workbook.Worksheets[ds_array[i, 0].ToString().Replace("*", "-")];
                            }
                        }
                        else
                        {
                            ep.Workbook.Worksheets.Add("MySheet");
                            sheet1 = ep.Workbook.Worksheets["MySheet"];
                        }

                        sheet1.Cells["A1"].LoadFromDataTable(dt, true);
                        sheet1.Cells["A1"].Value = "Toolid";
                        sheet1.Cells["B1"].Value = "課別";
                        sheet1.Cells["C1"].Value = "Locationid";
                        sheet1.Cells["D1"].Value = "柱位";
                        sheet1.Cells["E1"].Value = "設備類型";
                        sheet1.Cells["F1"].Value = "廠商";

                        sheet1.Cells.AutoFitColumns();
                    }
                }
            }
            else
            {
                ep.Workbook.Worksheets.Add("MySheet");
            }

            return ep.GetAsByteArray();
        }
    }
}