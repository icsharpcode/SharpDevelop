// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version value="$version"/>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Dom
{
	public class LanguageProperties
	{
		public readonly static LanguageProperties CSharp = new LanguageProperties(StringComparer.InvariantCulture);
		public readonly static LanguageProperties VBNet = new VBNetProperties();
		
		private class VBNetProperties : LanguageProperties
		{
			public VBNetProperties() : base(StringComparer.InvariantCultureIgnoreCase) {}
			
			public override bool ShowMember(IMember member, bool showStatic)
			{
				return member.IsStatic || !showStatic;
			}
		}
		
		StringComparer nameComparer;
		
		public LanguageProperties(StringComparer nameComparer)
		{
			this.nameComparer = nameComparer;
		}
		
		public StringComparer NameComparer {
			get {
				return nameComparer;
			}
		}
		
		public virtual bool ShowMember(IMember member, bool showStatic)
		{
			return member.IsStatic == showStatic;
		}
	}
}
