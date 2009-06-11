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
using Debugger.AddIn.Visualizers.Graph.Utils;

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
		/// <summary>
		/// The resulting object graph.
		/// </summary>
		private ObjectGraph resultGraph;
		/// <summary>
		/// System.Runtime.CompilerServices.GetHashCode method, for obtaining non-overriden hash codes from debuggee.
		/// </summary>
		private MethodInfo hashCodeMethod;
		
		/// <summary>
		/// Given hash code, lookup already existing node(s) with this hash code.
		/// </summary>
		private Lookup<int, ObjectNode> objectNodesForHashCode = new Lookup<int, ObjectNode>();
		
		/// <summary>
		/// Binding flags for getting member expressions.
		/// </summary>
		private readonly Debugger.MetaData.BindingFlags _bindingFlags = 
			BindingFlags.Public | BindingFlags.Instance | BindingFlags.Field | BindingFlags.GetProperty;
		
		/// <summary>
		/// Creates ObjectGraphBuilder.
		/// </summary>
		/// <param name="debuggerService">Debugger service.</param>
		public ObjectGraphBuilder(WindowsDebugger debuggerService)
		{
			this.debuggerService = debuggerService;
			
			DebugType helpersType =  DebugType.Create(debuggerService.DebuggedProcess, null, "System.Runtime.CompilerServices.RuntimeHelpers");
			this.hashCodeMethod = helpersType.GetMember("GetHashCode", BindingFlags.Public | BindingFlags.Static | BindingFlags.Method | BindingFlags.IncludeSuperType) as MethodInfo;
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
				throw new DebuggerVisualizerException("Expression cannot be empty.");
			}
			
			resultGraph = new ObjectGraph();
			
			// empty graph for null expression
			if (!debuggerService.GetValueFromName(expression).IsNull)
			{
				resultGraph.Root = buildGraphRecursive(debuggerService.GetValueFromName(expression).GetPermanentReference(), expandedNodes);
			}
			
			return resultGraph;
		}
		
		/// <summary>
		/// Builds the subgraph representing given value.
		/// </summary>
		/// <param name="rootValue">The Value for which the subgraph will be built.</param>
		/// <returns>ObjectNode representing the value + all recursive members.</returns>
		private ObjectNode buildGraphRecursive(Value rootValue, ExpandedNodes expandedNodes)
		{
			ObjectNode thisNode = createNewNode(rootValue); 
			
			// David: by calling this, we get an array of values, most of them probably invalid,
			// it would be nice to be able to get a collection of 'values' 
			// that are valid (basically a snapshot of object's state)
			// - a collection of custom objects, 
			// that contain the string value and DebugType,
			// and can enumerate child values.
			// It would be also nice to return IEnumerable or ReadonlyCollection
			// http://blogs.msdn.com/ericlippert/archive/2008/09/22/arrays-considered-somewhat-harmful.aspx
			/*string[] memberValues = rootValue.GetMemberValuesAsString(_bindingFlags);
			foreach	(string memberValue in memberValues)
			{
				//Value memberValuePerm = memberValue.GetPermanentReference();
				
			}*/
			
			foreach(Expression memberExpr in rootValue.Expression.AppendObjectMembers(rootValue.Type, _bindingFlags))
			{
				checkIsOfSupportedType(memberExpr);
				
				string memberName = memberExpr.CodeTail;
				if (isOfAtomicType(memberExpr))
				{
					// atomic members are added to the list of node's "properties"
					string memberValueAsString = memberExpr.Evaluate(debuggerService.DebuggedProcess).AsString;
				    thisNode.AddAtomicProperty(memberName, memberValueAsString, memberExpr);
				}
				else
				{
					ObjectNode targetNode = null;
					if (!isNull(memberExpr) && expandedNodes.IsExpanded(memberExpr.Code))
					{
						// for object members, edges are added
						Value memberValue = getPermanentReference(memberExpr);
						
						// if node for memberValue already exists, only add edge to it (so loops etc. are solved correctly)
						targetNode = getNodeForValue(memberValue);
						if (targetNode == null)
						{
							// if no node for memberValue exists, build the subgraph for the value
							targetNode = buildGraphRecursive(memberValue, expandedNodes);
						}
					}
					else 
					{
						targetNode = null;
					}
					thisNode.AddComplexProperty(memberName, "", memberExpr, targetNode);
				}
			}
			
			return thisNode;
		}
		
		/// <summary>
		/// Creates new node for the value.
		/// </summary>
		/// <param name="permanentReference">Value, has to be valid.</param>
		/// <returns>New empty object node representing the value.</returns>
		private ObjectNode createNewNode(Value permanentReference)
		{
			ObjectNode newNode = new ObjectNode();
			newNode.HashCode = invokeGetHashCode(permanentReference);
			
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
		private ObjectNode getNodeForValue(Value value)
		{
			int objectHashCode = invokeGetHashCode(value);
			// are there any nodes with the same hash code?
			LookupValueCollection<ObjectNode> nodesWithSameHashCode = objectNodesForHashCode[objectHashCode];
			if (nodesWithSameHashCode == null)
			{
				return null;
			}
			else
			{
				// if there is a node with same hash code, check if it has also the same address
				// (hash codes are not uniqe - http://stackoverflow.com/questions/750947/-net-unique-object-identifier)
				ulong objectAddress = value.GetObjectAddress();
				ObjectNode nodeWithSameAddress = nodesWithSameHashCode.Find(
					node => { return objectAddress == node.PermanentReference.GetObjectAddress(); } );
				return nodeWithSameAddress;
			}
		}
		
		/// <summary>
		/// Invokes GetHashCode on given value.
		/// </summary>
		/// <param name="value">Valid value.</param>
		/// <returns>Hash code of the object in the debugee.</returns>
		private int invokeGetHashCode(Value value)
		{
			// David: I had hard time finding out how to invoke static method
			// value.InvokeMethod is nice for instance methods.
			// what about MethodInfo.Invoke() ?
			// also, there could be an overload that takes 1 parameter instead of array
			string defaultHashCode = Eval.InvokeMethod(this.hashCodeMethod, null, new Value[]{value}).AsString;
			
			//MethodInfo method = value.Type.GetMember("GetHashCode", BindingFlags.Method | BindingFlags.IncludeSuperType) as MethodInfo;
			//string hashCode = value.InvokeMethod(method, null).AsString;
			return int.Parse(defaultHashCode);
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
		
		/// <summary>
		/// Checks whether given expression's type is atomic - atomic values will be added to node's property list.
		/// </summary>
		/// <param name="expr">Expression.</param>
		/// <returns>True if expression's type is atomic, False otherwise.</returns>
		private bool isOfAtomicType(Expression expr)
		{
			DebugType typeOfValue = expr.Evaluate(debuggerService.DebuggedProcess).Type;
			return (!typeOfValue.IsClass) || typeOfValue.IsString;
		}
		
		#region Expression helpers
			
		private Value getPermanentReference(Expression expr)
		{
			return expr.Evaluate(debuggerService.DebuggedProcess).GetPermanentReference();
		}
		
		private bool isNull(Expression expr)
		{
			return expr.Evaluate(debuggerService.DebuggedProcess).IsNull;
		}
		
		private DebugType getType(Expression expr)
		{
			return expr.Evaluate(debuggerService.DebuggedProcess).Type;
		}
		
		private ulong getObjectAddress(Expression expr)
		{
			return expr.Evaluate(debuggerService.DebuggedProcess).GetObjectAddress();
		}
		
		#endregion
	}
}
