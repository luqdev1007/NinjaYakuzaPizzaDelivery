namespace Assets._Project.Develop.Runtime.Gameplay.EntitiesCore
{
	public partial class Entity
	{
		public Assets._Project.Develop.Runtime.Gameplay.Features.WallJumpFeature.CanWallJump CanWallJumpC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.WallJumpFeature.CanWallJump>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> CanWallJump => CanWallJumpC.Value;

		public bool TryGetCanWallJump(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.WallJumpFeature.CanWallJump component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddCanWallJump()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.WallJumpFeature.CanWallJump() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddCanWallJump(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.WallJumpFeature.CanWallJump() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.WallJumpFeature.WallJumpLockTimer WallJumpLockTimerC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.WallJumpFeature.WallJumpLockTimer>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> WallJumpLockTimer => WallJumpLockTimerC.Value;

		public bool TryGetWallJumpLockTimer(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.WallJumpFeature.WallJumpLockTimer component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddWallJumpLockTimer()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.WallJumpFeature.WallJumpLockTimer() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddWallJumpLockTimer(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.WallJumpFeature.WallJumpLockTimer() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.WallJumpFeature.IsWallJumping IsWallJumpingC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.WallJumpFeature.IsWallJumping>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> IsWallJumping => IsWallJumpingC.Value;

		public bool TryGetIsWallJumping(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.WallJumpFeature.IsWallJumping component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsWallJumping()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.WallJumpFeature.IsWallJumping() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsWallJumping(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.WallJumpFeature.IsWallJumping() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.WallJumpFeature.WallJumpParams WallJumpParamsC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.WallJumpFeature.WallJumpParams>();

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddWallJumpParams(System.Single minVelocityY,UnityEngine.Vector2 jumpForce,System.Single controlLockDuration)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.WallJumpFeature.WallJumpParams() {MinVelocityY = minVelocityY, JumpForce = jumpForce, ControlLockDuration = controlLockDuration}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.TeamsFeature.Team TeamC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.TeamsFeature.Team>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.Features.TeamsFeature.Teams> Team => TeamC.Value;

		public bool TryGetTeam(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.Features.TeamsFeature.Teams> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.TeamsFeature.Team component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.Features.TeamsFeature.Teams>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddTeam()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.TeamsFeature.Team() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.Features.TeamsFeature.Teams>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddTeam(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.Features.TeamsFeature.Teams> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.TeamsFeature.Team() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.SpawnFeature.SpawnInitialTime SpawnInitialTimeC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.SpawnFeature.SpawnInitialTime>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> SpawnInitialTime => SpawnInitialTimeC.Value;

		public bool TryGetSpawnInitialTime(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.SpawnFeature.SpawnInitialTime component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSpawnInitialTime()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.SpawnFeature.SpawnInitialTime() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSpawnInitialTime(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.SpawnFeature.SpawnInitialTime() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.SpawnFeature.SpawnCurrentTime SpawnCurrentTimeC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.SpawnFeature.SpawnCurrentTime>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> SpawnCurrentTime => SpawnCurrentTimeC.Value;

		public bool TryGetSpawnCurrentTime(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.SpawnFeature.SpawnCurrentTime component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSpawnCurrentTime()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.SpawnFeature.SpawnCurrentTime() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSpawnCurrentTime(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.SpawnFeature.SpawnCurrentTime() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.SpawnFeature.InSpawnProcess InSpawnProcessC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.SpawnFeature.InSpawnProcess>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> InSpawnProcess => InSpawnProcessC.Value;

		public bool TryGetInSpawnProcess(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.SpawnFeature.InSpawnProcess component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddInSpawnProcess()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.SpawnFeature.InSpawnProcess() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddInSpawnProcess(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.SpawnFeature.InSpawnProcess() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.IsOnSlope IsOnSlopeC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.IsOnSlope>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> IsOnSlope => IsOnSlopeC.Value;

		public bool TryGetIsOnSlope(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.IsOnSlope component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsOnSlope()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.IsOnSlope() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsOnSlope(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.IsOnSlope() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeAccumSpeed SlopeAccumSpeedC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeAccumSpeed>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> SlopeAccumSpeed => SlopeAccumSpeedC.Value;

		public bool TryGetSlopeAccumSpeed(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeAccumSpeed component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSlopeAccumSpeed()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeAccumSpeed() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSlopeAccumSpeed(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeAccumSpeed() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeMask SlopeMaskC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeMask>();

		public UnityEngine.LayerMask SlopeMask => SlopeMaskC.Value;

		public bool TryGetSlopeMask(out UnityEngine.LayerMask value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeMask component);
			if (result)
				value = component.Value;
			else
				value = default(UnityEngine.LayerMask);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSlopeMask(UnityEngine.LayerMask value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeMask() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeJumpForce SlopeJumpForceC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeJumpForce>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<UnityEngine.Vector2> SlopeJumpForce => SlopeJumpForceC.Value;

		public bool TryGetSlopeJumpForce(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<UnityEngine.Vector2> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeJumpForce component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<UnityEngine.Vector2>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSlopeJumpForce()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeJumpForce() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<UnityEngine.Vector2>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSlopeJumpForce(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<UnityEngine.Vector2> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeJumpForce() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeMinAngle SlopeMinAngleC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeMinAngle>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> SlopeMinAngle => SlopeMinAngleC.Value;

		public bool TryGetSlopeMinAngle(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeMinAngle component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSlopeMinAngle()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeMinAngle() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSlopeMinAngle(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeMinAngle() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeMaxAngle SlopeMaxAngleC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeMaxAngle>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> SlopeMaxAngle => SlopeMaxAngleC.Value;

		public bool TryGetSlopeMaxAngle(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeMaxAngle component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSlopeMaxAngle()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeMaxAngle() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSlopeMaxAngle(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeMaxAngle() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeDownhillBaseForce SlopeDownhillBaseForceC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeDownhillBaseForce>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> SlopeDownhillBaseForce => SlopeDownhillBaseForceC.Value;

		public bool TryGetSlopeDownhillBaseForce(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeDownhillBaseForce component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSlopeDownhillBaseForce()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeDownhillBaseForce() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSlopeDownhillBaseForce(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeDownhillBaseForce() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeBoostMultiplier SlopeBoostMultiplierC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeBoostMultiplier>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> SlopeBoostMultiplier => SlopeBoostMultiplierC.Value;

		public bool TryGetSlopeBoostMultiplier(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeBoostMultiplier component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSlopeBoostMultiplier()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeBoostMultiplier() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSlopeBoostMultiplier(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeBoostMultiplier() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeMagnetForce SlopeMagnetForceC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeMagnetForce>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> SlopeMagnetForce => SlopeMagnetForceC.Value;

		public bool TryGetSlopeMagnetForce(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeMagnetForce component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSlopeMagnetForce()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeMagnetForce() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSlopeMagnetForce(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeMagnetForce() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeMaxAccumSpeed SlopeMaxAccumSpeedC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeMaxAccumSpeed>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> SlopeMaxAccumSpeed => SlopeMaxAccumSpeedC.Value;

		public bool TryGetSlopeMaxAccumSpeed(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeMaxAccumSpeed component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSlopeMaxAccumSpeed()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeMaxAccumSpeed() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSlopeMaxAccumSpeed(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeMaxAccumSpeed() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeAccumGainRate SlopeAccumGainRateC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeAccumGainRate>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> SlopeAccumGainRate => SlopeAccumGainRateC.Value;

		public bool TryGetSlopeAccumGainRate(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeAccumGainRate component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSlopeAccumGainRate()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeAccumGainRate() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSlopeAccumGainRate(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeAccumGainRate() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeAccumDecayRate SlopeAccumDecayRateC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeAccumDecayRate>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> SlopeAccumDecayRate => SlopeAccumDecayRateC.Value;

		public bool TryGetSlopeAccumDecayRate(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeAccumDecayRate component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSlopeAccumDecayRate()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeAccumDecayRate() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSlopeAccumDecayRate(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeAccumDecayRate() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeSlideOffDelay SlopeSlideOffDelayC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeSlideOffDelay>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> SlopeSlideOffDelay => SlopeSlideOffDelayC.Value;

		public bool TryGetSlopeSlideOffDelay(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeSlideOffDelay component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSlopeSlideOffDelay()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeSlideOffDelay() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSlopeSlideOffDelay(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeSlideOffDelay() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeMinEjectVelocity SlopeMinEjectVelocityC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeMinEjectVelocity>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> SlopeMinEjectVelocity => SlopeMinEjectVelocityC.Value;

		public bool TryGetSlopeMinEjectVelocity(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeMinEjectVelocity component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSlopeMinEjectVelocity()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeMinEjectVelocity() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSlopeMinEjectVelocity(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeMinEjectVelocity() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeEjectForceMultiplier SlopeEjectForceMultiplierC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeEjectForceMultiplier>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> SlopeEjectForceMultiplier => SlopeEjectForceMultiplierC.Value;

		public bool TryGetSlopeEjectForceMultiplier(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeEjectForceMultiplier component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSlopeEjectForceMultiplier()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeEjectForceMultiplier() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSlopeEjectForceMultiplier(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeEjectForceMultiplier() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeAutoSlidePush SlopeAutoSlidePushC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeAutoSlidePush>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> SlopeAutoSlidePush => SlopeAutoSlidePushC.Value;

		public bool TryGetSlopeAutoSlidePush(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeAutoSlidePush component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSlopeAutoSlidePush()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeAutoSlidePush() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSlopeAutoSlidePush(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature.SlopeAutoSlidePush() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.SlideFeature.CanSlide CanSlideC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.SlideFeature.CanSlide>();

		public Assets._Project.Develop.Runtime.Utilites.Conditions.ICompositeCondition CanSlide => CanSlideC.Value;

		public bool TryGetCanSlide(out Assets._Project.Develop.Runtime.Utilites.Conditions.ICompositeCondition value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.SlideFeature.CanSlide component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Conditions.ICompositeCondition);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddCanSlide(Assets._Project.Develop.Runtime.Utilites.Conditions.ICompositeCondition value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.SlideFeature.CanSlide() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.SlideFeature.IsSliding IsSlidingC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.SlideFeature.IsSliding>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> IsSliding => IsSlidingC.Value;

		public bool TryGetIsSliding(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.SlideFeature.IsSliding component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsSliding()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.SlideFeature.IsSliding() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsSliding(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.SlideFeature.IsSliding() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.SlideFeature.SlideDuration SlideDurationC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.SlideFeature.SlideDuration>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> SlideDuration => SlideDurationC.Value;

		public bool TryGetSlideDuration(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.SlideFeature.SlideDuration component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSlideDuration()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.SlideFeature.SlideDuration() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSlideDuration(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.SlideFeature.SlideDuration() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.SlideFeature.SlideSpeed SlideSpeedC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.SlideFeature.SlideSpeed>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> SlideSpeed => SlideSpeedC.Value;

		public bool TryGetSlideSpeed(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.SlideFeature.SlideSpeed component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSlideSpeed()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.SlideFeature.SlideSpeed() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSlideSpeed(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.SlideFeature.SlideSpeed() {Value = value}); 
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

		public Assets._Project.Develop.Runtime.Utilites.Buffer<UnityEngine.Collider2D> ContactCollidersBuffer => ContactCollidersBufferC.Value;

		public bool TryGetContactCollidersBuffer(out Assets._Project.Develop.Runtime.Utilites.Buffer<UnityEngine.Collider2D> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.ContactCollidersBuffer component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Buffer<UnityEngine.Collider2D>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddContactCollidersBuffer(Assets._Project.Develop.Runtime.Utilites.Buffer<UnityEngine.Collider2D> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.ContactCollidersBuffer() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.ContactEntitiesBuffer ContactEntitiesBufferC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.ContactEntitiesBuffer>();

		public Assets._Project.Develop.Runtime.Utilites.Buffer<Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity> ContactEntitiesBuffer => ContactEntitiesBufferC.Value;

		public bool TryGetContactEntitiesBuffer(out Assets._Project.Develop.Runtime.Utilites.Buffer<Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.ContactEntitiesBuffer component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Buffer<Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddContactEntitiesBuffer(Assets._Project.Develop.Runtime.Utilites.Buffer<Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity> value)
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

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> IsTouchDeathMask => IsTouchDeathMaskC.Value;

		public bool TryGetIsTouchDeathMask(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.IsTouchDeathMask component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsTouchDeathMask()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.IsTouchDeathMask() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsTouchDeathMask(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.IsTouchDeathMask() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.IsTouchAnotherTeam IsTouchAnotherTeamC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.IsTouchAnotherTeam>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> IsTouchAnotherTeam => IsTouchAnotherTeamC.Value;

		public bool TryGetIsTouchAnotherTeam(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.IsTouchAnotherTeam component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsTouchAnotherTeam()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.IsTouchAnotherTeam() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsTouchAnotherTeam(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.IsTouchAnotherTeam() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.CanPlunge CanPlungeC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.CanPlunge>();

		public Assets._Project.Develop.Runtime.Utilites.Conditions.ICompositeCondition CanPlunge => CanPlungeC.Value;

		public bool TryGetCanPlunge(out Assets._Project.Develop.Runtime.Utilites.Conditions.ICompositeCondition value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.CanPlunge component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Conditions.ICompositeCondition);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddCanPlunge(Assets._Project.Develop.Runtime.Utilites.Conditions.ICompositeCondition value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.CanPlunge() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.IsPlunging IsPlungingC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.IsPlunging>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> IsPlunging => IsPlungingC.Value;

		public bool TryGetIsPlunging(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.IsPlunging component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsPlunging()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.IsPlunging() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsPlunging(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.IsPlunging() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.PlungeSpeed PlungeSpeedC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.PlungeSpeed>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> PlungeSpeed => PlungeSpeedC.Value;

		public bool TryGetPlungeSpeed(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.PlungeSpeed component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddPlungeSpeed()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.PlungeSpeed() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddPlungeSpeed(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.PlungeSpeed() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.PlungeAOERadius PlungeAOERadiusC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.PlungeAOERadius>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> PlungeAOERadius => PlungeAOERadiusC.Value;

		public bool TryGetPlungeAOERadius(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.PlungeAOERadius component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddPlungeAOERadius()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.PlungeAOERadius() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddPlungeAOERadius(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.PlungeAOERadius() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.PlungeAOEDamage PlungeAOEDamageC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.PlungeAOEDamage>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> PlungeAOEDamage => PlungeAOEDamageC.Value;

		public bool TryGetPlungeAOEDamage(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.PlungeAOEDamage component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddPlungeAOEDamage()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.PlungeAOEDamage() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddPlungeAOEDamage(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.PlungeAOEDamage() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.PlungeKnockbackForce PlungeKnockbackForceC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.PlungeKnockbackForce>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> PlungeKnockbackForce => PlungeKnockbackForceC.Value;

		public bool TryGetPlungeKnockbackForce(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.PlungeKnockbackForce component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddPlungeKnockbackForce()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.PlungeKnockbackForce() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddPlungeKnockbackForce(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature.PlungeKnockbackForce() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.PhysicsFeature.AngularDrag AngularDragC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.PhysicsFeature.AngularDrag>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> AngularDrag => AngularDragC.Value;

		public bool TryGetAngularDrag(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.PhysicsFeature.AngularDrag component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAngularDrag()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.PhysicsFeature.AngularDrag() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAngularDrag(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.PhysicsFeature.AngularDrag() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.PhysicsFeature.LinearDrag LinearDragC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.PhysicsFeature.LinearDrag>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> LinearDrag => LinearDragC.Value;

		public bool TryGetLinearDrag(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.PhysicsFeature.LinearDrag component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddLinearDrag()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.PhysicsFeature.LinearDrag() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddLinearDrag(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.PhysicsFeature.LinearDrag() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.PhysicsFeature.Velocity VelocityC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.PhysicsFeature.Velocity>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<UnityEngine.Vector2> Velocity => VelocityC.Value;

		public bool TryGetVelocity(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<UnityEngine.Vector2> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.PhysicsFeature.Velocity component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<UnityEngine.Vector2>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddVelocity()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.PhysicsFeature.Velocity() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<UnityEngine.Vector2>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddVelocity(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<UnityEngine.Vector2> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.PhysicsFeature.Velocity() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.PhysicsFeature.CanPhysicalyInteract CanPhysicalyInteractC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.PhysicsFeature.CanPhysicalyInteract>();

		public Assets._Project.Develop.Runtime.Utilites.Conditions.ICompositeCondition CanPhysicalyInteract => CanPhysicalyInteractC.Value;

		public bool TryGetCanPhysicalyInteract(out Assets._Project.Develop.Runtime.Utilites.Conditions.ICompositeCondition value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.PhysicsFeature.CanPhysicalyInteract component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Conditions.ICompositeCondition);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddCanPhysicalyInteract(Assets._Project.Develop.Runtime.Utilites.Conditions.ICompositeCondition value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.PhysicsFeature.CanPhysicalyInteract() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.CanFlip CanFlipC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.CanFlip>();

		public Assets._Project.Develop.Runtime.Utilites.Conditions.ICompositeCondition CanFlip => CanFlipC.Value;

		public bool TryGetCanFlip(out Assets._Project.Develop.Runtime.Utilites.Conditions.ICompositeCondition value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.CanFlip component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Conditions.ICompositeCondition);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddCanFlip(Assets._Project.Develop.Runtime.Utilites.Conditions.ICompositeCondition value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.CanFlip() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.CanJump CanJumpC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.CanJump>();

		public Assets._Project.Develop.Runtime.Utilites.Conditions.ICompositeCondition CanJump => CanJumpC.Value;

		public bool TryGetCanJump(out Assets._Project.Develop.Runtime.Utilites.Conditions.ICompositeCondition value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.CanJump component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Conditions.ICompositeCondition);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddCanJump(Assets._Project.Develop.Runtime.Utilites.Conditions.ICompositeCondition value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.CanJump() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.CanDash CanDashC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.CanDash>();

		public Assets._Project.Develop.Runtime.Utilites.Conditions.ICompositeCondition CanDash => CanDashC.Value;

		public bool TryGetCanDash(out Assets._Project.Develop.Runtime.Utilites.Conditions.ICompositeCondition value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.CanDash component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Conditions.ICompositeCondition);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddCanDash(Assets._Project.Develop.Runtime.Utilites.Conditions.ICompositeCondition value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.CanDash() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.MinFallVelocityForAction MinFallVelocityForActionC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.MinFallVelocityForAction>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> MinFallVelocityForAction => MinFallVelocityForActionC.Value;

		public bool TryGetMinFallVelocityForAction(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.MinFallVelocityForAction component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddMinFallVelocityForAction()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.MinFallVelocityForAction() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddMinFallVelocityForAction(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.MinFallVelocityForAction() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.Acceleration AccelerationC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.Acceleration>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> Acceleration => AccelerationC.Value;

		public bool TryGetAcceleration(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.Acceleration component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAcceleration()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.Acceleration() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAcceleration(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.Acceleration() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.Deceleration DecelerationC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.Deceleration>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> Deceleration => DecelerationC.Value;

		public bool TryGetDeceleration(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.Deceleration component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDeceleration()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.Deceleration() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDeceleration(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.Deceleration() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.MoveSpeedMin MoveSpeedMinC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.MoveSpeedMin>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> MoveSpeedMin => MoveSpeedMinC.Value;

		public bool TryGetMoveSpeedMin(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.MoveSpeedMin component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddMoveSpeedMin()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.MoveSpeedMin() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddMoveSpeedMin(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.MoveSpeedMin() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.MoveDirection MoveDirectionC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.MoveDirection>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<UnityEngine.Vector2> MoveDirection => MoveDirectionC.Value;

		public bool TryGetMoveDirection(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<UnityEngine.Vector2> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.MoveDirection component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<UnityEngine.Vector2>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddMoveDirection()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.MoveDirection() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<UnityEngine.Vector2>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddMoveDirection(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<UnityEngine.Vector2> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.MoveDirection() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.MoveSpeed MoveSpeedC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.MoveSpeed>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> MoveSpeed => MoveSpeedC.Value;

		public bool TryGetMoveSpeed(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.MoveSpeed component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddMoveSpeed()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.MoveSpeed() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddMoveSpeed(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.MoveSpeed() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.IsMoving IsMovingC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.IsMoving>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> IsMoving => IsMovingC.Value;

		public bool TryGetIsMoving(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.IsMoving component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsMoving()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.IsMoving() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsMoving(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.IsMoving() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.CanMove CanMoveC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.CanMove>();

		public Assets._Project.Develop.Runtime.Utilites.Conditions.ICompositeCondition CanMove => CanMoveC.Value;

		public bool TryGetCanMove(out Assets._Project.Develop.Runtime.Utilites.Conditions.ICompositeCondition value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.CanMove component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Conditions.ICompositeCondition);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddCanMove(Assets._Project.Develop.Runtime.Utilites.Conditions.ICompositeCondition value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.CanMove() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.RotationDirection RotationDirectionC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.RotationDirection>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<UnityEngine.Vector3> RotationDirection => RotationDirectionC.Value;

		public bool TryGetRotationDirection(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<UnityEngine.Vector3> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.RotationDirection component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<UnityEngine.Vector3>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddRotationDirection()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.RotationDirection() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<UnityEngine.Vector3>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddRotationDirection(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<UnityEngine.Vector3> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.RotationDirection() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.RotationSpeed RotationSpeedC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.RotationSpeed>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> RotationSpeed => RotationSpeedC.Value;

		public bool TryGetRotationSpeed(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.RotationSpeed component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddRotationSpeed()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.RotationSpeed() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddRotationSpeed(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.RotationSpeed() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.CanRotate CanRotateC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.CanRotate>();

		public Assets._Project.Develop.Runtime.Utilites.Conditions.ICompositeCondition CanRotate => CanRotateC.Value;

		public bool TryGetCanRotate(out Assets._Project.Develop.Runtime.Utilites.Conditions.ICompositeCondition value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.CanRotate component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Conditions.ICompositeCondition);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddCanRotate(Assets._Project.Develop.Runtime.Utilites.Conditions.ICompositeCondition value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.CanRotate() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.MainHero.IsMainHero IsMainHeroC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.MainHero.IsMainHero>();

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsMainHero()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.MainHero.IsMainHero() ); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.MainHero.AudioComponent AudioC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.MainHero.AudioComponent>();

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAudio(Assets._Project.Develop.Runtime.Utilites.AudioManagement.AudioService service)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.MainHero.AudioComponent() {Service = service}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.CollectRange CollectRangeC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.CollectRange>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> CollectRange => CollectRangeC.Value;

		public bool TryGetCollectRange(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.CollectRange component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddCollectRange()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.CollectRange() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddCollectRange(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.CollectRange() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.AutoDeleteInitialTime AutoDeleteInitialTimeC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.AutoDeleteInitialTime>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> AutoDeleteInitialTime => AutoDeleteInitialTimeC.Value;

		public bool TryGetAutoDeleteInitialTime(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.AutoDeleteInitialTime component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAutoDeleteInitialTime()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.AutoDeleteInitialTime() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAutoDeleteInitialTime(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.AutoDeleteInitialTime() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.AutoDeleteCurrentTime AutoDeleteCurrentTimeC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.AutoDeleteCurrentTime>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> AutoDeleteCurrentTime => AutoDeleteCurrentTimeC.Value;

		public bool TryGetAutoDeleteCurrentTime(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.AutoDeleteCurrentTime component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAutoDeleteCurrentTime()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.AutoDeleteCurrentTime() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAutoDeleteCurrentTime(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.AutoDeleteCurrentTime() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.IsPullable IsPullableC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.IsPullable>();

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsPullable()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.IsPullable() ); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.IsPullingProcess IsPullingProcessC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.IsPullingProcess>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> IsPullingProcess => IsPullingProcessC.Value;

		public bool TryGetIsPullingProcess(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.IsPullingProcess component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsPullingProcess()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.IsPullingProcess() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsPullingProcess(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.IsPullingProcess() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.IsCollected IsCollectedC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.IsCollected>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> IsCollected => IsCollectedC.Value;

		public bool TryGetIsCollected(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.IsCollected component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsCollected()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.IsCollected() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsCollected(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.IsCollected() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.Coins CoinsC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.Coins>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Int32> Coins => CoinsC.Value;

		public bool TryGetCoins(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Int32> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.Coins component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Int32>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddCoins()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.Coins() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Int32>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddCoins(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Int32> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.Coins() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.LootIsDropped LootIsDroppedC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.LootIsDropped>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> LootIsDropped => LootIsDroppedC.Value;

		public bool TryGetLootIsDropped(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.LootIsDropped component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddLootIsDropped()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.LootIsDropped() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddLootIsDropped(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.LootIsDropped() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.CanDropLoot CanDropLootC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.CanDropLoot>();

		public Assets._Project.Develop.Runtime.Utilites.Conditions.ICompositeCondition CanDropLoot => CanDropLootC.Value;

		public bool TryGetCanDropLoot(out Assets._Project.Develop.Runtime.Utilites.Conditions.ICompositeCondition value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.CanDropLoot component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Conditions.ICompositeCondition);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddCanDropLoot(Assets._Project.Develop.Runtime.Utilites.Conditions.ICompositeCondition value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.CanDropLoot() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.LootTag LootTagC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.LootTag>();

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddLootTag()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.LootTag() ); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.ExperienceValue ExperienceValueC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.ExperienceValue>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> ExperienceValue => ExperienceValueC.Value;

		public bool TryGetExperienceValue(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.ExperienceValue component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddExperienceValue()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.ExperienceValue() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddExperienceValue(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.ExperienceValue() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.CollectableInProcess CollectableInProcessC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.CollectableInProcess>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> CollectableInProcess => CollectableInProcessC.Value;

		public bool TryGetCollectableInProcess(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.CollectableInProcess component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddCollectableInProcess()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.CollectableInProcess() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddCollectableInProcess(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature.CollectableInProcess() {Value = value}); 
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

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> CurrentHealth => CurrentHealthC.Value;

		public bool TryGetCurrentHealth(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.CurrentHealth component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddCurrentHealth()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.CurrentHealth() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddCurrentHealth(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.CurrentHealth() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.MaxHealth MaxHealthC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.MaxHealth>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> MaxHealth => MaxHealthC.Value;

		public bool TryGetMaxHealth(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.MaxHealth component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddMaxHealth()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.MaxHealth() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddMaxHealth(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.MaxHealth() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.MustDie MustDieC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.MustDie>();

		public Assets._Project.Develop.Runtime.Utilites.Conditions.ICompositeCondition MustDie => MustDieC.Value;

		public bool TryGetMustDie(out Assets._Project.Develop.Runtime.Utilites.Conditions.ICompositeCondition value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.MustDie component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Conditions.ICompositeCondition);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddMustDie(Assets._Project.Develop.Runtime.Utilites.Conditions.ICompositeCondition value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.MustDie() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.MustSelfRelease MustSelfReleaseC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.MustSelfRelease>();

		public Assets._Project.Develop.Runtime.Utilites.Conditions.ICompositeCondition MustSelfRelease => MustSelfReleaseC.Value;

		public bool TryGetMustSelfRelease(out Assets._Project.Develop.Runtime.Utilites.Conditions.ICompositeCondition value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.MustSelfRelease component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Conditions.ICompositeCondition);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddMustSelfRelease(Assets._Project.Develop.Runtime.Utilites.Conditions.ICompositeCondition value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.MustSelfRelease() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.IsDead IsDeadC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.IsDead>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> IsDead => IsDeadC.Value;

		public bool TryGetIsDead(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.IsDead component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsDead()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.IsDead() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsDead(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.IsDead() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.DeathProcessInitialTime DeathProcessInitialTimeC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.DeathProcessInitialTime>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> DeathProcessInitialTime => DeathProcessInitialTimeC.Value;

		public bool TryGetDeathProcessInitialTime(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.DeathProcessInitialTime component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDeathProcessInitialTime()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.DeathProcessInitialTime() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDeathProcessInitialTime(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.DeathProcessInitialTime() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.DeathProcessCurrentTime DeathProcessCurrentTimeC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.DeathProcessCurrentTime>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> DeathProcessCurrentTime => DeathProcessCurrentTimeC.Value;

		public bool TryGetDeathProcessCurrentTime(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.DeathProcessCurrentTime component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDeathProcessCurrentTime()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.DeathProcessCurrentTime() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDeathProcessCurrentTime(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.DeathProcessCurrentTime() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.InDeathProcess InDeathProcessC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.InDeathProcess>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> InDeathProcess => InDeathProcessC.Value;

		public bool TryGetInDeathProcess(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.InDeathProcess component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddInDeathProcess()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle.InDeathProcess() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddInDeathProcess(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> value)
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

		public Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature.JumpForceMax JumpForceMaxC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature.JumpForceMax>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> JumpForceMax => JumpForceMaxC.Value;

		public bool TryGetJumpForceMax(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature.JumpForceMax component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddJumpForceMax()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature.JumpForceMax() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddJumpForceMax(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature.JumpForceMax() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature.JumpChargeTime JumpChargeTimeC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature.JumpChargeTime>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> JumpChargeTime => JumpChargeTimeC.Value;

		public bool TryGetJumpChargeTime(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature.JumpChargeTime component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddJumpChargeTime()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature.JumpChargeTime() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddJumpChargeTime(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature.JumpChargeTime() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature.JumpRequest JumpRequestC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature.JumpRequest>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveEvent JumpRequest => JumpRequestC.Value;

		public bool TryGetJumpRequest(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveEvent value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature.JumpRequest component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveEvent);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddJumpRequest()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature.JumpRequest() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveEvent() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddJumpRequest(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveEvent value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature.JumpRequest() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature.JumpForce JumpForceC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature.JumpForce>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> JumpForce => JumpForceC.Value;

		public bool TryGetJumpForce(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature.JumpForce component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddJumpForce()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature.JumpForce() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddJumpForce(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature.JumpForce() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature.IsGrounded IsGroundedC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature.IsGrounded>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> IsGrounded => IsGroundedC.Value;

		public bool TryGetIsGrounded(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature.IsGrounded component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsGrounded()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature.IsGrounded() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsGrounded(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature.IsGrounded() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature.GravityScale GravityScaleC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature.GravityScale>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> GravityScale => GravityScaleC.Value;

		public bool TryGetGravityScale(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature.GravityScale component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddGravityScale()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature.GravityScale() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddGravityScale(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature.GravityScale() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature.JumpsAvailable JumpsAvailableC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature.JumpsAvailable>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Int32> JumpsAvailable => JumpsAvailableC.Value;

		public bool TryGetJumpsAvailable(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Int32> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature.JumpsAvailable component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Int32>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddJumpsAvailable()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature.JumpsAvailable() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Int32>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddJumpsAvailable(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Int32> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature.JumpsAvailable() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature.MaxJumps MaxJumpsC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature.MaxJumps>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Int32> MaxJumps => MaxJumpsC.Value;

		public bool TryGetMaxJumps(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Int32> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature.MaxJumps component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Int32>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddMaxJumps()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature.MaxJumps() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Int32>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddMaxJumps(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Int32> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature.MaxJumps() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature.JumpEvent JumpEventC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature.JumpEvent>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveEvent JumpEvent => JumpEventC.Value;

		public bool TryGetJumpEvent(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveEvent value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature.JumpEvent component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveEvent);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddJumpEvent()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature.JumpEvent() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveEvent() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddJumpEvent(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveEvent value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature.JumpEvent() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature.DoubleJumpEvent DoubleJumpEventC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature.DoubleJumpEvent>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveEvent DoubleJumpEvent => DoubleJumpEventC.Value;

		public bool TryGetDoubleJumpEvent(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveEvent value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature.DoubleJumpEvent component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveEvent);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDoubleJumpEvent()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature.DoubleJumpEvent() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveEvent() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDoubleJumpEvent(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveEvent value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature.DoubleJumpEvent() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.HangWall.CanWallHang CanWallHangC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.HangWall.CanWallHang>();

		public Assets._Project.Develop.Runtime.Utilites.Conditions.ICompositeCondition CanWallHang => CanWallHangC.Value;

		public bool TryGetCanWallHang(out Assets._Project.Develop.Runtime.Utilites.Conditions.ICompositeCondition value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.HangWall.CanWallHang component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Conditions.ICompositeCondition);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddCanWallHang(Assets._Project.Develop.Runtime.Utilites.Conditions.ICompositeCondition value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.HangWall.CanWallHang() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.HangWall.IsWallHanging IsWallHangingC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.HangWall.IsWallHanging>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> IsWallHanging => IsWallHangingC.Value;

		public bool TryGetIsWallHanging(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.HangWall.IsWallHanging component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsWallHanging()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.HangWall.IsWallHanging() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsWallHanging(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.HangWall.IsWallHanging() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.HangWall.WallHangSlideSpeed WallHangSlideSpeedC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.HangWall.WallHangSlideSpeed>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> WallHangSlideSpeed => WallHangSlideSpeedC.Value;

		public bool TryGetWallHangSlideSpeed(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.HangWall.WallHangSlideSpeed component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddWallHangSlideSpeed()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.HangWall.WallHangSlideSpeed() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddWallHangSlideSpeed(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.HangWall.WallHangSlideSpeed() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.HangWall.WallHangLayer WallHangLayerC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.HangWall.WallHangLayer>();

		public UnityEngine.LayerMask WallHangLayer => WallHangLayerC.Value;

		public bool TryGetWallHangLayer(out UnityEngine.LayerMask value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.HangWall.WallHangLayer component);
			if (result)
				value = component.Value;
			else
				value = default(UnityEngine.LayerMask);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddWallHangLayer(UnityEngine.LayerMask value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.HangWall.WallHangLayer() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.HangWall.WallJumpForce WallJumpForceC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.HangWall.WallJumpForce>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<UnityEngine.Vector2> WallJumpForce => WallJumpForceC.Value;

		public bool TryGetWallJumpForce(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<UnityEngine.Vector2> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.HangWall.WallJumpForce component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<UnityEngine.Vector2>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddWallJumpForce()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.HangWall.WallJumpForce() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<UnityEngine.Vector2>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddWallJumpForce(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<UnityEngine.Vector2> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.HangWall.WallJumpForce() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.HangWall.WallDirection WallDirectionC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.HangWall.WallDirection>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> WallDirection => WallDirectionC.Value;

		public bool TryGetWallDirection(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.HangWall.WallDirection component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddWallDirection()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.HangWall.WallDirection() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddWallDirection(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.HangWall.WallDirection() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.GravityFeature.BaseGravity BaseGravityC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.GravityFeature.BaseGravity>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> BaseGravity => BaseGravityC.Value;

		public bool TryGetBaseGravity(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.GravityFeature.BaseGravity component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddBaseGravity()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GravityFeature.BaseGravity() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddBaseGravity(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GravityFeature.BaseGravity() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.GravityFeature.GravityModifier GravityModifierC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.GravityFeature.GravityModifier>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> GravityModifier => GravityModifierC.Value;

		public bool TryGetGravityModifier(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.GravityFeature.GravityModifier component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddGravityModifier()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GravityFeature.GravityModifier() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddGravityModifier(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GravityFeature.GravityModifier() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.GravityFeature.GravityDirection GravityDirectionC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.GravityFeature.GravityDirection>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<UnityEngine.Vector2> GravityDirection => GravityDirectionC.Value;

		public bool TryGetGravityDirection(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<UnityEngine.Vector2> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.GravityFeature.GravityDirection component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<UnityEngine.Vector2>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddGravityDirection()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GravityFeature.GravityDirection() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<UnityEngine.Vector2>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddGravityDirection(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<UnityEngine.Vector2> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GravityFeature.GravityDirection() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature.ThrowEvent ThrowEventC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature.ThrowEvent>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveEvent ThrowEvent => ThrowEventC.Value;

		public bool TryGetThrowEvent(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveEvent value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature.ThrowEvent component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveEvent);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddThrowEvent()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature.ThrowEvent() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveEvent() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddThrowEvent(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveEvent value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature.ThrowEvent() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature.CurrentThrowableIndex CurrentThrowableIndexC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature.CurrentThrowableIndex>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Int32> CurrentThrowableIndex => CurrentThrowableIndexC.Value;

		public bool TryGetCurrentThrowableIndex(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Int32> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature.CurrentThrowableIndex component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Int32>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddCurrentThrowableIndex()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature.CurrentThrowableIndex() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Int32>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddCurrentThrowableIndex(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Int32> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature.CurrentThrowableIndex() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature.GrappleCharges GrappleChargesC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature.GrappleCharges>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Int32> GrappleCharges => GrappleChargesC.Value;

		public bool TryGetGrappleCharges(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Int32> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature.GrappleCharges component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Int32>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddGrappleCharges()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature.GrappleCharges() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Int32>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddGrappleCharges(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Int32> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature.GrappleCharges() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature.ShurikenCharges ShurikenChargesC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature.ShurikenCharges>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Int32> ShurikenCharges => ShurikenChargesC.Value;

		public bool TryGetShurikenCharges(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Int32> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature.ShurikenCharges component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Int32>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddShurikenCharges()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature.ShurikenCharges() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Int32>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddShurikenCharges(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Int32> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature.ShurikenCharges() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature.SleepDartCharges SleepDartChargesC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature.SleepDartCharges>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Int32> SleepDartCharges => SleepDartChargesC.Value;

		public bool TryGetSleepDartCharges(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Int32> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature.SleepDartCharges component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Int32>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSleepDartCharges()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature.SleepDartCharges() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Int32>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSleepDartCharges(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Int32> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature.SleepDartCharges() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature.IsThrowing IsThrowingC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature.IsThrowing>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> IsThrowing => IsThrowingC.Value;

		public bool TryGetIsThrowing(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature.IsThrowing component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsThrowing()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature.IsThrowing() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsThrowing(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature.IsThrowing() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.GrappleMinDistance GrappleMinDistanceC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.GrappleMinDistance>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> GrappleMinDistance => GrappleMinDistanceC.Value;

		public bool TryGetGrappleMinDistance(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.GrappleMinDistance component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddGrappleMinDistance()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.GrappleMinDistance() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddGrappleMinDistance(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.GrappleMinDistance() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.GrappleArrivalBounce GrappleArrivalBounceC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.GrappleArrivalBounce>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> GrappleArrivalBounce => GrappleArrivalBounceC.Value;

		public bool TryGetGrappleArrivalBounce(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.GrappleArrivalBounce component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddGrappleArrivalBounce()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.GrappleArrivalBounce() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddGrappleArrivalBounce(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.GrappleArrivalBounce() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.GrappleMaxDistance GrappleMaxDistanceC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.GrappleMaxDistance>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> GrappleMaxDistance => GrappleMaxDistanceC.Value;

		public bool TryGetGrappleMaxDistance(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.GrappleMaxDistance component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddGrappleMaxDistance()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.GrappleMaxDistance() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddGrappleMaxDistance(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.GrappleMaxDistance() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.IsGrappledTarget IsGrappledTargetC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.IsGrappledTarget>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> IsGrappledTarget => IsGrappledTargetC.Value;

		public bool TryGetIsGrappledTarget(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.IsGrappledTarget component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsGrappledTarget()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.IsGrappledTarget() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsGrappledTarget(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.IsGrappledTarget() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.CanGrapple CanGrappleC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.CanGrapple>();

		public Assets._Project.Develop.Runtime.Utilites.Conditions.ICompositeCondition CanGrapple => CanGrappleC.Value;

		public bool TryGetCanGrapple(out Assets._Project.Develop.Runtime.Utilites.Conditions.ICompositeCondition value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.CanGrapple component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Conditions.ICompositeCondition);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddCanGrapple(Assets._Project.Develop.Runtime.Utilites.Conditions.ICompositeCondition value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.CanGrapple() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.IsGrappling IsGrapplingC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.IsGrappling>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> IsGrappling => IsGrapplingC.Value;

		public bool TryGetIsGrappling(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.IsGrappling component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsGrappling()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.IsGrappling() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsGrappling(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.IsGrappling() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.GrappleSpeed GrappleSpeedC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.GrappleSpeed>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> GrappleSpeed => GrappleSpeedC.Value;

		public bool TryGetGrappleSpeed(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.GrappleSpeed component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddGrappleSpeed()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.GrappleSpeed() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddGrappleSpeed(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.GrappleSpeed() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.GrappleProjectileSpeed GrappleProjectileSpeedC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.GrappleProjectileSpeed>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> GrappleProjectileSpeed => GrappleProjectileSpeedC.Value;

		public bool TryGetGrappleProjectileSpeed(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.GrappleProjectileSpeed component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddGrappleProjectileSpeed()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.GrappleProjectileSpeed() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddGrappleProjectileSpeed(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.GrappleProjectileSpeed() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.GrappleAnchorPoint GrappleAnchorPointC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.GrappleAnchorPoint>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<UnityEngine.Vector3> GrappleAnchorPoint => GrappleAnchorPointC.Value;

		public bool TryGetGrappleAnchorPoint(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<UnityEngine.Vector3> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.GrappleAnchorPoint component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<UnityEngine.Vector3>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddGrappleAnchorPoint()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.GrappleAnchorPoint() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<UnityEngine.Vector3>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddGrappleAnchorPoint(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<UnityEngine.Vector3> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.GrappleAnchorPoint() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.GrappleArriveDistance GrappleArriveDistanceC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.GrappleArriveDistance>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> GrappleArriveDistance => GrappleArriveDistanceC.Value;

		public bool TryGetGrappleArriveDistance(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.GrappleArriveDistance component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddGrappleArriveDistance()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.GrappleArriveDistance() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddGrappleArriveDistance(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature.GrappleArriveDistance() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.CanGlide CanGlideC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.CanGlide>();

		public Assets._Project.Develop.Runtime.Utilites.Conditions.ICompositeCondition CanGlide => CanGlideC.Value;

		public bool TryGetCanGlide(out Assets._Project.Develop.Runtime.Utilites.Conditions.ICompositeCondition value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.CanGlide component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Conditions.ICompositeCondition);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddCanGlide(Assets._Project.Develop.Runtime.Utilites.Conditions.ICompositeCondition value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.CanGlide() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.IsGliding IsGlidingC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.IsGliding>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> IsGliding => IsGlidingC.Value;

		public bool TryGetIsGliding(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.IsGliding component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsGliding()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.IsGliding() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsGliding(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.IsGliding() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.GlideMaxFallSpeed GlideMaxFallSpeedC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.GlideMaxFallSpeed>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> GlideMaxFallSpeed => GlideMaxFallSpeedC.Value;

		public bool TryGetGlideMaxFallSpeed(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.GlideMaxFallSpeed component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddGlideMaxFallSpeed()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.GlideMaxFallSpeed() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddGlideMaxFallSpeed(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.GlideMaxFallSpeed() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.GlideSpeedDamping GlideSpeedDampingC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.GlideSpeedDamping>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> GlideSpeedDamping => GlideSpeedDampingC.Value;

		public bool TryGetGlideSpeedDamping(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.GlideSpeedDamping component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddGlideSpeedDamping()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.GlideSpeedDamping() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddGlideSpeedDamping(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.GlideSpeedDamping() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.GlideBounceForce GlideBounceForceC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.GlideBounceForce>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> GlideBounceForce => GlideBounceForceC.Value;

		public bool TryGetGlideBounceForce(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.GlideBounceForce component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddGlideBounceForce()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.GlideBounceForce() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddGlideBounceForce(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.GlideBounceForce() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.GlideSnapSpeed GlideSnapSpeedC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.GlideSnapSpeed>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> GlideSnapSpeed => GlideSnapSpeedC.Value;

		public bool TryGetGlideSnapSpeed(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.GlideSnapSpeed component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddGlideSnapSpeed()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.GlideSnapSpeed() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddGlideSnapSpeed(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.GlideSnapSpeed() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.GlideSnapDuration GlideSnapDurationC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.GlideSnapDuration>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> GlideSnapDuration => GlideSnapDurationC.Value;

		public bool TryGetGlideSnapDuration(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.GlideSnapDuration component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddGlideSnapDuration()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.GlideSnapDuration() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddGlideSnapDuration(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.GlideSnapDuration() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.GlideHorizontalDrag GlideHorizontalDragC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.GlideHorizontalDrag>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> GlideHorizontalDrag => GlideHorizontalDragC.Value;

		public bool TryGetGlideHorizontalDrag(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.GlideHorizontalDrag component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddGlideHorizontalDrag()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.GlideHorizontalDrag() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddGlideHorizontalDrag(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature.GlideHorizontalDrag() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Effects.SleepTimer SleepTimerC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Effects.SleepTimer>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> SleepTimer => SleepTimerC.Value;

		public bool TryGetSleepTimer(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Effects.SleepTimer component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSleepTimer()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Effects.SleepTimer() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSleepTimer(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Effects.SleepTimer() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.DriveBugFeature.DriveAvailableJumps DriveAvailableJumpsC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.DriveBugFeature.DriveAvailableJumps>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Int32> DriveAvailableJumps => DriveAvailableJumpsC.Value;

		public bool TryGetDriveAvailableJumps(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Int32> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.DriveBugFeature.DriveAvailableJumps component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Int32>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDriveAvailableJumps()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.DriveBugFeature.DriveAvailableJumps() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Int32>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDriveAvailableJumps(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Int32> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.DriveBugFeature.DriveAvailableJumps() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.DriveBugFeature.IsDriveActive IsDriveActiveC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.DriveBugFeature.IsDriveActive>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> IsDriveActive => IsDriveActiveC.Value;

		public bool TryGetIsDriveActive(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.DriveBugFeature.IsDriveActive component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsDriveActive()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.DriveBugFeature.IsDriveActive() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsDriveActive(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.DriveBugFeature.IsDriveActive() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.DriveBugFeature.DriveDuration DriveDurationC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.DriveBugFeature.DriveDuration>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> DriveDuration => DriveDurationC.Value;

		public bool TryGetDriveDuration(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.DriveBugFeature.DriveDuration component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDriveDuration()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.DriveBugFeature.DriveDuration() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDriveDuration(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.DriveBugFeature.DriveDuration() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.DriveBugFeature.DriveGravityScale DriveGravityScaleC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.DriveBugFeature.DriveGravityScale>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> DriveGravityScale => DriveGravityScaleC.Value;

		public bool TryGetDriveGravityScale(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.DriveBugFeature.DriveGravityScale component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDriveGravityScale()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.DriveBugFeature.DriveGravityScale() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDriveGravityScale(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.DriveBugFeature.DriveGravityScale() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature.CometDashStateComponent CometDashStateC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature.CometDashStateComponent>();

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddCometDashState(System.Int32 maxCharges,Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Int32> currentCharges,Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> currentMultiplier,System.Single multiplierDegradation,System.Single baseCooldown,System.Single overheatCooldown,Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> cooldownTimer)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature.CometDashStateComponent() {MaxCharges = maxCharges, CurrentCharges = currentCharges, CurrentMultiplier = currentMultiplier, MultiplierDegradation = multiplierDegradation, BaseCooldown = baseCooldown, OverheatCooldown = overheatCooldown, CooldownTimer = cooldownTimer}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature.ChargedSlashProjectileTag ChargedSlashProjectileTagC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature.ChargedSlashProjectileTag>();

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddChargedSlashProjectileTag()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature.ChargedSlashProjectileTag() ); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature.DashForceMin DashForceMinC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature.DashForceMin>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> DashForceMin => DashForceMinC.Value;

		public bool TryGetDashForceMin(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature.DashForceMin component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDashForceMin()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature.DashForceMin() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDashForceMin(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature.DashForceMin() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature.DashForceMax DashForceMaxC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature.DashForceMax>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> DashForceMax => DashForceMaxC.Value;

		public bool TryGetDashForceMax(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature.DashForceMax component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDashForceMax()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature.DashForceMax() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDashForceMax(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature.DashForceMax() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature.DashChargeTime DashChargeTimeC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature.DashChargeTime>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> DashChargeTime => DashChargeTimeC.Value;

		public bool TryGetDashChargeTime(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature.DashChargeTime component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDashChargeTime()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature.DashChargeTime() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDashChargeTime(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature.DashChargeTime() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature.DashCooldown DashCooldownC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature.DashCooldown>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> DashCooldown => DashCooldownC.Value;

		public bool TryGetDashCooldown(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature.DashCooldown component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDashCooldown()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature.DashCooldown() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDashCooldown(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature.DashCooldown() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature.IsDashing IsDashingC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature.IsDashing>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> IsDashing => IsDashingC.Value;

		public bool TryGetIsDashing(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature.IsDashing component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsDashing()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature.IsDashing() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsDashing(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature.IsDashing() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature.DashDuration DashDurationC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature.DashDuration>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> DashDuration => DashDurationC.Value;

		public bool TryGetDashDuration(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature.DashDuration component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDashDuration()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature.DashDuration() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDashDuration(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature.DashDuration() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature.AirDashMultiplier AirDashMultiplierC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature.AirDashMultiplier>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> AirDashMultiplier => AirDashMultiplierC.Value;

		public bool TryGetAirDashMultiplier(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature.AirDashMultiplier component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAirDashMultiplier()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature.AirDashMultiplier() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAirDashMultiplier(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature.AirDashMultiplier() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature.AirDashVerticalBoost AirDashVerticalBoostC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature.AirDashVerticalBoost>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> AirDashVerticalBoost => AirDashVerticalBoostC.Value;

		public bool TryGetAirDashVerticalBoost(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature.AirDashVerticalBoost component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAirDashVerticalBoost()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature.AirDashVerticalBoost() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAirDashVerticalBoost(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature.AirDashVerticalBoost() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature.DashDamage DashDamageC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature.DashDamage>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> DashDamage => DashDamageC.Value;

		public bool TryGetDashDamage(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature.DashDamage component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDashDamage()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature.DashDamage() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDashDamage(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature.DashDamage() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature.DashHitboxSize DashHitboxSizeC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature.DashHitboxSize>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<UnityEngine.Vector2> DashHitboxSize => DashHitboxSizeC.Value;

		public bool TryGetDashHitboxSize(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<UnityEngine.Vector2> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature.DashHitboxSize component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<UnityEngine.Vector2>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDashHitboxSize()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature.DashHitboxSize() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<UnityEngine.Vector2>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDashHitboxSize(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<UnityEngine.Vector2> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature.DashHitboxSize() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.ContactTakeDamage.BodyContactDamage BodyContactDamageC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.ContactTakeDamage.BodyContactDamage>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> BodyContactDamage => BodyContactDamageC.Value;

		public bool TryGetBodyContactDamage(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.ContactTakeDamage.BodyContactDamage component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddBodyContactDamage()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.ContactTakeDamage.BodyContactDamage() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddBodyContactDamage(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.ContactTakeDamage.BodyContactDamage() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.SuccessfulHitEvent SuccessfulHitEventC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.SuccessfulHitEvent>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveEvent SuccessfulHitEvent => SuccessfulHitEventC.Value;

		public bool TryGetSuccessfulHitEvent(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveEvent value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.SuccessfulHitEvent component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveEvent);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSuccessfulHitEvent()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.SuccessfulHitEvent() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveEvent() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSuccessfulHitEvent(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveEvent value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.SuccessfulHitEvent() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackHitStopScale AttackHitStopScaleC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackHitStopScale>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> AttackHitStopScale => AttackHitStopScaleC.Value;

		public bool TryGetAttackHitStopScale(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackHitStopScale component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackHitStopScale()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackHitStopScale() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackHitStopScale(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackHitStopScale() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackHitStopDuration AttackHitStopDurationC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackHitStopDuration>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> AttackHitStopDuration => AttackHitStopDurationC.Value;

		public bool TryGetAttackHitStopDuration(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackHitStopDuration component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackHitStopDuration()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackHitStopDuration() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackHitStopDuration(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackHitStopDuration() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackHitBounceForce AttackHitBounceForceC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackHitBounceForce>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> AttackHitBounceForce => AttackHitBounceForceC.Value;

		public bool TryGetAttackHitBounceForce(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackHitBounceForce component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackHitBounceForce()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackHitBounceForce() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackHitBounceForce(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackHitBounceForce() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.GroundHitBounceModifiers GroundHitBounceModifiersC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.GroundHitBounceModifiers>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<UnityEngine.Vector2> GroundHitBounceModifiers => GroundHitBounceModifiersC.Value;

		public bool TryGetGroundHitBounceModifiers(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<UnityEngine.Vector2> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.GroundHitBounceModifiers component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<UnityEngine.Vector2>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddGroundHitBounceModifiers()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.GroundHitBounceModifiers() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<UnityEngine.Vector2>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddGroundHitBounceModifiers(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<UnityEngine.Vector2> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.GroundHitBounceModifiers() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AirHitBounceModifiers AirHitBounceModifiersC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AirHitBounceModifiers>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<UnityEngine.Vector2> AirHitBounceModifiers => AirHitBounceModifiersC.Value;

		public bool TryGetAirHitBounceModifiers(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<UnityEngine.Vector2> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AirHitBounceModifiers component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<UnityEngine.Vector2>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAirHitBounceModifiers()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AirHitBounceModifiers() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<UnityEngine.Vector2>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAirHitBounceModifiers(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<UnityEngine.Vector2> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AirHitBounceModifiers() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackEnemyMask AttackEnemyMaskC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackEnemyMask>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<UnityEngine.LayerMask> AttackEnemyMask => AttackEnemyMaskC.Value;

		public bool TryGetAttackEnemyMask(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<UnityEngine.LayerMask> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackEnemyMask component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<UnityEngine.LayerMask>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackEnemyMask()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackEnemyMask() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<UnityEngine.LayerMask>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackEnemyMask(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<UnityEngine.LayerMask> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackEnemyMask() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackInvulnerabilityDuration AttackInvulnerabilityDurationC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackInvulnerabilityDuration>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> AttackInvulnerabilityDuration => AttackInvulnerabilityDurationC.Value;

		public bool TryGetAttackInvulnerabilityDuration(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackInvulnerabilityDuration component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackInvulnerabilityDuration()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackInvulnerabilityDuration() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackInvulnerabilityDuration(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackInvulnerabilityDuration() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackInvulnerabilityTimer AttackInvulnerabilityTimerC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackInvulnerabilityTimer>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> AttackInvulnerabilityTimer => AttackInvulnerabilityTimerC.Value;

		public bool TryGetAttackInvulnerabilityTimer(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackInvulnerabilityTimer component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackInvulnerabilityTimer()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackInvulnerabilityTimer() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackInvulnerabilityTimer(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackInvulnerabilityTimer() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.IsAttackInvulnerable IsAttackInvulnerableC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.IsAttackInvulnerable>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> IsAttackInvulnerable => IsAttackInvulnerableC.Value;

		public bool TryGetIsAttackInvulnerable(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.IsAttackInvulnerable component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsAttackInvulnerable()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.IsAttackInvulnerable() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsAttackInvulnerable(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.IsAttackInvulnerable() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.StartAttackRequest StartAttackRequestC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.StartAttackRequest>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveEvent StartAttackRequest => StartAttackRequestC.Value;

		public bool TryGetStartAttackRequest(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveEvent value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.StartAttackRequest component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveEvent);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddStartAttackRequest()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.StartAttackRequest() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveEvent() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddStartAttackRequest(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveEvent value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.StartAttackRequest() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.StartAttackEvent StartAttackEventC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.StartAttackEvent>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveEvent StartAttackEvent => StartAttackEventC.Value;

		public bool TryGetStartAttackEvent(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveEvent value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.StartAttackEvent component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveEvent);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddStartAttackEvent()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.StartAttackEvent() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveEvent() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddStartAttackEvent(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveEvent value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.StartAttackEvent() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.CanStartAttack CanStartAttackC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.CanStartAttack>();

		public Assets._Project.Develop.Runtime.Utilites.Conditions.ICompositeCondition CanStartAttack => CanStartAttackC.Value;

		public bool TryGetCanStartAttack(out Assets._Project.Develop.Runtime.Utilites.Conditions.ICompositeCondition value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.CanStartAttack component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Conditions.ICompositeCondition);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddCanStartAttack(Assets._Project.Develop.Runtime.Utilites.Conditions.ICompositeCondition value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.CanStartAttack() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.EndAttackEvent EndAttackEventC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.EndAttackEvent>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveEvent EndAttackEvent => EndAttackEventC.Value;

		public bool TryGetEndAttackEvent(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveEvent value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.EndAttackEvent component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveEvent);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddEndAttackEvent()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.EndAttackEvent() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveEvent() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddEndAttackEvent(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveEvent value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.EndAttackEvent() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackProcessInitialTime AttackProcessInitialTimeC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackProcessInitialTime>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> AttackProcessInitialTime => AttackProcessInitialTimeC.Value;

		public bool TryGetAttackProcessInitialTime(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackProcessInitialTime component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackProcessInitialTime()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackProcessInitialTime() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackProcessInitialTime(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackProcessInitialTime() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackProcessCurrentTime AttackProcessCurrentTimeC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackProcessCurrentTime>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> AttackProcessCurrentTime => AttackProcessCurrentTimeC.Value;

		public bool TryGetAttackProcessCurrentTime(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackProcessCurrentTime component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackProcessCurrentTime()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackProcessCurrentTime() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackProcessCurrentTime(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackProcessCurrentTime() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.InAttackProcess InAttackProcessC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.InAttackProcess>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> InAttackProcess => InAttackProcessC.Value;

		public bool TryGetInAttackProcess(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.InAttackProcess component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddInAttackProcess()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.InAttackProcess() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddInAttackProcess(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.InAttackProcess() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackRange AttackRangeC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackRange>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> AttackRange => AttackRangeC.Value;

		public bool TryGetAttackRange(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackRange component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackRange()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackRange() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackRange(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackRange() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackDamage AttackDamageC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackDamage>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> AttackDamage => AttackDamageC.Value;

		public bool TryGetAttackDamage(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackDamage component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackDamage()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackDamage() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackDamage(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackDamage() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackDelayTime AttackDelayTimeC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackDelayTime>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> AttackDelayTime => AttackDelayTimeC.Value;

		public bool TryGetAttackDelayTime(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackDelayTime component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackDelayTime()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackDelayTime() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackDelayTime(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackDelayTime() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackDelayEndEvent AttackDelayEndEventC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackDelayEndEvent>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveEvent AttackDelayEndEvent => AttackDelayEndEventC.Value;

		public bool TryGetAttackDelayEndEvent(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveEvent value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackDelayEndEvent component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveEvent);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackDelayEndEvent()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackDelayEndEvent() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveEvent() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackDelayEndEvent(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveEvent value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackDelayEndEvent() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.InstantAttackDamage InstantAttackDamageC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.InstantAttackDamage>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> InstantAttackDamage => InstantAttackDamageC.Value;

		public bool TryGetInstantAttackDamage(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.InstantAttackDamage component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddInstantAttackDamage()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.InstantAttackDamage() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddInstantAttackDamage(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.InstantAttackDamage() {Value = value}); 
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

		public Assets._Project.Develop.Runtime.Utilites.Conditions.ICompositeCondition MustCancelAttack => MustCancelAttackC.Value;

		public bool TryGetMustCancelAttack(out Assets._Project.Develop.Runtime.Utilites.Conditions.ICompositeCondition value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.MustCancelAttack component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Conditions.ICompositeCondition);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddMustCancelAttack(Assets._Project.Develop.Runtime.Utilites.Conditions.ICompositeCondition value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.MustCancelAttack() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackCanceledEvent AttackCanceledEventC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackCanceledEvent>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveEvent AttackCanceledEvent => AttackCanceledEventC.Value;

		public bool TryGetAttackCanceledEvent(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveEvent value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackCanceledEvent component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveEvent);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackCanceledEvent()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackCanceledEvent() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveEvent() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackCanceledEvent(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveEvent value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackCanceledEvent() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackCooldownInitialTime AttackCooldownInitialTimeC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackCooldownInitialTime>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> AttackCooldownInitialTime => AttackCooldownInitialTimeC.Value;

		public bool TryGetAttackCooldownInitialTime(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackCooldownInitialTime component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackCooldownInitialTime()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackCooldownInitialTime() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackCooldownInitialTime(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackCooldownInitialTime() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackCooldownCurrentTime AttackCooldownCurrentTimeC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackCooldownCurrentTime>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> AttackCooldownCurrentTime => AttackCooldownCurrentTimeC.Value;

		public bool TryGetAttackCooldownCurrentTime(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackCooldownCurrentTime component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackCooldownCurrentTime()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackCooldownCurrentTime() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackCooldownCurrentTime(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackCooldownCurrentTime() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.InAttackCooldown InAttackCooldownC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.InAttackCooldown>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> InAttackCooldown => InAttackCooldownC.Value;

		public bool TryGetInAttackCooldown(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.InAttackCooldown component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddInAttackCooldown()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.InAttackCooldown() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddInAttackCooldown(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.InAttackCooldown() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.DamageCooldown DamageCooldownC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.DamageCooldown>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> DamageCooldown => DamageCooldownC.Value;

		public bool TryGetDamageCooldown(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.DamageCooldown component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDamageCooldown()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.DamageCooldown() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDamageCooldown(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.DamageCooldown() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.DamageCooldownTimer DamageCooldownTimerC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.DamageCooldownTimer>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> DamageCooldownTimer => DamageCooldownTimerC.Value;

		public bool TryGetDamageCooldownTimer(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.DamageCooldownTimer component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDamageCooldownTimer()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.DamageCooldownTimer() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDamageCooldownTimer(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.DamageCooldownTimer() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.TakeDamageRequest TakeDamageRequestC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.TakeDamageRequest>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveEvent<DamageData> TakeDamageRequest => TakeDamageRequestC.Value;

		public bool TryGetTakeDamageRequest(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveEvent<DamageData> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.TakeDamageRequest component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveEvent<DamageData>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddTakeDamageRequest()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.TakeDamageRequest() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveEvent<DamageData>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddTakeDamageRequest(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveEvent<DamageData> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.TakeDamageRequest() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.TakeDamageEvent TakeDamageEventC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.TakeDamageEvent>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveEvent<DamageData> TakeDamageEvent => TakeDamageEventC.Value;

		public bool TryGetTakeDamageEvent(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveEvent<DamageData> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.TakeDamageEvent component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveEvent<DamageData>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddTakeDamageEvent()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.TakeDamageEvent() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveEvent<DamageData>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddTakeDamageEvent(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveEvent<DamageData> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.TakeDamageEvent() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.CanApplyDamage CanApplyDamageC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.CanApplyDamage>();

		public Assets._Project.Develop.Runtime.Utilites.Conditions.ICompositeCondition CanApplyDamage => CanApplyDamageC.Value;

		public bool TryGetCanApplyDamage(out Assets._Project.Develop.Runtime.Utilites.Conditions.ICompositeCondition value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.CanApplyDamage component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Conditions.ICompositeCondition);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddCanApplyDamage(Assets._Project.Develop.Runtime.Utilites.Conditions.ICompositeCondition value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.CanApplyDamage() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.AI.CurrentTarget CurrentTargetC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.AI.CurrentTarget>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity> CurrentTarget => CurrentTargetC.Value;

		public bool TryGetCurrentTarget(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.AI.CurrentTarget component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddCurrentTarget()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.AI.CurrentTarget() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddCurrentTarget(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity> value)
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

		public Assets._Project.Develop.Runtime.Gameplay.Common.IsAsleep IsAsleepC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Common.IsAsleep>();

		public Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> IsAsleep => IsAsleepC.Value;

		public bool TryGetIsAsleep(out Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Common.IsAsleep component);
			if (result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsAsleep()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Common.IsAsleep() { Value = new Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsAsleep(Assets._Project.Develop.Runtime.Utilites.Reactive.ReactiveVariable<System.Boolean> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Common.IsAsleep() {Value = value}); 
		}

	}
}
