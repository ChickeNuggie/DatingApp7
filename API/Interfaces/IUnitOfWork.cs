namespace API.Interfaces
{
//Unit of work is similar to a transaction where it save async and update everything simalteanousely than individual repository which may be confusing.
//i.e. if one repo update fail, all repo update fails.
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        IMessageRepository MessageRepository { get; }
        ILikesRepository LikesRepository { get; }
        IPhotoRepository PhotoRepository { get; }
        Task<bool> Complete(); // equivalent to save aync method.
        bool HasChanges(); // inform us if entity framework is tracking any changes inside its transaction.
    }
}