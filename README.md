# What is ConsoleRenderer
This is a simple and easy-to-use library to help render images in terminal windows that works on Windows, Linux and MacOs. This allows a developer to easily build cross-platform, retro looking graphical applications for the terminal using C# and .NET.

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
Please review the example implementations in the Examples project for additional code samples and interesting visual demos.

The Examples project can also be run with one of the following arguments to run each example program.
- BouncingText
- Pong
- Rectangles
- ColorNoise
- ColorMixing
- WhiteNoise
- Horizontal
- Vertical

### Other projects made with ConsoleRenderer
![afbeelding](https://i.imgur.com/mKvsIWW.png)
Unreleased mining game, by Nino van der Mark

# What's the performance like?
As a general disclaimer, I don't have access to a standalone Linux machine and benchmarks have thus far been done through WSL. If anyone wants to do some more rigorous testing, please be my guest!

The library performs relatively well on anything but redrawing large portions of the screen at once on both Linux & Windows as the `WhiteNoise` and `ColorNoise` examples demonstrate. These examples generally don't achieve framerates north of 10fps on either platform

It seems that performance is negatively impacted most by operations such as repositioning the cursor during rendering, as well as changing either the foreground or background color, meaning that contiguous regions with the same colors perform best, as well as redrawing only limited portions of the screen each frame. 

A good example of significant redraws at higher framerates is the `Rectangles` example, which renders at about 150fps on Windows and over 600fps on Linux on my Lenovo P51 laptop.

Finally, version `0.3.0` added interlaced rendering mode, which can be used to slice the amount of drawing operations in half, thus potentially doubling the framerate, by only updating half the screen rows each frame.

In summary, while it has some limitations, it can be made to run anywhere in the range of acceptable to blisteringly fast.

## Supplemental

After having done some experiments with the [bflat compiler](https://github.com/bflattened/bflat) in conjunction with [bflata](https://github.com/xiaoyuvax/bflata), it seems that native builds produced using these tools perform about 20%-30% better than binaries produced with the ordinary .NET build tooling.

# Contributing
If there are changes you'd like to see, feel free to create an issue or a PR.

If you have a project you've built using this library, let me know! I'm always interested to see what people come up with, and I'm eager to include examples of that on this page.
