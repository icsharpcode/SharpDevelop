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
		private Lookup<int, ObjectGraphNode> objectNodesForHashCode = new Lookup<int, ObjectGraphNode>();
		
		/// <summary>
		/// Binding flags for getting member expressions.
		/// </summary>
		private readonly Debugger.MetaData.BindingFlags memberBindingFlags =
			BindingFlags.Public | BindingFlags.Instance | BindingFlags.Field | BindingFlags.GetProperty;
		
		private readonly Debugger.MetaData.BindingFlags nonPublicInstanceMemberFlags =
			BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Field | BindingFlags.GetProperty;
		
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
		public ObjectGraph BuildGraphForExpression(string expression, ExpandedExpressions expandedNodes)
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
		
		public ObjectGraphNode ObtainNodeForExpression(Expression expr)
		{
			bool createdNewNode; // ignored (caller is not interested, otherwise he would use the other overload)
			return ObtainNodeForExpression(expr, out createdNewNode);
		}
		
		public ObjectGraphNode ObtainNodeForExpression(Expression expr, out bool createdNewNode)
		{
			return ObtainNodeForValue(expr.EvalPermanentReference(), out createdNewNode);
		}
		
		/// <summary>
		/// Returns node in the graph that represents given value, or returns new node if not found.
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
			thisNode.Content = new ThisNode();
			ThisNode contentRoot = thisNode.Content;
			
			loadNodeContent(contentRoot, thisNode.PermanentReference, thisNode.PermanentReference.Type);
		}
		
		private void loadNodeContent(AbstractNode node, Value value, DebugType type)
		{
			// base
			if (type.BaseType != null && type.BaseType.FullName != "System.Object")
			{
				var baseClassNode = new BaseClassNode(type.BaseType.FullName, type.BaseType.Name);
				node.AddChild(baseClassNode);
				loadNodeContent(baseClassNode, value, type.BaseType);
			}
			
			// non-public members
			var nonPublicProperties = getProperties(value, type, this.nonPublicInstanceMemberFlags);
			if (nonPublicProperties.Count > 0)
			{
				var nonPublicMembersNode = new NonPublicMembersNode();
				node.AddChild(nonPublicMembersNode);
				foreach (var nonPublicProperty in nonPublicProperties)
				{
					nonPublicMembersNode.AddChild(new PropertyNode(nonPublicProperty));
				}
			}
			
			// public members
			foreach (var property in getPublicProperties(value, type))
			{
				node.AddChild(new PropertyNode(property));
			}
		}
		
		private List<ObjectGraphProperty> getPublicProperties(Value value, DebugType shownType)
		{
			return getProperties(value, shownType, this.memberBindingFlags);
		}
		
		private List<ObjectGraphProperty> getProperties(Value value, DebugType shownType, BindingFlags flags)
		{
			List<ObjectGraphProperty> propertyList = new List<ObjectGraphProperty>();
			
			foreach (PropertyInfo memberProp in shownType.GetProperties(flags))
			{
				if (memberProp.Name.Contains("<"))
					continue;

				// TODO just temporary - ObjectGraphProperty needs an expression
				// to use expanded / nonexpanded (and to evaluate?)
				Expression propExpression = value.Expression.AppendMemberReference(memberProp);
				// Value, IsAtomic are lazy evaluated
				//var t = memberProp.DeclaringType;
				propertyList.Add(new ObjectGraphProperty
				                 { Name = memberProp.Name,
				                 	Expression = propExpression, Value = "",
				                 	/*PropInfo = memberProp,*/ IsAtomic = true, TargetNode = null });
				
			}
			
			return propertyList.Sorted(ObjectPropertyComparer.Instance);
			
			/*// take all properties for this value (type = value's real type)
			foreach(Expression memberExpr in value.Expression.AppendObjectMembers(shownType, flags))
			{
				// skip private backing fields
				if (memberExpr.CodeTail.Contains("<"))
					continue;
				
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
					bool memberIsNull = memberExpr.IsNull();
					propertyList.Add(createComplexProperty(memberName, memberExpr, null, memberIsNull));
				}
			}
			
			return propertyList.Sorted(ObjectPropertyComparer.Instance);*/
		}
		
		/// <summary>
		/// For each complex property of this node, create s neighbor graph node if needed and connects the neighbor to ObjectProperty.TargetNode.
		/// </summary>
		/// <param name="thisNode"></param>
		/// <param name="expandedNodes"></param>
		private void loadNeighborsRecursive(ObjectGraphNode thisNode, ExpandedExpressions expandedNodes)
		{
			//foreach(ObjectGraphProperty complexProperty in thisNode.ComplexProperties)
			foreach(ObjectGraphProperty complexProperty in thisNode.Properties)
			{
				ObjectGraphNode targetNode = null;
				// we are only evaluating expanded nodes here
				// (we have to do this to know the "shape" of the graph)
				// property laziness makes sense, as we are not evaluating atomic and non-expanded properties out of user's view
				if (/*!complexProperty.IsNull && we dont know yet if it's null */expandedNodes.IsExpanded(complexProperty.Expression))
				{
					// if expanded, evaluate this property
					Value memberValue = complexProperty.Expression.Evaluate(this.debuggerService.DebuggedProcess);
					if (memberValue.IsNull)
					{
						continue;
					}
					else
					{
						// if property value is not null, create neighbor
						memberValue = memberValue.GetPermanentReference();
						
						bool createdNew;
						// get existing node (loop) or create new
						targetNode = ObtainNodeForValue(memberValue, out createdNew);
						if (createdNew)
						{
							// if member node is new, recursively build its subtree
							loadNeighborsRecursive(targetNode, expandedNodes);
						}
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
