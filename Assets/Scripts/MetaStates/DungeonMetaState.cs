using UnityEngine;
using System.Collections;

// This is the main metastate. Handles navigation through the maze.

[AddComponentMenu("Scripts/SPGMetaStates/DungeonMetaState")]
public class DungeonMetaState : MonoBehaviour
{
    public MetaStateManager MetaStateManager;
    public DungeonGenerator DungeonGenerator;
    public bool GoToNextFloorWhenExitIsTouched = false;

	void Start()
    {        
	}
	
	void Update()
    {
        if (MetaStateManager.GetCurrentMetaState() != MetaState.Dungeon)
        {
            return;
        }

        if (Input.GetKeyUp(KeyCode.H))
        {
            MetaStateManager.SetNewMetaState(MetaState.HomeBase);
        }
        else if (Input.GetKeyUp(KeyCode.I))
        {
            MetaStateManager.SetNewMetaState(MetaState.Inventory);
        }
        else if (Input.GetKeyUp(KeyCode.Space) && !GoToNextFloorWhenExitIsTouched && DungeonGenerator.IsHeroOnStairsDown())
        {
            MetaStateManager.SetNewMetaState(MetaState.Story);
        }

        HandleHeroMovementInput();
	    HandleCheatInput();
    }

    void HandleCheatInput()
    {
		// Don't tell anyone this secret key or they'll be able to cheat at the game.
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            DungeonGenerator.GoToNextLevel();
        }
    }

    void HandleHeroMovementInput()
    {
        bool moveSuccess = false;

        if (Input.GetKeyUp(KeyCode.W))
        {
            DungeonGenerator.MoveHero(Direction.North);
            moveSuccess = true;
        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            DungeonGenerator.MoveHero(Direction.West);
            moveSuccess = true;
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            DungeonGenerator.MoveHero(Direction.South);
            moveSuccess = true;
        }
        else if (Input.GetKeyUp(KeyCode.D))
        {
            DungeonGenerator.MoveHero(Direction.East);
            moveSuccess = true;
        }

        if (moveSuccess && GoToNextFloorWhenExitIsTouched && DungeonGenerator.IsHeroOnStairsDown())
        {
            MetaStateManager.SetNewMetaState(MetaState.Story);
            return;
        }

		// Random chance to get in a battle as the Hero navigates around the Dungeon.
        if (moveSuccess && Random.Range(0, 10) < 1)
        {
            MetaStateManager.SetNewMetaState(MetaState.Battle);
        }
    }

    void OnGUI()
    {
        if (MetaStateManager.GetCurrentMetaState() != MetaState.Dungeon)
        {
            return;
        }

		// Display some information on how to play the game.
        GUI.Box(new Rect(25, 25, 150, 25), "Dungeon Floor: " + DungeonGenerator.DungeonFloorNumber);
        GUI.Label(new Rect(25, 55, 250, 25), "WASD = move");        
        GUI.Label(new Rect(25, 75, 250, 25), "Z = zoom in/out");
        GUI.Label(new Rect(25, 95, 250, 25), "H = back to home base");
        GUI.Label(new Rect(25, 115, 250, 25), "I = open inventory");
        if (!GoToNextFloorWhenExitIsTouched)
        {
            GUI.Label(new Rect(25, 135, 250, 25), "Spacebar on red = next floor");
        }        
    }
}
