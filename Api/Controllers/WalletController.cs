using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TouRest.Api.Common;
using TouRest.Api.Extensions;
using TouRest.Application.DTOs.Wallet;
using TouRest.Application.Interfaces;
using TouRest.Domain.Entities;

[Route("api/wallet")]
[ApiController]
[Authorize]
public class WalletController : ControllerBase
{
    private readonly IWalletService _walletService;
    private readonly IWalletTransactionService _walletTransactionService;
    private readonly IWalletTopUpService _walletTopUpService;

    public WalletController(
        IWalletService walletService,
        IWalletTransactionService walletTransactionService,
        IWalletTopUpService walletTopUpService)
    {
        _walletService = walletService;
        _walletTransactionService = walletTransactionService;
        _walletTopUpService = walletTopUpService;
    }

    [HttpGet]
    public async Task<IActionResult> GetWallet()
    {
        var userId = User.GetUserId();
        var result = await _walletService.GetWalletAsync(userId);
        return ApiResponseFactory.Ok(result);
    }

    [HttpGet("transactions")]
    public async Task<IActionResult> GetTransactions()
    {
        var userId = User.GetUserId();
        var result = await _walletTransactionService.GetTransactionsAsync(userId);
        return ApiResponseFactory.Ok(result);
    }

    [HttpGet("banks")]
    public async Task<IActionResult> GetSavedBanks()
    {
        var userId = User.GetUserId();
        var result = await _walletService.GetSavedBanksAsync(userId);
        return ApiResponseFactory.Ok(result);
    }

    [HttpPost("payout")]
    public async Task<IActionResult> RequestPayout([FromBody] PayoutRequestDTO request)
    {
        var userId = User.GetUserId();
        await _walletService.RequestPayoutAsync(userId, request);
        return ApiResponseFactory.Created(new { }, "Payout request submitted");
    }

    /// <summary>Create a PayOS top-up link for the current user's wallet.</summary>
    [HttpPost("topup")]
    public async Task<IActionResult> CreateTopUp([FromBody] WalletTopUpRequest request)
    {
        var userId = User.GetUserId();
        var result = await _walletTopUpService.CreateTopUpAsync(userId, request.Amount);
        return ApiResponseFactory.Created(result, "Top-up link created");
    }

    /// <summary>Poll top-up status (frontend calls every few seconds while QR is displayed).</summary>
    [HttpGet("topup/{orderCode:long}/status")]
    public async Task<IActionResult> GetTopUpStatus(long orderCode)
    {
        var result = await _walletTopUpService.GetStatusAsync(orderCode);
        if (result == null) return NotFound(new { message = "Top-up not found" });
        return ApiResponseFactory.Ok(result);
    }

    /// <summary>Called from success page to verify with PayOS and credit wallet.</summary>
    [HttpPost("topup/finalize/{orderCode:long}")]
    public async Task<IActionResult> FinalizeTopUp(long orderCode)
    {
        var userId = User.GetUserId();
        await _walletTopUpService.FinalizeTopUpByOrderCodeAsync(orderCode, userId);
        return ApiResponseFactory.Ok("Top-up finalized");
    }

    //[HttpPut("add-funds")]
    //public async Task<IActionResult> AddFunds([FromBody] AddFundsRequestDTO request)
    //{
    //    var userId = User.GetUserId();
    //    await _walletService.AddFundsAsync(userId, request);
    //    return ApiResponseFactory.Created(new { }, "Add funds request submitted");
    //}
    //[HttpPut("withdraw-funds")]
    //public async Task<IActionResult> WithdrawFunds([FromBody] WithdrawFundsRequestDTO request)
    //{
    //    var userId = User.GetUserId();
    //    await _walletService.WithdrawFundsAsync(userId, request);
    //    return ApiResponseFactory.Created(new { }, "Withdraw funds request submitted");
    //}
    //[HttpPut("add-bank")]
    //public async Task<IActionResult> AddBank([FromBody] AddBankRequestDTO request)
    //{
    //    var userId = User.GetUserId();
    //    await _walletService.AddBankAsync(userId, request);
    //    return ApiResponseFactory.Created(new { }, "Bank added successfully");
    //}
}