// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Xml;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Description of Path.
	/// </summary>
	public class ExtensionPath
	{
		string      name;
		AddIn       addIn;
		List<Codon> codons = new List<Codon>();
		
		public AddIn AddIn {
			get {
				return addIn;
			}
		}
		
		public string Name {
			get {
				return name;
			}
		}
		public List<Codon> Codons {
			get {
				return codons;
			}
		}
		
		public ExtensionPath(string name, AddIn addIn)
		{
			this.addIn = addIn;
			this.name = name;
		}
		
		public static void SetUp(ExtensionPath extensionPath, XmlReader reader, string endElement)
		{
			Stack<ICondition> conditionStack = new Stack<ICondition>();
			while (reader.Read()) {
				switch (reader.NodeType) {
					case XmlNodeType.EndElement:
						if (reader.LocalName == "Condition" || reader.LocalName == "ComplexCondition") {
							conditionStack.Pop();
						} else if (reader.LocalName == endElement) {
							return;
						}
						break;
					case XmlNodeType.Element:
						string elementName = reader.LocalName;
						if (elementName == "Condition") {
							conditionStack.Push(Condition.Read(reader));
						} else if (elementName == "ComplexCondition") {
							conditionStack.Push(Condition.ReadComplexCondition(reader));
						} else {
							Codon newCodon = new Codon(extensionPath.AddIn, elementName, Properties.ReadFromAttributes(reader), conditionStack.ToArray());
							extensionPath.codons.Add(newCodon);
							if (!reader.IsEmptyElement) {
								ExtensionPath subPath = extensionPath.AddIn.GetExtensionPath(extensionPath.Name + "/" + newCodon.Id);
								//foreach (ICondition condition in extensionPath.conditionStack) {
								//	subPath.conditionStack.Push(condition);
								//}
								SetUp(subPath, reader, elementName);
								//foreach (ICondition condition in extensionPath.conditionStack) {
								//	subPath.conditionStack.Pop();
								//}
							}
						}
						break;
				}
			}
		}
	}
}
