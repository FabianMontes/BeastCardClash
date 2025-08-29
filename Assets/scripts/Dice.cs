using UnityEngine;

public class Dice : MonoBehaviour
{
    // Variables
    public int Value { get; private set; } // Getter público del valor del dado
    int _maxValue; // Valor máximo
    bool _rolling; // Indica si se está lanzando el dado

    void Start()
    {
        _maxValue = CombatJudge.CombatJudgeInstance.maxDice;
        _rolling = false;
    }

    void Update()
    {
        // Si no se lanza el dado, no hace nada
        if (!_rolling) return;
        
        // Inicia a rotar el dado con un valor aleatorio
        CombatJudge.CombatJudgeInstance.StartRolling();
        Value = Random.Range(1, _maxValue + 1);
        
        // Rota el dado en cada frame
        Vector3 vector3 = new Vector3(0, 45, 0);
        switch (Value)
        {
            case 1:
                vector3.z = 90;
                break;
            case 2:
                vector3.x = -90;
                break;
            case 3:
                // 3 es el valor por defecto, entonces no tiene rotación adicional
                break;
            case 4:
                vector3.x = 180;
                break;
            case 5:
                vector3.x = 90;
                break;
            case 6:
                vector3.z = -90;
                break;
        }
        
        transform.rotation = Quaternion.Euler(vector3);
    }

    /// <summary>
    /// Cuando el mouse presiona el dado, lo lanza llamando a Roll
    /// </summary>
    void OnMouseDown()
    {
        if (CombatJudge.CombatJudgeInstance.FocusOnTurn()) Roll();
    }

    /// <summary>
    /// Indica que va a empezar el lanzamiento del dado
    /// </summary>
    public void Roll()
    {
        if (CombatJudge.CombatJudgeInstance.GetSetMoments() == SetMoments.PickDice) _rolling = true;
    }

    /// <summary>
    /// Cuando el mouse deja de apuntar al collider del dado, termina el lanzamiento llamando a Unroll
    /// </summary>
    void OnMouseExit()
    {
        if (CombatJudge.CombatJudgeInstance.FocusOnTurn()) Unroll();
    }
    
    /// <summary>
    /// Termina el lanzamiento y lo indica
    /// </summary>
    public void Unroll()
    {
        if (!_rolling) return;
        
        _rolling = false;
        CombatJudge.CombatJudgeInstance.Rolled();
    }

    /// <summary>
    /// Cuando se suelta el botón del mouse, termina el lanzamiento llamando a Unroll
    /// </summary>
    void OnMouseUp()
    {
        if (CombatJudge.CombatJudgeInstance.FocusOnTurn()) Unroll();
    }
}
