using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SignalR_Test.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
           
            return View();
        }
        public ActionResult Chat()
        {
            return View();
        }
        public decimal CalculateCartShippingCost(Cart cart, out string error)
        {
            decimal childMultiplier = 1;
           

            
            var ShippingCost = 0;

            if (cart.BookListOrderID.HasValue)
            {

                System.Collections.Generic.List<CartItem> newcartlist = new System.Collections.Generic.List<CartItem>();
                bool IsEbook = true;
                childMultiplier = 1;
                if (cart.ChildList != null && cart.Items.Count > 0)
                {
                    foreach (var child in cart.ChildList)
                    {
                        foreach (var item in (cart.School.BookLists[child.YearLevel].SubjectLists.Values.Select(p => p.Items)))
                        {
                            
                            //var aa = (id.Items[1]).Items[0];
                            foreach (var subitem in item)
                            {
                                CartItem cartitemss = cart.Items.FirstOrDefault(m => m.BookListOrderItemID.Equals(cart.School.BookLists[child.YearLevel].SubjectLists.Values.);


                                var itemm = cart.Items.Where(m => m.BookListOrderItemID.Equals(subitem.ID)).ToList();

                                CartItem cartitem = cart.Items.FirstOrDefault(m => m.BookListOrderItemID.Equals(subitem.ID));
                                if (cartitem != null)
                                    newcartlist.Add(cartitem);
                                if (!subitem.IsEBook)
                                {
                                    IsEbook = false;
                                }

                            }
                        }
                        childMultiplier = childMultiplier + 1;
                    }
                }

            }

          
            //return  childMultiplier * this._CalculateCartShippingCost(cart, out error);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }


    public class CartItem
    {
        public int ID { get; set; }
        
        public string Description { get; set; } // This is added incase product description changes later so prices are repeatable
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public bool GST { get; set; }
        public int? BookListOrderItemID { get; set; }
        public bool IsEBookFulfilled { get; set; }
        public bool IsEBook { get; set; }
    }

    public class Cart
    {
        public int? BookListOrderID { get; set; }
        public DateTime? Expires { get; set; }
        public int ID { get; set; }
        public Guid VertificationCode { get; set; }
        public List<CartItem> Items { get; set; }


        public bool Locked { get; set; }

        public bool TotalDirty { get; set; }

        public decimal ShippingCost { get; set; }

        public IEnumerable<Child> ChildList { get; set; }

        public School School { get; set; }

        public bool BooklistOnly { get; set; }

        public string YearLevel { get; set; }

        public DateTime? LockDate { get; set; }

        #region "Dirty Total Fields"
        public decimal? SubTotal { get; set; }

        public decimal? DiscountGSTItems { get; set; }

        public decimal? DiscountNonGSTItems { get; set; }

        public decimal? SubTotalGSTItems { get; set; }
        public decimal? SubTotalNonGSTItems { get; set; }
        #endregion


        #region "Calculated Fields"
        public decimal Discount
        {
            get
            {
                if (TotalDirty && !Locked)
                {
                    
                }
                return DiscountGSTItems.Value + DiscountNonGSTItems.Value;

            }
        }
        public decimal Total
        {
            get
            {
                if (TotalDirty && !Locked)
                {
                    
                }

                return (SubTotalGSTItems.Value - DiscountGSTItems.Value) + (SubTotalNonGSTItems.Value - DiscountNonGSTItems.Value) + ShippingCost;
            }
        }
        public decimal GST
        {
            get
            {
                

                return (SubTotalGSTItems.Value - DiscountGSTItems.Value + ShippingCost) / 11M;
            }
        }

        #endregion

       


    }
    public class School
    {
        [DisplayName("Lowest Year Level")]
        public int MinYear { get; set; }
        [DisplayName("Highest Year Level")]
        public int MaxYear { get; set; }
        public int ID { get; set; }
        [DisplayName("School Name")]
        public string Name { get; set; }
        [DisplayName("Menu Text")]
        public string MenuText { get; set; }
        [DisplayName("Slug (Url)")]
        public string Slug { get; set; }
        [DisplayName("Enable Online Orders")]
        public bool AllowOrders { get; set; }
        [DisplayName("School Pickup Date")]
        public DateTime? PickupDay { get; set; }
        [DisplayName("School Pickup Time")]
        public string PickupTimeRange { get; set; }
        [DisplayName("School Delivery")]
        public decimal? SchoolDeliveryCharges { get; set; }
        [DisplayName("Store Collection")]
        public decimal? StoreCollectionCharges { get; set; }
        [DisplayName("Home Delivery")]
        public decimal? HomeDeliveryCharges { get; set; }
        public IDictionary<int, YearBookList> BookLists { get; set; }
        public IEnumerable<KeyValuePair<int, YearBookList>> BookListSetter
        {
            set
            {
                if (BookLists == null)
                {
                    BookLists = new Dictionary<int, YearBookList>();
                }
                foreach (var kvp in value)
                {
                    BookLists.Add(kvp);
                }
            }
        }
        /*   public WebImage Logo { get; set; }*/

        public string TemplateName { get; set; }

        public string CustomMessage { get; set; }

        public bool UseGroupedSubjectWizard { get; set; }

        public bool ShowHistory { get; set; }

        public bool IsOtherSchool { get; set; }
    }
    public class YearBookList
    {
        public int ID { get; set; }
        public string YearLevel { get; set; }
        public bool Ready { get; set; }
        public byte[] PDFBookList { get; set; }
        public int ZOrder { get; set; }
        public DateTime CutOffDate { get; set; }
        public DateTime ReviewDate { get; set; }
        public bool IsTeacherEditable { get; set; }
        public bool IsApproved { get; set; }
        public IEnumerable<Guid> ApprovedBy { get; set; }
        public bool IsEmailRequired { get; set; }
        public string ApprovedBySetter
        {
            set
            {
                ApprovedBy = String.IsNullOrEmpty(value) ? new Guid[0] : value.Split(',').Select(a => new Guid(a));
            }
        }
        public IDictionary<int, SubjectBookList> SubjectLists;
        public IEnumerable<KeyValuePair<int, SubjectBookList>> SubjectListSetter
        {
            set
            {
                if (SubjectLists == null)
                {
                    SubjectLists = new Dictionary<int, SubjectBookList>();
                }
                foreach (var kvp in value)
                {
                    SubjectLists.Add(kvp);
                }
            }
        }

       

        public bool IsSchoolSupplied { get; set; }

    }
    public class Child
    {
        public int ID { get; set; }
        public int? SchoolID { get; set; }
        //public string Email { get; set; }
        public int YearLevel { get; set; }
        /*public string Name { get; set; }*/
        public string FirstName { get; set; }
        public int? SubjectGroupItemId { get; set; }
        public string FamilyName { get; set; }
        public Dictionary<int, int> BookListItems { get; set; } //key=BookListItem.ID value = quantity
        public IEnumerable<KeyValuePair<int, int>> BookListItemsSetter
        {
            set
            {
                BookListItems = new Dictionary<int, int>();
                foreach (var kvp in value)
                {
                    BookListItems.Add(kvp.Key, kvp.Value);
                }
            }
        } //key=BookListItem.ID value = quantity
        public Dictionary<int, int> UpSellItems { get; set; }  //key=Product.ID value=quantity

        public string Email { get; set; }
    }
    public class SubjectBookList
    {
        public int ID { get; set; }
        public int SubjectID { get; set; }
        public dynamic Subject { get; set; }
        public int YearLevel { get; set; }
        public IList<BookListItem> Items { get; set; }
        public int ZOrder { get; set; }
    }
    public class BookListItem
    {
        public int ID { get; set; }
        public int Quantity { get; set; }
        public int? ProductID { get; set; }
        /* public LightProduct Item
         {


         } //can be null
        */

        public int VersionID { get; set; }
       
        public String HtmlDescription { get; set; }
        public String Description { get; set; }
        public decimal Price { get; set; }
        public string ISBN { get; set; }


        public bool IsCompulsory { get; set; }

        public bool IsEBook { get; set; }

        public int SubjectID { get; set; }

        public bool IsAttentionRequired { get; set; }

        public bool IsNewEdition { get; set; }
    }
}