// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace Grunwald.BooBinding
{
	public class BooLanguageProperties : LanguageProperties
	{
		public readonly static BooLanguageProperties Instance = new BooLanguageProperties();
		
		public BooLanguageProperties() : base(StringComparer.InvariantCulture, BooCodeGenerator.Instance) {}
		
		public override bool ImportNamespaces {
			get {
				return true;
			}
		}
		
		public override bool ImportModules {
			get {
				return true;
			}
		}
		
		public override bool CanImportClasses {
			get {
				return true;
			}
		}
		
		public override bool AllowObjectConstructionOutsideContext {
			get {
				return true;
			}
		}
	}
}
