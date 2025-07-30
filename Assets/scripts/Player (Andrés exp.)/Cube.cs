using UnityEngine;
using UnityEngine.InputSystem;

public class Cube : MonoBehaviour
{
    // Variables
    [SerializeField] Transform camera;
    [SerializeField] Transform player;
    [SerializeField] float speed = 5f;
    [SerializeField] bool mode = true; // true = arrows, false = mouse

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (mode)
        {
            Vector3 position = player.position;
            if (Keyboard.current.upArrowKey.isPressed)
                position += camera.forward * speed * Time.deltaTime;

            if (Keyboard.current.downArrowKey.isPressed)
                position -= camera.forward * speed * Time.deltaTime;

            if (Keyboard.current.leftArrowKey.isPressed)
                position -= camera.right * speed * Time.deltaTime;

            if (Keyboard.current.rightArrowKey.isPressed)
                position += camera.right * speed * Time.deltaTime;

            transform.position = position;
        }
        else
        {
            if (Input.GetMouseButtonDown(0)) // clic izquierdo
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    Debug.Log("Golpeó: " + hit.collider.name);
                    Debug.DrawLine(ray.origin, hit.point, Color.red, 1f);

                    // Puedes usar el punto de impacto
                    Vector3 puntoImpacto = hit.point;

                    // Por ejemplo, mover un objeto allí
                    transform.position = puntoImpacto;
                }
            }
        }

    }
}
