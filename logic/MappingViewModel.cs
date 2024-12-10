using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.DirectoryServices;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using logic.imports;

namespace logic;

public class MappingViewModel: ObservableObject
{
    private static MappingViewModel _instance;
    public static MappingViewModel Instance => _instance ??= new MappingViewModel();
    public IntPtr serverEvent = IntPtr.Zero;
    public IntPtr clientEvent = IntPtr.Zero;
    public IntPtr pBaseAddress = IntPtr.Zero;
    public IntPtr hFileMapping = IntPtr.Zero;

    public string Text { get; set; } = string.Empty;
    public ObservableCollection<Message> Messages { get; set; } = new ObservableCollection<Message>();
    


    public MappingViewModel()
    {
    }

    public void SendMessage(IntPtr signalEvent)
    {
        byte[] dataSend = Encoding.UTF8.GetBytes(Text);

        const int bufferSize = 1024;
        if (dataSend.Length > bufferSize)
        {
            throw new Exception($"Data size ({dataSend.Length}) exceeds buffer size ({bufferSize}).");
        }

        Marshal.Copy(dataSend, 0, pBaseAddress, dataSend.Length);
        if (pBaseAddress == IntPtr.Zero)
            throw new Exception("Memory mapping failed: pBaseAddress is null.");
       
        Message sendText = new Message(Text, false);
        Messages.Add(sendText);

        MappingImports.SetEvent(signalEvent);
    }

    
    public void ReceiveMessage(bool isServer)
    {
        try
        {
            while (true)
            {
                if (isServer)
                    MappingImports.WaitForSingleObject(clientEvent, Constants.INFINITE);
                else
                    MappingImports.WaitForSingleObject(serverEvent, Constants.INFINITE);

                byte[] dataRead = new byte[1024];
                Marshal.Copy(pBaseAddress, dataRead, 0, dataRead.Length);
                string messageText = Encoding.UTF8.GetString(dataRead).Trim('\0');
            
                if (!string.IsNullOrWhiteSpace(messageText))
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Messages.Add(new Message(messageText, true));
                    });
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error in ReceiveMessage: {ex.Message}");
        }
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
            ViewFile();
            Task.Run(() => ReceiveMessage(true));
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
            ViewFile();
            Task.Run(() => ReceiveMessage(false));
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
    }
}