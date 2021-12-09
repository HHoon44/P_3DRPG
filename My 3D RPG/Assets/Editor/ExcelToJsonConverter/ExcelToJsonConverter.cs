using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using ExcelDataReader;
using Newtonsoft.Json;
using UnityEditor;

namespace ProjectW.Editor
{
    public class ExcelToJsonConvert
    {
        private readonly List<FileInfo> srcFiles;
        private readonly List<bool> isUseFiles;
        private readonly string savePath;
        private readonly int sheetCount;
        private readonly int headerRows;

        public ExcelToJsonConvert(string filePath, string savePath)
        {
            srcFiles = new List<FileInfo>();
            srcFiles.Add(new FileInfo(filePath));
            isUseFiles = new List<bool>();
            isUseFiles.Add(true);
            this.savePath = savePath;
            sheetCount = 1;
            headerRows = 2;
        }

        public int SaveJsonFiles()
        {
            return ReadAllTables(SaveSheetJson);
        }

        #region Read Table

        int ReadAllTables(Func<DataTable, string, int> exportFunc)
        {
            if (srcFiles == null || srcFiles.Count <= 0)
            {
                Debug.LogError("Error! No Excel Files!");
                return -1;
            }

            int result = 0;
            for (var i = 0; i < srcFiles.Count; i++)
            {
                if (isUseFiles[i])
                {
                    var file = srcFiles[i];
                    result += ReadTable(file.FullName, FileNameNoExt(file.Name), exportFunc);
                }
            }

            return result;
        }

        int ReadTable(string path, string fileName, Func<DataTable, string, int> exportFunc)
        {
            int result = 0;
            using (var stream = File.Open(path, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    int tableSheetNum = reader.ResultsCount;
                    if (tableSheetNum < 1)
                    {
                        Debug.LogError("Excel file is empty: " + path);
                        return -1;
                    }

                    var dataSet = reader.AsDataSet();

                    int checkCount = sheetCount <= 0 ? tableSheetNum : sheetCount;
                    for (int i = 0; i < checkCount; i++)
                    {
                        if (i < tableSheetNum)
                        {
                            string name = checkCount == 1 ?
                                fileName :
                                fileName + "_" + dataSet.Tables[i].TableName;
                            //result += SaveJson(dataSet.Tables[i], name);
                            result += exportFunc(dataSet.Tables[i], name);
                        }
                    }
                }
            }
            return result;
        }

        #endregion

        #region Save Json Files

        int SaveSheetJson(DataTable sheet, string fileName)
        {
            if (sheet.Rows.Count <= 0)
            {
                Debug.LogError("Excel Sheet is empty: " + sheet.TableName);
                return -1;
            }

            int columns = sheet.Columns.Count;
            int rows = sheet.Rows.Count;

            List<Dictionary<string, object>> tData = new List<Dictionary<string, object>>();

            for (int i = headerRows; i < rows; i++)
            {
                Dictionary<string, object> rowData = new Dictionary<string, object>();
                for (int j = 0; j < columns; j++)
                {
                    string key = sheet.Rows[0][j].ToString();
                    string type = sheet.Rows[1][j].ToString();

                    rowData[key] = SetObjectFiled(type, sheet.Rows[i][j].ToString());
                }

                tData.Add(rowData);
            }

            string json = JsonConvert.SerializeObject(tData, Formatting.Indented);

            // save to file
            string dstFolder = savePath;
            if (!Directory.Exists(dstFolder))
            {
                Directory.CreateDirectory(dstFolder);
            }

            string path = $"{dstFolder}/{fileName}.json";
            using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                using (TextWriter textWriter = new StreamWriter(fileStream, Encoding.UTF8))
                {
                    textWriter.Write(json);
                    Debug.Log("File saved: " + path);
                    return 1;
                }
            }
        }

        object SetObjectFiled(string type, string param)
        {
            object pObj = param;
            switch (type.ToLower())
            {
                case "string":
                    break;
                case "string[]":
                    pObj = param.Split(',');
                    break;
                case "bool":
                    pObj = bool.Parse(param);
                    break;
                case "byte":
                    pObj = byte.Parse(param);
                    break;
                case "int":
                    pObj = int.Parse(param);
                    break;
                case "int[]":
                    pObj = Array.ConvertAll(param.Split(','), element => int.Parse(element));
                    break;
                case "short":
                    pObj = short.Parse(param);
                    break;
                case "long":
                    pObj = long.Parse(param);
                    break;
                case "float":
                    pObj = float.Parse(param);
                    break;
                case "float[]":
                    pObj = Array.ConvertAll(param.Split(','), element => float.Parse(element));
                    break;
                case "double":
                    pObj = double.Parse(param);
                    break;
                case "decimal":
                    pObj = decimal.Parse(param);
                    break;
                default:
                    Assembly assembly = Assembly.Load("Assembly-CSharp");
                    var t = assembly.GetType(type);
                    if (t != null)
                    {
                        if (t.IsEnum)
                        {
                            pObj = Enum.Parse(t, param);
                        }
                    }
                    break;
            }

            return pObj;
        }
        #endregion

        string FileNameNoExt(string filename)
        {
            int length;
            if ((length = filename.LastIndexOf('.')) == -1)
                return filename;
            return filename.Substring(0, length);
        }
    }
}
