﻿using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

[TrackClipType(typeof(ScreenFaderClip))]
[TrackBindingType(typeof(Image))]
[TrackColor(0.875f, 0.5944853f, 0.1737132f)]
public class ScreenFaderTrack : TrackAsset
{
	public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
	{
		return ScriptPlayable<ScreenFaderMixerBehaviour>.Create(graph, inputCount);
	}

	public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
	{
		base.GatherProperties(director, driver);
	}
}
