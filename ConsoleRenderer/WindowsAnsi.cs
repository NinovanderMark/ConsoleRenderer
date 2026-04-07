using System;
using System.Runtime.InteropServices;
using System.Text;

namespace ConsoleRenderer;

internal class WindowsAnsi
{
    
    private const int StdOutputHandle = -11;
    private const uint VirtualTerminalProcessingFlag = 0x0004;

    private readonly Encoding _outputEncoding;
    private bool _isAnsiInitialized;

    internal WindowsAnsi(): this(Encoding.UTF8)
    {
    }
    
    internal WindowsAnsi(Encoding outputEncoding)
    {
        _outputEncoding = outputEncoding;
    }

    internal void EnsureAnsiSupport()
    {
        if (_isAnsiInitialized) return;
        
        // Windows needs VT mode for ANSI; Linux and macOS terminals already support it
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            EnableVirtualTerminalProcessing();

        Console.OutputEncoding = _outputEncoding;
        _isAnsiInitialized = true;
    }

    private static void EnableVirtualTerminalProcessing()
    {
        var handle = GetStdHandle(StdOutputHandle);
        if (handle != IntPtr.Zero && GetConsoleMode(handle, out var mode))
        {
            SetConsoleMode(handle, mode | VirtualTerminalProcessingFlag);
        }
    }

    [DllImport("kernel32")]
    private static extern IntPtr GetStdHandle(int nStdHandle);

    [DllImport("kernel32")]
    private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

    [DllImport("kernel32")]
    private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);
}