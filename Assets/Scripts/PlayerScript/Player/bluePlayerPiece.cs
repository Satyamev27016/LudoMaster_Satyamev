//// blueplayer

//using UnityEngine;
//using System.Collections;

//public class bluePlayerPiece : PlayerPiece
//{
//    void Start()
//    {
//      //  blueRollingDice = GetComponentInParent<Bluehome>().rollingDice;
//    }

//    public void OnMouseDown()
//    {
//        if (GameManager.gm.rollingDice != null)
//        {
//            if (!isReady)
//            {
//               // if (GameManager.gm.rollingDice == blueRollingDice && GameManager.gm.numberOfStepsToMove == 6 && GameManager.gm.canPlayerMove)
//                //{

//                    MakePlayerReadyToMove(pathParent.BluePath);
//                    GameManager.gm.blueOutPlayer += 1;
//                    return;
//                //}
//            }

//           // if (GameManager.gm.rollingDice == blueRollingDice && isReady && GameManager.gm.canPlayerMove)
//            //{
//                GameManager.gm.canPlayerMove = false;
//                MovePlayer(pathParent.BluePath);

//            //}
//        }

//    }


//}

using UnityEngine;
using System.Collections;

public class bluePlayerPiece : PlayerPiece
{
    public void OnMouseDown()
    {
        if (GameManager.gm.rollingDice != null && GameManager.gm.rollingDice == myRollingDice && GameManager.gm.canPlayerMove)
        {
            if (!isReady) 
            {
                
                if (GameManager.gm.numberOfStepsToMove == 6)
                {
                    GameManager.gm.canPlayerMove = false; 
                    MakePlayerReadyToMove(pathParent.BluePath);
                    GameManager.gm.blueOutPlayer += 1;
                    
                }
            }
            else 
            {
                
                if (GameManager.gm.numberOfStepsToMove > 0)
                {
                    GameManager.gm.canPlayerMove = false; 
                    MovePlayer(pathParent.BluePath);
                    
                }
            }
        }
    }
}