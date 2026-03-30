using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.AudioManagement;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.MainHero
{
    public class IsMainHero : IEntityComponent
    {
    }

    public class AudioComponent : IEntityComponent
    {
        public AudioService Service;
    }
}
