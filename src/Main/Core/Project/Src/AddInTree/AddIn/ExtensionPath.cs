using System;
using System.IO;
using System.Collections.Generic;
using System.Xml;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Description of Path.
	/// </summary>
	public class ExtensionPath
	{
		string           name;
		AddIn            addIn;
		List<Codon>      codons         = new List<Codon>();
		Stack<ICondition> conditionStack = new Stack<ICondition>();
		
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
		
		public static void SetUp(ExtensionPath extensionPath, XmlTextReader reader, string endElement)
		{
			while (reader.Read()) {
				switch (reader.NodeType) {
					case XmlNodeType.EndElement:
						if (reader.LocalName == "Condition" || reader.LocalName == "ComplexCondition") {
							extensionPath.conditionStack.Pop();
						} else if (reader.LocalName == endElement) {
							return;
						}
						break;
					case XmlNodeType.Element:
						string elementName = reader.LocalName;
						if (elementName == "Condition") {
							ICondition newCondition = Condition.Read(reader);
							extensionPath.conditionStack.Push(newCondition);
						} else if (elementName == "ComplexCondition") {
							extensionPath.conditionStack.Push(Condition.ReadComplexCondition(reader));
						} else {
							Codon newCodon = new Codon(extensionPath.AddIn, elementName, Properties.ReadFromAttributes(reader), extensionPath.conditionStack.ToArray());
							extensionPath.codons.Add(newCodon);
							if (!reader.IsEmptyElement) {
								ExtensionPath subPath = extensionPath.AddIn.GetExtensionPath(extensionPath.Name + "/" + newCodon.ID);
								foreach (ICondition condition in extensionPath.conditionStack) {
									subPath.conditionStack.Push(condition);
								}
								SetUp(subPath, reader, elementName);
								foreach (ICondition condition in extensionPath.conditionStack) {
									subPath.conditionStack.Pop();
								}
							}
						}
						break;
				}
			}
		}
	}
}
