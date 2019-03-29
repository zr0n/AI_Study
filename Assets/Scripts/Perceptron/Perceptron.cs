using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AI
{
    [System.Serializable]
    public class Perceptron 
    {

        public float[] weights = { 0f, 0f, 0f};

        public float learningRate = .01f;


        public Perceptron(bool randomizeWeights = true)
        {
            if (randomizeWeights)
                RandomizeWeights();
        }
        void RandomizeWeights()
        {
            for (int i = 0; i < weights.Length; i++)
                weights[i] = Random.Range(-1f, 1f);       
        }

        public int Guess(float[] inputs)
        {
            float sum = 0;

            for (int i = 0; i < inputs.Length; i++)
            {
                sum += inputs[i] * weights[i];
            }

            return sum > 0f ? 1 : (sum == 0f ? 0 : -1);
        }
        public void Train(float[] inputs, int correctResult)
        {
            int guess = Guess(inputs);
            Learn(inputs, guess, correctResult);
        }

        void Learn(float[] inputs, int guess, int correct)
        {
            int error = (correct - guess) ;
            for ( int i  = 0;i< weights.Length; i++)
            {
                weights[i] += inputs[i] * error * learningRate;
            }
        }
        public float GetY(float x )
        {
            return (weights[2] / weights[1]) - (weights[0] / weights[1]) * x;
        }
    }

}