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
using System.Linq;
using System.Xml;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Represents all contributions to a Path in a single .addin file.
	/// </summary>
	public class ExtensionPath
	{
		string name;
		AddIn addIn;
		List<List<Codon>> codons = new List<List<Codon>>();

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

		public IEnumerable<Codon> Codons {
			get {
				return
					from list in codons
					from c in list
					select c;
			}
		}

		/// <summary>
		/// Gets the codons separated by the groups they were created in.
		/// i.e. if two addins add the codons to the same path they will be in diffrent group.
		/// if the same addin adds the codon in diffrent path elements they will be in diffrent groups.
		/// </summary>
		public IEnumerable<IEnumerable<Codon>> GroupedCodons {
			get {
				return codons.AsReadOnly();
			}
		}

		public ExtensionPath(string name, AddIn addIn)
		{
			this.addIn = addIn;
			this.name = name;
		}

		public static void SetUp(ExtensionPath extensionPath, XmlReader reader, string endElement)
		{
			extensionPath.DoSetUp(reader, endElement, extensionPath.addIn);
		}
		
		void DoSetUp(XmlReader reader, string endElement, AddIn addIn)
		{
			Stack<ICondition> conditionStack = new Stack<ICondition>();
			List<Codon> innerCodons = new List<Codon>();
			while (reader.Read()) {
				switch (reader.NodeType) {
					case XmlNodeType.EndElement:
						if (reader.LocalName == "Condition" || reader.LocalName == "ComplexCondition") {
							conditionStack.Pop();
						} else if (reader.LocalName == endElement) {
							if (innerCodons.Count > 0)
								this.codons.Add(innerCodons);
							return;
						}
						break;
					case XmlNodeType.Element:
						string elementName = reader.LocalName;
						if (elementName == "Condition") {
							conditionStack.Push(Condition.Read(reader, addIn));
						} else if (elementName == "ComplexCondition") {
							conditionStack.Push(Condition.ReadComplexCondition(reader, addIn));
						} else {
							Codon newCodon = new Codon(this.AddIn, elementName, Properties.ReadFromAttributes(reader), conditionStack.ToArray());
							innerCodons.Add(newCodon);
							if (!reader.IsEmptyElement) {
								ExtensionPath subPath = this.AddIn.GetExtensionPath(this.Name + "/" + newCodon.Id);
								subPath.DoSetUp(reader, elementName, addIn);
							}
						}
						break;
				}
			}
			if (innerCodons.Count > 0)
				this.codons.Add(innerCodons);
		}
	}
}
