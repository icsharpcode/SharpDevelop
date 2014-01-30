// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.CodeCompletion;
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
		public static readonly IImage Class = GetIImage(CompletionImage.Class.BaseImage);
		public static readonly IImage Struct = GetIImage(CompletionImage.Struct.BaseImage);
		public static readonly IImage Interface = GetIImage(CompletionImage.Interface.BaseImage);
		public static readonly IImage Enum = GetIImage(CompletionImage.Enum.BaseImage);
		public static readonly IImage Method = GetIImage(CompletionImage.Method.BaseImage);
		public static readonly IImage Property = GetIImage(CompletionImage.Property.BaseImage);
		public static readonly IImage Field = GetIImage(CompletionImage.Field.BaseImage);
		public static readonly IImage Delegate = GetIImage(CompletionImage.Delegate.BaseImage);
		public static readonly IImage Event = GetIImage(CompletionImage.Event.BaseImage);
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
		
		public static IImage GetIcon(IType t)
		{
			ITypeDefinition def = t.GetDefinition();
			if (def != null)
				return GetIcon(def);
			else
				return null;
		}
		
		public static IImage GetIcon(ITypeDefinition t)
		{
			return GetIImage(CompletionImage.GetImage(t));
		}
		#endregion
		
		public static IImage Namespace { get { return GetIImage(CompletionImage.NamespaceImage); } }
		public static IImage Solution { get { return SD.ResourceService.GetImage("Icons.16x16.CombineIcon"); } }
		public static IImage Const { get { return GetIImage(CompletionImage.Literal.BaseImage); } }
		public static IImage GotoArrow { get { return SD.ResourceService.GetImage("Icons.16x16.SelectionArrow"); } }
		
		public static IImage LocalVariable { get { return SD.ResourceService.GetImage("Icons.16x16.Local"); } }
		public static IImage Parameter { get { return SD.ResourceService.GetImage("Icons.16x16.Parameter"); } }
		public static IImage Keyword { get { return SD.ResourceService.GetImage("Icons.16x16.Keyword"); } }
		public static IImage Operator { get { return SD.ResourceService.GetImage("Icons.16x16.Operator"); } }
		public static IImage CodeTemplate { get { return SD.ResourceService.GetImage("Icons.16x16.TextFileIcon"); } }
	}
}
