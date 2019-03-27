using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{

    public class Poison : Collectable
    {
        public static List<Poison> GetAllPoisons()
        {
            List<Poison> returnList = new List<Poison>();


            foreach (Collectable collectable in _allCollectableCache)
            {
                Poison poison = collectable as Poison;
                if (poison && !poison.isLoopOwner)
                    returnList.Add(poison);
            }

            return returnList;
        }
    }

}