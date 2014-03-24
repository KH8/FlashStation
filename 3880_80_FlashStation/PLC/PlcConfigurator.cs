namespace _3880_80_FlashStation.PLC
{
    class PlcConfigurator : PlcCommunicatorBase
    {
        #region Variables

        //Private
        //Status
        private PlcConfig _plcConfiguration;

        #endregion

        #region Properties

        //Public
        //Status
        public PlcConfig PlcConfiguration
        {
            get { return _plcConfiguration; }
            set { _plcConfiguration = value; }
        }

        #endregion

        #region Constructor

        public PlcConfigurator()
        {
            _plcConfiguration = new PlcConfig();
        }

        #endregion

        public void UpdateConfiguration(PlcConfig configuration)
        {
            //to do: be done verification
            configuration.PlcConfigurationStatus = 1;

            _plcConfiguration = configuration;
        }

    }
}
