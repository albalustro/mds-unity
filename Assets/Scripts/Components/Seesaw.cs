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


    float scaleState;


    IEnumerator Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        slotsA = mathValidator._validatableA.GetGameObject().GetComponentsInChildren<DropGroupSlot>();
        if (!mathValidator.compareConstante())
            slotsB = mathValidator._validatableB.GetGameObject().GetComponentsInChildren<DropGroupSlot>();

        int? A, B;
        WaitForSeconds wfs = new WaitForSeconds(0.16f);

        while(true)
        {
            mathValidator.GetNumericValues(out A, out B);

            if(!A.HasValue)
                A = 0;

            if(!B.HasValue)
                B = 0;


            if(A.Value > B.Value && scaleState != -1)
            {
                _spriteRenderer.sprite = Left_AIsGreater;
                TipTheScales(-1);
                yield return wfs;
            }

            else if(A.Value == B.Value && scaleState != 0)
            {
                _spriteRenderer.sprite = Middle_Equal;
                TipTheScales(0);
                yield return wfs;
            }

            else if(A.Value < B.Value && scaleState != 1)
            {
                _spriteRenderer.sprite = Right_BIsGreater;
                TipTheScales(1);
                yield return wfs;
            }
            yield return null;
        }
    }

    void TipTheScales(int myState)
    {
        switch(myState)
        {
            case -1:
                SetPosition(min_PosY, max_PosY);
                break;
            case 0:
                SetPosition(balanced_PosY, balanced_PosY);
                break;
            case 1:
                SetPosition(max_PosY, min_PosY);
                break;
        }
        scaleState = myState;
    }


    void SetPosition(float a_y, float b_y)
    {
        Vector3 pos;
        foreach(var slotA in slotsA)
        {
            pos = slotA.gameObject.transform.position;
            pos.y = a_y;
            slotA.gameObject.transform.position= pos;
            if(slotA.draggableReference != null)
            {
                LeanTween.move(slotA.draggableReference.gameObject, slotA.gameObject.transform.position, 0.6f);
            }
        }

        if (slotsB != null)
        {
            foreach (var slotB in slotsB)
            {
                pos = slotB.gameObject.transform.position;
                pos.y = b_y;
                slotB.gameObject.transform.position = pos;
                if (slotB.draggableReference != null)
                {
                    LeanTween.move(slotB.draggableReference.gameObject, slotB.gameObject.transform.position, 0.6f);
                }
            }
        }
    }


//#if UNITY_EDITOR
//    void OnDrawGizmos()
//    {
//        Gizmos.color = Color.red;

//        Gizmos.DrawWireCube(bounds.center, bounds.size);
//    }
//#endif
}
