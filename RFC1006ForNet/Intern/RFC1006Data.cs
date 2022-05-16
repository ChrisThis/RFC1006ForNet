//*************************************************************
//*************************************************************
// Autor: Egger Christopher 
// Web: homeTwoPointZero.dedyn.io
// Datum: 06.05.2022                                 
// Version: 1.0                                              
//                                                          
// Description:   
// Buffer incomming data and holds the user data.
// 
// Changelog:
//                                                          
//*************************************************************
//*************************************************************


namespace RFC1006ForNet.Intern
{
    /// <summary>
    /// RFC1006 data
    /// </summary>
    public class RFC1006Data
    {
        #region Getter / Setter

        /// <summary>
        /// Local TSAP lenght
        /// </summary>
        public ushort LenLocalTsap { get; set; }

        /// <summary>
        /// Local TSAP as string
        /// </summary>
        public string LocalTSAP { get; set; }

        /// <summary>
        /// Remote TSAP lenght
        /// </summary>
        public ushort LenRemoteTsap { get; set; }

        /// <summary>
        /// Remot TSAP as string
        /// </summary>
        public string RemoteTSAP { get; set; }

        /// <summary>
        /// Length of user data
        /// </summary>
        public int CompleteLenOfUserData { get; set; }

        /// <summary>
        /// Lenght of local buffer
        /// </summary>
        public int LenOfBuffer { get; set; }

        /// <summary>
        /// Lenght of TPDU
        /// </summary>
        public int LenOfTPDU { get; set; }

        /// <summary>
        /// TPDU counted
        /// </summary>
        public int TPDUCount { get; set; }

        /// <summary>
        /// Index of TPDU
        /// </summary>
        public int IdxTPDU { get; set; }

        /// <summary>
        /// Lenght of user header
        /// </summary>
        public int LenOfUserHeader { get; set; }

        /// <summary>
        /// Lenght of user data
        /// </summary>
        public int LenOfUserData { get; set; }

        /// <summary>
        /// Index of user data
        /// </summary>
        public int IdxUserData { get; set; }

        /// <summary>
        /// Register end of one TPDU
        /// </summary>
        public bool EndOfTPDU { get; set; }

        #endregion

        #region Buffer

        /// <summary>
        /// Recv byte buffer
        /// </summary>
        public byte[] Buffer = new byte[RFC1006Constants.RFC1006BufferSize];

        /// <summary>
        /// User data buffer
        /// </summary>
        public byte[] UserData = new byte[RFC1006Constants.RFC1006UserDataBufferSize];

        #endregion

        #region C'tor
        /// <summary>
        /// C'tor
        /// </summary>
        public RFC1006Data()
        {
            LenLocalTsap = 0x00;
            LocalTSAP = "";
            LenRemoteTsap = 0x00;
            RemoteTSAP = "";
            CompleteLenOfUserData = 0;
            LenOfBuffer = 0;
            LenOfTPDU = 0;
            TPDUCount = 0;
            IdxTPDU = 0;
            LenOfUserHeader = 0;
            LenOfUserData = 0;
            IdxUserData = 0;
            EndOfTPDU = false;
        }

        #endregion
    }
}
