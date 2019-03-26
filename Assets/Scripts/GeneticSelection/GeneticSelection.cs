using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AI
{

    public class GeneticSelection : MonoBehaviour
    {
        public string perfectGeneration = "Eat cheetos freak-o!!!";
        public float naturalEntropy = .01f;
        public int childrenPerGeneration = 10;
        public int frameRate = 1;
        public int generation = 0;

        int fittestScore = -1;
        int secondFittestScore = -1;
        string firstChampion;
        string secondChampion;

        bool createdPerfectChild;
        public List<string> childrenTryingToSurvive = new List<string>();


        string population;
        

        void Start()
        {
            Application.targetFrameRate = frameRate;
            RandomPopulate();
        }

        void Update()
        {
            if (createdPerfectChild)
                return;
            generation++;
            Selection();
            Populate();
        }
        
        void RandomPopulate()
        {
            for(int i = 0; i < childrenPerGeneration; i++)
            {
                childrenTryingToSurvive.Add(RandomString(perfectGeneration.Length));
            }
        }
        void Populate()
        {
            if (createdPerfectChild)
                return;

            childrenTryingToSurvive = Copulate(firstChampion, secondChampion);
        }

        void Selection()
        {
            int i = 0;
            foreach(string child in childrenTryingToSurvive)
            {
                int score = LevenshteinDistance(child, perfectGeneration);
            
                if (score < fittestScore || fittestScore == -1)
                {
                    if(score == 0)
                    {
                        createdPerfectChild = true;
                        Debug.Log("Found a perfect child at position #"+i);
                    }

                    fittestScore = score;
                    firstChampion = child;
                }
                else if(score < secondFittestScore || secondFittestScore == -1)
                {
                    secondFittestScore = score;
                    secondChampion = child;
                }
                i++;

            }
        }
        
        List<string> Copulate(string a, string b)
        {
            List<string> babies = new List<string>();
            

            for(int babyCount = 0; babyCount < childrenPerGeneration; babyCount++)
            {
                string baby = "";

                for (int i = 0; i < a.Length; i++)
                {
                    char gene;

                    if (Random.Range(0, 666) % 2 == 0 || i > b.Length - 1)
                        gene = a[i];
                    else
                        gene = b[i];

                    gene = Random.value < naturalEntropy ? Entropy(gene) : gene;

                    baby += gene;
                }

                babies.Add(baby);
            }

            return babies;
        }

        char Entropy(char c)
        {
            return RandomChar();
            //return char.ConvertFromUtf32(Random.value > .5f ? c + 1 : c - 1)[0];
        }
        /** Gives a score between 2 strings. How much closer the strings are, higher the score is. */
        public static int LevenshteinDistance(string a, string b, int lenA = -1, int lenB = -1)
        {
            int cost;

            if (lenA == -1)
                lenA = a.Length;
            if (lenA == 0)
                return lenB > -1 ? lenB : lenA;

            if (lenB == -1)
                lenB = b.Length;
            if (lenB == 0)
                return lenA;

            if (a[lenA - 1] == b[lenB - 1] || (""+a[lenA - 1]).ToUpper()[0] == b[lenB - 1]) //case insensitive
                cost = 0;
            else
                cost = 1;
            return Mathf.Min(
                LevenshteinDistance(a,b, lenA -1, lenB) + 1,
                LevenshteinDistance(a,b, lenA, lenB -1) + 1,
                LevenshteinDistance(a,b, lenA -1, lenB -1) + cost
                );
        }

        public static string RandomString(int length)
        {
            System.Random random = new System.Random();
            string result = "";
            for (int i = 0; i < length; i++)
            {
                result += RandomChar();
            }
            return result;
        }

        public static char RandomChar()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ 0123456789-! abcdefghijklmnopqrstuvwxyz";
            return chars[Random.Range(0, chars.Length)];
        }
    }

}