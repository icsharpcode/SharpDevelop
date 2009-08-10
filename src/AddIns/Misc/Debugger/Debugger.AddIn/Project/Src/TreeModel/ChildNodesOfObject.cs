// <file>
//     <copyright license="BSD-new" see="prj:///COPYING"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System.Collections;
using System.Collections.Generic;
using Debugger.MetaData;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.NRefactory.Ast;

namespace Debugger.AddIn.TreeModel
{
	public partial class Utils
	{
		public static IEnumerable<TreeNode> LazyGetChildNodesOfObject(Expression targetObject, DebugType shownType)
		{
			BindingFlags publicStaticFlags        = BindingFlags.Public | BindingFlags.Static | BindingFlags.Field | BindingFlags.GetProperty;
			BindingFlags publicInstanceFlags      = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Field | BindingFlags.GetProperty;
			BindingFlags nonPublicStaticFlags     = BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Field | BindingFlags.GetProperty;
			BindingFlags nonPublicInstanceFlags   = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Field | BindingFlags.GetProperty;
			
			DebugType baseType = shownType.BaseType;
			if (baseType != null) {
				yield return new TreeNode(
					DebuggerResourceService.GetImage("Icons.16x16.Class"),
					StringParser.Parse("${res:MainWindow.Windows.Debug.LocalVariables.BaseClass}"),
					//string.Empty,
					baseType.Name,
					baseType.FullName,
					baseType.FullName == "System.Object" ? null : Utils.LazyGetChildNodesOfObject(targetObject, baseType)
				);
			}
			
			if (shownType.HasMembers(nonPublicInstanceFlags)) {
				yield return new TreeNode(
					null,
					StringParser.Parse("${res:MainWindow.Windows.Debug.LocalVariables.NonPublicMembers}"),
					string.Empty,
					string.Empty,
					Utils.LazyGetMembersOfObject(targetObject, shownType, nonPublicInstanceFlags)
				);
			}
			
			if (shownType.HasMembers(publicStaticFlags) || shownType.HasMembers(nonPublicStaticFlags)) {
				IEnumerable<TreeNode> childs = Utils.LazyGetMembersOfObject(targetObject, shownType, publicStaticFlags);
				if (shownType.HasMembers(nonPublicStaticFlags)) {
					TreeNode nonPublicStaticNode = new TreeNode(
						null,
						StringParser.Parse("${res:MainWindow.Windows.Debug.LocalVariables.NonPublicStaticMembers}"),
						string.Empty,
						string.Empty,
						Utils.LazyGetMembersOfObject(targetObject, shownType, nonPublicStaticFlags)
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
			
			DebugType iListType = shownType.GetInterface(typeof(IList).FullName);
			if (iListType != null) {
				yield return new IListNode(targetObject, iListType);
			}
			
			foreach(TreeNode node in LazyGetMembersOfObject(targetObject, shownType, publicInstanceFlags)) {
				yield return node;
			}
		}
		
		public static IEnumerable<TreeNode> LazyGetMembersOfObject(Expression expression, DebugType type, BindingFlags bindingFlags)
		{
			List<TreeNode> members = new List<TreeNode>();
			
			foreach(FieldInfo field in type.GetFields(bindingFlags)) {
				members.Add(new ExpressionNode(ExpressionNode.GetImageForMember(field), field.Name, expression.AppendMemberReference(field)));
			}
			foreach(PropertyInfo property in type.GetProperties(bindingFlags)) {
				members.Add(new ExpressionNode(ExpressionNode.GetImageForMember(property), property.Name, expression.AppendMemberReference(property)));
			}
			
			members.Sort();
			return members;
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
