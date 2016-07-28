using UnityEngine;
using System.Collections;

public class Sanity : MonoBehaviour {

    public int ascentions = -1;
    public float tileGrowthRate = 3.0f;
    public float minSlothRatio = 0.80f;

    private EmotionProcessor epu;
    private RoomGenergreater rg;
    private FloorMaker fm;
    private Corrupt c;
    private Grandpa g;

    // Use this for initialization
    void Start() {
        epu = GetComponentInChildren<EmotionProcessor>();
        rg = FindObjectOfType<RoomGenergreater>();
        c = Camera.main.GetComponent<Corrupt>();
        g = GetComponent<Grandpa>();
    }

    /*
    7 deadly sins 4 u
    
    LUST
    * cause:
        * having a high amount of Love_Belonging but nothing supporting it
        * talking only to dateNPC makes 
    * effect:
        * more date NPCs
        * more weddings
        * more people in beds

    GLUTTONY
    * cause:
        * having 2x more of a founding stat in the maslow heirarchy than is necessary
        * looking at corndogs
        * looking at chefs
        * looking at tombstone (pizza)
    * effect:
        * chef npcs everywhere
    
    GREED
    * cause:
        * looking at a lot of things
        * looking at the same things?
    * effect:
        * puts more valuable items into the pool (lamps, boats, wheelchairs)
    
    SLOTH
    * cause:
        * not looking at anything
        * standing still for too long
    * effect:
        * levels get smaller
        * levels have lower chance of spawning all props
        * character gets slower
        * empty beds
    
    WRAITH (mad)
    * cause:
        * having more 'mad' than any other emotion
        * bumping into props too frequently
        * walking into walls
    * effect:
        * the fog becomes red
    
    ENVY
    * cause:
        * talking to date NPCs
        * talking to wedding NPCs
    * effect:
        * fewer exits
    
    PRIDE
    * cause:
        * having large quantities of 'powerful' emotions
    * effect:
        * more television related props appearing
        * more pictures appear
        * more babies appear
    
    UNUSED EFFECTS
    * duplicate items in the spawn pool
    * crying grandpa
    
    UNUSED FILTERS
    * if it causes an emotion, it should spawn more of an opposite
        * draw from 3 pools at once
        * too much in one feeds into the opposite


    */
    // Process all our emotions and play with room generator settings.
    void OnLevelWasLoaded() {
        // Increase the number of floors we've traversed.
        ascentions++;

        God.main.Log("LOOKED AT: " + epu.thingsSeen + "/" + rg.spawnedFurniture + "(" + (epu.thingsSeen / rg.spawnedFurniture) + ")");

        if( ascentions > 0 ){
            // We got to level 2.

            // Get ahold of the FloorMakers
            fm = FindObjectOfType<FloorMaker>();
            if( fm ) {
                fm.tilesLeft += Mathf.Pow(ascentions, tileGrowthRate);
                var ratioOfMovement = (g.timeNotMoving / g.timeInControl);
                if( ratioOfMovement <= minSlothRatio) {
                    fm.tilesLeft *= ratioOfMovement;
                }
            } else {
                God.main.LogWarning("NO FUCKIN' FLOOR MAKER WHAT THE SHIT");
            }

            // == EMOTION PROCESSOR FACTORS ==
            // maslowScore (the raw number of points put toward a maslow need)
            // emotionScore (the raw number of points put toward an emotion)
            // thingsSeen (the raw number of objects observed)

            // == SANITY FACTORS ==
            // ascentions (the number of doors the player has passed through)

            // == TILE SPAWNER FACTORS ==
            // tilesLeft (how many more units the tile spawner has to 
            // chanceToTurn[Left/Around/Right] (whether we'll turn a direction after each write)
            // chanceToClone (chance of creating a second floormaker on each write -- cloning splits available tiles and makes areas more complex/dense)
            // chanceToCloneDecay (how much to decay the chanceToClone after each clone is successfully made)
            // chanceOfBlackRoom (probability of placing a DarkRoom when the FloorMaker does a 180)

            // == ROOM GENERATOR VARIABLES ==
            // minDarkRooms (number of potential exits)
            // minDarkRoomRadius (no exits can exist before this distance)
            // maxDarkRoomRadius (no exits can exist beyond this distance)
            // chanceOf[Photo/WallFloor/Floor/Oversize] (0-1 prop per anchor when decorating)
            // decoration[Photos/Floor/Oversize] (list of props to potentially spawn in each position)
            // crazyDecoration[Photos/Floor/Oversize] (list of props to populate regular spawnlist with due to sanity)

            // == VISUAL CORRUPTION ==
            // corruptFactor (increase to make corruptions more frequent)
            // corruptTime (increase for longer spells of corruption)
            // # these basically undo each-other
        }
    }

    // == EMOTION PROCESSING METHODS ==
    void EvaluateMaslowNeeds() {
        /*
        Having SAFETY without PHYSIOLOGICAL needs implies:
                 *  COWARDACE. fear for life despite not having what is needed to live.
        Having LOVE BELONGING without SAFETY implies:
                 *    ???
                 
        Having Love Belonging without Safety implies: Insecure, Embarrassed, Helpless
        Sad > Ashamed
        a lotof those would probably fit in that
        lemme think while i poop
        i couldnt think while i pooped but im here now
        Self-actualization without esteem implies: Depressed > Miserable

        i wasn't trying to put them into emotional terms directly but it doesn't hurt
        I was thinking more like self actualization without esteem is like leading an empty life chasing hobbies half-heartedly

        Esteem without Love/beloning implies : Angry > Frustrated , Hurt > Jealous, Hostile > Selfish

        I think anything without its founding constituates describes a fleeting interaction

        Esteem without Love implies : Respected by many but no one that really loves you. You have achieved greatness in your life but squandered any hopes of finding love.
        Self-actualization without esteem : Ran only by facts without feelings, you are basically a robot. Dwelling deep in your writings, and a nose in books, life has passed you by.
        Love/belonging without safety: You have people that love you but refuse to accept anyone's kindness  and refuse to work on yourself.
        Safety without physiological: you forgot to poop. Good job.
        you have to have physiological or you die
        so that one has to just be there
        or
        You locked yourself away in your own home, only to slowly perish of starvation and dehydration

        well actually nothing provides physiological so i might have to skip that tier
        or you'd have to stare at one niche prop 7x more than any other prop to keep things stable.
        */
    }

    // MATH HELPERS
    public float IncreasePercent() {
        return 0.0f;
    }
}