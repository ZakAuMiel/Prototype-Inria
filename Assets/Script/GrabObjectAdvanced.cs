using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class GrabObjectAdvanced : MonoBehaviour
{
    [Header("Références")]
    public Camera playerCamera;
    public Transform grabPoint;
    public QuestManager questManager;

    [Header("Paramètres")]
    public float grabDistance = 3f;
    public float holdSmoothness = 10f;
    public float throwForce = 10f;

    [Header("Cube Tag")]
    public string cubeTag = "Cube";

    private Rigidbody grabbedRb;
    private Vector3 originalLinearVelocity;
    private float originalLinearDamping;
    private float originalAngularDamping;

    private bool hasTakenCube = false;
    private bool hasRotatedCube = false;
    private Vector3 lastRotation;

    void Update()
    {
        // Rotation du cube avec clic droit
        if (grabbedRb && Input.GetMouseButton(1))
        {
            float rotX = Input.GetAxis("Mouse X") * 10f;
            float rotY = -Input.GetAxis("Mouse Y") * 10f;

            grabbedRb.transform.Rotate(playerCamera.transform.up, rotX, Space.World);
            grabbedRb.transform.Rotate(playerCamera.transform.right, rotY, Space.World);

            // Détection rotation
            if (!hasRotatedCube && questManager != null)
            {
                Vector3 rotationDelta = grabbedRb.transform.eulerAngles - lastRotation;
                if (rotationDelta.magnitude > 5f)
                {
                    questManager.PlayerDidAction("RotateCube");
                    hasRotatedCube = true;
                }
            }

            lastRotation = grabbedRb.transform.eulerAngles;
        }

        // Lancer du cube avec clic gauche
        if (grabbedRb && Input.GetMouseButtonDown(0))
        {
            Rigidbody rbToThrow = grabbedRb; // copie locale pour éviter null
            if (questManager != null && rbToThrow.CompareTag(cubeTag))
            {
                questManager.PlayerDidAction("ThrowCube");
            }

            ReleaseObject(true);

            // Reset pour prochaine prise
            hasRotatedCube = false;
            hasTakenCube = false;
        }

        // Prendre ou lâcher avec E
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

            if (rb != null && rb.CompareTag(cubeTag))
            {
                grabbedRb = rb;

                // Sauvegarde des paramètres d'origine
                originalLinearVelocity = rb.linearVelocity;
                originalLinearDamping  = rb.linearDamping;
                originalAngularDamping = rb.angularDamping;

                rb.linearVelocity = Vector3.zero;
                rb.linearDamping  = 10f;
                rb.angularDamping = 10f;

                // Validation prise
                if (!hasTakenCube && questManager != null)
                {
                    questManager.PlayerDidAction("TakeCube");
                    hasTakenCube = true;
                }

                lastRotation = rb.transform.eulerAngles; // rotation initiale
            }
        }
    }

    void ReleaseObject(bool throwIt)
    {
        if (!grabbedRb) return;

        Rigidbody rb = grabbedRb; // copie locale
        rb.linearDamping  = originalLinearDamping;
        rb.angularDamping = originalAngularDamping;

        if (throwIt)
            rb.linearVelocity = playerCamera.transform.forward * throwForce;

        grabbedRb = null;
    }
}
