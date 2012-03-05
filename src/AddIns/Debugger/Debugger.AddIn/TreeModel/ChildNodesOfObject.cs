// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
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
		public static IEnumerable<TreeNode> GetChildNodesOfObject(ExpressionNode expr, DebugType shownType)
		{
			MemberInfo[] publicStatic      = shownType.GetFieldsAndNonIndexedProperties(BindingFlags.Public    | BindingFlags.Static   | BindingFlags.DeclaredOnly);
			MemberInfo[] publicInstance    = shownType.GetFieldsAndNonIndexedProperties(BindingFlags.Public    | BindingFlags.Instance | BindingFlags.DeclaredOnly);
			MemberInfo[] nonPublicStatic   = shownType.GetFieldsAndNonIndexedProperties(BindingFlags.NonPublic | BindingFlags.Static   | BindingFlags.DeclaredOnly);
			MemberInfo[] nonPublicInstance = shownType.GetFieldsAndNonIndexedProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
			
			DebugType baseType = (DebugType)shownType.BaseType;
			if (baseType != null) {
				yield return new TreeNode(
					"Icons.16x16.Class",
					StringParser.Parse("${res:MainWindow.Windows.Debug.LocalVariables.BaseClass}"),
					baseType.Name,
					baseType.FullName,
					baseType.FullName == "System.Object" ? (Func<IEnumerable<TreeNode>>) null : () => Utils.GetChildNodesOfObject(expr, baseType)
				);
			}
			
			if (nonPublicInstance.Length > 0) {
				yield return new TreeNode(
					StringParser.Parse("${res:MainWindow.Windows.Debug.LocalVariables.NonPublicMembers}"),
					() => GetMembersOfObject(expr, nonPublicInstance)
				);
			}
			
			if (publicStatic.Length > 0 || nonPublicStatic.Length > 0) {
				yield return new TreeNode(
					StringParser.Parse("${res:MainWindow.Windows.Debug.LocalVariables.StaticMembers}"),
					() => {
						var children = GetMembersOfObject(expr, publicStatic).ToList();
						if (nonPublicStatic.Length > 0) {
							children.Insert(0, new TreeNode(
								StringParser.Parse("${res:MainWindow.Windows.Debug.LocalVariables.NonPublicStaticMembers}"),
								() => GetMembersOfObject(expr, nonPublicStatic)
							));
						}
						return children;
					}
				);
			}
			
			if (shownType.GetInterface(typeof(IList).FullName) != null) {
				yield return new TreeNode(
					"IList",
					() => GetItemsOfIList(() => expr.Evaluate())
				);
			} else {
				DebugType listType, iEnumerableType, itemType;
				if (shownType.ResolveIEnumerableImplementation(out iEnumerableType, out itemType)) {
					yield return new TreeNode(
						null,
						"IEnumerable",
						"Expanding will enumerate the IEnumerable",
						string.Empty,
						() => GetItemsOfIList(() => DebuggerHelpers.CreateListFromIEnumeralbe(expr.Evaluate(), itemType, out listType))
					);
				}
			}
			
			foreach(TreeNode node in GetMembersOfObject(expr, publicInstance)) {
				yield return node;
			}
		}
		
		public static IEnumerable<TreeNode> GetMembersOfObject(ExpressionNode expr, MemberInfo[] members)
		{
			foreach(MemberInfo memberInfo in members.OrderBy(m => m.Name)) {
				var memberInfoCopy = memberInfo;
				string imageName = ExpressionNode.GetImageForMember((IDebugMemberInfo)memberInfo);
				yield return new ExpressionNode(imageName, memberInfo.Name, () => expr.Evaluate().GetMemberValue(memberInfoCopy));
			}
		}
		
		public static IEnumerable<TreeNode> GetItemsOfIList(Func<Value> getValue)
		{
			Value list = null;
			DebugType iListType = null;
			int count = 0;
			GetValueException error = null;
			try {
				// We use lambda for the value just so that we can get it in this try-catch block
				list = getValue().GetPermanentReference();
				iListType = (DebugType)list.Type.GetInterface(typeof(IList).FullName);
				// Do not get string representation since it can be printed in hex
				count = (int)list.GetPropertyValue(iListType.GetProperty("Count")).PrimitiveValue;
			} catch (GetValueException e) {
				// Cannot yield a value in the body of a catch clause (CS1631)
				error = e;
			}
			if (error != null) {
				yield return new TreeNode(null, "(error)", error.Message, string.Empty, null);
			} else if (count == 0) {
				yield return new TreeNode("(empty)", null);
			} else {
				PropertyInfo pi = iListType.GetProperty("Item");
				for(int i = 0; i < count; i++) {
					int iCopy = i;
					yield return new ExpressionNode("Icons.16x16.Field", "[" + i + "]", () => list.GetPropertyValue(pi, Eval.CreateValue(list.AppDomain, iCopy)));
				}
			}
		}
	}
}
