using System.Collections.Generic;

namespace SerialBin.AST
{
	public class SymbolTable
	{
		public bool ContainsSymbol(string name)
		{
			return symbolTable.ContainsKey(name);
		}
		public void AddSymbol(string name, Type type)
		{
			if(!ContainsSymbol(name))
			{
				symbolTable.Add(name, type);
			}
			else
			{
				throw new SemanticAnalysisException("Symbol \"" + name + "\" already exists.");
			}
		}
		public Type FindSymbol(string name)
		{
			Type symbolType;
			if(symbolTable.TryGetValue(name, out symbolType))
			{
				return symbolType;
			}
			else
			{
				throw new SemanticAnalysisException("Symbol \"" + name + "\" does not exist.");
			}
		}
		public Type ResolveType(RecordTypeName typeName)
		{
			if(typeName is SimpleTypeName)
			{
				return FindSymbol(((SimpleTypeName)typeName).name);
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

		private Dictionary<string, Type> symbolTable = new Dictionary<string, Type>();
	}

	public class FormatSpecification
	{
		public string formatName;
		public List<Record> records;
		public SymbolTable symbolTable;

		public FormatSpecification(string formatName)
		{
			this.formatName = formatName;
			records = new List<Record>();
			symbolTable = new SymbolTable();
		}
	}

	public class Record
	{
		public string name;
		public RecordTypeName typeName;

		public Record(string name, RecordTypeName typeName)
		{
			this.name = name;
			this.typeName = typeName;
		}
	}

	public interface RecordTypeName { }

	public class SimpleTypeName : RecordTypeName
	{
		public string name;

		public SimpleTypeName(string name)
		{
			this.name = name;
		}
	}
	public class ArrayTypeName : RecordTypeName
	{
		public Expression elementCount;
		public RecordTypeName elementTypeName;

		public ArrayTypeName(Expression elementCount, RecordTypeName elementTypeName)
		{
			this.elementCount = elementCount;
			this.elementTypeName = elementTypeName;
		}
	}

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

	public interface Expression { }

	public class Identifier : Expression
	{
		public string text;

		public Identifier(string text)
		{
			this.text = text;
		}
	}
	public class IntegerLiteral : Expression
	{
		public string text;

		public IntegerLiteral(string text)
		{
			this.text = text;
		}
	}
}