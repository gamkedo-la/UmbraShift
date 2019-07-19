﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{

	[SerializeField] private Texture2D defaultCursor;
	[SerializeField] private Texture2D invalidCursor;
	[SerializeField] private Texture2D moveCursor;
	[SerializeField] private Texture2D attackCursor;
	[SerializeField] private Texture2D hackCursor;
	Vector2 topLeft = Vector2.left + Vector2.up;
	private int walkableAreaLayer = 16384;
	private int visibleEnemyLayer = 256;
	private int interactableLayer = 8192;
	private int combinedLayer = 24832;

	// Start is called before the first frame update
	void Start()
    {
		Cursor.SetCursor(defaultCursor, topLeft, CursorMode.Auto);
    }

    void Update()
    {
		Cursor.SetCursor(GetLayerUnderMouse(),topLeft, CursorMode.Auto);
    }

	public Texture2D GetLayerUnderMouse()
	{
		Texture2D mouseCursor = defaultCursor;
		PlayerController chr = TurnManager.instance.ActivePlayerController;
		RaycastHit mouseRayHit;
		Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		bool mouseRayCast = Physics.Raycast(mouseRay, out mouseRayHit, 100f, combinedLayer);
		Debug.Log("Raycast Hit? : " + mouseRayCast);
		if (mouseRayCast)
		{
			Debug.Log("Layer Hit: " + mouseRayHit.collider.gameObject.layer);
		}
		if (mouseRayCast==false || TurnManager.instance.playersTurn == false || chr == null)
		{ return defaultCursor; }

		else if (mouseRayHit.collider.gameObject.layer == walkableAreaLayer)
		{
			bool valid = true;
			float dist = Vector3.Distance(mouseRayHit.point, chr.transform.position);
			// perhaps check the distance of the AI path instead of direct line-of-sight distance
			// perhaps do a raycast to check for intervening obstacles?
			if (dist > chr.maxMoveWithAvailableAP) { valid = false; }
			if (valid == true) { return moveCursor; } else { return defaultCursor; }
		}
		else if (mouseRayHit.collider.gameObject.layer == visibleEnemyLayer)
		{
			bool valid = true;
			//check for weapon range?
			//check for direct line of sight?
			if (valid == true) { return attackCursor; } else { return defaultCursor; }
		}
		else if (mouseRayHit.collider.gameObject.layer == interactableLayer)
		{
			bool valid = true;
			//line of sight?  Adjacency?  Max visibility?  Available power?
			if (valid == true) { return hackCursor; } else { return defaultCursor; }
		}
		else return defaultCursor;
	}
}
