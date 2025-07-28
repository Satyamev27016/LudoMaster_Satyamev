

using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PathPoint : MonoBehaviour
{
    public PathsObjectParent pathObjectParent;
    public List<PlayerPiece> playerPieceList = new List<PlayerPiece>();
    public PathPoint[] pathPointsToMoveOn;
    Coroutine playerMovementCoroutineReference;
    Coroutine celebrationCoroutineReference; 

    private void Start()
    {
        pathObjectParent = GetComponentInParent<PathsObjectParent>();
    }

    public bool AddPlayerPiece(PlayerPiece playerpiece)
    {
        if (this.name == "PathPoint (53)") 
        {
            AddPlayer(playerpiece);
            completed(playerpiece);
            return false; 
        }

        if (!pathObjectParent.safePoints.Contains(this))
        {
            if (playerPieceList.Count == 1)
            {
                string prePlayerPieceName = playerPieceList[0].name;
                string currentPlayerPieceName = playerpiece.name;
                currentPlayerPieceName = currentPlayerPieceName.Substring(0, currentPlayerPieceName.Length - 4);

                if (!prePlayerPieceName.Contains(currentPlayerPieceName))
                {
                    playerPieceList[0].isReady = false;
                    playerMovementCoroutineReference = StartCoroutine(revertOnStart(playerPieceList[0]));
                    playerPieceList[0].numberOfStepsAlreadyMove = 0;
                    RemovePlayerPiece(playerPieceList[0]);
                    playerPieceList.Add(playerpiece);
                    return false; 
                }
            }
        }

        AddPlayer(playerpiece);
        return true; 
    }

    IEnumerator revertOnStart(PlayerPiece playerpiece)
    {
        if (playerpiece.name.Contains("blue"))
        {
            GameManager.gm.blueOutPlayer -= 1;
            pathPointsToMoveOn = pathObjectParent.BluePath;
        }
        else if (playerpiece.name.Contains("red"))
        {
            GameManager.gm.redOutPlayer -= 1;
            pathPointsToMoveOn = pathObjectParent.RedPath;
        }
        else if (playerpiece.name.Contains("green"))
        {
            GameManager.gm.greenOutPlayer -= 1;
            pathPointsToMoveOn = pathObjectParent.GreenPath;
        }
        else if (playerpiece.name.Contains("yellow"))
        {
            GameManager.gm.yellowOutPlayer -= 1;
            pathPointsToMoveOn = pathObjectParent.YellowPath;
        }
        for (int i = playerpiece.numberOfStepsAlreadyMove - 1; i >= 0; i--) 
        {
            playerpiece.transform.position = pathPointsToMoveOn[i].transform.position;
            yield return new WaitForSeconds(0.05f);
        }
        playerpiece.transform.position = pathObjectParent.BasePoint[basePointPosition(playerpiece.name)].transform.position;

        if (playerMovementCoroutineReference != null)
        {
            StopCoroutine(playerMovementCoroutineReference); 
            playerMovementCoroutineReference = null; 
        }
    }

    void completed(PlayerPiece playerpiece)
    {
        int totalCompletePlayer = 0; 
        if (playerpiece.name.Contains("blue"))
        {
            GameManager.gm.blueOutPlayer -= 1;
            totalCompletePlayer = GameManager.gm.blueCompletePlayer += 1;
        }
        else if (playerpiece.name.Contains("red"))
        {
            GameManager.gm.redOutPlayer -= 1;
            totalCompletePlayer = GameManager.gm.redCompletePlayer += 1;
        }
        else if (playerpiece.name.Contains("green"))
        {
            GameManager.gm.greenOutPlayer -= 1;
            totalCompletePlayer = GameManager.gm.greenCompletePlayer += 1;
        }
        else if (playerpiece.name.Contains("yellow"))
        {
            GameManager.gm.yellowOutPlayer -= 1;
            totalCompletePlayer = GameManager.gm.yellowCompletePlayer += 1;
        }
        if (totalCompletePlayer == 4)
        {
            celebrationCoroutineReference = StartCoroutine(celebrationWiner());
        }
    }

    IEnumerator celebrationWiner()
    {
        GameManager.gm.celibration.SetActive(true);
        yield return new WaitForSeconds(2f);
        GameManager.gm.celibration.SetActive(false);

        if (celebrationCoroutineReference != null)
        {
            StopCoroutine(celebrationCoroutineReference); 
            celebrationCoroutineReference = null; 
        }
    }

    int basePointPosition(string name)
    {
        for (int i = 0; i < pathObjectParent.BasePoint.Length; i++) 
        {
            if (pathObjectParent.BasePoint[i].name == name)
            {
                return i;
            }
        }
        return -1; 
    }

    public void AddPlayer(PlayerPiece playerpiece)
    {
        playerPieceList.Add(playerpiece);
        rescaleAndRepositioningAllPlayerPieces();
    }

    public void RemovePlayerPiece(PlayerPiece playerpiece)
    {
        if (playerPieceList.Contains(playerpiece))
        {
            playerPieceList.Remove(playerpiece);
            rescaleAndRepositioningAllPlayerPieces();
        }
    }

    public void rescaleAndRepositioningAllPlayerPieces()
    {
        int placeCount = playerPieceList.Count;
        bool isOdd = (placeCount % 2) == 0 ? false : true;

        int extent = placeCount / 2;
      
        int counter = 0;
        int SpriteLayer = 0;

        if (isOdd)
        {
            for (int i = 0; i <= extent; i++)
            {
               
                playerPieceList[counter].transform.localScale = new Vector3(pathObjectParent.scales[placeCount - 1], pathObjectParent.scales[placeCount - 1], 0f);
                playerPieceList[counter].transform.position = new Vector3(transform.position.x + (i * pathObjectParent.positionDifference[placeCount - 1]), transform.position.y, 0f);
            }
        }
        else
        {
            for (int i = 0; i < extent; i++)
            {
                
                playerPieceList[counter].transform.localScale = new Vector3(pathObjectParent.scales[placeCount - 1], pathObjectParent.scales[placeCount - 1], 0f);
                playerPieceList[counter].transform.position = new Vector3(transform.position.x + (i * pathObjectParent.positionDifference[placeCount - 1]), transform.position.y, 0f);
            }
        }

        for (int i = 0; i < extent; i++)
        {
            playerPieceList[i].GetComponentInChildren<SpriteRenderer>().sortingOrder = SpriteLayer;
            SpriteLayer++;
        }
    }
}
