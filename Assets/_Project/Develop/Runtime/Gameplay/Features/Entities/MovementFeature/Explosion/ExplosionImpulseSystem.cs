using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Explosion
{
    /// <summary>
    /// Применяет импульс от взрыва к rigidbody своей сущности.
    /// </summary>
    /// <remarks>
    /// ИМПУЛЬС АДДИТИВНЫЙ. Скорость не обнуляется НИ ПО ОДНОЙ ОСИ — это
    /// принципиальное отличие от BounceSystem, и оно намеренное.
    ///
    /// BounceSystem реализует семантику ЗАМЕНЫ: гасит компоненту скорости вдоль
    /// UpAxis и ставит на её место LaunchVelocity, потому что трамплин задаёт
    /// предсказуемую высоту отскока — она не должна зависеть от того, с какой
    /// скоростью игрок на него прилетел.
    ///
    /// У взрыва задача обратная. Игрока, уже летящего от предыдущего взрыва или
    /// разогнанного дэшем, второй взрыв обязан ДОРАЗОГНАТЬ, а не подрезать. Любое
    /// обнуление здесь читалось бы как торможение в момент, когда игра обещает
    /// ускорение, и прямо ломало бы дизайн-пиллар «скорость = урон»: цепочка
    /// подрывов должна складываться в разгон, а не сбрасывать его в ноль на каждом
    /// звене. Поэтому — чистый AddForce поверх текущей скорости.
    ///
    /// Тикового канала нет намеренно, по образцу BounceSystem и
    /// DamageKnockbackSystem: применяем прямо в колбэке запроса. Источник
    /// (ExplosionSystem призрака) стреляет из своего колбэка детонации, системы
    /// движения к этому моменту своё OnFixedUpdate уже отработали. Откладывание до
    /// следующего OnFixedUpdate дало бы лишний физ-шаг задержки между вспышкой
    /// взрыва и отбросом — заметный разрыв в ощущении удара.
    /// </remarks>
    public class ExplosionImpulseSystem : IInitializableSystem, IDisposableSystem
    {
        private Rigidbody2D _rigidbody;

        private IDisposable _requestDisposable;

        public void OnInit(Entity entity)
        {
            _rigidbody = entity.Rigidbody;

            _requestDisposable = entity.ExplosionImpulseRequest.Subscribe(OnExplosionImpulseRequest);
        }

        private void OnExplosionImpulseRequest(Vector2 force)
        {
            if (_rigidbody == null)
            {
                return;
            }

            _rigidbody.AddForce(force, ForceMode2D.Impulse);
        }

        public void OnDispose()
        {
            _requestDisposable?.Dispose();
        }
    }
}
