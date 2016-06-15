using System;

/// <summary>
/// A 2-dimensional vector with integer components.
/// </summary>
public struct Vector2i : IEquatable<Vector2i>
{
	public static Vector2i zero { get { return new Vector2i(0, 0); } }
	public static Vector2i right { get { return new Vector2i(1, 0); } }
	public static Vector2i up { get { return new Vector2i(0, 1); } }
	public static Vector2i one { get { return new Vector2i(1, 1); } }

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