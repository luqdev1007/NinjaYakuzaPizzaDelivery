using Unity.Cinemachine;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Context
{
    public class GameplaySceneContext : MonoBehaviour
    {
        [field: Header("Cameras")]
        [field: SerializeField] public CinemachineCamera IntroCamera { get; private set; }
        [field: SerializeField] public CinemachineCamera ScoutingCamera { get; private set; }
        [field: SerializeField] public CinemachineCamera HeroCamera { get; private set; }

        [field: Header("Points")]
        [field: SerializeField] public Transform StartPoint { get; private set; }
        [field: SerializeField] public Transform FinishPoint { get; private set; }

        // Сериализованный список врагов (_enemies + Enemies + SyncWithScene) убран
        // намеренно. Это была вторая копия данных, которые уже лежат на компонентах
        // EnemySpawnMarker, и копия обновлялась только ручным нажатием кнопки SYNC.
        // Любой пропуск синхронизации давал тихий рассинхрон: расстановка в префабе
        // говорила одно, рантайм спавнил другое, ошибок при этом не было.
        // Теперь GameplayBootstrap читает маркеры напрямую с инстанса уровня —
        // единственный источник истины, синхронизировать нечего.
    }
}
