﻿using System;
using System.Collections.Generic;
using AIs;
using CJTools;
using Enums;
using UnityEngine;

public class WeaponMeleeController : WeaponController
{
	public static WeaponMeleeController Get()
	{
		return WeaponMeleeController.s_Instance;
	}

	protected override void Awake()
	{
		base.Awake();
		if (base.GetType() == typeof(WeaponMeleeController))
		{
			WeaponMeleeController.s_Instance = this;
			this.m_ControllerType = PlayerControllerType.WeaponMelee;
		}
		this.m_ArmTransform = this.m_Player.gameObject.transform.FindDeepChild("mixamorig:Arm.R");
		this.m_RHand = this.m_Player.gameObject.transform.FindDeepChild("mixamorig:Hand.R");
		this.m_HeadTransform = this.m_Player.gameObject.transform.FindDeepChild("mixamorig:Head");
		this.m_PredictionDataClipNames[291] = new List<string>();
		this.m_PredictionDataClipNames[291].Add("PL|AttackRightSlantSwing");
		this.m_PredictionDataClipNames[291].Add("PL|AttackLeftComboSwing");
		this.m_PredictionDataClipNames[291].Add("PL|AttackUpSwing");
		this.m_PredictionDataClipNames[312] = new List<string>();
		this.m_PredictionDataClipNames[312].Add("PL|AttackRightSlantSwing");
		this.m_PredictionDataClipNames[312].Add("PL|AttackLeftComboSwing");
		this.m_PredictionDataClipNames[312].Add("PL|AttackUpSwing");
		this.m_PredictionDataClipNames[288] = new List<string>();
		this.m_PredictionDataClipNames[288].Add("PL|BladeStoneAttackRightSwing");
		this.m_PredictionDataClipNames[288].Add("PL|AttackUpBladeSwing");
		this.m_PredictionDataClipNames[288].Add("PL|BladeStoneCombo");
		this.m_PredictionDataClipNames[289] = new List<string>();
		this.m_PredictionDataClipNames[289].Add("PL|BladeStoneAttackRightSwing");
		this.m_PredictionDataClipNames[289].Add("PL|AttackUpBladeSwing");
		this.m_PredictionDataClipNames[289].Add("PL|BladeStoneCombo");
		this.m_PredictionDataClipNames[331] = new List<string>();
		this.m_PredictionDataClipNames[331].Add("PL|BladeStoneAttackRightSwing");
		this.m_PredictionDataClipNames[331].Add("PL|AttackUpBladeSwing");
		this.m_PredictionDataClipNames[331].Add("PL|BladeStoneCombo");
		this.m_PredictionDataClipNames[287] = new List<string>();
		this.m_PredictionDataClipNames[287].Add("PL|BladeStoneAttackRightSwing");
		this.m_PredictionDataClipNames[287].Add("PL|AttackUpBladeSwing");
		this.m_PredictionDataClipNames[287].Add("PL|BladeStoneCombo");
		this.m_PredictionDataClipNames[315] = new List<string>();
		this.m_PredictionDataClipNames[315].Add("PL|BladeStoneAttackRightSwing");
		this.m_PredictionDataClipNames[315].Add("PL|AttackUpBladeSwing");
		this.m_PredictionDataClipNames[315].Add("PL|BladeStoneCombo");
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		Transform[] array = new Transform[]
		{
			this.m_Player.gameObject.transform.FindDeepChild("mixamorig:Arm.R"),
			this.m_Player.gameObject.transform.FindDeepChild("mixamorig:ForeArm.R"),
			this.m_Player.gameObject.transform.FindDeepChild("mixamorig:Hand.R")
		};
		this.SetupPredictionData();
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		this.m_Animator.SetBool(this.m_BAttackRight, false);
		this.m_Animator.SetBool(this.m_BAttackUp, false);
		this.m_Animator.SetBool(this.m_BAttackLeft, false);
	}

	public override void ControllerUpdate()
	{
		base.ControllerUpdate();
		this.UpdateAttackRelease();
		this.UpdateRelease();
		this.UpdateSwingSpeedMultiplier();
	}

	private void UpdateSwingSpeedMultiplier()
	{
		AnimatorTransitionInfo animatorTransitionInfo = this.m_Animator.GetAnimatorTransitionInfo(1);
		AnimatorStateInfo currentAnimatorStateInfo = this.m_Animator.GetCurrentAnimatorStateInfo(1);
		bool flag = animatorTransitionInfo.userNameHash == this.m_NRightSwingToReleaseTransition || animatorTransitionInfo.userNameHash == this.m_NLeftSwingToReleaseTransition || currentAnimatorStateInfo.shortNameHash == this.m_ReleaseStateRight1 || currentAnimatorStateInfo.shortNameHash == this.m_ReleaseStateUp1 || currentAnimatorStateInfo.shortNameHash == this.m_ReleaseStateLeft1;
		if (flag)
		{
			this.m_Animator.SetFloat(this.m_NSwingSpeedMultiplier, 0f);
		}
		else
		{
			this.m_Animator.SetFloat(this.m_NSwingSpeedMultiplier, 1f);
		}
	}

	private void UpdateAttackRelease()
	{
		if (this.m_AttackRelease)
		{
			this.m_AttackRelease = false;
		}
	}

	public override void OnAnimEvent(AnimEventID id)
	{
		base.OnAnimEvent(id);
		if (id == AnimEventID.MeleeRightAttackStart)
		{
			this.m_AttackDirection = AttackDirection.Right;
			this.m_Animator.SetBool(this.m_BAttackLeft, false);
			base.SetState(WeaponControllerState.Swing);
		}
		else if (id == AnimEventID.MeleeLeftAttackStart)
		{
			this.m_AttackDirection = AttackDirection.Left;
			this.m_Animator.SetBool(this.m_BAttackRight, false);
			base.SetState(WeaponControllerState.Swing);
		}
		else if (id == AnimEventID.MeleeUpAttackStart)
		{
			this.m_AttackDirection = AttackDirection.Up;
			base.SetState(WeaponControllerState.Swing);
		}
		else if (id == AnimEventID.MeleeRightAttackEnd)
		{
			this.PlayerMeleeRightAttackEnd();
		}
		else if (id == AnimEventID.MeleeUpAttackEnd)
		{
			this.PlayerMeleeUpAttackEnd();
		}
		else if (id == AnimEventID.MeleeLeftAttackEnd)
		{
			this.PlayerMeleeLeftAttackEnd();
		}
	}

	public override bool IsAttack()
	{
		AnimatorStateInfo currentAnimatorStateInfo = this.m_Animator.GetCurrentAnimatorStateInfo(1);
		return currentAnimatorStateInfo.shortNameHash == this.m_NMeleeAttackRightWindUp || currentAnimatorStateInfo.shortNameHash == this.m_NMeleeAttackRightSwing || currentAnimatorStateInfo.shortNameHash == this.m_NMeleeAttackLeftSwing || currentAnimatorStateInfo.shortNameHash == this.m_NMeleeAttackUpWindUp || currentAnimatorStateInfo.shortNameHash == this.m_NMeleeAttackUpSwing;
	}

	public override void OnInputAction(InputsManager.InputAction action)
	{
		if (action == InputsManager.InputAction.MeleeAttack)
		{
			this.Attack();
		}
	}

	protected override void OnHit()
	{
		base.OnHit();
	}

	protected override void Attack()
	{
		if (!this.CanAttack())
		{
			if (!this.m_ComboBlocked && !PlayerConditionModule.Get().IsStaminaLevel(this.m_BlockAttackStaminaLevel))
			{
				AnimatorStateInfo currentAnimatorStateInfo = this.m_Animator.GetCurrentAnimatorStateInfo(1);
				if ((currentAnimatorStateInfo.shortNameHash == this.m_NMeleeAttackRightSwing && currentAnimatorStateInfo.normalizedTime > 0.2f) || currentAnimatorStateInfo.shortNameHash == this.m_ReleaseStateRight1 || (currentAnimatorStateInfo.shortNameHash == this.m_NMeleeAttackRightReturn && currentAnimatorStateInfo.normalizedTime < 0.5f))
				{
					this.m_ReleaseComboScheduled = true;
				}
				if (currentAnimatorStateInfo.shortNameHash == this.m_NMeleeAttackRightSwing && currentAnimatorStateInfo.normalizedTime > 0.5f && currentAnimatorStateInfo.normalizedTime < 1f && !this.m_Animator.GetBool(this.m_BAttackLeft))
				{
					this.m_ComboScheduled = true;
				}
				else if (currentAnimatorStateInfo.shortNameHash == this.m_ReleaseStateRight1)
				{
					this.m_ComboScheduled = true;
				}
				else if (currentAnimatorStateInfo.shortNameHash == this.m_NMeleeAttackLeftSwing && currentAnimatorStateInfo.normalizedTime > 0.5f && currentAnimatorStateInfo.normalizedTime < 1f && !this.m_Animator.GetBool(this.m_BAttackRight))
				{
					this.m_ComboScheduled = true;
				}
				else if (this.m_Animator.GetBool(this.m_BAttackRightRelease))
				{
					this.m_Animator.SetBool(this.m_BAttackRight, true);
					this.m_ComboScheduled = true;
				}
			}
			return;
		}
		if (!this.IsAttack())
		{
			base.Attack();
			bool flag = false;
			if (!TorchController.Get().IsActive() && (this.m_Player.GetFPPController().IsDuck() || this.m_Player.GetLookController().m_LookDev.y < -40f))
			{
				flag = true;
			}
			if (flag)
			{
				this.m_Animator.SetBool(this.m_BAttackUp, true);
				this.m_AttackDirection = AttackDirection.Up;
			}
			else
			{
				this.m_Animator.SetBool(this.m_BAttackRight, true);
				this.m_AttackDirection = AttackDirection.Right;
			}
			if (Time.time - this.m_LastAttackSoundTime > 0.3f)
			{
				this.m_AudioModule.PlayAttackSound(1f, false);
				this.m_LastAttackSoundTime = Time.time;
			}
		}
		this.m_Animator.SetBool(this.m_BAttackRightRelease, false);
		this.m_Animator.SetBool(this.m_BAttackLeftRelease, false);
		this.m_Animator.SetBool(this.m_BAttackUpRelease, false);
		this.m_LastAttackTime = Time.time;
		this.m_ComboScheduled = false;
		this.m_ComboBlocked = false;
		this.m_ReleaseComboScheduled = false;
		this.m_ReleaseCombo = false;
		this.m_HitObjects.Clear();
	}

	private void UpdateRelease()
	{
		if (this.m_Animator.GetCurrentAnimatorStateInfo(1).shortNameHash == this.m_ReleaseStateRight1)
		{
			this.m_Animator.SetBool(this.m_BAttackRightRelease, false);
		}
	}

	protected override void AttackRelease()
	{
		this.m_HitObjects.Clear();
		this.m_AttackRelease = true;
		if (this.m_AttackDirection == AttackDirection.Right)
		{
			this.m_Animator.SetBool(this.m_BAttackRightRelease, true);
			if (this.m_ReleaseComboScheduled || this.m_ComboScheduled)
			{
				this.m_Animator.SetBool(this.m_BAttackRight, true);
				this.m_Animator.SetBool(this.m_BAttackLeft, false);
				this.m_ReleaseComboScheduled = false;
				this.m_ReleaseCombo = true;
			}
		}
		else if (this.m_AttackDirection == AttackDirection.Left)
		{
			this.m_Animator.SetBool(this.m_BAttackLeftRelease, true);
			if (this.m_ReleaseComboScheduled || this.m_ComboScheduled)
			{
				this.m_Animator.SetBool(this.m_BAttackRight, true);
				this.m_Animator.SetBool(this.m_BAttackLeft, false);
				this.m_ReleaseComboScheduled = false;
				this.m_ReleaseCombo = true;
			}
		}
		else if (this.m_AttackDirection == AttackDirection.Up)
		{
			this.m_Animator.SetBool(this.m_BAttackUpRelease, true);
		}
		Item currentItem = this.m_Player.GetCurrentItem(Hand.Right);
		float num = this.m_Player.GetStaminaDecrease(StaminaDecreaseReason.Swing);
		if (currentItem.IsKnife())
		{
			num *= Skill.Get<BladeSkill>().GetStaminaMul();
		}
		else if (currentItem.IsAxe())
		{
			num *= Skill.Get<AxeSkill>().GetStaminaMul();
		}
		else if (currentItem.IsMachete())
		{
			num *= Skill.Get<MacheteSkill>().GetStaminaMul();
		}
		this.m_Player.DecreaseStamina(num);
	}

	public void PlayerMeleeRightAttackEnd()
	{
		if (!this.m_ReleaseCombo)
		{
			this.m_Animator.SetBool(this.m_BAttackRight, false);
		}
		if (this.m_ComboScheduled)
		{
			this.m_Animator.SetBool(this.m_BAttackLeft, true);
		}
		this.m_ComboScheduled = false;
		this.m_ReleaseComboScheduled = false;
		this.m_ReleaseCombo = false;
		this.m_HitObjects.Clear();
		this.m_AlreadyHit = false;
	}

	public void PlayerMeleeLeftAttackEnd()
	{
		this.m_Animator.SetBool(this.m_BAttackLeft, false);
		if (this.m_ComboScheduled)
		{
			this.m_Animator.SetBool(this.m_BAttackRight, true);
		}
		this.m_ComboScheduled = false;
		this.m_ReleaseComboScheduled = false;
		this.m_ReleaseCombo = false;
		this.m_HitObjects.Clear();
		this.m_AlreadyHit = false;
	}

	public void PlayerMeleeUpAttackEnd()
	{
		this.m_Animator.SetBool(this.m_BAttackUp, false);
		this.m_ComboScheduled = false;
		this.m_ReleaseComboScheduled = false;
		this.m_ReleaseCombo = false;
		this.m_HitObjects.Clear();
	}

	protected override void EndAttack()
	{
		base.EndAttack();
		if (!this.m_ComboScheduled)
		{
			AnimatorStateInfo currentAnimatorStateInfo = this.m_Animator.GetCurrentAnimatorStateInfo(1);
			if (currentAnimatorStateInfo.shortNameHash == this.m_NMeleeAttackRightSwing)
			{
				this.PlayerMeleeRightAttackEnd();
			}
			else if (currentAnimatorStateInfo.shortNameHash == this.m_NMeleeAttackUpSwing)
			{
				this.PlayerMeleeUpAttackEnd();
			}
			else if (currentAnimatorStateInfo.shortNameHash == this.m_NMeleeAttackLeftSwing)
			{
				this.PlayerMeleeLeftAttackEnd();
			}
		}
		this.m_ReleaseComboScheduled = false;
		this.m_ComboScheduled = false;
		this.m_ComboBlocked = false;
		this.m_ReleaseCombo = false;
		this.m_HitObjects.Clear();
	}

	public override bool StopAnimOnHit(CJObject hit_obj, Collider coll = null)
	{
		if (hit_obj == null)
		{
			return false;
		}
		if (hit_obj.GetComponent<AI>() != null)
		{
			return false;
		}
		if (coll != null)
		{
			Item component = coll.gameObject.GetComponent<Item>();
			if (component != null)
			{
				if (component.m_Info.m_Health <= 0f)
				{
					this.m_ComboScheduled = false;
					this.m_ReleaseComboScheduled = false;
					this.m_ReleaseCombo = false;
					this.m_ComboBlocked = true;
					return false;
				}
				if (component.m_IsPlant)
				{
					return false;
				}
			}
			return true;
		}
		return true;
	}

	public override string ReplaceClipsGetItemName()
	{
		if (this.m_Player.GetCurrentItem(Hand.Right) == null)
		{
			return string.Empty;
		}
		if (this.m_Player.GetCurrentItem(Hand.Right).m_Info == null)
		{
			return string.Empty;
		}
		return this.m_Player.GetCurrentItem(Hand.Right).m_Info.m_ID.ToString();
	}

	public override AttackDirection GetAttackDirection()
	{
		return this.m_AttackDirection;
	}

	public override bool DuckDuringAttack()
	{
		AnimatorStateInfo currentAnimatorStateInfo = this.m_Animator.GetCurrentAnimatorStateInfo(1);
		return currentAnimatorStateInfo.shortNameHash == this.m_NMeleeAttackUpSwing || currentAnimatorStateInfo.shortNameHash == this.m_NMeleeAttackUpWindUp;
	}

	public override bool CanPlaySwingSound()
	{
		AnimatorStateInfo currentAnimatorStateInfo = this.m_Animator.GetCurrentAnimatorStateInfo(1);
		return currentAnimatorStateInfo.shortNameHash != this.m_ReleaseStateRight1 && currentAnimatorStateInfo.shortNameHash != this.m_ReleaseStateUp1 && currentAnimatorStateInfo.shortNameHash != this.m_ReleaseStateLeft1;
	}

	public override void ControllerLateUpdate()
	{
		base.ControllerLateUpdate();
		if (this.m_SpawnFX)
		{
			Item currentItem = Player.Get().GetCurrentItem(Hand.Right);
			if (currentItem != null)
			{
				this.m_FxToSpawnPos = currentItem.m_DamagerStart.position;
			}
			this.DelayedSpawnFX();
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	private void SetupPredictionData()
	{
		Item currentItem = this.m_Player.GetCurrentItem(Hand.Right);
		ItemID id = currentItem.m_Info.m_ID;
		if (this.m_PredictionData.ContainsKey((int)id))
		{
			return;
		}
		this.m_PredictionData[(int)id] = new Dictionary<AnimationClip, List<MeleeAttackPredictionData>>();
		RuntimeAnimatorController runtimeAnimatorController = this.m_Animator.runtimeAnimatorController;
		List<string> list = this.m_PredictionDataClipNames[291];
		if (this.m_PredictionDataClipNames.TryGetValue((int)id, out list))
		{
			list = this.m_PredictionDataClipNames[(int)id];
		}
		else
		{
			list = this.m_PredictionDataClipNames[291];
		}
		for (int i = 0; i < list.Count; i++)
		{
			AnimationClip animationClipByName = this.GetAnimationClipByName(runtimeAnimatorController, list[i]);
			this.m_PredictionData[(int)id][animationClipByName] = new List<MeleeAttackPredictionData>();
			for (int j = 0; j <= MeleeAttackPredictionData.s_NumSteps; j++)
			{
				float time = animationClipByName.length * ((float)j / (float)MeleeAttackPredictionData.s_NumSteps);
				animationClipByName.SampleAnimation(this.m_Player.gameObject, time);
				Vector3 position = this.m_Player.GetCurrentItem(Hand.Right).m_DamagerStart.position;
				Vector3 position2 = this.m_Player.GetCurrentItem(Hand.Right).m_DamagerEnd.position;
				MeleeAttackPredictionData meleeAttackPredictionData = new MeleeAttackPredictionData();
				meleeAttackPredictionData.m_DamagerStart = this.m_HeadTransform.InverseTransformPoint(position);
				meleeAttackPredictionData.m_DamagerEnd = this.m_HeadTransform.InverseTransformPoint(position2);
				meleeAttackPredictionData.m_NormalizedTime = (float)j / (float)MeleeAttackPredictionData.s_NumSteps;
				this.m_PredictionData[(int)id][animationClipByName].Add(meleeAttackPredictionData);
			}
		}
	}

	private AnimationClip GetAnimationClipByName(RuntimeAnimatorController rac, string clip_name)
	{
		AnimationClip[] animationClips = rac.animationClips;
		for (int i = 0; i < animationClips.Length; i++)
		{
			if (animationClips[i].name == clip_name)
			{
				return animationClips[i];
			}
		}
		return null;
	}

	protected override bool UpdateCollisions(Triangle blade_t0, Triangle blade_t1, Triangle handle_t0, Triangle handle_t1, Vector3 hit_dir, bool damage_window, out CJObject cj_obj, out Collider collider, out bool collision_with_handle, float damage_window_start, float damage_window_end, out Vector3 hit_pos)
	{
		collision_with_handle = false;
		Vector3 zero = Vector3.zero;
		Vector3 zero2 = Vector3.zero;
		Quaternion identity = Quaternion.identity;
		cj_obj = null;
		collider = null;
		hit_pos = Vector3.zero;
		Item currentItem = this.m_Player.GetCurrentItem(Hand.Right);
		if (currentItem == null || currentItem.m_Info == null)
		{
			return false;
		}
		ItemID id = currentItem.m_Info.m_ID;
		if (!this.m_PredictionData.ContainsKey((int)id))
		{
			return false;
		}
		AnimatorClipInfo[] currentAnimatorClipInfo = this.m_Animator.GetCurrentAnimatorClipInfo(1);
		AnimationClip animationClip = null;
		for (int i = 0; i < currentAnimatorClipInfo.Length; i++)
		{
			if (this.m_PredictionData[(int)id].ContainsKey(currentAnimatorClipInfo[i].clip))
			{
				animationClip = currentAnimatorClipInfo[i].clip;
			}
		}
		if (animationClip == null)
		{
			return false;
		}
		List<MeleeAttackPredictionData> list = this.m_PredictionData[(int)id][animationClip];
		AnimatorStateInfo currentAnimatorStateInfo = this.m_Animator.GetCurrentAnimatorStateInfo(1);
		float num = 0.02f;
		float num2 = (this.m_LastAnimFrame < 0f) ? 0f : (this.m_LastAnimFrame / currentAnimatorStateInfo.length);
		float time = num2;
		float num3 = currentAnimatorStateInfo.normalizedTime - num2;
		int num4 = (int)(num3 / num);
		if (num4 < 1)
		{
			num4 = 1;
		}
		num3 /= (float)num4;
		for (int j = 0; j < num4; j++)
		{
			float num5 = num2 + num3 * (float)(j + 1);
			if (num5 < damage_window_start || num5 > damage_window_end)
			{
				this.m_LastAnimFrame = this.m_Animator.GetCurrentAnimatorStateInfo(1).length * num5;
				cj_obj = null;
				collider = null;
				time = num5;
			}
			else
			{
				Vector3 damagerStartPos = this.GetDamagerStartPos(num5, ref list);
				Vector3 damagerEndPos = this.GetDamagerEndPos(num5, ref list);
				Vector3 damagerStartPos2 = this.GetDamagerStartPos(time, ref list);
				Vector3 damagerEndPos2 = this.GetDamagerEndPos(time, ref list);
				this.m_BladeT0Triangle.p0 = damagerStartPos;
				this.m_BladeT0Triangle.p1 = damagerStartPos2;
				this.m_BladeT0Triangle.p2 = damagerEndPos;
				this.m_BladeT1Triangle.p0 = damagerStartPos2;
				this.m_BladeT1Triangle.p1 = damagerEndPos2;
				this.m_BladeT1Triangle.p2 = damagerEndPos;
				base.GetBoxParameters(this.m_BladeT0Triangle, this.m_BladeT1Triangle, out zero2, out zero, out identity);
				Collider[] array = Physics.OverlapBox(zero2, zero, identity);
				this.SortCollisions(array);
				for (int k = 0; k < array.Length; k++)
				{
					bool flag = false;
					Collider collider2 = array[k];
					if (collider2.gameObject.IsWater())
					{
						this.OnHitWater(collider2);
					}
					if (!collider2.isTrigger || collider2.gameObject.IsAI())
					{
						if (!(collider2.gameObject == this.m_Player.gameObject) && !(collider2.gameObject == this.m_Player.GetCurrentItem(Hand.Right).gameObject))
						{
							if (!this.m_HitObjects.Contains(collider2.gameObject))
							{
								if (!(collider2.gameObject.GetComponent<Player>() != null))
								{
									CJObject component = collider2.gameObject.GetComponent<CJObject>();
									if (component == null && collider2.gameObject.transform.parent != null)
									{
										component = collider2.gameObject.transform.parent.GetComponent<CJObject>();
									}
									if (component != null && component.GetHitCollisionType() == HitCollisionType.Bones)
									{
										if (base.CheckBonesIntersection(blade_t0, blade_t1, component))
										{
											flag = true;
										}
									}
									else
									{
										flag = true;
									}
									if (flag && damage_window)
									{
										float anim_frame = num5 * currentAnimatorStateInfo.length;
										this.SetupAnimFrameForHit(zero2, zero, identity, anim_frame, collider2);
										this.m_LastBoxHalfSizes = zero;
										this.m_LastBoxPos = zero2;
										this.m_LastBoxQuaternion = identity;
										this.m_LastAnimFrame = this.m_Animator.GetCurrentAnimatorStateInfo(1).length * num5;
										cj_obj = component;
										collider = collider2;
										hit_pos = this.m_BladeT0Triangle.p0;
										float proportionalClamp = CJTools.Math.GetProportionalClamp(0f, 1f, num5, num2, currentAnimatorStateInfo.normalizedTime);
										this.m_AnimationStoppedHandPos = Vector3.Lerp(this.m_LastHandPos, this.m_RHand.transform.position, proportionalClamp);
										this.m_LastHandPos = this.m_RHand.position;
										return true;
									}
								}
							}
						}
					}
				}
				time = num5;
			}
		}
		this.m_LastBoxHalfSizes = zero;
		this.m_LastBoxPos = zero2;
		this.m_LastBoxQuaternion = identity;
		this.m_LastAnimFrame = this.m_Animator.GetCurrentAnimatorStateInfo(1).length * this.m_Animator.GetCurrentAnimatorStateInfo(1).normalizedTime;
		this.m_LastHandPos = this.m_RHand.position;
		cj_obj = null;
		collider = null;
		return false;
	}

	private Vector3 GetDamagerStartPos(float time, ref List<MeleeAttackPredictionData> data)
	{
		int num = (int)Mathf.Ceil(time * (float)MeleeAttackPredictionData.s_NumSteps);
		int num2 = (int)Mathf.Floor(time * (float)MeleeAttackPredictionData.s_NumSteps);
		if (num >= MeleeAttackPredictionData.s_NumSteps)
		{
			num = MeleeAttackPredictionData.s_NumSteps - 1;
		}
		if (num2 >= MeleeAttackPredictionData.s_NumSteps)
		{
			num2 = MeleeAttackPredictionData.s_NumSteps - 1;
		}
		float b = Mathf.Ceil(time * (float)MeleeAttackPredictionData.s_NumSteps) / (float)MeleeAttackPredictionData.s_NumSteps;
		float b2 = Mathf.Floor(time * (float)MeleeAttackPredictionData.s_NumSteps) / (float)MeleeAttackPredictionData.s_NumSteps;
		Vector3 b3 = this.m_HeadTransform.TransformPoint(data[num].m_DamagerStart);
		Vector3 a = this.m_HeadTransform.TransformPoint(data[num2].m_DamagerStart);
		float proportionalClamp = CJTools.Math.GetProportionalClamp(0f, 1f, time, b2, b);
		return Vector3.Lerp(a, b3, proportionalClamp);
	}

	private Vector3 GetDamagerEndPos(float time, ref List<MeleeAttackPredictionData> data)
	{
		int num = (int)Mathf.Ceil(time * (float)MeleeAttackPredictionData.s_NumSteps);
		int num2 = (int)Mathf.Floor(time * (float)MeleeAttackPredictionData.s_NumSteps);
		if (num >= MeleeAttackPredictionData.s_NumSteps)
		{
			num = MeleeAttackPredictionData.s_NumSteps - 1;
		}
		if (num2 >= MeleeAttackPredictionData.s_NumSteps)
		{
			num2 = MeleeAttackPredictionData.s_NumSteps - 1;
		}
		float b = Mathf.Ceil(time * (float)MeleeAttackPredictionData.s_NumSteps) / (float)MeleeAttackPredictionData.s_NumSteps;
		float b2 = Mathf.Floor(time * (float)MeleeAttackPredictionData.s_NumSteps) / (float)MeleeAttackPredictionData.s_NumSteps;
		Vector3 b3 = this.m_HeadTransform.TransformPoint(data[num].m_DamagerEnd);
		Vector3 a = this.m_HeadTransform.TransformPoint(data[num2].m_DamagerEnd);
		float proportionalClamp = CJTools.Math.GetProportionalClamp(0f, 1f, time, b2, b);
		return Vector3.Lerp(a, b3, proportionalClamp);
	}

	protected override void SetupAnimFrameForHit(Vector3 box_center, Vector3 box_half_sizes, Quaternion q, float anim_frame, Collider coll)
	{
		this.m_AnimationStopFrame = anim_frame;
	}

	private DestroyableChunkSource PredictHit()
	{
		Item currentItem = this.m_Player.GetCurrentItem(Hand.Right);
		if (currentItem == null || currentItem.m_Info == null)
		{
			return null;
		}
		ItemID id = currentItem.m_Info.m_ID;
		if (!this.m_PredictionData.ContainsKey((int)id))
		{
			return null;
		}
		AnimatorClipInfo[] currentAnimatorClipInfo = this.m_Animator.GetCurrentAnimatorClipInfo(1);
		AnimationClip animationClip = null;
		for (int i = 0; i < currentAnimatorClipInfo.Length; i++)
		{
			if (this.m_PredictionData[(int)id].ContainsKey(currentAnimatorClipInfo[i].clip))
			{
				animationClip = currentAnimatorClipInfo[i].clip;
			}
		}
		if (animationClip == null)
		{
			return null;
		}
		List<MeleeAttackPredictionData> list = this.m_PredictionData[(int)id][animationClip];
		int num = (int)Mathf.Floor(this.m_Animator.GetCurrentAnimatorStateInfo(1).normalizedTime * (float)MeleeAttackPredictionData.s_NumSteps);
		Vector3 zero = Vector3.zero;
		Vector3 zero2 = Vector3.zero;
		Vector3 p = Vector3.zero;
		Vector3 p2 = Vector3.zero;
		Vector3 vector = Vector3.zero;
		Vector3 p3 = Vector3.zero;
		Quaternion identity = Quaternion.identity;
		for (int j = num; j < list.Count - 1; j++)
		{
			float time = (float)j / (float)MeleeAttackPredictionData.s_NumSteps;
			float time2 = (float)(j + 1) / (float)MeleeAttackPredictionData.s_NumSteps;
			p = this.GetDamagerStartPos(time2, ref list);
			p2 = this.GetDamagerEndPos(time2, ref list);
			vector = this.GetDamagerStartPos(time, ref list);
			p3 = this.GetDamagerEndPos(time, ref list);
			this.m_BladeT0Triangle.p0 = p;
			this.m_BladeT0Triangle.p1 = vector;
			this.m_BladeT0Triangle.p2 = p2;
			this.m_BladeT1Triangle.p0 = vector;
			this.m_BladeT1Triangle.p1 = p3;
			this.m_BladeT1Triangle.p2 = p2;
			base.GetBoxParameters(this.m_BladeT0Triangle, this.m_BladeT1Triangle, out zero, out zero2, out identity);
			Collider[] array = Physics.OverlapBox(zero, zero2, identity);
			for (int k = 0; k < array.Length; k++)
			{
				DestroyableChunkSource component = array[k].gameObject.GetComponent<DestroyableChunkSource>();
				if (component != null && this.IsValidIKTargetPos(component.m_IKTargetPos))
				{
					return component;
				}
			}
		}
		return null;
	}

	private KeyValuePair<T1, T2> KeyPairValue<T1, T2>(T1 collider, T2 v)
	{
		throw new NotImplementedException();
	}

	private bool IsValidIKTargetPos(Vector3 pos)
	{
		Vector3 vector = Camera.main.WorldToScreenPoint(pos);
		return vector.x > (float)Screen.width * 0.1f && vector.x < (float)Screen.width * 0.9f && vector.y > (float)Screen.height * 0.12f && vector.y < (float)Screen.height * 0.88f;
	}

	protected override void SpawnFX(string fx_name, Vector3 hit_pos)
	{
		this.m_SpawnFX = true;
		this.m_FXToSpawn = fx_name;
		this.m_FxToSpawnPos = hit_pos;
	}

	private void DelayedSpawnFX()
	{
		Vector3 fxToSpawnPos = this.m_FxToSpawnPos;
		ParticlesManager.Get().Spawn(this.m_FXToSpawn, fxToSpawnPos, Quaternion.identity, null);
		this.m_SpawnFX = false;
	}

	public bool IsLeftAttack()
	{
		return this.m_Animator.GetCurrentAnimatorStateInfo(1).shortNameHash == this.m_NMeleeAttackLeftSwing;
	}

	public bool IsRightAttack()
	{
		return this.m_Animator.GetCurrentAnimatorStateInfo(1).shortNameHash == this.m_NMeleeAttackRightSwing;
	}

	public bool IsUpAttack()
	{
		return this.m_Animator.GetCurrentAnimatorStateInfo(1).shortNameHash == this.m_NMeleeAttackUpSwing;
	}

	private void SortCollisions(Collider[] colls)
	{
		for (int i = 0; i < colls.Length; i++)
		{
			for (int j = 0; j < colls.Length - 1; j++)
			{
				if (colls[j].GetType() == typeof(TerrainCollider))
				{
					Collider collider = colls[j];
					colls[j] = colls[j + 1];
					colls[j + 1] = collider;
				}
			}
		}
	}

	public override void ResetAttack()
	{
		base.ResetAttack();
		this.m_Animator.SetBool(this.m_BAttackUp, false);
		this.m_Animator.SetBool(this.m_BAttackRight, false);
		this.m_Animator.SetBool(this.m_BAttackLeft, false);
		this.m_Animator.SetBool(this.m_BAttackRightRelease, false);
		this.m_Animator.SetBool(this.m_BAttackUpRelease, false);
		this.m_Animator.SetBool(this.m_BAttackLeftRelease, false);
	}

	public bool CanBeInterrupted()
	{
		AnimatorStateInfo currentAnimatorStateInfo = this.m_Animator.GetCurrentAnimatorStateInfo(1);
		return !this.IsActive() || currentAnimatorStateInfo.shortNameHash == this.m_NMeleeIdle;
	}

	private int m_BAttackUp = Animator.StringToHash("AttackUp");

	private int m_BAttackRight = Animator.StringToHash("AttackRight");

	private int m_BAttackLeft = Animator.StringToHash("AttackLeft");

	private int m_BAttackRightRelease = Animator.StringToHash("AttackRightRelease");

	private int m_BAttackUpRelease = Animator.StringToHash("AttackUpRelease");

	private int m_BAttackLeftRelease = Animator.StringToHash("AttackLeftRelease");

	private int m_NMeleeAttackRightWindUp = Animator.StringToHash("AttackRightWindUp");

	private int m_NMeleeAttackRightSwing = Animator.StringToHash("AttackRightSwing");

	private int m_NMeleeAttackUpWindUp = Animator.StringToHash("AttackUpWindUp");

	private int m_NMeleeAttackUpSwing = Animator.StringToHash("AttackUpSwing");

	private int m_NMeleeAttackLeftSwing = Animator.StringToHash("AttackLeftSwing");

	private int m_ReleaseStateRight1 = Animator.StringToHash("MeleeAttackRightRelease");

	private int m_ReleaseStateLeft1 = Animator.StringToHash("MeleeAttackLeftRelease");

	private int m_ReleaseStateUp1 = Animator.StringToHash("MeleeAttackUpRelease");

	private int m_NMeleeAttackRightReturn = Animator.StringToHash("MeleeAttackRightReturn");

	private int m_NMeleeIdle = Animator.StringToHash("MeleeIdle");

	private float m_LastAttackSoundTime;

	private static WeaponMeleeController s_Instance;

	private bool m_AttackRelease;

	private AttackDirection m_AttackDirection = AttackDirection.Right;

	private Transform m_ArmTransform;

	private Transform m_RHand;

	private Dictionary<int, List<string>> m_PredictionDataClipNames = new Dictionary<int, List<string>>();

	private Dictionary<int, Dictionary<AnimationClip, List<MeleeAttackPredictionData>>> m_PredictionData = new Dictionary<int, Dictionary<AnimationClip, List<MeleeAttackPredictionData>>>();

	private Transform m_HeadTransform;

	private int m_NRightSwingToReleaseTransition = Animator.StringToHash("RightSwingToReleaseTransition");

	private int m_NLeftSwingToReleaseTransition = Animator.StringToHash("LeftSwingToReleaseTransition");

	private float m_LastAttackTime;

	private Triangle m_BladeT0Triangle = new Triangle();

	private Triangle m_BladeT1Triangle = new Triangle();

	private Vector3 m_LastHandPos = Vector3.zero;

	private Vector3 m_AnimationStoppedHandPos = Vector3.zero;

	private bool m_SpawnFX;

	private string m_FXToSpawn = string.Empty;

	private Vector3 m_FxToSpawnPos = Vector3.zero;
}
