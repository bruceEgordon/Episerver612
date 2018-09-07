using System;
using System.Collections.Generic;
using System.Text;
using EPiServer.Security;
using Mediachase.Commerce.Marketing.Objects;
using Mediachase.Commerce.Marketing;
using Mediachase.Commerce.Catalog.Objects;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Customers.Profile;
using System.Web.Security;
using Mediachase.Commerce.Security;
using Mediachase.Commerce.Customers;
using Mediachase.Commerce.Marketing.Dto;
using Mediachase.Commerce.Marketing.Managers;
using Mediachase.Commerce;
namespace CommerceTraining.Infrastructure.Pricing
{
    public class CustomPromotionHelper
    {
                PromotionContext _Context = null;

        /// <summary>
        /// Gets or sets the promotion context.
        /// </summary>
        /// <value>The promotion context.</value>
        public PromotionContext PromotionContext
        {
            get { return _Context; }
            set { _Context = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PromotionHelper"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public CustomPromotionHelper(PromotionContext context)
        {
            InitPromotionContext();
            _Context = context;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PromotionHelper"/> class.
        /// </summary>
        public CustomPromotionHelper()
        {
            InitPromotionContext();
            _Context = new PromotionContext(MarketingContext.Current.MarketingProfileContext, null, null);
        }

        /// <summary>
        /// Evals the specified filter.
        /// </summary>
        /// <param name="filter">The filter.</param>       
        public void Eval(PromotionFilter filter)
        {   // gets in here...and then nothing
            MarketingContext.Current.EvaluatePromotions(true, this.PromotionContext, filter, null, false);
        }

        /// <summary>
        /// Evals the specified filter with checkEntryLevelLimit
        /// </summary>
        /// <param name="filter"> The filter</param>
        /// <param name="checkEntryLevelLimit"> The check Entry Level Limit</param>
        public void Eval(PromotionFilter filter, bool checkEntryLevelLimit)
        {
            if (checkEntryLevelLimit)
            {
                Dictionary<PromotionDto.PromotionRow, decimal?> entryDiscountApplicationCount = new Dictionary<PromotionDto.PromotionRow, decimal?>();
                PromotionDto promotionDto = PromotionManager.GetPromotionDto(FrameworkContext.Current.CurrentDateTime);
                foreach (PromotionDto.PromotionRow promotion in promotionDto.Promotion)
                {
                    if (!promotion.IsMaxEntryDiscountQuantityNull())
                    {
                        entryDiscountApplicationCount.Add(promotion, promotion.MaxEntryDiscountQuantity);
                    }
                }
                MarketingContext.Current.EvaluatePromotions(true, this.PromotionContext, filter, entryDiscountApplicationCount, true);
            }
            else
            {
                Eval(filter);
            }
        }

        /// <summary>
        /// Inits the promotion context.
        /// </summary>
        public void InitPromotionContext()
        {
            // We assume if there is already shopping cart context defined for the current http request, that we do not need to reinitialize it again
            if (!MarketingContext.Current.MarketingProfileContext.ContainsKey(MarketingContext.ContextConstants.ShoppingCart))
            {
                SetContext(MarketingContext.ContextConstants.ShoppingCart, OrderContext.Current.GetCart(Cart.DefaultName, PrincipalInfo.CurrentPrincipal.GetContactId()));

                CustomerContact cutomerContact = PrincipalInfo.CurrentPrincipal.GetCustomerContact();
				if (cutomerContact != null)
                {
					SetContext(MarketingContext.ContextConstants.CustomerContact, cutomerContact);
					Guid contactId = (Guid)cutomerContact.PrimaryKeyId;
                    Guid organizationId = Guid.Empty;
					if (cutomerContact.ContactOrganization != null)
                    {
						organizationId = (Guid)cutomerContact.ContactOrganization.PrimaryKeyId;
                    }

					SetContext(MarketingContext.ContextConstants.CustomerSegments, MarketingContext.Current.GetCustomerSegments(contactId, organizationId));
                }

                // Set customer promotion history context
                SetContext(MarketingContext.ContextConstants.CustomerId, PrincipalInfo.CurrentPrincipal.GetContactId());
            }
        }         

        /// <summary>
        /// Sets the context.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="val">The val.</param>
        private void SetContext(string key, object val)
        {
            if (!MarketingContext.Current.MarketingProfileContext.ContainsKey(key))
                MarketingContext.Current.MarketingProfileContext.Add(key, val);
            else
                MarketingContext.Current.MarketingProfileContext[key] = val;
        }


    }
}