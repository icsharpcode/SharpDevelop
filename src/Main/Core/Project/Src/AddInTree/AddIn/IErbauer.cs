/*
 * Created by SharpDevelop.
 * User: Omnibrain
 * Date: 21.12.2004
 * Time: 21:51
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Description of Erbauer.
	/// </summary>
	public interface IErbauer
	{
		object BuildItem(object caller, Codon codon, ArrayList subItems);
	}
}
