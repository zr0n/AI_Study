using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{

    public class Matrix
    {
        public List<List<float>> matrix {
            get
            {
                if (_matrix == null)
                    _matrix = new List<List<float>>();
                return _matrix;
            }
        }

        public int Length
        {
            get
            {
                return _matrix.Count;
            }
        }

        public int LengthJ
        {
            get
            {
                if (_matrix.Count == 0)
                    return 0;

                return _matrix[0].Count;
            }
        }

        public List<float> this[int i]
        {
            get
            {
                return matrix[i];
            }
        }
        List<List<float>> _matrix;

        public Matrix(int sizeX = 0, int sizeY = 0, float maxValue = 1f, float minValue = -1f)
        {
            _matrix = new List<List<float>>();

            for (int i = 0; i < sizeX; i++)
            {
                matrix.Add(new List<float>());
                for (int j = 0; j < sizeY; j++)
                {
                    matrix[i].Add(Random.Range(minValue, maxValue));
                }
                
            }
        }
        public static Matrix operator *(Matrix m1, float f1)
        {
            Matrix result = new Matrix(m1.Length, m1.LengthJ);

            for (int i = 0; i < m1.Length; i++)
            {
                for(int j =0; j< m1.LengthJ; j++)
                {
                    result[i][j] = m1[i][j] * f1;
                }
            }

            return result;
        }
        public static Matrix operator +(Matrix m1, float f1)
        {
            Matrix result = new Matrix(m1.Length, m1.LengthJ);

            for (int i = 0; i < m1.Length; i++)
            {
                for (int j = 0; j < m1.LengthJ; j++)
                {
                    result[i][j] = m1[i][j] + f1;
                }
            }

            return result;
        }
        public static Matrix operator *(Matrix m1, Matrix m2)
        {
            Matrix result = new Matrix(m1.Length, m2.LengthJ);
            for (int i = 0; i < result.Length; i++)
            {
                for (int j = 0; j < result.LengthJ; j++)
                {
                    float sum = 0;
                    for (int k = 0; k < m1.LengthJ; k++)
                    {
                        sum += m1[i][k] * m2[k][j];
                    }
                    result[i][j] = sum;
                }

            }
            return result;
        }
        public static Matrix operator +(Matrix m1, Matrix m2)
        {
            Matrix result = new Matrix(m1.Length, m2.LengthJ);
            for (int i = 0; i < m2.Length; i++)
            {
                for (int j = 0; j < m2.LengthJ; j++)
                {
                    float sum = 0;
                    for (int k = 0; k < m1.LengthJ; k++)
                    {
                        sum += m1[i][k] + m2[k][j];
                    }
                    result[i][j] = sum;
                }

            }
            return result;
        }
        public override string ToString()
        {
            string output = "\n";
            for(int i = 0; i < Length; i++)
            {
                for (int j = 0; j < LengthJ; j++)
                {
                    output += matrix[i][j] + " ";
                }
                output += "\n";
            }
            return output;
        }

        public Matrix Transpose()
        {
            Matrix result = new Matrix(LengthJ, Length);
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result.LengthJ; j++)
                    result[i][j] = this[j][i];

            return result;
        }

    }

}