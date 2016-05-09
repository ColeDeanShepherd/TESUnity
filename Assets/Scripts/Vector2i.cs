public struct Vector2i
{
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
}