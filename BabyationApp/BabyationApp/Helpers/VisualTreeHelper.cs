using System;
using System.Collections.Generic;
using System.Reflection;
using Xamarin.Forms;

namespace BabyationApp.Helpers
{
    /// <summary>
    /// This class contains helper methods to find child in the visual tree
    /// </summary>
    public class VisualTreeHelper
    {
        /// <summary>
        /// Finds a child template with a name of the child
        /// </summary>
        /// <typeparam name="T">The type of child to find to</typeparam>
        /// <param name="parent">The parent in which to find the child</param>
        /// <param name="name">Name of the child to find to</param>
        /// <param name="recursive">Specify whether to search in recursive in the visual tree</param>
        /// <returns>Returns the templated child of type T in the parent if found; otherwise returns null</returns>
        public static T GetTemplateChild<T>(Element parent, string name, bool recursive = false) where T : Element
        {
            if (parent == null)
                return null;
            var templateChild = parent.FindByName<T>(name);
            if (templateChild != null)
                return templateChild;
            foreach (var child in FindVisualChildren<Element>(parent, recursive))
            {
                templateChild = GetTemplateChild<T>(child, name, recursive);
                if (templateChild != null)
                    return templateChild;
            }
            return null;
        }

        /// <summary>
        /// Finds only visual children in the visual tree of a parent
        /// </summary>
        /// <typeparam name="T">The type of visual child to find to</typeparam>
        /// <param name="element">The parent in which to find the visual children</param>
        /// <param name="recursive">Specify whether to search in recursive in the visual tree</param>
        /// <returns>The list of visual children in the parent</returns>
        public static IEnumerable<T> FindVisualChildren<T>(Element element, bool recursive = true) where T : Element
        {
            if (element != null && element is Layout)
            {                
                PropertyInfo childrenProperty = childrenProperty = element.GetType().GetRuntimeProperty("InternalChildren");
                if (childrenProperty == null)
                {
                    IEnumerable<PropertyInfo> properties = element.GetType().GetRuntimeProperties();
                    foreach (PropertyInfo pi in properties)
                    {
                        if (pi.Name == "InternalChildren")
                        {
                            childrenProperty = pi;
                            break;
                        }
                    }
                }
                
                if (childrenProperty != null)
                {
                    var children = (IEnumerable<Element>)childrenProperty.GetValue(element);
                    foreach (var child in children)
                    {
                        if (child != null && child is T)
                        {
                            yield return (T)child;
                        }
                        if (recursive)
                        {
                            foreach (T childOfChild in FindVisualChildren<T>(child, recursive))
                            {
                                yield return childOfChild;
                            }
                        }
                    }
                }
            }
        }
    }
}



