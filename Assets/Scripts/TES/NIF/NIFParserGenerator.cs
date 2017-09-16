﻿using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using UnityEngine;

namespace TESUnity
{
    public class NifParserGenerator
    {
        public const uint MORROWIND_NIF_VERSION = 0x04000002;

        public void GenerateParser(string nifXmlFilePath, uint nifVersion, string generatedParserFilePath)
        {
            nifXmlDoc = XDocument.Load(nifXmlFilePath);
            this.nifVersion = nifVersion;
            strBuilder = new StringBuilder();
            indentLevel = 0;

            GenerateEnums();

            File.WriteAllBytes(generatedParserFilePath, Encoding.UTF8.GetBytes(strBuilder.ToString()));
        }

        private const int SPACES_PER_INDENT = 4;

        uint nifVersion;
        private XDocument nifXmlDoc;
        private StringBuilder strBuilder;
        private int indentLevel;

        private int GetArrayDimensions(XElement addElement)
        {
            int arrayDimensions = 0;

            while(addElement.Attribute($"arr{arrayDimensions + 1}") != null)
            {
                arrayDimensions++;
            }

            return arrayDimensions;
        }
        private string GetArraySuffix(int arrayDimensions)
        {
            if(arrayDimensions == 0) { return ""; }

            return $"[{new String(',', arrayDimensions - 1)}]";
        }

        private string GetConvertedAddName(XElement addElement)
        {
            return addElement.Attribute("name").Value.Replace(" ", "");
        }
        private string GetConvertedOptionName(XElement enumElement, XElement optionElement)
        {
            var optionNamePrefix = enumElement.Attribute("prefix")?.Value;
            optionNamePrefix = (optionNamePrefix != null) ? (optionNamePrefix + '_') : "";

            var optionNameTail = optionElement.Attribute("name").Value.Replace(' ', '_').ToUpper();

            return optionNamePrefix + optionNameTail;
        }
        private string GetConvertedAddTypeName(XElement addElement)
        {
            var typeName = addElement.Attribute("type").Value;

            var templateAttributeValue = addElement.Attribute("template")?.Value;
            var typeParameterSuffix = (templateAttributeValue != null) ? $"<{ConvertTypeName(templateAttributeValue)}>" : "";

            var arraySuffix = GetArraySuffix(GetArrayDimensions(addElement));

            return typeName + typeParameterSuffix + arraySuffix;
        }
        private string ConvertTypeName(string typeName)
        {
            switch(typeName)
            {
                default:
                    return typeName;
            }
        }
        private string CleanDescription(string description)
        {
            if(description == null) { return description; }

            return Regex.Replace(description.Trim(), "\r?\n", "");
        }

        private uint VersionStringToInt(string versionString)
        {
            Debug.Assert(versionString != null);

            var versionNumberStrings = versionString.Split('.');
            Debug.Assert((versionNumberStrings.Length >= 1) && (versionNumberStrings.Length <= 4));

            uint versionInt = 0;

            for(int i = 0; i < versionNumberStrings.Length; i++)
            {
                versionInt |= uint.Parse(versionNumberStrings[i]) << (32 - (8 * (i + 1)));
            }

            return versionInt;
        }
        private bool IsElementInVersion(XElement element)
        {
            var minVersionStr = element.Attribute("ver1")?.Value;
            var minVersion = (minVersionStr != null) ? VersionStringToInt(minVersionStr) : 0;

            var maxVersionStr = element.Attribute("ver2")?.Value;
            var maxVersion = (maxVersionStr != null) ? VersionStringToInt(maxVersionStr) : uint.MaxValue;

            return ((nifVersion >= minVersion) && (nifVersion <= maxVersion));
        }

        private void Generate(char aChar)
        {
            strBuilder.Append(aChar);
        }
        private void Generate(string str)
        {
            strBuilder.Append(str);
        }
        private void GenerateLine(string line, int deltaIndentLevel = 0)
        {
            strBuilder.Append(line);
            EndLine(deltaIndentLevel);
        }
        private void EndLine(int deltaIndentLevel = 0)
        {
            indentLevel += deltaIndentLevel;

            strBuilder.AppendLine();
            strBuilder.Append(' ', SPACES_PER_INDENT * indentLevel);
        }

        private void GenerateEnums()
        {
            foreach(var enumElement in nifXmlDoc.Descendants("enum").Where(IsElementInVersion))
            {
                var enumName = enumElement.Attribute("name").Value;
                var enumElementType = ConvertTypeName(enumElement.Attribute("storage").Value);
                var enumDescription = CleanDescription(enumElement.Nodes().OfType<XText>().FirstOrDefault()?.Value);

                if(!string.IsNullOrWhiteSpace(enumDescription)) { GenerateLine($"// {enumDescription}"); }
                GenerateLine($"public enum {enumName} : {enumElementType}");
                GenerateLine("{", 1);
                GenerateEnumValues(enumElement);
                EndLine(-1);
                GenerateLine("}");
                GenerateLine("");
            }
        }
        private void GenerateEnumValues(XElement enumElement)
        {
            var optionElements = enumElement.Descendants("option").Where(IsElementInVersion).ToArray();
            var lastOptionElementIndex = optionElements.Length - 1;

            for(var i = 0; i < optionElements.Length; i++)
            {
                var optionElement = optionElements[i];
                
                var optionName = GetConvertedOptionName(enumElement, optionElement);
                var optionValue = optionElement.Attribute("value").Value;
                var optionDescription = CleanDescription(optionElement.Nodes().OfType<XText>().FirstOrDefault()?.Value);

                Generate($"{optionName} = {optionValue}");
                if(i < lastOptionElementIndex) { Generate(','); }
                if(!string.IsNullOrWhiteSpace(optionDescription)) { Generate($" // {optionDescription}"); }
                EndLine();
            }
        }

        private void GenerateNiObjects()
        {
            foreach(var niObjectElement in nifXmlDoc.Descendants("niobject").Where(IsElementInVersion))
            {
                var name = niObjectElement.Attribute("name").Value;
                var description = CleanDescription(niObjectElement.Nodes().OfType<XText>().FirstOrDefault()?.Value);
                var isAbstract = niObjectElement.Attribute("abstract")?.Value == "1";
                var baseClassName = niObjectElement.Attribute("inherit")?.Value;

                if(!string.IsNullOrWhiteSpace(description)) { GenerateLine($"// {description}"); }

                Generate("public ");
                if(isAbstract) { Generate("abstract "); }
                Generate($"class {name}");
                if(baseClassName != null) { Generate($" : {baseClassName}"); }
                EndLine();
                GenerateLine("{", 1);

                GenerateNiObjectFields(niObjectElement);
                EndLine(-1);

                GenerateLine("}");
                GenerateLine("");
            }
        }
        private void GenerateNiObjectFields(XElement niObjectElement)
        {
            foreach(var addElement in niObjectElement.Descendants("add").Where(IsElementInVersion))
            {
                var fieldName = GetConvertedAddName(addElement);
                var fieldType = GetConvertedAddTypeName(addElement);
                var fieldDescription = CleanDescription(addElement.Nodes().OfType<XText>().FirstOrDefault()?.Value);

                Generate($"public {fieldType} {fieldName};");
                if(!string.IsNullOrWhiteSpace(fieldDescription)) { Generate($" // {fieldDescription}"); }
                EndLine();
            }
        }
    }

	/*public class NIFParserGenerator
	{
		public void GenerateParser(string NIFXMLFilePath, string parserFilePath)
		{
			strBuilder = new StringBuilder();

			GenerateCode("// Automatically generated."); EndLine();
			EndLine();

			GenerateCode("using System;"); EndLine();
			EndLine();

			GenerateCode("namespace NIFReader"); EndLine();
			GenerateCode("{");
			EndLine(1);

			var XMLDocElement = NIFXMLDoc.DocumentElement;

			GenerateTypes(XMLDocElement);
			EndLine();
			GenerateBinaryReader(XMLDocElement);

			EndLine(-1);
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
		private void EndLine(int deltaIndentLevel = 0)
		{
			indentLevel += deltaIndentLevel;

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

		private void GenerateTypes(XmlElement XMLDocElement)
		{
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
			EndLine(1);

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

			EndLine(-1);
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
			EndLine(1);

			for(int memberIndex = 0; memberIndex < node.ChildNodes.Count; memberIndex++)
			{
				var memberNode = node.ChildNodes[memberIndex];

				if(memberNode.Name == "add")
				{
					GenerateAdd(memberNode);
				}
			}

			EndLine(-1);
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
			EndLine(1);

			for(int memberIndex = 0; memberIndex < node.ChildNodes.Count; memberIndex++)
			{
				var memberNode = node.ChildNodes[memberIndex];

				if(memberNode.Name == "add")
				{
					GenerateAdd(memberNode);
				}
			}

			EndLine(-1);
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

			GenerateCode(";");
			EndLine();
		}

		private void GenerateBinaryReader(XmlElement XMLDocElement)
		{
			GenerateCode("public static class NIFBinaryReader"); EndLine();
			GenerateCode("{"); EndLine(1);

			for(int i = 0; i < XMLDocElement.ChildNodes.Count; i++)
			{
				var node = XMLDocElement.ChildNodes[i];

				switch(node.Name)
				{
					case "version":
						//GenerateVersion(node);
						break;
					case "basic":
						//GenerateBasic(node);
						break;
					case "enum":
						//GenerateEnum(node);
						break;
					case "bitflags":
						//GenerateBitFlags(node);
						break;
					case "compound":
						//GenerateCompound(node);
						break;
					case "niobject":
						//GenerateNIObject(node);
						break;
					case "#comment":
						//GenerateComment(node);
						break;
					default:
						throw new NotImplementedException("Found an unexpected tag in nif.xml (" + node.Name + ").");
				}
			}

			EndLine(-1); GenerateCode("}");
		}
	}*/
}