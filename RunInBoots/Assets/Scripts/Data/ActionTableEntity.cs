using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActionTableEntity
{
	public int Key;
	public string Clip;
	public float ColliderX;
	public float ColliderY;
	public float TransitionDuration;
	public float GravityScale;
	public float MaxVelocityX;
	public float MaxVelocityY;
	public int NextAction;
	public string FrameUpdates;
}