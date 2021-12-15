using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using HentaiPages.CustomAttributes;
using HentaiPages.Models.Enums;
using Microsoft.AspNetCore.Mvc;

namespace HentaiPages.Models
{
    public class GalleryFilter
    {
        [FilterField(FilterFieldType.Type, "Favourite")] public bool? ShowLiked { get; set; }
        [FilterField(FilterFieldType.Characteristic, "Feet")] public bool? Feet { get; set; }
        [FilterField(FilterFieldType.Characteristic, "Thighs")] public bool? Thighs { get; set; }
        [FilterField(FilterFieldType.Characteristic, "Leg-wear")] public bool? Legwear { get; set; }
        [FilterField(FilterFieldType.Characteristic, "Butt")] public bool? Butt { get; set; }
        [FilterField(FilterFieldType.Actions, "Creampie")] public bool? Creampie { get; set; }
        [FilterField(FilterFieldType.Actions, "Sex ")] public bool? Sex { get; set; }
        [FilterField(FilterFieldType.Actions, "Masturbation")] public bool? Masturbation { get; set; }
        [FilterField(FilterFieldType.Characteristic, "Boobies")] public bool? Boobies { get; set; }
        [FilterField(FilterFieldType.Characteristic, "Pantsu")] public bool? Pantsu { get; set; }
        [FilterField(FilterFieldType.Profession, "Yuri")] public bool? Yuri { get; set; }
        [FilterField(FilterFieldType.Profession, "Catgirl")] public bool? Catgirl { get; set; }
        [FilterField(FilterFieldType.Type, "Animated")]  public bool? Gif { get; set; }
        [FilterField(FilterFieldType.Characteristic, "Real life")] public bool? Irl { get; set; }
        [FilterField(FilterFieldType.Characteristic, "Ecchi")] public bool? Ecchi { get; set; }
        [FilterField(FilterFieldType.Characteristic, "Wet")] public bool? Wet { get; set; }
        [FilterField(FilterFieldType.Type, "POV")] public bool? POV { get; set; }
        [FilterField(FilterFieldType.Actions, "Handjob")] public bool? Handjob { get; set; }
        [FilterField(FilterFieldType.Profession, "Group")] public bool? Group { get; set; }
        [FilterField(FilterFieldType.Profession, "School girl")] public bool? Schoolgirl { get; set; }
        [FilterField(FilterFieldType.Profession, "Secretary")] public bool? Secretary { get; set; }
        [FilterField(FilterFieldType.Characteristic, "Condom")] public bool? Condom { get; set; }
        [FilterField(FilterFieldType.Type, "Grayscale")] public bool? Grayscale { get; set; }
        [FilterField(FilterFieldType.Profession, "Casual")] public bool? Casual { get; set; }
        [FilterField(FilterFieldType.Characteristic, "Inviting")] public bool? Inviting { get; set; }
        [FilterField(FilterFieldType.Actions, "Cowgirl")] public bool? Cowgirl { get; set; }
        [FilterField(FilterFieldType.Characteristic, "Open legs")] public bool? LegsSpreading { get; set; }
        [FilterField(FilterFieldType.Profession, "Maid")] public bool? Maid { get; set; }
        [FilterField(FilterFieldType.Actions, "Blowjob")] public bool? Blowjob { get; set; }
        [FilterField(FilterFieldType.Characteristic, "Lingerie")] public bool? Lingerie { get; set; }
        [FilterField(FilterFieldType.Characteristic, "Tattoo")] public bool? Tattoo { get; set; }
        [FilterField(FilterFieldType.Characteristic, "Pussy")] public bool? Pussy { get; set; }
        [FilterField(FilterFieldType.Characteristic, "Blushing")] public bool? Blushing { get; set; }
        [FilterField(FilterFieldType.Characteristic, "Happy")] public bool? Happy { get; set; }
        [FilterField(FilterFieldType.Actions, "Boobjob")]  public bool? Boobjob { get; set; }
        [FilterField(FilterFieldType.Actions, "Cunilingus")]  public bool? PussyEating { get; set; }
        [FilterField(FilterFieldType.Characteristic, "Cumshot")] public bool? Cumshot { get; set; }
        [FilterField(FilterFieldType.Characteristic, "Sex toys")] public bool? Toys { get; set; }
        [FilterField(FilterFieldType.Actions, "Fingering")] public bool? Fingering { get; set; }
        [FilterField(FilterFieldType.Type, "Video")] public bool? Video { get; set; }
        [FilterField(FilterFieldType.Actions, "Footjob")] public bool? Footjob { get; set; }
        [FilterField(FilterFieldType.Actions, "Hiding")] public bool? Hiding { get; set; }
        [FilterField(FilterFieldType.Characteristic, "Dialogs")] public bool? Dialogs { get; set; }
        [FilterField(FilterFieldType.Actions, "Doggy")] public bool? Doggy { get; set; }
        [FilterField(FilterFieldType.Profession, "human't")] public bool? NotHuman { get; set; }
        [FilterField(FilterFieldType.Profession, "MILF")] public bool? Milf { get; set; }
        [FilterField(FilterFieldType.Profession, "Nurse")] public bool? Nurse { get; set; }

        public IEnumerable<GalleryFilterNameDisplayPair> Enumerate(FilterFieldType filterType)
        {
            return GetType().GetProperties()
                .Where(x => x.GetCustomAttribute<FilterFieldAttribute>().FilterType == filterType)
                .Select(x =>
                {
                    var attribute = x.GetCustomAttribute<FilterFieldAttribute>();
                    return new GalleryFilterNameDisplayPair()
                    {
                        FieldName = x.Name,
                        DisplayText = attribute.DisplayName,
                        Property = x
                    };
                })
                .OrderBy(x=>x.DisplayText);
        }
    }
}