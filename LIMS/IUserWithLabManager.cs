namespace LIMS
{
    public interface IUserWithLabManager
    {
        string UserId { get; }
        string UserName { get; }
        bool IsLabManager { get; }
    }
}
