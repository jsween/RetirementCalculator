using System;

namespace RetirementCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            byte numYears = 7;
            double avgWeeklyReturn = 1.01;
            double startingBalance = 10_500;
            byte debug = 1;

            double bitcoinVal = 32_700;
            double dogecoinVal = 0.01;

            int[] biWeeklyContributions = new int[] { 0, 200, 300, 400, 500, 600, 800 };

            for (int i = 0; i < 52 * numYears; i++)
            {
                bitcoinVal *= avgWeeklyReturn;
                dogecoinVal *= avgWeeklyReturn;
            }

            if (debug > 0) Console.WriteLine($"Bitcoin Value in {numYears + DateTime.Now.Year} {bitcoinVal:c}\nDogecoin Value {dogecoinVal:c}\n");

            foreach (int contribution in biWeeklyContributions)
            {
                CryptoAccount cryptoAccount = new CryptoAccount($"Crypto{contribution}", startingBalance, avgWeeklyReturn, contribution, debug);
                cryptoAccount.EstimateValueIn(numYears);
            }
        }
    }
}

public class CryptoAccount
{
    public static double TaxRate { get; set; } = 0.15;
    public string Name { get; set; }
    public double Balance { get; set; }
    public double WeeklyReturn { get; set; }
    public double BiWeeklyContributions { get; set; }
    public byte Debug { get; set; }


    public CryptoAccount(string name, double balance, double weeklyReturn, double biWeeklyContributions = 0, byte debug = 0)
    {
        Name = name;
        Balance = balance;
        WeeklyReturn = weeklyReturn;
        BiWeeklyContributions = biWeeklyContributions;
        Debug = debug;
    }

    public void EstimateValueIn(byte years)
    {
        if (years < 1) throw new Exception("Error: Years need to be positive.");

        double initBalance = Balance;

        // loop through years
        for (int i = 0; i < years; i++)
        {
            double startBalance = Balance, currentBalanceYear = Balance;
            if (Debug > 1) Console.WriteLine($"Start Balance: {startBalance:c}");

            // loop through weeks
            for (int j = 0; j < 52; j++)
            {
                if (j % 2 == 0)
                {
                    currentBalanceYear += BiWeeklyContributions;
                }
                currentBalanceYear *= WeeklyReturn;
            }//: End of weekly loop
            
            if (Debug > 1) Console.WriteLine($"CurrentBalance: {currentBalanceYear:c} startBalance: {startBalance:c}");
            double yearlyProfit = CalcTaxes(startBalance, currentBalanceYear);
            if (Debug > 1) Console.WriteLine($"Yearly Profit before taxes: {currentBalanceYear - startBalance:c}  After {TaxRate:p} taxes: {yearlyProfit:c}");
            // Add profit to Balance
            Balance += yearlyProfit;
            if (Debug > 0) { Console.WriteLine($"{DateTime.Now.Year + i + 1}: {Balance:c}"); }
        }//: End of year loop

        Console.WriteLine($"**********\nBalance in {years} years: {Balance:c}");
        double totalInvested = BiWeeklyContributions * 26 * years + initBalance;
        Console.WriteLine($"Total Invested with {BiWeeklyContributions:c} contributions: {totalInvested:c}    {Balance/totalInvested:p} Gained\n**********\n");
    }

    private double CalcTaxes(double beginBalance, double endBalance)
    {
        double profit = endBalance - beginBalance;
        if (profit < 479_00) TaxRate = 0.15;
        else TaxRate = 0.2; 
        return profit * (1 - TaxRate);
    }
}