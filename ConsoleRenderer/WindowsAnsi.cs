using System;
using System.Runtime.InteropServices;

namespace ConsoleRenderer;

internal static class WindowsAnsi
{
    
    private const int StdOutputHandle = -11;
    private const uint VirtualTerminalProcessingFlag = 0x0004;

    private static bool _isAnsiInitialized;
    

    internal static void EnsureAnsiSupport()
    {
        if (_isAnsiInitialized) return;
        
        // Windows needs VT mode for ANSI; Linux and macOS terminals already support it
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            EnableVirtualTerminalProcessing();

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