# What is ConsoleRenderer
This is a simple and easy-to-use library to help render images in terminal windows that works on Windows or Linux. This allows a developer to easily build retro looking applications using C# and .NET.

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

## Other examples
Please review the example implementations in the Examples project for additional code samples and interesting visual demo's.

The Examples project can also be run with one of the following arguments to run each example program.
- Noise
- Pong
- Rectangles

# What's the performance like?
Since version `0.2.0` the performance has been much improved, particularly on Windows systems. That said, redrawing large portions of the screen is still fairly slow on both Linux & Windows as the `Noise` example demonstrates.

Generally it seems that performance is negatively impacted by operations such as repositioning the cursor, as well as changing either the foreground or background color. Limiting how often these operations are called for any given frame can yield some fairly speedy drawing, as shown by the `Rectangles` example.

The latter also benefits from optimizations done to prevent redrawing pixels that haven't changed. Since new rectangles are no larger than 1/4 of the screen surface, this allows for framerates above 100 fps on Windows, and a multiple of that for Linux systems.

# Contributing
If there are changes you'd like to see, feel free to create an issue or a PR.
