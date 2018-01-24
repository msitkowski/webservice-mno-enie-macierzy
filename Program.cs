using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplyMatrixClient
{
    class Program
    {
        static double[][] RandomValues(int rows, int cols)
        {
            Random r = new Random();
            double[][] m = new double[rows][];

            for (int i = 0; i < rows; ++i)
            {
                m[i] = new double[cols];

                for (int j = 0; j < cols; ++j)
                {
                    m[i][j] = r.NextDouble();
                }
            }
            return m;
        }

        static void Display(double[][] data)
        {
            for (int i = 0; i < data.Length; ++i)
            {
                for (int j = 0; j < data[i].Length; ++j)
                {
                    Console.Write("{0} ", data[i][j]);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        static void Main(string[] args)
        {
            MultiplyMatrixServiceReference.MultiplyMatrixServiceClient client = new MultiplyMatrixServiceReference.MultiplyMatrixServiceClient();
            MultiplyMatrixServiceReference.Matrix matrix1 = new MultiplyMatrixServiceReference.Matrix();
            matrix1.Rows = 30;
            matrix1.Cols = 30;
            matrix1.MatrixContent = RandomValues(matrix1.Rows, matrix1.Cols);
            //Display(matrix1.MatrixContent);
            MultiplyMatrixServiceReference.Matrix matrix2 = new MultiplyMatrixServiceReference.Matrix();
            matrix2.Rows = 30;
            matrix2.Cols = 30;
            matrix2.MatrixContent = RandomValues(matrix2.Rows, matrix2.Cols);
            //Display(matrix2.MatrixContent);

            try
            {
                string matrix1_id = client.AddMatrix(matrix1);
                string matrix2_id = client.AddMatrix(matrix2);
                string result_matrix_id = client.MultiplyMatrix(matrix1_id, matrix2_id);
                Console.WriteLine("{0}, {1}, {2}", matrix1_id, matrix2_id, result_matrix_id);
                MultiplyMatrixServiceReference.Matrix result_matrix = client.GetMatrix(result_matrix_id);
                Display(result_matrix.MatrixContent);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            client.Close();
        }
    }
}
