using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AI
{
    [System.Serializable]
    public class Perceptron 
    {
        public float[] inputs = { 0f, 0f };

        public float[] weights = { 0f, 0f};

        public float learningRate = .001f;


        public Perceptron(float[] inputs, bool randomizeWeights = true)
        {
            this.inputs = inputs;
            if (randomizeWeights)
                RandomizeWeights();
        }
        void RandomizeWeights()
        {
            for (int i = 0; i < weights.Length; i++)
                weights[i] = Random.Range(-1f, 1f);       
        }

        public int Guess()
        {
            float sum = 0;

            for (int i = 0; i < inputs.Length; i++)
            {
                sum += inputs[i] * weights[i];
            }

            return sum > 0f ? 1 : (sum == 0f ? 0 : -1);
        }
        public void Train(int correctResult)
        {
            int guess = Guess();
            Learn(guess, correctResult);
        }

        void Learn(int guess, int correct)
        {
            int error = (correct - guess) ;
            for ( int i  = 0;i< weights.Length; i++)
                weights[i] += inputs[i] * error * learningRate;
        }
    }

}