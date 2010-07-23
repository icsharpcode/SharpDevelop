// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com" />
//     <version>$Revision$</version>
// </file>
using System;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// Description of DefaultOption.
	/// </summary>
	public class DefaultOption : AbstractFreezable, IOption
	{
		OptionType type;
		bool value;
		DomRegion region;
		
		public DefaultOption(OptionType type)
			: this(type, true)
		{
		}
		
		public DefaultOption(OptionType type, bool value)
			: this(type, value, DomRegion.Empty)
		{
		}
		
		public DefaultOption(OptionType type, bool value, DomRegion region)
		{
			this.type = type;
			this.value = value;
			this.region = region;
		}
		
		protected override void FreezeInternal()
		{
			base.FreezeInternal();
		}
		
		public DomRegion Region {
			get {
				return region;
			}
		}
		
		public OptionType Type {
			get {
				return type;
			}
		}
		
		public bool Value {
			get {
				if (type == OptionType.CompareBinary || type == OptionType.CompareText)
					return true;
				
				return value;
			}
		}
	}
}
