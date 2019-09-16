# precip-services
This is a C# project that when given a date, provide a precipitation prediction for that date in zip code 27612.
If no date is provided, return a precipitation prediction for the current date (via the overloaded method).
If the date is in the data set, return the actual precipitation for the provided date.
The value is either the value from the date provided in 27612-precipitation-data.xlsx if the date appears there,
  or an average from 27612-precipitation-data.xlsx of that same date from different years, even if the date provided
  is before those in the set.
Null values for precipitation in 27612-precipitation-data.xlsx are ignored.
Parameter:
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
    
## Additional information
* VS Code has some issues with the unit tests, however I am able to run these successfully from the command line with "dotnet test".
* The path provided to the xlsx file is so that it will work from running the unit tests in this fashion. I'm sure there is a better way.
* One of the unit tests will fail if run after 9/16/2019. See the unit test code for additional details.
