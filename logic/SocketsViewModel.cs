using System.Runtime.InteropServices;
using System.Text;

namespace logic;

public class SocketsViewModel : ObservableObject
{
    private const int AF_INET = 2; // IPv4
    private const int SOCK_STREAM = 1; // TCP
    private const int IPPROTO_TCP = 6;

    private IntPtr serverSocket = IntPtr.Zero;
    private IntPtr clientSocket = IntPtr.Zero;

    public List<Message> Messages { get; private set; } = new List<Message>();

    // WinAPI socket functions
    [DllImport("Ws2_32.dll", SetLastError = true)] private static extern IntPtr socket(int af, int type, int protocol);
    [DllImport("Ws2_32.dll", SetLastError = true)] private static extern int bind(IntPtr s, ref sockaddr_in addr, int namelen);
    [DllImport("Ws2_32.dll", SetLastError = true)] private static extern int listen(IntPtr s, int backlog);
    [DllImport("Ws2_32.dll", SetLastError = true)] private static extern IntPtr accept(IntPtr s, ref sockaddr_in addr, ref int addrlen);
    [DllImport("Ws2_32.dll", SetLastError = true)] private static extern int connect(IntPtr s, ref sockaddr_in addr, int namelen);
    [DllImport("Ws2_32.dll", SetLastError = true)] private static extern int send(IntPtr s, byte[] buf, int len, int flags);
    [DllImport("Ws2_32.dll", SetLastError = true)] private static extern int recv(IntPtr s, byte[] buf, int len, int flags);
    [DllImport("Ws2_32.dll", SetLastError = true)] private static extern int closesocket(IntPtr s);
    [DllImport("Ws2_32.dll", SetLastError = true)] private static extern int WSAStartup(ushort wVersionRequested, out WSAData wsaData);
    [DllImport("Ws2_32.dll", SetLastError = true)] private static extern int WSACleanup();

    // WinAPI structures
    [StructLayout(LayoutKind.Sequential)]
    private struct sockaddr_in
    {
        public short sin_family;
        public ushort sin_port;
        public uint sin_addr;
        public long sin_zero;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct WSAData
    {
        public ushort wVersion;
        public ushort wHighVersion;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 257)] public string szDescription;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 129)] public string szSystemStatus;
        public ushort iMaxSockets;
        public ushort iMaxUdpDg;
        public IntPtr lpVendorInfo;
    }

    // Start server
    public void StartServer(string ipAddress, int port)
    {
        InitWinSock();

        serverSocket = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
        if (serverSocket == IntPtr.Zero)
            throw new Exception("Failed to create server socket.");

        var serverAddr = new sockaddr_in
        {
            sin_family = AF_INET,
            sin_port = htons((ushort)port),
            sin_addr = inet_addr(ipAddress),
        };

        if (bind(serverSocket, ref serverAddr, Marshal.SizeOf(serverAddr)) != 0)
            throw new Exception("Failed to bind server socket.");

        if (listen(serverSocket, 10) != 0)
            throw new Exception("Failed to listen on server socket.");

        Messages.Add(new Message("Server started. Waiting for connections..."));

        Task.Run(() => AcceptClients());
    }

    // Accept client connections
    private void AcceptClients()
    {
        var clientAddr = new sockaddr_in();
        int addrSize = Marshal.SizeOf(clientAddr);

        while (true)
        {
            IntPtr clientSocket = accept(serverSocket, ref clientAddr, ref addrSize);
            if (clientSocket != IntPtr.Zero)
            {
                Messages.Add(new Message("Client connected. Waiting for connections..."));
                Task.Run(() => ReceiveMessages(clientSocket));
            }
        }
    }

    // Start client
    public void StartClient(string ipAddress, int port)
    {
        InitWinSock();

        clientSocket = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
        if (clientSocket == IntPtr.Zero)
            throw new Exception("Failed to create client socket.");

        var serverAddr = new sockaddr_in
        {
            sin_family = AF_INET,
            sin_port = htons((ushort)port),
            sin_addr = inet_addr(ipAddress),
        };

        if (connect(clientSocket, ref serverAddr, Marshal.SizeOf(serverAddr)) != 0)
            throw new Exception("Failed to connect to server.");

        Messages.Add(new Message("Connected to server."));
        Task.Run(() => ReceiveMessages(clientSocket));
    }

    // Send message
    public void SendMessage(string message)
    {
        IntPtr socket = clientSocket != IntPtr.Zero ? clientSocket : serverSocket;
        if (socket == IntPtr.Zero)
        {
            Messages.Add(new Message("Failed to send message."));
            return;
        }

        byte[] buffer = Encoding.UTF8.GetBytes(message);
        if (send(socket, buffer, buffer.Length, 0) <= 0)
            throw new Exception("Failed to send message.");

        Messages.Add(new Message(message, false));
    }

    // Receive messages
    private void ReceiveMessages(IntPtr socket)
    {
        byte[] buffer = new byte[1024];

        while (true)
        {
            int bytesRead = recv(socket, buffer, buffer.Length, 0);
            if (bytesRead <= 0)
            {
                Messages.Add(new Message("Failed to receive messages."));
                break;
            }

            string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Messages.Add(new Message(message, true));
        }
    }

    // Helper functions
    private void InitWinSock()
    {
        ushort version = 0x0202; // Winsock 2.2
        if (WSAStartup(version, out var wsaData) != 0)
            throw new Exception("Failed to initialize Winsock.");
    }

    private static ushort htons(ushort hostshort) => (ushort)((hostshort << 8) | (hostshort >> 8));

    private static uint inet_addr(string ipAddress)
    {
        var segments = ipAddress.Split('.');
        if (segments.Length != 4)
            throw new ArgumentException("Invalid IP address format.");

        uint ip = 0;
        for (int i = 0; i < 4; i++)
        {
            ip |= (uint)(byte.Parse(segments[i]) << (8 * i));
        }

        return ip;
    }

    public void Cleanup()
    {
        if (serverSocket != IntPtr.Zero)
            closesocket(serverSocket);
        if (clientSocket != IntPtr.Zero)
            closesocket(clientSocket);

        WSACleanup();
    }
}