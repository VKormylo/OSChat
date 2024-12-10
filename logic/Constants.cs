namespace logic;

public static class Constants
{
    public const string FILE_NAME = "FileMapping";
    public const uint PAGE_READWRITE = 0x04;
    public const uint FILE_SIZE = 1024;
    public const uint FILE_MAP_ALL_ACCESS = 0xF001F;
    public const uint INFINITE = 0xFFFFFFFF;
    public const string ServerEventName = "ServerEvent";
    public const string ClientEventName = "ClientEvent";

    public const uint PIPE_ACCESS_DUPLEX = 0x00000003;
    public const uint PIPE_READMODE_MESSAGE = 0x00000002;
    public const uint PIPE_TYPE_MESSAGE = 0x00000004;
    public const uint PIPE_WAIT = 0x00000000;
    public const uint PIPE_UNLIMITED_INSTANCES = 255;
    public const string PIPE_NAME = @"\\.\pipe\MyPipe";
    public const uint PIPE_TIMEOUT = 5000;
    public const uint PIPE_BUFFER_SIZE = 1024;
    public const uint GENERIC_READ = 0x80000000;
    public const uint GENERIC_WRITE = 0x40000000;
    public const uint OPEN_EXISTING = 3;
    public const uint EVENT_ALL_ACCESS = 0x001F0003;
    public const string SERVER_MESSAGE_EVENT_NAME = "SERVER_MESSAGE_EVENT";
    public const string CLIENT_MESSAGE_EVENT_NAME = "CLIENT_MESSAGE_EVENT";
    
    public const string LOG_ERROR_FILE_NAME = "log-errors.txt";
    public const string LOG_SUCCESS_FILE_NAME = "log-messages.txt";
}