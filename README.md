![dbqf](https://raw.githubusercontent.com/stuarta0/dbqf/master/resources/dbqf.png)

Database Query Framework

## Requirements

- Visual Studio Express 2013 or greater
- NuGet package manager
- .NET 2.0 for core libraries, .NET 4.5 for the standalone application

## What is it?

An SQL builder with awesome UI.

Long answer: a library primarily aimed at **simplifying presentation of complex data mining for the end user** in .NET applications.  The intent is to provide a way of mapping a complete relational data structure and allowing the UI to dynamically construct itself for various purposes.  Construction of the UI is modular with use of factories, adapters and (in the example Standalone project) dependency injection.  The code is database engine independent allowing you to create whatever SQL is required to suit your engine (currently tested: `MSSQL`, `SQLite`).  Note this is **not an ORM** but could conceivably have an adapter to an ORM's mapping.

The library employs many patterns from Gang of Four and the UI is abstracted using a variety of the **Presentation Model** (and subsequently **MVVM**).  By doing this, the core behaviour can be reused across many UI toolkits with two provided out-of-the-box: WinForms and WPF.  This project roughly follows **Test Driven Development** (TDD) and contains NUnit test cases for the core library.  It also employs **Fluent** patterns throughout the code to expidite object creation.

No external libraries are needed for the core and WinForms libraries.  The Standalone applications do require a number of external libraries but can be updated with NuGet package restore.

### Why would I want to use it?
- Automated UI generation for boring, repetitive search fields, operators and execution.
- UI controls generated from factories with out-of-the-box behaviour based on field types (but completely customisable).
- Hierarchical data fetching to display a tree of data while still allowing full search capabilities over the data *(undergoing rewrite)*.
- Modular code allowing replacement of any functionality: what fields to display, what controls to create, what operators are available, how SQL statements are generated, etcetera.
- For both WinForms and WPF:
  - Preset Control lists fields with name and a control with a common operator (`between` for numbers and dates, `contains` for string; customisable).
  - Simple Control allows selection of field and operator with a control created dependant on both.
  - Advanced Control allowing full drill-down of related fields, operators, controls dependant on path and hierarchical combinations (AND, OR) of all parameters.

## Enough words, show me!

### Standalone Application
Provided standalone application in .NET 4.5 WinForms that uses inversion of control, loads XML projects, has custom parsing and threaded behaviour.

![Example-Standalone](https://raw.githubusercontent.com/stuarta0/dbqf/master/docs/example-loading.png)

### Preset
Fully customisable list of field / value pairs.

![Example-Preset](https://raw.githubusercontent.com/stuarta0/dbqf/master/docs/example-preset.png)

### Standard
Fully customisable list of field / operator / value combinations.

![Example-Standard](https://raw.githubusercontent.com/stuarta0/dbqf/master/docs/example-standard.png)

### Advanced
Complete control over parameters with AND/OR from any field or path from within the database as well as selection of operator / value.

![Example-Advanced](https://raw.githubusercontent.com/stuarta0/dbqf/master/docs/example-advanced.png)

### Custom output fields
Choice of fields to retrieve using drag-drop from hierarchical representation of data structure.

![Example-Output](https://raw.githubusercontent.com/stuarta0/dbqf/master/docs/example-output.png)

### Generated SQL
Example of generated parameterised SQL.

![Example-Output](https://raw.githubusercontent.com/stuarta0/dbqf/master/docs/example-sql.png)

### In Detail (Preset)

![Preset](https://raw.githubusercontent.com/stuarta0/dbqf/master/docs/preset.png)

## Documentation

Full documentation TBA.  Code has XML documentation, test cases show usage, and Standalone project shows real-world use.

### Sample Code

Using the core, Sql and WinForms libraries, here's sample usage in a WinForms application:

```c#
  var config = new dbqf.Sql.Configuration.MatrixConfiguration()
      .Subject(new dbqf.Configuration.Subject("Test")
          .Sql("SELECT * FROM [Test]")
          .FieldId(new dbqf.Configuration.Field("Id", typeof(int)))
          .FieldDefault(new dbqf.Configuration.Field("Name", typeof(string)))
          .Field(new dbqf.Configuration.Field("Total", typeof(int)))
          .Field(new dbqf.Configuration.Field("Date Created", typeof(DateTime))));

  var preset = new dbqf.WinForms.PresetView(new dbqf.Display.Preset.PresetAdapter<Control>(
      new dbqf.WinForms.UIElements.WinFormsControlFactory(), 
      new dbqf.Display.ParameterBuilderFactory()));

  preset.Adapter.SetParts(new dbqf.Display.FieldPathFactory().GetFields(config[0]));
  preset.Dock = DockStyle.Fill;
  this.Controls.Add(preset);
```

We create a configuration fluently, which contains one subject called Test with Sql that selects from a table named Test and has four fields; an id, name, total and date created.

Once we have our configuration set up, we want to display it.  We'll use the PresetView in the WinForms library which requires a PresetAdapter<Control>.  The generic type tells us what types of controls the PresetView will be generating.  The adapter requires a control factory instance which will be used to generate controls for our view, and a parameter builder factory which will provide the defaults for how to search a field (equals, contains, between, etc).

Once we've got our view instantiated, we initialise it with some fields using a built-in factory that determines these for us.

After that we add it to the form and we've got ourselves a UI to search our configuration.  How it searches comes next.


## License

This project has an MIT license to allow the most freedom of use.  That said, myself and other users of the library would greatly appreciate any contributions you can make.