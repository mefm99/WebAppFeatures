﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WebAppFeatures.Helpers
{
    public class GenerateRandomNumbers
    {
        public static string RandomNumbers(int length)
        {
            const string valid = "1234567890";
            StringBuilder res = new StringBuilder();
            using RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] uintBuffer = new byte[sizeof(uint)];

            while (length-- > 0)
            {
                rng.GetBytes(uintBuffer);
                uint num = BitConverter.ToUInt32(uintBuffer, 0);
                res.Append(valid[(int)(num % (uint)valid.Length)]);
            }

            return res.ToString();
        }
    }
}
