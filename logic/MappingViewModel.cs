using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.DirectoryServices;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using logic.imports;

namespace logic;

public class MappingViewModel
{
    public string Text { get; set; } = string.Empty;
    public ObservableCollection<Message> Messages { get; set; } = new ObservableCollection<Message>();
    IntPtr pBaseAddress = IntPtr.Zero;
    IntPtr hFileMapping = IntPtr.Zero;
    public IntPtr serverEvent = IntPtr.Zero;
    public IntPtr clientEvent = IntPtr.Zero;


    public MappingViewModel()
    {
    }

    public void SendMessage(IntPtr signalEvent)
    {
        byte[] dataSend = Encoding.UTF8.GetBytes(Text);
        Marshal.Copy(dataSend,
            0,
            pBaseAddress,
            Encoding.UTF8.GetBytes(Text).Length);
        Message sendText = new Message(Encoding.UTF8.GetString(dataSend), false);
        Messages.Add(sendText);
        MappingImports.SetEvent(signalEvent);
    }

    public void ReceiveMessage(IntPtr waitEvent)
    {
        MappingImports.WaitForSingleObject(waitEvent, Constants.INFINITE);

        byte[] dataRead = new byte[1024];
        Marshal.Copy(pBaseAddress, dataRead, 0, dataRead.Length);
        Message receiveText = new Message(Encoding.UTF8.GetString(dataRead), true);
        Messages.Add(receiveText);
    }
    public void StartServer()
    {

        try
        {
            serverEvent = MappingImports.CreateEvent(IntPtr.Zero, false, false, Constants.ServerEventName);
            clientEvent = MappingImports.CreateEvent(IntPtr.Zero, false, false, Constants.ClientEventName);
            hFileMapping = MappingImports.CreateFileMapping(
                new IntPtr(-1),
                IntPtr.Zero,
                Constants.PAGE_READWRITE,
                0,
                Constants.FILE_SIZE,
                Constants.FILE_NAME);
        }
        catch (Exception e)
        {
            MessageBox.Show("Failed to create a file: " + e.Message);
        }   
    }
    public void StartClient()
    {
        try
        {
            serverEvent = MappingImports.OpenEvent(0x1F0003, false, Constants.ServerEventName);
            clientEvent = MappingImports.OpenEvent(0x1F0003, false, Constants.ClientEventName);
            hFileMapping = MappingImports.OpenFileMapping(Constants.FILE_MAP_ALL_ACCESS,
                false,
                Constants.FILE_NAME); 
        }
        catch (Exception e)
        {
            MessageBox.Show("Failed to start a client: " + e.Message);
        }

    }

    public void ViewFile()
    { 
        try
        {
            pBaseAddress = MappingImports.MapViewOfFile(
                hFileMapping,
                Constants.FILE_MAP_ALL_ACCESS,
                0,
                0,
                Constants.FILE_SIZE);
        }
        catch (Exception e)
        {
            MessageBox.Show("Failed to display a file in memory: " + e.Message);
        }
    }
    public void Exit()
    {
        MappingImports.UnmapViewOfFile(pBaseAddress);
        CommonImports.CloseHandle(hFileMapping);
        CommonImports.CloseHandle(serverEvent);
        CommonImports.CloseHandle(clientEvent);
    }
}