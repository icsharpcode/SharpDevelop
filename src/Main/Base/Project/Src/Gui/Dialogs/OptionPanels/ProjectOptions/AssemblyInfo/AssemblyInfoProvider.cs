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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.CSharp;
using Attribute = ICSharpCode.NRefactory.CSharp.Attribute;
using CSharpParser = ICSharpCode.NRefactory.CSharp.CSharpParser;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	/// <summary>
	/// Class for reading and writing assembly info file
	/// </summary>
	public class AssemblyInfoProvider
	{
		#region Constants

		private const string Attribute = "Attribute";

		private const string AssemblyTitle = "AssemblyTitle";
		private const string AssemblyDescription = "AssemblyDescription";
		private const string AssemblyCompany = "AssemblyCompany";
		private const string AssemblyProduct = "AssemblyProduct";
		private const string AssemblyCopyright = "AssemblyCopyright";
		private const string AssemblyTrademark = "AssemblyTrademark";
		private const string AssemblyDefaultAlias = "AssemblyDefaultAlias";
		private const string AssemblyVersion = "AssemblyVersion";
		private const string AssemblyFileVersion = "AssemblyFileVersion";
		private const string AssemblyInformationalVersion = "AssemblyInformationalVersion";
		private const string Guid = "Guid";
		private const string NeutralResourcesLanguage = "NeutralResourcesLanguage";
		private const string ComVisible = "ComVisible";
		private const string ClsCompliant = "CLSCompliant";
		private const string AssemblyFlags = "AssemblyFlags";

		#endregion

		/// <summary>
		/// Read assembly info from file
		/// </summary>
		/// <param name="assemblyInfoFileName">Source file name</param>
		/// <returns>Assembly info model</returns>
		public AssemblyInfo ReadAssemblyInfo(string assemblyInfoFileName)
		{
			using (var fileStream = new FileStream(assemblyInfoFileName, FileMode.Open, FileAccess.Read))
			{
				return ReadAssemblyInfo(fileStream);
			}
		}

		/// <summary>
		/// Read assembly info from stream
		/// </summary>
		/// <param name="stream">Sourcce stream</param>
		/// <returns>Assembly info model</returns>
		public AssemblyInfo ReadAssemblyInfo(Stream stream)
		{
			var syntaxTree = ReadSyntaxTree(stream);

			if (syntaxTree == null)
				throw new Exception("Can't read assembly info syntax tree.");

			var assemblyInfo = new AssemblyInfo {JitOptimization = true};

			foreach (var attributeSection in syntaxTree.Children.OfType<AttributeSection>())
			{
				foreach (var attribute in attributeSection.Attributes)
				{
					var attributeType = attribute.Type as SimpleType;
					if (attributeType == null)
						continue;

					switch (attributeType.Identifier)
					{
						case AssemblyTitle:
						case AssemblyTitle + Attribute:
							assemblyInfo.Title = GetAttributeValue<string>(attribute);
							break;
						case AssemblyDescription:
						case AssemblyDescription + Attribute:
							assemblyInfo.Description = GetAttributeValue<string>(attribute);
							break;
						case AssemblyCompany:
						case AssemblyCompany + Attribute:
							assemblyInfo.Company = GetAttributeValue<string>(attribute);
							break;
						case AssemblyProduct:
						case AssemblyProduct + Attribute:
							assemblyInfo.Product = GetAttributeValue<string>(attribute);
							break;
						case AssemblyCopyright:
						case AssemblyCopyright + Attribute:
							assemblyInfo.Copyright = GetAttributeValue<string>(attribute);
							break;
						case AssemblyTrademark:
						case AssemblyTrademark + Attribute:
							assemblyInfo.Trademark = GetAttributeValue<string>(attribute);
							break;
						case AssemblyDefaultAlias:
						case AssemblyDefaultAlias + Attribute:
							assemblyInfo.DefaultAlias = GetAttributeValue<string>(attribute);
							break;
						case AssemblyVersion:
						case AssemblyVersion + Attribute:
							assemblyInfo.AssemblyVersion = GetAttributeValue<string>(attribute);
							break;
						case AssemblyFileVersion:
						case AssemblyFileVersion + Attribute:
							assemblyInfo.AssemblyFileVersion = GetAttributeValue<string>(attribute);
							break;
						case AssemblyInformationalVersion:
						case AssemblyInformationalVersion + Attribute:
							assemblyInfo.InformationalVersion = GetAttributeValue<string>(attribute);
							break;
						case Guid:
						case Guid + Attribute:
							assemblyInfo.Guid = GetAttributeValueAsGuid(attribute);
							break;
						case NeutralResourcesLanguage:
						case NeutralResourcesLanguage + Attribute:
							assemblyInfo.NeutralLanguage = GetAttributeValue<string>(attribute);
							break;
						case ComVisible:
						case ComVisible + Attribute:
							assemblyInfo.ComVisible = GetAttributeValue<bool>(attribute);
							break;
						case ClsCompliant:
						case ClsCompliant + Attribute:
							assemblyInfo.ClsCompliant = GetAttributeValue<bool>(attribute);
							break;
						case AssemblyFlags:
						case AssemblyFlags + Attribute:
							var assemblyFlags = GetAssemblyFlagsFromAttribute(attribute);
							assemblyInfo.JitOptimization = !assemblyFlags.HasFlag(AssemblyNameFlags.EnableJITcompileOptimizer);
							assemblyInfo.JitTracking = assemblyFlags.HasFlag(AssemblyNameFlags.EnableJITcompileTracking);
							break;
					}
				}
			}

			return assemblyInfo;
		}

		/// <summary>
		/// Merge assembly info in file with the assembly info model
		/// </summary>
		/// <param name="assemblyInfo">Assembly info model</param>
		/// <param name="fileName">Source file name</param>
		public void MergeAssemblyInfo(AssemblyInfo assemblyInfo, string fileName)
		{
			string content;

			using (var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
			{
				content = MergeAssemblyInfo(assemblyInfo, fileStream);
			}

			if (!string.IsNullOrEmpty(content))
				File.WriteAllText(fileName, content);
		}

		/// <summary>
		/// Merge assembly info from the stream with the assembly info model
		/// </summary>
		/// <param name="assemblyInfo">Assembly info model</param>
		/// <param name="inputStream">Source file name</param>
		/// <returns>Result assembly info file content</returns>
		public string MergeAssemblyInfo(AssemblyInfo assemblyInfo, Stream inputStream)
		{
			var syntaxTree = ReadSyntaxTree(inputStream);

			if (syntaxTree == null)
				throw new Exception("Can't read assembly info syntax tree.");

			// Add missing namespaces
			AddNamespaceUsingIfNotExist(syntaxTree, "System");
			AddNamespaceUsingIfNotExist(syntaxTree, "System.Reflection");

			if (assemblyInfo.NeutralLanguage != null)
				AddNamespaceUsingIfNotExist(syntaxTree, "System.Resources");

			if (assemblyInfo.Guid.HasValue || assemblyInfo.ComVisible)
				AddNamespaceUsingIfNotExist(syntaxTree, "System.Runtime.InteropServices");

			// Update assembly info attributes
			SetAttributeValueOrAddAttributeIfNotDefault(syntaxTree, AssemblyTitle, assemblyInfo.Title);
			SetAttributeValueOrAddAttributeIfNotDefault(syntaxTree, AssemblyDescription, assemblyInfo.Description);
			SetAttributeValueOrAddAttributeIfNotDefault(syntaxTree, AssemblyCompany, assemblyInfo.Company);
			SetAttributeValueOrAddAttributeIfNotDefault(syntaxTree, AssemblyProduct, assemblyInfo.Product);
			SetAttributeValueOrAddAttributeIfNotDefault(syntaxTree, AssemblyCopyright, assemblyInfo.Copyright);
			SetAttributeValueOrAddAttributeIfNotDefault(syntaxTree, AssemblyTrademark, assemblyInfo.Trademark);
			SetAttributeValueOrAddAttributeIfNotDefault(syntaxTree, AssemblyDefaultAlias, assemblyInfo.DefaultAlias);
			SetAttributeValueOrAddAttributeIfNotDefault(syntaxTree, AssemblyVersion, assemblyInfo.AssemblyVersion, null, true);
			SetAttributeValueOrAddAttributeIfNotDefault(syntaxTree, AssemblyFileVersion, assemblyInfo.AssemblyFileVersion, null, true);
			SetAttributeValueOrAddAttributeIfNotDefault(syntaxTree, AssemblyInformationalVersion, assemblyInfo.InformationalVersion);
			SetAttributeValueOrAddAttributeIfNotDefault(syntaxTree, Guid, assemblyInfo.Guid, null, true);
			SetAttributeValueOrAddAttributeIfNotDefault(syntaxTree, NeutralResourcesLanguage, assemblyInfo.NeutralLanguage);
			SetAttributeValueOrAddAttributeIfNotDefault(syntaxTree, ComVisible, assemblyInfo.ComVisible, false);
			SetAttributeValueOrAddAttributeIfNotDefault(syntaxTree, ClsCompliant, assemblyInfo.ClsCompliant, false);

			AssemblyNameFlags assemblyFlags = AssemblyNameFlags.PublicKey;
			if (!assemblyInfo.JitOptimization)
				assemblyFlags = assemblyFlags | AssemblyNameFlags.EnableJITcompileOptimizer;
			if (assemblyInfo.JitTracking)
				assemblyFlags = assemblyFlags | AssemblyNameFlags.EnableJITcompileTracking;

			var flagsExpression = GetAssemblyFlagsExpression(assemblyFlags);

			SetAttributeValueOrAddAttributeIfNotDefault(syntaxTree, AssemblyFlags, (int)assemblyFlags, (int)AssemblyNameFlags.PublicKey, flagsExpression);

			return syntaxTree.ToString();
		}

		private SyntaxTree ReadSyntaxTree(Stream stream)
		{
			using (var streamReader = new StreamReader(stream))
			{
				var codeParser = new CSharpParser();
				return codeParser.Parse(streamReader);
			}
		}

		private T GetAttributeValue<T>(Attribute attribute, T defaultValue = default(T))
		{
			var attributeArguments = attribute.Arguments.OfType<PrimitiveExpression>().ToArray();
			if (attributeArguments.Length == 1 && attributeArguments[0].Value is T)
				return (T)attributeArguments[0].Value;

			return defaultValue;
		}

		private Guid? GetAttributeValueAsGuid(Attribute attribute)
		{
			var attributeArguments = attribute.Arguments.OfType<PrimitiveExpression>().ToArray();
			if (attributeArguments.Length == 1)
			{
				var guidString = attributeArguments[0].Value as string;
				if (!string.IsNullOrEmpty(guidString))
				{
					Guid guid;
					if (System.Guid.TryParse(guidString, out guid))
					{
						return guid;
					}
				}
			}

			return null;
		}

		private AssemblyNameFlags GetAssemblyFlagsFromAttribute(Attribute attribute)
		{
			if (attribute.Arguments.Count == 1)
				return GetAssemblyFlagsFromExpression(attribute.Arguments.ElementAt(0));

			return AssemblyNameFlags.PublicKey;
		}

		private AssemblyNameFlags GetAssemblyFlagsFromExpression(Expression expression)
		{
			if (expression == null)
				return AssemblyNameFlags.PublicKey;

			var primitiveExpression = expression as PrimitiveExpression;
			if (primitiveExpression != null && primitiveExpression.Value is int)
				return (AssemblyNameFlags)(int)primitiveExpression.Value;

			var memberReferenceExpression = expression as MemberReferenceExpression;
			if (memberReferenceExpression != null)
			{
				AssemblyNameFlags assemblyFlags;
				if (Enum.TryParse(memberReferenceExpression.MemberName, out assemblyFlags))
				{
					return assemblyFlags;
				}
			}

			var binaryExpression = expression as BinaryOperatorExpression;
			if (binaryExpression != null && binaryExpression.Operator == BinaryOperatorType.BitwiseOr)
				return GetAssemblyFlagsFromExpression(binaryExpression.Left) |
				       GetAssemblyFlagsFromExpression(binaryExpression.Right);

			return AssemblyNameFlags.PublicKey;
		}

		private Expression GetAssemblyFlagsExpression(AssemblyNameFlags assemblyFlags)
		{
			var flagNames = new List<string>();
			var flagValues = Enum.GetValues(typeof(AssemblyNameFlags));

			foreach (var flagValue in flagValues)
			{
				if (assemblyFlags.HasFlag((Enum)flagValue) && (int)flagValue > 0)
					flagNames.Add(flagValue.ToString());
			}

			Expression expression = null;

			while (flagNames.Count > 0)
			{
				var currentFlagName = flagNames[0];
				var flagExpression = new MemberReferenceExpression(new IdentifierExpression("AssemblyNameFlags"), currentFlagName);

				expression = expression == null
					? (Expression) flagExpression
					: new BinaryOperatorExpression(expression, BinaryOperatorType.BitwiseOr, flagExpression);

				flagNames.Remove(currentFlagName);
			}

			return expression;
		}

		private void SetAttributeValueOrAddAttributeIfNotDefault(
			SyntaxTree syntaxTree, 
			string attributeName, 
			object value, 
			object defaultValue = null, 
			bool transformValueToString = false)
		{
			if (value == null)
				return;

			var attributeValue = transformValueToString ? value.ToString() : value;

			SetAttributeValueOrAddAttributeIfNotDefault(
				syntaxTree,
				attributeName,
				value,
				defaultValue,
				new PrimitiveExpression(attributeValue));
		}

		private void SetAttributeValueOrAddAttributeIfNotDefault(
			SyntaxTree syntaxTree,
			string attributeName,
			object value,
			object defaultValue,
			Expression valueExpression)
		{
			if (value == null)
				return;

			var attribute = syntaxTree.Children.OfType<AttributeSection>().SelectMany(x => x.Attributes)
				.FirstOrDefault(x => x.Type is SimpleType && ((SimpleType)x.Type).Identifier == attributeName);

			if (attribute != null)
			{
				attribute.Arguments.Clear();
				attribute.Arguments.Add(valueExpression);
			}
			else if (!value.Equals(defaultValue))
			{
				attribute = new Attribute { Type = new SimpleType(attributeName) };
				attribute.Arguments.Add(valueExpression);

				var attributeSection = new AttributeSection(attribute) { AttributeTarget = "assembly" };
				syntaxTree.AddChild(attributeSection, new NRefactory.Role<AttributeSection>("Member"));
			}
		}

		private void AddNamespaceUsingIfNotExist(SyntaxTree syntaxTree, string @namespace)
		{
			AstNode nodeToInsertAfter = null;

			foreach (var usingDeclaration in syntaxTree.Children.OfType<UsingDeclaration>())
			{
				if (usingDeclaration.Namespace == @namespace)
					return;

				if (string.Compare(usingDeclaration.Namespace, @namespace, StringComparison.InvariantCulture) < 0)
					nodeToInsertAfter = usingDeclaration;
			}

			if (nodeToInsertAfter != null)
				syntaxTree.InsertChildAfter(nodeToInsertAfter, new UsingDeclaration(@namespace), new Role<UsingDeclaration>("Using"));
			else if (syntaxTree.HasChildren)
				syntaxTree.InsertChildBefore(syntaxTree.FirstChild, new UsingDeclaration(@namespace), new Role<UsingDeclaration>("Using"));
			else
				syntaxTree.AddChild(new UsingDeclaration(@namespace), new Role<UsingDeclaration>("Using"));
		}
	}
}