// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.IO;
using System.Linq;
using ICSharpCode.NRefactory.CSharp;
using Mono.CSharp;
using Attribute = ICSharpCode.NRefactory.CSharp.Attribute;
using CSharpParser = ICSharpCode.NRefactory.CSharp.CSharpParser;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	public class AssemblyInfoProvider
	{
		public AssemblyInfo Read(string fileName)
		{
			var syntaxTree = ReadSyntaxTree(fileName);

			if (syntaxTree == null)
				throw new Exception("Can't read assembly info syntax tree.");

			var assemblyInfo = new AssemblyInfo();

			foreach (var attributeSection in syntaxTree.Children.OfType<AttributeSection>())
			{
				foreach (var attribute in attributeSection.Attributes)
				{
					var attributeType = attribute.Type as SimpleType;
					if (attributeType == null)
						continue;

					switch (attributeType.Identifier)
					{
						case "AssemblyTitle":
							assemblyInfo.Title = GetAttributeValueAsString(attribute);
							break;
						case "AssemblyDescription":
							assemblyInfo.Description = GetAttributeValueAsString(attribute);
							break;
						case "AssemblyConfigurtation":
							break;
						case "AssemblyCompany":
							assemblyInfo.Company = GetAttributeValueAsString(attribute);
							break;
						case "AssemblyProduct":
							assemblyInfo.Product = GetAttributeValueAsString(attribute);
							break;
						case "AssemblyCopyright":
							assemblyInfo.Copyright = GetAttributeValueAsString(attribute);
							break;
						case "AssemblyTrademark":
							assemblyInfo.Trademark = GetAttributeValueAsString(attribute);
							break;
						case "AssemblyCulture":
							break;
						case "ComVisible":
							assemblyInfo.ComVisible = GetAttributeValueAsBool(attribute);
							break;
						case "Guid":
							assemblyInfo.Guid = GetAttributeValueAsGuid(attribute);
							break;
						case "AssemblyVersion":
							assemblyInfo.AssemblyVersion = GetAttributeValueAsVersion(attribute);
							break;
						case "AssemblyFileVersion":
							assemblyInfo.FileVersion = GetAttributeValueAsVersion(attribute);
							break;
						case "NeutralResourcesLanguageAttribute":
							assemblyInfo.NeutralLanguage = GetAttributeValueAsString(attribute);
							break;
					}
				}
			}

			return assemblyInfo;
		}

		public void Write(string fileName, AssemblyInfo assemblyInfo)
		{
			var syntaxTree = ReadSyntaxTree(fileName);

			if (syntaxTree == null)
				throw new Exception("Can't read assembly info syntax tree.");

			SetAttributeValueOrAddAttribute(syntaxTree, "AssemblyTitle", assemblyInfo.Title);
			SetAttributeValueOrAddAttribute(syntaxTree, "AssemblyDescription", assemblyInfo.Description);
			SetAttributeValueOrAddAttribute(syntaxTree, "AssemblyCompany", assemblyInfo.Company);
			SetAttributeValueOrAddAttribute(syntaxTree, "AssemblyProduct", assemblyInfo.Product);
			SetAttributeValueOrAddAttribute(syntaxTree, "AssemblyCopyright", assemblyInfo.Copyright);
			SetAttributeValueOrAddAttribute(syntaxTree, "AssemblyTrademark", assemblyInfo.Trademark);
			SetAttributeValueOrAddAttribute(syntaxTree, "ComVisible", assemblyInfo.ComVisible);
			SetAttributeValueOrAddAttribute(syntaxTree, "Guid", assemblyInfo.Guid.ToString());
			SetAttributeValueOrAddAttribute(syntaxTree, "AssemblyVersion", assemblyInfo.AssemblyVersion.ToString());
			SetAttributeValueOrAddAttribute(syntaxTree, "AssemblyFileVersion", assemblyInfo.FileVersion.ToString());
			SetAttributeValueOrAddAttribute(syntaxTree, "NeutralResourcesLanguageAttribute", assemblyInfo.NeutralLanguage);

			WriteSyntaxTree(syntaxTree, fileName);
		}

		private SyntaxTree ReadSyntaxTree(string fileName)
		{
			using (var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
			{
				using (var streamReader = new StreamReader(fileStream))
				{
					var codeParser = new CSharpParser();

					return codeParser.Parse(streamReader);
				}
			}
		}

		private void WriteSyntaxTree(SyntaxTree syntaxTree, string fileName)
		{
			File.WriteAllText(fileName, syntaxTree.ToString());
		}

		private string GetAttributeValueAsString(Attribute attribute)
		{
			var attributeArguments = attribute.Arguments.OfType<PrimitiveExpression>().ToArray();
			if (attributeArguments.Length == 1)
				return attributeArguments[0].Value as string;

			return string.Empty;
		}

		private bool GetAttributeValueAsBool(Attribute attribute)
		{
			var attributeArguments = attribute.Arguments.OfType<PrimitiveExpression>().ToArray();
			if (attributeArguments.Length == 1)
				return (bool)attributeArguments[0].Value;

			return false;
		}

		private Guid GetAttributeValueAsGuid(Attribute attribute)
		{
			var attributeArguments = attribute.Arguments.OfType<PrimitiveExpression>().ToArray();
			if (attributeArguments.Length == 1)
				return Guid.Parse(attributeArguments[0].Value as string);

			return Guid.Empty;
		}

		private Version GetAttributeValueAsVersion(Attribute attribute)
		{
			var attributeArguments = attribute.Arguments.OfType<PrimitiveExpression>().ToArray();
			if (attributeArguments.Length == 1)
			{
				var versionString = attributeArguments[0].Value as string;
				var versionParts = versionString.Split('.');
				if (versionParts.Length == 4)
				{
					return new Version(
						int.Parse(versionParts[0]),
						int.Parse(versionParts[1]),
						int.Parse(versionParts[2]),
						int.Parse(versionParts[3]));
				}
			}

			return null;
		}

		private void SetAttributeValueOrAddAttribute(SyntaxTree syntaxTree, string attributeName, object value)
		{
			var attribute = syntaxTree.Children.OfType<AttributeSection>().SelectMany(x => x.Attributes)
				.FirstOrDefault(x => x.Type is SimpleType && ((SimpleType)x.Type).Identifier == attributeName);

			if (attribute != null)
			{
				attribute.Arguments.Clear();
				attribute.Arguments.Add(new PrimitiveExpression(value));
			}
			else
			{
				attribute = new Attribute() { Type = new SimpleType(attributeName) };
				attribute.Arguments.Add(new PrimitiveExpression(value));

				var attributeSection = new AttributeSection(attribute) { AttributeTarget = "assembly" };
				syntaxTree.AddChild(attributeSection, new NRefactory.Role<AttributeSection>("Member"));
			}
		}
	}
}