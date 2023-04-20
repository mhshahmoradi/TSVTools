# TSVTools
TSVTools is a lightweight library that makes it easy to read and write TSV (tab-separated values) files in C#. It provides a simple and intuitive interface for working with TSV files, allowing you to quickly and easily read and write data in TSV format.

## Installation
You can install TSVTools via NuGet using the following command:

    dotnet add package TSVTools --version 1.0.0

## Usage
First you need write your input model:

    public class MyDataModel
    {
	    public  string Name { get; set; }
	    public  int Age { get; set; }
	    public  string Email { get; set; }
    }
    
  ### Reading from a TSV file
  To read data from a TSV file, you first need to create an instance of the `TsvFile` class with the path to the TSV file:
  

    var tsvFile = new TsvFile<MyDataModel>("path.tsv");
This will create a `TsvFile<MyDataModel>` object that you can use to access the data in the TSV file.

### Writing to a TSV file
To write data to a TSV file, you first need to create an instance of the `TsvFile` class with the column names for the TSV file:

    var tsvFile = new TsvFile<MyDataModel>();

You can then add data to the `TsvFile` object by creating instances of your `MyDataModel` class and adding them to the `TsvFile` object:

    var data = new MyDataModel { Name = "MHReza", Age = 18, Email = "mh.shahmorady@gmail.com" }; 
    tsvFile.AddRow(data);
Once you have added all your data to the `TsvFile` object, you can save the data to a TSV file:

    tsvFile.SaveToFile("path.tsv");
This will save the data to a TSV file at the specified path.

you can read data like this:

    foreach (var row in tsvFile)
    {
        Console.WriteLine($"Name: {row.Name} , age is : {row.Age} , Email is {row.Email}");
    }

### Appending to a TSV file
To append data from a TSV file to your data, you can use the `AppendFile` method:

    tsvFile.AppendFile("path/to/my/existing/tsv/file.tsv");

## Contributing
If you'd like to contribute to TSVTools, please submit a pull request or open an issue. We welcome all feedback and contributions!