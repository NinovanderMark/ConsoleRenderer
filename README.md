# What is ConsoleRenderer
This is a simple and easy-to-use library to help render images to the console that works on Windows or Linux. This allows a developer to easily build retro looking applications using C# and .NET.

I learned programming with QBasic on MS-DOS and miss the days where you can get graphics on screen with just a few lines of code, ConsoleRenderer is a bit of an homage to that minimal style of development. Modern graphics libraries tend to have a lot of ceremony to set up a render window and get things on screen, this library aims to be the opposite of that.

# Getting started
Simply create a new .NET C# Console application using a provided template, and the IDE of your choice. Then add the ConsoleRenderer NuGet package and instantiate the `ConsoleCanvas` class, as in the example below.
```csharp
static int Main(string[] args)
{
	var canvas = new ConsoleCanvas()
		.CreateBorder()
		.Render();

	Console.ReadKey();
	return 0;
}
```
This example renders an outline around the edges of the screen, regardless of the current size of the terminal window. Once the user presses any key, the application shuts down.

## Rendering 'pixels' on screen
To render pixels to the screen, the `Set()` operation can be used to change the value of any screen pixel in the buffer, which will be rendered to the screen the next time we call `Render()`. The following snippet renders a diagonal line of asterisks, starting top left and running down until reaching either the right or bottom side of the terminal window.
```csharp
var canvas = new ConsoleCanvas();
for (int t = 0; t*2 < canvas.Width && t < canvas.Height; t++)
	canvas.Set(t*2, t);

canvas.Render();
```

Please review the example implementations in the Samples directory of the code repository for additional code samples.

# What's the performance like?
It varies, but on my Lenovo P51 laptop the pong example runs at ~800fps on Windows in a 120x30 size Powershell terminal inside Windows Terminal. The same size WSL terminal on Ubuntu runs at ~3600fps, which is considerably faster. Updating every character on screen every frame is a lot slower, and can be as low as 10fps on Windows, or ~50fps on Ubuntu in WSL.

I welcome more rigorous benchmarks, as well as suggestions to improve performance, but the bottom line is that it's fast enough for anything that doesn't redraw the whole screen every frame.

# Contributing
If there are changes you'd like to see, feel free to create an issue or a PR.
