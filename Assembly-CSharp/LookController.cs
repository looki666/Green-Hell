﻿using System;
using CJTools;
using UnityEngine;

public class LookController : PlayerController
{
	public static LookController Get()
	{
		return LookController.s_Instance;
	}

	protected override void Awake()
	{
		base.Awake();
		LookController.s_Instance = this;
		this.m_ControllerType = PlayerControllerType.Look;
		this.m_CharacterController = base.gameObject.GetComponent<CharacterController>();
		this.m_CharacterControllerLastOffset = this.m_CharacterController.center;
	}

	public void SetLookAtObject(GameObject obj)
	{
		this.m_LookAtObject = obj;
	}

	public void ResetLookAtObject(GameObject obj)
	{
		this.m_LookAtObject = null;
	}

	public void UpdateLookDev(float x, float y)
	{
		if (this.UpdateLookAtObject())
		{
			return;
		}
		this.m_WantedLookDev.x = this.m_WantedLookDev.x + x;
		this.m_WantedLookDev.y = this.m_WantedLookDev.y + y;
		if (this.m_WantedLookDev.y > 80f)
		{
			this.m_WantedLookDev.y = 80f;
		}
		if (this.m_WantedLookDev.y < -64f)
		{
			this.m_WantedLookDev.y = -64f;
		}
		this.m_LookDev.x = this.m_LookDev.x % 360f;
		this.m_WantedLookDev.x = this.m_WantedLookDev.x % 360f;
		if (this.m_LookDev.x < 0f)
		{
			this.m_LookDev.x = 360f + this.m_LookDev.x;
		}
		if (this.m_WantedLookDev.x < 0f)
		{
			this.m_WantedLookDev.x = 360f + this.m_WantedLookDev.x;
		}
		this.m_LookDev.x = Mathf.LerpAngle(this.m_WantedLookDev.x, this.m_LookDev.x, 0.5f);
		this.m_LookDev.y = Mathf.LerpAngle(this.m_WantedLookDev.y, this.m_LookDev.y, 0.5f);
	}

	public void CalculateLookDev(Vector3 dir)
	{
		Vector3 vector = dir;
		vector.y = 0f;
		if (vector.magnitude == 0f)
		{
			this.m_LookDev.x = 0f;
		}
		else
		{
			vector.Normalize();
			this.m_LookDev.x = Mathf.Acos(vector.z) * 57.29578f;
			this.m_LookDev.x = this.m_LookDev.x + ((vector.x >= 0f) ? 0f : (180f - this.m_LookDev.x));
		}
		vector = dir;
		vector.x = 0f;
		if (vector.magnitude == 0f)
		{
			this.m_LookDev.y = 0f;
		}
		else
		{
			vector.Normalize();
			this.m_LookDev.y = Mathf.Asin(vector.y) * 57.29578f * Mathf.Sign(vector.y);
		}
	}

	public void UpdateCharacterControllerCenter()
	{
		Vector3 center = this.m_CharacterController.center;
		this.m_CharacterControllerLastOffset = center;
		float num = -40f;
		if (this.m_LookDev.y <= num)
		{
			center.z = 0.35f;
		}
		else
		{
			center.z = CJTools.Math.GetProportionalClamp(0.35f, -0.35f, this.m_LookDev.y, num, 80f);
		}
		this.m_CharacterController.center = center;
		this.m_CharacterControllerDelta = center - this.m_CharacterControllerLastOffset;
	}

	private bool UpdateLookAtObject()
	{
		if (!this.m_LookAtObject)
		{
			return false;
		}
		Vector3 vector = this.m_LookAtObject.transform.position - Camera.main.transform.position;
		vector.Normalize();
		Vector2 zero = Vector2.zero;
		zero.x = 57.29578f * Mathf.Atan2(vector.x, vector.z);
		zero.x %= 360f;
		if (zero.x < 0f)
		{
			zero.x = 360f + zero.x;
		}
		float num = Mathf.Sqrt(vector.x * vector.x + vector.z * vector.z);
		if (num == 0f)
		{
			zero.y = ((vector.y <= 0f) ? -90f : 90f);
		}
		else
		{
			zero.y = 57.29578f * Mathf.Atan(vector.y / num);
		}
		float num2 = Mathf.Abs(zero.x - this.m_LookDev.x);
		if (num2 > 180f)
		{
			if (zero.x > this.m_LookDev.x)
			{
				this.m_LookDev.x = this.m_LookDev.x + 360f;
			}
			else
			{
				zero.x += 360f;
			}
		}
		this.m_LookDev += (zero - this.m_LookDev) * Time.deltaTime * this.m_LookAtObjectFactor;
		this.m_LookDev.x = this.m_LookDev.x % 360f;
		if (this.m_LookDev.x < 0f)
		{
			this.m_LookDev.x = 360f + this.m_LookDev.x;
		}
		this.m_WantedLookDev = this.m_LookDev;
		return true;
	}

	public Vector2 m_LookDev = default(Vector2);

	public Vector2 m_WantedLookDev = default(Vector2);

	public float m_LookDevSpeed = 0.5f;

	public const float CAMERA_MAX_ANGLE_UP = 80f;

	public const float CAMERA_MAX_ANGLE_DOWN = 64f;

	private GameObject m_LookAtObject;

	private float m_LookAtObjectFactor = 2f;

	private static LookController s_Instance;

	private CharacterController m_CharacterController;

	private Vector3 m_CharacterControllerLastOffset = Vector3.zero;

	public Vector3 m_CharacterControllerDelta = Vector3.zero;

	public const float m_CharacterColliderMaxOffsetZ = 0.35f;
}
