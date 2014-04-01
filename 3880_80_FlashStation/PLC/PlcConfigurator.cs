namespace _3880_80_FlashStation.PLC
{
    public class PlcConfigurator : PlcCommunicatorBase
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

        #region Methods

        public void UpdateConfiguration(PlcConfig configuration)
        {
            //todo: config verification must be done
            configuration.PlcConfigurationStatus = 1;
            _plcConfiguration = configuration;
        }

        #endregion


    }
}
