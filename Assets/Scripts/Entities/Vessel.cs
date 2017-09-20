using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SPECIES
{
    SKELETON,
    PRIMAL,
    OGRE,
    PUCK,
    MINOTAUR,
    CENTAUR,
    FISH,
    BIRD,
    DOG,
    CAT,
    EAGLE,
    BOAR,
    SLIME,
    TREE,
    TREEFOLK,
    FAIRY,
    DRAGON,
    ANCIENT,
    GIANT,
    ELEMENTAL,
    ANGEL,
    GOD,
}

public enum VARIATION // Starting states
{
    SLOW,           // Dimwitted, high failure rate, slow movement speed
    QUICK,          // Quicker than normal
    TIRED,          // Spawned with high exhaustion and SLEEPY status
    AGGRESSIVE,     // Spawned with high aggression
    LOUD,           // Noise, dialogue is amplified
    QUIET,          // Noise, dialogue is minimized
    SWEATY,         // Medium Exhaustion, Temperature Up
    CLAMMY,         // Sick, temperature down
    FEVERED,        // Sick, temperature up
    SICK,           // Nausea
    PLAGUED,        // Nausea, Plague spreader
    DILIGENT,       // Focus Up, less likely to 
    WORRYING,       // Focus Down, double stress gain
    PROCRASTINATING,// Lower basic instinct thresholds
    STRONG,         // Spawned with additional power
    WEAK,           // Spawned with less power
    FRAIL,          // Spawned with less HP than normal
    HEARTY,         // Spawned with additional HP
    ARMORED,        // Spawned with some armor
    HEAVILY_ARMORED,// Spawned with lots of armor
    DOCILE,         // Spawned with no aggression
    ALPHA,          // Spawned with maximum ranking among peers (leader), strong
    LONE,           // Spawned with no alliances
    FOREIGN,        // Spawned with no alliances
    UNDEAD,         // Turned from death
    DEAD,           // Recent death
    DECAYING,       // Dead for extended period
}

// Sooth the worrying squirrel with SOOTH and gain significant regard. Will follow and help. Random chance of appearing somewhere to help
// Extreme low regards will seek you out for assassination

public enum PROFESSION
{
    BANDIT,
}

public enum OBJECTIVE
{
    SEEK_COVER,
    FIND_FOOD,
    DEFEND_TARGET,
}

// King is rank #1 (of those within Faction: Kingdom)
// Alpha is rank #1 (of those within Faction: Western Pack)
// King has no rank within Faction: Gods
// King has 
// Servant - Seek food for King
// Mother - Seek food for children

/*
 * Status Effects
 * Traditionally buffs, debuffs, status effects
 * Short term effects, channeled to stick, mana consuming
 * See EverQuest, Final Fantasy VI, Breath of the Wild
 * Effects meant to be cured by others
 */
public enum STATUS 
{
    POISON,         // Slight, long-term DOT (120s), removed with cleanse or antidote
    BLEEDING,       // Slight, long-term DOT (20s), removed with mend
    BURN,           // Extreme, short-term DOT (10s), focusless, temperature up, able to spread, exacerbated by wind, doused by water
    DRAIN,          // Half Poison, HP transfer to caster
    ARMOR,          // Protect against damage, 50% damage (Until used)
    SHIELD,         // Protect against damage (Regenerates, 15m)
    SLOW,           // Slower coursing speed
    FLOAT,          // Glide (5s)
    HEAT_TOLERANCE, // Increase upper temperature range (15m)
    COLD_TOLERANCE, // Decrease lower temperature range (15m)
    LUCKY,          // Increases chance of finding rare items (1Hr)
    SOOTH,          // Stress clear (10s)
    FLOW,           // Slow coursing speed boost (15m)
    SURGE,          // Extreme coursing speed (10s)
    HASTEN,         // Quicker movement, coursing speed (2m, 1 Bar = 1 Sec.)
    INVISIBLE,      // Invisibility (10s, Coursed)
    TRUESIGHT,      // Able to see magic, invisible (Coursed, Item)
    SILENCE,        // Unable to course, can move
    MUFFLE,         // Dampens noises (15m)
    DEAFEN,         // Unable to hear noise (20s)
    BERSERK,        // Boosted melee only, quick fatigue / stress gain
    EXHAUST,        // Additional energy deficit (10s)
    INVIGORATE,     // Additional energy reserve (Until used)
    VULNERABLE,     // Higher chance for status to stick
    CRIPPLE,        // Prevents physical fatigue drain (10s)
    ROOT,           // Prevents movement, able to cast, (5s)
    SNARE,          // Move speed halved, able to cast, (5s)
    BOUND,          // Prevents movement, unable to act, (Rescue)
    BLIND,          // Difficulty seeing (higher chance to miss), targeting (5s)
    SLEEP,          // Prevents action, removes exhaustion (Strike)
    SLEEPY,         // Half energy (Stress)
    DEEP_SLEEP,     // Prevents action, unable to wake through normal means
    CAPTURE,        // Dragged, unable to act, killed if not rescued (Rescue)
    STUN,           // Prevents action, short-term (5s)
    NAUSEA,         // Infrequent stun, interrupt (60s)
    PARALYZED,      // Unable to act (10s)
    PLAGUE,         // Change to spread nausea to those in radius
    FREEZE,         // Prevents action, temperature down over time (10s, Fire)
    PETRIFICATION,  // Turned to stone if not removed (10m)
    DOOM,           // All HP lost once full (2m)
    CURSE,          // High chance negative coursing side effects, of type (Rolling a 1)
    BLESSING,       // High chance positive coursing side effects, of type (Rolling a 20)
    BURDEN,         // Unable to jump, slower movement
    FEATHERLIGHT,   // Able to jump higher, soft landing (5s)
    PANIC,          // Extreme, short-term stress gain (10s)
    TERROR,         // Unable to move + panic (10s)
    DRUNK,          // Warped vision, more interruptions (60s)
    STUPOR,         // Extreme warped vision, no focus (120s)
    DISTRACTION,    // No focus (10s)
    FOCUS,          // Few interruptions (Meditation Length)
    ILLUSION,       // Physically transformation (Until worn)
    ALLURE,         // Significant temporary regard boost, slow regard gain (permanent), draws attention
    HYPNOTIZE,      // Unable to act, controlled by caster (20s, Strike)
    INJURY_LEG,     // Unable to jump, move speed 75% (Until mended)
    INJURY_HEAD,    // Unable to focus, chance to interrupt (Until mended)
    INJURY_ARM_L,   // Unable to course left (Until mended)
    INJURY_ARM_R,   // Unable to course right (Until mended)
    SUFFOCATION,    // Drown when full
    BREATH,         // Unable to suffocate
    AMPLIFY,        // Magic effects amplified
    SYPHON,         // Slow fatigue gain
    FURRED,         // Extreme cold tolerance
    FINNED,         // Extreme swim speed
    GILLED,         // Breath underwater
    UNCONSCIOUS,    // Knocked out (15m, Until rescued)
    DEAD,           // Unable to act (Indefinite)
}

public class Vessel : MonoBehaviour {
    
    private GameObject _body;

    // Vectors
    private Vector3 _saveLoc;
    private Vector3 _boundLoc;
    //private ZONE _bindZone;
    
    private SPECIES _species;   // Classification (Nautilus, fish, bird, skeleton, cat, snail, robot, tree, minotaur, centaur, inanimate)
    private SPECIES _illusion;  // Projected species
    
    private int _hp;            // Hits allowed before falling unconscious
    private int _shield;        // Recharging magic shield, taken before health and armor
    private int _armor;         // Damage reduction (50%), repaired outside of battle
    private int _focus;         // Ability to perform actions without interruption
    private int _stress;        // Effect of battle, strenuous activity, and extreme pushes. Cleared over time
    private int _energy;        // Required for actions & coursing 
    private int _fatigue;       // Gained from running, action use, etc. Cleared over time
    private int _temp;          // Thermal measurement from optimal (-100, +100), capable of ascending and descending based on environment and internal factors 
    private int _tempLo;        // Thermal lower bounds
    private int _tempHi;        // Thermal upper bounds
    private int _noise;         // Sound emitted, used for detection
    private int _exhaustion;    // Slow build-up over time, cleared by sleep
    private int _thirst;        // Required liquid, essence, oils, etc.
    private int _hunger;        // Required sustenance, nutrients
    private int _urgency;       // Required waste purging
    

    // *** RULES ***
    // HP recovery NOT innate
    // Energy recovery innate
    // Potions - SOW Landmark or POE Flasks (left bottle, right bottle)
    // Use for health, fatigue recovery, status cleanse, speed boost. Rechargeable at intervals (Estus, OW Medkit). NOT stored consumables, enduring dosage

    // Food?
    // Drink?
    // Reagents
    // Factions? Should belong to them or no?
    // New relationship (Eagles Hunt Frogs, Frogs Hunt Flies, Frogs Fear Eagles)
    // Eagle.addPrey(Frog) {pFrog.addPredator(this.species)}
    // Giant.addPrey(All)
    // Orc.addEnemy(Halflings)
    // Orc.
    // Gifts (care, time, physical, touch, etc.) - Favor
    private Dictionary<STATUS, int> _status; // Changes to the base vessel via effects, world influence, food, etc. (ID, Intensity)
    private Dictionary<string, int> _regard; // Personal regards (Outweigh other considerations)
    private Dictionary<string, int> _tastes; // Individual taste tags (Negative dislike, positive like). Shiny, Knowledge, Sweet, Sour, Spicy, Meat, Alcohol, Soft, Colorful, Technology

    private float _speedMoveAt; // Current movement speed
    private float _speedMovePace;
    private float _speedMoveChase;
    private float _speedMoveFlee;

    private float _speedCourse;
    
    private Vector3 _spawnLoc;
    private Vector3 _targetLoc;
    private GameObject _target;

    private Dictionary<string, int> _aggro; // ID, Intensity

    // Use this for initialization
    void Start () {
        STATUS tStatus;
        
        tStatus = STATUS.AMPLIFY;
        if(tStatus != null)
        {

        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
