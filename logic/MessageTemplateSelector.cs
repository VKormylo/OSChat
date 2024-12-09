using System.Windows;
using System.Windows.Controls;

namespace logic;

public class MessageTemplateSelector : DataTemplateSelector
{
    public DataTemplate? SentTemplate { get; set; }
    public DataTemplate? ReceivedTemplate { get; set; }

    public override DataTemplate? SelectTemplate(object? item, DependencyObject container)
    {
        if (item is Message message)
        {
            return message.IsReceived ? ReceivedTemplate : SentTemplate;
        }
        return base.SelectTemplate(item, container);
    }
}
