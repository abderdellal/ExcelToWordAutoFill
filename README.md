# ExcelToWordAutoFill<br/>
This is a simple tool to autofill .doc forms from an excel document.<br/>
The user has to create place holders in the .doc file (example : #name#) and those place holders are going to be replaced by values extracted from the Excel document.<br/>
To Fill a form automatically from an Excel Table the user has to select which form he wants to fill and search for the specified line he wants to extract values from.<br/>
But before that he has to create a new form in the add form view and configure which place holders take which columns, he also has to specify the search column that he's going to use, to search for a specific line.<br/>
When configuring a new form the user can specify if the first line of the excel table is the header, if this is the case, he can use the autofetch from headers button to generate new placeholders with their respective columns so that he can use them in his .doc document.<br/>
An .sdf file is created in the user folder, this file is the database file.<br/>
This application has been tested with Windows 10 and 8 with Microsoft 2016.<br/>