using System.IO;
using System.Windows;

namespace logic;

public static class FileLogging
{
    public static void LogToFile(string message, string receiver = "client", string type = "message")
    {
        string filePath = string.Empty;
        string relativePath = "../../../../";
        string content = string.Empty;
        string sender = receiver == "client" ? "server" : "client";
        if (type == "message")
        {
            filePath = $"{relativePath}{Constants.LOG_SUCCESS_FILE_NAME}";
            content = $"Message ({sender}->{receiver}): {message}";
        };
        if(type == "error")
        {
            filePath = $"{relativePath}{Constants.LOG_ERROR_FILE_NAME}";
            content = $"Error ({sender}->{receiver}): {message}";
        };
        try
        {
            File.AppendAllText(filePath, content + Environment.NewLine);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error logging to file: {ex.Message}");
        }
    }
}