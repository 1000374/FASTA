using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABIF_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            #region
            //float f = 0.9599f;
            //byte[] bbyte = BitConverter.GetBytes(f);
            //Array.Reverse(bbyte);
            //string s = BitConverter.ToString(bbyte).Replace("-", "");
            //double d = 0.384756887;
            //byte[] dbyte = BitConverter.GetBytes(d);
            //Array.Reverse(dbyte);
            //string s1 = BitConverter.ToString(dbyte).Replace("-", "");


            //List<byte> lllll = new List<byte>();
            //byte[] nnnn = new byte[20];
            //lllll.AddRange(nnnn);
            //byte[] mmm = { 1, 2, 3, 4, 5 };
            //byte[] test = lllll.ToArray();

            //Array.Copy(mmm, 0, test, 2, mmm.Length);
            //lllll.Clear();
            //lllll.AddRange(test);
            #endregion

            byte[] sssss = { 12, 15, 167 };
            string strMonth = Convert.ToInt16(sssss[2]).ToString();

            float d = 0.8706f;

            string hexStr = "A4DF5E3F";//3F5EDFA4

            // byte[] buf = pf.HexStringToByteArray(hexStr);

            // float f = BitConverter.ToSingle(buf, 0);


            byte[] bbyte = BitConverter.GetBytes(d);
            // Array.Reverse(bbyte);
            string ss = BitConverter.ToString(bbyte).Replace("-", "").PadLeft(8, '0');
            // buf = pf.HexStringToByteArray(ss);
            //  f = BitConverter.ToSingle(buf, 0);

            long b = Convert.ToInt64(ss, 16);
            double x = (double)b;


            //PubFunction pf = new PubFunction();
            byte[] bascii = Encoding.ASCII.GetBytes("4");
            char ch = (char)61;
            byte[] nyt = { 1, 0, 1 };
            string bbbb = Encoding.ASCII.GetString(nyt);


            // byte[] bvalue = pf.HexStringToByteArray(Convert.ToString(61, 16).PadLeft(2, '0'));
            string txt1 = "78,78,65,65,78,67,84,71,71,71,84,84,65,71,71,67,84,71,71,84,71,84,84,65,71,71,71,84,84,67,84,84,84,71,84,84,84,84,84,71,71,71,71,84,84,84,71,71,67,65,71,65,71,65,84,71,84,71,84,84,84,65,65,71,84,71,67,84,71,84,71,71,67,67,65,71,65,65,71,67,71,71,71,71,71,71,65,71,71,71,71,71,71,71,84,84,84,71,71,84,71,71,65,65,65,84,84,84,84,84,84,71,84,84,65,84,71,65,84,71,84,67,84,71,84,71,84,71,71,65,65,65,71,67,71,71,67,84,71,84,71,67,65,71,65,67,65,84,84,67,65,65,84,84,71,84,84,65,84,84,65,84,84,65,84,71,84,67,67,84,65,67,65,65,71,67,65,84,84,65,65,84,84,65,65,84,84,65,65,67,65,67,65,67,84,84,84,65,71,84,65,65,71,84,65,84,71,84,84,67,71,67,67,84,71,84,65,65,84,65,84,84,71,65,65,67,71,84,65,71,71,84,71,67,71,65,84,65,65,65,84,65,65,84,71,71,71,65,84,71,65,71,71,67,65,71,71,65,65,84,67,65,65,65,71,65,67,65,71,65,84,65,67,84,71,67,71,65,67,65,84,65,71,71,71,84,71,67,84,67,67,71,71,67,84,67,67,65,71,67,71,84,67,84,67,71,67,65,65,84,71,67,84,65,84,67,71,67,71,84,71,67,65,67,65,67,67,67,67,67,67,65,71,65,67,71,65,65,65,65,84,65,67,67,65,65,65,84,71,67,65,84,71,71,65,71,65,71,67,84,67,67,67,71,84,71,65,71,84,71,71,84,84,65,65,84,65,71,71,71,84,71,65,84,65,71,65,67,67,65,67,84,71,71,67,67,71,84,67,71,84,67,71,71,84,84,84,84,65,67,65,65";
            int[] nnnn = Array.ConvertAll<string, int>(txt1.Split(','), s => Convert.ToInt16(s));
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < nnnn.Length; i++)
            {
                sb.Append(Convert.ToString((char)nnnn[i]));
            }
            sb.ToString();
            ///////////
            //写标签//
            //////////
            //string path = Environment.CurrentDirectory + "\\test40.fsa";
            //string pathtxt = Environment.CurrentDirectory + "\\fsa标签模板4色.txt";
            //string[] txt = File.ReadAllLines(pathtxt);
            //string[] C_tag = new string[txt.Length];
            //string[] C_Type = new string[txt.Length];
            //string[] C_Dataoffset = new string[txt.Length];
            //for (int i = 0; i < txt.Length; i++)
            //{
            //    string[] strtxt = txt[i].Split(new string[] { "[", "]", ";" }, StringSplitOptions.RemoveEmptyEntries);
            //    C_tag[i] = strtxt[0];
            //    C_Type[i] = strtxt[1];
            //    C_Dataoffset[i] = strtxt[2];
            //}

            //ABIF abif = new ABIF(path);
            //List<DirEntryValue> list = new List<DirEntryValue>();
            ////string[] C_tag = { "CTID", "CTTL", "CpEP", "DATA1", "EpVt", "RUND1", "RUNT1", "NOIS", "NrmS" };
            ////string[] C_Type = { "cString", "pString", "char", "short", "long", "date", "time", "float", "double" };
            ////string[] C_Dataoffset = { "wwwqwwwedsfdsfddf", "weatestyy", "what", "1,2,3,4,5,6,7,8,9,12", "15000", "2018/3/29", "18:43:43:20", "0.9599,1.3906,1.4316,1.4092", "0.9599333,1.390336,1.4344416,1.4055592" };
            ////String[] C_tag = { "NrmS" };
            ////string[] C_Type = { "double" };
            ////string[] C_Dataoffset = { "0.9599333,1.390336,1.4344416,1.4055592" };
            //for (int i = 0; i < C_tag.Length; i++)
            //{
            //    DirEntryValue dirv = new DirEntryValue();
            //    dirv.Name = C_tag[i];
            //    if (dirv.Name.Length == 4)
            //    {
            //        dirv.Number = 1;
            //    }
            //    else
            //    {
            //        string name = dirv.Name.Substring(0, 4);
            //        dirv.Number = int.Parse(dirv.Name.Substring(4));
            //        dirv.Name = name;
            //    }
            //    dirv.Elementtype = C_Type[i];
            //    dirv.Dataoffset = C_Dataoffset[i];
            //    dirv.Datahandle = "0";
            //    list.Add(dirv);
            //}
            //abif.Write_ABIF(list);

            ////////////
            //替换标签//
            ////////////

            //string path = Environment.CurrentDirectory + "\\test26 - 副本.fsa";
            //string pathtxt = Environment.CurrentDirectory + "\\test.txt";
            //string[] txt = File.ReadAllLines(pathtxt);
            //string[] C_tag = new string[txt.Length];
            //string[] C_num = new string[txt.Length];
            //string[] C_Dataoffset = new string[txt.Length];
            //ABIF abif = new ABIF(path);
            //for (int i = 0; i < txt.Length; i++)
            //{
            //    string[] strtxt = txt[i].Split(new string[] { "[", "]", ";" }, StringSplitOptions.RemoveEmptyEntries);
            //    abif.Replace_ABIF_Tag( strtxt[0], int.Parse(strtxt[1]), strtxt[2]);
            //}


            ////////////
            //读取标签//
            ////////////
            string pathtag = Environment.CurrentDirectory + "\\fsa标签模板4色.txt";

            string path = Environment.CurrentDirectory + "\\temp-05.fsa";
            ABIF abif = new ABIF(path);
            bool b1 = abif.is_ABIF_Format();
            string ssss = abif.File_Version();

            Int32 iiii = abif.Num_Dir_Entries();

            string[] txt = File.ReadAllLines(pathtag);
            string name = "";
            int num = 0;
            StringBuilder sb2 = new StringBuilder();
            for (int i = 0; i < txt.Length; i++)
            {
                string[] tagnum = txt[i].Split(new string[] { "[", "]", ";" }, StringSplitOptions.RemoveEmptyEntries);
                if (tagnum[0].Length == 4)
                {
                    name = tagnum[0];
                    num = 1;
                }
                else
                {
                    name = tagnum[0].Substring(0, 4);
                    num = int.Parse(tagnum[0].Substring(4));
                }
                sb2.Append(abif.Tag_ABIF_Value(name, num) + "\r\n");
                // Console.WriteLine(abif.Tag_ABIF_Value(path, name, num));
            }
            string path1 = Environment.CurrentDirectory + "\\5moban.txt";
            File.WriteAllText(path1, sb2.ToString());

            //// console.writeline(abif.tag_abif_type(path, "ctid", 1));
            //console.writeline(abif.tag_abif_value(path, "ctid", 1));
            Console.ReadKey();
        }
    }
}
