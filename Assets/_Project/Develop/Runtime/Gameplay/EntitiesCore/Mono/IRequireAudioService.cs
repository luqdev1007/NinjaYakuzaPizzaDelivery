using Assets._Project.Develop.Runtime.Utilities.AudioManagment;

namespace Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono
{
    public interface IRequireAudioService
    {
        void Construct(IAudioService audioService);
    }
}