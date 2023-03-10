using _Client_.Scripts.Components;
using _Client_.Scripts.Models;
using _Client_.Scripts.Services.Skins;
using _Client_.Scripts.Systems;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.ExtendedSystems;
using SFramework.Core.Runtime;
using SFramework.ECS.Runtime;
using SFramework.UI.Runtime;
using UnityEngine;

namespace _Client_.Scripts
{
    public sealed class _ContextRoot : SFContextRoot
    {
        private IEcsSystems _systems;

        [SerializeField]
        private ScriptableObject[] _data;

        protected override void PreInit()
        {
            Input.multiTouchEnabled = true;
            Application.targetFrameRate = Screen.currentResolution.refreshRate;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        protected override void Setup(SFContainer container)
        {
            foreach (var data in _data)
            {
                if (data == null) continue;
                container.Bind(data);
            }
            
            container.Bind<ISFWorldsService>(new SFWorldsService());
            container.Bind<ISFUIService>(new SFUIService());
            container.Bind<ISkinsService>(new SkinsService());
            container.Bind<SessionModel>(new SessionModel());
        }

        protected override void Init(ISFContainer container)
        {
            var _world = container.Resolve<ISFWorldsService>().Default;
            _systems = new EcsSystems(_world, container);
            _systems
                .Add(new GameStartSystem())
                .Add(new WallsResizeSystem())
                .Add(new PaddleAIControlSystem())
                .Add(new PaddleDesktopControlSystem())
                .Add(new PaddleMobileControlSystem())
                .Add(new PaddleCollisionSystem())
                .Add(new PaddleReflectionSystem())
                .Add(new SpeedTweenSystem())
                .Add(new WallReflectionSystem())
                .Add(new BallMovementSystem())
                .Add(new GoalCheckSystem())
                .Add(new GameStopSystem())

#if UNITY_EDITOR
                .Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem())
#endif
                .DelHere<InputSpeed>()
                .Inject()
                .Init();
        }

        private void Update()
        {
            _systems?.Run();
        }

        private void OnDestroy()
        {
            if (_systems != null)
            {
                _systems.Destroy();
                _systems = null;
            }
        }
    }
}