using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class RefactoredInputSystems: MonoBehaviour
{
    public UnityEvent<Vector2> OnMovementInput, OnPointerInput;
    public UnityEvent OnAttack;

    [SerializeField] private InputActionReference movementInput, pointerInput, attacktInput;

    private void OnEnable()
    {
        attacktInput.action.performed += PerformAttack;
    }

    private void OnDisable()
    {
        attacktInput.action.performed -= PerformAttack;
    }

    private void PerformAttack(InputAction.CallbackContext context)
    {
        OnAttack?.Invoke();
    }

    private void Update()
    {
        OnMovementInput?.Invoke(GetMovement());
        OnPointerInput?.Invoke(GetPointer());
    }

    public Vector2 GetMovement()
    {
        return movementInput.action.ReadValue<Vector2>().normalized;
    }

    public Vector3 GetPointer()
    {
        Vector3 targetPos = pointerInput.action.ReadValue<Vector2>();
        targetPos.z = Camera.main.nearClipPlane;
        return Camera.main.ScreenToWorldPoint(targetPos);
    }
}