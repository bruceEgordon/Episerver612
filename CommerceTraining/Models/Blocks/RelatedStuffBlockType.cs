using System;
using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.ServiceLocation;
using EPiServer;
using EPiServer.Web.Routing;

namespace CommerceTraining.Models.Blocks
{
    [ContentType(DisplayName = "RelatedStuffBlockType", GUID = "ccb3fa4c-f797-4d1c-ab05-5de2e9deef2b", Description = "")]
    public class RelatedStuffBlockType : BlockData
    {

        [CultureSpecific]
        [Display(
            Name = "Name",
            Description = "Name field's description",
            GroupName = SystemTabNames.Content,
            Order = 1)]
        public virtual String Name { get; set; }

        public virtual string RelatingTo { get; set; }

        Injected<IPageRouteHelper> p_helper;
        Injected<IContentLoader> loader;
        Injected<IContentRouteHelper> helper; // ...ContentRouteHelper obsoleted in 10 

        public RelatedStuffBlockType() // can´t do ?
        {
            // hits this... gets "Somthing went wrong" - site works ... Edit doesn´t work

            // taken out nov-16
            //ContentReference someRef = helper.Service.ContentLink; // ok, so far ... boom here
            //this.theRef = someRef; // ok, but it gets overritten by the prop
            //OtherClass.proppen = someRef;
            
            
            
            //string x = loader.Service.Get<IContent>(theRef).Name; // the variation // error here 
            //this.RelatingTo = x; // as a test
        }

        public override void SetValue(string index, object value)
        {
            base.SetValue(index, value);
        }

        /*Type 'System.Collections.Generic.IEnumerable`1[[System.String, mscorlib, Version=4.0.0.0, 
 * Culture=neutral, PublicKeyToken=b77a5c561934e089]]' could not be mapped to a PropertyDefinitionType */
        //public virtual IEnumerable<string> Group { get; set; }

        // Associations - prop ger bara det som dyker upp i CMS Cat-Edit R/O

        // Type 'EPiServer.Commerce.SpecializedProperties.PropertyDictionarySingle' 
        // could not be mapped to a PropertyDefinitionType
        // public virtual PropertyDictionarySingle Dict { get; set; }

        public virtual string Dict { get; set; } // boring

        /*Type 'EPiServer.Core.PropertyList`1[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]' 
         * could not be mapped to a PropertyDefinitionType*/
        //public virtual EPiServer.Core.PropertyList<string> ListDict { get; set; } 

        //Type 'EPiServer.SpecializedProperties.PropertyCheckBoxList' could not be mapped to a PropertyDefinitionType
        //public virtual EPiServer.SpecializedProperties.PropertyCheckBoxList chkList { get; set; }

        // not needed... or?
        [UIHint(EPiServer.Commerce.UIHint.CatalogEntry)]
        public virtual ContentReference theRef { get; set; }

        // it says it´s not used anywhere
        // PriceTypeSelector doesn´t show
        // some errors - look above

        //public override 
    }

    // it gets too static ... lingers as long the block exists ... takes "the first woken up" 
    public static class OtherClass
    {
        public static ContentReference proppen { get; set; }

    }
}