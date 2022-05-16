//*************************************************************
//*************************************************************
// Autor: Egger Christopher 
// Web: homeTwoPointZero.dedyn.io
// Datum: 06.05.2022                                 
// Version: 1.0                                              
//                                                          
// Description:                                                         
// Constants for RFC1006 communication
//
// Changelog:
//                                                          
//*************************************************************
//*************************************************************


namespace RFC1006ForNet.Intern
{
    /// <summary>
    /// RFC1006 constants
    /// </summary>
    public class RFC1006Constants
    {
        /// <summary>
        /// Max buffer size recv by socket
        /// </summary>
        public const int RFC1006BufferSize = 1048560;

        /// <summary>
        /// Max buffer size for recv user data
        /// </summary>
        public const int RFC1006UserDataBufferSize = 65535;

        /// <summary>
        /// RFC905 TPDU conn request
        /// </summary>
        public const byte RFC905_TPDU_ConnRequest = 0xE0;

        /// <summary>
        /// RFC905 TPDU conn confirm
        /// </summary>
        public const byte RFC905_TPDU_ConnConfirm = 0xD0;

        /// <summary>
        /// Max size (bytes) of one TPDU
        /// </summary>
        public const int RFC905_TPDU_MaxSize = 1028;

        /// <summary>
        /// RFC905 TPDU end of telegram
        /// </summary>
        public const byte RFC905_TPDU_EndOfTG = 0X80;

        /// <summary>
        /// RFC905 Header len
        /// </summary>
        public const int RFC905_HEADER_DataLen = 7;

        /// <summary>
        /// Authentication method enum
        /// </summary>
        public enum AUTHENTICATION_METHOD
        {
            NO_AUTHENTICATION_METHOD = 0x0,
            SERVER_AUTHENTICATION = 0x1,
            CLIENT_AUTHENTICATION = 0x2,
        };

        /// <summary>
        /// Authentication state enum
        /// </summary>
        public enum AUTHENTICATION_STATE
        {
            NO_AUTHENTICATION_STATE = 0x0,
            CLIENT_AUTHENTICATION_SEND = 0x1,
            AUTHENTICATION_SUCCESSFULL = 0x2,
        };
    }
}
