using UnityEngine;
using System.Collections;

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

        if (Input.GetKeyUp(KeyCode.I))
        {
            MetaStateManager.SetNewMetaState(MetaState.Inventory);
        }

        if (Input.GetKeyUp(KeyCode.Space) && !GoToNextFloorWhenExitIsTouched && DungeonGenerator.IsHeroOnStairsDown())
        {
            MetaStateManager.SetNewMetaState(MetaState.Story);
        }

        HandleHeroMovementInput();
	    HandleCheatInput();
    }

    void HandleCheatInput()
    {
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
        if (Input.GetKeyUp(KeyCode.A))
        {
            DungeonGenerator.MoveHero(Direction.West);
            moveSuccess = true;
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            DungeonGenerator.MoveHero(Direction.South);
            moveSuccess = true;
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            DungeonGenerator.MoveHero(Direction.East);
            moveSuccess = true;
        }

        if (moveSuccess && GoToNextFloorWhenExitIsTouched && DungeonGenerator.IsHeroOnStairsDown())
        {
            MetaStateManager.SetNewMetaState(MetaState.Story);
            return;
        }

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
