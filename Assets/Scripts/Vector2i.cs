using System;

public struct Vector2i : IEquatable<Vector2i>
{
	public static Vector2i zero
	{
		get
		{
			return new Vector2i(0, 0);
		}
	}

	public int x, y;

	public Vector2i(int x, int y)
	{
		this.x = x;
		this.y = y;
	}
	public bool Equals(Vector2i other)
	{
		return (x == other.x) && (y == other.y);
	}
	public override int GetHashCode()
	{
		return x ^ y;
	}
	public override string ToString()
	{
		return "(" + x.ToString() + ", " + y.ToString() + ")";
	}
}