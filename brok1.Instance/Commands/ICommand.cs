namespace brok1.Instance.Commands
{
    public interface ICommand<T>
    {
        public Task Execute();
    }
}
