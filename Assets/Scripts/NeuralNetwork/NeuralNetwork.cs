using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{

    public class NeuralNetwork : MonoBehaviour
    {
        public List<float> inputsList = new List<float>();
        public List<List<float>> trainingInputs = new List<List<float>>
        {
            new List<float> { 1, 0 },
            new List<float> { 0, 1 },
            new List<float> { 1, 1 },
            new List<float> { 0, 0 }
        };
        public List<List<float>> trainingTargets = new List<List<float>>
        {
            new List<float> { 1 },
            new List<float> { 1 },
            new List<float> { 0 },
            new List<float> { 0 }
        };
        public Matrix inputs;

        public bool randomizeInputsAtStart;
        public bool feedOnTick = true;
        public bool debugOutput = true;

        public Matrix biasH;
        public Matrix biasO;
        public float learningRate = .1f;

        public int outputNodes = 1, hiddenLayerNodes = 3;
        public int desiredRandomizeInputsLength = 2;
        public int desiredFrameRate = 1;
        public int trainingLoops = 1000;

        public Matrix weightsIH;
        public Matrix weightsHO;
        public Matrix output;

        const float MIN_INPUT = -20f;
        const float MAX_INPUT = 20f;
        const float MIN_BIAS = 0f;
        const float MAX_BIAS = 1f;


        // Start is called before the first frame update
        void Start()
        {
            if (randomizeInputsAtStart)
                RandomizeInput();

            biasH = new Matrix(hiddenLayerNodes, 1);
            biasO = new Matrix(outputNodes, 1);

            biasH.name = "biasH";
            biasO.name = "biasO";


            Application.targetFrameRate = desiredFrameRate;
            BuildInputsMatrixFromList();

            weightsIH = new Matrix(hiddenLayerNodes, inputsList.Count);
            weightsHO = new Matrix(outputNodes, hiddenLayerNodes);

            weightsIH.name = "weightsIH";
            weightsHO.name = "weightsHO";

            Matrix trainingInputsMatrix = Matrix.FromList(trainingInputs);
            Debug.Log(trainingInputsMatrix);
            Matrix trainingTargetsMatrix = Matrix.FromList(trainingTargets);
            Debug.Log(trainingTargetsMatrix);

            for (int i = 0; i < trainingLoops; i++)
            {
                int randomIndex = Random.Range(0, trainingInputsMatrix.Length - 1);
                Train(new Matrix(trainingInputsMatrix[randomIndex]), new Matrix(trainingTargetsMatrix[randomIndex]));
            }
            FeedForward();
            DebugOutput();

        }

        void Train(Matrix input, Matrix correct)
        {
            input.name = "Training Input";
            correct.name = "Desired target";
            inputs = input;

            Matrix hidden = weightsIH * inputs;
            hidden.name = "Hidden (Training)";
            hidden += biasH;
            hidden.ApplyForAll(Sigmoid);

            Matrix outputs = weightsHO * hidden;
            outputs += biasO;
            outputs.ApplyForAll(Sigmoid);

            Matrix error = correct - outputs;
            //Debug.Log("Error " + error);


            Matrix gradients = new Matrix(outputs.matrix);
            gradients.ApplyForAll(dsigmoid);
            //Debug.Log("Gradients " + gradients);
            gradients *= error;
            gradients *= learningRate;


            Matrix hiddenT = hidden.Transpose();
            hiddenT.name = "hiddenT";
            Matrix weightsHODeltas = gradients * hiddenT;
            weightsHO += weightsHODeltas;

            biasO += gradients;

            Matrix wheightsOHTransposed = weightsHO.Transpose();
            Matrix hiddenErrors = wheightsOHTransposed * error;

            Matrix hiddenGradient = hidden.Transpose() ;
            hiddenGradient.ApplyForAll(dsigmoid);
            hiddenGradient *= hiddenErrors;
            hiddenGradient *= learningRate;

            Matrix inputsTransposed = inputs.Transpose();
            Matrix weightsIHDeltas = hiddenGradient * inputsTransposed;
            weightsIH += weightsIHDeltas;
            biasH += hiddenGradient;

        }
        // Update is called once per frame
        void Update()
        {
            if (feedOnTick)
                FeedForward();

            if (debugOutput)
                DebugOutput();
        }

        public void FeedForward(List<float> inputs)
        {
            if (this.inputsList != inputs)
            {
                this.inputsList = inputs;
                BuildInputsMatrixFromList();
              

            }
            FeedForward();
        }
        void BuildInputsMatrixFromList()
        {
            inputs = new Matrix(inputsList);
            inputs.name = "Inputs";
        }
        void FeedForward()
        {
            
            //Debug.Log("WeightsIH");
            //Debug.Log(weightsIH);
            //Debug.Log("biasH");
            //Debug.Log(biasH);
            Matrix hidden = weightsIH * inputs;
            hidden.name = "Hidden";
            //Debug.Log("Hidden");
            //Debug.Log(hidden);
            hidden += biasH;
            hidden.ApplyForAll(Sigmoid);


            output = weightsHO * hidden;
            //Debug.Log("Weights " + weightsOH);
            //Debug.Log("Hidden " + hidden);
            //Debug.Log("Op " + output);
            output += biasO;
            output.name = "output";
            output.ApplyForAll(Sigmoid);
        }

        void DebugOutput()
        {
            if (output == null || output.matrix.Count == 0)
            {
                Debug.Log("output is empty");
                return;
            }

            Debug.Log("Output: " + output.ToString());
        }

        static float Sigmoid(float x)
        {
            return 1 / (1 + Mathf.Exp(-x));
        }

        static float dsigmoid(float x)
        {
            return x * (1 - x);
        }
        void RandomizeInput()
        {
            inputsList = new List<float>();
            for(int i = 0; i< desiredRandomizeInputsLength; i++)
            {
                inputsList.Add(Random.Range(MIN_INPUT, MAX_INPUT));
            }
        }
    }

}