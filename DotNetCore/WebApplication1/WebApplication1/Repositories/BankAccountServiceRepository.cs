using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics.Metrics;
using System.Reflection;
using System.Text;
using System.Security.Cryptography;
using WebApplication1.DTO.InputDTO;
using WebApplication1.Enums;
using WebApplication1.Helpers;
using WebApplication1.DTO.InputDTO.BankAccountServicesDTO;
using WebApplication1.DataModel;

namespace WebApplication1.Repositories
{
    public class BankAccountServiceRepository : IBankAccountServiceRepository
    {
        private readonly string _connectionString;

        public BankAccountServiceRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int Add(BankAccount bankAccount)
        {
            using (SqlConnection sqlConnection = new(_connectionString))
            {
                {
                    string sqlQuery = @"INSERT INTO BankAccounts(BankName, BranchName, IfscCode, AccountNumber, 
                            AccountType, AccountHolder1Name, Holder1Email, Holder1Address, AccountBalance)

                            VALUES (@BankName, @BranchName, @IfscCode, @AccountNumber, @AccountType, @AccountHolder1Name,
                            @Holder1Email, @Holder1Address, @AccountBalance)
                            Select Scope_Identity()";
                    SqlCommand sqlCommand = new(sqlQuery, sqlConnection);
                    sqlCommand.Parameters.AddWithValue("@BankName", bankAccount.BankName);
                    sqlCommand.Parameters.AddWithValue("@BranchName", bankAccount.BranchName);
                    sqlCommand.Parameters.AddWithValue("@IfscCode", bankAccount.IfscCode);
                    sqlCommand.Parameters.AddWithValue("@AccountNumber", bankAccount.AccountNumber);
                    sqlCommand.Parameters.AddWithValue("@AccountType", bankAccount.AccountType);
                    sqlCommand.Parameters.AddWithValue("@AccountHolder1Name", bankAccount.AccountHolder1Name);
                    sqlCommand.Parameters.AddWithValue("@Holder1Email", bankAccount.Holder1Email);
                    sqlCommand.Parameters.AddWithValue("@Holder1Address", bankAccount.Holder1Address);
                    sqlCommand.Parameters.AddWithValue("@AccountBalance", bankAccount.AccountBalance);
                    sqlConnection.Open();
                    bankAccount.Id = Convert.ToInt32(sqlCommand.ExecuteScalar());
                    sqlConnection.Close();
                    return bankAccount.Id;
                }
            }
        }

        //public int CreateSavingsAccount(SavingsAccountDto savingsAccount)    
        //{   
        //    using (SqlConnection sqlConnection = new(_connectionString))
        //    {
        //        {
        //            string sqlQuery = @"INSERT INTO BankAccounts(BankName, BranchName, IfscCode, AccountNumber, 
        //                    AccountType, AccountHolder1Name, Holder1Email, Holder1Address, AccountBalance)

        //                    VALUES (@BankName, @BranchName, @IfscCode, @AccountNumber, @AccountType, @AccountHolder1Name,
        //                    @Holder1Email, @Holder1Address, @AccountBalance)
        //                    Select Scope_Identity()";
        //            SqlCommand sqlCommand = new(sqlQuery, sqlConnection);
        //            sqlCommand.Parameters.AddWithValue("@BankName", savingsAccount.BankName);
        //            sqlCommand.Parameters.AddWithValue("@BranchName", savingsAccount.BranchName);
        //            sqlCommand.Parameters.AddWithValue("@IfscCode", savingsAccount.IfscCode);
        //            sqlCommand.Parameters.AddWithValue("@AccountNumber", savingsAccount.AccountNumber);
        //            sqlCommand.Parameters.AddWithValue("@AccountType", savingsAccount.AccountType);
        //            sqlCommand.Parameters.AddWithValue("@AccountHolder1Name", savingsAccount.AccountHolder1Name);
        //            sqlCommand.Parameters.AddWithValue("@Holder1Email", savingsAccount.Holder1Email);
        //            sqlCommand.Parameters.AddWithValue("@Holder1Address", savingsAccount.Holder1Address);
        //            sqlCommand.Parameters.AddWithValue("@AccountBalance", savingsAccount.AccountBalance);
        //            sqlConnection.Open();   
        //            savingsAccount.Id = Convert.ToInt32(sqlCommand.ExecuteScalar());
        //            sqlConnection.Close();
        //            return savingsAccount.Id;
        //        }
        //    }
        //}

        //public int CreateCurrentAccount(CurrentAccountDto currentAccount)
        //{
        //    using (SqlConnection sqlConnection = new(_connectionString))
        //    {
        //        {   
        //            string sqlQuery = @"INSERT INTO BankAccounts(BankName, BranchName, IfscCode, AccountNumber, 
        //                    AccountType, AccountHolder1Name, Holder1Email, Holder1Address, CompanyName, AccountBalance)

        //                    VALUES (@BankName, @BranchName, @IfscCode, @AccountNumber, @AccountType, @AccountHolder1Name,
        //                    @Holder1Email, @Holder1Address, @CompanyName, @AccountBalance)
        //                    Select Scope_Identity()";
        //            SqlCommand sqlCommand = new(sqlQuery, sqlConnection);
        //            sqlCommand.Parameters.AddWithValue("@BankName", currentAccount.BankName);
        //            sqlCommand.Parameters.AddWithValue("@BranchName", currentAccount.BranchName);
        //            sqlCommand.Parameters.AddWithValue("@IfscCode", currentAccount.IfscCode);
        //            sqlCommand.Parameters.AddWithValue("@AccountNumber", currentAccount.AccountNumber);
        //            sqlCommand.Parameters.AddWithValue("@AccountType", currentAccount.AccountType);
        //            sqlCommand.Parameters.AddWithValue("@AccountHolder1Name", currentAccount.AccountHolder1Name);
        //            sqlCommand.Parameters.AddWithValue("@Holder1Email", currentAccount.Holder1Email);
        //            sqlCommand.Parameters.AddWithValue("@Holder1Address", currentAccount.Holder1Address);
        //            sqlCommand.Parameters.AddWithValue("@CompanyName", currentAccount.CompanyName);
        //            sqlCommand.Parameters.AddWithValue("@AccountBalance", currentAccount.AccountBalance);
        //            sqlConnection.Open();
        //            currentAccount.Id = Convert.ToInt32(sqlCommand.ExecuteScalar());
        //            sqlConnection.Close();
        //            return currentAccount.Id;
        //        }
        //    }
        //}

        //public int CreateJointAccount(JointAccountDto jointAccount)
        //{
        //    using (SqlConnection sqlConnection = new(_connectionString))
        //    {
        //        {
        //            string sqlQuery = @"INSERT INTO BankAccounts(BankName, BranchName, IfscCode, AccountNumber, 
        //                    AccountType, AccountHolder1Name, AccountHolder2Name, Holder1Email, Holder2Email,
        //                    Holder1Address, Holder2Address, AccountBalance)

        //                    VALUES (@BankName, @BranchName, @IfscCode, @AccountNumber, @AccountType,
        //                    @AccountHolder1Name, @AccountHolder2Name, @Holder1Email, @Holder2Email, @Holder1Address,
        //                    @Holder2Address, @AccountBalance)
        //                    Select Scope_Identity()";
        //            SqlCommand sqlCommand = new(sqlQuery, sqlConnection);
        //            sqlCommand.Parameters.AddWithValue("@BankName", jointAccount.BankName);
        //            sqlCommand.Parameters.AddWithValue("@BranchName", jointAccount.BranchName);
        //            sqlCommand.Parameters.AddWithValue("@IfscCode", jointAccount.IfscCode);
        //            sqlCommand.Parameters.AddWithValue("@AccountNumber", jointAccount.AccountNumber);
        //            sqlCommand.Parameters.AddWithValue("@AccountType", jointAccount.AccountType);
        //            sqlCommand.Parameters.AddWithValue("@AccountHolder1Name", jointAccount.AccountHolder1Name);
        //            sqlCommand.Parameters.AddWithValue("@AccountHolder2Name", jointAccount.AccountHolder2Name);
        //            sqlCommand.Parameters.AddWithValue("@Holder1Email", jointAccount.Holder1Email);
        //            sqlCommand.Parameters.AddWithValue("@Holder2Email", jointAccount.Holder2Email);
        //            sqlCommand.Parameters.AddWithValue("@Holder1Address", jointAccount.Holder1Address);
        //            sqlCommand.Parameters.AddWithValue("@Holder2Address", jointAccount.Holder2Address);
        //            sqlCommand.Parameters.AddWithValue("@AccountBalance", jointAccount.AccountBalance);
        //            sqlConnection.Open();
        //            jointAccount.Id = Convert.ToInt32(sqlCommand.ExecuteScalar());
        //            sqlConnection.Close();
        //            return jointAccount.Id;
        //        }
        //    }
        //}
    }
}
