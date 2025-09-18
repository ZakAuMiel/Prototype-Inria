using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class GrabObjectAdvanced : MonoBehaviour
{
    [Header("Références")]
    public Camera playerCamera;
    public Transform grabPoint;

    [Header("Paramètres de prise")]
    public float grabDistance = 3f;
    public float holdSmoothness = 10f;

    [Header("Paramètres de lancer")]
    public float throwForce = 10f;

    private Rigidbody grabbedRb;
    private Vector3 originalLinearVelocity;
    private float originalLinearDamping;
    private float originalAngularDamping;

    void Update()
    {
        // Clic droit : rotation de l’objet dans la main
        if (grabbedRb && Input.GetMouseButton(1))
        {
            float rotX = Input.GetAxis("Mouse X") * 10f;
            float rotY = -Input.GetAxis("Mouse Y") * 10f;
            grabbedRb.transform.Rotate(playerCamera.transform.up, rotX, Space.World);
            grabbedRb.transform.Rotate(playerCamera.transform.right, rotY, Space.World);
        }

        // Clic gauche : lancer
        if (grabbedRb && Input.GetMouseButtonDown(0))
        {
            ReleaseObject(true);
        }

        // Touche E : prendre ou lâcher
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (grabbedRb) ReleaseObject(false);
            else TryGrab();
        }
    }

    void FixedUpdate()
    {
        if (grabbedRb)
        {
            // Maintenir l’objet vers le point de prise
            Vector3 targetPos = grabPoint.position;
            Vector3 newPos = Vector3.Lerp(grabbedRb.position, targetPos, Time.fixedDeltaTime * holdSmoothness);
            grabbedRb.MovePosition(newPos);
        }
    }

    void TryGrab()
    {
        if (playerCamera == null) return;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, grabDistance))
        {
            Rigidbody rb = hit.collider.attachedRigidbody;
            if (rb != null)
            {
                grabbedRb = rb;

                // Sauvegarde des paramètres d’origine
                originalLinearVelocity = rb.linearVelocity;
                originalLinearDamping  = rb.linearDamping;
                originalAngularDamping = rb.angularDamping;

                // Ajustement pour la tenue
                rb.linearVelocity = Vector3.zero;
                rb.linearDamping  = 10f;
                rb.angularDamping = 10f;
            }
        }
    }

    void ReleaseObject(bool throwIt)
    {
        if (!grabbedRb) return;

        // Restauration des valeurs
        grabbedRb.linearDamping  = originalLinearDamping;
        grabbedRb.angularDamping = originalAngularDamping;

        if (throwIt)
        {
            grabbedRb.linearVelocity = playerCamera.transform.forward * throwForce;
        }

        grabbedRb = null;
    }
}
