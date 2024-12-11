using System.IO;
using System.Windows;

namespace logic;

public static class FileLogging
{
    public static void LogToFile(string message, string sender = "client", string type = "message")
    {
        string filePath = string.Empty;
        string relativePath = "../../../../";
        string content = string.Empty;
        string receiver = sender == "client" ? "server" : "client";
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        if (type == "message")
        {
            filePath = $"{relativePath}{Constants.LOG_SUCCESS_FILE_NAME}";
            content = $"[{timestamp}] Message ({sender}->{receiver}): {message}";
        };
        if(type == "error")
        {
            filePath = $"{relativePath}{Constants.LOG_ERROR_FILE_NAME}";
            content = $"[{timestamp}] Error (on {sender}): {message}";
        };
        try
        {
            File.AppendAllText(filePath, content + Environment.NewLine);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}