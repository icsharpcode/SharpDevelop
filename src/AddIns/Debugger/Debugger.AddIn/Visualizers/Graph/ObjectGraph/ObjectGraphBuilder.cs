// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using System.Collections.Generic;
using System.Reflection;
using Debugger.AddIn.TreeModel;
using Debugger.AddIn.Visualizers.Graph.Layout;
using Debugger.AddIn.Visualizers.Utils;
using Debugger.MetaData;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.Visitors;
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
		private Lookup<int, ObjectGraphNode> objectNodesForHashCode = new Lookup<int, ObjectGraphNode>();
		
		/// <summary>
		/// Binding flags for getting member expressions.
		/// </summary>
		private readonly BindingFlags publicInstanceMemberFlags =
			BindingFlags.Public | BindingFlags.Instance;
		
		private readonly BindingFlags nonPublicInstanceMemberFlags =
			BindingFlags.NonPublic | BindingFlags.Instance;
		
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
			if (string.IsNullOrEmpty(expression)) {
				throw new DebuggerVisualizerException("Please specify an expression.");
			}

			var debuggedProcess = this.debuggerService.DebuggedProcess;
			if (debuggedProcess == null || debuggedProcess.IsRunning || debuggedProcess.SelectedStackFrame == null) {
				throw new DebuggerVisualizerException("Please use the visualizer when debugging.");
			}
			
			var rootExpression = ExpressionEvaluator.ParseExpression(expression, SupportedLanguage.CSharp);
			Value rootValue = rootExpression.Evaluate(debuggedProcess);
			if (rootValue.IsNull)	{
				throw new DebuggerVisualizerException(expression + " is null.");
			}
			return buildGraphForValue(rootValue.GetPermanentReference(), rootExpression, expandedNodes);
		}
		
		private ObjectGraph buildGraphForValue(Value rootValue, Expression rootExpression, ExpandedExpressions expandedNodes)
		{
			resultGraph = new ObjectGraph();
			//resultGraph.Root = buildGraphRecursive(debuggerService.GetValueFromName(expression).GetPermanentReference(), expandedNodes);
			resultGraph.Root = createNewNode(rootValue, rootExpression);
			loadContent(resultGraph.Root);
			loadNeighborsRecursive(resultGraph.Root, expandedNodes);
			return resultGraph;
		}
		
		public ObjectGraphNode ObtainNodeForExpression(Expression expr)
		{
			bool createdNewNode; // ignored (caller is not interested, otherwise he would use the other overload)
			return ObtainNodeForExpression(expr, out createdNewNode);
		}
		
		public ObjectGraphNode ObtainNodeForExpression(Expression expr, out bool createdNewNode)
		{
			return ObtainNodeForValue(expr.EvalPermanentReference(), expr, out createdNewNode);
		}
		
		/// <summary>
		/// Returns node in the graph that represents given value, or returns new node if not found.
		/// </summary>
		/// <param name="value">Value for which to obtain the node/</param>
		/// <param name="createdNew">True if new node was created, false if existing node was returned.</param>
		public ObjectGraphNode ObtainNodeForValue(Value value, Expression expression, out bool createdNew)
		{
			createdNew = false;
			ObjectGraphNode nodeForValue = getExistingNodeForValue(value);
			if (nodeForValue == null)
			{
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
		/// <param name="thisNode"></param>
		private void loadContent(ObjectGraphNode thisNode)
		{
			var contentRoot = new ThisNode();
			thisNode.Content = contentRoot;
			
			DebugType collectionType;
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
				var debugListExpression = DebuggerHelpers.CreateDebugListExpression(thisNode.Expression, itemType, out debugListType);
				LoadNodeCollectionContent(contentRoot, debugListExpression, debugListType);
			} else {
				// it is an object
				LoadNodeObjectContent(contentRoot, thisNode.Expression, thisNode.PermanentReference.Type);
			}
		}

		void AddRawViewNode(AbstractNode contentRoot, ObjectGraphNode thisNode)
		{
			var rawViewNode = new RawViewNode();
			contentRoot.AddChild(rawViewNode);
			LoadNodeObjectContent(rawViewNode, thisNode.Expression, thisNode.PermanentReference.Type);
		}
		
		void LoadNodeCollectionContent(AbstractNode node, Expression thisObject, DebugType iListType)
		{
			thisObject = thisObject.CastToIList();
			int listCount = thisObject.GetIListCount();
			PropertyInfo indexerProp = iListType.GetProperty("Item");
			
			for (int i = 0; i < listCount; i++)	{
				Expression itemExpr = thisObject.AppendIndexer(i);
				PropertyNode itemNode = new PropertyNode(
					new ObjectGraphProperty { Name = "[" + i + "]", MemberInfo = indexerProp, Expression = itemExpr, Value = "", IsAtomic = true, TargetNode = null });
				node.AddChild(itemNode);
			}
		}
		
		void LoadNodeObjectContent(AbstractNode node, Expression expression, DebugType type)
		{
			// base
			if (type.BaseType != null && type.BaseType.FullName != "System.Object")
			{
				var baseClassNode = new BaseClassNode(type.BaseType.FullName, type.BaseType.Name);
				node.AddChild(baseClassNode);
				LoadNodeObjectContent(baseClassNode, expression, (DebugType)type.BaseType);
			}
			
			// non-public members
			var nonPublicProperties = getProperties(expression, type, this.nonPublicInstanceMemberFlags);
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
			foreach (var property in getProperties(expression, type, this.publicInstanceMemberFlags))
			{
				node.AddChild(new PropertyNode(property));
			}
		}
		
		private List<ObjectGraphProperty> getProperties(Expression expression, DebugType shownType, BindingFlags flags)
		{
			List<ObjectGraphProperty> propertyList = new List<ObjectGraphProperty>();
			
			foreach (MemberInfo memberProp in shownType.GetFieldsAndNonIndexedProperties(flags))
			{
				if (memberProp.Name.Contains("<")) {
					// skip backing fields
					continue;
				}
				if (memberProp.DeclaringType != shownType) {
					// skip properties declared in the base type
					continue;
				}

				// ObjectGraphProperty needs an expression
				// to know whether it is expanded, and to evaluate
				Expression propExpression = expression.AppendMemberReference((IDebugMemberInfo)memberProp);
				// Value, IsAtomic are lazy evaluated
				propertyList.Add(new ObjectGraphProperty
				                 { Name = memberProp.Name,
				                 	Expression = propExpression, Value = "",
				                 	MemberInfo = memberProp, IsAtomic = true, TargetNode = null });
				
			}
			return propertyList.Sorted(ObjectPropertyComparer.Instance);
		}
		
		/// <summary>
		/// For each complex property of this node, create s neighbor graph node if needed and connects the neighbor to ObjectProperty.TargetNode.
		/// </summary>
		/// <param name="thisNode"></param>
		/// <param name="expandedNodes"></param>
		private void loadNeighborsRecursive(ObjectGraphNode thisNode, ExpandedExpressions expandedNodes)
		{
			// evaluate properties first in case property getters are changing some fields - the fields will then have correct values
			foreach(ObjectGraphProperty complexProperty in thisNode.PropertiesFirstThenFields)
			{
				ObjectGraphNode targetNode = null;
				// We are only evaluating expanded nodes here.
				// We have to do this to know the "shape" of the graph.
				// We do not evaluate atomic and non-expanded properties, those will be lazy evaluated when drawn.
				if (expandedNodes.IsExpanded(complexProperty.Expression))
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
						targetNode = ObtainNodeForValue(memberValue, complexProperty.Expression, out createdNew);
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
		private ObjectGraphNode createNewNode(Value permanentReference, Expression expression)
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
				throw new DebuggerVisualizerException("Arrays are not supported yet");
			}
		}
	}
}
