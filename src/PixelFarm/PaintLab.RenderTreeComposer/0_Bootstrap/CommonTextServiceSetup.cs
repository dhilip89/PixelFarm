﻿//MIT, 2017-2018, WinterDev

using System.IO;
using Typography.TextServices;
namespace YourImplementation
{
    class MyIcuDataProvider : Typography.TextBreak.IIcuDataProvider
    {
        public string icuDir;

        public Stream GetDataStream(string strmUrl)
        {
            string fullname = icuDir + "/" + strmUrl;
            if (File.Exists(fullname))
            {
                return new FileStream(fullname, FileMode.Open);
            }
            return null;
        }
    }

    static class CommonTextServiceSetup
    {
        static bool s_isInit;
        internal static MyIcuDataProvider s_icuDataProvider;
        internal static IFontLoader myFontLoader;


        public static void SetupDefaultValues()
        {
            if (s_isInit) return;

            //
            myFontLoader = new OpenFontStore();
            //test Typography's custom text break, 
            //check if we have that data?

            //string typographyDir = @"../../PixelFarm/Typography/Typography.TextBreak/icu58/brkitr_src/dictionaries";
            string typographyDir = @"../../PixelFarm/Typography/Typography.TextBreak/icu60/brkitr_src/dictionaries";
            s_icuDataProvider = new MyIcuDataProvider();
            if (System.IO.Directory.Exists(typographyDir))
            {
                s_icuDataProvider.icuDir = typographyDir;
            }
            Typography.TextBreak.CustomBreakerBuilder.Setup(s_icuDataProvider);
            s_isInit = true;            
        }
    }


}