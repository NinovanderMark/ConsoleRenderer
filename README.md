# What is ConsoleRenderer
This is a simple and easy-to-use library to help render images to the console that works on Windows or Linux. This allows a developer to easily build retro looking applications using C# and .NET.

I learned programming with QBasic on MS-DOS and miss the days where you can get graphics on screen with just a few lines of code, ConsoleRenderer is a bit of an homage to that minimal style of development. Modern graphics libraries tend to have a lot of ceremony to set up a render window and get things on screen, this library aims to be the opposite of that.

## What's the performance like?
It varies, but on my Lenovo P51 laptop the pong example runs at ~800fps on Windows in a 120x30 size Powershell terminal inside Windows Terminal. The same size WSL terminal on Ubuntu runs at ~3600fps, which is considerably faster. Updating every character on screen every frame is a lot slower, and can be as low as 10fps on Windows, or ~50fps on Ubuntu in WSL.

I welcome more rigorous benchmarks, as well as suggestions to improve performance, but the bottom line is that it's fast enough for anything that doesn't redraw the whole screen every frame.

# Why is it on GitHub
It's on GitHub because I enjoy little projects like this, and because I believe in sharing what you enjoy with others, who may also enjoy it.

# What is the future of this project
Right now it's a fun little thing that I made. Maybe I will end up using it in other projects and need additional functionality, maybe other people want to use it in theirs, who knows?

# How to build and run it
Simply open the solution in Visual Studio and hit run to get a simple console window with a blue bouncing ball. The sample project is just a basic example of how it may be used.

For other IDE's, be sure to have the .NET 6 SDK installed and any relevant plugins for C#. There should be no dependencies other than what is in the System namespaces.

# Contributing
If there are changes you'd like to see, feel free to create an issue or a PR.
