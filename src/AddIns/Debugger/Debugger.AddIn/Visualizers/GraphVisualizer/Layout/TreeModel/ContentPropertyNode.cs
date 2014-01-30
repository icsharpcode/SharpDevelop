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
using System.ComponentModel;
using Debugger.AddIn.TreeModel;
using Debugger.AddIn.Visualizers.Graph.Drawing;
using Debugger.MetaData;
using ICSharpCode.SharpDevelop;

namespace Debugger.AddIn.Visualizers.Graph.Layout
{
	/// <summary>
	/// ViewModel for property node in tree of properties, to be bound to View (PositionedGraphNodeControl).
	/// </summary>
	public class ContentPropertyNode : ContentNode, IEvaluate
	{
		public ContentPropertyNode(PositionedNode containingNode, ContentNode parent)
			: base(containingNode, parent)
		{
		}
		
		/// <summary>
		/// The PositionedNodeProperty this node contains.
		/// </summary>
		public PositionedNodeProperty Property { get; private set; }
		
		public bool IsEvaluated
		{
			get { return this.Property.IsEvaluated; }
		}
		
		public override bool IsPropertyExpanded
		{
			get { return this.Property.IsPropertyExpanded; }
			set { this.Property.IsPropertyExpanded = value; }
		}
		
		public void Evaluate()
		{
			this.Property.Evaluate();
			this.Text = this.Property.Value;
		}

		public override bool ShowExpandPropertyButton
		{
			get
			{
				// show expand button for non-null non-atomic objects
				return (!this.Property.IsAtomic && !this.Property.IsNull);
			}
		}
		
		public override void InitOverride(AbstractNode source, Expanded expanded)
		{
			if (!(source is PropertyNode))
				throw new InvalidOperationException(string.Format("{0} must initialize from {1}", typeof(ContentPropertyNode).Name, typeof(PropertyNode).Name));
			
			PropertyNode sourcePropertyNode = source as PropertyNode;
			
			this.Name = sourcePropertyNode.Property.Name;
			// Important to set Text here, as we might be just building new view over existing (evaluated) model.
			// If the model is not evaluated yet, this will be string.Empty and filled in Evaluate().
			this.Text = sourcePropertyNode.Property.Value;
			this.IsNested = false;
			this.IsExpanded = false;			// always false, Property nodes are never expanded (they have IsPropertyExpanded)
			this.Property = new PositionedNodeProperty(
				sourcePropertyNode.Property, this.ContainingNode,
				expanded.Expressions.IsExpanded(sourcePropertyNode.Property.Expression.Expr));
			if (PositionedGraphNodeControl.IsShowMemberIcon) {
				EvalMemberIcon();
			}
		}
		
		void EvalMemberIcon()
		{
			// TODO reimplement
			/*if ((this.Property != null) && (this.Property.ObjectGraphProperty != null)) {
				var memberInfo = (IDebugMemberInfo)this.Property.ObjectGraphProperty.MemberInfo;
				if (memberInfo != null) {
					var image = new ResourceServiceImage(ValueNode.GetImageForMember(memberInfo));
					this.MemberIcon = image.ImageSource;
				}
			}*/
		}
	}
}
