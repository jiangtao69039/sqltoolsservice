// WARNING:
// This file was generated by the Microsoft DataWarehouse String Resource Tool 7.0.0.0
// from information in sr.strings
// DO NOT MODIFY THIS FILE'S CONTENTS, THEY WILL BE OVERWRITTEN
//
namespace Microsoft.SqlTools.Migration
{
    using System;
    using System.Reflection;
    using System.Resources;
    using System.Globalization;

    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class SR
    {
        protected SR()
        { }

        public static CultureInfo Culture
        {
            get
            {
                return Keys.Culture;
            }
            set
            {
                Keys.Culture = value;
            }
        }


        [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
        public class Keys
        {
            static ResourceManager resourceManager = new ResourceManager("Microsoft.SqlTools.Migration.Localization.SR", typeof(SR).GetTypeInfo().Assembly);

            static CultureInfo _culture = null;


            private Keys()
            { }

            public static CultureInfo Culture
            {
                get
                {
                    return _culture;
                }
                set
                {
                    _culture = value;
                }
            }

            public static string GetString(string key)
            {
                return resourceManager.GetString(key, _culture);
            }

        }
    }
}
