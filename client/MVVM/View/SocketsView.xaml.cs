using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using logic;

namespace client.MVVM.View;

public partial class SocketsView : UserControl
{
    private readonly SocketsViewModel _socketsViewModel = SocketsViewModel.Instance;
    public SocketsView()
    {
        InitializeComponent();
        DataContext = _socketsViewModel;
        _socketsViewModel.MessageReceivedEvent += OnMessageReceived;
        
        Task.Run(() =>
        { 
            StartClient();
        });
    }

    private void StartClient()
    {
        string ipAddress = "127.0.0.1";
        int port = 80;
        
        try
        {
            SocketsViewModel.Instance.StartClient(ipAddress, port);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }
    
    private void MessageInput_OnGotFocus(object sender, RoutedEventArgs e)
    {
        if (MessageInput.Text == "Type your message here...")
        {
            MessageInput.Text = string.Empty;
        }
    }

    private void MessageInput_OnLostFocus(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(MessageInput.Text))
        {
            MessageInput.Text = "Type your message here...";
        }
    }

    private void SendMessage()
    {
        string message = MessageInput.Text;
        if (!string.IsNullOrEmpty(message))
        {
            try
            {
                SocketsViewModel.Instance.SendMessage(message, true);
                FileLogging.LogToFile(message);
                MessageInput.Text = "";
                MessagesContainer.ScrollIntoView(MessagesContainer.Items[MessagesContainer.Items.Count - 1]);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error sending message: {ex.Message}");
                FileLogging.LogToFile($"Error sending message: {ex.Message}", "client", "error");
            }
        }
    }

    private void SendMessageBtn_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        SendMessage();
    }

    private void MessageInput_OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            SendMessage();
        }
    }
    
    private void OnMessageReceived()
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            MessagesContainer.ScrollIntoView(MessagesContainer.Items[MessagesContainer.Items.Count - 1]);
        });
    }
}