using UnityEngine;
using System.Collections;

public class ContainerMove : MonoBehaviour {
	public float MoveSpeed = .1f;
	public bool UnlockPosX = true;
	public bool UnlockPosZ;
	private float NewPosOffset = 2;
	private float InitPosX;
	private float InitPosZ;
	private float PosX;
	private float PosZ;
	private bool SwitchMoveX;
	private bool SwitchMoveZ;

	void Start() {
		InitPosX = transform.position.x;
		InitPosZ = transform.position.z;

		PosX = InitPosX;
		PosZ = InitPosZ;
	}

	void Update() {
		if(UnlockPosX) {
			if(!SwitchMoveX) {
				if(PosX < InitPosX + NewPosOffset)
					PosX += MoveSpeed;
				else
					SwitchMoveX = true;
			} else {
				if(PosX > InitPosX - NewPosOffset)
					PosX -= MoveSpeed;
				else
					SwitchMoveX = false;
			}
		}
		if(UnlockPosZ) {
			if(!SwitchMoveZ) {
				if(PosZ < InitPosZ + NewPosOffset)
					PosZ += MoveSpeed;
				else
					SwitchMoveZ = true;
			} else {
				if(PosZ > InitPosZ - NewPosOffset)
					PosZ -= MoveSpeed;
				else
					SwitchMoveZ = false;
			}
		}

		transform.position = new Vector3 (PosX, transform.position.y, PosZ);
	}
}
