using Assets._Project.Develop.Runtime.Configs.Gameplay.Style;
using System;
using System.Collections.Generic;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.StyleFeature
{
    public class StyleEvaluator : IDisposable
    {
        private readonly RankStyleService _styleService;
        private readonly StyleActionsConfig _config;

        private readonly List<StyleActionType> _usedActionsHistory = new();

        public StyleEvaluator(RankStyleService styleService, StyleActionsConfig config)
        {
            _styleService = styleService;
            _config = config;
        }

        public void ProcessHit(StyleActionType attackType, bool isLethal)
        {
            float points = GetBasePoints(attackType);

            if (isLethal)
            {
                points += _config.KillBonus;
            }

            RegisterAction(attackType, points);
        }

        public void ProcessDash()
        {
            RegisterAction(StyleActionType.Dash, _config.DashPoints);
        }

        public void ProcessWallJump()
        {
            RegisterAction(StyleActionType.WallJump, _config.WallJumpPoints);
        }

        public void ProcessWallHangAttach()
        {
            RegisterAction(StyleActionType.WallHangAttach, _config.WallHangAttachPoints);
        }

        public void ProcessGrappleAttach()
        {
            RegisterAction(StyleActionType.GrappleAttach, _config.GrappleAttachPoints);
        }

        public void ProcessPlungeSlam()
        {
            RegisterAction(StyleActionType.PlungeSlam, _config.PlungeSlamPoints);
        }

        public void ProcessPlayerHit()
        {
            _styleService.ApplyDamagePenalty();
            _usedActionsHistory.Clear();
        }

        private void RegisterAction(StyleActionType type, float rawPoints)
        {
            bool isDirectRepeat = _usedActionsHistory.Count > 0
                && _usedActionsHistory[_usedActionsHistory.Count - 1] == type;

            if (isDirectRepeat)
            {
                return;
            }

            float multiplier = _usedActionsHistory.Contains(type) ? 1f : _config.DiversityMultiplier;
            _styleService.AddPoints(rawPoints * multiplier);
            PushHistory(type);
        }

        private void PushHistory(StyleActionType type)
        {
            _usedActionsHistory.Add(type);

            if (_usedActionsHistory.Count > _config.DiversityHistorySize)
            {
                _usedActionsHistory.RemoveAt(0);
            }
        }

        private float GetBasePoints(StyleActionType type)
        {
            switch (type)
            {
                case StyleActionType.LightAttack:
                    return _config.LightAttackPoints;
                case StyleActionType.SpeedDamage:
                    return _config.SpeedDamagePoints;
                default:
                    return 0f;
            }
        }

        public void Dispose()
        {
            _usedActionsHistory.Clear();
        }
    }
}