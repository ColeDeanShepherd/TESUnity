public struct Vector2i
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
	public override int GetHashCode()
	{
		return x ^ y;
	}
	public override string ToString()
	{
		return "(" + x.ToString() + ", " + y.ToString() + ")";
	}
}