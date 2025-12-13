using System.Collections;
using System.Collections.Generic; // using Unity.Multiplayer.Center.Common.Analytics;
// using UnityEditor.UIElements;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;

    private bool isMoving;

    private Vector2 input;

    private Animator animator;

    // public LayerMask solidObjectsLayer;
    public LayerMask interactableLayer;

    private void Awake()
    {
            animator = GetComponent<Animator>();
    }

    public void HandleUpdate()
    {
        if (!isMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");


            if (input.x != 0) input.y = 0;


            if (input != Vector2.zero)
            {

                // animator.SetFloat("moveX", input.x); Temporarily disabled this during firebase implementation
                // animator.SetFloat("moveY", input.y); Temporarily disabled this during firebase implementation

                var targetPos = transform.position;
                targetPos.x += input.x;
                targetPos.y += input.y;

                if (IsWalkable(targetPos))
                    StartCoroutine(Move(targetPos));
            }

        }

        // animator.SetBool("isMoving", isMoving); Temporarily disabled this during firebase implementation


        if (Input.GetKeyDown(KeyCode.Z))
            Interact();
    }

    void Interact()
    {
        var facingDir = new Vector3(animator.GetFloat("moveX"), animator.GetFloat("moveY"));
        var interactPos = transform.position + facingDir;

        //Debug.DrawLine(transform.position, interactPos, Color.red, 1f);

        var collider = Physics2D.OverlapCircle(interactPos, 0.2f, interactableLayer);
        if (collider != null)
        {
            // Debug.Log("there is an NPC here!");
            collider.GetComponent<Interactable>()?.Interact();
        }

    }

    IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;

        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;

        isMoving = false;
    }
    
    private bool IsWalkable(Vector3 targetPos)
    {
        if (Physics2D.OverlapCircle(targetPos, 0.2f, interactableLayer) != null) // solidObjectsLayer
        {
            return false;
        }

        return true;
     }
    
}
