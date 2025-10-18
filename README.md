# LSM (Log Structured Merge Tree) Index System

Fully functional LSM Index System written in C#. 

The project features include: 
* Random data generating for table data based on user input for number of rows and columns
* Creating LSM index for any column
* Creating search conditions with logical operator (AND/OR) for concatenation
* Creating aggregate functions for any column
* Manual row adding
* Manual row deleting
* Searching table without using index
* Searching table with indexing

# How to run the project

* Open `./Projekat.sln`
* Position in project direcory with `cd ./LSM_Index_System`
* (Optional) Change values for number of rows and columns in `./Program.cs`
* Run with `dotnet run`
* Generated data is in directory `./data`
* Generated output is in directory `./outputs`
