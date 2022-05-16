//*************************************************************
//*************************************************************
// Autor: Egger Christopher 
// Web: homeTwoPointZero.dedyn.io
// Datum: 06.05.2022                                 
// Version: 1.0                                              
//                                                          
// Description:                                                         
// RFC1006 tcp client.
// Evaluate and send the data 
//
// Changelog:
//                                                          
//*************************************************************
//*************************************************************


using System;
using System.Net;
using System.Net.Sockets;
using RFC1006ForNet.Intern;


namespace RFC1006ForNet
{
    /// <summary>
    /// RFC1006 tcp client
    /// </summary>
    public abstract class RFC1006TcpClient
    {
        #region private members

        /// <summary>
        /// RFC1006 send data
        /// </summary>
        private RFC1006Data _sendData = new RFC1006Data();

        /// <summary>
        /// RFC1006 recv data
        /// </summary>
        private RFC1006Data _recvData = new RFC1006Data();

        /// <summary>
        /// Tcp client for raw data handling over the socket
        /// </summary>
        private TcpClient _tcpClient = null;

        /// <summary>
        /// Authentication mehtod (we are client or server)
        /// </summary>
        private RFC1006Constants.AUTHENTICATION_METHOD _authMethod = RFC1006Constants.AUTHENTICATION_METHOD.NO_AUTHENTICATION_METHOD;

        /// <summary>
        /// Authentication state
        /// </summary>
        private RFC1006Constants.AUTHENTICATION_STATE _authState = RFC1006Constants.AUTHENTICATION_STATE.NO_AUTHENTICATION_STATE;

        /// <summary>
        /// Recv raw data buffer for tcp client
        /// </summary>
        private byte[] _recvBuffer = new byte[RFC1006Constants.RFC1006BufferSize];

        #endregion


        #region C'tor

        /// <summary>
        /// C'tor
        /// </summary>
        public RFC1006TcpClient()
        {
        }

        #endregion

        #region public methods

        /// <summary>
        /// Connect to an RFC1006 server
        /// </summary>
        public void ConnectToServer(string host, string localTSAP, string remoteTSAP)
        {
            Console.WriteLine("Connect to rfc1006 server");
            _sendData.LocalTSAP = localTSAP;
            _sendData.RemoteTSAP = remoteTSAP;

            _authMethod = RFC1006Constants.AUTHENTICATION_METHOD.CLIENT_AUTHENTICATION;

            //   IPHostEntry host1 = Dns.GetHostEntry(host);
            IPAddress ipAddress = IPAddress.Parse(host);
            IPEndPoint endPoint = new IPEndPoint(ipAddress, 102);

            if (_tcpClient == null)
            {
                _tcpClient = new TcpClient(endPoint);
            }
            _tcpClient.Connect(endPoint);
        }

        /// <summary>
        /// TcpClient from an tcp server connection
        /// </summary>
        public void SetServerTcpClient(TcpClient c, string localTSAP, string remoteTSAP)
        {
            _tcpClient = c;
            _sendData.LocalTSAP = localTSAP;
            _sendData.RemoteTSAP = remoteTSAP;
            _authMethod = RFC1006Constants.AUTHENTICATION_METHOD.SERVER_AUTHENTICATION;

            // RecvRawData();
            while (_tcpClient.Connected)
            {
                RecvRawData();
            }
        }

        /// <summary>
        /// Send data and rturny sended bytes
        /// </summary>
        /// <param name="data">Userdata to send.</param>
        /// <param name="length">Length of user data.</param>
        public int SendData(byte[] data, int length)
        {
            _sendData.LenOfUserData = length;
            Array.Copy(data, 0, _sendData.UserData, 0, length);

            Console.WriteLine("Send user data " + length);

            RFC1006DataHandler.FillTPDUCounts(_sendData, length);

            while (_sendData.TPDUCount > 0)
            {
                RFC1006DataHandler.FillDataHeader(_sendData, data, length);
                int bWritten = _tcpClient.Client.Send(_sendData.Buffer, _sendData.LenOfTPDU, SocketFlags.None);
                Console.WriteLine("Bytes written " + bWritten);
            }

            return length;
        }

        #endregion

        #region private members

        /// <summary>
        /// Recv and evaluate raw data from an tcp client
        /// </summary>
        private void RecvRawData()
        {
            try
            {
                int bytesRead = _tcpClient.Client.Receive(_recvBuffer, 0, RFC1006Constants.RFC1006BufferSize, SocketFlags.None);
                Console.WriteLine("Raw data recv " + bytesRead);
                if (bytesRead > 0)
                {
                    //Copy data into our buffer
                    Array.Copy(_recvBuffer, 0, _recvData.Buffer, _recvData.LenOfBuffer, bytesRead);
                    _recvData.LenOfBuffer += bytesRead;

                    while (RFC1006DataHandler.RecFillDataHeader(_recvData))
                    {
                        //    RFC1006Tools.DumpByteArray(_recvData.Buffer, _recvData.LenOfBuffer);

                        if (_authState == RFC1006Constants.AUTHENTICATION_STATE.AUTHENTICATION_SUCCESSFULL)
                        {
                            RFC1006DataHandler.AddUserDataBuffer(_recvData);

                            if (RFC1006DataHandler.EndOfTPDU(_recvData))
                            {
                                //Data recv 
                                NewDataRecv(_recvData.UserData, _recvData.CompleteLenOfUserData);
                                RFC1006DataHandler.ResetBuffer(_recvData);
                            }
                            else
                            {
                                if (!RFC1006DataHandler.SelectNextTPDU(_recvData)) return;
                                continue;
                            }
                        }
                        else
                        {
                            if (_authState == RFC1006Constants.AUTHENTICATION_STATE.CLIENT_AUTHENTICATION_SEND)
                            {
                                Console.WriteLine("Client auth send");
                                if (!RFC1006DataHandler.RecFillClientAuthHeader(_recvData)) return;
                                if (_sendData.LocalTSAP != _recvData.RemoteTSAP)
                                {
                                    Console.WriteLine("Recv local tsap isn't the same like send remote tsap");
                                    //Disconnect
                                    return;
                                }
                                else if (_recvData.LocalTSAP != _sendData.RemoteTSAP)
                                {
                                    Console.WriteLine("Recv remote tsap isn't the same like send local tsap");
                                    //Disconnect
                                    return;
                                }

                                _authState = RFC1006Constants.AUTHENTICATION_STATE.AUTHENTICATION_SUCCESSFULL;
                                RFC1006DataHandler.ResetBuffer(_recvData);

                                //Connected
                            }
                            else
                            {
                                Console.WriteLine("Server auth send");
                                if (!RFC1006DataHandler.RecFillServerAuthHeader(_recvData)) return;
                                if (_sendData.LocalTSAP != _recvData.RemoteTSAP)
                                {
                                    Console.WriteLine("Recv local tsap isn't the same like send remote tsap");
                                    //Disconnect
                                    return;
                                }
                                else if (_recvData.LocalTSAP != _sendData.RemoteTSAP)
                                {
                                    Console.WriteLine("Recv remote tsap isn't the same like send local tsap");
                                    //Disconnect
                                    return;
                                }

                                _sendData.LenOfUserData = 0;
                                RFC1006DataHandler.FillServerHeader(_sendData, _recvData);

                                //  _sendData.Buffer[7] = _recvData.Buffer[9];
                                SendRawData();
                                _authState = RFC1006Constants.AUTHENTICATION_STATE.AUTHENTICATION_SUCCESSFULL;
                                RFC1006DataHandler.ResetBuffer(_recvData);
                            }
                        }
                    }
                }

                //    RecvRawData();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        /// <summary>
        /// Send raw data on an tcp client
        /// </summary>
        private void SendRawData()
        {
            RFC1006Tools.DumpByteArray(_sendData.Buffer, _sendData.LenOfBuffer);

            int bytesSent = _tcpClient.Client.Send(_sendData.Buffer, _sendData.LenOfBuffer, SocketFlags.None);
            Console.WriteLine("Sent {0} bytes.", bytesSent);
        }

        #endregion

        #region abstract data methods

        /// <summary>
        /// New data from RFC1006 socket recv
        /// </summary>
        public abstract void NewDataRecv(byte[] recvData, int length);

        #endregion
    }
}

