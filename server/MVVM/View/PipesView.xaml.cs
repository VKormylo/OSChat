using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using logic;
using logic.imports;

namespace server.MVVM.View;

public partial class PipesView : UserControl
{
    
    private readonly PipesViewModel _pipesViewModel = PipesViewModel.Instance;
    
    public PipesView()
    {
        InitializeComponent();
        DataContext = _pipesViewModel;
        _pipesViewModel.MessageReceivedEvent += OnMessageReceived;

        Task.Run(_pipesViewModel.StartServer);
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
        string text = MessageInput.Text;
        if (!string.IsNullOrEmpty(text))
        {
            _pipesViewModel.SendMessage(text);
            MessageInput.Text = "";
            listBox.ScrollIntoView(listBox.Items[listBox.Items.Count - 1]);
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
            listBox.ScrollIntoView(listBox.Items[listBox.Items.Count - 1]);
        });
    }
}