using System;
using HPages.Models.Enums;

namespace HPages.CustomAttributes
{
    public class FilterFieldAttribute : Attribute
    {
        public FilterFieldType FilterType;
        public string DisplayName;
        
        public FilterFieldAttribute(FilterFieldType filterType, string displayName)
        {
            FilterType = filterType;
            DisplayName = displayName;
        }
    }
}