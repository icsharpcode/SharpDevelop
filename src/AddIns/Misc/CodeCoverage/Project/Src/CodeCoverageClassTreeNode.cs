// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.CodeCoverage
{
	public class CodeCoverageClassTreeNode : CodeCoverageMethodsTreeNode
	{
		public CodeCoverageClassTreeNode(string name, List<CodeCoverageMethod> methods) : base(name, methods, CodeCoverageImageListIndex.Class)
		{
		}

		public override void ActivateItem()
		{
			if (Nodes.Count > 0) {
				CodeCoverageMethodTreeNode methodNode = Nodes[0] as CodeCoverageMethodTreeNode;
				if (methodNode != null && methodNode.Method.SequencePoints.Count > 0) {
					FileService.OpenFile(methodNode.Method.SequencePoints[0].Document);
				}
				// when the first node is a property:
				CodeCoverageMethodsTreeNode methodsNode = Nodes[0] as CodeCoverageMethodsTreeNode;
				if (methodsNode != null && methodsNode.Methods.Count > 0) {
					var sequencePoints = methodsNode.Methods[0].SequencePoints;
					if (sequencePoints != null) {
						FileService.OpenFile(sequencePoints[0].Document);
					}
				}
			}
		}
		
		protected override void Initialize()
		{
			Nodes.Clear();

			// Add methods.
			CodeCoveragePropertyCollection properties = new CodeCoveragePropertyCollection();
			foreach (CodeCoverageMethod method in Methods) {
				if (method.IsProperty) {
					properties.Add(method);
				} else {
					CodeCoverageMethodTreeNode node = new CodeCoverageMethodTreeNode(method);
					node.AddTo(this);
				}
			}
			
			// Add properties.
			foreach (CodeCoverageProperty property in properties) {
				CodeCoveragePropertyTreeNode node = new CodeCoveragePropertyTreeNode(property);
				node.AddTo(this);
			}
			
			// Sort nodes.
			SortChildNodes();
		}
	}
}
