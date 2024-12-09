using System.Runtime.InteropServices;

namespace logic.imports;

public static class CommonImports
{
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool CloseHandle(IntPtr hObject);
}