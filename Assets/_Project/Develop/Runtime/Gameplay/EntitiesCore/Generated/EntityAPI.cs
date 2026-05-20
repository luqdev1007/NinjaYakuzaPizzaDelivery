namespace Assets._Project.Develop.Runtime.Gameplay.EntitiesCore
{
	public partial class Entity
	{
		public Assets._Project.Develop.Runtime.Gameplay.Features.TeamsFeature.Team TeamC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.TeamsFeature.Team>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.Features.TeamsFeature.Teams> Team => TeamC.Value;

		public bool TryGetTeam(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.Features.TeamsFeature.Teams> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.TeamsFeature.Team component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.Features.TeamsFeature.Teams>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddTeam()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.TeamsFeature.Team() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.Features.TeamsFeature.Teams>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddTeam(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.Features.TeamsFeature.Teams> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.TeamsFeature.Team() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.SpawnFeature.SpawnInitialTime SpawnInitialTimeC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.SpawnFeature.SpawnInitialTime>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> SpawnInitialTime => SpawnInitialTimeC.Value;

		public bool TryGetSpawnInitialTime(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.SpawnFeature.SpawnInitialTime component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSpawnInitialTime()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.SpawnFeature.SpawnInitialTime() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSpawnInitialTime(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.SpawnFeature.SpawnInitialTime() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.SpawnFeature.SpawnCurrentTime SpawnCurrentTimeC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.SpawnFeature.SpawnCurrentTime>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> SpawnCurrentTime => SpawnCurrentTimeC.Value;

		public bool TryGetSpawnCurrentTime(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.SpawnFeature.SpawnCurrentTime component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSpawnCurrentTime()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.SpawnFeature.SpawnCurrentTime() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSpawnCurrentTime(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.SpawnFeature.SpawnCurrentTime() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.SpawnFeature.InSpawnProcess InSpawnProcessC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.SpawnFeature.InSpawnProcess>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> InSpawnProcess => InSpawnProcessC.Value;

		public bool TryGetInSpawnProcess(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.SpawnFeature.InSpawnProcess component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddInSpawnProcess()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.SpawnFeature.InSpawnProcess() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddInSpawnProcess(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.SpawnFeature.InSpawnProcess() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.SpawnFeature.SpawnEvent SpawnEventC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.SpawnFeature.SpawnEvent>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent SpawnEvent => SpawnEventC.Value;

		public bool TryGetSpawnEvent(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.SpawnFeature.SpawnEvent component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSpawnEvent()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.SpawnFeature.SpawnEvent() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSpawnEvent(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.SpawnFeature.SpawnEvent() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.BodyCollider BodyColliderC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.BodyCollider>();

		public UnityEngine.Collider2D BodyCollider => BodyColliderC.Value;

		public bool TryGetBodyCollider(out UnityEngine.Collider2D value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.BodyCollider component);
			if (result)
				value = component.Value;
			else
				value = default(UnityEngine.Collider2D);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddBodyCollider(UnityEngine.Collider2D value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.BodyCollider() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.GroundMask GroundMaskC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.GroundMask>();

		public UnityEngine.LayerMask GroundMask => GroundMaskC.Value;

		public bool TryGetGroundMask(out UnityEngine.LayerMask value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.GroundMask component);
			if (result)
				value = component.Value;
			else
				value = default(UnityEngine.LayerMask);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddGroundMask(UnityEngine.LayerMask value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.GroundMask() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.ContactsDetectingMask ContactsDetectingMaskC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.ContactsDetectingMask>();

		public UnityEngine.LayerMask ContactsDetectingMask => ContactsDetectingMaskC.Value;

		public bool TryGetContactsDetectingMask(out UnityEngine.LayerMask value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.ContactsDetectingMask component);
			if (result)
				value = component.Value;
			else
				value = default(UnityEngine.LayerMask);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddContactsDetectingMask(UnityEngine.LayerMask value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.ContactsDetectingMask() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.ContactCollidersBuffer ContactCollidersBufferC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.ContactCollidersBuffer>();

		public Assets._Project.Develop.Runtime.Utilities.Buffer<UnityEngine.Collider2D> ContactCollidersBuffer => ContactCollidersBufferC.Value;

		public bool TryGetContactCollidersBuffer(out Assets._Project.Develop.Runtime.Utilities.Buffer<UnityEngine.Collider2D> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.ContactCollidersBuffer component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Buffer<UnityEngine.Collider2D>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddContactCollidersBuffer(Assets._Project.Develop.Runtime.Utilities.Buffer<UnityEngine.Collider2D> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.ContactCollidersBuffer() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.ContactEntitiesBuffer ContactEntitiesBufferC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.ContactEntitiesBuffer>();

		public Assets._Project.Develop.Runtime.Utilities.Buffer<Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity> ContactEntitiesBuffer => ContactEntitiesBufferC.Value;

		public bool TryGetContactEntitiesBuffer(out Assets._Project.Develop.Runtime.Utilities.Buffer<Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.ContactEntitiesBuffer component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Buffer<Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddContactEntitiesBuffer(Assets._Project.Develop.Runtime.Utilities.Buffer<Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.ContactEntitiesBuffer() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.DeathMask DeathMaskC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.DeathMask>();

		public UnityEngine.LayerMask DeathMask => DeathMaskC.Value;

		public bool TryGetDeathMask(out UnityEngine.LayerMask value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.DeathMask component);
			if (result)
				value = component.Value;
			else
				value = default(UnityEngine.LayerMask);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDeathMask(UnityEngine.LayerMask value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.DeathMask() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.IsTouchDeathMask IsTouchDeathMaskC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.IsTouchDeathMask>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> IsTouchDeathMask => IsTouchDeathMaskC.Value;

		public bool TryGetIsTouchDeathMask(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.IsTouchDeathMask component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsTouchDeathMask()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.IsTouchDeathMask() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsTouchDeathMask(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.IsTouchDeathMask() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.IsTouchAnotherTeam IsTouchAnotherTeamC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.IsTouchAnotherTeam>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> IsTouchAnotherTeam => IsTouchAnotherTeamC.Value;

		public bool TryGetIsTouchAnotherTeam(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.IsTouchAnotherTeam component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsTouchAnotherTeam()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.IsTouchAnotherTeam() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsTouchAnotherTeam(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.IsTouchAnotherTeam() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.PhysicsFeature.BaseGravityScale BaseGravityScaleC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.PhysicsFeature.BaseGravityScale>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> BaseGravityScale => BaseGravityScaleC.Value;

		public bool TryGetBaseGravityScale(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.PhysicsFeature.BaseGravityScale component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddBaseGravityScale()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.PhysicsFeature.BaseGravityScale() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddBaseGravityScale(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.PhysicsFeature.BaseGravityScale() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.PhysicsFeature.AngularDrag AngularDragC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.PhysicsFeature.AngularDrag>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> AngularDrag => AngularDragC.Value;

		public bool TryGetAngularDrag(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.PhysicsFeature.AngularDrag component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAngularDrag()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.PhysicsFeature.AngularDrag() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAngularDrag(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.PhysicsFeature.AngularDrag() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.PhysicsFeature.LinearDrag LinearDragC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.PhysicsFeature.LinearDrag>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> LinearDrag => LinearDragC.Value;

		public bool TryGetLinearDrag(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.PhysicsFeature.LinearDrag component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddLinearDrag()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.PhysicsFeature.LinearDrag() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddLinearDrag(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.PhysicsFeature.LinearDrag() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.PhysicsFeature.Velocity VelocityC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.PhysicsFeature.Velocity>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector2> Velocity => VelocityC.Value;

		public bool TryGetVelocity(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector2> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.PhysicsFeature.Velocity component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector2>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddVelocity()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.PhysicsFeature.Velocity() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector2>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddVelocity(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector2> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.PhysicsFeature.Velocity() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.PhysicsFeature.CanPhysicalyInteract CanPhysicalyInteractC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.PhysicsFeature.CanPhysicalyInteract>();

		public Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition CanPhysicalyInteract => CanPhysicalyInteractC.Value;

		public bool TryGetCanPhysicalyInteract(out Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.PhysicsFeature.CanPhysicalyInteract component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddCanPhysicalyInteract(Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.PhysicsFeature.CanPhysicalyInteract() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.PhysicsFeature.IsGrounded IsGroundedC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.PhysicsFeature.IsGrounded>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> IsGrounded => IsGroundedC.Value;

		public bool TryGetIsGrounded(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.PhysicsFeature.IsGrounded component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsGrounded()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.PhysicsFeature.IsGrounded() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsGrounded(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.PhysicsFeature.IsGrounded() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.MainHero.IsMainHero IsMainHeroC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.MainHero.IsMainHero>();

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsMainHero()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.MainHero.IsMainHero() ); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.LootPickedEvent LootPickedEventC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.LootPickedEvent>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent<Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.LootType> LootPickedEvent => LootPickedEventC.Value;

		public bool TryGetLootPickedEvent(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent<Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.LootType> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.LootPickedEvent component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent<Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.LootType>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddLootPickedEvent()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.LootPickedEvent() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent<Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.LootType>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddLootPickedEvent(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent<Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.LootType> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.LootPickedEvent() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.LootCollectRange LootCollectRangeC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.LootCollectRange>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> LootCollectRange => LootCollectRangeC.Value;

		public bool TryGetLootCollectRange(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.LootCollectRange component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddLootCollectRange()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.LootCollectRange() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddLootCollectRange(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.LootCollectRange() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.LootInitialLifeTime LootInitialLifeTimeC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.LootInitialLifeTime>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> LootInitialLifeTime => LootInitialLifeTimeC.Value;

		public bool TryGetLootInitialLifeTime(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.LootInitialLifeTime component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddLootInitialLifeTime()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.LootInitialLifeTime() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddLootInitialLifeTime(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.LootInitialLifeTime() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.LootCurrentLifeTime LootCurrentLifeTimeC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.LootCurrentLifeTime>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> LootCurrentLifeTime => LootCurrentLifeTimeC.Value;

		public bool TryGetLootCurrentLifeTime(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.LootCurrentLifeTime component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddLootCurrentLifeTime()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.LootCurrentLifeTime() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddLootCurrentLifeTime(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.LootCurrentLifeTime() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.CanDropLoot CanDropLootC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.CanDropLoot>();

		public Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition CanDropLoot => CanDropLootC.Value;

		public bool TryGetCanDropLoot(out Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.CanDropLoot component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddCanDropLoot(Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.CanDropLoot() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.HealthBarPoint HealthBarPointC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.HealthBarPoint>();

		public UnityEngine.Transform HealthBarPoint => HealthBarPointC.Value;

		public bool TryGetHealthBarPoint(out UnityEngine.Transform value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.HealthBarPoint component);
			if (result)
				value = component.Value;
			else
				value = default(UnityEngine.Transform);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddHealthBarPoint(UnityEngine.Transform value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.HealthBarPoint() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.CurrentHealth CurrentHealthC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.CurrentHealth>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> CurrentHealth => CurrentHealthC.Value;

		public bool TryGetCurrentHealth(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.CurrentHealth component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddCurrentHealth()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.CurrentHealth() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddCurrentHealth(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.CurrentHealth() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.MaxHealth MaxHealthC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.MaxHealth>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> MaxHealth => MaxHealthC.Value;

		public bool TryGetMaxHealth(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.MaxHealth component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddMaxHealth()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.MaxHealth() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddMaxHealth(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.MaxHealth() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.MustDie MustDieC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.MustDie>();

		public Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition MustDie => MustDieC.Value;

		public bool TryGetMustDie(out Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.MustDie component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddMustDie(Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.MustDie() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.MustSelfRelease MustSelfReleaseC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.MustSelfRelease>();

		public Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition MustSelfRelease => MustSelfReleaseC.Value;

		public bool TryGetMustSelfRelease(out Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.MustSelfRelease component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddMustSelfRelease(Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.MustSelfRelease() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.IsDead IsDeadC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.IsDead>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> IsDead => IsDeadC.Value;

		public bool TryGetIsDead(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.IsDead component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsDead()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.IsDead() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsDead(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.IsDead() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.DeathProcessInitialTime DeathProcessInitialTimeC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.DeathProcessInitialTime>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> DeathProcessInitialTime => DeathProcessInitialTimeC.Value;

		public bool TryGetDeathProcessInitialTime(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.DeathProcessInitialTime component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDeathProcessInitialTime()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.DeathProcessInitialTime() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDeathProcessInitialTime(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.DeathProcessInitialTime() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.DeathProcessCurrentTime DeathProcessCurrentTimeC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.DeathProcessCurrentTime>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> DeathProcessCurrentTime => DeathProcessCurrentTimeC.Value;

		public bool TryGetDeathProcessCurrentTime(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.DeathProcessCurrentTime component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDeathProcessCurrentTime()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.DeathProcessCurrentTime() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDeathProcessCurrentTime(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.DeathProcessCurrentTime() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.InDeathProcess InDeathProcessC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.InDeathProcess>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> InDeathProcess => InDeathProcessC.Value;

		public bool TryGetInDeathProcess(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.InDeathProcess component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddInDeathProcess()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.InDeathProcess() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddInDeathProcess(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.InDeathProcess() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.DisableCollidersOnDeath DisableCollidersOnDeathC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.DisableCollidersOnDeath>();

		public System.Collections.Generic.List<UnityEngine.Collider2D> DisableCollidersOnDeath => DisableCollidersOnDeathC.Value;

		public bool TryGetDisableCollidersOnDeath(out System.Collections.Generic.List<UnityEngine.Collider2D> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.DisableCollidersOnDeath component);
			if (result)
				value = component.Value;
			else
				value = default(System.Collections.Generic.List<UnityEngine.Collider2D>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDisableCollidersOnDeath()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.DisableCollidersOnDeath() { Value = new System.Collections.Generic.List<UnityEngine.Collider2D>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDisableCollidersOnDeath(System.Collections.Generic.List<UnityEngine.Collider2D> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.DisableCollidersOnDeath() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature.IntentMovement IntentMovementC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature.IntentMovement>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector2> IntentMovement => IntentMovementC.Value;

		public bool TryGetIntentMovement(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector2> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature.IntentMovement component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector2>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIntentMovement()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature.IntentMovement() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector2>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIntentMovement(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector2> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature.IntentMovement() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature.IntentJump IntentJumpC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature.IntentJump>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> IntentJump => IntentJumpC.Value;

		public bool TryGetIntentJump(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature.IntentJump component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIntentJump()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature.IntentJump() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIntentJump(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature.IntentJump() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature.IntentSlide IntentSlideC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature.IntentSlide>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> IntentSlide => IntentSlideC.Value;

		public bool TryGetIntentSlide(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature.IntentSlide component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIntentSlide()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature.IntentSlide() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIntentSlide(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature.IntentSlide() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature.IntentDash IntentDashC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature.IntentDash>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> IntentDash => IntentDashC.Value;

		public bool TryGetIntentDash(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature.IntentDash component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIntentDash()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature.IntentDash() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIntentDash(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature.IntentDash() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature.IntentAttack IntentAttackC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature.IntentAttack>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> IntentAttack => IntentAttackC.Value;

		public bool TryGetIntentAttack(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature.IntentAttack component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIntentAttack()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature.IntentAttack() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIntentAttack(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature.IntentAttack() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature.ThrowEvent ThrowEventC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature.ThrowEvent>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent ThrowEvent => ThrowEventC.Value;

		public bool TryGetThrowEvent(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature.ThrowEvent component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddThrowEvent()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature.ThrowEvent() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddThrowEvent(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature.ThrowEvent() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature.ThrowRequest ThrowRequestC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature.ThrowRequest>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent ThrowRequest => ThrowRequestC.Value;

		public bool TryGetThrowRequest(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature.ThrowRequest component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddThrowRequest()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature.ThrowRequest() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddThrowRequest(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature.ThrowRequest() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.GrappleMinDistance GrappleMinDistanceC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.GrappleMinDistance>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> GrappleMinDistance => GrappleMinDistanceC.Value;

		public bool TryGetGrappleMinDistance(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.GrappleMinDistance component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddGrappleMinDistance()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.GrappleMinDistance() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddGrappleMinDistance(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.GrappleMinDistance() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.GrappleArrivalBounce GrappleArrivalBounceC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.GrappleArrivalBounce>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> GrappleArrivalBounce => GrappleArrivalBounceC.Value;

		public bool TryGetGrappleArrivalBounce(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.GrappleArrivalBounce component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddGrappleArrivalBounce()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.GrappleArrivalBounce() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddGrappleArrivalBounce(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.GrappleArrivalBounce() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.GrappleMaxDistance GrappleMaxDistanceC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.GrappleMaxDistance>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> GrappleMaxDistance => GrappleMaxDistanceC.Value;

		public bool TryGetGrappleMaxDistance(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.GrappleMaxDistance component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddGrappleMaxDistance()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.GrappleMaxDistance() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddGrappleMaxDistance(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.GrappleMaxDistance() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.IsGrappledTarget IsGrappledTargetC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.IsGrappledTarget>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> IsGrappledTarget => IsGrappledTargetC.Value;

		public bool TryGetIsGrappledTarget(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.IsGrappledTarget component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsGrappledTarget()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.IsGrappledTarget() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsGrappledTarget(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.IsGrappledTarget() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.CanGrapple CanGrappleC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.CanGrapple>();

		public Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition CanGrapple => CanGrappleC.Value;

		public bool TryGetCanGrapple(out Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.CanGrapple component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddCanGrapple(Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.CanGrapple() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.IsGrappling IsGrapplingC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.IsGrappling>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> IsGrappling => IsGrapplingC.Value;

		public bool TryGetIsGrappling(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.IsGrappling component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsGrappling()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.IsGrappling() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsGrappling(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.IsGrappling() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.GrappleSpeed GrappleSpeedC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.GrappleSpeed>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> GrappleSpeed => GrappleSpeedC.Value;

		public bool TryGetGrappleSpeed(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.GrappleSpeed component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddGrappleSpeed()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.GrappleSpeed() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddGrappleSpeed(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.GrappleSpeed() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.GrappleProjectileSpeed GrappleProjectileSpeedC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.GrappleProjectileSpeed>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> GrappleProjectileSpeed => GrappleProjectileSpeedC.Value;

		public bool TryGetGrappleProjectileSpeed(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.GrappleProjectileSpeed component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddGrappleProjectileSpeed()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.GrappleProjectileSpeed() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddGrappleProjectileSpeed(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.GrappleProjectileSpeed() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.GrappleAnchorPoint GrappleAnchorPointC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.GrappleAnchorPoint>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector3> GrappleAnchorPoint => GrappleAnchorPointC.Value;

		public bool TryGetGrappleAnchorPoint(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector3> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.GrappleAnchorPoint component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector3>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddGrappleAnchorPoint()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.GrappleAnchorPoint() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector3>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddGrappleAnchorPoint(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector3> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.GrappleAnchorPoint() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.GrappleArriveDistance GrappleArriveDistanceC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.GrappleArriveDistance>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> GrappleArriveDistance => GrappleArriveDistanceC.Value;

		public bool TryGetGrappleArriveDistance(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.GrappleArriveDistance component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddGrappleArriveDistance()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.GrappleArriveDistance() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddGrappleArriveDistance(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.GrappleArriveDistance() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.CanPlunge CanPlungeC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.CanPlunge>();

		public Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition CanPlunge => CanPlungeC.Value;

		public bool TryGetCanPlunge(out Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.CanPlunge component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddCanPlunge(Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.CanPlunge() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.PlungeAccelerationMultiplier PlungeAccelerationMultiplierC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.PlungeAccelerationMultiplier>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> PlungeAccelerationMultiplier => PlungeAccelerationMultiplierC.Value;

		public bool TryGetPlungeAccelerationMultiplier(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.PlungeAccelerationMultiplier component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddPlungeAccelerationMultiplier()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.PlungeAccelerationMultiplier() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddPlungeAccelerationMultiplier(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.PlungeAccelerationMultiplier() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.MinPlungeImpactSpeedThreshold MinPlungeImpactSpeedThresholdC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.MinPlungeImpactSpeedThreshold>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> MinPlungeImpactSpeedThreshold => MinPlungeImpactSpeedThresholdC.Value;

		public bool TryGetMinPlungeImpactSpeedThreshold(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.MinPlungeImpactSpeedThreshold component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddMinPlungeImpactSpeedThreshold()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.MinPlungeImpactSpeedThreshold() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddMinPlungeImpactSpeedThreshold(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.MinPlungeImpactSpeedThreshold() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.PlungeImpactEvent PlungeImpactEventC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.PlungeImpactEvent>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent<System.Single> PlungeImpactEvent => PlungeImpactEventC.Value;

		public bool TryGetPlungeImpactEvent(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.PlungeImpactEvent component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddPlungeImpactEvent()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.PlungeImpactEvent() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddPlungeImpactEvent(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.PlungeImpactEvent() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.IsPlunging IsPlungingC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.IsPlunging>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> IsPlunging => IsPlungingC.Value;

		public bool TryGetIsPlunging(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.IsPlunging component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsPlunging()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.IsPlunging() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsPlunging(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.IsPlunging() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.PlungeSpeed PlungeSpeedC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.PlungeSpeed>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> PlungeSpeed => PlungeSpeedC.Value;

		public bool TryGetPlungeSpeed(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.PlungeSpeed component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddPlungeSpeed()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.PlungeSpeed() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddPlungeSpeed(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.PlungeSpeed() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.PlungeLandImpactRangeMin PlungeLandImpactRangeMinC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.PlungeLandImpactRangeMin>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> PlungeLandImpactRangeMin => PlungeLandImpactRangeMinC.Value;

		public bool TryGetPlungeLandImpactRangeMin(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.PlungeLandImpactRangeMin component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddPlungeLandImpactRangeMin()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.PlungeLandImpactRangeMin() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddPlungeLandImpactRangeMin(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.PlungeLandImpactRangeMin() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.PlungeLandImpactDamageMin PlungeLandImpactDamageMinC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.PlungeLandImpactDamageMin>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> PlungeLandImpactDamageMin => PlungeLandImpactDamageMinC.Value;

		public bool TryGetPlungeLandImpactDamageMin(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.PlungeLandImpactDamageMin component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddPlungeLandImpactDamageMin()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.PlungeLandImpactDamageMin() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddPlungeLandImpactDamageMin(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.PlungeLandImpactDamageMin() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.PlungeLandImpactKnockbackForceMin PlungeLandImpactKnockbackForceMinC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.PlungeLandImpactKnockbackForceMin>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> PlungeLandImpactKnockbackForceMin => PlungeLandImpactKnockbackForceMinC.Value;

		public bool TryGetPlungeLandImpactKnockbackForceMin(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.PlungeLandImpactKnockbackForceMin component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddPlungeLandImpactKnockbackForceMin()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.PlungeLandImpactKnockbackForceMin() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddPlungeLandImpactKnockbackForceMin(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.PlungeLandImpactKnockbackForceMin() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.CanSlopeJump CanSlopeJumpC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.CanSlopeJump>();

		public Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition CanSlopeJump => CanSlopeJumpC.Value;

		public bool TryGetCanSlopeJump(out Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.CanSlopeJump component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddCanSlopeJump(Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.CanSlopeJump() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.CanSlopeSlip CanSlopeSlipC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.CanSlopeSlip>();

		public Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition CanSlopeSlip => CanSlopeSlipC.Value;

		public bool TryGetCanSlopeSlip(out Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.CanSlopeSlip component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddCanSlopeSlip(Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.CanSlopeSlip() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.IsOnSlope IsOnSlopeC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.IsOnSlope>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> IsOnSlope => IsOnSlopeC.Value;

		public bool TryGetIsOnSlope(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.IsOnSlope component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsOnSlope()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.IsOnSlope() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsOnSlope(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.IsOnSlope() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.BaseSlopeJumpForce BaseSlopeJumpForceC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.BaseSlopeJumpForce>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> BaseSlopeJumpForce => BaseSlopeJumpForceC.Value;

		public bool TryGetBaseSlopeJumpForce(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.BaseSlopeJumpForce component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddBaseSlopeJumpForce()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.BaseSlopeJumpForce() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddBaseSlopeJumpForce(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.BaseSlopeJumpForce() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.JumpForceModifier JumpForceModifierC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.JumpForceModifier>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> JumpForceModifier => JumpForceModifierC.Value;

		public bool TryGetJumpForceModifier(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.JumpForceModifier component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddJumpForceModifier()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.JumpForceModifier() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddJumpForceModifier(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.JumpForceModifier() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeJumpForceModifier SlopeJumpForceModifierC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeJumpForceModifier>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector2> SlopeJumpForceModifier => SlopeJumpForceModifierC.Value;

		public bool TryGetSlopeJumpForceModifier(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector2> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeJumpForceModifier component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector2>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSlopeJumpForceModifier()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeJumpForceModifier() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector2>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSlopeJumpForceModifier(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector2> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeJumpForceModifier() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeMask SlopeMaskC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeMask>();

		public UnityEngine.LayerMask SlopeMask => SlopeMaskC.Value;

		public bool TryGetSlopeMask(out UnityEngine.LayerMask value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeMask component);
			if (result)
				value = component.Value;
			else
				value = default(UnityEngine.LayerMask);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSlopeMask(UnityEngine.LayerMask value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeMask() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.MinFallVelocityForAutoSlide MinFallVelocityForAutoSlideC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.MinFallVelocityForAutoSlide>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> MinFallVelocityForAutoSlide => MinFallVelocityForAutoSlideC.Value;

		public bool TryGetMinFallVelocityForAutoSlide(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.MinFallVelocityForAutoSlide component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddMinFallVelocityForAutoSlide()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.MinFallVelocityForAutoSlide() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddMinFallVelocityForAutoSlide(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.MinFallVelocityForAutoSlide() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeNormal SlopeNormalC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeNormal>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector2> SlopeNormal => SlopeNormalC.Value;

		public bool TryGetSlopeNormal(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector2> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeNormal component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector2>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSlopeNormal()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeNormal() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector2>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSlopeNormal(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector2> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeNormal() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeBaseSlideSpeed SlopeBaseSlideSpeedC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeBaseSlideSpeed>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> SlopeBaseSlideSpeed => SlopeBaseSlideSpeedC.Value;

		public bool TryGetSlopeBaseSlideSpeed(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeBaseSlideSpeed component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSlopeBaseSlideSpeed()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeBaseSlideSpeed() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSlopeBaseSlideSpeed(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeBaseSlideSpeed() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeSlideAcceleration SlopeSlideAccelerationC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeSlideAcceleration>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> SlopeSlideAcceleration => SlopeSlideAccelerationC.Value;

		public bool TryGetSlopeSlideAcceleration(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeSlideAcceleration component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSlopeSlideAcceleration()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeSlideAcceleration() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSlopeSlideAcceleration(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeSlideAcceleration() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeMaxSlideSpeed SlopeMaxSlideSpeedC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeMaxSlideSpeed>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> SlopeMaxSlideSpeed => SlopeMaxSlideSpeedC.Value;

		public bool TryGetSlopeMaxSlideSpeed(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeMaxSlideSpeed component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSlopeMaxSlideSpeed()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeMaxSlideSpeed() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSlopeMaxSlideSpeed(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeMaxSlideSpeed() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeMaxAccumSpeed SlopeMaxAccumSpeedC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeMaxAccumSpeed>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> SlopeMaxAccumSpeed => SlopeMaxAccumSpeedC.Value;

		public bool TryGetSlopeMaxAccumSpeed(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeMaxAccumSpeed component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSlopeMaxAccumSpeed()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeMaxAccumSpeed() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSlopeMaxAccumSpeed(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeMaxAccumSpeed() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeMaxStableAngle SlopeMaxStableAngleC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeMaxStableAngle>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> SlopeMaxStableAngle => SlopeMaxStableAngleC.Value;

		public bool TryGetSlopeMaxStableAngle(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeMaxStableAngle component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSlopeMaxStableAngle()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeMaxStableAngle() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSlopeMaxStableAngle(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeMaxStableAngle() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeSlipForce SlopeSlipForceC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeSlipForce>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> SlopeSlipForce => SlopeSlipForceC.Value;

		public bool TryGetSlopeSlipForce(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeSlipForce component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSlopeSlipForce()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeSlipForce() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSlopeSlipForce(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeSlipForce() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeAccumGainRate SlopeAccumGainRateC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeAccumGainRate>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> SlopeAccumGainRate => SlopeAccumGainRateC.Value;

		public bool TryGetSlopeAccumGainRate(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeAccumGainRate component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSlopeAccumGainRate()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeAccumGainRate() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSlopeAccumGainRate(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeAccumGainRate() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeAccumSpeed SlopeAccumSpeedC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeAccumSpeed>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> SlopeAccumSpeed => SlopeAccumSpeedC.Value;

		public bool TryGetSlopeAccumSpeed(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeAccumSpeed component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSlopeAccumSpeed()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeAccumSpeed() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSlopeAccumSpeed(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeAccumSpeed() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeAngle SlopeAngleC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeAngle>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> SlopeAngle => SlopeAngleC.Value;

		public bool TryGetSlopeAngle(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeAngle component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSlopeAngle()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeAngle() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSlopeAngle(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeAngle() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeMinAngle SlopeMinAngleC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeMinAngle>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> SlopeMinAngle => SlopeMinAngleC.Value;

		public bool TryGetSlopeMinAngle(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeMinAngle component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSlopeMinAngle()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeMinAngle() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSlopeMinAngle(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeMinAngle() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeMaxAngle SlopeMaxAngleC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeMaxAngle>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> SlopeMaxAngle => SlopeMaxAngleC.Value;

		public bool TryGetSlopeMaxAngle(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeMaxAngle component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSlopeMaxAngle()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeMaxAngle() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSlopeMaxAngle(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeMaxAngle() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeJumpEvent SlopeJumpEventC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeJumpEvent>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent<System.Single> SlopeJumpEvent => SlopeJumpEventC.Value;

		public bool TryGetSlopeJumpEvent(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeJumpEvent component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSlopeJumpEvent()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeJumpEvent() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSlopeJumpEvent(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope.SlopeJumpEvent() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slide.CanSlide CanSlideC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slide.CanSlide>();

		public Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition CanSlide => CanSlideC.Value;

		public bool TryGetCanSlide(out Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slide.CanSlide component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddCanSlide(Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slide.CanSlide() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slide.SlideHitBoxSize SlideHitBoxSizeC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slide.SlideHitBoxSize>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector2> SlideHitBoxSize => SlideHitBoxSizeC.Value;

		public bool TryGetSlideHitBoxSize(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector2> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slide.SlideHitBoxSize component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector2>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSlideHitBoxSize()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slide.SlideHitBoxSize() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector2>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSlideHitBoxSize(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector2> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slide.SlideHitBoxSize() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slide.IsSliding IsSlidingC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slide.IsSliding>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> IsSliding => IsSlidingC.Value;

		public bool TryGetIsSliding(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slide.IsSliding component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsSliding()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slide.IsSliding() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsSliding(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slide.IsSliding() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slide.SlideCooldown SlideCooldownC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slide.SlideCooldown>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> SlideCooldown => SlideCooldownC.Value;

		public bool TryGetSlideCooldown(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slide.SlideCooldown component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSlideCooldown()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slide.SlideCooldown() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSlideCooldown(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slide.SlideCooldown() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slide.SlideDuration SlideDurationC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slide.SlideDuration>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> SlideDuration => SlideDurationC.Value;

		public bool TryGetSlideDuration(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slide.SlideDuration component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSlideDuration()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slide.SlideDuration() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSlideDuration(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slide.SlideDuration() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slide.SlideSpeed SlideSpeedC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slide.SlideSpeed>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> SlideSpeed => SlideSpeedC.Value;

		public bool TryGetSlideSpeed(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slide.SlideSpeed component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSlideSpeed()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slide.SlideSpeed() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSlideSpeed(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slide.SlideSpeed() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move.CurrentMovementState CurrentMovementStateC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move.CurrentMovementState>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move.MovementStates> CurrentMovementState => CurrentMovementStateC.Value;

		public bool TryGetCurrentMovementState(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move.MovementStates> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move.CurrentMovementState component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move.MovementStates>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddCurrentMovementState()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move.CurrentMovementState() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move.MovementStates>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddCurrentMovementState(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move.MovementStates> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move.CurrentMovementState() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move.CanFlip CanFlipC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move.CanFlip>();

		public Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition CanFlip => CanFlipC.Value;

		public bool TryGetCanFlip(out Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move.CanFlip component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddCanFlip(Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move.CanFlip() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move.Acceleration AccelerationC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move.Acceleration>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> Acceleration => AccelerationC.Value;

		public bool TryGetAcceleration(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move.Acceleration component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAcceleration()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move.Acceleration() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAcceleration(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move.Acceleration() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move.Deceleration DecelerationC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move.Deceleration>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> Deceleration => DecelerationC.Value;

		public bool TryGetDeceleration(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move.Deceleration component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDeceleration()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move.Deceleration() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDeceleration(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move.Deceleration() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move.MoveSpeedMin MoveSpeedMinC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move.MoveSpeedMin>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> MoveSpeedMin => MoveSpeedMinC.Value;

		public bool TryGetMoveSpeedMin(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move.MoveSpeedMin component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddMoveSpeedMin()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move.MoveSpeedMin() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddMoveSpeedMin(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move.MoveSpeedMin() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move.MoveDirection MoveDirectionC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move.MoveDirection>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector2> MoveDirection => MoveDirectionC.Value;

		public bool TryGetMoveDirection(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector2> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move.MoveDirection component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector2>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddMoveDirection()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move.MoveDirection() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector2>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddMoveDirection(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector2> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move.MoveDirection() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move.MoveSpeed MoveSpeedC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move.MoveSpeed>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> MoveSpeed => MoveSpeedC.Value;

		public bool TryGetMoveSpeed(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move.MoveSpeed component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddMoveSpeed()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move.MoveSpeed() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddMoveSpeed(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move.MoveSpeed() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move.IsMoving IsMovingC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move.IsMoving>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> IsMoving => IsMovingC.Value;

		public bool TryGetIsMoving(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move.IsMoving component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsMoving()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move.IsMoving() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsMoving(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move.IsMoving() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move.CanMove CanMoveC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move.CanMove>();

		public Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition CanMove => CanMoveC.Value;

		public bool TryGetCanMove(out Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move.CanMove component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddCanMove(Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move.CanMove() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move.RotationDirection RotationDirectionC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move.RotationDirection>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector3> RotationDirection => RotationDirectionC.Value;

		public bool TryGetRotationDirection(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector3> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move.RotationDirection component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector3>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddRotationDirection()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move.RotationDirection() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector3>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddRotationDirection(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector3> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move.RotationDirection() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move.RotationSpeed RotationSpeedC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move.RotationSpeed>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> RotationSpeed => RotationSpeedC.Value;

		public bool TryGetRotationSpeed(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move.RotationSpeed component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddRotationSpeed()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move.RotationSpeed() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddRotationSpeed(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move.RotationSpeed() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move.CanRotate CanRotateC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move.CanRotate>();

		public Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition CanRotate => CanRotateC.Value;

		public bool TryGetCanRotate(out Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move.CanRotate component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddCanRotate(Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move.CanRotate() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Jump.CanJump CanJumpC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Jump.CanJump>();

		public Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition CanJump => CanJumpC.Value;

		public bool TryGetCanJump(out Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Jump.CanJump component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddCanJump(Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Jump.CanJump() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Jump.JumpForceMin JumpForceMinC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Jump.JumpForceMin>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> JumpForceMin => JumpForceMinC.Value;

		public bool TryGetJumpForceMin(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Jump.JumpForceMin component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddJumpForceMin()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Jump.JumpForceMin() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddJumpForceMin(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Jump.JumpForceMin() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Jump.JumpForceMax JumpForceMaxC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Jump.JumpForceMax>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> JumpForceMax => JumpForceMaxC.Value;

		public bool TryGetJumpForceMax(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Jump.JumpForceMax component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddJumpForceMax()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Jump.JumpForceMax() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddJumpForceMax(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Jump.JumpForceMax() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Jump.JumpChargeTime JumpChargeTimeC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Jump.JumpChargeTime>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> JumpChargeTime => JumpChargeTimeC.Value;

		public bool TryGetJumpChargeTime(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Jump.JumpChargeTime component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddJumpChargeTime()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Jump.JumpChargeTime() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddJumpChargeTime(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Jump.JumpChargeTime() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Jump.JumpRequest JumpRequestC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Jump.JumpRequest>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent JumpRequest => JumpRequestC.Value;

		public bool TryGetJumpRequest(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Jump.JumpRequest component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddJumpRequest()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Jump.JumpRequest() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddJumpRequest(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Jump.JumpRequest() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Jump.JumpEvent JumpEventC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Jump.JumpEvent>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent JumpEvent => JumpEventC.Value;

		public bool TryGetJumpEvent(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Jump.JumpEvent component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddJumpEvent()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Jump.JumpEvent() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddJumpEvent(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Jump.JumpEvent() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Jump.CanWallJump CanWallJumpC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Jump.CanWallJump>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> CanWallJump => CanWallJumpC.Value;

		public bool TryGetCanWallJump(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Jump.CanWallJump component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddCanWallJump()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Jump.CanWallJump() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddCanWallJump(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Jump.CanWallJump() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Jump.WallJumpForceMultiplier WallJumpForceMultiplierC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Jump.WallJumpForceMultiplier>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector2> WallJumpForceMultiplier => WallJumpForceMultiplierC.Value;

		public bool TryGetWallJumpForceMultiplier(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector2> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Jump.WallJumpForceMultiplier component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector2>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddWallJumpForceMultiplier()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Jump.WallJumpForceMultiplier() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector2>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddWallJumpForceMultiplier(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector2> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Jump.WallJumpForceMultiplier() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Jump.WallMask WallMaskC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Jump.WallMask>();

		public UnityEngine.LayerMask WallMask => WallMaskC.Value;

		public bool TryGetWallMask(out UnityEngine.LayerMask value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Jump.WallMask component);
			if (result)
				value = component.Value;
			else
				value = default(UnityEngine.LayerMask);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddWallMask(UnityEngine.LayerMask value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Jump.WallMask() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Jump.IsWallJumping IsWallJumpingC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Jump.IsWallJumping>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> IsWallJumping => IsWallJumpingC.Value;

		public bool TryGetIsWallJumping(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Jump.IsWallJumping component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsWallJumping()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Jump.IsWallJumping() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsWallJumping(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Jump.IsWallJumping() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Jump.WallJumpEvent WallJumpEventC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Jump.WallJumpEvent>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent WallJumpEvent => WallJumpEventC.Value;

		public bool TryGetWallJumpEvent(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Jump.WallJumpEvent component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddWallJumpEvent()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Jump.WallJumpEvent() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddWallJumpEvent(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Jump.WallJumpEvent() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Jump.WallJumpRequest WallJumpRequestC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Jump.WallJumpRequest>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent WallJumpRequest => WallJumpRequestC.Value;

		public bool TryGetWallJumpRequest(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Jump.WallJumpRequest component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddWallJumpRequest()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Jump.WallJumpRequest() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddWallJumpRequest(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Jump.WallJumpRequest() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.HangWall.CanWallHang CanWallHangC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.HangWall.CanWallHang>();

		public Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition CanWallHang => CanWallHangC.Value;

		public bool TryGetCanWallHang(out Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.HangWall.CanWallHang component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddCanWallHang(Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.HangWall.CanWallHang() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.HangWall.IsWallHanging IsWallHangingC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.HangWall.IsWallHanging>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> IsWallHanging => IsWallHangingC.Value;

		public bool TryGetIsWallHanging(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.HangWall.IsWallHanging component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsWallHanging()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.HangWall.IsWallHanging() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsWallHanging(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.HangWall.IsWallHanging() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.HangWall.WallHangSlideSpeed WallHangSlideSpeedC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.HangWall.WallHangSlideSpeed>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> WallHangSlideSpeed => WallHangSlideSpeedC.Value;

		public bool TryGetWallHangSlideSpeed(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.HangWall.WallHangSlideSpeed component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddWallHangSlideSpeed()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.HangWall.WallHangSlideSpeed() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddWallHangSlideSpeed(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.HangWall.WallHangSlideSpeed() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.HangWall.WallHangLayer WallHangLayerC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.HangWall.WallHangLayer>();

		public UnityEngine.LayerMask WallHangLayer => WallHangLayerC.Value;

		public bool TryGetWallHangLayer(out UnityEngine.LayerMask value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.HangWall.WallHangLayer component);
			if (result)
				value = component.Value;
			else
				value = default(UnityEngine.LayerMask);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddWallHangLayer(UnityEngine.LayerMask value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.HangWall.WallHangLayer() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.HangWall.WallJumpForce WallJumpForceC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.HangWall.WallJumpForce>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector2> WallJumpForce => WallJumpForceC.Value;

		public bool TryGetWallJumpForce(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector2> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.HangWall.WallJumpForce component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector2>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddWallJumpForce()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.HangWall.WallJumpForce() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector2>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddWallJumpForce(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector2> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.HangWall.WallJumpForce() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.HangWall.WallDirection WallDirectionC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.HangWall.WallDirection>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> WallDirection => WallDirectionC.Value;

		public bool TryGetWallDirection(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.HangWall.WallDirection component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddWallDirection()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.HangWall.WallDirection() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddWallDirection(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.HangWall.WallDirection() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Dash.CanDash CanDashC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Dash.CanDash>();

		public Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition CanDash => CanDashC.Value;

		public bool TryGetCanDash(out Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Dash.CanDash component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddCanDash(Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Dash.CanDash() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Dash.IsDashing IsDashingC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Dash.IsDashing>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> IsDashing => IsDashingC.Value;

		public bool TryGetIsDashing(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Dash.IsDashing component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsDashing()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Dash.IsDashing() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsDashing(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Dash.IsDashing() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Dash.DashRequest DashRequestC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Dash.DashRequest>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent DashRequest => DashRequestC.Value;

		public bool TryGetDashRequest(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Dash.DashRequest component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDashRequest()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Dash.DashRequest() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDashRequest(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Dash.DashRequest() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Dash.DashStartedEvent DashStartedEventC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Dash.DashStartedEvent>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent DashStartedEvent => DashStartedEventC.Value;

		public bool TryGetDashStartedEvent(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Dash.DashStartedEvent component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDashStartedEvent()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Dash.DashStartedEvent() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDashStartedEvent(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Dash.DashStartedEvent() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Dash.DashCompletedEvent DashCompletedEventC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Dash.DashCompletedEvent>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent DashCompletedEvent => DashCompletedEventC.Value;

		public bool TryGetDashCompletedEvent(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Dash.DashCompletedEvent component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDashCompletedEvent()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Dash.DashCompletedEvent() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDashCompletedEvent(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Dash.DashCompletedEvent() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Dash.DashForceMin DashForceMinC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Dash.DashForceMin>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> DashForceMin => DashForceMinC.Value;

		public bool TryGetDashForceMin(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Dash.DashForceMin component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDashForceMin()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Dash.DashForceMin() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDashForceMin(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Dash.DashForceMin() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Dash.DashForceMax DashForceMaxC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Dash.DashForceMax>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> DashForceMax => DashForceMaxC.Value;

		public bool TryGetDashForceMax(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Dash.DashForceMax component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDashForceMax()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Dash.DashForceMax() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDashForceMax(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Dash.DashForceMax() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Dash.DashChargeTimeMax DashChargeTimeMaxC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Dash.DashChargeTimeMax>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> DashChargeTimeMax => DashChargeTimeMaxC.Value;

		public bool TryGetDashChargeTimeMax(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Dash.DashChargeTimeMax component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDashChargeTimeMax()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Dash.DashChargeTimeMax() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDashChargeTimeMax(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Dash.DashChargeTimeMax() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Dash.DashCooldown DashCooldownC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Dash.DashCooldown>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> DashCooldown => DashCooldownC.Value;

		public bool TryGetDashCooldown(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Dash.DashCooldown component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDashCooldown()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Dash.DashCooldown() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDashCooldown(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Dash.DashCooldown() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Dash.DashDuration DashDurationC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Dash.DashDuration>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> DashDuration => DashDurationC.Value;

		public bool TryGetDashDuration(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Dash.DashDuration component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDashDuration()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Dash.DashDuration() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDashDuration(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Dash.DashDuration() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Dash.AirDashMultiplier AirDashMultiplierC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Dash.AirDashMultiplier>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> AirDashMultiplier => AirDashMultiplierC.Value;

		public bool TryGetAirDashMultiplier(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Dash.AirDashMultiplier component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAirDashMultiplier()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Dash.AirDashMultiplier() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAirDashMultiplier(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Dash.AirDashMultiplier() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Dash.AirDashVerticalBoost AirDashVerticalBoostC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Dash.AirDashVerticalBoost>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> AirDashVerticalBoost => AirDashVerticalBoostC.Value;

		public bool TryGetAirDashVerticalBoost(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Dash.AirDashVerticalBoost component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAirDashVerticalBoost()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Dash.AirDashVerticalBoost() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAirDashVerticalBoost(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Dash.AirDashVerticalBoost() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.AirJump.CanAirJump CanAirJumpC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.AirJump.CanAirJump>();

		public Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition CanAirJump => CanAirJumpC.Value;

		public bool TryGetCanAirJump(out Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.AirJump.CanAirJump component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddCanAirJump(Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.AirJump.CanAirJump() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.AirJump.MustRestoreAirJumpsCount MustRestoreAirJumpsCountC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.AirJump.MustRestoreAirJumpsCount>();

		public Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition MustRestoreAirJumpsCount => MustRestoreAirJumpsCountC.Value;

		public bool TryGetMustRestoreAirJumpsCount(out Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.AirJump.MustRestoreAirJumpsCount component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddMustRestoreAirJumpsCount(Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.AirJump.MustRestoreAirJumpsCount() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.AirJump.AirJumpForceMin AirJumpForceMinC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.AirJump.AirJumpForceMin>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> AirJumpForceMin => AirJumpForceMinC.Value;

		public bool TryGetAirJumpForceMin(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.AirJump.AirJumpForceMin component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAirJumpForceMin()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.AirJump.AirJumpForceMin() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAirJumpForceMin(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.AirJump.AirJumpForceMin() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.AirJump.AirJumpForceMax AirJumpForceMaxC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.AirJump.AirJumpForceMax>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> AirJumpForceMax => AirJumpForceMaxC.Value;

		public bool TryGetAirJumpForceMax(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.AirJump.AirJumpForceMax component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAirJumpForceMax()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.AirJump.AirJumpForceMax() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAirJumpForceMax(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.AirJump.AirJumpForceMax() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.AirJump.AirJumpChargeTime AirJumpChargeTimeC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.AirJump.AirJumpChargeTime>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> AirJumpChargeTime => AirJumpChargeTimeC.Value;

		public bool TryGetAirJumpChargeTime(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.AirJump.AirJumpChargeTime component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAirJumpChargeTime()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.AirJump.AirJumpChargeTime() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAirJumpChargeTime(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.AirJump.AirJumpChargeTime() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.AirJump.AirJumpsMaxCount AirJumpsMaxCountC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.AirJump.AirJumpsMaxCount>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Int32> AirJumpsMaxCount => AirJumpsMaxCountC.Value;

		public bool TryGetAirJumpsMaxCount(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Int32> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.AirJump.AirJumpsMaxCount component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Int32>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAirJumpsMaxCount()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.AirJump.AirJumpsMaxCount() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Int32>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAirJumpsMaxCount(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Int32> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.AirJump.AirJumpsMaxCount() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.AirJump.AirJumpsCount AirJumpsCountC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.AirJump.AirJumpsCount>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Int32> AirJumpsCount => AirJumpsCountC.Value;

		public bool TryGetAirJumpsCount(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Int32> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.AirJump.AirJumpsCount component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Int32>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAirJumpsCount()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.AirJump.AirJumpsCount() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Int32>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAirJumpsCount(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Int32> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.AirJump.AirJumpsCount() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.AirJump.AirJumpRequest AirJumpRequestC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.AirJump.AirJumpRequest>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent AirJumpRequest => AirJumpRequestC.Value;

		public bool TryGetAirJumpRequest(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.AirJump.AirJumpRequest component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAirJumpRequest()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.AirJump.AirJumpRequest() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAirJumpRequest(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.AirJump.AirJumpRequest() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.AirJump.AirJumpEvent AirJumpEventC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.AirJump.AirJumpEvent>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent AirJumpEvent => AirJumpEventC.Value;

		public bool TryGetAirJumpEvent(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.AirJump.AirJumpEvent component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAirJumpEvent()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.AirJump.AirJumpEvent() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAirJumpEvent(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.AirJump.AirJumpEvent() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.CanGlide CanGlideC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.CanGlide>();

		public Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition CanGlide => CanGlideC.Value;

		public bool TryGetCanGlide(out Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.CanGlide component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddCanGlide(Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.CanGlide() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.GlideGravityScale GlideGravityScaleC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.GlideGravityScale>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> GlideGravityScale => GlideGravityScaleC.Value;

		public bool TryGetGlideGravityScale(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.GlideGravityScale component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddGlideGravityScale()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.GlideGravityScale() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddGlideGravityScale(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.GlideGravityScale() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.IsGliding IsGlidingC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.IsGliding>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> IsGliding => IsGlidingC.Value;

		public bool TryGetIsGliding(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.IsGliding component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsGliding()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.IsGliding() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsGliding(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.IsGliding() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.GlideMaxFallSpeed GlideMaxFallSpeedC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.GlideMaxFallSpeed>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> GlideMaxFallSpeed => GlideMaxFallSpeedC.Value;

		public bool TryGetGlideMaxFallSpeed(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.GlideMaxFallSpeed component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddGlideMaxFallSpeed()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.GlideMaxFallSpeed() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddGlideMaxFallSpeed(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.GlideMaxFallSpeed() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.GlideSpeedDamping GlideSpeedDampingC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.GlideSpeedDamping>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> GlideSpeedDamping => GlideSpeedDampingC.Value;

		public bool TryGetGlideSpeedDamping(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.GlideSpeedDamping component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddGlideSpeedDamping()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.GlideSpeedDamping() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddGlideSpeedDamping(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.GlideSpeedDamping() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.GlideBounceForce GlideBounceForceC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.GlideBounceForce>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> GlideBounceForce => GlideBounceForceC.Value;

		public bool TryGetGlideBounceForce(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.GlideBounceForce component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddGlideBounceForce()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.GlideBounceForce() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddGlideBounceForce(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.GlideBounceForce() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.GlideSnapSpeed GlideSnapSpeedC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.GlideSnapSpeed>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> GlideSnapSpeed => GlideSnapSpeedC.Value;

		public bool TryGetGlideSnapSpeed(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.GlideSnapSpeed component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddGlideSnapSpeed()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.GlideSnapSpeed() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddGlideSnapSpeed(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.GlideSnapSpeed() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.GlideSnapDuration GlideSnapDurationC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.GlideSnapDuration>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> GlideSnapDuration => GlideSnapDurationC.Value;

		public bool TryGetGlideSnapDuration(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.GlideSnapDuration component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddGlideSnapDuration()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.GlideSnapDuration() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddGlideSnapDuration(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.GlideSnapDuration() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.GlideHorizontalDrag GlideHorizontalDragC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.GlideHorizontalDrag>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> GlideHorizontalDrag => GlideHorizontalDragC.Value;

		public bool TryGetGlideHorizontalDrag(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.GlideHorizontalDrag component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddGlideHorizontalDrag()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.GlideHorizontalDrag() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddGlideHorizontalDrag(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.GlideHorizontalDrag() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.ContactTakeDamage.BodyContactDamage BodyContactDamageC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.ContactTakeDamage.BodyContactDamage>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> BodyContactDamage => BodyContactDamageC.Value;

		public bool TryGetBodyContactDamage(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.ContactTakeDamage.BodyContactDamage component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddBodyContactDamage()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.ContactTakeDamage.BodyContactDamage() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddBodyContactDamage(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.ContactTakeDamage.BodyContactDamage() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.ContactTakeDamage.BodyContactDamageMultiplier BodyContactDamageMultiplierC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.ContactTakeDamage.BodyContactDamageMultiplier>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> BodyContactDamageMultiplier => BodyContactDamageMultiplierC.Value;

		public bool TryGetBodyContactDamageMultiplier(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.ContactTakeDamage.BodyContactDamageMultiplier component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddBodyContactDamageMultiplier()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.ContactTakeDamage.BodyContactDamageMultiplier() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddBodyContactDamageMultiplier(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.ContactTakeDamage.BodyContactDamageMultiplier() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.SuccessfulHitEvent SuccessfulHitEventC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.SuccessfulHitEvent>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent SuccessfulHitEvent => SuccessfulHitEventC.Value;

		public bool TryGetSuccessfulHitEvent(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.SuccessfulHitEvent component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSuccessfulHitEvent()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.SuccessfulHitEvent() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSuccessfulHitEvent(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.SuccessfulHitEvent() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackHitStopScale AttackHitStopScaleC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackHitStopScale>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> AttackHitStopScale => AttackHitStopScaleC.Value;

		public bool TryGetAttackHitStopScale(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackHitStopScale component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackHitStopScale()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackHitStopScale() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackHitStopScale(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackHitStopScale() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackHitStopDuration AttackHitStopDurationC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackHitStopDuration>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> AttackHitStopDuration => AttackHitStopDurationC.Value;

		public bool TryGetAttackHitStopDuration(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackHitStopDuration component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackHitStopDuration()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackHitStopDuration() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackHitStopDuration(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackHitStopDuration() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackHitBounceForce AttackHitBounceForceC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackHitBounceForce>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> AttackHitBounceForce => AttackHitBounceForceC.Value;

		public bool TryGetAttackHitBounceForce(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackHitBounceForce component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackHitBounceForce()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackHitBounceForce() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackHitBounceForce(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackHitBounceForce() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.GroundHitBounceModifiers GroundHitBounceModifiersC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.GroundHitBounceModifiers>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector2> GroundHitBounceModifiers => GroundHitBounceModifiersC.Value;

		public bool TryGetGroundHitBounceModifiers(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector2> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.GroundHitBounceModifiers component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector2>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddGroundHitBounceModifiers()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.GroundHitBounceModifiers() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector2>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddGroundHitBounceModifiers(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector2> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.GroundHitBounceModifiers() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AirHitBounceModifiers AirHitBounceModifiersC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AirHitBounceModifiers>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector2> AirHitBounceModifiers => AirHitBounceModifiersC.Value;

		public bool TryGetAirHitBounceModifiers(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector2> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AirHitBounceModifiers component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector2>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAirHitBounceModifiers()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AirHitBounceModifiers() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector2>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAirHitBounceModifiers(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector2> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AirHitBounceModifiers() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackHitMask AttackHitMaskC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackHitMask>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.LayerMask> AttackHitMask => AttackHitMaskC.Value;

		public bool TryGetAttackHitMask(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.LayerMask> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackHitMask component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.LayerMask>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackHitMask()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackHitMask() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.LayerMask>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackHitMask(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.LayerMask> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackHitMask() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackInvulnerabilityDuration AttackInvulnerabilityDurationC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackInvulnerabilityDuration>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> AttackInvulnerabilityDuration => AttackInvulnerabilityDurationC.Value;

		public bool TryGetAttackInvulnerabilityDuration(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackInvulnerabilityDuration component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackInvulnerabilityDuration()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackInvulnerabilityDuration() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackInvulnerabilityDuration(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackInvulnerabilityDuration() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackInvulnerabilityTimer AttackInvulnerabilityTimerC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackInvulnerabilityTimer>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> AttackInvulnerabilityTimer => AttackInvulnerabilityTimerC.Value;

		public bool TryGetAttackInvulnerabilityTimer(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackInvulnerabilityTimer component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackInvulnerabilityTimer()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackInvulnerabilityTimer() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackInvulnerabilityTimer(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackInvulnerabilityTimer() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.StartAttackRequest StartAttackRequestC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.StartAttackRequest>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent StartAttackRequest => StartAttackRequestC.Value;

		public bool TryGetStartAttackRequest(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.StartAttackRequest component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddStartAttackRequest()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.StartAttackRequest() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddStartAttackRequest(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.StartAttackRequest() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.StartAttackEvent StartAttackEventC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.StartAttackEvent>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent StartAttackEvent => StartAttackEventC.Value;

		public bool TryGetStartAttackEvent(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.StartAttackEvent component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddStartAttackEvent()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.StartAttackEvent() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddStartAttackEvent(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.StartAttackEvent() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.CanStartAttack CanStartAttackC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.CanStartAttack>();

		public Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition CanStartAttack => CanStartAttackC.Value;

		public bool TryGetCanStartAttack(out Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.CanStartAttack component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddCanStartAttack(Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.CanStartAttack() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.EndAttackEvent EndAttackEventC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.EndAttackEvent>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent EndAttackEvent => EndAttackEventC.Value;

		public bool TryGetEndAttackEvent(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.EndAttackEvent component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddEndAttackEvent()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.EndAttackEvent() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddEndAttackEvent(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.EndAttackEvent() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackProcessInitialTime AttackProcessInitialTimeC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackProcessInitialTime>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> AttackProcessInitialTime => AttackProcessInitialTimeC.Value;

		public bool TryGetAttackProcessInitialTime(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackProcessInitialTime component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackProcessInitialTime()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackProcessInitialTime() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackProcessInitialTime(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackProcessInitialTime() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackProcessCurrentTime AttackProcessCurrentTimeC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackProcessCurrentTime>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> AttackProcessCurrentTime => AttackProcessCurrentTimeC.Value;

		public bool TryGetAttackProcessCurrentTime(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackProcessCurrentTime component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackProcessCurrentTime()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackProcessCurrentTime() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackProcessCurrentTime(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackProcessCurrentTime() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.InAttackProcess InAttackProcessC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.InAttackProcess>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> InAttackProcess => InAttackProcessC.Value;

		public bool TryGetInAttackProcess(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.InAttackProcess component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddInAttackProcess()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.InAttackProcess() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddInAttackProcess(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.InAttackProcess() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackRange AttackRangeC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackRange>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> AttackRange => AttackRangeC.Value;

		public bool TryGetAttackRange(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackRange component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackRange()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackRange() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackRange(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackRange() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackDamage AttackDamageC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackDamage>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> AttackDamage => AttackDamageC.Value;

		public bool TryGetAttackDamage(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackDamage component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackDamage()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackDamage() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackDamage(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackDamage() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackDelayTime AttackDelayTimeC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackDelayTime>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> AttackDelayTime => AttackDelayTimeC.Value;

		public bool TryGetAttackDelayTime(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackDelayTime component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackDelayTime()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackDelayTime() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackDelayTime(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackDelayTime() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackDelayEndEvent AttackDelayEndEventC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackDelayEndEvent>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent AttackDelayEndEvent => AttackDelayEndEventC.Value;

		public bool TryGetAttackDelayEndEvent(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackDelayEndEvent component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackDelayEndEvent()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackDelayEndEvent() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackDelayEndEvent(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackDelayEndEvent() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.ShootPoint ShootPointC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.ShootPoint>();

		public UnityEngine.Transform ShootPoint => ShootPointC.Value;

		public bool TryGetShootPoint(out UnityEngine.Transform value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.ShootPoint component);
			if (result)
				value = component.Value;
			else
				value = default(UnityEngine.Transform);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddShootPoint(UnityEngine.Transform value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.ShootPoint() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.MustCancelAttack MustCancelAttackC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.MustCancelAttack>();

		public Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition MustCancelAttack => MustCancelAttackC.Value;

		public bool TryGetMustCancelAttack(out Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.MustCancelAttack component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddMustCancelAttack(Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.MustCancelAttack() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackCanceledEvent AttackCanceledEventC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackCanceledEvent>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent AttackCanceledEvent => AttackCanceledEventC.Value;

		public bool TryGetAttackCanceledEvent(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackCanceledEvent component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackCanceledEvent()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackCanceledEvent() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackCanceledEvent(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackCanceledEvent() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackCooldownInitialTime AttackCooldownInitialTimeC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackCooldownInitialTime>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> AttackCooldownInitialTime => AttackCooldownInitialTimeC.Value;

		public bool TryGetAttackCooldownInitialTime(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackCooldownInitialTime component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackCooldownInitialTime()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackCooldownInitialTime() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackCooldownInitialTime(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackCooldownInitialTime() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackCooldownCurrentTime AttackCooldownCurrentTimeC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackCooldownCurrentTime>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> AttackCooldownCurrentTime => AttackCooldownCurrentTimeC.Value;

		public bool TryGetAttackCooldownCurrentTime(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackCooldownCurrentTime component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackCooldownCurrentTime()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackCooldownCurrentTime() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackCooldownCurrentTime(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackCooldownCurrentTime() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.InAttackCooldown InAttackCooldownC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.InAttackCooldown>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> InAttackCooldown => InAttackCooldownC.Value;

		public bool TryGetInAttackCooldown(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.InAttackCooldown component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddInAttackCooldown()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.InAttackCooldown() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddInAttackCooldown(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.InAttackCooldown() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.DamageCooldown DamageCooldownC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.DamageCooldown>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> DamageCooldown => DamageCooldownC.Value;

		public bool TryGetDamageCooldown(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.DamageCooldown component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDamageCooldown()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.DamageCooldown() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDamageCooldown(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.DamageCooldown() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.DamageCooldownTimer DamageCooldownTimerC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.DamageCooldownTimer>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> DamageCooldownTimer => DamageCooldownTimerC.Value;

		public bool TryGetDamageCooldownTimer(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.DamageCooldownTimer component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDamageCooldownTimer()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.DamageCooldownTimer() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDamageCooldownTimer(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.DamageCooldownTimer() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.TakeDamageRequest TakeDamageRequestC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.TakeDamageRequest>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent<DamageData> TakeDamageRequest => TakeDamageRequestC.Value;

		public bool TryGetTakeDamageRequest(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent<DamageData> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.TakeDamageRequest component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent<DamageData>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddTakeDamageRequest()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.TakeDamageRequest() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent<DamageData>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddTakeDamageRequest(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent<DamageData> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.TakeDamageRequest() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.TakeDamageEvent TakeDamageEventC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.TakeDamageEvent>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent<DamageData> TakeDamageEvent => TakeDamageEventC.Value;

		public bool TryGetTakeDamageEvent(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent<DamageData> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.TakeDamageEvent component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent<DamageData>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddTakeDamageEvent()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.TakeDamageEvent() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent<DamageData>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddTakeDamageEvent(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent<DamageData> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.TakeDamageEvent() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.CanApplyDamage CanApplyDamageC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.CanApplyDamage>();

		public Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition CanApplyDamage => CanApplyDamageC.Value;

		public bool TryGetCanApplyDamage(out Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.CanApplyDamage component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddCanApplyDamage(Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.CanApplyDamage() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.AI.CurrentTarget CurrentTargetC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.AI.CurrentTarget>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity> CurrentTarget => CurrentTargetC.Value;

		public bool TryGetCurrentTarget(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.AI.CurrentTarget component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddCurrentTarget()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.AI.CurrentTarget() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddCurrentTarget(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.AI.CurrentTarget() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Common.RigidbodyComponent RigidbodyC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Common.RigidbodyComponent>();

		public UnityEngine.Rigidbody2D Rigidbody => RigidbodyC.Value;

		public bool TryGetRigidbody(out UnityEngine.Rigidbody2D value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Common.RigidbodyComponent component);
			if (result)
				value = component.Value;
			else
				value = default(UnityEngine.Rigidbody2D);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddRigidbody(UnityEngine.Rigidbody2D value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Common.RigidbodyComponent() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Common.TransformComponent TransformC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Common.TransformComponent>();

		public UnityEngine.Transform Transform => TransformC.Value;

		public bool TryGetTransform(out UnityEngine.Transform value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Common.TransformComponent component);
			if (result)
				value = component.Value;
			else
				value = default(UnityEngine.Transform);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddTransform(UnityEngine.Transform value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Common.TransformComponent() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Common.IsInvulnerable IsInvulnerableC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Common.IsInvulnerable>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> IsInvulnerable => IsInvulnerableC.Value;

		public bool TryGetIsInvulnerable(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Common.IsInvulnerable component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsInvulnerable()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Common.IsInvulnerable() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsInvulnerable(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Common.IsInvulnerable() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Common.FallActionThreshold FallActionThresholdC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Common.FallActionThreshold>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> FallActionThreshold => FallActionThresholdC.Value;

		public bool TryGetFallActionThreshold(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Common.FallActionThreshold component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddFallActionThreshold()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Common.FallActionThreshold() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddFallActionThreshold(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Common.FallActionThreshold() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Common.LookDirectionX LookDirectionXC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Common.LookDirectionX>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> LookDirectionX => LookDirectionXC.Value;

		public bool TryGetLookDirectionX(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Common.LookDirectionX component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddLookDirectionX()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Common.LookDirectionX() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddLookDirectionX(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Common.LookDirectionX() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Common.LockoutDuration LockoutDurationC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Common.LockoutDuration>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> LockoutDuration => LockoutDurationC.Value;

		public bool TryGetLockoutDuration(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Common.LockoutDuration component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddLockoutDuration()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Common.LockoutDuration() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddLockoutDuration(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Common.LockoutDuration() {Value = value}); 
		}

	}
}
