using UnityEngine;
using System.Collections;
using MDS.Gameplay.DragDrop;

public class Seesaw : MDSBehaviour {

	public float max_PosY;
	public float min_PosY;
	public float balanced_PosY;

	public Sprite Right_BIsGreater;
	public Sprite Middle_Equal;
	public Sprite Left_AIsGreater;

	private DropGroupSlot[] slotsA, slotsB;

	public MathValidador mathValidator;

	private SpriteRenderer _spriteRenderer;

	public int scaleState;

    protected override void Awake()
    {
        base.Awake();
        _spriteRenderer = GetComponent<SpriteRenderer>();
		slotsA = mathValidator._validatableA.GetGameObject().GetComponentsInChildren<DropGroupSlot>();
		slotsB = mathValidator._validatableB.GetGameObject().GetComponentsInChildren<DropGroupSlot>();
    }

    void Update()
    {

        int? A, B;

        mathValidator.GetNumericValues(out A, out B);

        if(!A.HasValue)
            A = 0;

        if(!B.HasValue)
            B = 0;

		if (A.Value > B.Value) {
			_spriteRenderer.sprite = Left_AIsGreater;
			if (scaleState != -1){
				StartCoroutine(TipTheScales (-1));
			}
		}

		else if (A.Value == B.Value) {
			_spriteRenderer.sprite = Middle_Equal;
			if (scaleState != 0){
				StartCoroutine(TipTheScales (0));
			}
		}

		else if (A.Value < B.Value) {
			_spriteRenderer.sprite = Right_BIsGreater;
			if (scaleState != 1){
				StartCoroutine(TipTheScales (1));
			}
		}
    }

	IEnumerator TipTheScales(int myState){
		if (myState == -1){
				Vector3 temp;
				yield return new WaitForSeconds (0.5f);
				foreach(var slotA in slotsA)
				{
					temp = slotA.transform.position;
					temp.y = min_PosY;
					slotA.transform.position = temp;
					temp.z = -1;
					if (slotA.draggableReference != null){
						slotA.draggableReference.transform.position = temp;
					}
				}

				foreach(var slotB in slotsB)
				{
					temp = slotB.transform.position;
					temp.y = max_PosY;
					slotB.transform.position = temp;
					temp.z = -1;
					if (slotB.draggableReference != null) {
						slotB.draggableReference.transform.position = temp;
					}
				}

				scaleState = -1;
		}

		if (myState == 0){
				Vector3 temp;
				yield return new WaitForSeconds (0.5f);
				foreach(var slotA in slotsA)
				{
					temp = slotA.transform.position;
					temp.y = balanced_PosY;
					slotA.transform.position = temp;
					temp.z = -1;
					if (slotA.draggableReference != null) {
						slotA.draggableReference.transform.position = temp;
					}
				}

				foreach(var slotB in slotsB)
				{
					temp = slotB.transform.position;
					temp.y = balanced_PosY;
					slotB.transform.position = temp;
					temp.z = -1;
					if (slotB.draggableReference != null) {
						slotB.draggableReference.transform.position = temp;
					}
				}

				scaleState = 0;
		}

		if (myState == 1){
				Vector3 temp;
				yield return new WaitForSeconds (0.5f);
				foreach(var slotA in slotsA)
				{
					temp = slotA.transform.position;
					temp.y = max_PosY;
					slotA.transform.position = temp;
					temp.z = -1;
					if (slotA.draggableReference != null) {
						slotA.draggableReference.transform.position = temp;
					}
				}

				foreach(var slotB in slotsB)
				{
					temp = slotB.transform.position;
					temp.y = min_PosY;
					slotB.transform.position = temp;
					temp.z = -1;
					if (slotB.draggableReference != null) {
						slotB.draggableReference.transform.position = temp;
					}
				}

				scaleState = 1;
		}



	}
}
