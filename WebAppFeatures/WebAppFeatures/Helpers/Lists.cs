using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppFeatures.Helpers
{
    public static class Lists
    {
        public enum GetCountryListFromEnum
        {
            Egypt
        }
        public enum GetCodeCountryListFromEnum
        {
            EG
        }
        public enum GetCityListFromEnum
        {
            Alexandria,
            Cairo,
            Giza,
            Suez,
            Ismailia,
            Qena,
            Luxor,
            KafrAlSheikh,
            Matruh
        }
        public enum Gender
        {
            None,
            Male,
            Female
        }
    }
}
