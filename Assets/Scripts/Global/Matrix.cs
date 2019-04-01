using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    [System.Serializable]
    public class Matrix
    {
        public string name = "Undefined";
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
                
                return matrix.Count;
            }
        }

        public int LengthJ
        {
            get
            {
                if (matrix.Count == 0)
                    return 0;

                return _matrix[0].Count;
            }
        }

        public List<float> this[int i]
        {
            get
            {
                if(i < matrix.Count)
                    return matrix[i];
                return null;
            }
        }
        List<List<float>> _matrix;

        public Matrix(int sizeX = 0, int sizeY = 0, float maxValue = 1f, float minValue = 0f)
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
        public Matrix(List<float> list)
        {
            var newMatrix = new Matrix(list.Count, 1);
            for(int i = 0; i < list.Count; i++)
            {
                newMatrix[i][0] = list[i];
            }
            this._matrix = newMatrix.matrix;
        }
        public Matrix(List<List<float>> list)
        {
            Matrix newMatrix = Matrix.FromList(list);
            this._matrix = newMatrix.matrix;
        }
        public static Matrix FromList(List<List<float>> list)
        {
            Matrix result = new Matrix();
            int i = 0;
            foreach(var l in list)
            {
                result._matrix.Add(new List<float>());
                foreach (float value in l)
                    result._matrix[i].Add(value);
                i++;
            }
            return result;
        }
        public static Matrix operator *(Matrix m1, float f1)
        {
            Matrix result = new Matrix(m1.Length, m1.LengthJ);
            result.name = m1.name;
            for (int i = 0; i < m1.Length; i++)
            {

                for (int j =0; j< m1.LengthJ; j++)
                {
                    result[i][j] = (m1[i][j] * f1);
                }
            }

            return result;
        }
        public static Matrix operator +(Matrix m1, float f1)
        {
            Matrix result = new Matrix(m1.Length, m1.LengthJ);
            result.name = "["+m1.name +"]";
            for (int i = 0; i < m1.Length; i++)
            {
                result.matrix[i] = new List<float>();
                for (int j = 0; j < m1.LengthJ; j++)
                {
                    result[i][j] = (m1[i][j] + f1);
                }
            }

            return result;
        }
        public static Matrix operator *(Matrix m1, Matrix m2)
        {
                //Debug.Log("Matrix Debug " + m1.name + " x " + m2.name);
            if (m1.LengthJ != m2.Length)
            {
                Debug.LogError("Invalid Multiplication: " + m1.LengthJ + " X " + m2.Length );
                Debug.Log(m1);
                Debug.Log(m2);
                return null;
            }
            Matrix result = new Matrix(m1.Length, m2.LengthJ);
            //result.name = "[" + m1.name + "*"+m2.name+"]";

            for (int i = 0; i < result.Length; i++)
            {
                //Debug.Log("i " + i);

                for (int j = 0; j < result.LengthJ; j++)
                {
                    //Debug.Log("j " + j);
                    float sum = 0;
                    for (int k = 0; k < m1.LengthJ; k++)
                    {
                        //Debug.Log("k " + k);
                        sum += m1[i][k] * m2[k][j];
                    }
                    result[i][j] = sum;
                }

            }
            return result;
        }
        public static Matrix operator +(Matrix m1, Matrix m2)
        {
            Matrix result = new Matrix(m1.Length, m1.LengthJ);
            //Debug.Log("Matrix Debug " + m1.name + " + " + m2.name);
            //result.name = "[" + m1.name + "+" + m2.name + "]";
            for (int i = 0; i < m1.Length; i++)
            {
                for (int j = 0; j < m1.LengthJ; j++)
                {
                    if(i >= m1.Length || j >= m1[i].Count)
                    {
                        result[i][j] = (m2[i][j]);
                    }
                    else if (i >= m2.Length || j >= m2[i].Count)
                    {
                        result[i][j] = (m1[i][j]);
                    }
                    else
                        result[i][j] = (m1[i][j] + m2[i][j]);
                }

            }
            return result;
        }
        public static Matrix operator -(Matrix m1, Matrix m2)
        {
            Matrix result = new Matrix(m1.Length, m1.LengthJ);
            for (int i = 0; i < m2.Length; i++)
            {
                for (int j = 0; j < m2.LengthJ; j++)
                {
                    if (i >= m1.Length || j >= m1[i].Count)
                    {
                        result[i][j] = (m2[i][j]);
                    }
                    else if (i >= m2.Length || j >= m2[i].Count)
                    {
                        result[i][j] = (m1[i][j]);
                    }
                    else
                        result[i][j] = (m1[i][j] - m2[i][j]);
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
                                            
        public void ApplyForAll(System.Func<float, float> callback)
        {
            Matrix newMatrix = new Matrix(Length, LengthJ);
            newMatrix.name = name;
            for(int i = 0; i < Length; i++)
            {
                for(int j = 0; j< LengthJ; j++)
                {
                    newMatrix[i][j] = callback(_matrix[i][j]);
                }
            }
            this._matrix = newMatrix._matrix;
        }

        public void Randomize()
        {
            Randomize(matrix);
            foreach (var level in matrix)
                Randomize(level);
        }
        public static void Randomize<T>(List<T> matrix)
        {
            for (int i = 0; i < matrix.Count; i++)
            {
                var temp = matrix[i];
                int randomIndex = Random.Range(i, matrix.Count);
                matrix[i] = matrix[randomIndex];
                matrix[randomIndex] = temp;
            }
        }

    }

}