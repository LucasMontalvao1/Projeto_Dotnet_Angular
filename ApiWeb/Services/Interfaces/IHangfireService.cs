using Hangfire;

namespace ApiWeb.Services.Interfaces
{
    public interface IHangfireService
    {
        void EnqueueJob();

        void ExecuteJob();
    }
}
