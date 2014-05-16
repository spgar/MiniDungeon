using UnityEngine;
using System.Collections;

public enum MetaState
{
    None,
    MainMenu,
    HomeBase,
    Dungeon,
    Battle,    
    Inventory,
    Story
}

public class MetaStateManager : MonoBehaviour
{
    private MetaState currentMetaState;
    private MetaState pendingNewMetaState;

    public MetaState GetCurrentMetaState()
    {
        return currentMetaState;
    }

    public void SetNewMetaState(MetaState newMetaState)
    {
        pendingNewMetaState = newMetaState;
    }

	void Start()
	{
        pendingNewMetaState = MetaState.None;
        currentMetaState = MetaState.MainMenu;	    
	}
	
	void Update()
    {
        if (pendingNewMetaState != MetaState.None)
        {
            currentMetaState = pendingNewMetaState;
            pendingNewMetaState = MetaState.None;
        }
	}
}
