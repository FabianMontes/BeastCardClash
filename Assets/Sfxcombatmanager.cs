using UnityEngine;

public class Sfxcombatmanager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    AudioSource audio;
    [SerializeField]AudioClip[] clips;
    void Start()
    {
        audio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (Combatjudge.combatjudge.GetSetMoments())
        {
            case SetMoments.PickDice:
                break;
            case SetMoments.RollDice:
                break;
            case SetMoments.RevealDice:
                break;
            case SetMoments.GlowRock:
                break;
            case SetMoments.MoveToRock:
                break;
            case SetMoments.SelecCombat:
                break;
            case SetMoments.PickCard:
                break;
            case SetMoments.Reveal:
                break;
            case SetMoments.Result:
                break;
            case SetMoments.End:
                break;
            case SetMoments.Loop:
                break;
            case SetMoments.round:
                break;
            case SetMoments.rounded:
                break;
        }
    }

    public void changeSource(int index)
    {
        audio.Stop();
        audio.resource = clips[index];
        audio.Play();

    }
}
