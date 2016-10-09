using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows;
using StockSharp.Algo.Candles;
using StockSharp.Algo.Storages;
using System.Collections.ObjectModel;
using StockSharp.Localization;
using StockSharp.BusinessEntities;
using StockSharp.Messages;

namespace SDataExt
{
    public class DataLoader
    {
        public string Code { get; set; }
        public TimeSpan TimeFrame { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public string DType { get; set; }
        public string Path { get; set; }
        public double[] dlDates { get; set; }
        public double[,] dlInfo { get; set; }

        public string[] SecArray{ get; set; }

        public List<string> SecList { get; set; }

        private StorageRegistry str { get; set; }
        private LocalMarketDataDrive lmd { get; set; }


        private class SecCatalogEntry
        {
            public string SecCode { get; set; }
            public string SecPath { get; set; }

        }

        public DataLoader(string _Code, TimeSpan _TimeFrame, DateTime _From, DateTime _To, string _DType, string _Path)
        {
            Code = _Code;
            TimeFrame = _TimeFrame;
            From = _From;
            To = _To;
            DType = _DType;
            Path = _Path != null ? _Path : @"D:\SH\Hydra";
            if (dlDates==null) InitDl();
        }

        public DataLoader()
        {
            Path =  @"D:\SH\Hydra";
            InitDl();
        }

        public void InitDl()
        {
            dlDates = new double[0];
            dlInfo = new double[0,0];
            SecList = new List<string>();
            SecArray = new string[0];
            //SStor = new SimStorage();
            str = new StorageRegistry();
            lmd = new LocalMarketDataDrive();
            lmd.Path = Path;
            str.DefaultDrive = lmd; //
        }

        public double GetList()
        {
            double result = 0; // Неудачный результат
            int i = 0;
            var DList = Directory.GetDirectories(Path, "*",SearchOption.AllDirectories).Where(ds=>ds.Contains("@") && !ds.Contains("20") && !ds.Contains("Temp"));
            foreach (string secDir in DList)
            {
                string secName = secDir.Split("\\".ToCharArray()).FirstOrDefault(f => f.Contains("@"));
                if (secName == null) secName = secDir;
                SecList.Add(secName);
                i = i +1;
            }

            SecArray = SecList.ToArray();
            if (SecArray.Length>1)  result = 1; // Yдачный результат
            return result;
        }

        public double GetData()
        {
            
            double[] dlClose;
            double[] dlLow;
            double[] dlHigh;
            double[] dlOpen;
            double[] dlVol;


            if (Code == null || TimeFrame == null || From == null || To == null || DType == null || Path == null)
                MessageBox.Show("Not all parameters are specified!");

            var candles = str.GetCandleStorage(typeof (TimeFrameCandle), CreateSecurity(Code), TimeFrame).Load(From, To);
            
            double result = 0; // Неудачный результат
            if (candles.Any() == false)
            {   result = 0;// Неудачный результат
            }
            else
            {
                result = 1;// Удачный результат
                dlDates = candles.Select(c => (double)c.CloseTime.Ticks / 10000000 / 3600 / 24 + 367).ToArray();

                switch (DType)
                {
                    case "A":
                    case "ALL":
                        dlClose = candles.Select(c => (double)c.ClosePrice).ToArray();
                        dlOpen = candles.Select(c => (double)c.OpenPrice).ToArray();
                        dlLow = candles.Select(c => (double)c.LowPrice).ToArray();
                        dlHigh = candles.Select(c => (double)c.HighPrice).ToArray();
                        dlVol = candles.Select(c => (double)c.TotalVolume).ToArray();
                        dlInfo = ConcatCW(dlOpen,dlHigh,dlLow,dlClose,dlVol);
                        break;
                    case "C":
                    case "CLOSE":
                        dlClose = candles.Select(c => (double)c.ClosePrice).ToArray();
                        dlInfo = ConcatCW(dlClose);
                        break;
                    case "O":
                    case "OPEN":
                        dlOpen = candles.Select(c => (double)c.OpenPrice).ToArray();
                        dlInfo = ConcatCW(dlOpen);
                        break;
                    case "V":
                    case "VOL":
                        dlVol = candles.Select(c => (double)c.TotalVolume).ToArray();
                        dlInfo = ConcatCW(dlVol);
                        break;
                    default:
                        dlClose = candles.Select(c => (double)c.ClosePrice).ToArray();
                        dlInfo = ConcatCW(dlClose);
                        break;
                }
            }
            
            return result;


        }

        public Security CreateSecurity(string secStr)
        {
            Security security;

            int strLn = secStr.IndexOfAny("@".ToCharArray());
            string secBorad = secStr.Substring(strLn + 1);

            ExchangeBoard eb = new ExchangeBoard();
            SecurityTypes st = new SecurityTypes();
            switch (secBorad)
            {
                case "FORTS":
                    eb = ExchangeBoard.Forts;
                    st = SecurityTypes.Future;
                    break;
                case "EQBR":
                    eb = ExchangeBoard.MicexEqbr;
                    st = SecurityTypes.Stock;
                    break;
                default:
                    eb = ExchangeBoard.Micex;
                    st = SecurityTypes.Future;
                    break;
            }

            string secCode = secStr.Substring(0, strLn);

            security = new Security()
            {
                Id = secStr,
                Code = secCode,
                Board = eb,
                Type = st
            };

            return security;

        }

        public static T[,] ConcatCW<T>(params T[][] arrays)
        {
            int length = arrays.Max(ar=>ar.Length); 
            int n = arrays.Count();
            
            T[,] result = new T[n,length];
            int ind = 0;

            foreach (T[] array in arrays)
            {
                T[,] source = new T[1,length];
                for (int i = 0; i < length; i++)
                    source.SetValue(array[i], 0,i);
                Array.Copy(source, 0, result, ind, length);
                ind += length;
            }
            return result;
        }
    }

    


}
