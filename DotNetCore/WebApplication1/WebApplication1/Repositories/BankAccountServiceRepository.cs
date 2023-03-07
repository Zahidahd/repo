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
                string sqlQuery = "";
                SqlCommand sqlCommand = new(sqlQuery, sqlConnection);
                sqlCommand.Parameters.AddWithValue("@BankName", bankAccount.BankName);
                sqlCommand.Parameters.AddWithValue("@BranchName", bankAccount.BranchName);
                sqlCommand.Parameters.AddWithValue("@IfscCode", bankAccount.IfscCode);
                sqlCommand.Parameters.AddWithValue("@AccountNumber", bankAccount.AccountNumber);
                sqlCommand.Parameters.AddWithValue("@AccountType", bankAccount.AccountType);
                sqlCommand.Parameters.AddWithValue("@AccountHolder1Name", bankAccount.AccountHolder1Name);
                sqlCommand.Parameters.AddWithValue("@Holder1Email", bankAccount.Holder1Email);
                sqlCommand.Parameters.AddWithValue("@Holder1Address", bankAccount.Holder1Address);

                if (bankAccount.AccountType == AccountTypes.SavingsAccount)
                {
                    sqlQuery = @"INSERT INTO BankAccounts(BankName, BranchName, IfscCode, AccountNumber, 
							AccountType, AccountHolder1Name, Holder1Email, Holder1Address)

							VALUES (@BankName, @BranchName, @IfscCode, @AccountNumber, @AccountType, @AccountHolder1Name,
							@Holder1Email, @Holder1Address)
							Select Scope_Identity()";
                }
                else if (bankAccount.AccountType == AccountTypes.CurrentAccount)
                {
                    sqlQuery = @"INSERT INTO BankAccounts(BankName, BranchName, IfscCode, AccountNumber, 
						  AccountType, AccountHolder1Name, Holder1Email, Holder1Address, CompanyName, GSTNo)

						  VALUES (@BankName, @BranchName, @IfscCode, @AccountNumber, @AccountType, @AccountHolder1Name,
						  @Holder1Email, @Holder1Address, @CompanyName, @GSTNo)
						  Select Scope_Identity()";

                    sqlCommand.Parameters.AddWithValue("@CompanyName", bankAccount.CompanyName);
                    sqlCommand.Parameters.AddWithValue("@GSTNo", bankAccount.GSTNo);
                }
                else if (bankAccount.AccountType == AccountTypes.JointAccount)
                {
                    sqlQuery = @"INSERT INTO BankAccounts(BankName, BranchName, IfscCode, AccountNumber, 
						   AccountType, AccountHolder1Name, AccountHolder2Name, Holder1Email, Holder2Email,
						   Holder1Address, Holder2Address)

						   VALUES (@BankName, @BranchName, @IfscCode, @AccountNumber, @AccountType,
						   @AccountHolder1Name, @AccountHolder2Name, @Holder1Email, @Holder2Email, @Holder1Address,
						   @Holder2Address)
						   Select Scope_Identity()";

                    sqlCommand.Parameters.AddWithValue("@AccountHolder2Name", bankAccount.AccountHolder2Name);
                    sqlCommand.Parameters.AddWithValue("@Holder2Email", bankAccount.Holder2Email);
                    sqlCommand.Parameters.AddWithValue("@Holder2Address", bankAccount.Holder2Address);
                }

                sqlCommand.CommandText = sqlQuery;
                sqlConnection.Open();
                bankAccount.Id = Convert.ToInt32(sqlCommand.ExecuteScalar());
                sqlConnection.Close();
                return bankAccount.Id;
            }
        }
    }
}
