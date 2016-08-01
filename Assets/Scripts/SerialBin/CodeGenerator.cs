using System;
using System.Text;
using System.IO;

namespace SerialBin
{
	using AST;

	// TODO: Use unique nested for loop indices.
	// TODO: Replace for loops for uint8 with ReadBytes();
	public class CodeGenerator
	{
		public string GenerateCode(FormatSpecification formatSpecification)
		{
			this.formatSpecification = formatSpecification;
			codeBuilder = new StringBuilder();
			indentationLevel = 0;

			GenerateLine("using System;");
			GenerateLine("using System.IO;");
			GenerateLine("using UnityEngine;");
			StartNextLine();
			GenerateLine("public class " + GetFileClassName());
			GenerateLine('{', 1);
			GenerateMembers();
			StartNextLine();
			GenerateDeserializeFunction();
			StartNextLine();
			GenerateBinaryReaderClass();
			GenerateLine('}');

			return codeBuilder.ToString();
		}

		FormatSpecification formatSpecification;
		private StringBuilder codeBuilder;
		private int indentationLevel;

		private string GetFileClassName()
		{
			return formatSpecification.formatName + "File";
		}

		private void Generate(char character)
		{
			codeBuilder.Append(character);
		}
		private void Generate(string code)
		{
			codeBuilder.Append(code);
		}
		private void StartNextLine(int deltaIndentationLevel = 0)
		{
			indentationLevel += deltaIndentationLevel;

			codeBuilder.Append('\n');
			codeBuilder.Append(new string('\t', indentationLevel));
		}
		private void GenerateLine(char character, int deltaIndentationLevel = 0)
		{
			Generate(character);
			StartNextLine(deltaIndentationLevel);
		}
		private void GenerateLine(string code, int deltaIndentationLevel = 0)
		{
			Generate(code);
			StartNextLine(deltaIndentationLevel);
		}

		private void GenerateExpression(Expression expression)
		{
			if(expression is Identifier)
			{
				GenerateIdentifier((Identifier)expression);
			}
			else if(expression is IntegerLiteral)
			{
				GenerateIntegerLiteral((IntegerLiteral)expression);
			}
			else
			{
				throw new NotImplementedException("Unsupported expression: " + expression.GetType().Name);
			}
		}
		private void GenerateIdentifier(Identifier identifier)
		{
			Generate(identifier.text);
		}
		private void GenerateIntegerLiteral(IntegerLiteral integerLiteral)
		{
			Generate(integerLiteral.text);
		}

		private void GenerateType(Type type)
		{
			if(type is IntegerType)
			{
				GenerateIntegerType((IntegerType)type);
			}
			else if(type is ArrayType)
			{
				GenerateArrayType((ArrayType)type);
			}
			else
			{
				throw new NotImplementedException("Unsupported type: " + type.GetType().Name);
			}
		}
		private void GenerateIntegerType(IntegerType integerType)
		{
			// Generate the sign prefix.
			string signPrefix;

			if(integerType.byteCount > 1)
			{
				signPrefix = integerType.isSigned ? "" : "u";
			}
			else
			{
				signPrefix = integerType.isSigned ? "s" : "";
			}

			Generate(signPrefix);

			// Generate the size suffix.
			string sizeSuffix;
			switch(integerType.byteCount)
			{
				case 1:
					sizeSuffix = "byte";
					break;
				case 2:
					sizeSuffix = "short";
					break;
				case 4:
					sizeSuffix = "int";
					break;
				case 8:
					sizeSuffix = "long";
					break;
				default:
					throw new NotImplementedException("Unsupported integer type byte count: " + integerType.byteCount.ToString());
			}

			Generate(sizeSuffix);
		}
		private void GenerateArrayType(ArrayType arrayType)
		{
			GenerateType(arrayType.elementType);
			Generate("[]");
		}

		private void GenerateReadFunctionSuffix(Type type)
		{
			if(type is IntegerType)
			{
				GenerateIntegerType((IntegerType)type);
			}
			else if(type is ArrayType)
			{
				throw new ArgumentOutOfRangeException("GenerateReadFunctionSuffix is not applicable to ArrayType.");
			}
			else
			{
				throw new NotImplementedException("Unsupported type: " + type.GetType().Name);
			}
		}
		private void GenerateIntegerTypeReadFunctionSuffix(IntegerType integerType)
		{
			// Generate the sign prefix.
			Generate(integerType.isSigned ? "" : "U");

			// Generate the size suffix.
			string sizeSuffix;
			switch(integerType.byteCount)
			{
				case 1:
					sizeSuffix = "Int8";
					break;
				case 2:
					sizeSuffix = "Int16";
					break;
				case 4:
					sizeSuffix = "Int32";
					break;
				case 8:
					sizeSuffix = "Int64";
					break;
				default:
					throw new NotImplementedException("Unsupported integer type byte count: " + integerType.byteCount.ToString());
			}

			Generate(sizeSuffix);
		}

		private void GenerateReadCode(string recordName, Type type)
		{
			if(type is IntegerType)
			{
				GenerateReadCode(recordName, (IntegerType)type);
			}
			else if(type is ArrayType)
			{
				GenerateReadCode(recordName, (ArrayType)type);
			}
			else
			{
				throw new NotImplementedException("Unsupported type: " + type.GetType().Name);
			}
		}
		private void GenerateReadCode(string recordName, IntegerType integerType)
		{
			Generate(recordName);
			Generate(" = reader.Read");
			GenerateIntegerTypeReadFunctionSuffix(integerType);

			// Generate the rest of the assignment.
			Generate("();");
		}
		private void GenerateReadCode(string recordName, ArrayType arrayType)
		{
			Generate(recordName);
			Generate(" = new ");
			GenerateType(arrayType.elementType);
			Generate('[');
			GenerateExpression(arrayType.elementCount);
			Generate("];"); StartNextLine();

			Generate("for(uint i = 0; i < "); Generate(recordName); Generate(".Length; i++)"); StartNextLine();
			GenerateLine('{', 1);
			GenerateReadCode(recordName + "[i]", arrayType.elementType);
			StartNextLine(-1);
			GenerateLine('}');
		}

		private void GenerateMembers()
		{
			foreach(var record in formatSpecification.records)
			{
				Generate("public ");
				GenerateType(formatSpecification.symbolTable.ResolveType(record.typeName));
				Generate(' ');
				Generate(record.name);
				Generate(';');
				StartNextLine();
			}
		}
		private void GenerateDeserializeFunction()
		{
			GenerateLine("public void Deserialize(Stream stream)");
			GenerateLine('{', 1);

			GenerateLine("var reader = new SerialBinBinaryReader(stream);");
			StartNextLine();

			foreach(var record in formatSpecification.records)
			{
				GenerateReadCode(record.name, formatSpecification.symbolTable.ResolveType(record.typeName));
				StartNextLine();
				StartNextLine();
			}

			StartNextLine(-1);
			GenerateLine('}');
		}
		private void GenerateBinaryReaderClass()
		{
			GenerateLine(File.ReadAllText("Assets/Scripts/SerialBin/ReadUtils.txt"), -1);
		}
	}
}