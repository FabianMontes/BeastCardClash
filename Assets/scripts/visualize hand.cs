using UnityEngine;

public class visualizehand : MonoBehaviour
{
    [SerializeField] Transform guide;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.GetChild(1).gameObject.SetActive(guide.gameObject.activeSelf);
    }
}
