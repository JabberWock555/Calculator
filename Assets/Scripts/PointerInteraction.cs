using UnityEngine;
using JabberWockStudio.Calculator;

public class PointerInteraction : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.TryGetComponent<Button>(out Button button))
                {
                    button.OnButtonClicked();
                }
            }
        }
    }
}
