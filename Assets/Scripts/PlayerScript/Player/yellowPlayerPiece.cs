


using UnityEngine;
using System.Collections;

public class yellowPlayerPiece : PlayerPiece
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
                    MakePlayerReadyToMove(pathParent.YellowPath); 
                    GameManager.gm.yellowOutPlayer += 1;          
                }
            }
            else 
            {
                
                if (GameManager.gm.numberOfStepsToMove > 0)
                {
                    GameManager.gm.canPlayerMove = false; 
                    MovePlayer(pathParent.YellowPath);     
                }
            }
        }
    }
}