// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;

using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.SharpDevelop
{
	public class ClassBrowserImage : IImage
	{
		readonly IImage baseImage;
		
		public System.Windows.Media.ImageSource ImageSource {
			get { return baseImage.ImageSource; }
		}
		
		public System.Drawing.Bitmap Bitmap {
			get { return baseImage.Bitmap; }
		}
		
		public System.Drawing.Icon Icon {
			get { return baseImage.Icon; }
		}
		
		/// <summary>
		/// The image's index in the ClassBrowserIconService.ImageList.
		/// </summary>
		public int ImageIndex { get; private set; }
		
		internal ClassBrowserImage(IImage baseImage, int index)
		{
			this.baseImage = baseImage;
			this.ImageIndex = index;
		}
	}
	
	public static class ClassBrowserIconService
	{
		#region WinForms ImageList
		static ImageList imglist;
		
		/// <summary>
		/// Gets the ImageList.
		/// Do not directly add images to the list, you must use <see cref="AddImage"/>!
		/// </summary>
		public static ImageList ImageList {
			get {
				Gui.WorkbenchSingleton.AssertMainThread();
				if (imglist == null) {
					lock (lockObj) {
						imglist = new ImageList();
						AddNewImagesToList();
					}
				}
				return imglist;
			}
		}
		
		static void AddNewImagesToList()
		{
			lock (lockObj) {
				if (imglist.Images.Count > imglistEntries.Count)
					throw new InvalidOperationException("Too many images in list (list was modified externally?)");
				while (imglist.Images.Count < imglistEntries.Count) {
					imglist.Images.Add(imglistEntries[imglist.Images.Count].Bitmap);
				}
			}
		}
		
		static readonly object lockObj = new Object();
		static readonly List<ClassBrowserImage> imglistEntries = new List<ClassBrowserImage>();
		
		public static ClassBrowserImage GetImageByIndex(int index)
		{
			lock (lockObj) {
				return imglistEntries[index];
			}
		}
		
		static ClassBrowserImage AddImage(string resourceName)
		{
			return AddImage(new ResourceServiceImage(resourceName));
		}
		
		public static ClassBrowserImage AddImage(IImage baseImage)
		{
			if (baseImage == null)
				throw new ArgumentNullException("baseImage");
			ClassBrowserImage image;
			bool imgListPresent;
			lock (lockObj) {
				image = new ClassBrowserImage(baseImage, imglistEntries.Count);
				imglistEntries.Add(image);
				imgListPresent = (imglist != null);
			}
			// We need to do the call outside the lock to prevent deadlocks.
			// We cannot use an async call because we need to ensure that the image is added to the list
			// before we return.
			if (imgListPresent) {
				Gui.WorkbenchSingleton.SafeThreadCall(AddNewImagesToList);
			}
			return image;
		}
		#endregion
		
		public static readonly System.Windows.Size ImageSize = new System.Windows.Size(16, 16);
		
		#region Entity Images
		static readonly ClassBrowserImage[] entityImages = {
			AddImage("Icons.16x16.Class"),
			AddImage("Icons.16x16.InternalClass"),
			AddImage("Icons.16x16.ProtectedClass"),
			AddImage("Icons.16x16.PrivateClass"),
			
			AddImage("Icons.16x16.Struct"),
			AddImage("Icons.16x16.InternalStruct"),
			AddImage("Icons.16x16.ProtectedStruct"),
			AddImage("Icons.16x16.PrivateStruct"),
			
			AddImage("Icons.16x16.Interface"),
			AddImage("Icons.16x16.InternalInterface"),
			AddImage("Icons.16x16.ProtectedInterface"),
			AddImage("Icons.16x16.PrivateInterface"),
			
			AddImage("Icons.16x16.Enum"),
			AddImage("Icons.16x16.InternalEnum"),
			AddImage("Icons.16x16.ProtectedEnum"),
			AddImage("Icons.16x16.PrivateEnum"),
			
			AddImage("Icons.16x16.Method"),
			AddImage("Icons.16x16.InternalMethod"),
			AddImage("Icons.16x16.ProtectedMethod"),
			AddImage("Icons.16x16.PrivateMethod"),
			
			AddImage("Icons.16x16.Property"),
			AddImage("Icons.16x16.InternalProperty"),
			AddImage("Icons.16x16.ProtectedProperty"),
			AddImage("Icons.16x16.PrivateProperty"),
			
			AddImage("Icons.16x16.Field"),
			AddImage("Icons.16x16.InternalField"),
			AddImage("Icons.16x16.ProtectedField"),
			AddImage("Icons.16x16.PrivateField"),
			
			AddImage("Icons.16x16.Delegate"),
			AddImage("Icons.16x16.InternalDelegate"),
			AddImage("Icons.16x16.ProtectedDelegate"),
			AddImage("Icons.16x16.PrivateDelegate"),
			
			AddImage("Icons.16x16.Event"),
			AddImage("Icons.16x16.InternalEvent"),
			AddImage("Icons.16x16.ProtectedEvent"),
			AddImage("Icons.16x16.PrivateEvent"),
			
			AddImage("Icons.16x16.Indexer"),
			AddImage("Icons.16x16.InternalIndexer"),
			AddImage("Icons.16x16.ProtectedIndexer"),
			AddImage("Icons.16x16.PrivateIndexer"),
			
			AddImage("Icons.16x16.ExtensionMethod"),
			AddImage("Icons.16x16.InternalExtensionMethod"),
			AddImage("Icons.16x16.ProtectedExtensionMethod"),
			AddImage("Icons.16x16.PrivateExtensionMethod")
		};
		
		const int ClassIndex           = 0;
		const int StructIndex          = ClassIndex + 1 * 4;
		const int InterfaceIndex       = ClassIndex + 2 * 4;
		const int EnumIndex            = ClassIndex + 3 * 4;
		const int MethodIndex          = ClassIndex + 4 * 4;
		const int PropertyIndex        = ClassIndex + 5 * 4;
		const int FieldIndex           = ClassIndex + 6 * 4;
		const int DelegateIndex        = ClassIndex + 7 * 4;
		const int EventIndex           = ClassIndex + 8 * 4;
		const int IndexerIndex         = ClassIndex + 9 * 4;
		const int ExtensionMethodIndex = ClassIndex + 10 * 4;
		
		const int internalModifierOffset  = 1;
		const int protectedModifierOffset = 2;
		const int privateModifierOffset   = 3;
		
		public static readonly ClassBrowserImage Class = entityImages[ClassIndex];
		public static readonly ClassBrowserImage Struct = entityImages[StructIndex];
		public static readonly ClassBrowserImage Interface = entityImages[InterfaceIndex];
		public static readonly ClassBrowserImage Enum = entityImages[EnumIndex];
		public static readonly ClassBrowserImage Method = entityImages[MethodIndex];
		public static readonly ClassBrowserImage Property = entityImages[PropertyIndex];
		public static readonly ClassBrowserImage Field = entityImages[FieldIndex];
		public static readonly ClassBrowserImage Delegate = entityImages[DelegateIndex];
		public static readonly ClassBrowserImage Event = entityImages[EventIndex];
		public static readonly ClassBrowserImage Indexer = entityImages[IndexerIndex];
		#endregion
		
		#region Get Methods for Entity Images
		
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
		
		public static ClassBrowserImage GetIcon(IEntity entity)
		{
			if (entity is IMethod)
				return GetIcon(entity as IMethod);
			else if (entity is IProperty)
				return GetIcon(entity as IProperty);
			else if (entity is IField)
				return GetIcon(entity as IField);
			else if (entity is IEvent)
				return GetIcon(entity as IEvent);
			else if (entity is IClass)
				return GetIcon(entity as IClass);
			else
				throw new ArgumentException("unknown entity type");
		}
		
		public static ClassBrowserImage GetIcon(IMethod method)
		{
			if (method.IsOperator)
				return Operator;
			else if (method.IsExtensionMethod)
				return entityImages[ExtensionMethodIndex + GetModifierOffset(method.Modifiers)];
			else
				return entityImages[MethodIndex + GetModifierOffset(method.Modifiers)];
		}
		
		public static ClassBrowserImage GetIcon(IProperty property)
		{
			if (property.IsIndexer)
				return entityImages[IndexerIndex + GetModifierOffset(property.Modifiers)];
			else
				return entityImages[PropertyIndex + GetModifierOffset(property.Modifiers)];
		}
		
		public static ClassBrowserImage GetIcon(IField field)
		{
			if (field.IsConst) {
				return Const;
			} else if (field.IsParameter) {
				return Parameter;
			} else if (field.IsLocalVariable) {
				return LocalVariable;
			} else {
				return entityImages[FieldIndex + GetModifierOffset(field.Modifiers)];
			}
		}
		
		public static ClassBrowserImage GetIcon(IEvent evt)
		{
			return entityImages[EventIndex + GetModifierOffset(evt.Modifiers)];
		}
		
		public static ClassBrowserImage GetIcon(IClass c)
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
			return entityImages[imageIndex + GetModifierOffset(c.Modifiers)];
		}
		
		static int GetVisibilityOffset(MethodBase methodinfo)
		{
			if (methodinfo.IsAssembly) {
				return internalModifierOffset;
			}
			if (methodinfo.IsPrivate) {
				return privateModifierOffset;
			}
			if (!(methodinfo.IsPrivate || methodinfo.IsPublic)) {
				return protectedModifierOffset;
			}
			return 0;
		}
		
		public static ClassBrowserImage GetIcon(MethodBase methodinfo)
		{
			return entityImages[MethodIndex + GetVisibilityOffset(methodinfo)];
		}
		
		public static ClassBrowserImage GetIcon(PropertyInfo propertyinfo)
		{
			if (propertyinfo.CanRead && propertyinfo.GetGetMethod(true) != null) {
				return entityImages[PropertyIndex + GetVisibilityOffset(propertyinfo.GetGetMethod(true))];
			}
			if (propertyinfo.CanWrite && propertyinfo.GetSetMethod(true) != null) {
				return entityImages[PropertyIndex + GetVisibilityOffset(propertyinfo.GetSetMethod(true))];
			}
			return entityImages[PropertyIndex];
		}
		
		public static ClassBrowserImage GetIcon(FieldInfo fieldinfo)
		{
			if (fieldinfo.IsLiteral) {
				return Const;
			}
			
			if (fieldinfo.IsAssembly) {
				return entityImages[FieldIndex + internalModifierOffset];
			}
			
			if (fieldinfo.IsPrivate) {
				return entityImages[FieldIndex + privateModifierOffset];
			}
			
			if (!(fieldinfo.IsPrivate || fieldinfo.IsPublic)) {
				return entityImages[FieldIndex + protectedModifierOffset];
			}
			
			return entityImages[FieldIndex];
		}
		
		public static ClassBrowserImage GetIcon(EventInfo eventinfo)
		{
			if (eventinfo.GetAddMethod(true) != null) {
				return entityImages[EventIndex + GetVisibilityOffset(eventinfo.GetAddMethod(true))];
			}
			return entityImages[EventIndex];
		}
		
		public static ClassBrowserImage GetIcon(System.Type type)
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
				return entityImages[BASE + privateModifierOffset];
			}
			
			if (type.IsNotPublic || type.IsNestedAssembly) {
				return entityImages[BASE + internalModifierOffset];
			}
			
			if (type.IsNestedFamily) {
				return entityImages[BASE + protectedModifierOffset];
			}
			return entityImages[BASE];
		}
		#endregion
		
		public static readonly ClassBrowserImage Namespace = AddImage("Icons.16x16.NameSpace");
		public static readonly ClassBrowserImage Solution = AddImage("Icons.16x16.CombineIcon");
		public static readonly ClassBrowserImage Const = AddImage("Icons.16x16.Literal");
		public static readonly ClassBrowserImage GotoArrow = AddImage("Icons.16x16.SelectionArrow");
		
		public static readonly ClassBrowserImage LocalVariable = AddImage("Icons.16x16.Local");
		public static readonly ClassBrowserImage Parameter = AddImage("Icons.16x16.Parameter");
		public static readonly ClassBrowserImage Keyword = AddImage("Icons.16x16.Keyword");
		public static readonly ClassBrowserImage Operator = AddImage("Icons.16x16.Operator");
		public static readonly ClassBrowserImage CodeTemplate = AddImage("Icons.16x16.TextFileIcon");
	}
}
