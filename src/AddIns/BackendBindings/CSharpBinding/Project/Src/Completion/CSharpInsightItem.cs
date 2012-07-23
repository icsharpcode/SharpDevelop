// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.Completion;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace CSharpBinding.Completion
{
	sealed class CSharpInsightItem : IInsightItem
	{
		public readonly IParameterizedMember Method;
		readonly IAmbience ambience;
		
		public CSharpInsightItem(IParameterizedMember method, IAmbience ambience)
		{
			this.Method = method;
			this.ambience = ambience;
		}
		
		string header;
		
		public object Header {
			get {
				if (header == null) {
					ambience.ConversionFlags = ConversionFlags.StandardConversionFlags;
					header = ambience.ConvertEntity(Method);
				}
				return header;
			}
		}
		
		public object Content {
			get { return null; }
		}
	}
	
	sealed class CSharpParameterDataProvider : IParameterDataProvider
	{
		readonly int startOffset;
		internal readonly IReadOnlyList<CSharpInsightItem> items;
		
		public CSharpParameterDataProvider(int startOffset, IEnumerable<CSharpInsightItem> items)
		{
			this.startOffset = startOffset;
			this.items = items.ToList();
		}
		
		int IParameterDataProvider.Count {
			get { return items.Count; }
		}
		
		public int StartOffset {
			get { return startOffset; }
		}
		
		string IParameterDataProvider.GetHeading(int overload, string[] parameterDescription, int currentParameter)
		{
			throw new NotImplementedException();
		}
		
		string IParameterDataProvider.GetDescription(int overload, int currentParameter)
		{
			throw new NotImplementedException();
		}
		
		string IParameterDataProvider.GetParameterDescription(int overload, int paramIndex)
		{
			throw new NotImplementedException();
		}
		
		int IParameterDataProvider.GetParameterCount(int overload)
		{
			throw new NotImplementedException();
		}
		
		bool IParameterDataProvider.AllowParameterList(int overload)
		{
			throw new NotImplementedException();
		}
	}
}
