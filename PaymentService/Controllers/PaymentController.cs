using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using PaymentService.Models;
using PaymentService.Interfaces;

namespace PaymentService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Payment>>> GetAllPayments()
        {
            return Ok(await _paymentService.GetAllPayments());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Payment>> GetPayment(int id)
        {
            var payment = await _paymentService.GetPaymentById(id);
            if (payment == null)
                return NotFound();

            return Ok(payment);
        }

        [HttpPost]
        public async Task<ActionResult<Payment>> CreatePayment(PaymentDto paymentDto)
        {
            var payment = await _paymentService.ProcessPayment(paymentDto);
            return CreatedAtAction(nameof(GetPayment), new { id = payment.Id }, payment);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePayment(int id, PaymentDto paymentDto)
        {
            var result = await _paymentService.UpdatePayment(id, paymentDto);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            var result = await _paymentService.DeletePayment(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpPost("{id}/refund")]
        public async Task<IActionResult> RefundPayment(int id)
        {
            var result = await _paymentService.RefundPayment(id);
            if (!result)
                return BadRequest("Refund failed");

            return Ok();
        }

        [HttpGet("{id}/status")]
        public async Task<ActionResult<PaymentStatus>> CheckPaymentStatus(int id)
        {
            return Ok(await _paymentService.CheckPaymentStatus(id));
        }

        [HttpGet("transactions/{userId}")]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetUserTransactions(int userId)
        {
            return Ok(await _paymentService.ListTransactions(userId));
        }
    }
}