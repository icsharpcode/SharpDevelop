/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 16.03.2013
 * Time: 20:24
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System.Security;

[assembly: SecurityRules(SecurityRuleSet.Level1)]

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("ICSharpCode.Reporting.Test")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("ICSharpCode.Reports.Addin")]