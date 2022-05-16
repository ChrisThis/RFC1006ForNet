//*************************************************************
//*************************************************************
// Autor: Egger Christopher 
// Web: homeTwoPointZero.dedyn.io
// Datum: 06.05.2022                                 
// Version: 1.0                                              
//                                                          
// Description:                                                         
// Tools to make my life easy... 
//
// Changelog:
//                                                          
//*************************************************************
//*************************************************************


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFC1006ForNet.Intern
{
    /// <summary>
    /// RFC1006 tools to make my life easy
    /// </summary>
    public class RFC1006Tools
    {
        /// <summary>
        /// Conerty the byte array into hex string and write it out
        /// </summary>
        public static void DumpByteArray(byte[] ba, int len)
        {
            StringBuilder hex = new StringBuilder(len * 2);
            int cnt = 0;
            foreach (byte b in ba)
            {
                if (cnt >= len) break;
                cnt++;
                hex.AppendFormat("{0:x2}", b);
            }
            string s = hex.ToString();
            Console.WriteLine(s);
        }
    }
}
