using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Dashboard.Web._business.Infrastrucure
{
    public class CsvProcessor
    {
        #region Extract CSV
        /// <summary>
        /// Extracts the CSV.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectList">The object list.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">objectList</exception>
        /// <exception cref="System.Exception">List contains no objects</exception>
        public static byte[] ExtractCSV<T>(List<T> objectList, string[] propertiesToExclude, char seperator)
        {
            if (objectList == null)
            {
                throw new ArgumentNullException("objectList");
            }

            //Extract CSV header
            string header = GenerateCSVHeader(typeof(T), propertiesToExclude, seperator);

            byte[] byteConversion = null;

            string dataString = header;

            try
            {
                foreach (var item in objectList)
                {
                    string rowValue = GenerateCSVRow(item, propertiesToExclude, seperator);
                    dataString += System.Environment.NewLine + rowValue;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            //Extract byte[]
            byteConversion = Encoding.ASCII.GetBytes(dataString);
            return byteConversion;
        }

        /// <summary>
        /// Extracts the CSV.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectList">The object list.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">objectList</exception>
        /// <exception cref="System.Exception">List contains no objects</exception>
        public static void WriteCSV<T>(string fullPath, List<T> objectList, string[] propertiesToExclude, char seperator)
        {
            byte[] csvArray = ExtractCSV<T>(objectList, propertiesToExclude, seperator);

            File.WriteAllBytes(fullPath, csvArray);
        }

        /// <summary>
        /// Generates the CSV header.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">This object does not have any properties</exception>
        private static string GenerateCSVHeader(Type t, string[] propertiesToExclude, char seperator)
        {
            string retString = "";

            //Ensure there are properties
            if (!t.GetProperties().Any())
            {
                throw new Exception("This object does not have any properties");
            }

            //Extract property List
            List<PropertyInfo> propList = t.GetProperties()
                                            .Where(x => propertiesToExclude == null || !propertiesToExclude.ToList().Exists(y => y == x.Name)) //Has properties to exclude and property is not in the exclusions list
                                            .ToList();

            if (!propList.Any())
            {
                throw new Exception("This object does not have any properties that were not excluded");
            }

            //Add first Item to list
            retString += propList.First().Name;

            for (int i = 1; i < propList.Count; i++)
            {
                string property = propList[i].Name;

                //Add remaining items to list
                retString += seperator + property;
            }

            return retString;
        }

        /// <summary>
        /// Generates the CSV row.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">This object does not have any properties</exception>
        private static string GenerateCSVRow(object obj, string[] propertiesToExclude, char seperator)
        {
            string retString = "";

            //Extract type
            Type t = obj.GetType();

            //Ensure there are properties
            if (!t.GetProperties().Any())
            {
                throw new Exception("This object does not have any properties");
            }

            //Extract property List
            List<PropertyInfo> propList = t.GetProperties()
                                            .Where(x => propertiesToExclude == null || !propertiesToExclude.ToList().Exists(y => y == x.Name)) //Has properties to exclude and property is not in the exclusions list
                                            .ToList();

            //Add first Item to list
            retString += propList.First().GetValue(obj);

            for (int i = 1; i < propList.Count; i++)
            {
                //Add remaining items to list
                retString += seperator.ToString() + propList[i].GetValue(obj);
            }

            return retString;
        }
        #endregion

        #region Extract Class
        /// <summary>
        /// Reads values from a Csv and generates a list of type 'T'
        /// </summary>
        /// <param name="csvPath">The path to the Csv or txt</param>
        /// <param name="separator">The delimiting character</param>
        /// <param name="typeAndCsvMustMatchExactly">Whether or not all fields in delimeted header row must have matching property in type 'T'</param>
        /// <returns>A list of type 'T'</returns>
        public static List<T> ExtractClassList<T>(string csvPath, char separator, bool typeAndCsvMustMatchExactly)
        {
            List<string> csvValues = LoadDelimitedCsv(csvPath); //Load Csv values

            return ExtractClassList<T>(csvValues, separator, typeAndCsvMustMatchExactly);
        }

        public static List<T> ExtractClassList<T>(ICollection<string> csvRows, char separator, bool typeAndCsvMustMatchExactly)
        {
            var retList = new List<T>();

            if (!csvRows.Any()) //Nothing in the Csv
            {
                throw new Exception("Csv is empty");
            }

            if (csvRows.Count == 1) //Only a header row
            {
                return new List<T>();
            }

            string headerRow = csvRows.First(); //extract header row

            if (typeAndCsvMustMatchExactly && !ValidateHeaderRow(typeof (T), headerRow, separator))
                //Must check match and Type and CSV do not match
            {
                throw new Exception("Type and CSV do not match");
            }

            //loop through data and create instances of a class
            for (int i = 1; i < csvRows.Count; i++)
            {
                var row = csvRows.ElementAt(i);

                if (IsEmptyRow(row, separator))
                {
                    continue;
                }

                T type = ExtractClass<T>(headerRow, row, separator, false);
                retList.Add(type);
            }

            return retList;
        }

        private static bool IsEmptyRow(string row, char seperator)
        {
            var list = row.Split(seperator);

            if (list.Any(item => !string.IsNullOrWhiteSpace(item)))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Loads a Csv into a list of string
        /// </summary>
        /// <param name="path">The path to the Csv or txt</param>
        /// <returns>A list containing all rows in Csv. First in list will be the header row</returns>
        public static List<string> LoadDelimitedCsv(string path)
        {
            List<string> retList = new List<string>();

            if (!(Path.GetExtension(path).ToUpper() == ".CSV" || Path.GetExtension(path).ToUpper() == ".TXT")) //Check it is a Csv or txt
            {
                throw new Exception("File is not a CSV");
            }

            if (!File.Exists(path)) //Check the file exists
            {
                throw new Exception("File " + path + " does not exist");
            }

            StreamReader fs = new StreamReader(path);
            try
            {
                while (!fs.EndOfStream)
                {
                    retList.Add(fs.ReadLine());
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error loading Csv file", ex);
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }
            }

            return retList;
        }

        /// <summary>
        /// Reads values from delimited string and sets matches properties in type 't'
        /// </summary>
        /// <typeparam name="T">The type whose properties must be set</typeparam>
        /// <param name="headerRow">The delimited header row</param>
        /// <param name="dataRow">The delimited data row</param>
        /// <param name="separator">The delimiting character</param>
        /// <param name="typeAndCsvMustMatchExactly">Whether or not all fields in delimeted header row must have matching property in type 'T'</param>
        /// <returns>An instance of type 'T'</returns>
        public static T ExtractClass<T>(string headerRow, string dataRow, char separator, bool typeAndCsvMustMatchExactly)
        {
            List<string> headerValueList = ExtractValues(headerRow, separator); //extract header values
            List<string> dataValueList = ExtractValues(dataRow, separator); //extract data values

            if (dataValueList.Count != headerValueList.Count) //Header rows and data rows do not match
            {
                throw new Exception("Header rows and data rows do not match");
            }

            if (typeAndCsvMustMatchExactly && !ValidateHeaderRow(typeof(T), headerRow, separator)) //Must check match and Type and CSV do not match
            {
                throw new Exception("Type and CSV do not match");
            }

            T retType = (T)Activator.CreateInstance(typeof(T)); //Create instance of type

            //Loop through values and set properties on type
            for (int i = 0; i < headerValueList.Count; i++)
            {
                string headerValue = headerValueList[i]; //header value
                string dataValue = dataValueList[i]; //data value

                var prop = typeof(T).GetProperty(headerValue, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance); //set 

                if (prop != null) //dont set property if it does not exist
                {
                    if (prop.PropertyType == typeof(DateTime))
                    {
                        DateTime date = (DateTime.Parse(dataValue));
                        prop.SetValue(retType, date, null);
                    }
                    // Same check for nullable DateTime.
                    else if (prop.PropertyType == typeof(Nullable<DateTime>))
                    {
                        DateTime? date = (DateTime?)prop.GetValue(retType, null);
                        DateTime? newDate = DateTime.SpecifyKind(date.Value, DateTimeKind.Utc);
                        prop.SetValue(retType, newDate, null);
                    }
                    else if (prop.PropertyType == typeof(long))
                    {
                        var longVal = long.Parse(dataValue);
                        prop.SetValue(retType, longVal, null);
                    }
                    else if (prop.PropertyType == typeof(int))
                    {
                        var intVal = int.Parse(dataValue);
                        prop.SetValue(retType, intVal, null);
                    }
                    else
                    {
                        prop.SetValue(retType, dataValue, null);
                    }

                }
            }

            return retType;
        }

        /// <summary>
        /// Validates a header row against a Type. Checks properties in type against columns in delimited string
        /// </summary>
        /// <param name="t">The type</param>
        /// <param name="headerRow">The delimited string</param>
        /// <param name="separator">The dilimiting character</param>
        /// <returns>True if the type has all properties in field columns</returns>
        private static bool ValidateHeaderRow(Type t, string headerRow, char separator)
        {
            List<PropertyInfo> propertyList = t.GetProperties().ToList(); //Load properties for T
            List<string> valueList = ExtractValues(headerRow, separator); //Extract values in header row

            if (propertyList.Count != valueList.Count) //property count and header column count are different
            {
                return false;
            }

            foreach (var item in propertyList)
            {
                bool isInCsvHeader = valueList.Any(x => x.ToLower() == item.Name.ToLower());

                if (!isInCsvHeader) //this property is not in the csv
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Extracts the values from a delimited string
        /// </summary>
        /// <param name="valueString">String to split</param>
        /// <param name="separator">The separator</param>
        /// <returns></returns>
        private static List<string> ExtractValues(string valueString, char separator)
        {
            List<string> retList = new List<string>();

            retList = valueString.Split(separator).ToList();

            return retList;
        }
        #endregion
    }
}
