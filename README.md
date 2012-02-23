Yuv.KA
======

About
-----

The short version: Yuv.KA is a video manipulation program.

The long version: Yuv.KA is a video manipulation program using .NET 4.0, [WPF](http://en.wikipedia.org/wiki/Windows_Presentation_Foundation) and [Caliburn.Micro](http://caliburnmicro.codeplex.com/). It's based on the idea of representing a video manipulation pipeline as a graph (that's abstract nodes connected via edges). It doesn't support sound playback, as it's targeted at the yuv420 format, which contains only raw frame data, and no sound information. Yuv.KA was built in 2012 as a project for the "Praxis der Softwareentwicklung" course at the [University of Karlsruhe (TH)](http://kit.edu). The title is a combination of 'yuv' (which is one of the names used for the [YCbCr](http://en.wikipedia.org/wiki/YCbCr) color space, which is what Yuv.KA reads and writes, and the file extension of the corresponging videos), and 'KA' (which is an abbreviation of Karlsruhe). [Yufka](http://en.wikipedia.org/wiki/Yufka) is also a type of Turkish bread which is the namesake of a Turkish meal quite popular here in *ole Germany*, hence the logo.

Technology
----------

 * [Windows Presentation Foundation](http://msdn.microsoft.com/en-us/library/aa970268.aspx)
 * [.NET 4.0](http://www.microsoft.com/download/en/details.aspx?id=17718)
 * Dependency Injection using [MEF](http://msdn.microsoft.com/en-us/library/dd460648.aspx)
 * Model-View-ViewModel using [Caliburn.Micro 1.3.0](http://caliburnmicro.codeplex.com)
 * Full task-based parallelization using [Task Parallel Library](http://msdn.microsoft.com/en-us/library/dd460717.aspx)
 * Extensible by plugins


What you'll need
----------------

First off, you'll need *Windows*. Sorry Linux and Mac OS users, but Yuv.KA heavily relies on WPF, and that's not available on mono[1]. (At least not at the time of writing)  
You'll also need the [.NET Framework 4.0](http://www.microsoft.com/net)

Plugins
-------

Yuv.KA supports plugin node packages that just have to be dropped into the Plugins folder inside the installation (e.g. `C:\Program Files\Yuv.KA\Plugins\`, but your mileage may vary). These plugins will be loaded at runtime and incorporated into the program, ready for you to use them.

Writing Plugins
---------------

Writing a plugin is pretty simple. I'm going to assume you've got Visual Studio and Yuv.KA somewhere of your computer.

 1. In Visual Studio, create a new Project. Select WPF Application as the project type.
 2. Delete the App.xaml and MainWindow.xaml files VS creates for you. You won't be needing these.
 3. Right-click your project in the solution explorer. Choose Properties (Alt + Enter).
 4. Set the 'Output Type' to be 'Class Library'. Save the change.
 5. Right-click your project and select 'Add Reference...'. Browse to the location of your Yuv.KA installation, and add references to the files YuvKA.Implementation.dll, YuvKA.exe, and Caliburn.Micro.dll. (Be sure to also look in the Plugins folder.) [2]
 6. You're good to go! Create a new class (Shift + Alt + C).
 7. Let your class inherit from YuvKA.Pipeline.Node (or any subclass thereof you may know), and implement the necessary methods[3].
 8. If you want to give your node some parameters visible to the user, attach the `[Browsable(true)]` attribute[4] to the property you want to be visible to the user[5]. (Under certain circumstances, you may also specify the `[DisplayName(<YourName>)]` attribute.)
 9. Once done with this, just build the dll and drop it into Yuv.KA's installation folder or Plugin folder - if everything went right, your node should now be detected by the program.

 [1] - See their [official statement](http://www.mono-project.com/WPF).  
 [2] - Depending on what you're trying to do, you might also need references to `System.Runtime.Serialization` (if you want to have persistent data across pipeline saving/loading) or `System.ComponentModel.DataAnnotations` (If you for example want to give numeric property value certain ranges with `[Range(,)]`).  
 [3] - For the basic `Node`, this is just the `Process` method. You may want to define your own constructor in order to give the node a proper `Name`.  
 [4] - You'll need to be `using System.ComponentModel` for that.  
 [5] - Of course, you can't do this for arbitrary classes. The PropertyEditor included in YuvKA gives you quite a few things to play around with, including booleans, enumerations, file paths, colors, doubles, integers, and a few others. For the full list, check out the source. If you need more than this, you'll need to implement it yourself. To do this, you need to supply a UI for the control in XAML with proper caliburn bindings and a parametrized subclass of `PropertyViewModel<T>`.  
