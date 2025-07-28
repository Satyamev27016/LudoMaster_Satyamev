

using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class RollingDice : MonoBehaviour
{
    [SerializeField] Sprite[] numberSprite;
    [SerializeField] SpriteRenderer numberSpriteHolder;
    [SerializeField] SpriteRenderer rollingDiceAnimation;
    [SerializeField] int numberGot;
    private AudioSource audioSource;

    Coroutine generateRandamNumberDice;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnMouseDown()
    {
       
        if ((GameManager.gm.rollingDice == this || GameManager.gm.rollingDice == null) && GameManager.gm.canDiceRoll)
        {
            generateRandamNumberDice = StartCoroutine(rollingDice());
        }
    }

    IEnumerator rollingDice()
    {
        yield return new WaitForEndOfFrame();

        // Disable rolling immediately as the roll has started.
        GameManager.gm.canDiceRoll = false;

        if (audioSource != null)
        {
            audioSource.Play();
        }

        // Original code for animation and number generation
        numberSpriteHolder.gameObject.SetActive(false);
        rollingDiceAnimation.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);

        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        numberGot = Random.Range(0, 6); 
        numberSpriteHolder.sprite = numberSprite[numberGot];
        numberGot++; 


        GameManager.gm.numberOfStepsToMove = numberGot;
        GameManager.gm.rollingDice = this; 


        numberSpriteHolder.gameObject.SetActive(true);
        rollingDiceAnimation.gameObject.SetActive(false);

     

        GameManager.gm.canPlayerMove = true;

        
        if (GameManager.gm.numberOfStepsToMove != 6 && GetCurrentPlayersPiecesInBase() == 4)
        {
            yield return new WaitForSeconds(0.2f);
            GameManager.gm.canDiceTransfer = true; 
            GameManager.gm.rollingDiceTransfer(); 
        }

    }

    
    public int GetCurrentPlayersPiecesInBase()
    {
       
        if (GameManager.gm.rollingDice == GameManager.gm.rollingDices[0]) 
        {
            return 4 - GameManager.gm.blueOutPlayer; 
        }
        else if (GameManager.gm.rollingDice == GameManager.gm.rollingDices[1]) 
        {
            return 4 - GameManager.gm.redOutPlayer;
        }
        else if (GameManager.gm.rollingDice == GameManager.gm.rollingDices[2]) 
        {
            return 4 - GameManager.gm.greenOutPlayer;
        }
        else 
        {
            return 4 - GameManager.gm.yellowOutPlayer;
        }
    }
}
