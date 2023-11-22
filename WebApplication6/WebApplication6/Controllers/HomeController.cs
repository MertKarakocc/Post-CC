using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ThreeDPayment.Helpers;
using ThreeDPayment.Models;

namespace WebApplication6.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(CreditCardModel model)
        {
            TempData["CreditCard"] = model;

            return RedirectToAction("ThreeDGate");
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
        public ActionResult ThreeDGate()
        {
            
            var cardModel = TempData["CreditCard"] as CreditCardModel;
            if (cardModel == null)
                return RedirectToAction("Home");

            string processType = "Auth";
            string clientId = " ";
            string storeKey = " ";
            string storeType = "3d_pay_hosting";
            string successUrl = "http://localhost:56232/Home/Success";
            string unsuccessUrl = "http://localhost:56232/Home/UnSuccess";
            string randomKey = ThreeDHelper.CreateRandomValue(10, false, false, true, false);
            string installment = "1";
            string orderNumber = ThreeDHelper.CreateRandomValue(8, false, false, true, false);//Sipariş numarası
            string currencyCode = "949";
            string languageCode = "tr";
            string cardType = "1"; 
            string orderAmount = "0.01";


            string hashFormat = clientId + orderNumber + orderAmount + successUrl + unsuccessUrl + processType + installment + randomKey + storeKey;

            var paymentCollection = new NameValueCollection();

            //Mağaza bilgileri
            paymentCollection.Add("hash", ThreeDHelper.ConvertSHA1(hashFormat));
            paymentCollection.Add("clientid", clientId);
            paymentCollection.Add("storetype", storeType);
            paymentCollection.Add("rnd", randomKey);
            paymentCollection.Add("okUrl", successUrl);
            paymentCollection.Add("failUrl", unsuccessUrl);
            paymentCollection.Add("islemtipi", processType);
            //Ödeme bilgileri
            paymentCollection.Add("currency", currencyCode);
            paymentCollection.Add("lang", languageCode);
            paymentCollection.Add("amount", orderAmount);
            paymentCollection.Add("oid", orderNumber);
            //Kredi kart bilgileri
            paymentCollection.Add("pan", cardModel.CardNumber);
            paymentCollection.Add("cardHolderName", cardModel.HolderName);
            paymentCollection.Add("cv2", cardModel.CV2);
            paymentCollection.Add("Ecom_Payment_Card_ExpDate_Year", cardModel.ExpireYear);
            paymentCollection.Add("Ecom_Payment_Card_ExpDate_Month", cardModel.ExpireMonth);
            paymentCollection.Add("taksit", installment);
            paymentCollection.Add("cartType", cardType);


            object paymentForm = ThreeDHelper.PrepareForm("https://entegrasyon.asseco-see.com.tr/fim/est3Dgate", paymentCollection);

            return View(paymentForm);
        }

        public ActionResult Success()
        {
            return View();
        }

        public ActionResult UnSuccess()
        {
            return View();
        }
    }
}