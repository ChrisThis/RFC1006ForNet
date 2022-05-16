//*************************************************************
//*************************************************************
// Autor: Egger Christopher 
// Web: homeTwoPointZero.dedyn.io
// Datum: 06.05.2022                                 
// Version: 1.0                                              
//                                                          
// Description:   
// Fill and read the data from RFC1006 nad RFC905 protocol
// 
// Changelog:
//                                                          
//*************************************************************
//*************************************************************


using System;
using System.Text;


namespace RFC1006ForNet.Intern
{
    /// <summary>
    /// RFC1006 data handler
    /// </summary>
    public class RFC1006DataHandler
    {
        /// <summary>
        /// Fill client header for authentication
        /// </summary>
        /// <param name="header">RFC1006 data holder.</param>
        public static void FillClientHeader(RFC1006Data rfc1006Data)
        {
            int offset = 12;

            rfc1006Data.Buffer[0] = 0x3; //Version is always 3
            rfc1006Data.Buffer[1] = 0x00; //Unknown

            //here should be the header len and telegram len, but we will do this after we insert all buffer

            rfc1006Data.Buffer[5] = RFC1006Constants.RFC905_TPDU_ConnRequest; // TPDU Code
            rfc1006Data.Buffer[6] = 0x00; // CR CDT
            rfc1006Data.Buffer[7] = 0x00; // DST -->
            rfc1006Data.Buffer[8] = 0x00; // REF
                                          //   rfc1006Data.Buffer[9] = 0x14; // 0x03; // SRC -->
            rfc1006Data.Buffer[9] = 0x47; // 0x03; // SRC -->
            rfc1006Data.Buffer[10] = 0x00; // REF
            rfc1006Data.Buffer[11] = 0xC1; // CLASS OPTION

            //local tsap
            rfc1006Data.Buffer[offset] = (byte)rfc1006Data.LocalTSAP.Length;
            offset++;
            //  memcpy(&rfc1006Data.buffer[offset], Header->localTSAP.c_str(), rfc1006Data.buffer[offset - 1] + 1);
            Array.Copy(Encoding.ASCII.GetBytes(rfc1006Data.LocalTSAP), 0, rfc1006Data.Buffer, offset, rfc1006Data.Buffer[offset - 1]);
            offset += rfc1006Data.Buffer[offset - 1];

            //TSap seperator
            rfc1006Data.Buffer[offset] = 0xc2;
            offset++;

            //remote tsap
            rfc1006Data.Buffer[offset] = (byte)rfc1006Data.RemoteTSAP.Length;
            offset++;

            Array.Copy(Encoding.ASCII.GetBytes(rfc1006Data.RemoteTSAP), 0, rfc1006Data.Buffer, offset, rfc1006Data.Buffer[offset - 1]);
            offset += rfc1006Data.Buffer[offset - 1];

            //Unknown buffer
            rfc1006Data.Buffer[offset] = 0xc0;
            offset++;
            rfc1006Data.Buffer[offset] = 0x01;
            offset++;
            rfc1006Data.Buffer[offset] = 0x0a;

            //Add user data
            //   memcpy(&rfc1006Data.buffer[offset], Header->UserData, rfc1006Data.lenOfUserData);
            Array.Copy(rfc1006Data.UserData, 0, rfc1006Data.Buffer, offset, rfc1006Data.LenOfUserData);

            //Len
            rfc1006Data.LenOfTPDU = offset + rfc1006Data.LenOfUserData + 1;

            var i = (byte)rfc1006Data.LenOfTPDU;
            rfc1006Data.Buffer[2] = (byte)((i >> 8) & 0xFF);
            rfc1006Data.Buffer[3] = (byte)(rfc1006Data.LenOfTPDU & 0xFF);
            rfc1006Data.Buffer[4] = (byte)(offset + 1 - 5);

            rfc1006Data.LenOfBuffer = rfc1006Data.LenOfTPDU;
        }

        /// <summary>
        /// Fill server header for authentication
        /// </summary>
        /// <param name="header">RFC1006 data holder.</param>
        public static void FillServerHeader(RFC1006Data rfc1006Data, RFC1006Data recvClientData)
        {
            int offset = 15;

            rfc1006Data.Buffer[0] = 0x3; //Version is always 3
            rfc1006Data.Buffer[1] = 0x00; //Unknown

            //here should be the header len and telegramm len, but we will this after we insert all buffer
            rfc1006Data.Buffer[5] = RFC1006Constants.RFC905_TPDU_ConnConfirm; // TPDU Code
            rfc1006Data.Buffer[6] = 0x00; // CR CDT

            //Copy pos 9 recv. from client into send data pos 7.
            rfc1006Data.Buffer[7] = recvClientData.Buffer[9];

            rfc1006Data.Buffer[8] = 0x00; // REF
            rfc1006Data.Buffer[9] = 0x4a; // SRC -->
            rfc1006Data.Buffer[10] = 0x00; // REF
            rfc1006Data.Buffer[11] = 0xC0; // CLASS OPTION
            rfc1006Data.Buffer[12] = 0x01; // Variable Part
            rfc1006Data.Buffer[13] = 0x0a; // User Data
            rfc1006Data.Buffer[14] = 0xc1; // User Data

            //local tsap
            rfc1006Data.Buffer[offset] = (byte)rfc1006Data.LocalTSAP.Length;
            offset++;
            Array.Copy(Encoding.ASCII.GetBytes(rfc1006Data.LocalTSAP), 0, rfc1006Data.Buffer, offset, rfc1006Data.Buffer[offset - 1]);
            offset += rfc1006Data.Buffer[offset - 1];

            //TSap seperator
            rfc1006Data.Buffer[offset] = 0xc2;
            offset++;

            //remote tsap
            rfc1006Data.Buffer[offset] = (byte)rfc1006Data.RemoteTSAP.Length;
            offset++;
            Array.Copy(Encoding.ASCII.GetBytes(rfc1006Data.RemoteTSAP), 0, rfc1006Data.Buffer, offset, rfc1006Data.Buffer[offset - 1]);
            offset += rfc1006Data.Buffer[offset - 1];

            //Len
            rfc1006Data.LenOfTPDU = offset + rfc1006Data.LenOfUserData;
            var i = (byte)rfc1006Data.LenOfTPDU;
            rfc1006Data.Buffer[2] = (byte)((i >> 8) & 0xFF);
            rfc1006Data.Buffer[3] = (byte)(rfc1006Data.LenOfTPDU & 0xFF);
            rfc1006Data.Buffer[4] = (byte)(offset - 5);

            rfc1006Data.LenOfBuffer = rfc1006Data.LenOfTPDU;
        }


        /// <summary>
        /// Check how many tpdu streams we have to write. Depence on user data length
        /// </summary>
        /// <param name="header">RFC1006 data holder.</param>
        /// <param name="userDataLength">Length of user data to send over socket</param>
        public static void FillTPDUCounts(RFC1006Data rfc1006Data, int userDataLength)
        {
            // 1TPDU has max 1024 bytes of user data
            rfc1006Data.TPDUCount = (userDataLength / (RFC1006Constants.RFC905_TPDU_MaxSize - RFC1006Constants.RFC905_HEADER_DataLen)) + 1;

            //Reset index
            rfc1006Data.IdxTPDU = 0;
            rfc1006Data.IdxUserData = 0;
        }

        /// <summary>
        /// Fill one tpdu with data
        /// </summary>
        /// <param name="header">RFC1006 data holder.</param>
        /// <param name="userDataLength">Length of user data to send over socket</param>
        public static void FillDataHeader(RFC1006Data rfc1006Data, byte[] userData, int userDataLength)
        {
            rfc1006Data.Buffer[0] = 0x3; //Version is always 3
            rfc1006Data.Buffer[1] = 0x00; //Unknown
            rfc1006Data.Buffer[4] = 0x02; //Header len
            rfc1006Data.Buffer[5] = 0xf0; //code  |credit

            //Check if we send mor then one TPDU
            if (rfc1006Data.TPDUCount > 1)
            {
                rfc1006Data.Buffer[6] = 0; //TPDU-NR and EOT 100 000 --> 0x80
                rfc1006Data.LenOfTPDU = RFC1006Constants.RFC905_TPDU_MaxSize;
            }
            else
            {
                rfc1006Data.Buffer[6] = RFC1006Constants.RFC905_TPDU_EndOfTG; //TPDU-NR and EOT 100 000 --> 0x80
                rfc1006Data.LenOfTPDU = userDataLength - rfc1006Data.IdxUserData + RFC1006Constants.RFC905_HEADER_DataLen;
            }

            Array.Copy(userData, 0, rfc1006Data.Buffer, RFC1006Constants.RFC905_HEADER_DataLen, userDataLength);

            rfc1006Data.LenOfBuffer = rfc1006Data.LenOfTPDU; //userDataLength

            //Set len of TPDU
            //  rfc1006Data.Buffer[2] = (rfc1006Data.LenOfTPDU >> 8) & 0xFF;
            //   rfc1006Data.Buffer[3] = rfc1006Data.LenOfTPDU & 0xFF;
            byte[] arr = BitConverter.GetBytes(rfc1006Data.LenOfTPDU);
            rfc1006Data.Buffer[2] = arr[1];
            rfc1006Data.Buffer[3] = arr[0];

            //Decrempt TPDUCount
            rfc1006Data.TPDUCount--;

            //Set index of next PDU
            rfc1006Data.IdxTPDU += RFC1006Constants.RFC905_TPDU_MaxSize;

            //Set index of next user data
            rfc1006Data.IdxUserData += rfc1006Data.LenOfTPDU - RFC1006Constants.RFC905_HEADER_DataLen;
        }

        /// <summary>
        /// Recv header from server (we are client) authentication header
        /// </summary>
        /// <param name="header">RFC1006 data holder.</param>
        public static bool RecFillClientAuthHeader(RFC1006Data rfc1006Data)
        {
            int offset = 15;

            rfc1006Data.LenOfTPDU = rfc1006Data.Buffer[3];
            rfc1006Data.LenOfUserData = rfc1006Data.LenOfTPDU - 5 - rfc1006Data.Buffer[4];

            rfc1006Data.LenRemoteTsap = rfc1006Data.Buffer[offset];
            offset++;

            rfc1006Data.RemoteTSAP = "";
            rfc1006Data.RemoteTSAP = Encoding.Default.GetString(rfc1006Data.Buffer, offset, rfc1006Data.LenRemoteTsap);
            offset += rfc1006Data.LenRemoteTsap;
            offset += 1;

            rfc1006Data.LenLocalTsap = rfc1006Data.Buffer[offset];
            offset++;
            rfc1006Data.LocalTSAP = "";
            rfc1006Data.LocalTSAP = Encoding.Default.GetString(rfc1006Data.Buffer, offset, rfc1006Data.LenLocalTsap);

            return rfc1006Data.Buffer[3] <= rfc1006Data.LenOfTPDU;
        }


        /// <summary>
        /// Recv header from client (we are server) authentication header
        /// </summary>
        /// <param name="header">RFC1006 data holder.</param>
        public static bool RecFillServerAuthHeader(RFC1006Data rfc1006Data)
        {
            int offset = 12;

            rfc1006Data.LenOfBuffer = rfc1006Data.Buffer[3];
            rfc1006Data.LenOfUserData = rfc1006Data.LenOfBuffer - 5 - rfc1006Data.Buffer[4];

            rfc1006Data.LenRemoteTsap = rfc1006Data.Buffer[offset];
            offset++;

            rfc1006Data.RemoteTSAP = "";
            rfc1006Data.RemoteTSAP = Encoding.Default.GetString(rfc1006Data.Buffer, offset, rfc1006Data.LenRemoteTsap);
            offset += rfc1006Data.LenRemoteTsap;
            offset += 1;

            rfc1006Data.LenLocalTsap = rfc1006Data.Buffer[offset];
            offset++;
            rfc1006Data.LocalTSAP = "";
            rfc1006Data.LocalTSAP = Encoding.Default.GetString(rfc1006Data.Buffer, offset, rfc1006Data.LenLocalTsap);

            return rfc1006Data.Buffer[3] <= rfc1006Data.LenOfBuffer;
        }


        /// <summary>
        /// Fill header with recv data
        /// </summary>
        /// <param name="header">RFC1006 data holder.</param>
        public static bool RecFillDataHeader(RFC1006Data rfc1006Data)
        {
            if (rfc1006Data.LenOfBuffer < RFC1006Constants.RFC905_HEADER_DataLen) return false;

            rfc1006Data.LenOfTPDU = ((rfc1006Data.Buffer[rfc1006Data.IdxTPDU + 2] & 0xFF) << 8);
            rfc1006Data.LenOfTPDU += rfc1006Data.Buffer[rfc1006Data.IdxTPDU + 3] & 0xFF;

            rfc1006Data.LenOfUserData = rfc1006Data.LenOfTPDU - RFC1006Constants.RFC905_HEADER_DataLen;

            rfc1006Data.EndOfTPDU = ((char)rfc1006Data.Buffer[rfc1006Data.IdxTPDU + 6] == (char)(RFC1006Constants.RFC905_TPDU_EndOfTG));

            return rfc1006Data.LenOfBuffer >= rfc1006Data.LenOfTPDU;
        }


        /// <summary>
        /// Add user data to buffer
        /// </summary>
        /// <param name="header">RFC1006 data holder.</param>
        public static void AddUserDataBuffer(RFC1006Data rfc1006Data)
        {
            var idxBufferStart = rfc1006Data.IdxTPDU + RFC1006Constants.RFC905_HEADER_DataLen;
            Array.Copy(rfc1006Data.Buffer, idxBufferStart, rfc1006Data.UserData, rfc1006Data.CompleteLenOfUserData, rfc1006Data.LenOfUserData);
            rfc1006Data.CompleteLenOfUserData += rfc1006Data.LenOfUserData;
        }


        /// <summary>
        /// Select next TPDU packet
        /// </summary>
        /// <param name="header">RFC1006 data holder.</param>
        public static bool SelectNextTPDU(RFC1006Data rfc1006Data)
        {
            int idx = rfc1006Data.IdxTPDU + rfc1006Data.LenOfTPDU;
            //Select index of next TPDU
            rfc1006Data.IdxTPDU = idx;

            if (idx >= rfc1006Data.LenOfBuffer) return false;
            //Select index of next TPDU
            //   rfc1006Data.IdxTPDU = idx;

            return true;
        }


        /// <summary>
        /// Is TPDU finished
        /// </summary>
        /// <param name="header">RFC1006 data holder.</param>
        public static bool EndOfTPDU(RFC1006Data rfc1006Data)
        {
            return rfc1006Data.EndOfTPDU && ((rfc1006Data.IdxTPDU + rfc1006Data.LenOfTPDU) <= rfc1006Data.LenOfBuffer);
        }


        /// <summary>
        /// Reset buffer
        /// </summary>
        /// <param name="header">RFC1006 data holder.</param>
        public static void ResetBuffer(RFC1006Data rfc1006Data)
        {
            //Copy last buffer to first
            rfc1006Data.LenOfBuffer = rfc1006Data.LenOfBuffer - (rfc1006Data.LenOfTPDU + rfc1006Data.IdxTPDU);
            //   memcpy(&rfc1006Data.Buffer[0], &rfc1006Data.Buffer[Header->idxTPDU + Header->lenOfTPDU], Header->lenOfBuffer);
            Array.Copy(rfc1006Data.Buffer, rfc1006Data.IdxTPDU + rfc1006Data.LenOfTPDU, rfc1006Data.Buffer, 0, rfc1006Data.LenOfBuffer);

            rfc1006Data.IdxTPDU = 0;

            //Reset user data
            rfc1006Data.UserData = new byte[RFC1006Constants.RFC1006BufferSize];
            rfc1006Data.CompleteLenOfUserData = 0;
        }
    }
}
