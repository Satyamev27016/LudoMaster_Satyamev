

//using UnityEngine;
//using System.Collections.Generic;
//using System.Collections;

//public class GameManager : MonoBehaviour
//{
//    public static GameManager gm;
//    public int numberOfStepsToMove;
//    public RollingDice rollingDice;
//    public bool canPlayerMove = true;
//    public bool canDiceRoll = true;
//    public bool canDiceTransfer = false;
//    public int blueOutPlayer;
//    public int redOutPlayer;
//    public int greenOutPlayer;
//    public int yellowOutPlayer;
//    public int blueCompletePlayer;
//    public int redCompletePlayer;
//    public int greenCompletePlayer;
//    public int yellowCompletePlayer;
//    public GameObject celibration;
//    List<PathPoint> playerPathPointList = new List<PathPoint>();
//    public List<RollingDice> rollingDices;
//    [Header("Player Rings")]
//    public RingUVRotate blueRing;


//    public void ActivateRingForCurrentPlayer()
//    {
//        if (blueRing == null)
//        {
//            Debug.LogError("BlueRing is not assigned in GameManager!");
//            return;
//        }
//        blueRing.ActivateRing(true);  // Only Blue Ring
//    }


//    private void Awake()
//    {
//        gm = this;
//        blueOutPlayer = 0;
//        redOutPlayer = 0;
//        greenOutPlayer = 0;
//        yellowOutPlayer = 0;
//        blueCompletePlayer = 0;
//        redCompletePlayer = 0;
//        greenCompletePlayer = 0;
//        yellowCompletePlayer = 0;
//        canPlayerMove = false; 
//        canDiceRoll = true;    

//        if (rollingDices != null && rollingDices.Count > 0)
//        {
//            for (int i = 0; i < rollingDices.Count; i++)
//            {
//                rollingDices[i].gameObject.SetActive(false);
//            }
//            rollingDices[0].gameObject.SetActive(true);
//            rollingDice = rollingDices[0];
//            }
//        ActivateRingForCurrentPlayer();
//    }

//    public void AddPathPoint(PathPoint pathPoint)
//    {
//        playerPathPointList.Add(pathPoint);
//    }

//    public void RemovePathPoint(PathPoint pathPoint)
//    {
//        if (playerPathPointList.Contains(pathPoint))
//        {
//            playerPathPointList.Remove(pathPoint);
//        }
//    }

//    public void rollingDiceTransfer()
//    {
//        int currentDiceIndex = rollingDices.IndexOf(rollingDice);
//        int nextDiceIndex = (currentDiceIndex + 1) % rollingDices.Count;
//        rollingDices[currentDiceIndex].gameObject.SetActive(false);
//        rollingDices[nextDiceIndex].gameObject.SetActive(true);
//        rollingDice = rollingDices[nextDiceIndex];
//        canDiceRoll = true;      
//        canPlayerMove = false;   
//        numberOfStepsToMove = 0; 
//        canDiceTransfer = false;
//        ActivateRingForCurrentPlayer();

//    }






//    public void HandlePlayerMoveComplete(bool wasKnockOrCompletion)
//    {

//        int stepsRolledThisTurn = numberOfStepsToMove;


//        if (stepsRolledThisTurn == 6 || !wasKnockOrCompletion)
//        {
//            // Player gets another turn - don't transfer dice
//            numberOfStepsToMove = 0; 
//            canDiceRoll = true;
//            canPlayerMove = false; 

//            Debug.Log("Player gets another turn! (6 rolled: " + (stepsRolledThisTurn == 6) + ", Special event: " + (!wasKnockOrCompletion) + ")");

//        }
//        else
//        {
//            // Normal move completed - transfer the turn to the next player.
//            numberOfStepsToMove = 0; 
//            rollingDiceTransfer();
//            Debug.Log("Turn transferred to next player");
//        }
//    }
//}



using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager gm;
    public int numberOfStepsToMove;
    public RollingDice rollingDice;
    public bool canPlayerMove = true;
    public bool canDiceRoll = true;
    public bool canDiceTransfer = false;
    public int blueOutPlayer;
    public int redOutPlayer;
    public int greenOutPlayer;
    public int yellowOutPlayer;
    public int blueCompletePlayer;
    public int redCompletePlayer;
    public int greenCompletePlayer;
    public int yellowCompletePlayer;
    public GameObject celibration;
    List<PathPoint> playerPathPointList = new List<PathPoint>();
    public List<RollingDice> rollingDices;

    [Header("Player Rings")]
    public RingUVRotate blueRing;

    [Header("Blue Player")]
    public Transform bluePlayer2a;

    private bool isBluePlayerTurn = true;

    private void Awake()
    {
        gm = this;
        blueOutPlayer = 0;
        redOutPlayer = 0;
        greenOutPlayer = 0;
        yellowOutPlayer = 0;
        blueCompletePlayer = 0;
        redCompletePlayer = 0;
        greenCompletePlayer = 0;
        yellowCompletePlayer = 0;
        canPlayerMove = false;
        canDiceRoll = true;

        if (rollingDices != null && rollingDices.Count > 0)
        {
            for (int i = 0; i < rollingDices.Count; i++)
            {
                rollingDices[i].gameObject.SetActive(false);
            }
            rollingDices[0].gameObject.SetActive(true);
            rollingDice = rollingDices[0];
        }

        ActivateRingForCurrentPlayer();
    }

    public void ActivateRingForCurrentPlayer()
    {
        if (blueRing == null)
        {
            Debug.LogError("BlueRing is not assigned in GameManager!");
            return;
        }

        if (bluePlayer2a == null)
        {
            Debug.LogError("BluePlayer2a is not assigned in GameManager!");
            return;
        }

        if (isBluePlayerTurn)
        {
            blueRing.SetPlayer(bluePlayer2a);
            blueRing.ActivateRing(true);
        }
        else
        {
            blueRing.ActivateRing(false);
        }
    }

    public void AddPathPoint(PathPoint pathPoint)
    {
        playerPathPointList.Add(pathPoint);
    }

    public void RemovePathPoint(PathPoint pathPoint)
    {
        if (playerPathPointList.Contains(pathPoint))
        {
            playerPathPointList.Remove(pathPoint);
        }
    }

    public void rollingDiceTransfer()
    {
        int currentDiceIndex = rollingDices.IndexOf(rollingDice);
        int nextDiceIndex = (currentDiceIndex + 1) % rollingDices.Count;
        rollingDices[currentDiceIndex].gameObject.SetActive(false);
        rollingDices[nextDiceIndex].gameObject.SetActive(true);
        rollingDice = rollingDices[nextDiceIndex];

        isBluePlayerTurn = (nextDiceIndex == 0);

        canDiceRoll = true;
        canPlayerMove = false;
        numberOfStepsToMove = 0;
        canDiceTransfer = false;

        ActivateRingForCurrentPlayer();
    }

    public void HandlePlayerMoveComplete(bool wasKnockOrCompletion)
    {
        int stepsRolledThisTurn = numberOfStepsToMove;

        if (stepsRolledThisTurn == 6 || !wasKnockOrCompletion)
        {
            numberOfStepsToMove = 0;
            canDiceRoll = true;
            canPlayerMove = false;
            Debug.Log("Player gets another turn! (6 rolled: " + (stepsRolledThisTurn == 6) + ", Special event: " + (!wasKnockOrCompletion) + ")");
        }
        else
        {
            numberOfStepsToMove = 0;
            rollingDiceTransfer();
            Debug.Log("Turn transferred to next player");
        }
    }
}