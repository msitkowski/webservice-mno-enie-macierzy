using System;

public class Matrix
{
	public Matrix()
	{
	}

    public Matrix(int r, int c)
    {
        rows = r;
        cols = c;
        M = new double[rows];

        for (int i = 0; i < rows; ++i)
        {
            M[i] = new double[cols];

            for (int j = 0; j < cols; ++j)
            {
                M[i][j] = 0;
            }
        }
    }

    public int getRows()
    {
        return rows;
    }

    public int getCols()
    {
        return cols;
    }

    public double[][] getMatrix()
    {
        return M;
    }

    private int rows;
    private int cols;
    private double[][] M;
}
