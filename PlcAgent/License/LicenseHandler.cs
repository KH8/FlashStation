using System;
using System.IO;
using System.Windows;
using _PlcAgent.Log;
using _PlcAgent.Signature;

namespace _PlcAgent.License
{
    public static class LicenseHandler
    {
        #region Variables

        private static string _signature;
        private static string _personalSignatureStored;

        private static BlowFish _blowFishEncryptor;

        #endregion


        #region Properties

        public static string LicenseOwnerName = "Null";
        public static string SignatureStored = "Null";
        public static string LicenseCreationTime = "Null";

        #endregion


        #region Methods

        public static Boolean CheckLicense()
        {
            _signature = SignatureHandler.GetSignature();

            const string fileName = @"License\license.lic";

            try
            {
                // Create the reader for data.
                var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                var r = new BinaryReader(fs);

                // Read data from Test.data.

                SignatureStored = r.ReadString();
                LicenseOwnerName = r.ReadString();
                LicenseCreationTime = r.ReadString();
                _personalSignatureStored = r.ReadString();

                r.Close();
                fs.Close();
            }
            catch (Exception)
            {
                Logger.Log("License file does not exist");
                MessageBox.Show("License file does not exist", "License ");
                return false;
            }

            _blowFishEncryptor = new BlowFish(HexKey.PersonalSignatureValue);
            _personalSignatureStored = _blowFishEncryptor.Decrypt_CTR(_personalSignatureStored);

            _blowFishEncryptor = new BlowFish(HexKey.SignatureValue);
            if (Equals(LicenseOwnerName + LicenseCreationTime, _blowFishEncryptor.Decrypt_CTR(_personalSignatureStored)))
            {
                if (Equals(_signature, _blowFishEncryptor.Decrypt_CTR(SignatureStored)))
                {
                    Logger.Log("License verified. License owner: " + LicenseOwnerName + ", date: " + LicenseCreationTime);
                    return true;
                }
            }
            Logger.Log("License is not valid");
            MessageBox.Show("License is not valid", "License ");
            return false;
        }

        #endregion

    }
}
