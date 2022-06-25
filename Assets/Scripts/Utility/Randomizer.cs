using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public static class Randomizer
{
    public static IEnumerable<int> GetUniqueRandoms(int arrayOffset, int highestNum, int lowestNum = 0)
    {
        HashSet<int> randomNumbers = new HashSet<int>();
        int randoms;
        for (int i = 0; i < highestNum - arrayOffset; i++)
        {
            do
            {
                randoms = Random.Range(lowestNum, highestNum);
            } while (!randomNumbers.Add(randoms));

            yield return randoms;
        }
    }
    // public static int GetRandomPillColor(Enum colorArray)
    // {
    //     
    // }

    #region altRandomGen

    /* public static IEnumerable<int> GetUniqueRandoms(int arrayOffset, int highestNum, int lowestNum = 0)
     {
         if (highestNum <= lowestNum || arrayOffset < 0 || (arrayOffset > highestNum - lowestNum && highestNum - lowestNum > 0))
         {
             Debug.LogError("GetUniqueRandoms error!");
             yield break;
         }
 
         // generate arrayOffset random values.
         HashSet<int> randoms = new HashSet<int>();
         int random;
         for (int top = arrayOffset; top < highestNum; top++)
         {
             // May strike a duplicate.
             // Need to add +1 to make inclusive generator
             // +1 is safe even for MaxVal highestNum value because top < highestNum
             random = Random.Range(lowestNum, top + 1);
             if (!randoms.Add(random))
             {
                 // collision, add inclusive highestNum.
                 // which could not possibly have been added before.
                 randoms.Add(top);
                 yield return top;
             }
             else
             {
                 yield return random;
             }
         }
     }
     */

    #endregion
}