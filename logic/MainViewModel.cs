namespace logic;

public class MainViewModel: ObservableObject
{
    public RelayCommand? SocketsViewCommand { get; set; }
    public RelayCommand? PipesViewCommand { get; set; }
    public RelayCommand? MappingViewCommand { get; set; }
    public SocketsViewModel SocketsVM { get; set; }
    public PipesViewModel PipesVM { get; set; }
    public MappingViewModel MappingVM { get; set; }
    private object? _currentView;
    public object CurrentView
    {
        get { return _currentView; }
        set
        {
            _currentView = value;
            OnPropertyChanged();
        }
    }
    public MainViewModel()
    {
        SocketsVM = new SocketsViewModel();
        PipesVM = new PipesViewModel();
        MappingVM = new MappingViewModel();
        CurrentView = SocketsVM;
        SocketsViewCommand = new RelayCommand(o =>
        {
            CurrentView = SocketsVM;
        });
        PipesViewCommand = new RelayCommand(o =>
        {
            CurrentView = PipesVM;
        });
        MappingViewCommand = new RelayCommand(o =>
        {
            CurrentView = MappingVM;
        });
    }
}