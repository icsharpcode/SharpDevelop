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
using System.Reflection;
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
			assemblyInfo.JitOptimization = true;
			
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
							assemblyInfo.Title = GetAttributeValue<string>(attribute);
							break;
						case "AssemblyDescription":
							assemblyInfo.Description = GetAttributeValue<string>(attribute);
							break;
						case "AssemblyConfigurtation":
							break;
						case "AssemblyCompany":
							assemblyInfo.Company = GetAttributeValue<string>(attribute);
							break;
						case "AssemblyProduct":
							assemblyInfo.Product = GetAttributeValue<string>(attribute);
							break;
						case "AssemblyCopyright":
							assemblyInfo.Copyright = GetAttributeValue<string>(attribute);
							break;
						case "AssemblyTrademark":
							assemblyInfo.Trademark = GetAttributeValue<string>(attribute);
							break;
						case "AssemblyDefaultAlias":
							assemblyInfo.DefaultAlias = GetAttributeValue<string>(attribute);
							break;
						case "AssemblyVersion":
							assemblyInfo.AssemblyVersion = GetAttributeValueAsVersion(attribute);
							break;
						case "AssemblyFileVersion":
							assemblyInfo.AssemblyFileVersion = GetAttributeValueAsVersion(attribute);
							break;
						case "AssemblyInformationalVersion":
							assemblyInfo.InformationalVersion = GetAttributeValueAsVersion(attribute);
							break;
						case "Guid":
							assemblyInfo.Guid = GetAttributeValueAsGuid(attribute);
							break;
						case "NeutralResourcesLanguage":
							assemblyInfo.NeutralLanguage = GetAttributeValue<string>(attribute);
							break;
						case "ComVisible":
							assemblyInfo.ComVisible = GetAttributeValue<bool>(attribute);
							break;
						case "CLSCompliant":
							assemblyInfo.ClsCompliant = GetAttributeValue<bool>(attribute);
							break;
						case "AssemblyFlags":
							var assemblyFlags = (AssemblyNameFlags)GetAttributeValue<int>(attribute, (int)AssemblyNameFlags.PublicKey);
							assemblyInfo.JitOptimization = !assemblyFlags.HasFlag(AssemblyNameFlags.EnableJITcompileOptimizer);
							assemblyInfo.JitTracking = assemblyFlags.HasFlag(AssemblyNameFlags.EnableJITcompileTracking);
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

			SetAttributeValueOrAddAttributeIfNotDefault(syntaxTree, "AssemblyTitle", assemblyInfo.Title);
			SetAttributeValueOrAddAttributeIfNotDefault(syntaxTree, "AssemblyDescription", assemblyInfo.Description);
			SetAttributeValueOrAddAttributeIfNotDefault(syntaxTree, "AssemblyCompany", assemblyInfo.Company);
			SetAttributeValueOrAddAttributeIfNotDefault(syntaxTree, "AssemblyProduct", assemblyInfo.Product);
			SetAttributeValueOrAddAttributeIfNotDefault(syntaxTree, "AssemblyCopyright", assemblyInfo.Copyright);
			SetAttributeValueOrAddAttributeIfNotDefault(syntaxTree, "AssemblyTrademark", assemblyInfo.Trademark);
			SetAttributeValueOrAddAttributeIfNotDefault(syntaxTree, "AssemblyDefaultAlias", assemblyInfo.DefaultAlias);
			SetAttributeValueOrAddAttributeIfNotDefault(syntaxTree, "AssemblyVersion", assemblyInfo.AssemblyVersion, null, true);
			SetAttributeValueOrAddAttributeIfNotDefault(syntaxTree, "AssemblyFileVersion", assemblyInfo.AssemblyFileVersion, null, true);
			SetAttributeValueOrAddAttributeIfNotDefault(syntaxTree, "AssemblyInformationalVersion", assemblyInfo.InformationalVersion, null, true);
			SetAttributeValueOrAddAttributeIfNotDefault(syntaxTree, "Guid", assemblyInfo.Guid, null, true);
			SetAttributeValueOrAddAttributeIfNotDefault(syntaxTree, "NeutralResourcesLanguage", assemblyInfo.NeutralLanguage);
			SetAttributeValueOrAddAttributeIfNotDefault(syntaxTree, "ComVisible", assemblyInfo.ComVisible, false);
			SetAttributeValueOrAddAttributeIfNotDefault(syntaxTree, "CLSCompliant", assemblyInfo.ClsCompliant, false);

			AssemblyNameFlags assemblyFlags = AssemblyNameFlags.PublicKey;
			if (!assemblyInfo.JitOptimization)
				assemblyFlags = assemblyFlags | AssemblyNameFlags.EnableJITcompileOptimizer;
			if (assemblyInfo.JitTracking)
				assemblyFlags = assemblyFlags | AssemblyNameFlags.EnableJITcompileTracking;

			SetAttributeValueOrAddAttributeIfNotDefault(syntaxTree, "AssemblyFlags", (int)assemblyFlags, (int)AssemblyNameFlags.PublicKey);

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

		private T GetAttributeValue<T>(Attribute attribute, T defaultValue = default(T))
		{
			var attributeArguments = attribute.Arguments.OfType<PrimitiveExpression>().ToArray();
			if (attributeArguments.Length == 1 && attributeArguments[0].Value is T)
				return (T)attributeArguments[0].Value;

			return defaultValue;
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
				if (!string.IsNullOrEmpty(versionString))
				{
					Version version;
					if (Version.TryParse(versionString, out version))
						return version;
				}
			}

			return null;
		}

		private void SetAttributeValueOrAddAttributeIfNotDefault(SyntaxTree syntaxTree, string attributeName, object value, object defaultValue = null, bool transformValueToString = false)
		{
			if (value == null)
				return;

			var attribute = syntaxTree.Children.OfType<AttributeSection>().SelectMany(x => x.Attributes)
				.FirstOrDefault(x => x.Type is SimpleType && ((SimpleType)x.Type).Identifier == attributeName);

			var attributeValue = transformValueToString ? value.ToString() : value;

			if (attribute != null)
			{
				attribute.Arguments.Clear();
				attribute.Arguments.Add(new PrimitiveExpression(attributeValue));
			}
			else if (value != null && !value.Equals(defaultValue))
			{
				attribute = new Attribute() { Type = new SimpleType(attributeName) };
				attribute.Arguments.Add(new PrimitiveExpression(attributeValue));

				var attributeSection = new AttributeSection(attribute) { AttributeTarget = "assembly" };
				syntaxTree.AddChild(attributeSection, new NRefactory.Role<AttributeSection>("Member"));
			}
		}
	}
}