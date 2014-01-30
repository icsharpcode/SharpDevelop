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
using System.Reflection;
using ICSharpCode.NRefactory.Utils;
using Debugger.AddIn.TreeModel;
using Debugger.AddIn.Visualizers.Graph.Layout;
using Debugger.AddIn.Visualizers.Utils;
using Debugger.MetaData;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Services;

namespace Debugger.AddIn.Visualizers.Graph
{
	// The object graph building starts with given expression and recursively
	// explores all its members.
	//
	// Important part of the algorithm is finding if we already have a node
	// for given value - to detect loops and shared references correctly.
	// This is done using the following algorithm:
	// 
	// getNodeForValue(value)
	//   get the hashCode for the value
	//   find if there is already a node with this hashCode (in O(1))
	//     if not, we can be sure we have not seen this value yet
	//     if yes, it might be different object with the same hashCode -> compare addresses
	//
	// 'different object with the same hashCode' are possible - my question on stackoverflow:
	// http://stackoverflow.com/questions/750947/-net-unique-object-identifier
	//
	// This way, the whole graph building is O(n) in the size of the resulting graph.
	// However, evals are still very expensive -> lazy evaluation of only values that are actually seen by user.
	
	/// <summary>
	/// Builds <see cref="ObjectGraph" /> for given string expression.
	/// </summary>
	public class ObjectGraphBuilder
	{
		/// <summary>
		/// The underlying debugger service used for getting expression values.
		/// </summary>
		private WindowsDebugger debuggerService;

		private ObjectGraph resultGraph;
		/// <summary>
		/// Underlying object graph data struture.
		/// </summary>
		public ObjectGraph ResultGraph { get { return this.resultGraph; } }
		
		/// <summary>
		/// Given hash code, lookup already existing node(s) with this hash code.
		/// </summary>
		private MultiDictionary<int, ObjectGraphNode> objectNodesForHashCode = new MultiDictionary<int, ObjectGraphNode>();
		
		/// <summary>
		/// Creates ObjectGraphBuilder.
		/// </summary>
		/// <param name="debuggerService">Debugger service.</param>
		public ObjectGraphBuilder(WindowsDebugger debuggerService)
		{
			this.debuggerService = debuggerService;
		}
		
		/// <summary>
		/// Builds full object graph for given string expression.
		/// </summary>
		/// <param name="expression">Expression valid in the program being debugged (eg. variable name)</param>
		/// <returns>Object graph</returns>
		public ObjectGraph BuildGraphForExpression(GraphExpression expression, ExpandedExpressions expandedNodes)
		{
			if (WindowsDebugger.CurrentStackFrame == null) {
				throw new DebuggerVisualizerException("Please use the visualizer when debugging.");
			}
			
			Value rootValue = expression.GetValue();
			if (rootValue.IsNull) {
				throw new DebuggerVisualizerException(expression + " is null.");
			}
			return buildGraphForValue(rootValue.GetPermanentReference(WindowsDebugger.EvalThread), expression, expandedNodes);
		}
		
		private ObjectGraph buildGraphForValue(Value rootValue, GraphExpression rootExpression, ExpandedExpressions expandedNodes)
		{
			resultGraph = new ObjectGraph();
			//resultGraph.Root = buildGraphRecursive(debuggerService.GetValueFromName(expression).GetPermanentReference(), expandedNodes);
			resultGraph.Root = createNewNode(rootValue, rootExpression);
			loadContent(resultGraph.Root);
			loadNeighborsRecursive(resultGraph.Root, expandedNodes);
			return resultGraph;
		}
		
		public ObjectGraphNode ObtainNodeForExpression(GraphExpression expr)
		{
			bool createdNewNode; // ignored (caller is not interested, otherwise he would use the other overload)
			return ObtainNodeForExpression(expr, out createdNewNode);
		}
		
		public ObjectGraphNode ObtainNodeForExpression(GraphExpression expr, out bool createdNewNode)
		{
			return ObtainNodeForValue(expr.GetValue().GetPermanentReference(WindowsDebugger.EvalThread), expr, out createdNewNode);
		}
		
		/// <summary>
		/// Returns node in the graph that represents given value, or returns new node if not found.
		/// </summary>
		/// <param name="value">Value for which to obtain the node/</param>
		/// <param name="createdNew">True if new node was created, false if existing node was returned.</param>
		public ObjectGraphNode ObtainNodeForValue(Value value, GraphExpression expression, out bool createdNew)
		{
			createdNew = false;
			ObjectGraphNode nodeForValue = getExistingNodeForValue(value);
			if (nodeForValue == null) {
				// if no node for memberValue exists, create it
				nodeForValue = createNewNode(value, expression);
				loadContent(nodeForValue);
				createdNew = true;
			}
			return nodeForValue;
		}
		
		/// <summary>
		/// Fills node Content property tree.
		/// </summary>
		private void loadContent(ObjectGraphNode thisNode)
		{
			var contentRoot = new ThisNode();
			thisNode.Content = contentRoot;
			
			// Object graph visualizer: collection support temp disabled (porting to new NRefactory).
			/*DebugType collectionType;
			DebugType itemType;
			if (thisNode.PermanentReference.Type.ResolveIListImplementation(out collectionType, out itemType))
			{
				//AddRawViewNode(contentRoot, thisNode);
				// it is an IList
				LoadNodeCollectionContent(contentRoot, thisNode.Expression, collectionType);
			} else if (thisNode.PermanentReference.Type.ResolveIEnumerableImplementation(out collectionType, out itemType)) {
				//AddRawViewNode(contentRoot, thisNode);
				// it is an IEnumerable
				DebugType debugListType;
				var debugListExpression = new GraphExpression(
					DebuggerHelpers.CreateDebugListExpression(thisNode.Expression.Expr, itemType, out debugListType),
					() => DebuggerHelpers.CreateListFromIEnumerable(thisNode.Expression.GetValue())
				);
				LoadNodeCollectionContent(contentRoot, debugListExpression, debugListType);
			} else*/ {
				// it is an object
				LoadNodeObjectContent(contentRoot, thisNode.Expression, thisNode.PermanentReference.Type);
			}
		}

		void AddRawViewNode(AbstractNode contentRoot, ObjectGraphNode thisNode) {
			var rawViewNode = new RawViewNode();
			contentRoot.AddChild(rawViewNode);
			LoadNodeObjectContent(rawViewNode, thisNode.Expression, thisNode.PermanentReference.Type);
		}
		
		// Object graph visualizer: collection support temp disabled (porting to new NRefactory).
		/*void LoadNodeCollectionContent(AbstractNode node, GraphExpression thisObject, DebugType iListType)
		{
			var thisObjectAsIList = new GraphExpression(thisObject.Expr.CastToIList(), thisObject.GetValue);
			int listCount = thisObjectAsIList.GetValue().GetIListCount();
			PropertyInfo indexerProp = iListType.GetProperty("Item");
			
			var v = new List<String>();
			
			for (int i = 0; i < listCount; i++)	{
				var itemExpr = new GraphExpression(
					thisObjectAsIList.Expr.AppendIndexer(i),
					() => thisObjectAsIList.GetValue().GetIListItem(i)  // EXPR-EVAL, Does a 'cast' to IList
				);
				PropertyNode itemNode = new PropertyNode(
					new ObjectGraphProperty { Name = "[" + i + "]", MemberInfo = indexerProp, Expression = itemExpr, Value = "", IsAtomic = true, TargetNode = null });
				node.AddChild(itemNode);
			}
		}*/
		
		void LoadNodeObjectContent(AbstractNode node, GraphExpression expression, IType type)
		{
			// base
			var baseType = type.DirectBaseTypes.FirstOrDefault();
			if (baseType != null) {
				var baseClassNode = new BaseClassNode(baseType.FullName, baseType.Name);
				node.AddChild(baseClassNode);
				LoadNodeObjectContent(baseClassNode, expression, baseType);
			}
			
			var members = type.GetFieldsAndNonIndexedProperties(GetMemberOptions.IgnoreInheritedMembers).
				Where(m => !m.IsStatic && !m.IsSynthetic && !m.Name.EndsWith(">k__BackingField")).
				ToList();
			// non-public members
			var nonPublicProperties = createProperties(expression, members.Where(m => !m.IsPublic));
			if (nonPublicProperties.Count > 0) {
				var nonPublicMembersNode = new NonPublicMembersNode();
				node.AddChild(nonPublicMembersNode);
				foreach (var nonPublicProperty in nonPublicProperties) {
					nonPublicMembersNode.AddChild(new PropertyNode(nonPublicProperty));
				}
			}
			
			// public members
			foreach (var property in createProperties(expression, members.Where(m => m.IsPublic))) {
				node.AddChild(new PropertyNode(property));
			}
		}
		
		private List<ObjectGraphProperty> createProperties(GraphExpression expression, IEnumerable<IMember> members)
		{
			List<ObjectGraphProperty> propertyList = new List<ObjectGraphProperty>();
			foreach (IMember member in members) {
				/*if (member.Name.Contains("<")) {
					// skip backing fields
					continue;
				}*/
				// ObjectGraphProperty needs string representation to know whether it is expanded
				var propExpression = new GraphExpression(
					expression.Expr + "." + member.Name,
					() => expression.GetValue().GetMemberValue(WindowsDebugger.EvalThread, member)
				);
				// Value, IsAtomic are lazy evaluated
				propertyList.Add(new ObjectGraphProperty
				                 { Name = member.Name,
				                 	Expression = propExpression, Value = "",
				                 	MemberInfo = member, IsAtomic = true, TargetNode = null });
				
			}
			return propertyList.Sorted(ObjectPropertyComparer.Instance);
		}
		
		/// <summary>
		/// For each complex property of this node, creates a neighbor graph node if needed and connects 
		/// it using to ObjectProperty.TargetNode.
		/// </summary>
		private void loadNeighborsRecursive(ObjectGraphNode thisNode, ExpandedExpressions expandedNodes)
		{
			// evaluate properties first in case property getters are changing some fields - the fields will then have correct values
			foreach(ObjectGraphProperty complexProperty in thisNode.PropertiesFirstThenFields) {
				ObjectGraphNode targetNode = null;
				// We are only evaluating expanded nodes here.
				// We have to do this to know the "shape" of the graph.
				// We do not evaluate atomic and non-expanded properties, those will be lazy evaluated when drawn.
				if (expandedNodes.IsExpanded(complexProperty.Expression.Expr)) {
					// if expanded, evaluate this property
					Value memberValue = complexProperty.Expression.GetValue();
					if (memberValue.IsNull) {
						continue;
					} else {
						// if property value is not null, create neighbor
						memberValue = memberValue.GetPermanentReference(WindowsDebugger.EvalThread);
						
						bool createdNew;
						// get existing node (loop) or create new
						targetNode = ObtainNodeForValue(memberValue, complexProperty.Expression, out createdNew);
						if (createdNew) {
							// if member node is new, recursively build its subtree
							loadNeighborsRecursive(targetNode, expandedNodes);
						}
					}
				}
				// connect property to target ObjectGraphNode
				complexProperty.TargetNode = targetNode;
			}
		}
		
		/// <summary>
		/// Creates new node for the value.
		/// </summary>
		/// <param name="permanentReference">Value, has to be valid.</param>
		/// <returns>New empty object node representing the value.</returns>
		private ObjectGraphNode createNewNode(Value permanentReference, GraphExpression expression)
		{
			if (permanentReference == null)	throw new ArgumentNullException("permanentReference");
			
			ObjectGraphNode newNode = new ObjectGraphNode();
			if (permanentReference.Type != null) {
				newNode.TypeName = permanentReference.Type.FormatNameCSharp();
			}
			newNode.HashCode = permanentReference.InvokeDefaultGetHashCode();
			
			resultGraph.AddNode(newNode);
			// remember this node's hashcode for quick lookup
			objectNodesForHashCode.Add(newNode.HashCode, newNode);
			
			// permanent reference to the object this node represents is useful for graph building,
			// and matching nodes in animations
			newNode.PermanentReference = permanentReference;
			newNode.Expression = expression;
			
			return newNode;
		}
		
		/// <summary>
		/// Finds node that represents the same instance as given value.
		/// </summary>
		/// <param name="value">Valid value representing an instance.</param>
		private ObjectGraphNode getExistingNodeForValue(Value value) 
		{
			int objectHashCode = value.InvokeDefaultGetHashCode();
			// are there any nodes with the same hash code?
			var nodesWithSameHashCode = objectNodesForHashCode[objectHashCode];
			if (nodesWithSameHashCode == null) {
				return null;
			} else {
				// if there is a node with same hash code, check if it has also the same address
				// (hash codes are not uniqe - http://stackoverflow.com/questions/750947/-net-unique-object-identifier)
				ulong objectAddress = value.GetObjectAddress();
				ObjectGraphNode nodeWithSameAddress = 
					nodesWithSameHashCode.FirstOrDefault(node => node.PermanentReference.GetObjectAddress() == objectAddress);
				return nodeWithSameAddress;
			}
		}
	}
}
