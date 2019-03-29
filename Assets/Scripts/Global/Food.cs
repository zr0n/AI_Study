using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{

    public class Food : Collectable
    {
        public static List<Food> GetAllFoods()
        {
            List<Food> returnList = new List<Food>();
            

            foreach(Collectable collectable in _allCollectableCache)
            {
                Food food = collectable as Food;
                if (food && !food.isLoopOwner)
                    returnList.Add(food);
            }

            return returnList;
        }

    }

}