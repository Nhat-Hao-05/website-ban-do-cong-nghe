using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyEStore.Entities;
using MyEStore.Models.ViewModels;
using System.Globalization;

namespace MyEStore.Controllers
{
    public class PaymentController : Controller
    {
        private readonly PaypalClient _paypalClient;
        private readonly MyeStoreContext _db;

        public PaymentController(PaypalClient paypalClient, MyeStoreContext db)
        {
            _paypalClient = paypalClient;
            _db = db;
        }

        [HttpPost("/payment/create-paypal-order")]
        public async Task<IActionResult> CreateOrder([FromBody] Amount amount)
        {
            if (!decimal.TryParse(amount.value, NumberStyles.Number,
                                  CultureInfo.InvariantCulture, out var valueDecimal)
                || valueDecimal <= 0)
            {
                return BadRequest(new { message = "Số tiền không hợp lệ" });
            }

            var currency_code = string.IsNullOrWhiteSpace(amount.currency_code) ? "USD" : amount.currency_code;

            var hoaDon = new HoaDon
            {
                MaKh = "VINET",
                NgayDat = DateTime.Now,
                HoTen = "Thầy Giáo Ba",
                MaNv = "lvc",
                DiaChi = "59 rue de l'Abbaye",
                CachThanhToan = "Cash",
                CachVanChuyen = "Airline",
                PhiVanChuyen = 5.0,
                MaTrangThai = 0,
                GhiChu = "Thanh toán qua PayPal"
            };

            _db.HoaDons.Add(hoaDon);
            await _db.SaveChangesAsync();

            try
            {
                var valueString = valueDecimal.ToString("F2", CultureInfo.InvariantCulture);
                var paypalOrderId = await _paypalClient.CreateOrder(valueString, currency_code, hoaDon.MaHd.ToString());

                if (string.IsNullOrWhiteSpace(paypalOrderId))
                    return StatusCode(502, new { message = "Không lấy được Paypal Order ID" });

                hoaDon.PaypalOrderID = paypalOrderId;
                _db.HoaDons.Update(hoaDon);
                await _db.SaveChangesAsync();

                return Ok(new { id = paypalOrderId });
            }
            catch (Exception ex)
            {
                hoaDon.MaTrangThai = -1;
                _db.HoaDons.Update(hoaDon);
                await _db.SaveChangesAsync();

                return StatusCode(500, new { message = ex.GetBaseException().Message });
            }
        }

        [HttpPost("/payment/capture-paypal-order")]
        public async Task<IActionResult> CaptureOrder(string orderID)
        {
            if (string.IsNullOrWhiteSpace(orderID))
                return BadRequest(new { status = "fail", message = "Thiếu orderID" });

            try
            {
                var response = await _paypalClient.CaptureOrder(orderID);

                var hoaDon = await _db.HoaDons.FirstOrDefaultAsync(h => h.PaypalOrderID == orderID);
                if (hoaDon == null)
                    return BadRequest(new { status = "fail", message = "Không tìm thấy hóa đơn" });

                hoaDon.MaTrangThai = 1;
                hoaDon.NgayGiao = DateTime.Now;

                _db.HoaDons.Update(hoaDon);
                await _db.SaveChangesAsync();

                var transactionId = response.purchase_units[0].payments.captures[0].id;
                var amount = response.purchase_units[0].payments.captures[0].amount.value;
                var currency = response.purchase_units[0].payments.captures[0].amount.currency_code;

                return Ok(new
                {
                    status = "success",
                    orderId = hoaDon.MaHd,
                    transactionId,
                    amount,
                    currency
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "fail", message = ex.GetBaseException().Message });
            }
        }

        public IActionResult PaypalSuccess(string orderId, string transactionId, decimal amount, string currency)
        {
            var model = new PaypalTransactionViewModel
            {
                OrderId = orderId,
                TransactionId = transactionId,
                Status = "Thành công (PayPal)",
                Amount = amount,
                Currency = currency
            };
            return View("Success", model);
        }




        public IActionResult Fail(string orderId)
        {
            
            var model = new PaypalTransactionViewModel
            {
                OrderId = orderId
            };
            return View(model);
        }
    }
}
