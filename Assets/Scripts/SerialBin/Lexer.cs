using System.Collections.Generic;

namespace SerialBin
{
	public struct TextPosition
	{
		public int charIndex;
		public int lineIndex;
		public int columnIndex;
	}

	public enum TokenType
	{
		Identifier,
		IntegerLiteral,

		Colon,
		LeftSquareBracket,
		RightSquareBracket,
		Comma
		
		// Comment,
		// WhiteSpace
	}

	public struct Token
	{
		public TokenType type;
		public string text;
		public TextPosition startPosition;
		public TextPosition endPosition;
	}

	public class Lexer
	{
		public List<Token> Tokenize(string text)
		{
			return new List<Token>();
		}
	}
}