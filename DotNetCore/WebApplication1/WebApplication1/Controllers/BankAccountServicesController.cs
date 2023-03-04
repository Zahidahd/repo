using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using WebApplication1.DataModel;
using WebApplication1.DTO.InputDTO;
using WebApplication1.DTO.InputDTO.BankAccountServicesDTO;
using WebApplication1.Repositories;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankAccountServicesController : ControllerBase
    {
        IBankAccountServiceRepository _bankAccountServiceRepository;    
            
        public BankAccountServicesController(IBankAccountServiceRepository bankAccountServiceRepository)
        {
            _bankAccountServiceRepository = bankAccountServiceRepository;
        }

        [HttpPost]
        [Route("CreateSavingsAccount")]
        public IActionResult CreateSavingsAccount([FromBody] SavingsAccountDto savingsAccount)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var bankAccount = new BankAccount
                    {
                        BankName = savingsAccount.BankName,
                        BranchName = savingsAccount.BranchName,
                        IfscCode = savingsAccount.IfscCode,
                        AccountNumber = savingsAccount.AccountNumber,
                        AccountType = Enums.AccountTypes.SavingsAccount,
                        AccountHolder1Name = savingsAccount.AccountHolder1Name,
                        Holder1Email = savingsAccount.Holder1Email,
                        Holder1Address = savingsAccount.Holder1Address,
                    };

                    int savingsAccountId = _bankAccountServiceRepository.Add(bankAccount);
                    return Ok(savingsAccountId);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", @"Unable to save changes. 
                    Try again, and if the problem persists 
                    see your system administrator.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        //[HttpPost]
        //[Route("CreateCurrentAccount")]
        //public IActionResult CreateCurrentAccount([FromBody] CurrentAccountDto currentAccount)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            var bankAccount = new BankAccount
        //            {
        //                BankName = currentAccount.BankName,
        //                BranchName = currentAccount.BranchName,
        //                IfscCode = currentAccount.IfscCode,
        //                AccountNumber = currentAccount.AccountNumber,
        //                AccountType = Enums.AccountTypes.CurrentAccount,
        //                AccountHolder1Name = currentAccount.AccountHolder1Name,
        //                Holder1Email = currentAccount.Holder1Email,
        //                Holder1Address = currentAccount.Holder1Address,
        //                CompanyName = currentAccount.CompanyName,
        //                GSTNo = currentAccount.GSTNo,
        //            };

        //            //Approach 2 - AutoMapper 
        //            //BankAccount bankAccount = _mapper.Map<CurrentAccountDto, BankAccount>(currentAccount);

        //            int currentAccountId = _bankAccountServiceRepository.Add(bankAccount);
        //            return Ok(currentAccountId);
        //        }
        //        return BadRequest();
        //    }
        //    catch (Exception ex)
        //    {
        //        ModelState.AddModelError("", @"Unable to save changes. 
        //            Try again, and if the problem persists 
        //            see your system administrator.");
        //        return StatusCode(StatusCodes.Status500InternalServerError);
        //    }
        //}

        //[HttpPost]
        //[Route("CreateJointAccount")]
        //public IActionResult CreateJointAccount([FromBody] JointAccountDto jointAccount)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            int jointAccountId = _bankAccountServiceRepository.CreateJointAccount(jointAccount);
        //            return Ok(jointAccountId);
        //        }
        //        return BadRequest();
        //    }
        //    catch (Exception ex)
        //    {
        //        ModelState.AddModelError("", @"Unable to save changes. 
        //            Try again, and if the problem persists 
        //            see your system administrator.");
        //        return StatusCode(StatusCodes.Status500InternalServerError);
        //    }
        //}
    }
}
