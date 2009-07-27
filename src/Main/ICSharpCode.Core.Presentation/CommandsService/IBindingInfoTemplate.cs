/*
 * Created by SharpDevelop.
 * User: Sergej Andrejev
 * Date: 7/23/2009
 * Time: 2:38 PM
 */
using System;

namespace ICSharpCode.Core.Presentation.CommandsService
{
    /// <summary>
    /// Description of IBindingInfoTemplate.
    /// </summary>
    public interface IBindingInfoTemplate
    {

    public struct BindingInfoTemplate : IBindingInfo
    {
        public string OwnerInstanceName
        {
        	get; set;
        }
        
        public ICollection<UIElement> OwnerInstances
        {
        	get; set;
        }
        
        public string OwnerTypeName
        {
        	get; set;
        }
        
        public ICollection<Type> OwnerTypes
        {
        	get; set;
        }
        
        public string RoutedCommandName
        {
        	get; set;
        }
        
        public BindingGroupCollection Groups 
        {
        	get;
        } 
    }
}
