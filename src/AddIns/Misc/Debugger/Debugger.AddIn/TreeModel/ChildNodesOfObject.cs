// <file>
//     <copyright license="BSD-new" see="prj:///COPYING"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using Debugger.AddIn.Visualizers.Utils;
using Debugger.MetaData;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Services;

namespace Debugger.AddIn.TreeModel
{
	public partial class Utils
	{
		public static IEnumerable<TreeNode> LazyGetChildNodesOfObject(Expression targetObject, DebugType shownType)
		{
			MemberInfo[] publicStatic      = shownType.GetFieldsAndNonIndexedProperties(BindingFlags.Public    | BindingFlags.Static   | BindingFlags.DeclaredOnly);
			MemberInfo[] publicInstance    = shownType.GetFieldsAndNonIndexedProperties(BindingFlags.Public    | BindingFlags.Instance | BindingFlags.DeclaredOnly);
			MemberInfo[] nonPublicStatic   = shownType.GetFieldsAndNonIndexedProperties(BindingFlags.NonPublic | BindingFlags.Static   | BindingFlags.DeclaredOnly);
			MemberInfo[] nonPublicInstance = shownType.GetFieldsAndNonIndexedProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
			
			DebugType baseType = (DebugType)shownType.BaseType;
			if (baseType != null) {
				yield return new TreeNode(
					DebuggerResourceService.GetImage("Icons.16x16.Class"),
					StringParser.Parse("${res:MainWindow.Windows.Debug.LocalVariables.BaseClass}"),
					baseType.Name,
					baseType.FullName,
					baseType.FullName == "System.Object" ? null : Utils.LazyGetChildNodesOfObject(targetObject, baseType)
				);
			}
			
			if (nonPublicInstance.Length > 0) {
				yield return new TreeNode(
					null,
					StringParser.Parse("${res:MainWindow.Windows.Debug.LocalVariables.NonPublicMembers}"),
					string.Empty,
					string.Empty,
					Utils.LazyGetMembersOfObject(targetObject, nonPublicInstance)
				);
			}
			
			if (publicStatic.Length > 0 || nonPublicStatic.Length > 0) {
				IEnumerable<TreeNode> childs = Utils.LazyGetMembersOfObject(targetObject, publicStatic);
				if (nonPublicStatic.Length > 0) {
					TreeNode nonPublicStaticNode = new TreeNode(
						null,
						StringParser.Parse("${res:MainWindow.Windows.Debug.LocalVariables.NonPublicStaticMembers}"),
						string.Empty,
						string.Empty,
						Utils.LazyGetMembersOfObject(targetObject, nonPublicStatic)
					);
					childs = Utils.PrependNode(nonPublicStaticNode, childs);
				}
				yield return new TreeNode(
					null,
					StringParser.Parse("${res:MainWindow.Windows.Debug.LocalVariables.StaticMembers}"),
					string.Empty,
					string.Empty,
					childs
				);
			}
			
			DebugType iListType = (DebugType)shownType.GetInterface(typeof(IList).FullName);
			if (iListType != null) {
				yield return new IListNode(targetObject);
			} else {
				DebugType iEnumerableType, itemType;
				if (shownType.ResolveIEnumerableImplementation(out iEnumerableType, out itemType)) {
					yield return new IEnumerableNode(targetObject, itemType);
				}
			}
			
			foreach(TreeNode node in LazyGetMembersOfObject(targetObject, publicInstance)) {
				yield return node;
			}
		}
		
		public static IEnumerable<TreeNode> LazyGetMembersOfObject(Expression expression, MemberInfo[] members)
		{
			List<TreeNode> nodes = new List<TreeNode>();
			foreach(MemberInfo memberInfo in members) {
				nodes.Add(new ExpressionNode(ExpressionNode.GetImageForMember((IDebugMemberInfo)memberInfo), memberInfo.Name, expression.AppendMemberReference((IDebugMemberInfo)memberInfo)));
			}
			nodes.Sort();
			return nodes;
		}

		
		public static IEnumerable<TreeNode> LazyGetItemsOfIList(Expression targetObject)
		{
			int count = 0;
			try {
				count = GetIListCount(targetObject);
			} catch (GetValueException) {
				yield break;
			}
			
			for(int i = 0; i < count; i++) {
				yield return new ExpressionNode(ExpressionNode.GetImageForArrayIndexer(), "[" + i + "]", targetObject.AppendIndexer(i));
			}
		}
		
		/// <summary>
		/// Evaluates System.Collections.ICollection.Count property on given object.
		/// </summary>
		/// <exception cref="GetValueException">Evaluating System.Collections.ICollection.Count on targetObject failed.</exception>
		public static int GetIListCount(Expression targetObject)
		{
			Value list = targetObject.Evaluate(WindowsDebugger.CurrentProcess);
			var iCollectionInterface = list.Type.GetInterface(typeof(ICollection).FullName);
			if (iCollectionInterface == null) {
				throw new GetValueException(targetObject, targetObject.PrettyPrint() + " does not implement System.Collections.ICollection");
			}
			PropertyInfo countProperty = iCollectionInterface.GetProperty("Count");
			// Do not get string representation since it can be printed in hex
			return (int)list.GetPropertyValue(countProperty).PrimitiveValue;
		}
		
		public static IEnumerable<TreeNode> PrependNode(TreeNode node, IEnumerable<TreeNode> rest)
		{
			yield return node;
			if (rest != null) {
				foreach(TreeNode absNode in rest) {
					yield return absNode;
				}
			}
		}
	}
}
