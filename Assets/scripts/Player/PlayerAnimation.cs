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
    // Variables
    [SerializeField] private List<GameObject> Prefabs;
    [SerializeField] private SpieceEnum SpieceEnum;
    [SerializeField] private NavMeshAgent Agent;
    private MeshAnimation MeshAnimate;
    private List<int> SpieceList;
    private Vector3 MeshScale = new Vector3(0.5f, 0.5f, 0.5f); // Pone el modelo a la mitad de tamaño
    private Vector3 MeshPosition = new Vector3(0, -1, 0); // Pone el modelo a nivel del suelo

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject Children = Instantiate(Prefabs[(int)SpieceEnum], transform.position, transform.rotation);
        Children.transform.localScale = MeshScale;
        Children.transform.parent = transform;
        Children.transform.localPosition = MeshPosition;
        MeshAnimate = Children.GetComponent<MeshAnimation>();
        MeshAnimate.SetSkin(0);
        SpieceList.Add(0);
        SpieceList.Add(0);
        SpieceList.Add(0);
        SpieceList.Add(0);
    }

    // Update is called once per frame
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
        MeshAnimate.SetSkin(SpieceList[(int)SpieceEnum]);
    }
}