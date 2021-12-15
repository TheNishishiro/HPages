using System;
using HentaiPages.Models.Enums;

namespace HentaiPages.CustomAttributes
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