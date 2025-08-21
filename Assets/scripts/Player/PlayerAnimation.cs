using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

// Lista de especies de animales
public enum SpieceEnum
{
    bear, frog, condor, chamaleon
}

public class PlayerAnimation : MonoBehaviour
{
    [Header("Prefabs, Spcieces and Agent")]
    // Variables
    [SerializeField] private List<GameObject> Prefabs; // Lista de prefabs disponibles
    [SerializeField] private SpieceEnum SpieceEnum; // Especie del animal
    [SerializeField] private NavMeshAgent Agent; // Componente NavMeshAgent del jugador
    private MeshAnimation MeshAnimate; // Componente MeshAnimation del personaje jugador
    private Vector3 MeshScale = new Vector3(0.5f, 0.5f, 0.5f); // Pone el modelo a la mitad de tamaño
    private Vector3 MeshPosition = new Vector3(0, -1, 0); // Pone el modelo a nivel del suelo

    void Start()
    {
        // Invoca al prefab como hijo del jugador
        GameObject Children = Instantiate(Prefabs[(int)SpieceEnum], transform.position, transform.rotation);

        // Establece la escala, padre y posición del modelo
        Children.transform.localScale = MeshScale;
        Children.transform.parent = transform;
        Children.transform.localPosition = MeshPosition;

        // Halla el MeshAnimation del prefab y lo inicializa en la primera skin
        MeshAnimate = Children.GetComponent<MeshAnimation>();
        MeshAnimate.SetSkin(0);
    }

    void Update()
    {
        // Retorna si no hay prefabs cargados
        if (MeshAnimate == null) return;

        // Actualiza la animación del animal según su velocidad
        string SpeedRatio = (Agent.velocity.magnitude / Agent.speed).ToString();
        MeshAnimate.UpdateAnimation("Speed", SpeedRatio);
    }

    // Cambia la especie del animal
    void UpdateSpiece(SpieceEnum Spiece)
    {
        // Retorna si no hay prefabs cargados
        if (Spiece == SpieceEnum) return;

        // Cambia la apariencia en función de la especie elegida
        SpieceEnum = Spiece;
        GameObject Children = Instantiate(Prefabs[(int)SpieceEnum], transform.position, transform.rotation);
        Children.transform.parent = transform;
        MeshAnimate = Children.GetComponent<MeshAnimation>();
    }
}