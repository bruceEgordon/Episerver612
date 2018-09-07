using EPiServer.Find;
using SpecialFindClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommerceTraining.SupportingClasses
{
    public class BoughtThisBoughtThat
    {
        
        public IEnumerable<string> GetItems(string entryCode)
        {
            List<string> localList = new List<string>();

            IClient client = Client.CreateFromConfig(); // make the "client" more permanent?

            var result = client.Search<OrderValues>() // could be just the items in a separate class --> more precise
                .For(entryCode)
                .InField("LineItemCodes")
                // bad date-filter, need new one - check in cmd-app
                //.Filter(f=> f.orderDate.InRange(DateTime.UtcNow,DateTime.UtcNow.AddMonths(-1)))
                //                .FilterHits(f=>f.LineItemCodes.)
                .GetResult();

            // can do smarter, but it´s explicit :)
            foreach (var item in result)
            {
                foreach (var item2 in item.LineItemCodes)
                {
                    if (item2 != entryCode) // excluding what was searched for
                    {
                        if (localList.Contains(item2))
                        {
                            // do nothing
                        }
                        else // add it
                        {
                            localList.Add(item2);
                        }

                    }
                }
            }

            return localList;
        }

    }
}