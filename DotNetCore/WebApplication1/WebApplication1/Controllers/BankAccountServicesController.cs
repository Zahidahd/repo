using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using WebApplication1.DataModel;
using WebApplication1.DTO.InputDTO;
using WebApplication1.DTO.InputDTO.BankAccountServicesDTO;
using WebApplication1.Repositories;
using AutoMapper;
using WebApplication1.Enums;

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
        public IActionResult CreateSavingsAccount([FromBody] SavingsAccountDto savingsAccountDto)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    //Approach1
                   // var bankAccount = new BankAccount
                    //{
                    //    BankName = savingsAccount.BankName,
                    //    BranchName = savingsAccount.BranchName,
                    //    IfscCode = savingsAccount.IfscCode,
                    //    AccountNumber = savingsAccount.AccountNumber,
                    //    AccountType = Enums.AccountTypes.SavingsAccount,
                    //    AccountHolder1Name = savingsAccount.AccountHolder1Name,
                    //    Holder1Email = savingsAccount.Holder1Email,
                    //    Holder1Address = savingsAccount.Holder1Address,
                    //};
                    
                    // using AutoMapper
                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<SavingsAccountDto, BankAccount>();
                    });

                    IMapper iMapper = config.CreateMapper();
                    BankAccount bankAccount= iMapper.Map<SavingsAccountDto, BankAccount>(savingsAccountDto);
                    bankAccount.AccountType = AccountTypes.SavingsAccount;

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

        [HttpPost]
        [Route("CreateCurrentAccount")]
        public IActionResult CreateCurrentAccount([FromBody] CurrentAccountDto currentAccountDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //Approach1
                    //var bankAccount = new BankAccount
                    //{
                    //    BankName = currentAccountDto.BankName,
                    //    BranchName = currentAccountDto.BranchName,
                    //    IfscCode = currentAccountDto.IfscCode,
                    //    AccountNumber = currentAccountDto.AccountNumber,
                    //    AccountType = Enums.AccountTypes.CurrentAccount,
                    //    AccountHolder1Name = currentAccountDto.AccountHolder1Name,
                    //    Holder1Email = currentAccountDto.Holder1Email,
                    //    Holder1Address = currentAccountDto.Holder1Address,
                    //    CompanyName = currentAccountDto.CompanyName,
                    //    GSTNo = currentAccountDto.GSTNo,
                    //};

                    //Approach2 Using AutoMapper
                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<CurrentAccountDto, BankAccount>();
                    });

                    IMapper iMapper = config.CreateMapper();
                    BankAccount bankAccount = iMapper.Map<CurrentAccountDto, BankAccount>(currentAccountDto);
                    bankAccount.AccountType = AccountTypes.CurrentAccount;

                    int currentAccountId = _bankAccountServiceRepository.Add(bankAccount);
                    return Ok(currentAccountId);
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

        [HttpPost]
        [Route("CreateJointAccount")]
        public IActionResult CreateJointAccount([FromBody] JointAccountDto jointAccountDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //Approach1
                    //var bankAccount = new BankAccount
                    //{
                    //    BankName = jointAccountDto.BankName,
                    //    BranchName = jointAccountDto.BranchName,
                    //    IfscCode = jointAccountDto.IfscCode,
                    //    AccountNumber = jointAccountDto.AccountNumber,
                    //    AccountType = Enums.AccountTypes.CurrentAccount,
                    //    AccountHolder1Name = jointAccountDto.AccountHolder1Name,
                    //    AccountHolder2Name = jointAccountDto.AccountHolder2Name,
                    //    Holder1Email = jointAccountDto.Holder1Email,
                    //    Holder2Email = jointAccountDto.Holder2Email,
                    //    Holder1Address = jointAccountDto.Holder1Address,
                    //    Holder2Address = jointAccountDto.Holder2Address,
                    //};

                    //Approach2 Using AutoMapper
                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<JointAccountDto, BankAccount>();
                    });

                    IMapper iMapper = config.CreateMapper();
                    BankAccount bankAccount = iMapper.Map<JointAccountDto, BankAccount>(jointAccountDto);
                    bankAccount.AccountType = AccountTypes.JointAccount;

                    int jointAccountId = _bankAccountServiceRepository.Add(bankAccount);
                    return Ok(jointAccountId);
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
    }
}
