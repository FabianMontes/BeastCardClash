using UnityEngine;

public enum Inscription
{
    fire = 0, earth = 1, water = 2, air = 3, duel = 4, pick = 6, empty = 5
}

[DefaultExecutionOrder(-2)]

public class RockBehavior : MonoBehaviour
{
    public PlayZone father;
    [SerializeField] private Sprite[] sprite;
    [SerializeField] public Inscription inscription = Inscription.empty;

    PlayerToken[] playersOn = null;
    SpriteRenderer simbol;
    SpriteRenderer itself;
    public Vector3 direction = Vector3.forward;
    public float angle = 0;
    public int numbchild = 0;
    public bool shiny = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        simbol = transform.GetChild(0).GetComponent<SpriteRenderer>();
        itself = transform.GetComponent<SpriteRenderer>();
        simbol.sprite = sprite[(int)inscription];

        if (father == null) return;

        transform.position = father.transform.position + direction * father.radius;
        transform.localScale = Vector3.one * father.RockScale;
        transform.rotation = Quaternion.Euler(90, angle - 90, 0); // first set it to look up, second to look to the left
    }

    // Update is called once per frame
    void Update()
    {
        if (Combatjudge.combatjudge.GetSetMoments() != SetMoments.GlowRock) shiny = false;
        itself.color = shiny ? Color.yellow : Color.black;
    }

    private void OnMouseDown()
    {
        if (shiny) FindFirstObjectByType<Combatjudge>().MoveToRock(this);
    }

    public RockBehavior[] getNeighbor(int al)
    {
        int many = father.many;
        RockBehavior[] neighbors = new RockBehavior[2];
        int oneside = ((numbchild + al) % many);
        int otherside = ((numbchild - al + many) % many);
        neighbors[0] = father.transform.GetChild(oneside).GetComponent<RockBehavior>();
        neighbors[1] = father.transform.GetChild(otherside).GetComponent<RockBehavior>();
        return neighbors;
    }

    public void AddPlayer(PlayerToken token)
    {
        if (playersOn == null)
        {
            playersOn = new PlayerToken[1];
            playersOn[0] = token;
        }
        else
        {
            PlayerToken[] newPlay = new PlayerToken[playersOn.Length + 1];
            for (int i = 0; i < playersOn.Length; i++)
            {
                newPlay[i] = playersOn[i];
            }
            newPlay[playersOn.Length] = token;
            playersOn = newPlay;
        }
    }

    public void RemovePlayer(PlayerToken token)
    {
        if (playersOn == null) return;

        int a = 0;
        for (int i = 0; i < playersOn.Length; i++)
        {
            if (playersOn[i] == token)
            {
                a++;
                playersOn[i] = null;
            }
        }

        PlayerToken[] newPlay = new PlayerToken[playersOn.Length - a];
        int j = 0;
        for (int i = 0; i < playersOn.Length; i++)
        {
            if (playersOn[i] != null)
            {
                newPlay[j] = playersOn[i];
                j++;
            }
        }

        playersOn = newPlay;
    }

    public bool manyOn()
    {
        return playersOn != null ? playersOn.Length > 1 : false;
    }

    public int GetPlayersOn()
    {
        int many = 0;
        foreach (PlayerToken p in playersOn)
        {
            many += (int)Mathf.Pow(2, p.player.indexPlayer);
        }

        return many;
    }

    public int ManyPlayerOn()
    {
        return playersOn.Length;
    }
}
