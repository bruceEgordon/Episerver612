
Front: http://localhost:65461/
Back: http://localhost:49158/
https: https://localhost:44328/

====================================
ECF 12 
 - Site upgraded, CM works
 - Payment-Proj updated and added to the solution
 - ServiceAPI client, no changes... seems to work (read)
	Upload - not tseted
 - The Shipping provider updated (pseudo coding for now)
	Need further work, and test
 - Had a strange error with my TaxCalculator, removed for now, cleaned and reinserted ... but no calc.
	A lot has happened, I'm using obsoleted stuff... need to have a look
		CalculateSaletaxes - Reflector craches
		CalculateShippingtax - Reflector craches
		GetSalesTax, GetShippingTax and a few more not marked as obsoleted
		...lots of obsoleted things in the "DefaultTaxCalculator""
 - All calculators has changed - need a second look
	...also for Fund & Bruce


===============================

New inventoryCheck (zero in stock first) with AdjustOrRemove (StartPage)
Comparing "AdjustOrRemove" and the lower level service

 - All disabled (zero in stock)
	can buy, silent validation
 - Enable at entry (record & wh disabled)
	removed due to disabled wh
		enabling the wh (no stock)
			Validation silent - get cart & LI
			Seems like the record is ignored ... as it may should, but Entry is enabled
				Enabling the record
					removed due to "no stock" ... as it should
					Get an empty cart
		wh enabled (with stock, record disabled)
			Can buy - get a cart + LI - silent validation
				?? Seems like record is ignored ... but it mattered with no stock..??
				Get a cart & LI
	- Diable on the Entry (wh, record enabled - no stock)
		Silent validation
		Get Cart & LI

 - Record disabled + zero in stock
	can add, get cart - silent validation .. as above
		Enabling the record
			removed ...no stock - get an empty cart .. as above
  - WH enabled (zero in stock)
	validation removes because "zero in stock"
		(in memoryLineItem)
	adding to stock, but record is disabled
		?? Gets in there, cart exist after ... as it should...or...??
			Disbling inventory on the entry
			...no effect, gets in there ... as it should

---

Doing it with the service, more detailed and low level (AdminPage)
 - Everything disabled (but we have stock)
	"Success" as ResponseType
	It's a success
	No change in stock
	Request increases
	(Cancel crash)
 - Enable the Entry only - is this even used??
	Success
	No change in stock
	Request goes up
	(Cancel crash)
 - Enable the record (with stock) - this matters 
	Success
	decrease in stock, reserved goes up
		Cancel (enabled now)
			increase in stock, reserved goes down
			Disable the record
				Success
				Requested goes up, no change in stock, get the key
				Cancel is ignored
					Enable the record and zero in stock
						"NotEnough"
						Cancel crash
							Requested = Available (12)
								r up a down ... expected...eventually "NotEnough"
								Cancel = ok

 - Enable The warehouse (all is enabled)
	Success + Success
	Requested increases and decreases when doing a cancel
 - Entry & wh is enabled... not the record, and cancel is "disabled" in code
	Requested increases, but no change in available ... fishy
 - Entry & record disabled
	Requested increases, but no change in available ... fishy
 - WH disabled (all disabled)
	Requested increases, but no change in available ... fishy
 - Entry & record enabled, not the WH
	Stock decrease, requested increase
		...wh-setting don't matter, it seems
		Cancel was disabled

		enabling cancel
		Increase & decrease works fine - request & cancel ... no crash


PurchaseRequestedQuantity goes up ... fishy, see above
Test all the Cancel-Crashes again
Test a misspelled/empty WH .. see if the messages "are valid"
IN CM (pricing tab) Track Inventory correspond to "Is tracked" in CMS-UI 
The column PurchaseRequestedQuantity is the one that takes care of reservations
ItemIsUntracked is never hit
NotEnough is hit
PurchaseRequestedQuantity is not represented in any UI

Tuan Doan - is this the goto-guy?


Added here after "CleanUp"
 - ...\Infrastructure\MarketingInitialization.cs
 - A check on CheckOnParentAndNode()
	Parent is always the node
	Children is only the ContentModel navigation stuff
		A Package doesn't have "children"
	It's all about Relations now
	NodeEntryRelation is new in 11 (don't use the older NodeRelation)
		Parent - child - sort ... etc.
	URL-changes - still the ParentLink - but, Relations build the URL
		used to be based on SortOrder
		...but behaves as before 11 with the built-in stuff
			...can make it custom (Rahl-blog)



====================================
 - CustomTaxCalculator
	Exec-order
		.ctor
		CalculateTaxTotal
		GetTaxValues
		... new info/findings in the file... as comments

 - The Solution-CleanUp-Proj is started, but need to fix OrderControllers here first, and then "copy over"
	...clean-up paused for now
 - Things changed here to be copied over ... but more need to change
	CatalogControllerBase (MyControllerBase) - done
	...\Infrastructure\MyCustomMarketService.cs - done
	is this one used ...\Controllers\MarketController.cs ?? ... excluded now - check to see if it breaks
	VariationController - done
	OrderControllerBase - done 
	CartController - done
	TrousersController - Done
	CartAndCheckout\CartAndCheckoutService.cs - Done

Latest changes (after sending stuff to Damien)
 - Taxes (added - o	Retrieve taxes, 2 ways:)
 - Show slides and Commerce Manager first up to the “Jurisdiction-code slide” ... added

 - Added some info for the CustomBfHandler to Trainerguide
	I put a few lines in the Configs\DataManager file in CM
	Nothing is tested or even started yet ... doing it so far for Damien & T-Guide

 - The pattern of the provider model - added to T-Guide 

 - Search
	Removed the UnifiedSearch part
	Retained and re-written the "Setting-Up" part (for people new to Find)
	Changed/Added/Removed the "namtiv-indexing" of OrderValues & AddressValues to just debug/inspect...
		...supposed to be a teaser for the Find-Dev-Course



====================================
Moving it all to 11.2.6
 - CustomIndexBuilder (new one)... works after update
	Added to T-Guide

 - Something fishy with the LineItem-validation (have custom one, but don't get hit)
	...check this

 - PaymentGateway added (\Bin both sites) IPaymentPlugIn - the newest one
	Added to T-Guide

 - Rendering my %-promo on PromotionPage
 	Added to T-Guide (and the proj)

 - Taxes (US-article found)
  When an item is purchased on sale, is sales tax due on the original price or the reduced price of the item?
  If the item is on sale at a reduced price, or with a store coupon issued by the seller, sales tax is charged 
	on the reduced price.  
  If the reduced price is the result of a manufacturer's reimbursement, such as a manufacturer's coupon 
	or rebate or any other third-party's reimbursement, then the sales tax is charged on the full price 
	of the item without regard to the reimbursement to the seller.
  See Tax Bulletin ST-140 (TB-ST-140) Coupons and Food Stamps.
  See Tax Bulletin ST-860 (TB-ST-860) Taxable Receipt - How Discounts, Trade-Ins, and Additional Charges Affect Sales Tax
  "Handling" should be included in VAT/Tax

ToDo...
 - Override default TaxCalculator
 - Clean up in "PricingServices"... have a new one
 - Check the Default Tax Category ... seems broken... use to work nice
 have
	C:\Episerver6\CommerceTraining\CommerceTraining\Infrastructure\Promotions\CustomPromotionEngineContentLoader.cs
	...kolla i Fund/ga: Adv ... tror jag har ett par till någonstans man kan visa (is in "TheLatest"-proj)
 - Go through Split Pay/Ship-code + T-Guide ... as a unit
 - Go trough PriceCalculators... clean-up & unify

Optimized Prices 
..okay, seem to work now ... spoke with Quan about it
...Infrastructure\Pricing\CustomIPriceOptimizer.cs + added line to Init-Mod.
Added the "sv"-market to demo the stuff (add this to the original pricing demo)
...also works with "Arindam-Prices" 1--> 10 & 2 --> 11 
	... all done in the "sv"-market
		Added some of this to T-Guide

OddBirds
	ProdLine - ProdColor - SKU_Size
	...is there now, but the model is misspelled :)
		Added to T-Guide

Changed the "topLevelCategories" for the StartPage to IContent...
	...so we can use the FilterForVisitor.Filter(...)
		...else we see the ServiceApi-demo-stuff

BF - Added (comes from the cmd-app)
 - GetPoData - using undocumented BF-func.
 	Added to T-Guide


====================================
Fishy
 - WH/Inv - my code works, but ComMan doesn't show it properly
	Reserve/Cancel/Put back - inventory
 - ServiceAPI-docs are missing "IsPrimary" ... works here in my example

====================================
Need to fix
 CreateWithCode() in BlouseController works...Parent/Child in 11.2
	...this is done in "Fund" ... corrected there... and here

Markets for the shirt "not white" ...?

====================================

All installed:
 - CMS
 - ECF
 - CM
 - Find.Commerce
	EPiServer.Commerce.Core (>= 11.0.0 && < 12.0.0)
	epi.Find.CMS
	epi.Find.Framework
	update-epidatabase needed
	...manual paste-in of config
 - ServiceAPI

 Seem to work

 ==============================

Checking if code works in 11
 - AdminPage
	Markets - okay, but doesn't show up in CM (visible after a while)
	Taxes - okay, but cannot not be set on Node/Sku - check that (is there in CM)
		Works if you create in the node directly (no prod)
		Have some new code in SetDefaultValues() for the SKU
	Warehouses - doesn't show up in CM, They are created but cannot assign Inv-Rec in CMS-UI...?
		Show up after a restart of it all in CM
		Still nothing when assigning in CMS-UI... visible in CM directly... strange...?
	Create node/entry...nope... is code missing?
	BF
		Create - nothing in CM ... but no error
		Have to set permissions to see it... remember...?
			Method not found from the view...?
			Exec when the view loads... then it worked
			
A simple CheckOut works (the white shirt only)

	Shipping-params code work

	Split pay & ship works ... after som hassle
		Cart did not work with custom fields
		...and I had to not use SerializedCart ... swapped to older ... check this...
			...have new info about it... have a look
		Serialized used in \Episerver5\ ... Fund
			Have added MDP-stuff --> name-lookup

	ServiceAPI works
		...incl. "IsPrimary"

	AssociationGroups works
		...have to test the rest, but it's probably un-changed








