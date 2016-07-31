using System;
namespace SerialBin
{
	using AST;

	public interface Type { }

	public class IntegerType : Type
	{
		public uint byteCount;
		public bool isSigned;
		public bool isBigEndian;

		public IntegerType(uint byteCount, bool isSigned, bool isBigEndian)
		{
			this.byteCount = byteCount;
			this.isSigned = isSigned;
			this.isBigEndian = isBigEndian;
		}
	}

	public class ArrayType : Type
	{
		public Expression elementCount;
		public Type elementType;

		public ArrayType(Expression elementCount, Type elementType)
		{
			this.elementCount = elementCount;
			this.elementType = elementType;
		}
	}

	public class SemanticAnalysisException : Exception
	{
		public SemanticAnalysisException() { }
		public SemanticAnalysisException(string message) : base(message) { }
		public SemanticAnalysisException(string message, Exception innerException) : base(message, innerException) { }
	}

	public class SemanticAnalyzer
	{
		public void Analyze(FormatSpecification formatSpecification)
		{
			symbolTable = formatSpecification.symbolTable;
			RegisterPrimitiveTypes();

			foreach(var record in formatSpecification.records)
			{
				symbolTable.AddSymbol(record.name, ResolveType(record.typeName));
			}
		}

		private SymbolTable symbolTable;

		private void RegisterPrimitiveTypes()
		{
			// unsigned integer types
			symbolTable.AddSymbol("u8", new IntegerType(1, false, false));

			symbolTable.AddSymbol("u16LE", new IntegerType(2, false, false));
			symbolTable.AddSymbol("u16BE", new IntegerType(2, false, true));

			symbolTable.AddSymbol("u32LE", new IntegerType(4, false, false));
			symbolTable.AddSymbol("u32BE", new IntegerType(4, false, true));

			symbolTable.AddSymbol("u64LE", new IntegerType(8, false, false));
			symbolTable.AddSymbol("u64BE", new IntegerType(8, false, true));

			// signed integer types
			symbolTable.AddSymbol("i8", new IntegerType(1, true, false));

			symbolTable.AddSymbol("i16LE", new IntegerType(2, true, false));
			symbolTable.AddSymbol("i16BE", new IntegerType(2, true, true));

			symbolTable.AddSymbol("i32LE", new IntegerType(4, true, false));
			symbolTable.AddSymbol("i32BE", new IntegerType(4, true, true));

			symbolTable.AddSymbol("i64LE", new IntegerType(8, true, false));
			symbolTable.AddSymbol("i64BE", new IntegerType(8, true, true));
		}
		private Type ResolveType(RecordTypeName typeName)
		{
			if(typeName is SimpleTypeName)
			{
				return symbolTable.FindSymbol(((SimpleTypeName)typeName).name);
			}
			else if(typeName is ArrayTypeName)
			{
				var arrayTypeName = (ArrayTypeName)typeName;
				return new ArrayType(arrayTypeName.elementCount, ResolveType(arrayTypeName.elementTypeName));
			}
			else
			{
				throw new SemanticAnalysisException("Unsupported type name: " + typeName.GetType().Name);
			}
		}
	}
}