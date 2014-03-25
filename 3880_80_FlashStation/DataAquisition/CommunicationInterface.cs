namespace _3880_80_FlashStation.DataAquisition
{
    abstract class CommunicationInterface
    {
        public struct ReadData
        {
            
        }

        public struct WriteData
        {

        }
    }

    abstract class CommunicationInterfaceComponent
    {
        private int _id;
        private int _pos;
        private string _type;

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public int Pos
        {
            get { return _pos; }
            set { _pos = value; }
        }

        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public CommunicationInterfaceComponent()
        {
            _id = 0;
            _pos = 0;
            _type = "Undefined";
        }
    }
}
