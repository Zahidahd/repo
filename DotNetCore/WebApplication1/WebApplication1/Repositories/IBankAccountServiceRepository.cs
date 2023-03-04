﻿using WebApplication1.DataModel;
using WebApplication1.DTO.InputDTO;
using WebApplication1.DTO.InputDTO.BankAccountServicesDTO;

namespace WebApplication1.Repositories
{
    public interface IBankAccountServiceRepository
    {
        public int Add(BankAccount bankAccount);
        //public int CreateBankAccount(BankAccountServiceDto bankAccount);
        //public int CreateSavingsAccount(SavingsAccountDto savingsAccount);
        //public int CreateCurrentAccount(CurrentAccountDto currentAccount);
        //public int CreateJointAccount(JointAccountDto jointAccount);
    }
}
    