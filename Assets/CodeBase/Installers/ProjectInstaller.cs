using Codebase.Services;
using Codebase.Services.Inputs;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Codebase.Installers
{
    public class ProjectInstaller : MonoInstaller
    {
        [SerializeField]
        private EventSystem _eventSystem;
        [SerializeField]
        private AudioService _audioService;

        public override void InstallBindings()
        {
            //Container.Bind<IInputService>().To<DesktopInput>().FromNew().AsSingle().NonLazy();
            Container.Bind<AudioService>().FromComponentInNewPrefab(_audioService).AsSingle().NonLazy();
            Container.Bind<EventSystem>().FromComponentInNewPrefab(_eventSystem).AsSingle().NonLazy();
        }
    }
}