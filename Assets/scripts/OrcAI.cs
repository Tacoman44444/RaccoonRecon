using AI.BehaviorTrees;
using AI.Pathfinding;
using BlackboardSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OrcAI : MonoBehaviour
{
    [SerializeField] List<Transform> patrolPoints;
    [SerializeField] Transform player;
    [SerializeField] Transform hostage;
    [SerializeField] LayerMask obstructionMask;
    [SerializeField] GridWorld world;
    [SerializeField] VisionCone visionCone;
    [SerializeField] private AlertEmitter emitter;

    private BehaviorTree tree;
    private Blackboard hostageBlackboard;
    private AIController controller;
    private Dictionary<AlertType, float> alertRadius;
    private float killingDistance = 1.0f;
    private float drunkDuration = 10.0f;
    private float drunkTimer = 0.0f;
    private Action lightOn;
    private Action lightOff;

    readonly Blackboard blackboard = new Blackboard();
    BlackboardKey alertCueKey;
    BlackboardKey alertServed;
    BlackboardKey detected;
    BlackboardKey drunk;

    void Awake()
    {
        lightOn += turnOnLight;
        lightOff += turnOffLight;
        tree = new BehaviorTree("Orc");
        controller = new AIController(transform, patrolPoints, new AStar(world), 5.0f, 7.0f, 8.0f);
        emitter.OnAlertEmitted += onAlert;

        //make the alert dictionary
        alertRadius = new Dictionary<AlertType, float>();
        alertRadius[AlertType.FOOTSTEPS_WALK] = 5f;
        alertRadius[AlertType.FOOTSTEPS_RUN] = 10f;
        alertRadius[AlertType.USED_ABILITY] = 0f;
        alertRadius[AlertType.BROKEN_BONES] = 8f;

        alertCueKey = blackboard.GetOrRegisterKey("AlertCueKey");
        alertServed = blackboard.GetOrRegisterKey("AlertServed");
        drunk = blackboard.GetOrRegisterKey("Drunk");
        blackboard.SetValue(drunk, false);
        
        Leaf patrol = new Leaf("Patrol", new PatrolCommand(controller));
        Leaf alert = new Leaf("Alert", new AlertCommand(controller, blackboard));
        Leaf search = new Leaf("Search", new SearchCommand(controller));
        Leaf detect = new Leaf("Detect", new DetectPlayerCommandOrc(controller, playerInLOS));
        Leaf attack = new Leaf("Attack", new CombatCommand(controller, player), 10);
        Leaf drunkCommand = new Leaf("Drunk", new DrunkCommand(controller, drunkDuration));

        Leaf alerted = new Leaf("IsAlerted", new Condition(isAlerted));
        Leaf inVision = new Leaf("InVision", new Condition(playerInLOS));
        Leaf killingDistance = new Leaf("InKilingDistance", new Condition(inKillingDistance));
        Leaf isdrunk = new Leaf("IsDrunk", new Condition(isDrunk));
        Leaf issober = new Leaf("IsSober", new Condition(isSober));

        Leaf lightoff = new Leaf("TurnLightOff", new ActionCommand(lightOff));
        Leaf lighton = new Leaf("TurnLightOn", new ActionCommand(lightOn));
        Leaf kill = new Leaf("Kill", new ActionCommand(killPlayer));
        Leaf activateAlert = new Leaf("ActivateAlertIndicator", new ActionCommand(activateAlertIndicator));
        Leaf activateCombat = new Leaf("ActivateCombatIndicator", new ActionCommand(activateCombatIndicator));
        Leaf deactivate = new Leaf("DeactivateIndicators", new ActionCommand(deactivateIndicators));

        Sequence combatSequence = new Sequence("CombatSequence", 100);
        combatSequence.AddChild(issober);
        combatSequence.AddChild(inVision);
        combatSequence.AddChild(deactivate);
        combatSequence.AddChild(activateCombat);
        combatSequence.AddChild(attack);
        combatSequence.AddChild(kill);

        Sequence searchSequence = new Sequence("SearchSequence", 50);
        searchSequence.AddChild(issober);
        searchSequence.AddChild(alerted);
        searchSequence.AddChild(deactivate);
        searchSequence.AddChild(activateAlert);
        searchSequence.AddChild(alert);
        searchSequence.AddChild(search);

        Sequence drunkSequence = new Sequence("DrunkSequence", 20);
        drunkSequence.AddChild(deactivate);
        drunkSequence.AddChild(isdrunk);
        drunkSequence.AddChild(lightoff);
        drunkSequence.AddChild(drunkCommand);
        drunkSequence.AddChild(lighton);

        Sequence patrolSequence = new Sequence("PatrolSequence", 10);
        patrolSequence.AddChild(deactivate);
        patrolSequence.AddChild(patrol);

        PrioritySelector rootSelector = new PrioritySelector("RootSelector");
        rootSelector.AddChild(combatSequence);
        rootSelector.AddChild(searchSequence);
        rootSelector.AddChild(drunkSequence);
        rootSelector.AddChild(patrolSequence);

        tree.AddChild(rootSelector);

        hostageBlackboard = GameObject.Find("Hostage").GetComponent<HostageAI>().GetBlackboard();
        detected = hostageBlackboard.GetOrRegisterKey("IsDetected");
    }
    void Update()
    {
        tree.Process();

        if (visionCone.EntityInSight(hostage.position, obstructionMask))
        {
            hostageBlackboard.SetValue(detected, true);
        } 
        else
        {
            hostageBlackboard.SetValue(detected, false);
        }

        GameObject[] grogs = GameObject.FindGameObjectsWithTag("Grog");
        foreach (GameObject grog in  grogs)
        {
            float distance = Vector3.Distance(transform.position, grog.transform.position);
            if (distance < 2.0f)
            {
                Destroy(grog);
                blackboard.SetValue(drunk, true);
                SoundManager.PlaySound(SoundType.GROG_CONSUMED);
            }
        }
        if (blackboard.TryGetValue<bool>(drunk, out bool val) && val)
        {
            Debug.Log("we drunnk");
            drunkTimer += Time.deltaTime;
            if (drunkTimer > drunkDuration)
            {
                blackboard.SetValue(drunk, false);
                drunkTimer = 0.0f;
            }
        }
        
    }

    private void onAlert(Vector2 alert, AlertType type)
    {
        float distance = Vector2.Distance(transform.position, alert);
        if (distance <= alertRadius[type])
        {
            blackboard.SetValue(alertCueKey, alert);
            blackboard.SetValue(alertServed, false);
        }
    }

    public bool isAlerted()
    {
        if (blackboard.TryGetValue<bool>(alertServed, out bool isServed) )
        {
            return !isServed;
        }
        return false;
    }

    public bool playerInLOS()
    {

        return visionCone.EntityInSight(hostage.position, obstructionMask);
    }

    public bool inKillingDistance()
    {
        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= killingDistance)
        {
            Debug.Log("in killing distance");
            return true;
        }
        else
        {
            Debug.Log("not in killing distance");
            return false;
        }
    }

    public bool isDrunk()
    {
        if (blackboard.TryGetValue<bool>(drunk, out bool val))
        {
            Debug.Log(val);
            return val;
        }
        Debug.Log("ERROR::blackboard did not work");
        return false;
    }

    public bool isSober()
    {
        if (blackboard.TryGetValue<bool>(drunk, out bool val))
        {
            return !val;
        }
        Debug.Log("ERROR::blackboard did not work");
        return false;
    }

    public void turnOffLight()
    {
        Transform coneLight = transform.Find("Cone Light");
        if (coneLight == null)
        {
            Debug.Log("ERROR::could not find cone light child object");
            return;
        }
        else
        {
            coneLight.gameObject.SetActive(false);
        }
    }

    public void turnOnLight()
    {
        Transform coneLight = transform.Find("Cone Light");
        if (coneLight == null)
        {
            Debug.Log("ERROR::could not find cone light child object");
            return;
        }
        else
        {
            coneLight.gameObject.SetActive(true);
        }
    }

    public void killPlayer()
    {
        SoundManager.PlaySound(SoundType.PLAYER_KILLED);
        GameResult.PlayerWon = false;
        SceneManager.LoadScene("GameOverScene");
        Debug.Log("GAME OVER");
    }

    public void activateAlertIndicator()
    {
        Transform alertIndicator = transform.Find("AlertIndicator");
        alertIndicator.gameObject.SetActive(true);
        SoundManager.PlaySound(SoundType.ORC_ALERTED);
    }

    public void activateCombatIndicator()
    {
        Transform combatIndicator = transform.Find("CombatIndicator");
        combatIndicator.gameObject.SetActive(true);
        SoundManager.PlaySound(SoundType.ORC_COMBAT);
    }

    public void deactivateIndicators()
    {
        Transform alertIndicator = transform.Find("AlertIndicator");
        Transform combatIndicator = transform.Find("CombatIndicator");
        alertIndicator.gameObject.SetActive(false);
        combatIndicator.gameObject.SetActive(false);

    }
}
