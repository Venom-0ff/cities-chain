using System;
using System.ServiceModel;
using CitiesChainLibrary;

namespace CitiesChainHostService
{
    /// <summary>
    /// A host application for the CitiesChain WCF service.
    /// </summary>
    internal class Program
    {
        static void Main()
        {
            ServiceHost serviceHost = null;
            try
            {
                serviceHost = new ServiceHost(typeof(CitiesChain));

                serviceHost.Open();
                Console.WriteLine("Service Host is running! Press any key to shut down.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Console.ReadKey();
                serviceHost?.Close();
            }
        }
    }
}
