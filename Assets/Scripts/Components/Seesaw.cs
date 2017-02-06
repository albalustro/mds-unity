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

    IEnumerator TipTheScales(int myState)
    {

        if(myState == -1)
        {
            SetPosition(min_PosY, max_PosY);
			scaleState = -1;
            yield return new WaitForSeconds(0.5f);
        }

        if(myState == 0)
        {
            SetPosition(balanced_PosY, balanced_PosY);
			scaleState = 0;
            yield return new WaitForSeconds(0.5f);
        }

        if(myState == 1)
        {
            SetPosition(max_PosY, min_PosY);
			scaleState = 1;
            yield return new WaitForSeconds(0.5f);
        }

    }


    void SetPosition(float a_y, float b_y)
    {

        foreach(var slotA in slotsA)
        {
            LeanTween.moveY(slotA.gameObject, a_y, 0.5f);
            if(slotA.draggableReference != null)
            {
                LeanTween.moveLocalY(slotA.draggableReference.gameObject, a_y, 0.5f);
            }
        }

        foreach(var slotB in slotsB)
        {
            LeanTween.moveY(slotB.gameObject, b_y, 0.5f);
            if(slotB.draggableReference != null)
            {
                LeanTween.moveLocalY(slotB.draggableReference.gameObject, b_y, 0.5f);
            }
        }
    }
}
