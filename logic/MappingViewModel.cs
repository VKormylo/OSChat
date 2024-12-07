using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.DirectoryServices;
using System.Net.Sockets;
using System.Windows;

namespace logic;

public class MappingViewModel
{
    public string Text { get; set; } = string.Empty;
    public ObservableCollection<Message> Message { get; set; } = new ObservableCollection<Message>();

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr CreateFileMapping(
        IntPtr hFile,
        IntPtr lpFileMappingAttributes,
        uint flProtect,
        uint dwMaximumSizeHigh,
        uint dwMaximumSizeLow,
        string lpName);
    
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr MapViewOfFile(
        IntPtr hFileMappingObject,
        uint dwDesiredAccess,
        uint dwFileOffsetHigh,
        uint dwFileOffsetLow,
        uint dwNumberOfBytesToMap);
    
                                                    
public MappingViewModel()
    {
        
    }

    public void SendMessage()
    {
        
    }

    public void ReceiveMessage()
    {
        
    }
    
    protected void CreatAndViewFile()
    {
        IntPtr hFileMapping = new IntPtr();
        try
        {
            hFileMapping = CreateFileMapping(
                new IntPtr(-1),
                IntPtr.Zero,
                Constants.PAGE_READWRITE,
                0,
                Constants.FILE_SIZE,
                Constants.FILE_NAME);

            IntPtr pBaseAddress = MapViewOfFile(
                hFileMapping,
                Constants.FILE_MAP_ALL_ACCESS,
                0,
                0,
                Constants.FILE_SIZE);
        }
        catch (Exception e)
        {
            MessageBox.Show("Failed to create or display a file in memory: " + e.Message);
        }
    }
}