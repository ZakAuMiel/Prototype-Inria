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

    [Header("Materials")]
    public Material normalMaterial;   // Matériau par défaut
    public Material highlightMaterial; // Matériau pour hover + grab

    private Rigidbody grabbedRb;
    private Vector3 originalLinearVelocity;
    private float originalLinearDamping;
    private float originalAngularDamping;

    private bool hasTakenCube = false;
    private bool hasRotatedCube = false;
    private Vector3 lastRotation;

    // pour gérer le survol
    private Renderer currentHoverRenderer;
    private Renderer grabbedRenderer;

    void Update()
    {
        HandleHover();

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
            if (questManager != null && grabbedRb.CompareTag(cubeTag))
                questManager.PlayerDidAction("ThrowCube");

            ReleaseObject(true);
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

    void HandleHover()
    {
        // Pas de highlight si on tient déjà un cube
        if (grabbedRb)
            return;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, grabDistance))
        {
            Renderer r = hit.collider.GetComponent<Renderer>();
            if (r && hit.collider.CompareTag(cubeTag))
            {
                if (currentHoverRenderer != r)
                {
                    // enlever highlight précédent
                    if (currentHoverRenderer) currentHoverRenderer.material = normalMaterial;
                    currentHoverRenderer = r;
                    currentHoverRenderer.material = highlightMaterial;
                }
            }
            else
            {
                if (currentHoverRenderer)
                {
                    currentHoverRenderer.material = normalMaterial;
                    currentHoverRenderer = null;
                }
            }
        }
        else
        {
            if (currentHoverRenderer)
            {
                currentHoverRenderer.material = normalMaterial;
                currentHoverRenderer = null;
            }
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

                // garde le highlight pendant le grab
                grabbedRenderer = hit.collider.GetComponent<Renderer>();
                if (grabbedRenderer) grabbedRenderer.material = highlightMaterial;

                // Validation prise
                if (!hasTakenCube && questManager != null)
                {
                    questManager.PlayerDidAction("TakeCube");
                    hasTakenCube = true;
                }

                lastRotation = rb.transform.eulerAngles;
            }
        }
    }

    void ReleaseObject(bool throwIt)
    {
        if (!grabbedRb) return;

        Rigidbody rb = grabbedRb;
        rb.linearDamping  = originalLinearDamping;
        rb.angularDamping = originalAngularDamping;

        if (throwIt)
            rb.linearVelocity = playerCamera.transform.forward * throwForce;

        // remet le matériau normal à la release
        if (grabbedRenderer)
        {
            grabbedRenderer.material = normalMaterial;
            grabbedRenderer = null;
        }

        grabbedRb = null;
        hasRotatedCube = false;
        hasTakenCube = false;
    }
}
