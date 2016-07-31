using System;
using System.Collections.Generic;

namespace SerialBin
{
	using AST;

	public class ParserException : Exception
	{
		public ParserException() { }
		public ParserException(string message) : base(message) { }
		public ParserException(string message, Exception innerException) : base(message, innerException) { }
	}

	public class Parser
	{
		public FormatSpecification Parse(List<Token> tokens)
		{
			this.tokens = tokens;
			nextTokenIndex = 0;

			var formatSpecification = new FormatSpecification();

			Token nextToken;
			while(TryPeekToken(out nextToken))
			{
				if(nextToken.type == TokenType.Identifier)
				{
					formatSpecification.records.Add(ReadRecord());
				}
				else
				{
					throw new ParserException(CreateUnexpectedTokenExceptionMessage(nextToken.type));
				}
			}

			return formatSpecification;
		}

		private List<Token> tokens;
		private int nextTokenIndex;

		private string CreateUnexpectedTokenExceptionMessage(TokenType encounteredTokenType)
		{
			return "Encountered an unexpected token: " + encounteredTokenType.ToString() + ".";
		}

		private bool TryPeekToken(out Token peekedToken)
		{
			if(nextTokenIndex < tokens.Count)
			{
				peekedToken = tokens[nextTokenIndex];
				return true;
			}
			else
			{
				peekedToken = new Token();
				return false;
			}
		}
		private Token PeekToken()
		{
			Token nextToken;

			if(TryPeekToken(out nextToken))
			{
				return nextToken;
			}
			else
			{
				throw new ParserException("Unexpectedly reached the end of the tokens.");
			}
		}
		private Token ReadToken()
		{
			var nextToken = PeekToken();
			nextTokenIndex++;

			return nextToken;
		}
		private Token ReadExpectedToken(TokenType expectedTokenType)
		{
			var nextToken = ReadToken();

			if(nextToken.type == expectedTokenType)
			{
				return nextToken;
			}
			else
			{
				throw new ParserException("Expected " + expectedTokenType.ToString() + " but encountered " + nextToken.type.ToString() + ".");
			}
		}

		private Record ReadRecord()
		{
			var name = ReadExpectedToken(TokenType.Identifier).text;
			ReadExpectedToken(TokenType.Colon);
			var typeName = ReadRecordTypeName();

			return new Record(name, typeName);
		}

		private RecordTypeName ReadRecordTypeName()
		{
			Token nextToken = PeekToken();

			if(nextToken.type == TokenType.Identifier)
			{
				return ReadSimpleTypeName();
			}
			else if(nextToken.type == TokenType.LeftSquareBracket)
			{
				return ReadArrayTypeName();
			}
			else
			{
				throw new ParserException(CreateUnexpectedTokenExceptionMessage(nextToken.type));
			}
		}
		private SimpleTypeName ReadSimpleTypeName()
		{
			return new SimpleTypeName(ReadExpectedToken(TokenType.Identifier).text);
		}
		private ArrayTypeName ReadArrayTypeName()
		{
			ReadExpectedToken(TokenType.LeftSquareBracket);
			var elementCount = ReadExpression();
			ReadExpectedToken(TokenType.Comma);
			var elementTypeName = ReadRecordTypeName();
			ReadExpectedToken(TokenType.RightSquareBracket);

			return new ArrayTypeName(elementCount, elementTypeName);
		}

		private Expression ReadExpression()
		{
			Token nextToken = PeekToken();

			if(nextToken.type == TokenType.Identifier)
			{
				return ReadIdentifier();
			}
			else if(nextToken.type == TokenType.IntegerLiteral)
			{
				return ReadIntegerLiteral();
			}
			else
			{
				throw new ParserException(CreateUnexpectedTokenExceptionMessage(nextToken.type));
			}
		}
		private Identifier ReadIdentifier()
		{
			return new Identifier(ReadExpectedToken(TokenType.Identifier).text);
		}
		private IntegerLiteral ReadIntegerLiteral()
		{
			return new IntegerLiteral(ReadExpectedToken(TokenType.IntegerLiteral).text);
		}
	}
}