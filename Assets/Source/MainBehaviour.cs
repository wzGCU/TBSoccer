using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MainBehaviour : MonoBehaviour
{
    private int xPos = 0, yPos = 0;

    public int wongames, lostgames;

    private bool isGameFinished = false;
    private bool playerMove = true;
	public bool startedPlayer = true;

    private Vector3[] moves = new Vector3[500];
    private int movesCounter = 1;

    private int newPosX, newPosY;

    public LineRenderer LineRenderer;
    public UIHandle CanvasHandler;
    public Transform BallPosition;

    // Start is called before the first frame update
    void Start()
    {
		/*
		 * Set the line renderer to put the lines on moves, set the initial position to 0,0)
		 */
        LineRenderer.positionCount = 1;
        moves[0] = new Vector3(xPos, yPos, -5.0f);

        for(int i = 1; i<moves.Length; i++)
        {
            moves[i] = new Vector3(100f, 100f, 1.0f); //Setting a high variable to other elements so can check in future if the position is taken
        }

		/*
		 * Load data if the game has it
		 */

		if (PlayerPrefs.HasKey("wonGames"))
		{
			wongames = PlayerPrefs.GetInt("wonGames");
			lostgames = PlayerPrefs.GetInt("lostGames");
			startedPlayer = PlayerPrefs.GetInt("whoStarted") == 1 ? true : false;
		}
        else
        {
			wongames = 0;
			lostgames = 0;
			startedPlayer = true;
        }

		/*
		 * If last round was started by player, next round is started by AI
		 */
		if (startedPlayer)
        {
			playerMove = true;

        }
        else
        {
			playerMove = false;
        }
	}

    
    void Update()
    {
        MousePosition();
        if (!isGameFinished){
            if (playerMove)
            {
				if (Input.GetMouseButtonDown(0) && canMove())
				{
					MoveBall(newPosX, newPosY);
				}
			}
            else
            {
				MoveBallAI();
            }
			
        }
    }

	/*
	 * Updates the mouse position on board and convert it to new move
	 */
	void MousePosition()
	{
		RaycastHit mouseray;
		if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out mouseray, 25.0f, LayerMask.GetMask("BallArena")))
		{
			newPosX = (int)Decimal.Round((decimal)mouseray.point.x);
			newPosY = (int)Decimal.Round((decimal)mouseray.point.y);
		}
	}

	/*
	 * Check if the new move can be done
	 */
	bool canMove()
    {
		/*
		 * Check if the move was done before
		 */
		for (int i = 0; i < movesCounter; i++)
		{
			if (moves[i].x == xPos && moves[i].y == yPos)
			{
				if (i > 0 && (moves[i - 1].x == newPosX && moves[i - 1].y == newPosY)) 
				{
					return false;
				}
			}
		}

		/*
		 * Check if its exact position
		 */
		if (newPosX == xPos && newPosY == yPos)
		{
			return false;
		}

		/*
		 *  Check if player tried to do a 2-line move for X and Y axis
		 */
		else if ((newPosX < xPos - 1) || (newPosX > xPos + 1) ||
				((newPosY < yPos - 1) || (newPosY > yPos + 1)))
		{
			return false;
		}

		/*
		 * Check if tried to put on either left or right border 
		 */
		else if (((xPos == -4 && newPosX == -4) && (yPos != newPosY)) ||
				((xPos == 4 && newPosX == 4) && (yPos != newPosY)))
		{
			return false;
		}

		/*
		 * Check if put on top or bottom border if not goal
		 */
		else if (((yPos == -5 && newPosY == -5) && (xPos != newPosX)) ||
		((yPos == 5 && newPosY == 5) && (xPos != newPosX)))
		{
			return false;
		}

		/*
		 *  Check if tried to put on outside the arena
		 */
		else if ((newPosX < -4 || newPosX > 4) || 
				 (newPosY < -5 && newPosX != 0) ||
				 (newPosY > 5 && newPosX != 0))
		{
			return false;
		}

		/*
		 * If none of these have been comitted then player can move
		 */
		else
		{
			return true;
		}
	}


	/*
	 * Function that puts the ball to the new position, renders the line of the move and does the checks
	 */
	void MoveBall(int newBallX, int newBallY)
    {
		xPos = newBallX;
		yPos = newBallY;
		moves[movesCounter] = new Vector3(xPos, yPos, -5.0f);
		LineRenderer.positionCount++;
		movesCounter++;
		LineRenderer.SetPositions(moves);
		CheckIfFinished();
		CheckIfCanAgain();
		BallPosition.position = new Vector3(xPos, yPos, -8.0f);
    }

	/*
	 * Function of how the AI tries to move differently based on the X-position (it will try to get to middle first)
	 * I know it is a bit long, and I can see how I could improve it in the future
	 */
	void MoveBallAI()
    {
		if(xPos <0)
        {
			newPosX = xPos + 1; 
			newPosY = yPos + 1;
			if (canMove())
            {
				MoveBall(newPosX, newPosY);
				return;
            }

			newPosX = xPos;
			newPosY = yPos + 1;
            if (canMove())
            {
				MoveBall(newPosX, newPosY);
				return;
            }

			newPosX = xPos - 1;
			newPosY = yPos + 1;
			if (canMove())
			{
				MoveBall(newPosX, newPosY);
				return;
			}

			newPosX = xPos - 1;
			newPosY = yPos;
			if (canMove())
			{
				MoveBall(newPosX, newPosY);
				return;
			}

			newPosX = xPos + 1;
			newPosY = yPos;
			if (canMove())
			{
				MoveBall(newPosX, newPosY);
				return;
			}

			newPosX = xPos + 1;
			newPosY = yPos - 1;
			if (canMove())
			{
				MoveBall(newPosX, newPosY);
				return;
			}

			newPosX = xPos - 1;
			newPosY = yPos - 1;
			if (canMove())
			{
				MoveBall(newPosX, newPosY);
				return;
			}

			newPosX = xPos;
			newPosY = yPos - 1;
			if (canMove())
			{
				MoveBall(newPosX, newPosY);
				return;
			}
		}

		else if (xPos > 0)
        {
			newPosX = xPos - 1;
			newPosY = yPos + 1;
			if (canMove())
			{
				MoveBall(newPosX, newPosY);
				return;
			}

			newPosX = xPos + 1;
			newPosY = yPos + 1;
			if (canMove())
			{
				MoveBall(newPosX, newPosY);
				return;
			}

			newPosX = xPos;
			newPosY = yPos + 1;
			if (canMove())
			{
				MoveBall(newPosX, newPosY);
				return;
			}

			newPosX = xPos - 1;
			newPosY = yPos;
			if (canMove())
			{
				MoveBall(newPosX, newPosY);
				return;
			}

			newPosX = xPos + 1;
			newPosY = yPos;
			if (canMove())
			{
				MoveBall(newPosX, newPosY);
				return;
			}

			newPosX = xPos + 1;
			newPosY = yPos - 1;
			if (canMove())
			{
				MoveBall(newPosX, newPosY);
				return;
			}

			newPosX = xPos;
			newPosY = yPos - 1;
			if (canMove())
			{
				MoveBall(newPosX, newPosY);
				return;
			}

			newPosX = xPos - 1;
			newPosY = yPos - 1;
			if (canMove())
			{
				MoveBall(newPosX, newPosY);
				return;
			}
		}
        else
        {
			newPosX = xPos;
			newPosY = yPos + 1;
			if (canMove())
			{
				MoveBall(newPosX, newPosY);
				return;
			}

			newPosX = xPos + 1;
			newPosY = yPos + 1;
			if (canMove())
			{
				MoveBall(newPosX, newPosY);
				return;
			}

			newPosX = xPos - 1;
			newPosY = yPos + 1;
			if (canMove())
			{
				MoveBall(newPosX, newPosY);
				return;
			}

			newPosX = xPos - 1;
			newPosY = yPos;
			if (canMove())
			{
				MoveBall(newPosX, newPosY);
				return;
			}
			newPosX = xPos + 1;
			newPosY = yPos;
			if (canMove())
			{
				MoveBall(newPosX, newPosY);
				return;
			}

			newPosX = xPos + 1;
			newPosY = yPos - 1;
			if (canMove())
			{
				MoveBall(newPosX, newPosY);
				return;
			}

			newPosX = xPos;
			newPosY = yPos - 1;
			if (canMove())
			{
				MoveBall(newPosX, newPosY);
				return;
			}

			newPosX = xPos - 1;
			newPosY = yPos - 1;
			if (canMove())
			{
				MoveBall(newPosX, newPosY);
				return;
			}
		}
    }

	/*
	 * Check if the ball can move again, for example when it is on the boundary or when it goes thru previous position
	 */
	void CheckIfCanAgain()
    {
		for (int i = 0; i < movesCounter - 2; i++) // check previous ball spots
		{
			if (moves[i].x == xPos && moves[i].y == yPos)
			{
				return;
			}
		}

		/*
		 * Check for left and right boundaries
		 */
		if (xPos == -4 || xPos == 4)
		{
			return;
		}

		/*
		 * Check for upper and lower boundary
		 */
		if ((yPos == -5 || yPos == 5) && (xPos != 0))
		{
			return;
		}
		playerMove = !playerMove;
	}

	void GeneratePossibleBalls()
    {

    }

	/*
	 * Check if the ball is at either goal
	 */
	void CheckIfFinished()
    {
		if (yPos < -5)
		{
			// Game is won by the player
			CanvasHandler.UIGameFinished(true);
			isGameFinished = true;
		}
		else if (yPos > 5)
		{
			//Game is lost for the player
			CanvasHandler.UIGameFinished(false);
			isGameFinished = true;
		}
	}

	/*
	 * Function that gets called from the UIHandle class to finish the game if its caused by a button.
	 */
	public void SetGameFinished()
    {
		isGameFinished = true;
    }
}
