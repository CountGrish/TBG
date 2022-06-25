using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectsToDelete 
{
    class Attributes
    {
        private List<IPillVirusBehaviours> objectsWithSameColors;
        private int primaryColor;

        internal Attributes(List<IPillVirusBehaviours> objectsWithSameColors, int primaryColor)
        {
            this.objectsWithSameColors = objectsWithSameColors;
            this.primaryColor = primaryColor;
        }
        
        internal List<IPillVirusBehaviours> getSimilarColors(Attributes side2)
        {
            if (isSameColor(side2))
            {
                return objectsWithSameColors.Union(side2.ObjectsWithSameColors).ToList();
            }
            List<IPillVirusBehaviours> similarColors = objectsWithSameColors;
            for (int i = 1; i < side2.getCount() && isSameColor(side2.ObjectsWithSameColors[i].ThisID); i++)
            {
                similarColors.Add(side2.ObjectsWithSameColors[i]);
            }
            return similarColors;
        }

        private bool isSameColor(Attributes side2)
        {
            int white = (int) PillEnum.White;
            return primaryColor == white || side2.PrimaryColor == white || primaryColor == side2.primaryColor;
        }
        private bool isSameColor(int color)
        {
            int white = (int) PillEnum.White;
            return primaryColor == white || color == white || primaryColor == color;
        }

        internal int getCount()
        {
            return objectsWithSameColors.Count;
        }
        internal List<IPillVirusBehaviours> ObjectsWithSameColors => objectsWithSameColors;

        internal int PrimaryColor => primaryColor;
    }
    
    private Transform[,] grid;
    Transform pillpart;
    private Attributes[] objects;
    private List<IPillVirusBehaviours> objsForDeletion;

    public List<IPillVirusBehaviours> ObjsForDeletion => objsForDeletion;

    private int primaryColor;
    private int count;

    public ObjectsToDelete(Transform[,] grid, Transform pillpart)
    {
        this.grid = grid;
        this.pillpart = pillpart;
        objects = new Attributes[4];
        objsForDeletion = new List<IPillVirusBehaviours>();
        GetSameColorsFromAllDirections(GetPositions());
        MergeObjForDeletion(objects[0],objects[1]);
        MergeObjForDeletion(objects[1],objects[0]);
        MergeObjForDeletion(objects[2],objects[3]);
        MergeObjForDeletion(objects[3],objects[2]);
    }

    private int[] GetPositions()
    {
        Vector2 childPos = VectorF.Round(pillpart.position);

        return new[]{(int)childPos.x, (int)childPos.y};
    }

    private void MergeObjForDeletion(Attributes side1, Attributes side2)
    {
        List<IPillVirusBehaviours> objToDelete;
        objToDelete = side1.getSimilarColors(side2);
        if (objToDelete.Count >= 4)
        {
            objsForDeletion = objsForDeletion.Union(objToDelete).ToList();
        }
    }

    private void GetSameColorsFromAllDirections( int[] position)
    {
        objects[0] = CheckIdenticalColors(position[0],position[1], -1, 0);
        objects[1] = CheckIdenticalColors(position[0],position[1], 1, 0);
        objects[2] = CheckIdenticalColors(position[0],position[1], 0, -1);
        objects[3] = CheckIdenticalColors(position[0],position[1], 0, 1);

    }
    
    private Attributes CheckIdenticalColors(int xCurrent,int yCurrent,int xDirection, int yDirection)
    {
        const int whiteID = 3;
        bool primaryColorFound = false;
        IPillVirusBehaviours objCurrent;
        bool continueSearch = true;
        int primaryColor = whiteID;
        List<IPillVirusBehaviours> sameColor = new List<IPillVirusBehaviours>();
        do
        {
            objCurrent = grid[yCurrent, xCurrent].GetComponent<IPillVirusBehaviours>();
            if (!primaryColorFound && objCurrent.IsPrimaryColorCompatible())
            {
                primaryColor = objCurrent.ThisID;
                primaryColorFound = true;
            }

            if (objCurrent.IsEqualTo(primaryColor))
            {
                sameColor.Add(objCurrent);
                xCurrent += xDirection;
                yCurrent += yDirection;
                if (Board.IsWithinBoard(xCurrent, yCurrent))
                {
                    if (!grid[yCurrent, xCurrent]) continueSearch = false;
                }
                else continueSearch = false;
            }
            else
            {
                continueSearch = false;
            }
        } while (continueSearch);

        return new Attributes(sameColor,primaryColor);
    }
}