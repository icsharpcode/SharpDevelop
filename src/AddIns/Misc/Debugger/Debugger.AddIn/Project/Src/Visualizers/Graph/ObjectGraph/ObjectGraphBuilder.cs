// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníèek" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Services;
using Debugger;
using Debugger.MetaData;
using Debugger.Expressions;
using Debugger.AddIn.Visualizers.Utils;
using Debugger.AddIn.Visualizers.Common;

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
	// this way, the whole graph building is O(n) in the size of the resulting graph
	// It tries to call as few Expression.Evaluate as possible, since Expression.Evaluate is expensive (5ms) -
	// this is a prototype and the speed can be I believe greatly improved for future versions.
	
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
		private Lookup<int, ObjectGraphNode> objectNodesForHashCode = new Lookup<int, ObjectGraphNode>();
		
		/// <summary>
		/// Binding flags for getting member expressions.
		/// </summary>
		private readonly Debugger.MetaData.BindingFlags memberBindingFlags =
			BindingFlags.Public | BindingFlags.Instance | BindingFlags.Field | BindingFlags.GetProperty;
		
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
		public ObjectGraph BuildGraphForExpression(string expression, ExpandedNodes expandedNodes)
		{
			if (string.IsNullOrEmpty(expression))
			{
				throw new DebuggerVisualizerException("Expression is empty.");
			}
			
			resultGraph = new ObjectGraph();
			Value rootValue = debuggerService.GetValueFromName(expression);
			if (rootValue == null)
			{
				throw new DebuggerVisualizerException("Please use the visualizer when debugging.");
			}
			
			// empty graph for null expression
			if (!rootValue.IsNull)
			{
				//resultGraph.Root = buildGraphRecursive(debuggerService.GetValueFromName(expression).GetPermanentReference(), expandedNodes);
				resultGraph.Root = createNewNode(debuggerService.GetValueFromName(expression).GetPermanentReference());
				loadContent(resultGraph.Root);
				loadNeighborsRecursive(resultGraph.Root, expandedNodes);
			}
			
			return resultGraph;
		}
		
		public ObjectGraphNode ObtainNodeForExpression(Expression expr, out bool createdNewNode)
		{
			return ObtainNodeForValue(expr.GetPermanentReference(), out createdNewNode);
		}
		
		public ObjectGraphNode ObtainNodeForExpression(Expression expr)
		{
			bool createdNewNode; // ignored (caller is not interested, otherwise he would use the other overload)
			return ObtainNodeForExpression(expr, out createdNewNode);
		}
		
		/// <summary>
		/// Returns node in the graph that represents given value, or returns new node if no node found.
		/// </summary>
		/// <param name="value">Value for which to obtain the node/</param>
		/// <param name="createdNew">True if new node was created, false if existing node was returned.</param>
		public ObjectGraphNode ObtainNodeForValue(Value value, out bool createdNew)
		{
			createdNew = false;
			ObjectGraphNode nodeForValue = getExistingNodeForValue(value);
			if (nodeForValue == null)
			{
				// if no node for memberValue exists, create it
				nodeForValue = createNewNode(value);
				loadContent(nodeForValue);
				createdNew = true;
			}
			return nodeForValue;
		}
		
		/// <summary>
		/// Fills node Content property tree.
		/// </summary>
		/// <param name="thisNode"></param>
		private void loadContent(ObjectGraphNode thisNode)
		{
			thisNode.Content = new NestedNode(NestedNodeType.ThisNode);
			NestedNode contentRoot = thisNode.Content;
			
			DebugType thisType = thisNode.PermanentReference.Type;
			
			// recursive, remove NestedNodeType enum
			if (thisType.BaseType != null && thisType.BaseType.FullName != "System.Object")
			{
				var baseClassNode = contentRoot.AddChild(new NestedNode(NestedNodeType.BaseClassNode));
				foreach (var baseProperty in getPublicProperties(thisNode.PermanentReference, thisType.BaseType))
				{
					baseClassNode.AddChild(new PropertyNode(baseProperty));
				}
			}
			
			foreach (var property in getPublicProperties(thisNode.PermanentReference, thisType))
			{
				contentRoot.AddChild(new PropertyNode(property));
			}
		}
		
		private List<ObjectGraphProperty> getPublicProperties(Value value, DebugType shownType)
		{
			List<ObjectGraphProperty> propertyList = new List<ObjectGraphProperty>();
			
			// take all properties for this value (type = value's real type)
			foreach(Expression memberExpr in value.Expression.AppendObjectMembers(shownType, this.memberBindingFlags))
			{
				checkIsOfSupportedType(memberExpr);
				
				string memberName = memberExpr.CodeTail;
				if (memberExpr.IsOfAtomicType())
				{
					// properties are now lazy-evaluated
					//  string memberValueAsString = memberExpr.Evaluate(debuggerService.DebuggedProcess).InvokeToString();
					propertyList.Add(createAtomicProperty(memberName, memberExpr));
				}
				else
				{
					ObjectGraphNode targetNode = null;
					bool memberIsNull = memberExpr.IsNull();
					propertyList.Add(createComplexProperty(memberName, memberExpr, targetNode, memberIsNull));
				}
			}
			
			return propertyList.Sorted(ObjectPropertyComparer.Instance);
		}
		
		/// <summary>
		/// For each complex property of this node, create s neighbor graph node if needed and connects the neighbor to ObjectProperty.TargetNode.
		/// </summary>
		/// <param name="thisNode"></param>
		/// <param name="expandedNodes"></param>
		private void loadNeighborsRecursive(ObjectGraphNode thisNode, ExpandedNodes expandedNodes)
		{
			foreach(ObjectGraphProperty complexProperty in thisNode.ComplexProperties)
			{
				Expression memberExpr = complexProperty.Expression;
				ObjectGraphNode targetNode = null;
				// we are only evaluating expanded nodes here 
				// (we have to do this to know the "shape" of the graph)
				// property laziness makes sense, as we are not evaluating atomic and non-expanded properties out of user's view
				if (!complexProperty.IsNull && expandedNodes.IsExpanded(memberExpr.Code))
				{
					Value memberValue = memberExpr.GetPermanentReference();
					
					bool createdNew;
					// get existing node (loop) or create new
					targetNode = ObtainNodeForValue(memberValue, out createdNew);
					if (createdNew)
					{
						// if member node is new, recursively build its subtree
						loadNeighborsRecursive(targetNode, expandedNodes);
					}
				}
				else
				{
					targetNode = null;
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
		private ObjectGraphNode createNewNode(Value permanentReference)
		{
			ObjectGraphNode newNode = new ObjectGraphNode();
			newNode.HashCode = permanentReference.InvokeDefaultGetHashCode();
			
			resultGraph.AddNode(newNode);
			// remember this node's hashcode for quick lookup
			objectNodesForHashCode.Add(newNode.HashCode, newNode);
			
			// permanent reference to the object this node represents is useful for graph building,
			// and matching nodes in animations
			newNode.PermanentReference = permanentReference;
			
			return newNode;
		}
		
		/// <summary>
		/// Finds node that represents the same instance as given value.
		/// </summary>
		/// <param name="value">Valid value representing an instance.</param>
		/// <returns></returns>
		private ObjectGraphNode getExistingNodeForValue(Value value)
		{
			int objectHashCode = value.InvokeDefaultGetHashCode();
			// are there any nodes with the same hash code?
			LookupValueCollection<ObjectGraphNode> nodesWithSameHashCode = objectNodesForHashCode[objectHashCode];
			if (nodesWithSameHashCode == null)
			{
				return null;
			}
			else
			{
				// if there is a node with same hash code, check if it has also the same address
				// (hash codes are not uniqe - http://stackoverflow.com/questions/750947/-net-unique-object-identifier)
				ulong objectAddress = value.GetObjectAddress();
				ObjectGraphNode nodeWithSameAddress = nodesWithSameHashCode.Find(
					node => { return node.PermanentReference.GetObjectAddress() == objectAddress; } );
				return nodeWithSameAddress;
			}
		}
		
		public ObjectGraphProperty createAtomicProperty(string name, Expression expression)
		{
			// value is empty (will be lazy-evaluated later)
			return new ObjectGraphProperty
			         { Name = name, Value = "", Expression = expression, IsAtomic = true, TargetNode = null };
		}
		
		public ObjectGraphProperty createComplexProperty(string name, Expression expression, ObjectGraphNode targetNode, bool isNull)
		{
			// value is empty (will be lazy-evaluated later)
			return new ObjectGraphProperty
			         { Name = name, Value = "", Expression = expression, IsAtomic = false, TargetNode = targetNode, IsNull = isNull };
		}
		
		/// <summary>
		/// Checks whether given expression's type is supported by the graph builder.
		/// </summary>
		/// <param name="expr">Expression to be checked.</param>
		private void checkIsOfSupportedType(Expression expr)
		{
			DebugType typeOfValue = expr.Evaluate(debuggerService.DebuggedProcess).Type;
			if (typeOfValue.IsArray)
			{
				// arrays will be supported of course in the final version
				throw new DebuggerVisualizerException("Arrays are not supported yet");
			}
		}
	}
}
