using UnityEngine;
using System.Collections;

public class Draggable : MDSBehaviour
{
    private bool draggingItem = false;
    private GameObject draggedObject;
    private Vector2 touchOffset;
    private SpriteRenderer render;

    void Start()
    {
        render = this.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (HasInput)
            DragOrPickUp();
        else
            if (draggingItem)
                DropItem();
    }

    /// <summary>
    /// Retorna a posição atual do toque/clique.
    /// </summary>
    Vector2 CurrentTouchPosition
    {
        get
        {
            Vector2 inputPos;
            inputPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            return inputPos;
        }
    }

    /// <summary>
    /// Método chamado quando um toque/clique é detectado. 
    /// Verifica se o toque/clique foi em um objeto arrastável.
    /// Se sim, pega o objeto e atualiza sua posição de acordo com o movimento do mouse.
    /// </summary>
    private void DragOrPickUp()
    {
        var inputPosition = CurrentTouchPosition;

        if (draggingItem)
            draggedObject.transform.position = inputPosition + touchOffset;
        else
        {
            RaycastHit2D[] touches = Physics2D.RaycastAll(inputPosition, inputPosition, 0.5f);
            if (touches.Length > 0)
            {
                var hit = touches[0];
                if (hit.transform != null)
                {
                    draggingItem = true;
                    draggedObject = hit.transform.gameObject;
                    draggedObject.GetComponent<SpriteRenderer>().sortingOrder = 5;
                    touchOffset = (Vector2)hit.transform.position - inputPosition;
                    draggedObject.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
                }
            }
        }
    }

    /// <summary>
    /// Retorna verdadeiro se houver alguma interação do usuário por toque ou clique.
    /// </summary>
    private bool HasInput { get { return Input.GetMouseButton(0); } }

    /// <summary>
    /// Solta o objeto que foi pego ou está sendo arrastado pelo toque/mouse.
    /// </summary>
    void DropItem()
    {
        draggingItem = false;
        draggedObject.transform.localScale = new Vector3(1f, 1f, 1f);
        render.sortingOrder = 0;
    }
}

