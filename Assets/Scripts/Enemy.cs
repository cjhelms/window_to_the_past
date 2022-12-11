using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    enum State
    {
        Chase,
        InitDash,
        InitTelegraph,
        Telegraph,
        Dash,
        InitCooldown,
        Cooldown,
        Rewind,
    }    

    // Parameters set in Unity editor
    [SerializeField] int MaxHistory;
    [SerializeField] int FlashbackSpeedup;
    [SerializeField] float AttackDistance;
    [SerializeField] float ChaseSpeed;
    [SerializeField] float TelegraphCycleDuration;
    [SerializeField] float TelegraphDuration;
    [SerializeField] float DashDistance;
    [SerializeField] float DashDuration;
    [SerializeField] float CooldownDuration;

    // Player the enemy should target, set outside of enemy
    public GameObject player
    { get; set; }

    // State of the enemy, used to change behavior of enemy based on situation
    State state;

    // Sprite of the enemy, used to display the enemy on the screen
    SpriteRenderer sprite;

    // Variables for deciding when telegraph is complete
    float telegraphTimer;
    float telegraphCycleCount;

    // Variables used to execute dash, set when initialized the dash
    Vector3 dashStart;
    Vector3 dashEnd;

    // Timer for interpolating position between start and end of dash over dash duration
    float dashTimer;

    // Timer for determining when the cooldown is over
    float cooldownTimer;

    // Record of properties used to rewind the enemy
    Vector3[] positionHistory; 
    Color[] spriteColorHistory; 
    int historySz;
    int historyNdx;
    int historyDecimator;

    public void SetTarget(GameObject player)
    {
        this.player = player;
    }

    public void Flashback()
    {
        state = State.Rewind;
        Debug.Log("[any] --> Rewind");
    }

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        state = State.Chase;
        positionHistory = new Vector3[MaxHistory];
        spriteColorHistory = new Color[MaxHistory];
        historyDecimator = 0;
    }

    void Update()
    {
        if(player == null)
        {
            Debug.Log("Player is null");
            return;
        }

        if(state != State.Rewind && historyDecimator > FlashbackSpeedup) {
            // Record history so enemy can be rewound during flashback
            RecordHistory();
            historyDecimator = 0;
        }
        else
        {
            historyDecimator++;
        }

        // State machine
        switch(state)
        {
            case State.Chase:
                Chase();
                break;
            case State.InitDash:
                InitDash();
                break;
            case State.InitTelegraph:
                InitTelegraph();
                break;
            case State.Telegraph:
                Telegraph();
                break;
            case State.Dash:
                Dash();
                break;
            case State.InitCooldown:
                InitCooldown();
                break;
            case State.Cooldown:
                Cooldown();
                break;
            case State.Rewind:
                Rewind();
                break;
            default:
                Debug.Log("Unhandled state! " + state);
                break;
        }
    }

    void RecordHistory()
    {
        positionHistory[historyNdx] = gameObject.transform.position;
        spriteColorHistory[historyNdx] = sprite.color; 
        historyNdx = (historyNdx + 1) % MaxHistory;
        if(historySz < MaxHistory)
        {
            historySz++;
        }
    }

    void Chase()
    {
        gameObject.transform.position += GetDeltaPositionVector();
        if (GetTargetVector().magnitude < AttackDistance) 
        { 
             state = State.InitDash;
             Debug.Log("Chase --> InitDash");
        }
    } 

    void InitDash()
    {
        Vector3 targetVector = GetTargetVector();        
        dashStart = gameObject.transform.position;        
        dashEnd = new Vector3(
            dashStart.x + (targetVector.normalized.x * DashDistance),
            dashStart.y + (targetVector.normalized.y * DashDistance),
            0.0f);
        dashTimer = 0.0f;
        state = State.InitTelegraph; 
        Debug.Log("InitDash --> InitTelegraph");
    }

    void InitTelegraph()
    {
        telegraphTimer = 0.0f;
        telegraphCycleCount = 0;
        state = State.Telegraph;
        Debug.Log("InitTelegraph --> Telegraph");
    }

    void Telegraph()
    {
        telegraphTimer += Time.deltaTime;
        float timeSinceLastCycle = (telegraphTimer - telegraphCycleCount * TelegraphCycleDuration);
        if(timeSinceLastCycle >= TelegraphCycleDuration)
        {
            telegraphCycleCount++;
            if(sprite.color == Color.red)
            {
                sprite.color = Color.white;
            }
            else
            {
                sprite.color = Color.red;
            }
        }
        if(telegraphTimer >= TelegraphDuration)
        {
            sprite.color = Color.red;
            state = State.Dash;
            Debug.Log("Chase --> Dash");
        }
    }

    void Dash()
    {
        if (dashTimer < DashDuration)
        {
            float timeRatio = dashTimer / DashDuration;
            gameObject.transform.position = new Vector3(
                Mathf.Lerp(dashStart.x, dashEnd.x, timeRatio),
                Mathf.Lerp(dashStart.y, dashEnd.y, timeRatio),
                0.0f);
            dashTimer += Time.deltaTime;
        }
        else
        {
            state = State.InitCooldown;
            Debug.Log("Dash --> InitCooldown");
        }
    }

    void InitCooldown()
    {
        cooldownTimer = 0.0f;
        sprite.color = Color.gray;
        state = State.Cooldown;
        Debug.Log("InitCooldown --> Cooldown");
    }

    void Cooldown()
    {
        cooldownTimer += Time.deltaTime;
        if(cooldownTimer >= CooldownDuration)
        {
            sprite.color = Color.red;
            state = State.Chase;
            Debug.Log("Cooldown --> Chase");
        }
    }

    void Rewind()
    {
        historyNdx--;
        if(historyNdx < 0)
        {
            historyNdx = MaxHistory - 1;
        }
        gameObject.transform.position = positionHistory[historyNdx];
        sprite.color = spriteColorHistory[historyNdx];
        historySz--;
        if(historySz == 0)
        {
            state = State.Chase;
        }
    }

    Vector3 GetTargetVector()
    {
        return player.transform.position - gameObject.transform.position;
    }

    Vector3 GetDeltaPositionVector()
    {
        return GetTargetVector().normalized * ChaseSpeed * Time.deltaTime;
    }
}
