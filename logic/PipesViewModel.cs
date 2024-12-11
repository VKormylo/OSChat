using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Data;
using logic.imports;

namespace logic;

public delegate void MessageReceivedHandler();

public class PipesViewModel: ObservableObject
{
    private static PipesViewModel _instance;
    
    public static PipesViewModel Instance => _instance ??= new PipesViewModel();
    
    public ObservableCollection<Message> Messages { get; } = new ObservableCollection<Message>();

    public event MessageReceivedHandler MessageReceivedEvent;
    
    private IntPtr _pipeHandle;

    private IntPtr _serverEvent;
    
    private IntPtr _clientEvent;

    private bool _isServer = false;

    public PipesViewModel()
    {
        BindingOperations.EnableCollectionSynchronization(Messages, new object());
    }

    public void StartServer()
    {
        _isServer = true;
        _serverEvent = CommonImports.CreateEvent(
            IntPtr.Zero,
            false,
            false,
            Constants.SERVER_MESSAGE_EVENT_NAME);
        
        _clientEvent = CommonImports.CreateEvent(
            IntPtr.Zero,
            false,
            false,
            Constants.CLIENT_MESSAGE_EVENT_NAME);
        
        if (_serverEvent == new IntPtr(-1)
            || _clientEvent == new IntPtr(-1))
        {
            FileLogging.LogToFile("Failed to create server or client event.", "server", "error");
            MessageBox.Show(
                "Create event failed", 
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            
            return;
        }
        
        _pipeHandle = PipesImports.CreateNamedPipe(
            Constants.PIPE_NAME,
            Constants.PIPE_ACCESS_DUPLEX,
            Constants.PIPE_TYPE_MESSAGE | Constants.PIPE_READMODE_MESSAGE | Constants.PIPE_WAIT,
            Constants.PIPE_UNLIMITED_INSTANCES,
            Constants.PIPE_BUFFER_SIZE,
            Constants.PIPE_BUFFER_SIZE,
            Constants.PIPE_TIMEOUT,
            IntPtr.Zero);
        
        if (_pipeHandle == new IntPtr(-1))
        {
            FileLogging.LogToFile("Failed to create named pipe.", "server", "error");
            MessageBox.Show(
                "Create named pipe failed", 
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            return;
        }
        
        bool isConnected = PipesImports.ConnectNamedPipe(_pipeHandle, IntPtr.Zero);

        if (!isConnected)
        {
            FileLogging.LogToFile("Failed to connect named pipe.", "server", "error");
            MessageBox.Show(
                "Connect named pipe failed", 
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            return;
        }

        FileLogging.LogToFile("Server started. Waiting for connection...", "server");
        
        Task.Run(ReceiveMessages);
    }

    public void StartClient()
    {
        _serverEvent = CommonImports.OpenEvent(
            Constants.EVENT_ALL_ACCESS,
            false,
            Constants.SERVER_MESSAGE_EVENT_NAME);
        
        _clientEvent = CommonImports.OpenEvent(
            Constants.EVENT_ALL_ACCESS,
            false,
            Constants.CLIENT_MESSAGE_EVENT_NAME);
        
        if (_serverEvent == new IntPtr(-1)
            || _clientEvent == new IntPtr(-1))
        {
            FileLogging.LogToFile("Failed to open server or client event.", "client", "error");
            MessageBox.Show(
                "Open event failed", 
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            
            return;
        }
        
        _pipeHandle = CommonImports.CreateFile(
            Constants.PIPE_NAME,
            Constants.GENERIC_READ | Constants.GENERIC_WRITE,
            0,
            IntPtr.Zero,
            Constants.OPEN_EXISTING,
            0,
            IntPtr.Zero);
        
        if (_pipeHandle == new IntPtr(-1))
        {
            FileLogging.LogToFile("Failed to open pipe.", "client", "error");
            MessageBox.Show(
                "Create file failed", 
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            return;
        }
        
        FileLogging.LogToFile("Connected to server.");
        
        Task.Run(ReceiveMessages);
    }

    public void SendMessage(string text)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(text);
        uint bytesWritten;
        
        bool fWrite = CommonImports.WriteFile(
            _pipeHandle,
            buffer,
            (uint)buffer.Length,
            out bytesWritten,
            IntPtr.Zero);
        
        if (!fWrite)
        {
            FileLogging.LogToFile("Failed to send message.", _isServer ? "server" : "client", "error");
            MessageBox.Show(
                "Write file failed",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            return;
        }

        CommonImports.SetEvent(_isServer ? _serverEvent : _clientEvent);
        
        Application.Current.Dispatcher.Invoke(() => Messages.Add(new Message(text)));
    }

    public void ReceiveMessages()
    {
        while (true)
        {
            byte[] buffer = new byte[Constants.PIPE_BUFFER_SIZE];
            uint bytesRead;
            
            uint result = CommonImports.WaitForSingleObject(
                _isServer ? _clientEvent : _serverEvent,
                Constants.INFINITE);
            bool fRead = CommonImports.ReadFile(
                _pipeHandle,
                buffer,
                (uint)buffer.Length,
                out bytesRead,
                IntPtr.Zero);

            if (!fRead || bytesRead == 0)
            {
                FileLogging.LogToFile(
                    "Disconnected from another process.",
                    _isServer ? "server" : "client",
                    "error");
                MessageBox.Show(
                    "Another process disconnected",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            string receivedText = Encoding.UTF8.GetString(buffer, 0, (int)bytesRead);
            Application.Current.Dispatcher.Invoke(() => Messages.Add(new Message(receivedText, true)));
            MessageReceivedEvent.Invoke();
        }
    }

    public void Cleanup()
    {
        if (_pipeHandle != new IntPtr(-1) && _pipeHandle != IntPtr.Zero)
        {
            CommonImports.CloseHandle(_pipeHandle);
        }
        
        if (_serverEvent != new IntPtr(-1) && _serverEvent != IntPtr.Zero)
        {
            CommonImports.CloseHandle(_serverEvent);
        }
        
        if (_clientEvent != new IntPtr(-1) && _clientEvent != IntPtr.Zero)
        {
            CommonImports.CloseHandle(_clientEvent);
        }
    }
}