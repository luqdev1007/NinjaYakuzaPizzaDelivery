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
        private readonly Dictionary<StyleActionType, float> _cooldowns = new();
        private readonly List<StyleActionType> _expiredCooldownBuffer = new();
        private readonly List<StyleActionType> _cooldownKeysBuffer = new();

        public StyleEvaluator(RankStyleService styleService, StyleActionsConfig config)
        {
            _styleService = styleService;
            _config = config;
        }

        public void Tick(float deltaTime)
        {
            if (_cooldowns.Count == 0)
            {
                return;
            }

            _cooldownKeysBuffer.Clear();

            foreach (StyleActionType key in _cooldowns.Keys)
            {
                _cooldownKeysBuffer.Add(key);
            }

            _expiredCooldownBuffer.Clear();

            foreach (StyleActionType key in _cooldownKeysBuffer)
            {
                float remaining = _cooldowns[key] - deltaTime;

                if (remaining <= 0f)
                {
                    _expiredCooldownBuffer.Add(key);
                }
                else
                {
                    _cooldowns[key] = remaining;
                }
            }

            foreach (StyleActionType expiredType in _expiredCooldownBuffer)
            {
                _cooldowns.Remove(expiredType);
            }
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
            float multiplier;

            if (_cooldowns.ContainsKey(type))
            {
                multiplier = _config.MinDiversityMultiplier;
            }
            else if (_usedActionsHistory.Contains(type))
            {
                multiplier = 1f;
            }
            else
            {
                multiplier = _config.DiversityMultiplier;
            }

            _styleService.AddPoints(rawPoints * multiplier);

            _cooldowns[type] = _config.RepeatCooldownSeconds;
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
            _cooldowns.Clear();
        }
    }
}