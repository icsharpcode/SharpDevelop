// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Dom
{
	public interface IProperty : IMethodOrProperty
	{
		DomRegion GetterRegion {
			get;
		}

		DomRegion SetterRegion {
			get;
		}

		bool CanGet {
			get;
		}

		bool CanSet {
			get;
		}
		
		bool IsIndexer {
			get;
		}
		
		ModifierEnum GetterModifiers {
			get;
		}
		
		ModifierEnum SetterModifiers {
			get;
		}
	}
}
