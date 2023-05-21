using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float InteractionRange = 2;
    public float DetectionRange = 3;

    InteractionScript interactionObject;
    private TinyInput tinyInput;

    private void Awake()
    {
        tinyInput = InputManager.Instance.input;
        tinyInput.Player.Interaction.started += ctx => { if (interactionObject) { interactionObject.Interaction(); } };
    }
    public void OnEnable() { tinyInput.Enable(); }
    public void OnDisable() { tinyInput.Disable(); }

    public void Update()
    {
        interactionObject = null;
        Collider[] colliderArray = Physics.OverlapSphere(Camera.main.transform.position, DetectionRange);
        foreach (Collider collider in colliderArray)
        {
            if(collider.TryGetComponent<InteractionScript>(out InteractionScript interaction))
            {
                interaction.OnDetection();
            }
        }
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.TransformDirection(Vector3.forward) * InteractionRange, out hit, InteractionRange))
        {
            if(hit.transform.TryGetComponent<InteractionScript>(out InteractionScript interaction))
            {
                interaction.OnSelected();
                interactionObject = interaction;
            }
        }
    }

    // Gizmos
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0.9f, 0.015f, 0.1f);
        Gizmos.DrawSphere(Camera.main.transform.position, DetectionRange);
        Gizmos.DrawWireSphere(Camera.main.transform.position, DetectionRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(Camera.main.transform.position, Camera.main.transform.TransformDirection(Vector3.forward) * InteractionRange);
    }
}
