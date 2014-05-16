using UnityEngine;
using System.Collections;

public class DungeonGenerator : MonoBehaviour
{
    private enum BuildingBlockType
    {
        None,
        Empty,
        WNES,
        WE,
        NS,
        WN,
        NE,
        ES,
        WS,
        WNE,
        WES,
        NES,
        WNS,
        Wx,
        Nx,
        Ex,
        Sx
    }

    public enum StairsPlacementType
    {
        Random,
        AtEdges
    }

    private class CellLocation
    {
        public CellLocation(int setX, int setZ)
        {
            x = setX;
            z = setZ;
        }

        public int x;
        public int z;

        public override bool Equals(object obj)
        {
            if (obj is CellLocation)
            {
                CellLocation testObj = (CellLocation)obj;
                if (testObj.x == x && testObj.z == z)
                {
                    return true;
                }
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public MetaStateManager MetaStateManager;
    public Hero Hero;

    public Transform PrefabBuildingBlock_Empty;
    public Transform PrefabBuildingBlock_WNES;
    public Transform PrefabBuildingBlock_WE;
    public Transform PrefabBuildingBlock_NS;
    public Transform PrefabBuildingBlock_WN;
    public Transform PrefabBuildingBlock_NE;
    public Transform PrefabBuildingBlock_ES;
    public Transform PrefabBuildingBlock_WS;
    public Transform PrefabBuildingBlock_WNE;
    public Transform PrefabBuildingBlock_WES;
    public Transform PrefabBuildingBlock_NES;
    public Transform PrefabBuildingBlock_WNS;
    public Transform PrefabBuildingBlock_Wx;
    public Transform PrefabBuildingBlock_Nx;
    public Transform PrefabBuildingBlock_Ex;
    public Transform PrefabBuildingBlock_Sx;

    public Transform PrefabStairsUp;
    public Transform PrefabStairsDown;    

    public Material MaterialOverrideStairsUp;
    public Material MaterialOverrideStairsDown;

    public Transform PrefabBuildingBlockParent;
    public Transform OtherStuffParent;
    public int NumBuildingBlocksAcross;
    public int NumBuildingBlocksUp;

    public StairsPlacementType StairsPlacement = StairsPlacementType.Random;

    private float prefabBuildingBlockWidth;
    private float prefabBuildingBlockHeight;

    private CellLocation heroLocation;
    private CellLocation stairsUpLocation;
    private CellLocation stairsDownLocation;

    [HideInInspector]
    public int DungeonFloorNumber = 1;

    private BuildingBlockType[,] buildingBlockTypeGrid;

    Transform GetPrefabFromBuildingBlockType(BuildingBlockType bbType)
    {
        switch (bbType)
        {
            case BuildingBlockType.Empty:
                return PrefabBuildingBlock_Empty;
            case BuildingBlockType.WNES:
                return PrefabBuildingBlock_WNES;
            case BuildingBlockType.WE:
                return PrefabBuildingBlock_WE;
            case BuildingBlockType.NS:
                return PrefabBuildingBlock_NS;
            case BuildingBlockType.WN:
                return PrefabBuildingBlock_WN;
            case BuildingBlockType.NE:
                return PrefabBuildingBlock_NE;
            case BuildingBlockType.ES:
                return PrefabBuildingBlock_ES;
            case BuildingBlockType.WS:
                return PrefabBuildingBlock_WS;
            case BuildingBlockType.WNE:
                return PrefabBuildingBlock_WNE;
            case BuildingBlockType.WES:
                return PrefabBuildingBlock_WES;
            case BuildingBlockType.NES:
                return PrefabBuildingBlock_NES;
            case BuildingBlockType.WNS:
                return PrefabBuildingBlock_WNS;
            case BuildingBlockType.Wx:
                return PrefabBuildingBlock_Wx;
            case BuildingBlockType.Nx:
                return PrefabBuildingBlock_Nx;
            case BuildingBlockType.Ex:
                return PrefabBuildingBlock_Ex;
            case BuildingBlockType.Sx:
                return PrefabBuildingBlock_Sx;

            default:
                return PrefabBuildingBlock_Empty;
        }
    }

    BuildingBlockType GetRandomBuildingBlockType()
    {
        ArrayList allTypes = GetListOfAllBuildingBlockTypes();
        return (BuildingBlockType)allTypes[Random.Range(0, allTypes.Count)];
    }

    void InstantiateDungeonBuildingBlocksFromGrid()
    {
        prefabBuildingBlockWidth = PrefabBuildingBlock_WNES.localScale.x;
        prefabBuildingBlockHeight = PrefabBuildingBlock_WNES.localScale.z;

        for (int i = 0; i < NumBuildingBlocksAcross; ++i)
        {
            for (int j = 0; j < NumBuildingBlocksUp; ++j)
            {                
                if (buildingBlockTypeGrid[i, j] != BuildingBlockType.None)
                {
                    float instantiateXPosition = transform.position.x + (i * prefabBuildingBlockWidth);
                    float instantiateZPosition = transform.position.z + (j * prefabBuildingBlockHeight);
                    Transform prefabToMake = GetPrefabFromBuildingBlockType(buildingBlockTypeGrid[i, j]);
                    Transform createBlock = (Transform)Instantiate(prefabToMake, new Vector3(instantiateXPosition, 0.0f, instantiateZPosition), Quaternion.identity);

                    if (MaterialOverrideStairsUp != null && i == stairsUpLocation.x && j == stairsUpLocation.z)
                    {
                        for (int childIndex = 0; childIndex < createBlock.GetChildCount(); ++childIndex)
                        {
                            Transform child = createBlock.GetChild(childIndex);
                            child.renderer.material = MaterialOverrideStairsUp;
                        }
                    }
                    else if (MaterialOverrideStairsDown != null && i == stairsDownLocation.x && j == stairsDownLocation.z)
                    {
                        for (int childIndex = 0; childIndex < createBlock.GetChildCount(); ++childIndex)
                        {
                            Transform child = createBlock.GetChild(childIndex);
                            child.renderer.material = MaterialOverrideStairsDown;
                        }
                    }

                    createBlock.parent = PrefabBuildingBlockParent;                    
                }
            }
        }
    }

    ArrayList GetListOfAllBuildingBlockTypes()
    {
        ArrayList returnList = new ArrayList();
        foreach (BuildingBlockType value in BuildingBlockType.GetValues(typeof(BuildingBlockType)))
        {
            returnList.Add(value);
        }
        return returnList;
    }

    BuildingBlockType GetQualifyingBlockType(int placementGridX, int placementGridZ, bool hasWest, bool hasNorth, bool hasEast, bool hasSouth, bool noWest, bool noNorth, bool noEast, bool noSouth, bool useDeadEnds)
    {
        if (useDeadEnds)
        {
            return BuildingBlockType.Empty;
        }
             
        ArrayList currentQualifyingTypes = GetListOfAllBuildingBlockTypes();

        currentQualifyingTypes.Remove(BuildingBlockType.Empty);
        currentQualifyingTypes.Remove(BuildingBlockType.None);

        currentQualifyingTypes.Remove(BuildingBlockType.Wx);
        currentQualifyingTypes.Remove(BuildingBlockType.Nx);
        currentQualifyingTypes.Remove(BuildingBlockType.Ex);
        currentQualifyingTypes.Remove(BuildingBlockType.Sx);
     
        if (hasWest == true)
        {
            currentQualifyingTypes.Remove(BuildingBlockType.NS);
            currentQualifyingTypes.Remove(BuildingBlockType.NE);
            currentQualifyingTypes.Remove(BuildingBlockType.ES);
            currentQualifyingTypes.Remove(BuildingBlockType.NES);
        }
        
        if (hasNorth == true)
        {
            currentQualifyingTypes.Remove(BuildingBlockType.WE);
            currentQualifyingTypes.Remove(BuildingBlockType.WS);
            currentQualifyingTypes.Remove(BuildingBlockType.ES);
            currentQualifyingTypes.Remove(BuildingBlockType.WES);            
        }

        if (hasEast == true)
        {
            currentQualifyingTypes.Remove(BuildingBlockType.NS);
            currentQualifyingTypes.Remove(BuildingBlockType.WN);
            currentQualifyingTypes.Remove(BuildingBlockType.WS);
            currentQualifyingTypes.Remove(BuildingBlockType.WNS);
        }

        if (hasSouth == true)
        {
            currentQualifyingTypes.Remove(BuildingBlockType.WE);
            currentQualifyingTypes.Remove(BuildingBlockType.NE);
            currentQualifyingTypes.Remove(BuildingBlockType.WN);
            currentQualifyingTypes.Remove(BuildingBlockType.WNE);
        }

        if (placementGridX == 0 || noWest == true)
        {
            currentQualifyingTypes.Remove(BuildingBlockType.WNES);
            currentQualifyingTypes.Remove(BuildingBlockType.WE);            
            currentQualifyingTypes.Remove(BuildingBlockType.WN);
            currentQualifyingTypes.Remove(BuildingBlockType.WS);
            currentQualifyingTypes.Remove(BuildingBlockType.WNE);
            currentQualifyingTypes.Remove(BuildingBlockType.WES);
            currentQualifyingTypes.Remove(BuildingBlockType.WNS);            
        }

        if (placementGridX == NumBuildingBlocksAcross - 1 || noEast == true)
        {
            currentQualifyingTypes.Remove(BuildingBlockType.WNES);
            currentQualifyingTypes.Remove(BuildingBlockType.WE);
            currentQualifyingTypes.Remove(BuildingBlockType.NE);
            currentQualifyingTypes.Remove(BuildingBlockType.ES);
            currentQualifyingTypes.Remove(BuildingBlockType.WNE);
            currentQualifyingTypes.Remove(BuildingBlockType.WES);
            currentQualifyingTypes.Remove(BuildingBlockType.NES);
        }
        
        if (placementGridZ == 0 || noSouth == true)
        {
            currentQualifyingTypes.Remove(BuildingBlockType.WNES);
            currentQualifyingTypes.Remove(BuildingBlockType.NS);
            currentQualifyingTypes.Remove(BuildingBlockType.WS);
            currentQualifyingTypes.Remove(BuildingBlockType.ES);
            currentQualifyingTypes.Remove(BuildingBlockType.WES);
            currentQualifyingTypes.Remove(BuildingBlockType.NES);
            currentQualifyingTypes.Remove(BuildingBlockType.WNS);
        }

        if (placementGridZ == NumBuildingBlocksUp - 1 || noNorth == true)
        {
            currentQualifyingTypes.Remove(BuildingBlockType.WNES);
            currentQualifyingTypes.Remove(BuildingBlockType.NS);
            currentQualifyingTypes.Remove(BuildingBlockType.NE);
            currentQualifyingTypes.Remove(BuildingBlockType.WN);
            currentQualifyingTypes.Remove(BuildingBlockType.WNE);
            currentQualifyingTypes.Remove(BuildingBlockType.NES);
            currentQualifyingTypes.Remove(BuildingBlockType.WNS);
        }

        if (currentQualifyingTypes.Count == 0)
        {
            return BuildingBlockType.Empty;
        }
        else
        {
            return (BuildingBlockType)currentQualifyingTypes[Random.Range(0, currentQualifyingTypes.Count)];   
        }        
    }

    bool CellLocationHasEast(CellLocation testLocation)
    {
        if (testLocation.x == NumBuildingBlocksAcross - 1)
        {
            return false;
        }

        BuildingBlockType testType = buildingBlockTypeGrid[testLocation.x, testLocation.z];
        switch (testType)
        {
            case BuildingBlockType.WNES:
            case BuildingBlockType.WE:
            case BuildingBlockType.NE:
            case BuildingBlockType.ES:
            case BuildingBlockType.WNE:
            case BuildingBlockType.WES:
            case BuildingBlockType.NES:
            case BuildingBlockType.Ex:
                return true;
            default:
                return false;
        }
    }

    bool CellLocationHasWest(CellLocation testLocation)
    {
        if (testLocation.x == 0)
        {
            return false;
        }

        BuildingBlockType testType = buildingBlockTypeGrid[testLocation.x, testLocation.z];
        switch (testType)
        {
            case BuildingBlockType.WNES:
            case BuildingBlockType.WE:
            case BuildingBlockType.WN:
            case BuildingBlockType.WS:
            case BuildingBlockType.WNE:
            case BuildingBlockType.WES:
            case BuildingBlockType.WNS:
            case BuildingBlockType.Wx:
                return true;
            default:
                return false;
        }
    }

    bool CellLocationHasNorth(CellLocation testLocation)
    {
        if (testLocation.z == NumBuildingBlocksUp - 1)
        {
            return false;
        }

        BuildingBlockType testType = buildingBlockTypeGrid[testLocation.x, testLocation.z];
        switch (testType)
        {
            case BuildingBlockType.WNES:
            case BuildingBlockType.NS:
            case BuildingBlockType.NE:
            case BuildingBlockType.WN:
            case BuildingBlockType.WNE:
            case BuildingBlockType.NES:
            case BuildingBlockType.WNS:
            case BuildingBlockType.Nx:
                return true;
            default:
                return false;
        }
    }

    bool CellLocationHasSouth(CellLocation testLocation)
    {
        if (testLocation.z == 0)
        {
            return false;
        }

        BuildingBlockType testType = buildingBlockTypeGrid[testLocation.x, testLocation.z];
        switch (testType)
        {            
            case BuildingBlockType.WNES:
            case BuildingBlockType.NS:
            case BuildingBlockType.WS:
            case BuildingBlockType.ES:
            case BuildingBlockType.WES:
            case BuildingBlockType.NES:
            case BuildingBlockType.WNS:
            case BuildingBlockType.Sx:
                return true;
            default:
                return false;
        }
    }

    BuildingBlockType ChooseValidBuildingBlockType(CellLocation location)
    {
        bool hasWest = false;
        bool hasNorth = false;
        bool hasEast = false;
        bool hasSouth = false;
        bool noWest = false;
        bool noNorth = false;
        bool noEast = false;
        bool noSouth = false;

        if (location.x > 0 && buildingBlockTypeGrid[location.x - 1, location.z] != BuildingBlockType.Empty)
        {
            if (CellLocationHasEast(new CellLocation(location.x - 1, location.z)))
            {
                hasWest = true;
            }
            else
            {
                noWest = true;
            }
        }

        if (location.x < NumBuildingBlocksAcross - 1 && buildingBlockTypeGrid[location.x + 1, location.z] != BuildingBlockType.Empty)
        {
            if (CellLocationHasWest(new CellLocation(location.x + 1, location.z)))
            {
                hasEast = true;
            }
            else
            {
                noEast = true;
            }
        }

        if (location.z > 0  && buildingBlockTypeGrid[location.x, location.z - 1] != BuildingBlockType.Empty)
        {
            if (CellLocationHasNorth(new CellLocation(location.x, location.z - 1)))
            {
                hasSouth = true;
            }
            else
            {
                noSouth = true;
            }
        }

        if (location.z < NumBuildingBlocksUp - 1 && buildingBlockTypeGrid[location.x, location.z + 1] != BuildingBlockType.Empty)
        {
            if (CellLocationHasSouth(new CellLocation(location.x, location.z + 1)))
            {
                hasNorth = true;
            }
            else
            {
                noNorth = true;
            }
        }

        return GetQualifyingBlockType(location.x, location.z, hasWest, hasNorth, hasEast, hasSouth, noWest, noNorth, noEast, noSouth, false);        
    }

    void FillOutGrid()
    {
        for (int i = 0; i < NumBuildingBlocksAcross; ++i)
        {
            for (int j = 0; j < NumBuildingBlocksUp; ++j)
            {
                if (buildingBlockTypeGrid[i, j] == BuildingBlockType.Empty)
                {
                    buildingBlockTypeGrid[i, j] = ChooseValidBuildingBlockType(new CellLocation(i, j));
                }
            }
        }
    }    

    bool CellConnectedToCell(int startLocX, int startLocZ, int goalLocX, int goalLocZ, ref ArrayList knownExistingConnections)
    {
        ArrayList alreadySearchedList = new ArrayList();
        ArrayList toSearchList = new ArrayList();

        bool foundPath = false;
        bool doneWithSearch = false;
        toSearchList.Add(new CellLocation(startLocX, startLocZ));

        while (!doneWithSearch)
        {
            if (toSearchList.Count == 0)
            {
                doneWithSearch = true;
                break;
            }

            CellLocation toSearch = (CellLocation)toSearchList[0];
            toSearchList.RemoveAt(0);
            if (alreadySearchedList.Contains(toSearch) == false)
            {
                alreadySearchedList.Add(toSearch);
            }

            if ((toSearch.x == goalLocX && toSearch.z == goalLocZ) || knownExistingConnections.Contains(toSearch))
            {
                doneWithSearch = true;
                foundPath = true;
                
                foreach (CellLocation pos in alreadySearchedList)
                {
                    knownExistingConnections.Add(pos);
                }

                foreach (CellLocation pos in toSearchList)
                {
                    knownExistingConnections.Add(pos);
                }                

                break;
            }
            else
            {
                if (CellLocationHasEast(new CellLocation(toSearch.x, toSearch.z)))
                {
                    CellLocation newLocation = new CellLocation(toSearch.x + 1, toSearch.z);
                    if (toSearchList.Contains(newLocation) == false && alreadySearchedList.Contains(newLocation) == false)
                    {
                        toSearchList.Add(newLocation);
                    }
                }

                if (CellLocationHasWest(new CellLocation(toSearch.x, toSearch.z)))
                {
                    CellLocation newLocation = new CellLocation(toSearch.x - 1, toSearch.z);
                    if (toSearchList.Contains(newLocation) == false && alreadySearchedList.Contains(newLocation) == false)
                    {
                        toSearchList.Add(newLocation);
                    }
                }

                if (CellLocationHasNorth(new CellLocation(toSearch.x, toSearch.z)))
                {
                    CellLocation newLocation = new CellLocation(toSearch.x, toSearch.z + 1);
                    if (toSearchList.Contains(newLocation) == false && alreadySearchedList.Contains(newLocation) == false)
                    {
                        toSearchList.Add(newLocation);
                    }
                }

                if (CellLocationHasSouth(new CellLocation(toSearch.x, toSearch.z)))
                {
                    CellLocation newLocation = new CellLocation(toSearch.x, toSearch.z - 1);
                    if (toSearchList.Contains(newLocation) == false && alreadySearchedList.Contains(newLocation) == false)
                    {
                        toSearchList.Add(newLocation);
                    }
                }
            }
        }

        return foundPath;
    }

    void FixUpIslands()
    {
        ArrayList knownExistingConnections = new ArrayList();

        for (int i = 0; i < NumBuildingBlocksAcross; ++i)
        {
            for (int j = 0; j < NumBuildingBlocksUp; ++j)
            {
                if (buildingBlockTypeGrid[i, j] != BuildingBlockType.Empty && !CellConnectedToCell(i, j, NumBuildingBlocksAcross / 2, NumBuildingBlocksUp / 2, ref knownExistingConnections))
                {
                    buildingBlockTypeGrid[i, j] = BuildingBlockType.Empty;
                }
            }
        } 
    }

    BuildingBlockType GetDeadEndCapType(int locX, int locZ)
    {
        if (locX > 0 && CellLocationHasEast(new CellLocation(locX - 1, locZ)))
        {
            return BuildingBlockType.Wx;
        }
        else if (locX < NumBuildingBlocksAcross - 1 && CellLocationHasWest(new CellLocation(locX + 1, locZ)))
        {
            return BuildingBlockType.Ex;
        }
        else if (locZ > 0 && CellLocationHasNorth(new CellLocation(locX, locZ - 1)))
        {
            return BuildingBlockType.Sx;
        }
        else if (locZ < NumBuildingBlocksUp - 1 && CellLocationHasSouth(new CellLocation(locX, locZ + 1)))
        {
            return BuildingBlockType.Nx;
        }

        return BuildingBlockType.Empty;
    }

    void CapOffDeadEnds()
    {
        for (int i = 0; i < NumBuildingBlocksAcross; ++i)
        {
            for (int j = 0; j < NumBuildingBlocksUp; ++j)
            {
                if (buildingBlockTypeGrid[i, j] == BuildingBlockType.Empty)
                {
                    buildingBlockTypeGrid[i, j] = GetDeadEndCapType(i, j);
                }
            }
        }
    }

    void FillDungeonBuildingBlockGrid()
    {
        for (int i = 0; i < NumBuildingBlocksAcross; ++i)
        {
            for (int j = 0; j < NumBuildingBlocksUp; ++j)
            {
                buildingBlockTypeGrid[i, j] = BuildingBlockType.Empty;
            }
        }
        
        FillOutGrid();
        FixUpIslands();
        CapOffDeadEnds();
    }

    void SetHeroLocation(CellLocation newLocation)
    {
        heroLocation = newLocation;

        prefabBuildingBlockWidth = PrefabBuildingBlock_WNES.localScale.x;
        prefabBuildingBlockHeight = PrefabBuildingBlock_WNES.localScale.z;

        float heroPositionX = transform.position.x + (newLocation.x * prefabBuildingBlockWidth);
        float heroPositionZ = transform.position.z + (newLocation.z * prefabBuildingBlockHeight);

        Hero.transform.position = new Vector3(heroPositionX, 1.0f, heroPositionZ);
    }

    ArrayList GetPotentialStairPositions()
    {
        ArrayList returnList = new ArrayList();

        for (int i = 0; i < NumBuildingBlocksAcross; ++i)
        {
            for (int j = 0; j < NumBuildingBlocksUp; ++j)
            {
                if (buildingBlockTypeGrid[i, j] != BuildingBlockType.Empty)
                {
                    returnList.Add(new CellLocation(i, j));
                }
            }
        }

        return returnList;
    }

    ArrayList GetPotentialStairPositionsInRow(int row)
    {
        ArrayList returnList = new ArrayList();

        for (int i = 0; i < NumBuildingBlocksAcross; ++i)
        {
            if (buildingBlockTypeGrid[i, row] != BuildingBlockType.Empty)
            {
                returnList.Add(new CellLocation(i, row));
            }
        }

        return returnList;
    }

    ArrayList GetPotentialStairPositionsInColumn(int column)
    {
        ArrayList returnList = new ArrayList();

        for (int i = 0; i < NumBuildingBlocksUp; ++i)
        {
            if (buildingBlockTypeGrid[column, i] != BuildingBlockType.Empty)
            {
                returnList.Add(new CellLocation(column, i));
            }
        }

        return returnList;
    }

    private void InstantiateStairs(CellLocation chosenUp, CellLocation chosenDown)
    {
        prefabBuildingBlockWidth = PrefabBuildingBlock_WNES.localScale.x;
        prefabBuildingBlockHeight = PrefabBuildingBlock_WNES.localScale.z;

        int stairsUpGridX = chosenUp.x;
        int stairsUpGridZ = chosenUp.z;
        float stairsUpLocX = transform.position.x + (stairsUpGridX * prefabBuildingBlockWidth);
        float stairsUpLocZ = transform.position.z + (stairsUpGridZ * prefabBuildingBlockHeight);
        Transform createStairsUp = (Transform)Instantiate(PrefabStairsUp, new Vector3(stairsUpLocX, 0.0f, stairsUpLocZ), Quaternion.identity);

        int stairsDownGridX = chosenDown.x;
        int stairsDownGridZ = chosenDown.z;
        float stairsDownLocX = transform.position.x + (stairsDownGridX * prefabBuildingBlockWidth);
        float stairsDownLocZ = transform.position.z + (stairsDownGridZ * prefabBuildingBlockHeight);
        Transform createStairsDown = (Transform)Instantiate(PrefabStairsDown, new Vector3(stairsDownLocX, 0.0f, stairsDownLocZ), Quaternion.identity);

        stairsUpLocation = new CellLocation(stairsUpGridX, stairsUpGridZ);
        stairsDownLocation = new CellLocation(stairsDownGridX, stairsDownGridZ);

        createStairsUp.parent = OtherStuffParent;
        createStairsDown.parent = OtherStuffParent;
    }

    private void CreateStairsRandom()
    {
        ArrayList potentialStairPositions = GetPotentialStairPositions();        

        int chosenUpIndex = Random.Range(0, potentialStairPositions.Count);
        CellLocation chosenUp = (CellLocation)potentialStairPositions[chosenUpIndex];
        potentialStairPositions.RemoveAt(chosenUpIndex);
        int chosenDownIndex = Random.Range(0, potentialStairPositions.Count);
        CellLocation chosenDown = (CellLocation)potentialStairPositions[chosenDownIndex];

        InstantiateStairs(chosenUp, chosenDown);
    }

    private void CreateStairsAtEdges()
    {
        bool placingHorizontally = (Random.Range(0, 2) == 0);
                
        ArrayList potentialPositions0 = new ArrayList();
        ArrayList potentialPositions1 = new ArrayList();

        if (placingHorizontally)
        {
            for (int i = 0; i < NumBuildingBlocksAcross; ++i)
            {
                potentialPositions0 = GetPotentialStairPositionsInColumn(i);
                if (potentialPositions0.Count > 0)
                {
                    break;
                }
            }

            for (int i = NumBuildingBlocksAcross - 1; i >= 0; --i)
            {
                potentialPositions1 = GetPotentialStairPositionsInColumn(i);
                if (potentialPositions1.Count > 0)
                {
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < NumBuildingBlocksUp; ++i)
            {
                potentialPositions0 = GetPotentialStairPositionsInRow(i);
                if (potentialPositions0.Count > 0)
                {
                    break;
                }
            }

            for (int i = NumBuildingBlocksUp - 1; i >= 0; --i)
            {
                potentialPositions1 = GetPotentialStairPositionsInRow(i);
                if (potentialPositions1.Count > 0)
                {
                    break;
                }
            }
        }

        if (potentialPositions0.Count <= 0 || potentialPositions1.Count <= 0)
        {
            Debug.LogWarning("DungeonGenerator::CreateStairsAtEdges() couldn't find valid potential placement stairs up/down positions!");
        }

        bool stairsUpFromPositions0 = (Random.Range(0, 2) == 0);
        if (stairsUpFromPositions0)
        {
            int chosenUpIndex = Random.Range(0, potentialPositions0.Count);
            CellLocation chosenUp = (CellLocation)potentialPositions0[chosenUpIndex];
            potentialPositions1.Remove(new CellLocation(chosenUp.x, chosenUp.z));
            int chosenDownIndex = Random.Range(0, potentialPositions1.Count);
            CellLocation chosenDown = (CellLocation)potentialPositions1[chosenDownIndex];
            InstantiateStairs(chosenUp, chosenDown);
        }
        else
        {
            int chosenUpIndex = Random.Range(0, potentialPositions1.Count);
            CellLocation chosenUp = (CellLocation)potentialPositions1[chosenUpIndex];
            potentialPositions0.Remove(new CellLocation(chosenUp.x, chosenUp.z));
            int chosenDownIndex = Random.Range(0, potentialPositions0.Count);
            CellLocation chosenDown = (CellLocation)potentialPositions0[chosenDownIndex];
            InstantiateStairs(chosenUp, chosenDown);
        }
    }

    void CreateDungeonFloor()
    {
        FillDungeonBuildingBlockGrid();

        if (StairsPlacement == StairsPlacementType.Random)
        {
            CreateStairsRandom();
        }
        else if (StairsPlacement == StairsPlacementType.AtEdges)
        {
            CreateStairsAtEdges();
        }

        InstantiateDungeonBuildingBlocksFromGrid();

        SetHeroLocation(new CellLocation(stairsUpLocation.x, stairsUpLocation.z));
    }

	void Start()
	{	    
        buildingBlockTypeGrid = new BuildingBlockType[NumBuildingBlocksAcross, NumBuildingBlocksUp];
        CreateDungeonFloor();
	}

    void DeleteDungeonFloor()
    {
        for (int i = 0; i < PrefabBuildingBlockParent.childCount; ++i)
        {
            Destroy(PrefabBuildingBlockParent.GetChild(i).gameObject);
        }

        for (int i = 0; i < OtherStuffParent.childCount; ++i)
        {
            Destroy(OtherStuffParent.GetChild(i).gameObject);
        }
    }

    public void GoToNextLevel()
    {
        DeleteDungeonFloor();
        CreateDungeonFloor();
        DungeonFloorNumber++;
    }

    public bool IsHeroOnStairsDown()
    {
        return heroLocation.x == stairsDownLocation.x && heroLocation.z == stairsDownLocation.z;
    }

    public bool MoveHero(Direction direction)
    {
        if (direction == Direction.North && CellLocationHasNorth(heroLocation))
        {
            SetHeroLocation(new CellLocation(heroLocation.x, heroLocation.z + 1));
            return true;
        }
        else if (direction == Direction.West && CellLocationHasWest(heroLocation))
        {
            SetHeroLocation(new CellLocation(heroLocation.x - 1, heroLocation.z));
            return true;
        }
        else if (direction == Direction.South && CellLocationHasSouth(heroLocation))
        {
            SetHeroLocation(new CellLocation(heroLocation.x, heroLocation.z - 1));
            return true;
        }
        else if (direction == Direction.East && CellLocationHasEast(heroLocation))
        {
            SetHeroLocation(new CellLocation(heroLocation.x + 1, heroLocation.z));
            return true;
        }

        return false;
    }	
}
