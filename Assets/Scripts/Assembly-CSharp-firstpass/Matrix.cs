using System;
using System.IO;

public class Matrix
{
	private double[,] m_data;

	public int Row
	{
		get
		{
			return m_data.GetLength(0);
		}
	}

	public int Col
	{
		get
		{
			return m_data.GetLength(1);
		}
	}

	public double this[int row, int col]
	{
		get
		{
			return m_data[row, col];
		}
		set
		{
			m_data[row, col] = value;
		}
	}

	public Matrix(int row)
	{
		m_data = new double[row, row];
	}

	public Matrix(int row, int col)
	{
		m_data = new double[row, col];
	}

	public Matrix(Matrix m)
	{
		int row = m.Row;
		int col = m.Col;
		m_data = new double[row, col];
		for (int i = 0; i < row; i++)
		{
			for (int j = 0; j < col; j++)
			{
				m_data[i, j] = m[i, j];
			}
		}
	}

	public void SetUnit()
	{
		for (int i = 0; i < m_data.GetLength(0); i++)
		{
			for (int j = 0; j < m_data.GetLength(1); j++)
			{
				m_data[i, j] = ((i == j) ? 1 : 0);
			}
		}
	}

	public void SetValue(double d)
	{
		for (int i = 0; i < m_data.GetLength(0); i++)
		{
			for (int j = 0; j < m_data.GetLength(1); j++)
			{
				m_data[i, j] = d;
			}
		}
	}

	public Matrix Exchange(int i, int j)
	{
		for (int k = 0; k < Col; k++)
		{
			double num = m_data[i, k];
			m_data[i, k] = m_data[j, k];
			m_data[j, k] = num;
		}
		return this;
	}

	private Matrix Multiple(int index, double mul)
	{
		for (int i = 0; i < Col; i++)
		{
			m_data[index, i] *= mul;
		}
		return this;
	}

	private Matrix MultipleAdd(int index, int src, double mul)
	{
		for (int i = 0; i < Col; i++)
		{
			m_data[index, i] += m_data[src, i] * mul;
		}
		return this;
	}

	public Matrix Transpose()
	{
		Matrix matrix = new Matrix(Col, Row);
		for (int i = 0; i < Row; i++)
		{
			for (int j = 0; j < Col; j++)
			{
				matrix[j, i] = m_data[i, j];
			}
		}
		return matrix;
	}

	private int Pivot(int row)
	{
		int num = row;
		for (int i = row + 1; i < Row; i++)
		{
			if (m_data[i, row] > m_data[num, row])
			{
				num = i;
			}
		}
		return num;
	}

	public Matrix Inverse()
	{
		if (Row != Col)
		{
			Exception ex = new Exception("\ufffd\ufffd\ufffd\ufffd\ufffdľ\ufffd\ufffd\ufffd\ufffd\ufffd\ufffdǷ\ufffd\ufffd\ufffd");
			throw ex;
		}
		StreamWriter streamWriter = new StreamWriter("..\\annex\\close_matrix.txt");
		Matrix matrix = new Matrix(this);
		Matrix matrix2 = new Matrix(Row);
		matrix2.SetUnit();
		for (int i = 0; i < Row; i++)
		{
			int num = matrix.Pivot(i);
			if (matrix.m_data[num, i] == 0.0)
			{
				Exception ex2 = new Exception("\ufffd\ufffd\ufffd\ufffd\ufffdľ\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffdʽ\ufffd\ufffd\u05b5\ufffd\ufffd\ufffd\ufffd0,");
				throw ex2;
			}
			if (num != i)
			{
				matrix.Exchange(i, num);
				matrix2.Exchange(i, num);
			}
			matrix2.Multiple(i, 1.0 / matrix[i, i]);
			matrix.Multiple(i, 1.0 / matrix[i, i]);
			for (int j = i + 1; j < Row; j++)
			{
				double mul = (0.0 - matrix[j, i]) / matrix[i, i];
				matrix.MultipleAdd(j, i, mul);
			}
			streamWriter.WriteLine("tmp=\r\n" + matrix);
			streamWriter.WriteLine("ret=\r\n" + matrix2);
		}
		streamWriter.WriteLine("**=\r\n" + this * matrix2);
		for (int num2 = Row - 1; num2 > 0; num2--)
		{
			for (int num3 = num2 - 1; num3 >= 0; num3--)
			{
				double mul = (0.0 - matrix[num3, num2]) / matrix[num2, num2];
				matrix.MultipleAdd(num3, num2, mul);
				matrix2.MultipleAdd(num3, num2, mul);
			}
		}
		streamWriter.WriteLine("tmp=\r\n" + matrix);
		streamWriter.WriteLine("ret=\r\n" + matrix2);
		streamWriter.WriteLine("***=\r\n" + this * matrix2);
		streamWriter.Close();
		return matrix2;
	}

	public bool IsSquare()
	{
		return Row == Col;
	}

	public bool IsSymmetric()
	{
		if (Row != Col)
		{
			return false;
		}
		for (int i = 0; i < Row; i++)
		{
			for (int j = i + 1; j < Col; j++)
			{
				if (m_data[i, j] != m_data[j, i])
				{
					return false;
				}
			}
		}
		return true;
	}

	public double ToDouble()
	{
		return m_data[0, 0];
	}

	public override string ToString()
	{
		string text = string.Empty;
		for (int i = 0; i < Row; i++)
		{
			for (int j = 0; j < Col; j++)
			{
				text += string.Format("{0} ", m_data[i, j]);
			}
			text += "\r\n";
		}
		return text;
	}

	public static Matrix operator +(Matrix lhs, Matrix rhs)
	{
		if (lhs.Row != rhs.Row)
		{
			Exception ex = new Exception("\ufffd\ufffd\ufffdӵ\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd");
			throw ex;
		}
		if (lhs.Col != rhs.Col)
		{
			Exception ex2 = new Exception("\ufffd\ufffd\ufffdӵ\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd");
			throw ex2;
		}
		int row = lhs.Row;
		int col = lhs.Col;
		Matrix matrix = new Matrix(row, col);
		for (int i = 0; i < row; i++)
		{
			for (int j = 0; j < col; j++)
			{
				double num2 = (matrix[i, j] = lhs[i, j] + rhs[i, j]);
			}
		}
		return matrix;
	}

	public static Matrix operator -(Matrix lhs, Matrix rhs)
	{
		if (lhs.Row != rhs.Row)
		{
			Exception ex = new Exception("\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd");
			throw ex;
		}
		if (lhs.Col != rhs.Col)
		{
			Exception ex2 = new Exception("\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd");
			throw ex2;
		}
		int row = lhs.Row;
		int col = lhs.Col;
		Matrix matrix = new Matrix(row, col);
		for (int i = 0; i < row; i++)
		{
			for (int j = 0; j < col; j++)
			{
				double num2 = (matrix[i, j] = lhs[i, j] - rhs[i, j]);
			}
		}
		return matrix;
	}

	public static Matrix operator *(Matrix lhs, Matrix rhs)
	{
		if (lhs.Col != rhs.Row)
		{
			Exception ex = new Exception("\ufffd\ufffd\ufffd\u02f5\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffdƥ\ufffd\ufffd");
			throw ex;
		}
		Matrix matrix = new Matrix(lhs.Row, rhs.Col);
		for (int i = 0; i < lhs.Row; i++)
		{
			for (int j = 0; j < rhs.Col; j++)
			{
				double num = 0.0;
				for (int k = 0; k < lhs.Col; k++)
				{
					num += lhs[i, k] * rhs[k, j];
				}
				matrix[i, j] = num;
			}
		}
		return matrix;
	}

	public static Matrix operator /(Matrix lhs, Matrix rhs)
	{
		return lhs * rhs.Inverse();
	}

	public static Matrix operator +(Matrix m)
	{
		return new Matrix(m);
	}

	public static Matrix operator -(Matrix m)
	{
		Matrix matrix = new Matrix(m);
		for (int i = 0; i < matrix.Row; i++)
		{
			for (int j = 0; j < matrix.Col; j++)
			{
				matrix[i, j] = 0.0 - matrix[i, j];
			}
		}
		return matrix;
	}

	public static Matrix operator *(double d, Matrix m)
	{
		Matrix matrix = new Matrix(m);
		for (int i = 0; i < matrix.Row; i++)
		{
			for (int j = 0; j < matrix.Col; j++)
			{
				Matrix matrix2;
				Matrix matrix3 = (matrix2 = matrix);
				int row;
				int row2 = (row = i);
				int col;
				int col2 = (col = j);
				double num = matrix2[row, col];
				matrix3[row2, col2] = num * d;
			}
		}
		return matrix;
	}

	public static Matrix operator /(double d, Matrix m)
	{
		return d * m.Inverse();
	}
}
