using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using UnityEngine;

// TODO: enum duplicates
// TODO: conditionals
// TODO: arrays
// TODO: default values
public class NIFParserGenerator
{
	public const uint NIFVersion = 0x04000002;

	public void GenerateParser(string NIFXMLFilePath, string parserFilePath)
	{
		strBuilder = new StringBuilder();

		NIFXMLDoc = new XmlDocument();
		NIFXMLDoc.Load(NIFXMLFilePath);

		var XMLDocElement = NIFXMLDoc.DocumentElement;

		GenerateCode("// Automatically generated.");
		EndLine();
		EndLine();

		GenerateCode("using System;");
		EndLine();
		EndLine();

		GenerateCode("namespace NIFReader");
		EndLine();
		GenerateCode("{");
		indentLevel++;
		EndLine();

		for(int i = 0; i < XMLDocElement.ChildNodes.Count; i++)
		{
			var node = XMLDocElement.ChildNodes[i];

			switch(node.Name)
			{
				case "version":
					GenerateVersion(node);
					break;
				case "basic":
					GenerateBasic(node);
					break;
				case "enum":
					GenerateEnum(node);
					break;
				case "bitflags":
					GenerateBitFlags(node);
					break;
				case "compound":
					GenerateCompound(node);
					break;
				case "niobject":
					GenerateNIObject(node);
					break;
				case "#comment":
					GenerateComment(node);
					break;
				default:
					throw new NotImplementedException("Found an unexpected tag in nif.xml (" + node.Name + ").");
			}
		}

		indentLevel--;
		EndLine();
		GenerateCode("}");

		File.WriteAllBytes(parserFilePath, Encoding.UTF8.GetBytes(strBuilder.ToString()));
	}

	private StringBuilder strBuilder;
	private XmlDocument NIFXMLDoc;
	private int indentLevel = 0;

	private uint VersionStringToInt(string versionString)
	{
		var versionNumberStrings = versionString.Split('.');
		uint versionInt = 0;

		Debug.Assert((versionNumberStrings.Length >= 1) && (versionNumberStrings.Length <= 4));

		for(int i = 0; i < versionNumberStrings.Length; i++)
		{
			var versionPartInt = uint.Parse(versionNumberStrings[i]);
			versionInt |= uint.Parse(versionNumberStrings[i]) << (32 - (8 * (i + 1)));
		}

		return versionInt;
	}

	private void GenerateCode(string codeStr)
	{
		strBuilder.Append(codeStr);
	}
	private void EndLine()
	{
		strBuilder.AppendLine("");
		strBuilder.Append('\t', indentLevel);
	}
	private string FormatIdentifier(string identifier)
	{
		identifier = identifier.Replace(' ', '_');

		var invalidChars = new Regex(@"[\(\)\?:/]");
		identifier = invalidChars.Replace(identifier, "");

		return identifier;
	}
	private void GenerateIdentifier(string identifier)
	{
		GenerateCode(FormatIdentifier(identifier));
	}
	private string FormatTypeName(string typeName)
	{
		switch(typeName)
		{
			case "ulittle32":
				return "uint";
			case "BlockTypeIndex":
				return "ushort";
			case "char":
				return "byte";
			case "FileVersion":
				return "uint";
			case "Flags":
				return "ushort";
			case "hfloat":
				return "float";
			case "HeaderString":
				return "string";
			case "LineString":
				return "string";
			case "Ptr":
				return "int";
			case "Ref":
				return "int";
			case "StringOffset":
				return "uint";
			case "StringIndex":
				return "uint";
			case "TEMPLATE":
				return "T";
			case "string":
				return "NIFString";
			default:
				break;
		}

		return FormatIdentifier(typeName);
	}
	private void GenerateTypeName(string typeName)
	{
		GenerateCode(FormatTypeName(typeName));
	}

	private void GenerateComment(XmlNode node)
	{
	}
	private void GenerateVersion(XmlNode node)
	{
	}
	private void GenerateBasic(XmlNode node)
	{
	}
	private void GenerateBitFlags(XmlNode node)
	{
		GenerateEnum(node, true);
	}
	private void GenerateEnum(XmlNode node, bool isBitflags = false)
	{
		if(isBitflags)
		{
			GenerateCode("[Flags]");
			EndLine();
		}

		GenerateCode("public enum ");
		GenerateTypeName(node.Attributes["name"].Value);
		GenerateCode(" : ");
		GenerateTypeName(node.Attributes["storage"].Value);
		EndLine();

		GenerateCode("{");
		indentLevel++;
		EndLine();

		for(int valueIndex = 0; valueIndex < node.ChildNodes.Count; valueIndex++)
		{
			var enumValueNode = node.ChildNodes[valueIndex];

			if(enumValueNode.Name == "option")
			{
				GenerateIdentifier(enumValueNode.Attributes["name"].Value);
				GenerateCode(" = ");
				GenerateCode(enumValueNode.Attributes["value"].Value);
				GenerateCode(",");
				EndLine();
			}
		}

		indentLevel--;
		EndLine();
		GenerateCode("}");
		EndLine();
	}
	private void GenerateCompound(XmlNode node)
	{
		// Ignore NifSkope types.
		if(node.Attributes["name"].Value.StartsWith("ns "))
		{
			return;
		}

		GenerateCode("public class ");
		GenerateTypeName(node.Attributes["name"].Value);

		if(node.Attributes["istemplate"] != null && node.Attributes["istemplate"].Value == "1")
		{
			GenerateCode("<T>");
		}

		EndLine();

		GenerateCode("{");
		indentLevel++;
		EndLine();

		for(int memberIndex = 0; memberIndex < node.ChildNodes.Count; memberIndex++)
		{
			var memberNode = node.ChildNodes[memberIndex];

			if(memberNode.Name == "add")
			{
				GenerateAdd(memberNode);
			}
		}


		indentLevel--;
		EndLine();
		GenerateCode("}");
		EndLine();
	}
	private void GenerateNIObject(XmlNode node)
	{
		GenerateCode("public ");

		if(node.Attributes["abstract"] != null && node.Attributes["abstract"].Value == "1")
		{
			GenerateCode("abstract ");
		}

		GenerateCode("class ");

		GenerateTypeName(node.Attributes["name"].Value);

		if(node.Attributes["inherit"] != null)
		{
			GenerateCode(" : ");
			GenerateTypeName(node.Attributes["inherit"].Value);
		}

		EndLine();

		GenerateCode("{");
		indentLevel++;
		EndLine();

		for(int memberIndex = 0; memberIndex < node.ChildNodes.Count; memberIndex++)
		{
			var memberNode = node.ChildNodes[memberIndex];

			if(memberNode.Name == "add")
			{
				GenerateAdd(memberNode);
			}
		}


		indentLevel--;
		EndLine();
		GenerateCode("}");
		EndLine();
	}
	private void GenerateAdd(XmlNode node)
	{
		uint minVersion = (node.Attributes["ver1"] != null) ? VersionStringToInt(node.Attributes["ver1"].Value) : 0;
		uint maxVersion = (node.Attributes["ver2"] != null) ? VersionStringToInt(node.Attributes["ver2"].Value) : uint.MaxValue;

		if((NIFVersion < minVersion) || (NIFVersion > maxVersion))
		{
			return;
		}

		var typeName = node.Attributes["type"].Value;
		//var formattedTypeName = FormatTypeName(typeName);
		//GenerateCode(formattedTypeName);
		GenerateTypeName(typeName);

		if((typeName != "Ptr") && (typeName != "Ref") && (node.Attributes["template"] != null))
		{
			GenerateCode("<");
			GenerateTypeName(node.Attributes["template"].Value);
			GenerateCode(">");
		}

		if(node.Attributes["arr1"] != null)
		{
			GenerateCode("[");

			if(node.Attributes["arr2"] != null)
			{
				GenerateCode(",");

				if(node.Attributes["arr3"] != null)
				{
					GenerateCode(",");
				}
			}

			GenerateCode("]");
		}

		GenerateCode(" ");

		GenerateIdentifier(node.Attributes["name"].Value);

		/*if(node.Attributes["default"] != null)
		{
			GenerateCode(" = ");
			GenerateCode(node.Attributes["default"].Value);

			if(formattedTypeName == "float")
			{
				GenerateCode("f");
			}
		}*/

		GenerateCode(";");
		EndLine();
	}
}