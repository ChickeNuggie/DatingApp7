using API.DTO;
using API.Entities;

namespace API.Interfaces
{
    public interface IPhotoRepository
    {
        //Task: To break down a larger feature or project into smaller, more manageable pieces: By creating tasks for specific parts of a project,
        // creating tasks and assigning them priorities, you can ensure that the most important work is completed first.
        //To track progress or To facilitate communication about specific pieces of work
        Task<IEnumerable<PhotoForApprovalDto>> GetUnapprovedPhotos();
        Task<Photo> GetPhotoById(int id);
        void RemovedPhoto(Photo photo);

    }
}