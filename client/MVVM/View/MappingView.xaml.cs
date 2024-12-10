using System.Windows.Controls;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System;
using logic;

namespace client.MVVM.View;

public partial class MappingView : UserControl
{
   
    private readonly MappingViewModel _mappingViewModel = MappingViewModel.Instance;
    public MappingView()
    {
        InitializeComponent();
        DataContext = MappingViewModel.Instance;
        MappingViewModel.Instance.MessageReceivedEvent += onReceieved;
        
        Task.Run(() =>
        { 
            MappingViewModel.Instance.StartClient();
        });
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
        MappingViewModel.Instance.Text= MessageInput.Text;
        if (!string.IsNullOrEmpty(MappingViewModel.Instance.Text))
        {
            try
            {
                MappingViewModel.Instance.SendMessage(MappingViewModel.Instance.clientEvent);
                MessageInput.Text = "";
                MessagesContainer.ScrollIntoView(MessagesContainer.Items[MessagesContainer.Items.Count - 1]);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error sending message: {ex.Message}");
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
    
    public void onReceieved()
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            MessagesContainer.ScrollIntoView(MessagesContainer.Items[MessagesContainer.Items.Count - 1]);
        });

    }
}