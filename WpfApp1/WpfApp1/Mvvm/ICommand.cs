namespace WpfApp1.Mvvm
{
    public interface ICommand
    {
        string Name { get; }
        void Execute();
        void UnExecute();
    }
}
