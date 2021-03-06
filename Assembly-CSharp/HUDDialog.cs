﻿using System;
using UnityEngine;
using UnityEngine.UI;

public class HUDDialog : HUDBase
{
	public override void SetupGroups()
	{
		base.SetupGroups();
		base.AddToGroup(HUDManager.HUDGroup.Game);
		base.AddToGroup(HUDManager.HUDGroup.Inventory3D);
	}

	protected override void Awake()
	{
		base.Awake();
		this.m_TextGen = new TextGenerator();
		this.m_WTIcon = base.transform.Find("WalkieTalkieIcon").GetComponent<RawImage>();
	}

	protected override bool ShouldShow()
	{
		return DialogsManager.Get().m_CurrentDialog != null && ((DialogsManager.Get().m_CurrentDialog.m_CurrentNode != null && DialogsManager.Get().m_CurrentDialog.m_CurrentNode.m_Wait == 0f) || (DialogsManager.Get().m_CurrentDialog.m_CurrentAdditionalNode != null && DialogsManager.Get().m_CurrentDialog.m_CurrentAdditionalNode.m_Wait == 0f));
	}

	protected override void OnShow()
	{
		base.OnShow();
		DialogNode node = this.GetNode();
		if (node != null)
		{
			this.Setup(node);
		}
	}

	private DialogNode GetNode()
	{
		DialogNode dialogNode = DialogsManager.Get().m_CurrentDialog.m_CurrentNode;
		if (dialogNode == null)
		{
			dialogNode = DialogsManager.Get().m_CurrentDialog.m_CurrentAdditionalNode;
		}
		return dialogNode;
	}

	protected override void Update()
	{
		base.Update();
		if (DialogsManager.Get().m_CurrentDialog == null)
		{
			return;
		}
		DialogNode node = this.GetNode();
		if (node != null && node != this.m_CurrentNode)
		{
			this.Setup(node);
		}
	}

	private void Setup(DialogNode node)
	{
		if (node.m_Wait > 0f)
		{
			this.m_Text.gameObject.SetActive(false);
			return;
		}
		this.m_Text.gameObject.SetActive(true);
		this.m_Text.text = GreenHellGame.Instance.GetLocalization().Get(node.m_Name);
		TextGenerationSettings generationSettings = this.m_Text.GetGenerationSettings(this.m_Text.rectTransform.rect.size);
		generationSettings.scaleFactor = 1f;
		float num = Mathf.Min(this.m_TextGen.GetPreferredWidth(this.m_Text.text, generationSettings), generationSettings.generationExtents.x);
		this.m_CurrentNode = node;
		this.m_WTIcon.gameObject.SetActive(node.m_WalkieTalkie);
		if (this.m_WTIcon.gameObject.activeSelf)
		{
			Vector3 localPosition = this.m_Text.rectTransform.localPosition;
			localPosition.x -= num * 0.5f + 25f;
			this.m_WTIcon.transform.localPosition = localPosition;
		}
	}

	public Text m_Text;

	private TextGenerator m_TextGen;

	private DialogNode m_CurrentNode;

	private RawImage m_WTIcon;
}
