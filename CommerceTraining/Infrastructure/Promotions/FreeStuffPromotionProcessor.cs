﻿using EPiServer;
using EPiServer.Commerce.Marketing;
using EPiServer.Commerce.Marketing.Extensions;
using EPiServer.Commerce.Marketing.Promotions;
using EPiServer.Commerce.Order;
using EPiServer.Core;
using EPiServer.Framework.Localization;
using Mediachase.Commerce.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommerceTraining.Infrastructure.Promotions
{
    public class FreeStuffPromotionProcessor : EntryPromotionProcessorBase<FreeStuffPromotion>
    {
        private CollectionTargetEvaluator _collectionTargetEvaluator;
        private FulfillmentEvaluator _fulfillmentEvaluator;
        private LocalizationService _localizationService;
        private IContentLoader _contentLoader;
        private ReferenceConverter _referenceConverter;

        public FreeStuffPromotionProcessor(CollectionTargetEvaluator collectionTargetEvaluator, FulfillmentEvaluator fulfillmentEvaluator,
            LocalizationService localizationService, IContentLoader contentLoader, ReferenceConverter referenceConverter,
            RedemptionDescriptionFactory redemptionDescriptionFactory) : base(redemptionDescriptionFactory)
        {
            _collectionTargetEvaluator = collectionTargetEvaluator;
            _fulfillmentEvaluator = fulfillmentEvaluator;
            _localizationService = localizationService;
            _contentLoader = contentLoader;
            _referenceConverter = referenceConverter;
        }
        protected override RewardDescription Evaluate(FreeStuffPromotion promotionData, PromotionProcessorContext context)
        {
            ContentReference freeItem = promotionData.FreeItem.Items.First();
            string freeItemCode = _referenceConverter.GetCode(freeItem);

            var lineItems = context.OrderForm.GetAllLineItems(); //GetAllLineItems extension method needs using for EPiServer.Commerce.Order

            IList<string> skuCodes = _collectionTargetEvaluator.GetApplicableCodes(lineItems, promotionData.FreeItem.Items, promotionData.FreeItem.MatchRecursive);

            FulfillmentStatus status = promotionData.RequiredQty.GetFulfillmentStatus(context.OrderForm, _collectionTargetEvaluator, _fulfillmentEvaluator);

            return RewardDescription.CreateFreeItemReward(status, GetRedemptions(skuCodes, promotionData, context),)
        }

        protected override PromotionItems GetPromotionItems(FreeStuffPromotion promotionData)
        {
            throw new NotImplementedException();
        }
    }
}