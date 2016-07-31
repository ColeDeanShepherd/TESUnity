using System;
using System.Collections.Generic;

namespace SerialBin
{
	public struct TextPosition
	{
		public int charIndex;
		public int lineIndex;
		public int columnIndex;

		public TextPosition(int charIndex, int lineIndex, int columnIndex)
		{
			this.charIndex = charIndex;
			this.lineIndex = lineIndex;
			this.columnIndex = columnIndex;
		}
	}

	public enum TokenType
	{
		Identifier,
		IntegerLiteral,

		// punctuation
		Colon,
		Comma,
		LeftSquareBracket,
		RightSquareBracket,

		// ignored tokens
		WhiteSpace,
		Comment
	}

	public struct Token
	{
		public TokenType type;
		public string text;
		public TextPosition startPosition;
		public TextPosition endPosition;

		public Token(TokenType type, string text, TextPosition startPosition, TextPosition endPosition)
		{
			this.type = type;
			this.text = text;
			this.startPosition = startPosition;
			this.endPosition = endPosition;
		}
	}

	public class LexerException : Exception
	{
		public TextPosition textPosition;

		public LexerException() { }
		public LexerException(string message, TextPosition textPosition) : base(CreateMessage(message, textPosition))
		{
			this.textPosition = textPosition;
		}
		public LexerException(string message, TextPosition textPosition, Exception innerException) : base(CreateMessage(message, textPosition), innerException)
		{
			this.textPosition = textPosition;
		}

		private static string CreateMessage(string message, TextPosition textPosition)
		{
			return "Line " + (textPosition.lineIndex + 1).ToString() + ", column " + (textPosition.columnIndex + 1).ToString() + ": " + message;
		}
	}

	public class Lexer
	{
		public List<Token> Tokenize(string text)
		{
			this.text = text;
			position = new TextPosition(0, 0, 0);

			var tokens = new List<Token>();

			char nextCharacter;
			while(TryPeekChar(out nextCharacter))
			{
				if(char.IsLetter(nextCharacter))
				{
					tokens.Add(ReadIdentifier());
				}
				else if(char.IsDigit(nextCharacter))
				{
					tokens.Add(ReadIntegerLiteral());
				}
				else if(nextCharacter == ':')
				{
					tokens.Add(ReadSingleCharacterToken(':', TokenType.Colon));
				}
				else if(nextCharacter == ',')
				{
					tokens.Add(ReadSingleCharacterToken(',', TokenType.Comma));
				}
				else if(nextCharacter == '[')
				{
					tokens.Add(ReadSingleCharacterToken('[', TokenType.LeftSquareBracket));
				}
				else if(nextCharacter == ']')
				{
					tokens.Add(ReadSingleCharacterToken(']', TokenType.RightSquareBracket));
				}
				else if(char.IsWhiteSpace(nextCharacter))
				{
					ReadWhiteSpace(); // skip the white space
				}
				else if(nextCharacter == '/')
				{
					ReadComment(); // skip the comment
				}
				else
				{
					throw new LexerException("Encountered an unexpected character: '" + nextCharacter + "'.", position);
				}
			}

			return tokens;
		}

		private string text;
		private TextPosition position;

		private Token CreateToken(TokenType type, TextPosition startPosition, TextPosition endPosition)
		{
			var textLength = endPosition.charIndex - startPosition.charIndex;
			var text = this.text.Substring(startPosition.charIndex, textLength);

			return new Token(type, text, startPosition, endPosition);
		}

		private bool TryPeekChar(out char peekedChar, uint offset = 0)
		{
			int peekCharIndex = position.charIndex + (int)offset;

			if(peekCharIndex < text.Length)
			{
				peekedChar = text[peekCharIndex];
				return true;
			}
			else
			{
				peekedChar = (char)0;
				return false;
			}
		}
		private char PeekChar(uint offset = 0)
		{
			// Peek the character.
			char nextChar;
			if(TryPeekChar(out nextChar, offset))
			{
				return nextChar;
			}
			else
			{
				throw new LexerException("Unexpectedly reached the end of the text.", position);
			}
		}
		private char ReadChar()
		{
			var nextChar = PeekChar();

			// Update the lexer's position.
			position.charIndex++;

			if(nextChar != '\n')
			{
				position.columnIndex++;
			}
			else
			{
				position.lineIndex++;
				position.columnIndex = 0;
			}

			return nextChar;
		}
		private char ReadExpectedChar(char expectedChar)
		{
			var readCharPosition = position;
			var nextChar = ReadChar();

			if(nextChar == expectedChar)
			{
				return nextChar;
			}
			else
			{
				throw new LexerException("Expected '" + expectedChar + "' but encountered '" + nextChar + "'.", readCharPosition);
			}
		}

		private Token ReadWhiteSpace()
		{
			var tokenStartPosition = position;

			char nextChar;
			while(TryPeekChar(out nextChar) && char.IsWhiteSpace(nextChar))
			{
				ReadChar();
			}

			var tokenEndPosition = position;

			return CreateToken(TokenType.WhiteSpace, tokenStartPosition, tokenEndPosition);
		}

		private bool IsValidFirstIdentifierChar(char character)
		{
			return (character == '_') || char.IsLetter(character);
		}
		private bool IsValidIdentifierChar(char character)
		{
			return IsValidFirstIdentifierChar(character) || char.IsDigit(character);
		}
		private Token ReadIdentifier()
		{
			var tokenStartPosition = position;

			char nextChar;
			if(TryPeekChar(out nextChar) && IsValidFirstIdentifierChar(nextChar))
			{
				// Read the first identifier character.
				ReadChar();

				// Read the rest of the identifier's characters.
				while(TryPeekChar(out nextChar) && IsValidIdentifierChar(nextChar))
				{
					ReadChar();
				}
			}

			var tokenEndPosition = position;

			return CreateToken(TokenType.Identifier, tokenStartPosition, tokenEndPosition);
		}
		private Token ReadIntegerLiteral()
		{
			var tokenStartPosition = position;

			char nextChar;
			while(TryPeekChar(out nextChar) && char.IsDigit(nextChar))
			{
				ReadChar();
			}

			var tokenEndPosition = position;

			return CreateToken(TokenType.IntegerLiteral, tokenStartPosition, tokenEndPosition);
		}
		private Token ReadSingleCharacterToken(char tokenChar, TokenType tokenType)
		{
			var tokenStartPosition = position;
			var tokenText = ReadExpectedChar(tokenChar).ToString();
			var tokenEndPosition = position;

			return new Token(tokenType, tokenText, tokenStartPosition, tokenEndPosition);
		}
		private Token ReadComment()
		{
			var tokenStartPosition = position;

			ReadExpectedChar('/');
			ReadExpectedChar('*');

			int nestingLevel = 1;

			while(nestingLevel > 0)
			{
				if((PeekChar() == '/') && (PeekChar(1) == '*'))
				{
					ReadChar(); // Read the '/'.
					ReadChar(); // Read the '*'.

					nestingLevel++;
				}
				else if((PeekChar() == '*') && (PeekChar(1) == '/'))
				{
					ReadChar(); // Read the '*'.
					ReadChar(); // Read the '/'.
					
					nestingLevel--;
				}
				else
				{
					ReadChar(); // Skip the comment character.
				}
			}

			var tokenEndPosition = position;

			return CreateToken(TokenType.Identifier, tokenStartPosition, tokenEndPosition);
		}
	}
}