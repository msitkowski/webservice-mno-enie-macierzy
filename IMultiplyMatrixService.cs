using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace MultiplyMatrixServer
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IMultiplyMatrixService" in both code and config file together.
    [ServiceContract]
    public interface IMultiplyMatrixService
    {
        [OperationContract]
        Matrix GetMatrix(string mFileName);

        [OperationContract]
        string AddMatrix(Matrix matrix);

        [OperationContract]
        string MultiplyMatrix(string m1FileName, string m2Filename);
    }

    [DataContract]
    public class Matrix
    {
        private int rows;
        private int cols;
        private double[,] M;

        public Matrix()
        {
            rows = 0;
            cols = 0;
            M = null;
        }

        public Matrix(int r, int c)
        {
            rows = r;
            cols = c;
            M = new double[rows, cols];

            for (int i = 0; i < rows; ++i)
            {
                for (int j = 0; j < cols; ++j)
                {
                    M[i, j] = 0.0;
                }
            }
        }

        public Matrix(string fileName)
        {
            string path = "D:\\STUDIA\\programy\\MultiplyMatrixServer\\MultiplyMatrixServer\\App_Data\\";

            if (!File.Exists(path + fileName))
            {
                throw new FileNotFoundException("File {0} not exist!", fileName); 
            }
            else
            {
                using (FileStream fs = File.OpenRead(path + fileName))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        string[] dimensions = sr.ReadLine().TrimEnd(Environment.NewLine.ToCharArray()).Split(' ');
                        rows = Convert.ToInt32(dimensions[0]);
                        cols = Convert.ToInt32(dimensions[1]);
                        M = new double[rows, cols];

                        for (int i = 0; i < rows; ++i)
                        {
                            string[] line = sr.ReadLine().TrimEnd(Environment.NewLine.ToCharArray()).Split(' ');

                            for (int j = 0; j < cols; ++j)
                            {
                                M[i, j] = Convert.ToDouble(line[j]);
                            }
                        }
                        sr.Close();
                    }
                    fs.Close();
                }
            }
        }

        [DataMember]
        public int Rows
        {
            get { return rows; }
            set
            {
                if (M == null)
                {
                    rows = value;
                }
                else
                {
                    Console.WriteLine("Rows number can not be changed when matrix is not empty.");
                }
            }
        }

        [DataMember]
        public int Cols
        {
            get { return cols; }
            set
            {
                if (M == null)
                {
                    cols = value;
                }
                else
                {
                    Console.WriteLine("Columns number can not be changed when matrix is not empty.");
                }
            }
        }
        
        [DataMember]
        private double[][] MatrixContent;

        [OnSerializing]
        public void BeforeSerializing(StreamingContext ctx)
        {
            int r = M.GetLength(0);
            int c = M.GetLength(1);
            MatrixContent = new double[r][];

            for (int i = 0; i < r; ++i)
            {
                MatrixContent[i] = new double[c];

                for (int j = 0; j < c; ++j)
                {
                    MatrixContent[i][j] = M[i, j];
                }
            }
        }

        [OnDeserialized]
        public void AfterDeserializing(StreamingContext ctx)
        {
            if (MatrixContent == null)
            {
                M = null;
            }
            else
            {
                int r = MatrixContent.Length;
                if (r == 0)
                {
                    M = new double[0, 0];
                }
                else
                {
                    int c = MatrixContent[0].Length;
                    for (int i = 1; i < r; ++i)
                    {
                        if (MatrixContent[i].Length != c)
                        {
                            throw new InvalidOperationException("Surrogate (jagged) array does not correspond to a rectangular one");
                        }
                    }

                    M = new double[r, c];
                    for (int i = 0; i < r; ++i)
                    {
                        for (int j = 0; j < c; ++j)
                        {
                            M[i, j] = MatrixContent[i][j];
                        }
                    }
                }
            }
        }

        public void setRandomValues()
        {
            Random r = new Random();

            for (int i = 0; i < rows; ++i)
            {
                for (int j = 0; j < cols; ++j)
                {
                    M[i, j] = r.NextDouble();
                }
            }
        }

        public void displayMatrix()
        {
            for (int i = 0; i < rows; ++i)
            {
                for (int j = 0; j < cols; ++j)
                {
                    Console.Write("{0} ", M[i, j]);
                }
                Console.WriteLine();
            }
        }

        internal string saveInFile()
        {
            if (M == null)
            {
                throw new Exception("Matrix is empty!");
            }
            else
            {
                Random r = new Random();
                int id = r.Next();
                string fileName = id.ToString() + "_matrix" + rows.ToString() + "x" + cols.ToString() + ".txt";
                string path = "D:\\STUDIA\\programy\\MultiplyMatrixServer\\MultiplyMatrixServer\\App_Data\\";

                while (File.Exists(path + fileName))
                {
                    id = r.Next();
                    fileName = id.ToString() + "_matrix" + rows.ToString() + "x" + cols.ToString() + ".txt";
                }

                using (FileStream fs = File.Open(path + fileName, FileMode.Create))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        sw.WriteLine(rows.ToString() + " " + cols.ToString());

                        for (int i = 0; i < rows; ++i)
                        {
                            for (int j = 0; j < cols; ++j)
                            {
                                if (j == cols - 1)
                                {
                                    sw.WriteLine(M[i, j]);
                                }
                                else
                                {
                                    sw.Write(M[i, j].ToString() + " ");
                                }
                            }
                        }
                        sw.Close();
                    }
                    fs.Close();
                }
                return fileName;
            }
        }

        internal Matrix Multiply(Matrix matrix1, Matrix matrix2)
        {
            if (matrix1.cols != matrix2.rows)
            {
                throw new Exception("Wrong matrixes dimensions! First matrix cols number must equals second matrix rows number.");
            }
            else
            {
                Matrix result = new Matrix(matrix1.rows, matrix2.cols);

                Parallel.For(0, matrix1.rows, i =>
                {
                    for (int j = 0; j < matrix2.cols; ++j)
                    {
                        for (int k = 0; k < matrix2.cols; ++k)
                        {
                            result.M[i, j] += matrix1.M[i, k] * matrix2.M[k, j];
                        }
                    }
                });
                return result;
            }
        }
    }
}
