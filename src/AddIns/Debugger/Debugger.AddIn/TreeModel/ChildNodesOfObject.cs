// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using Debugger.AddIn.Visualizers.Utils;
using Debugger.MetaData;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.SharpDevelop.Debugging;

namespace Debugger.AddIn.TreeModel
{
	public partial class Utils
	{
		public static IEnumerable<TreeNode> LazyGetChildNodesOfObject(TreeNode current, Expression targetObject, DebugType shownType)
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
					current,
					newNode => baseType.FullName == "System.Object" ? null : Utils.LazyGetChildNodesOfObject(newNode, targetObject, baseType)
				);
			}
			
			if (nonPublicInstance.Length > 0) {
				yield return new TreeNode(
					null,
					StringParser.Parse("${res:MainWindow.Windows.Debug.LocalVariables.NonPublicMembers}"),
					string.Empty,
					string.Empty,
					current,
					newNode => Utils.LazyGetMembersOfObject(newNode, targetObject, nonPublicInstance)
				);
			}
			
			if (publicStatic.Length > 0 || nonPublicStatic.Length > 0) {
				yield return new TreeNode(
					null,
					StringParser.Parse("${res:MainWindow.Windows.Debug.LocalVariables.StaticMembers}"),
					string.Empty,
					string.Empty,
					current,
					p => {
						var children = Utils.LazyGetMembersOfObject(p, targetObject, publicStatic);
						if (nonPublicStatic.Length > 0) {
							TreeNode nonPublicStaticNode = new TreeNode(
								null,
								StringParser.Parse("${res:MainWindow.Windows.Debug.LocalVariables.NonPublicStaticMembers}"),
								string.Empty,
								string.Empty,
								p,
								newNode => Utils.LazyGetMembersOfObject(newNode, targetObject, nonPublicStatic)
							);
							children = Utils.PrependNode(nonPublicStaticNode, children);
						}
						return children;
					}
				);
			}
			
			DebugType iListType = (DebugType)shownType.GetInterface(typeof(IList).FullName);
			if (iListType != null) {
				yield return new IListNode(current, targetObject);
			} else {
				DebugType iEnumerableType, itemType;
				if (shownType.ResolveIEnumerableImplementation(out iEnumerableType, out itemType)) {
					yield return new IEnumerableNode(current, targetObject, itemType);
				}
			}
			
			foreach(TreeNode node in LazyGetMembersOfObject(current, targetObject, publicInstance)) {
				yield return node;
			}
		}
		
		public static IEnumerable<TreeNode> LazyGetMembersOfObject(TreeNode parent, Expression expression, MemberInfo[] members)
		{
			List<TreeNode> nodes = new List<TreeNode>();
			foreach(MemberInfo memberInfo in members) {
				string imageName;
				var image = ExpressionNode.GetImageForMember((IDebugMemberInfo)memberInfo, out imageName);
				var exp = new ExpressionNode(parent, image, memberInfo.Name, expression.AppendMemberReference((IDebugMemberInfo)memberInfo));
				exp.ImageName = imageName;
				nodes.Add(exp);
			}
			nodes.Sort();
			return nodes;
		}

		
		public static IEnumerable<TreeNode> LazyGetItemsOfIList(TreeNode parent, Expression targetObject)
		{
			// Add a cast, so that we are sure the expression has an indexer.
			// (The expression can be e.g. of type 'object' but its value is a List.
			// Without the cast, evaluating "expr[i]" would fail, because object does not have an indexer).
			targetObject = targetObject.CastToIList();
			int count = 0;
			GetValueException error = null;
			try {
				count = targetObject.GetIListCount();
			} catch (GetValueException e) {
				// Cannot yield a value in the body of a catch clause (CS1631)
				error = e;
			}
			if (error != null) {
				yield return new TreeNode(null, "(error)", error.Message, null, null, _ => null);
			} else if (count == 0) {
				yield return new TreeNode(null, "(empty)", null, null, null, _ => null);
			} else {
				for(int i = 0; i < count; i++) {
					string imageName;
					var image = ExpressionNode.GetImageForArrayIndexer(out imageName);
					var itemNode = new ExpressionNode(parent, image, "[" + i + "]", targetObject.AppendIndexer(i));
					itemNode.ImageName = imageName;
					yield return itemNode;
				}
			}
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
