using System.Collections.ObjectModel;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using client;

namespace logic;

public class SocketsViewModel
{
    private TcpClient _client;
    private TcpListener _server;
    private NetworkStream _stream;

    public ObservableCollection<Message> Messages { get; set; } = new ObservableCollection<Message>();


    public SocketsViewModel() {}

        public async Task ConnectToServer(string serverIp, int port)
        {
            try
            {
                _client = new TcpClient();
                await _client.ConnectAsync(serverIp, port);
                _stream = _client.GetStream();

                await Task.Run(() => ReceiveMessages());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка підключення до сервера: {ex.Message}");
            }
        }

        public async Task StartServer(int port)
        {
            try
            {
                _server = new TcpListener(System.Net.IPAddress.Any, port);
                _server.Start();
                _client = await _server.AcceptTcpClientAsync();
                _stream = _client.GetStream();

                await Task.Run(() => ReceiveMessages());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка запуску сервера: {ex.Message}");
            }
        }

        public async Task SendMessage(string text)
        {
            try
            {
                if (_stream != null)
                {
                    var message = new Message(text, false);

                    App.Current.Dispatcher.Invoke(() => Messages.Add(message));

                    byte[] data = Encoding.UTF8.GetBytes(text);
                    await _stream.WriteAsync(data, 0, data.Length);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка відправлення повідомлення: {ex.Message}");
            }
        }

        private async Task ReceiveMessages()
        {
            try
            {
                while (true)
                {
                    if (_stream != null)
                    {
                        byte[] buffer = new byte[1024];
                        int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length);

                        if (bytesRead > 0)
                        {
                            string messageText = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                            var message = new Message(messageText, true);

                            App.Current.Dispatcher.Invoke(() => Messages.Add(message));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка отримання повідомлень: {ex.Message}");
            }
        }
}