using UnityEngine;

enum SetupConfig
{
    normal, fullall, fullone
}


[DefaultExecutionOrder(-3)]
public class PlayZone : MonoBehaviour
{
    
    [Header("StadiumSetup")]
    [SerializeField] public float radius=5;
    [SerializeField] public int many=10;
    [SerializeField] SetupConfig config;
    [Header("StadiumRock")]
    
    [SerializeField] public float RockScale=1;
    [SerializeField] GameObject rockPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() // Crear zona de batalla
    {
        float angle = 360 / many;
        float anglerad = angle * Mathf.PI / 180f;
        int elem = 0;
        int nonelem = 0;

        int redelement = (int)Mathf.Ceil(many / 5f);
        int divelement = (int)Mathf.Ceil(many * 1.0f / redelement);
        int nelem = 0;


        for (int i = 0; i < many; i++) // the estup creates like a none clock creations order
        {
            float x = Mathf.Cos(anglerad * i);
            float z = Mathf.Sin(anglerad * i);
            Inscription inscripcion = Inscription.empty;
            switch (config)
            {
                case SetupConfig.normal:

                    if (i == divelement * nelem)
                    {

                        inscripcion = (Inscription)(nonelem + 4);
                        nonelem = (nonelem + 1) % 2; // how many none unique elements you want
                        nelem++;
                    }
                    else
                    {

                        inscripcion = (Inscription)(elem);
                        elem = (elem + 1) % 4;// many elements
                    }
                    break;
                case SetupConfig.fullall:
                    inscripcion = Inscription.pick;
                    break;
                case SetupConfig.fullone:
                    inscripcion = Inscription.duel; // set who you want in all maps
                    break;
            }
            Vector3 dir = new Vector3(x, 0, z);
            GameObject stone = Instantiate(rockPrefab);
            stone.transform.parent = transform;
            stone.GetComponent<RockBehavior>().father = this;
            stone.GetComponent<RockBehavior>().angle = -angle * i; // has to look at the oposite of the creation rotation
            stone.GetComponent<RockBehavior>().direction = dir;
            stone.GetComponent<RockBehavior>().inscription = inscripcion;
            stone.GetComponent<RockBehavior>().numbchild = i;


        }
    }
    
    // Update is called once per frame
    void Update()
    {
        
        
    }
}
