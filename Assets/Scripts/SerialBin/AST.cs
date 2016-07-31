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

		private Dictionary<string, Type> symbolTable = new Dictionary<string, Type>();
	}

	public class FormatSpecification
	{
		public List<Record> records;
		public SymbolTable symbolTable;

		public FormatSpecification()
		{
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