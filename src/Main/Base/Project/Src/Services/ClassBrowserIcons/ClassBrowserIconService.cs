// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.Core.WinForms;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.SharpDevelop
{
	public static class ClassBrowserIconService
	{
		#region GetIImage
		static ConcurrentDictionary<ImageSource, IImage> imageCache = new ConcurrentDictionary<ImageSource, IImage>();
		
		internal static IImage GetIImage(ImageSource imageSource)
		{
			return imageCache.GetOrAdd(imageSource, _ => new ImageSourceImage(_));
		}
		#endregion
		
		#region Entity Images
		[Obsolete]
		public static readonly IImage Class = GetIImage(CompletionImage.Class.BaseImage);
		[Obsolete]
		public static readonly IImage Struct = GetIImage(CompletionImage.Struct.BaseImage);
		[Obsolete]
		public static readonly IImage Interface = GetIImage(CompletionImage.Interface.BaseImage);
		[Obsolete]
		public static readonly IImage Enum = GetIImage(CompletionImage.Enum.BaseImage);
		[Obsolete]
		public static readonly IImage Method = GetIImage(CompletionImage.Method.BaseImage);
		[Obsolete]
		public static readonly IImage Property = GetIImage(CompletionImage.Property.BaseImage);
		[Obsolete]
		public static readonly IImage Field = GetIImage(CompletionImage.Field.BaseImage);
		[Obsolete]
		public static readonly IImage Delegate = GetIImage(CompletionImage.Delegate.BaseImage);
		[Obsolete]
		public static readonly IImage Event = GetIImage(CompletionImage.Event.BaseImage);
		[Obsolete]
		public static readonly IImage Indexer = GetIImage(CompletionImage.Indexer.BaseImage);
		#endregion
		
		#region Get Methods for Entity Images
		
		public static IImage GetIcon(IEntity entity)
		{
			return GetIImage(CompletionImage.GetImage(entity));
		}
		
		public static IImage GetIcon(IUnresolvedEntity entity)
		{
			return GetIImage(CompletionImage.GetImage(entity));
		}
		
		public static IImage GetIcon(IVariable v)
		{
			if (v is IField) {
				return GetIcon((IEntity)v);
			} else if (v.IsConst) {
				return Const;
			} else if (v is IParameter) {
				return Parameter;
			} else {
				return LocalVariable;
			}
		}
		
		// This overload exists to avoid the ambiguity between IEntity and IVariable
		public static IImage GetIcon(IField v)
		{
			return GetIcon((IEntity)v);
		}
		#endregion
		
		public static readonly IImage Namespace = GetIImage(CompletionImage.NamespaceImage);
		public static readonly IImage Solution = new ResourceServiceImage("Icons.16x16.CombineIcon");
		public static readonly IImage Const = GetIImage(CompletionImage.Literal.BaseImage);
		public static readonly IImage GotoArrow = new ResourceServiceImage("Icons.16x16.SelectionArrow");
		
		public static readonly IImage LocalVariable = new ResourceServiceImage("Icons.16x16.Local");
		public static readonly IImage Parameter = new ResourceServiceImage("Icons.16x16.Parameter");
		public static readonly IImage Keyword = new ResourceServiceImage("Icons.16x16.Keyword");
		public static readonly IImage Operator = new ResourceServiceImage("Icons.16x16.Operator");
		public static readonly IImage CodeTemplate = new ResourceServiceImage("Icons.16x16.TextFileIcon");
	}
}
