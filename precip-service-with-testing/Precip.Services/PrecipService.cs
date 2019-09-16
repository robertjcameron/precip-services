using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Precip.Services
{
    public class PrecipService
    {
        //the date is at the 6th column in the spreadsheet, so in a zero-based list that's the 5th column
        private const int CellOffsetForDate = 5;

        //likewise the precipitation value is at the 7th column in the spreadsheet, so in a zero-based list that's the 6th column
        private const int CellOffsetForPrecip = 6;
        private const int WorkbookSheetPosition = 0;
        private const string DesiredDateFormat = "yyyy-MM-dd";
        private const string JSON_NoDataFoundMsg = "{\"error\":\"no data found for date\"}";
        private const string JSON_InvalidDateMsg = "{\"error\":\"invalid date\"}";
        private const string JSON_Snippet_date = "{\"date\":\"";
        private const string JSON_Snippet_type_actual_value = "\",\"type\":\"actual\",\"value\":";
        private const string JSON_Snippet_type_predict_value = "\",\"type\":\"predict\",\"value\":";

        /* Given a date, provide a precipitation prediction for that date in zip code 27612.
    		If no date is provided, return a precipitation prediction for the current date (via the overloaded method).
    		If the date is in the data set, return the actual precipitation for the provided date.
    		The value is either the value from the date provided in 27612-precipitation-data.xlsx if the date appears there,
    		or an average from 27612-precipitation-data.xlsx of that same date from different years, even if the date provided is before those in the set.
            Null values for precipitation in 27612-precipitation-data.xlsx are ignored.
    	Accepts:
    		date in yyyy-MM-dd format
    	Returns:
    		JSON with the following:
    		"date": the date corresponding to the response, in yyyy-MM-dd format (the input parameter, or the current date if no parameter was provided)
    		"type": "predict" if this is a prediction, or "actual" if this date is in the data set
    		"value" : an amount of precipitation in the same measurement as in 27612-precipitation-data.xlsx (presumably decimal values of an inch)
    	OR, if input parameter is not a valid date in the specified format:
    		{"error:":"invalid date"}
    	OR, if the date does not appear at all in the data file:
    		{"error:":"no data found for date"}
    	*/
        public String GetPrecip(String requestDateString)
        {
            DateTime precipDate;
            if (DateTime.TryParseExact(requestDateString, DesiredDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out precipDate))
            {
                IWorkbook precipWorkbook;
                
                using (FileStream fs = new FileStream("../../../../Precip.Services/27612-precipitation-data.xlsx",FileMode.Open, FileAccess.Read))
//                using (FileStream fs = new FileStream("C:\\aaa rob\\vscode-workspace\\precip-service-with-testing\\Precip.Services\\27612-precipitation-data.xlsx",FileMode.Open, FileAccess.Read))
                {
                    precipWorkbook = WorkbookFactory.Create(fs);
                }

                ISheet precipSheet = precipWorkbook.GetSheetAt(WorkbookSheetPosition);
                // I don't see search/filter/sort options, so iterating through the spreadsheet.
                System.Collections.IEnumerator precipRows = precipSheet.GetRowEnumerator();
                List<double> precipsForDate = new List<double>();

                // Skip the header row.
                precipRows.MoveNext();

                // Loop through the spreadsheet. If an entry is found for the matching month, day, and year, return the precipitation value.
                // Otherwise collect all precipitation values with the matching month and day.
                while (precipRows.MoveNext())
                {
                    IRow precipRow = (XSSFRow) precipRows.Current;
                    DateTime rowDate = precipRow.GetCell(CellOffsetForDate).DateCellValue;

                    if ((rowDate.Month == precipDate.Month) && (rowDate.Day == precipDate.Day))
                    {
                        if (rowDate.Year == precipDate.Year)
                        {
                            double actualPrecipVal = precipRow.GetCell(CellOffsetForPrecip).NumericCellValue;
                            return $"{JSON_Snippet_date}{precipDate.ToString(DesiredDateFormat)}{JSON_Snippet_type_actual_value}{actualPrecipVal}}}";
                        }
                        else if (precipRow.GetCell(CellOffsetForPrecip) != null)
                        {
                            precipsForDate.Add(precipRow.GetCell(CellOffsetForPrecip).NumericCellValue);
                        }
                    }
                }
 
                if (precipsForDate.Count == 0)
                {
                    return JSON_NoDataFoundMsg;
                }
                else
                {
                    double precipsAverage = precipsForDate.Average();
                    return $"{JSON_Snippet_date}{precipDate.ToString(DesiredDateFormat)}{JSON_Snippet_type_predict_value}{precipsAverage}}}";
                }
            }
            else
            // The input value failed to parse as a date.
            {
                return JSON_InvalidDateMsg;
            }


        }

        public String GetPrecip()
        {
            return GetPrecip(DateTime.Today.ToString(DesiredDateFormat));
        }

    //TODO
    //get the file location a better way

    }
}