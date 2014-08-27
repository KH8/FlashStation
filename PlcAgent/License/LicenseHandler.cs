using System;
using System.IO;
using System.Windows;
using PlcAgentLicenceGenerator.BlowFish;
using PlcAgentLicenceGenerator.Signature;
using _PlcAgent.Log;
using _PlcAgent.Signature;

namespace _PlcAgent.License
{
    public static class LicenseHandler
    {
        #region Variables

        private static string _signature;

        private static BlowFish _blowFishEncryptor;

        #endregion


        #region Properties

        public static string LicenceOwnerName = "Null";

        #endregion


        #region Methods

        public static Boolean CheckLicense()
        {
            _signature = SignatureHandler.GetSignature();
            _blowFishEncryptor = new BlowFish(HexKey.Value);

            const string fileName = @"License\licence.lic";
            string storedSignature;

            try
            {
                // Create the reader for data.
                var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                var r = new BinaryReader(fs);

                // Read data from Test.data.
                LicenceOwnerName = r.ReadString();
                storedSignature = r.ReadString();

                r.Close();
                fs.Close();
            }
            catch (Exception)
            {
                Logger.Log("Licence file does not exist");
                MessageBox.Show("Licence file does not exist", "License ");
                return false;
            }

            if (Equals(_signature, _blowFishEncryptor.Decrypt_CTR(storedSignature)))
            {
                Logger.Log("Licence verified. Licence owner: " + LicenceOwnerName);
                return true;
            }
            Logger.Log("Licence is not valid");
            MessageBox.Show("Licence is not valid", "License ");
            return false;
        }

        #endregion

    }
}
