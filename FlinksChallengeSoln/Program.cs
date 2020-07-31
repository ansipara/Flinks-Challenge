using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlinksChallengeSoln
{
    class Program
    {
        static IWebDriver driver;
        const string URL = "https://challenge.flinks.io/";
        
        static List<string> tokens = new List<string>();

        static void Main(string[] args)
        {
            SetUp();
        }


        private static void SetUp()
        {
            driver = new ChromeDriver();
            driver.Navigate().GoToUrl("https://challenge.flinks.io/");
            RandomMouseMove(driver);
            var challengeElement = GetElementByXpath("//p[contains(text(),'ChallengeID')]");
            var challengeID = challengeElement.Text;
            tokens.Add(challengeID);
            GetElementByXpath("//a[text()='START']").Click();
            try
            {
                GetTokens();
                driver.Close();
            }
            finally
            {
                string projectPath = System.IO.Path.GetDirectoryName(System.IO.Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()));
                string path = projectPath + "/tokens.txt";

                using (StreamWriter outputFile = new StreamWriter(path, false))
                {
                    foreach (string token in tokens)
                        outputFile.WriteLine(token);
                }
            }
        }

        private static void RandomMouseMove(IWebDriver driver)
        {
            Random r = new Random();
            int xOffset = r.Next(500);
            int yOffset = r.Next(500);

            try
            {
                Actions actionProvider = new Actions(driver);
                // Performs mouse move action onto the offset position
                actionProvider.MoveByOffset(xOffset, yOffset).Build().Perform();
            }catch(Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }

        private static void GetTokens()
        {
            try
            {
                while(tokens.Count < 50)
                {
                    var timeout =new Random().Next(500,5000);
                    //System.Threading.Thread.Sleep(timeout);

                    FillPassWordAndLogIn();
                    var tokenElement = GetElementByXpath("//p[contains(text(),'Authentification token')]");
                    var token = tokenElement.Text.Replace("Authentification token:", "");
                    Console.WriteLine("Token : " + token);
                    tokens.Add(token);
                    RandomMouseMove(driver);
                    timeout = new Random().Next(2000,5000);
                    //System.Threading.Thread.Sleep(timeout);
                    GetElementByXpath("//a[text()='BACK']").Click();
                }
            }
            catch(Exception e)
            {
                //Console.WriteLine(e.StackTrace);
                if(tokens.Count < 50)
                {
                    GetTokens();
                }
                
            }
        }

        private static void FillPassWordAndLogIn()
        {
            RandomType(driver.FindElement(By.CssSelector("input[name='username'],input[name='username2']")), Config.username);
            RandomType(driver.FindElement(By.CssSelector("input[name='password'],input[name='password2']")), Config.password);

            GetElementByXpath("//button[contains(text(),'Sign In')]	 ").Click();
        }

        private static void RandomType(IWebElement webElement, String str)
        {
            foreach (char c in str)
            {
                System.Threading.Thread.Sleep(new Random().Next(1500));
                webElement.SendKeys(c.ToString());
            }
        }

        private static bool IsElementPresent(By by)
        {
            try
            {
                driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        private static IWebElement GetElementByXpath(string str)
        {
            return driver.FindElement(By.XPath(str));
        }
    }
}
