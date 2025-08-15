using UnityEngine;

public class changenamee : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void named(string name)
    {
        GameState.singleton.SetPlayer(name);
    }
}
