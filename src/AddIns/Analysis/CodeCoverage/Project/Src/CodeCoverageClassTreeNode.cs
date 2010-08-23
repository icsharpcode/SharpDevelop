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
		public CodeCoverageClassTreeNode(string name, List<CodeCoverageMethod> methods)
			: base(name, methods, CodeCoverageImageListIndex.Class)
		{
		}

		public override void ActivateItem()
		{
			foreach (CodeCoverageTreeNode node in Nodes) {
				CodeCoverageMethodTreeNode methodNode = node as CodeCoverageMethodTreeNode;
				CodeCoverageMethodsTreeNode methodsNode = node as CodeCoverageMethodsTreeNode;
				
				bool openedFile = false;
				if (methodNode != null) {
					openedFile = OpenFile(methodNode.Method.SequencePoints);
				} else if ((methodsNode != null) && (methodsNode.Methods.Count > 0)) {
					openedFile = OpenFile(methodsNode.Methods[0].SequencePoints);
				}
				
				if (openedFile) {
					break;
				}
			}
		}
		
		bool OpenFile(List<CodeCoverageSequencePoint> sequencePoints)
		{
			foreach (CodeCoverageSequencePoint point in sequencePoints) {
				if (point.HasDocument()) {
					OpenFile(point.Document);
					return true;
				}
			}
			return false;
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
			
			// Add properties.s
			foreach (CodeCoverageProperty property in properties) {
				CodeCoveragePropertyTreeNode node = new CodeCoveragePropertyTreeNode(property);
				node.AddTo(this);
			}
			
			// Sort nodes.
			SortChildNodes();
		}
	}
}
