using Codebase.Services;
using Codebase.Services.Inputs;
using UnityEngine.EventSystems;
using UnityEngine;
using Zenject;

namespace Codebase.Installers
{
    public class GlobalInstaller : MonoInstaller
    {
        [SerializeField]
        private EventSystem _eventSystem;
        [SerializeField]
        private AudioService _audioService;

        public override void InstallBindings()
        {
            Container.Bind<SceneService>().FromNew().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<DesktopInput>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<CursorToggle>().AsSingle().NonLazy();
            Container.Bind<AudioService>().FromComponentInNewPrefab(_audioService).AsSingle().NonLazy();
            Container.Bind<EventSystem>().FromComponentInNewPrefab(_eventSystem).AsSingle().NonLazy();
        }
    }
}

