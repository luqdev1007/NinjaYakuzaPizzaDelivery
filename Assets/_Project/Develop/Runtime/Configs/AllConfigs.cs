using Assets._Project.Develop.Runtime.Configs.Dialog;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Loot;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Style;
using Assets._Project.Develop.Runtime.Configs.Meta.Stats;
using Assets._Project.Develop.Runtime.Configs.Meta.Wallet;
using Assets._Project.Develop.Runtime.Gameplay.Features.StyleFeature;
using Assets._Project.Develop.Runtime.Utilites.AudioManagement;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs
{
    [CreateAssetMenu(fileName = "AllConfigs", menuName = "Configs/AllConfigs")]
    public class AllConfigs : ScriptableObject
    {
        [Header("Meta & Economics")]
        [Tooltip("Стартовое состояние кошелька игрока.")]
        [field: SerializeField] public StartWalletConfig Wallet { get; private set; }

        [Tooltip("Иконки для различных типов валют.")]
        [field: SerializeField] public CurrencyIconsConfig CurrencyIcons { get; private set; }

        [Tooltip("Настройки наград за победы и штрафов за поражения.")]
        [field: SerializeField] public GameRewardsConfig Rewards { get; private set; }

        [Header("Gameplay Systems")]
        [Tooltip("Список всех уровней и их настройки.")]
        [field: SerializeField] public LevelsListConfig Levels { get; private set; }

        [Tooltip("Глобальный провайдер лута (враги, сундуки, пропсы).")]
        [field: SerializeField] public MasterLootProviderConfig Loot { get; private set; }

        [Tooltip("Настройки рангов стиля (D, C, B, A, S...).")]
        [field: SerializeField] public StyleRankConfig StyleRanks { get; private set; }

        [Tooltip("Настройки очков стиля за конкретные действия.")]
        [field: SerializeField] public StyleActionsConfig StyleActions { get; private set; }

        [Header("Entities")]
        [Tooltip("Конфиг главного героя со всеми механиками передвижения и боя.")]
        [field: SerializeField] public MainHeroConfig Hero { get; private set; }

        [Tooltip("Конфиг базового противника-призрака.")]
        [field: SerializeField] public GhostConfig GhostEnemy { get; private set; }

        [Header("Narrative & Dialogs")]
        [Tooltip("Данные всех персонажей (имена, портреты).")]
        [field: SerializeField] public CharactersConfig Characters { get; private set; }

        [Tooltip("Конфиг обучающего диалога.")]
        [field: SerializeField] public DialogConfig TutorialDialog { get; private set; }

        [Header("Audio")]
        [Tooltip("Глобальные настройки звуков и музыки.")]
        [field: SerializeField] public AudioConfig Audio { get; private set; }
    }
}