using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace MultiplyMatrixServer
{
    public class MultiplyMatrixService : IMultiplyMatrixService
    {
        public Matrix GetMatrix(string mFileName)
        {
            return new Matrix(mFileName);
        }

        public string AddMatrix(Matrix matrix)
        {
            return matrix.saveInFile();
        }

        public string MultiplyMatrix(string m1FileName, string m2Filename)
        {
            Matrix m = new Matrix();
            m = m.Multiply(new Matrix(m1FileName), new Matrix(m2Filename));

            return m.saveInFile();
        }
    }
}
