using System;
using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.SpecializedProperties;
using EPiServer.Commerce.Catalog.DataAnnotations;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.ServiceLocation;
using EPiServer;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Managers;
using EPiServer.Commerce.Catalog.Linking;
using System.Collections.Generic;
using System.Collections;

namespace CommerceTraining.Models.Catalog
{
    [CatalogContentType(MetaClassName = "Shirt_Variation"
        , DisplayName = "ShirtVariation"
        , GUID = "c0058f2d-9893-41d9-8c19-19c94d34ded1"
        , Description = "Use with mens shirts")]
    public class ShirtVariation : DefaultVariation
    {
        //[IncludeInDefaultSearch]
        //[CultureSpecific]
        //[Display(
        //    Name = "Main body",
        //    Description = "The main body, for text, images and tables.",
        //    GroupName = SystemTabNames.Content,
        //    Order = 1)]
        //public virtual XhtmlString MainBody { get; set; }

        [Searchable]
        [IncludeInDefaultSearch]
        public virtual string Size { get; set; }

        [Searchable]
        [IncludeValuesInSearchResults]
        public virtual string Color { get; set; }

        public virtual bool CanBeMonogrammed { get; set; }

        // added for adv. 
        //public virtual bool RequireSpecialShipping { get; set; }... moved into "base"

        [Searchable]
        [IncludeValuesInSearchResults]
        [Tokenize]
        [IncludeInDefaultSearch]
        public virtual string ThematicTag { get; set; }

        /* Adv. below */

        public virtual ContentArea ProductArea { get; set; }

        public virtual decimal Margin { get; set; } // Added for Adv + Find (.InRange)

        [Searchable]
        [IncludeValuesInSearchResults]
        [IncludeInDefaultSearch]
        public virtual string Brand { get; set; } // added for Adv. + Find ()

        public virtual string theTaxCategory { get; set; }


        Injected<IContentLoader> _loader;
        //Injected<ReferenceConverter> _refConv;
        public override void SetDefaultValues(ContentType contentType)
        {
            #region JustChecking - prob. no demo of "What is my parent"

            //contentType = ID = 27, Name = "SomeVariation"


            #region Used to check


            //CatalogContentBase theOut0 = null;
            //var stuff0 = _loader.Service.TryGet<CatalogContentBase>(this.ParentLink, out theOut0); // could use this
            //// this gives the product if created on the Product
            //// gets the shirt node if created direct in the node

            //EntryContentBase theOut00 = null;
            //var stuff00 = _loader.Service.TryGet<EntryContentBase>(this.ParentLink, out theOut00); // could use this
            //                                                                                       // SameSame when doing on the Product
            //                                                                                       // gets null if created direct in the node...of course
            //                                                                                       // gets the Product if assigent to one

            #endregion

            //var cat = theOut00.Categories; // seems to be CMS-categories ... error if created in the node directly
            // no error if done at the product

            //var cat1 = theOut00.GetNodeRelations(); // gets the relations... error if created in the node
            //var t = cat1.GetEnumerator().Current.Target; gets error

            //var t = theOut0.GetOriginalType();
            //var v = this.ParentLink;

            // Changed i 9 (triggers twice now, not four times)
            // ...see the ReadMe.txt in NumberNine ECF-8.17 for more info...
            // 1073741825, SomeNode is "TheParentNode"... gets that when "outside" of a product
            // Need to walk up the hierarchy to find the node ... when "doing it" on a Product

            #endregion

            base.SetDefaultValues(contentType);

            #region Old garbage

            // ...For the lab (if any), Added a null-check if the editor did not choose any category 
            //CatalogContentBase theOut = null; // would like more flexibility here
            //var stuff2 = _loader.Service.TryGet<CatalogContentBase>(this.ParentLink, out theOut); // cannot use this
            // Gets null for the above
            //var c = this.GetCategories();
            //var xx = this.ParentLink.GetOriginalType();
            //var yy = stuff2.GetOriginalType();


            //var relations = this.GetNodeRelations(); // error if done on the node 
            //var stuff3 = _loader.Service.TryGet<ProductContent>(this.ParentLink, out onProduct); // could use this

            // could do a check if node or product
            // NodeEntryrelation have IsPrimary
            // just checking the parent... could have several levels of products
            // check if NodeEntryRelation is set here

            //NodeEntryRelation 11.2 & ServiceAPI is different with this thing

            /*
            var rels = this.GetNodeRelations(); // not set yet, unusable
            foreach (var item in rels)
            {
                NodeEntryRelation r = (NodeEntryRelation)item;
                if (r.IsPrimary)
                {
                    var id = r.Child.ID;
                }
            }
            */

            //CatalogContentBase theOut4 = null; // would like more flexibility here

            #endregion

            CatalogContentBase myParent = _loader.Service.Get<CatalogContentBase>(this.ParentLink); //

            // just checking, used in "old garbage"
            //IEnumerable<IContent> ans = _loader.Service.GetAncestors(myCategory.ContentLink);

            #region More old garbage
            //List<IContent> ans2 =

            //var z = this.Categories; // nothing, but some other category
            //var zz = this.GetCategories(); // nothing, not set yet

            //string taxCat = String.Empty;
            //ContentReference ccref = null;

            #endregion

            // Changed so the ServiceAPI works
            if (myParent.GetOriginalType() == typeof(NodeContent))
            {

                FashionNode fashionNode = (FashionNode)myParent;

                // sooo much easier now 
                this.TaxCategoryId = int.Parse(fashionNode.TaxCategories);
                this.theTaxCategory = CatalogTaxManager.GetTaxCategoryNameById(Int16.Parse(fashionNode.TaxCategories));

            }

            #region Oldest garbage

            //foreach (var item in ans) // Nodes
            //{
            //    if (item.GetOriginalType() == typeof(FashionNode)) // so it doesn't crash in the catalog
            //    {
            //        var item2 = (FashionNode)item;
            //        if (item2.TaxCategories != "")
            //        {
            //            taxCat = item2.TaxCategories;
            //            ccref = item2.ContentLink;
            //            break;
            //        }

            //    }

            //}


            //var a = ans.GetEnumerator().Current;
            //var aa = ans.GetEnumerator().MoveNext().GetOriginalType();


            //ContentReference ppp = null;
            //var ccc = ans.GetEnumerator();
            //ccc.MoveNext();
            //var c = ccc.Current.ContentLink;
            //while (ccc.Current.GetOriginalType() == typeof(EntryContentBase))
            //{
            //    ppp = ccc.Current.ContentLink;
            //}

            //var daStuff = _loader.Service.Get<FashionNode>(ccref);
            //var t = daStuff.TaxCategories;

            //if (daStuff != null) // in other words we got the "right thing"
            //{
            //    FashionNode node = (FashionNode)daStuff;
            //    if (node.TaxCategories != null) // need to check this
            //    {
            //        //we have the parent node
            //        this.TaxCategoryId = Int16.Parse(node.TaxCategories); // change this to int in the model
            //        // ...have a look if the Tax-drop-down in pricing dialog changes to the string... yes it does
            //        this.theTaxCategory = CatalogTaxManager.GetTaxCategoryNameById(Int16.Parse(node.TaxCategories));
            //        this.MainBody = new XhtmlString(theOut.Name);
            //    }
            //}

            //do
            //{

            //    //CatalogContentBase theOut3 = null; // would like more flexibility here
            //    //pp = _loader.Service.Get<CatalogContentBase>(pp.ParentLink); // cannot use this
            //    p.ParentLink.GetOriginalType();
            //    var check = typeof(EntryContentBase);
            //    if (p.GetOriginalType() == typeof(FashionNode))
            //    {
            //        b = true;
            //        p = _loader.Service.

            //    }
            //    FashionNode theOut4 = null;
            //    var pp = _loader.Service.TryGet<FashionNode>(stuff4.ParentLink, out theOut4);

            //} while (b);


            // Original
            //if (stuff2) // in other words we got the "right thing"
            //{
            //    FashionNode node = (FashionNode)theOut;
            //    if (node.TaxCategories != null) // need to check this
            //    {
            //        //we have the parent node
            //        this.TaxCategoryId = Int16.Parse(node.TaxCategories); // change this to int in the model
            //        // ...have a look if the Tax-drop-down in pricing dialog changes to the string... yes it does
            //        this.theTaxCategory = CatalogTaxManager.GetTaxCategoryNameById(Int16.Parse(node.TaxCategories));
            //        this.MainBody = new XhtmlString(theOut.Name);
            //    }
            //}

            #endregion
        }


    }
}