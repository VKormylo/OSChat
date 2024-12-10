using System.Runtime.InteropServices;

namespace logic.imports;

public static class PipesImports
{
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr CreateNamedPipe(
        string lpName,
        uint dwOpenMode,
        uint dwPipeMode,
        uint nMaxInstances,
        uint nOutBufferSize,
        uint nInBufferSize,
        uint nDefaultTimeOut,
        IntPtr lpSecurityAttributes);
    
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool ConnectNamedPipe(IntPtr hNamedPipe, IntPtr lpOverlapped);
}