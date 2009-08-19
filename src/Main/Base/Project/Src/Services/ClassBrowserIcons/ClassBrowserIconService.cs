// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Reflection;
using System.Windows.Forms;

using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.SharpDevelop
{
	public static class ClassBrowserIconService
	{
		public const int NamespaceIndex = 3;
		public const int SolutionIndex  = 14;
		public const int ConstIndex   = 15;
		public const int GotoArrowIndex = 13;
		
		public const int LocalVariableIndex = 16;
		public const int ParameterIndex = 17;
		public const int OperatorIndex = 18;
		public const int KeywordIndex = NamespaceIndex; //19;
		public const int CodeTemplateIndex = 20;
		
		public const int ClassIndex     = 21;
		public const int StructIndex    = ClassIndex + 1 * 4;
		public const int InterfaceIndex = ClassIndex + 2 * 4;
		public const int EnumIndex      = ClassIndex + 3 * 4;
		public const int MethodIndex    = ClassIndex + 4 * 4;
		public const int PropertyIndex  = ClassIndex + 5 * 4;
		public const int FieldIndex     = ClassIndex + 6 * 4;
		public const int DelegateIndex  = ClassIndex + 7 * 4;
		public const int EventIndex     = ClassIndex + 8 * 4;
		public const int IndexerIndex   = ClassIndex + 9 * 4;
		
		const int internalModifierOffset  = 1;
		const int protectedModifierOffset = 2;
		const int privateModifierOffset   = 3;
		
		static ImageList imglist = null;
		
		public static ImageList ImageList {
			get {
				return imglist;
			}
		}
		
		static int GetModifierOffset(ModifierEnum modifier)
		{
			if ((modifier & ModifierEnum.Public) == ModifierEnum.Public) {
				return 0;
			}
			if ((modifier & ModifierEnum.Protected) == ModifierEnum.Protected) {
				return protectedModifierOffset;
			}
			if ((modifier & ModifierEnum.Internal) == ModifierEnum.Internal) {
				return internalModifierOffset;
			}
			return privateModifierOffset;
		}
		
		public static int GetIcon(IMember member)
		{
			if (member is IMethod)
				return GetIcon(member as IMethod);
			else if (member is IProperty)
				return GetIcon(member as IProperty);
			else if (member is IField)
				return GetIcon(member as IField);
			else if (member is IEvent)
				return GetIcon(member as IEvent);
			else
				throw new ArgumentException("unknown member type");
		}
		
		public static int GetIcon(IMethod method)
		{
			if (method.GetIsOperator())
				return OperatorIndex;
			else
				return MethodIndex + GetModifierOffset(method.Modifiers);
		}
		
		public static int GetIcon(IProperty property)
		{
			if (property.IsIndexer)
				return IndexerIndex + GetModifierOffset(property.Modifiers);
			else
				return PropertyIndex + GetModifierOffset(property.Modifiers);
		}
		
		public static int GetIcon(IField field)
		{
			if (field.IsConst) {
				return ConstIndex;
			} else if (field.IsParameter) {
				return ParameterIndex;
			} else if (field.IsLocalVariable) {
				return LocalVariableIndex;
			} else {
				return FieldIndex + GetModifierOffset(field.Modifiers);
			}
		}
		
		public static int GetIcon(IEvent evt)
		{
			return EventIndex + GetModifierOffset(evt.Modifiers);
		}
		
		public static int GetIcon(IClass c)
		{
			int imageIndex = ClassIndex;
			switch (c.ClassType) {
				case ClassType.Delegate:
					imageIndex = DelegateIndex;
					break;
				case ClassType.Enum:
					imageIndex = EnumIndex;
					break;
				case ClassType.Struct:
					imageIndex = StructIndex;
					break;
				case ClassType.Interface:
					imageIndex = InterfaceIndex;
					break;
			}
			return imageIndex + GetModifierOffset(c.Modifiers);
		}
		
		public static int GetIcon(MethodBase methodinfo)
		{
			if (methodinfo.IsAssembly) {
				return MethodIndex + internalModifierOffset;
			}
			if (methodinfo.IsPrivate) {
				return MethodIndex + privateModifierOffset;
			}
			if (!(methodinfo.IsPrivate || methodinfo.IsPublic)) {
				return MethodIndex + protectedModifierOffset;
			}
			
			return MethodIndex;
		}
		
		public static int GetIcon(PropertyInfo propertyinfo)
		{
			if (propertyinfo.CanRead && propertyinfo.GetGetMethod(true) != null) {
				return PropertyIndex + GetIcon(propertyinfo.GetGetMethod(true)) - MethodIndex;
			}
			if (propertyinfo.CanWrite && propertyinfo.GetSetMethod(true) != null) {
				return PropertyIndex + GetIcon(propertyinfo.GetSetMethod(true)) - MethodIndex;
			}
			return PropertyIndex;
		}
		
		public static int GetIcon(FieldInfo fieldinfo)
		{
			if (fieldinfo.IsLiteral) {
				return ConstIndex;
			}
			
			if (fieldinfo.IsAssembly) {
				return FieldIndex + internalModifierOffset;
			}
			
			if (fieldinfo.IsPrivate) {
				return FieldIndex + privateModifierOffset;
			}
			
			if (!(fieldinfo.IsPrivate || fieldinfo.IsPublic)) {
				return FieldIndex + protectedModifierOffset;
			}
			
			return FieldIndex;
		}
		
		public static int GetIcon(EventInfo eventinfo)
		{
			if (eventinfo.GetAddMethod(true) != null) {
				return EventIndex + GetIcon(eventinfo.GetAddMethod(true)) - MethodIndex;
			}
			return EventIndex;
		}
		
		public static int GetIcon(System.Type type)
		{
			int BASE = ClassIndex;
			
			if (type.IsValueType) {
				BASE = StructIndex;
			}
			if (type.IsEnum) {
				BASE = EnumIndex;
			}
			if (type.IsInterface) {
				BASE = InterfaceIndex;
			}
			if (type.IsSubclassOf(typeof(System.Delegate))) {
				BASE = DelegateIndex;
			}
			
			if (type.IsNestedPrivate) {
				return BASE + 3;
			}
			
			if (type.IsNotPublic || type.IsNestedAssembly) {
				return BASE + 1;
			}
			
			if (type.IsNestedFamily) {
				return BASE + 2;
			}
			return BASE;
		}
		
		static ClassBrowserIconService()
		{
			imglist = new ImageList();
			imglist.ColorDepth = ColorDepth.Depth32Bit;
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.Assembly"));
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.OpenAssembly"));
			
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.Library"));
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.NameSpace"));
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.SubTypes"));
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.SuperTypes"));
			
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.ClosedFolderBitmap"));
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.OpenFolderBitmap"));
			
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.Reference"));
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.ClosedReferenceFolder"));
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.OpenReferenceFolder"));
			
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.ResourceFileIcon"));
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.Event"));
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.SelectionArrow"));
			
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.CombineIcon"));
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.Literal")); // const
			
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.Local"));
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.Parameter"));
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.Operator"));
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.Keyword"));
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.TextFileIcon"));
			
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.Class")); //21
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.InternalClass"));
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.ProtectedClass"));
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.PrivateClass"));
			
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.Struct"));
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.InternalStruct"));
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.ProtectedStruct"));
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.PrivateStruct"));
			
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.Interface"));
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.InternalInterface"));
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.ProtectedInterface"));
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.PrivateInterface"));
			
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.Enum"));
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.InternalEnum"));
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.ProtectedEnum"));
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.PrivateEnum"));
			
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.Method"));
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.InternalMethod"));
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.ProtectedMethod"));
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.PrivateMethod"));
			
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.Property"));
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.InternalProperty"));
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.ProtectedProperty"));
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.PrivateProperty"));
			
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.Field"));
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.InternalField"));
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.ProtectedField"));
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.PrivateField"));
			
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.Delegate"));
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.InternalDelegate"));
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.ProtectedDelegate"));
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.PrivateDelegate"));
			
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.Event"));
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.InternalEvent"));
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.ProtectedEvent"));
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.PrivateEvent"));
			
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.Indexer"));
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.InternalIndexer"));
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.ProtectedIndexer"));
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.PrivateIndexer"));
		}
	}
}
