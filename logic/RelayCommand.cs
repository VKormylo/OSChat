using System.Windows.Input;
namespace logic;

public class RelayCommand : ICommand
{
    // Create fields for executing command
    private Action<object> _execute;
    private Func<object, bool> _canExecute;

    public event EventHandler? CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove {  CommandManager.RequerySuggested -= value; }
    }
    // Creating RelayCommand to trigger commands from UI elements
    public RelayCommand(Action<object> execute, Func<object, bool> canExecute)
    {
        _execute = execute;
        _canExecute = canExecute;
    }
    // Check if command can be executed
    public bool CanExecute(object? parameter)
    {
        return _canExecute == null || CanExecute(parameter);
    }
    // Execute command
    public void Execute(object? parameter)
    {
        _execute(parameter);
    }
}