using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABIF_Test
{
    /******************************************************************* 
   * Copyright (C)  版权所有
   * 文件名称:ABIF
   * 命名空间:ABIF_Test
   * 创建时间:2018年3月29日13:12:31
   * 作    者: 陈杰
   * 描    述：ABIF的编写替换等操作
   * 修改记录:修改类的记录
   * 修改人:修改此类的人名称
   * 版 本 号:v1.0.0
   **********************************************************************/
    public class ABIF
    {
        private const string VERSION = "1.01";
        private readonly string path;
        public ABIF(string path)
        {
            this.path = path;
        }
        /// <summary>
        /// 写文件抬头
        /// </summary>
        /// <param name="num_element">所有标签个数</param>
        /// <param name="dataOffset">第一个标签的偏移量</param>
        /// <returns></returns>
        private byte[] Write_ABIF_Head(int num_element, int dataOffset)
        {
            List<byte> list_head = new List<byte>();
            byte[] bheadinfo = { 0x41, 0x42, 0x49, 0x46, 0x00, 0x65, 0x74, 0x64, 0x69, 0x72, 0x00, 0x00, 0x00, 0x01, 0x03, 0xff, 0x00, 0x1c };
            string strnumele = Convert.ToString(num_element, 16).PadLeft(8, '0');
            byte[] bnumelement = HexStringToByteArray(strnumele);
            string strdatasize = Convert.ToString(num_element * 28, 16).PadLeft(8, '0');
            byte[] bdatasize = HexStringToByteArray(strdatasize);

            byte[] bnull = { 0x0, 0x0, 0x0, 0x0 };

            string strotherInfo = "Company:SuYuanJiYin,Author:chenjie,E-mail:jc1000374@hotmail.com";
            byte[] bohter = Encoding.ASCII.GetBytes(strotherInfo);
            //byte[] bohter = new byte[94];
            //for (int i = 0; i < bohter.Length; i++)
            //{
            //    bohter[i] = 255;
            //}

            string strdataOffset = Convert.ToString(dataOffset, 16).PadLeft(8, '0');
            byte[] bdataOffset = HexStringToByteArray(strdataOffset);
            list_head.AddRange(bheadinfo);
            list_head.AddRange(bnumelement);
            list_head.AddRange(bdatasize);
            list_head.AddRange(bdataOffset);
            list_head.AddRange(bnull);
            list_head.AddRange(bohter);
            return list_head.ToArray();
        }

        /// <summary>
        /// 写标签
        ///Int32 name;  //tag name  4
        ///Int32 number;  //tag number  4
        ///Int16 elementtype; //element type code 
        ///Int16 elementsize; //size in bytes of one element
        ///Int32 numelements; //number of elements in item
        ///Int32 datasize;  //size in bytes of item
        ///Int32 dataoffset; //item's data, or offset in file
        ///Int32 datahandle;  //reserved
        /// </summary>
        /// <param name="dv">要写入的值</param>
        /// <returns></returns>
        private byte[] Write_ABIF_Tag(DirEntryValue dv)
        {
            List<byte> list_tag = new List<byte>();
            string strname = dv.Name;
            int inumber = dv.Number;
            string strelementtype = dv.Elementtype;
            int ielementsize = dv.Elementsize;
            int inumelements = dv.Numelements;
            int idatasize = dv.Datasize;
            string strdataoffset = dv.Dataoffset;
            string strdatahandle = dv.Datahandle;

            byte[] bname = Encoding.ASCII.GetBytes(strname);//标签
            list_tag.AddRange(bname);
            string strnumber = Convert.ToString(inumber, 16).PadLeft(8, '0');
            byte[] bnumber = HexStringToByteArray(strnumber);//tagnum
            list_tag.AddRange(bnumber);
            strelementtype = Convert.ToString(switch_Types(strelementtype), 16).PadLeft(4, '0');
            byte[] belementtype = HexStringToByteArray(strelementtype);//类型
            list_tag.AddRange(belementtype);
            string strelementsize = Convert.ToString(ielementsize, 16).PadLeft(4, '0');
            byte[] belementsize = HexStringToByteArray(strelementsize);//一个元素占内存大小
            list_tag.AddRange(belementsize);
            string strnumelements = Convert.ToString(inumelements, 16).PadLeft(8, '0');
            byte[] bnumelements = HexStringToByteArray(strnumelements);//元素个数
            list_tag.AddRange(bnumelements);
            string strdatasize = Convert.ToString(idatasize, 16).PadLeft(8, '0');
            byte[] bdatasize = HexStringToByteArray(strdatasize);//元素总的字节大小
            list_tag.AddRange(bdatasize);
            if (dv.Dataoffset.Equals(""))//dv.Data字节数<=4
            {
                list_tag.AddRange(dv.Data);
            }
            else
            {
                string strDataoffset = Convert.ToString(int.Parse(dv.Dataoffset), 16).PadLeft(8, '0');
                byte[] bDataoffset = HexStringToByteArray(strDataoffset);//数据元素的便宜量
                list_tag.AddRange(bDataoffset);

            }
            string strdatahandleH = Convert.ToString(int.Parse(strdatahandle), 16).PadLeft(8, '0');
            byte[] bdatahandle = HexStringToByteArray(strdatahandleH);//元素总的字节大小
            list_tag.AddRange(bdatahandle);
            return list_tag.ToArray();
        }

        /// <summary>
        /// 写ABIF
        /// 注：同名tag用tag+num组成
        /// 必写项目： dv.Name；dv.Elementtype； dv.Dataoffset
        /// </summary>
        /// <param name="list_dv">要写入文本的集合</param>
        /// <returns></returns>
        public bool Write_ABIF(List<DirEntryValue> list_dv)
        {
            bool bMake = false;
            //try
            //{
            List<byte> list = new List<byte>();
            // List<byte> list_tag = new List<byte>();
            List<byte> list_value = new List<byte>();
            int elementnum = list_dv.Count;
            //写Head
            byte[] bHead = Write_ABIF_Head(elementnum, 0);

            int ihead_len = bHead.Length;//表头字节数
            int iTag_len = elementnum * 28;//标签所占的字节
            int iTotal_len = ihead_len;//计算偏移量
            //添加标签位置
            byte[] bspecTag = new byte[iTag_len];
            // list.AddRange(bspecTag);

            for (int i = 0; i < elementnum; i++)
            {
                DirEntryValue dv =  list_dv[i];

                //数据用，分割
                if (dv.Elementtype.Equals("short") || dv.Elementtype.Equals("long") || dv.Elementtype.Equals("float") || dv.Elementtype.Equals("double") || dv.Elementtype.Equals("date") || dv.Elementtype.Equals("time"))//Value
                {
                    string[] strValue = dv.Dataoffset.Split(new string[] { ",", "-", ":", "/" }, StringSplitOptions.RemoveEmptyEntries);
                    if (dv.Elementtype.Equals("short"))//2字节组
                    {
                        //byte[] bMid = new byte[4];
                        //Buffer.BlockCopy(dv.Data, 0, bMid, 2, dv.Data.Length);
                        //list_tag.AddRange(bMid);

                        dv.Elementsize = 2;
                        string[] ss = Array.ConvertAll<string, string>(strValue, s => Convert.ToString((int)Math.Round(double.Parse(s)), 16).ToUpper().PadLeft(4, '0'));
                        dv.Numelements = ss.Length;
                        if (ss.Length == 1)
                        {
                            string strM = ss[0] + "0000";
                            dv.Data = HexStringToByteArray(strM);
                        }
                        else
                        {
                            StringBuilder sb = new StringBuilder();
                            for (int j = 0; j < ss.Length; j++)
                            {
                                sb.Append(ss[j]);
                            }
                            dv.Data = HexStringToByteArray(sb.ToString());
                        }

                    }
                    else if (dv.Elementtype.Equals("date"))//日期
                    {
                        dv.Elementsize = 4;
                        dv.Numelements = 1;
                        string ss = Convert.ToString(int.Parse(strValue[0]), 16).ToUpper().PadLeft(4, '0');
                        ss += Convert.ToString(int.Parse(strValue[1]), 16).ToUpper().PadLeft(2, '0');
                        ss += Convert.ToString(int.Parse(strValue[2]), 16).ToUpper().PadLeft(2, '0');
                        dv.Data = HexStringToByteArray(ss);
                    }
                    else if (dv.Elementtype.Equals("time"))//时间
                    {
                        dv.Elementsize = 4;
                        dv.Numelements = 1;

                        StringBuilder sb = new StringBuilder();
                        string ss = "";
                        for (int j = 0; j < strValue.Length; j++)
                        {
                            if (j < 3)
                            {
                                ss = Convert.ToString((int)Math.Round(double.Parse(strValue[j])), 16).ToUpper().PadLeft(2, '0');
                            }
                            else
                            {
                                ss = Convert.ToString((int)Math.Round(double.Parse(strValue[j].Substring(0, 2))), 16).ToUpper().PadLeft(2, '0');
                            }
                            sb.Append(ss);
                        }


                        dv.Data = HexStringToByteArray(sb.ToString());
                    }
                    else if (dv.Elementtype.Equals("long"))//四字节
                    {
                        dv.Elementsize = 4;
                        string[] ss = Array.ConvertAll<string, string>(strValue, s => Convert.ToString((int)Math.Round(double.Parse(s)), 16).ToUpper().PadLeft(8, '0'));
                        dv.Numelements = ss.Length;
                        StringBuilder sb = new StringBuilder();
                        for (int j = 0; j < ss.Length; j++)
                        {
                            sb.Append(ss[j]);
                        }
                        dv.Data = HexStringToByteArray(sb.ToString());
                    }
                    else if (dv.Elementtype.Equals("float"))//四字节
                    {
                        dv.Elementsize = 4;
                        dv.Numelements = strValue.Length;
                        StringBuilder sb = new StringBuilder();
                        for (int j = 0; j < strValue.Length; j++)
                        {
                            float f = float.Parse(strValue[j]);
                            byte[] bbyte = BitConverter.GetBytes(f);
                            Array.Reverse(bbyte);
                            string ss = BitConverter.ToString(bbyte).Replace("-", "").PadLeft(8, '0');
                            sb.Append(ss);

                        }

                        dv.Data = HexStringToByteArray(sb.ToString());
                    }
                    else if (dv.Elementtype.Equals("double"))//8字节
                    {
                        dv.Elementsize = 8;
                        dv.Numelements = strValue.Length;
                        StringBuilder sb = new StringBuilder();
                        for (int j = 0; j < strValue.Length; j++)
                        {
                            double d = double.Parse(strValue[j]);
                            byte[] bbyte = BitConverter.GetBytes(d);
                            Array.Reverse(bbyte);
                            string ss = BitConverter.ToString(bbyte).Replace("-", "").PadLeft(16, '0');
                            sb.Append(ss);

                        }
                        Console.WriteLine(sb.ToString());
                        dv.Data = HexStringToByteArray(sb.ToString());
                    }
                }
                else if (dv.Elementtype.Equals("cString"))//refreace
                {
                    dv.Elementsize = 1;
                    string sssss = dv.Dataoffset + "\0";
                    string strDataV = (dv.Dataoffset + "\0").PadRight(4, '0');
                    dv.Data = Encoding.ASCII.GetBytes(strDataV);
                    dv.Numelements = sssss.Length;

                }
                else if (dv.Elementtype.Equals("pString"))//refreace
                {
                    dv.Elementsize = 1;
                    List<byte> listB = new List<byte>();
                    byte bDatanum = Convert.ToByte(dv.Dataoffset.Length);
                    byte[] bData = Encoding.ASCII.GetBytes(dv.Dataoffset);
                    listB.Add(bDatanum);
                    listB.AddRange(bData);
                    if (listB.Count < 4)
                    {
                        byte[] bAdd = new byte[4 - listB.Count];
                        listB.AddRange(bAdd);
                    }
                    dv.Data = listB.ToArray();
                    dv.Numelements = bData.Length + 1;// bData.Length+1

                }
                else if (dv.Elementtype.Equals("char"))//refreace
                {
                    if (dv.Name.Equals("PCON") || dv.Name.Equals("PBAS"))
                    {
                        string[] strValue = dv.Dataoffset.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        StringBuilder sb = new StringBuilder();
                        for (int j = 0; j < strValue.Length; j++)
                        {
                            //string soh = Convert.ToString((char)bascii[0]);
                            sb.Append(Convert.ToString((char)int.Parse(strValue[j])));
                        }

                        dv.Elementsize = 1;
                        dv.Numelements = sb.ToString().Length;
                        dv.Dataoffset = sb.ToString().PadRight(4, '0');
                        dv.Data = Encoding.ASCII.GetBytes(sb.ToString());
                    }
                    else
                    {
                        dv.Elementsize = 1;
                        dv.Numelements = dv.Dataoffset.Length;
                        dv.Dataoffset = dv.Dataoffset.PadRight(4, '0');
                        dv.Data = Encoding.ASCII.GetBytes(dv.Dataoffset);
                    }
                }
                else
                {
                    Console.WriteLine("UNknown");
                    break;
                }
                dv.Datasize = dv.Numelements * dv.Elementsize;
                if (dv.Datasize <= 4)//dataoffset is item's data// dv.Numelements
                {
                    dv.Dataoffset = "";
                    //替换目标元素
                    byte[] btag = Write_ABIF_Tag(dv);

                    Buffer.BlockCopy(btag, 0, bspecTag, btag.Length * i, btag.Length);

                    // ihead_len += btag.Length;
                    dv.Datasize = 0;
                }
                else if (dv.Datasize > 4)//dataoffset is offset 
                {
                    dv.Dataoffset = iTotal_len.ToString();
                    //替换目标元素
                    byte[] btag = Write_ABIF_Tag(dv);
                    Buffer.BlockCopy(btag, 0, bspecTag, btag.Length * i, btag.Length);

                    // ihead_len += btag.Length;
                    list_value.AddRange(dv.Data);
                }
                iTotal_len += dv.Datasize;
            }

            bHead = Write_ABIF_Head(elementnum, list_value.Count() + ihead_len);
            list.AddRange(bHead);
            list.AddRange(list_value.ToArray());
            list.AddRange(bspecTag);
            File.WriteAllBytes(path, list.ToArray());
            bMake = true;
            //}
            //catch (Exception e) { Console.WriteLine(e.ToString()); }
            return bMake;
        }

        /// <summary>
        /// 替换标签
        /// </summary> 
        /// <param name="tag_name">标签名称</param>
        /// <param name="tag_num">标签序号</param>
        /// <param name="tag_value">标签值</param>
        /// <returns></returns>
        public bool Replace_ABIF_Tag(string tag_name, int tag_num, string tag_value)
        {
            bool bmark = false;
            //try
            //{
            byte[] data = File.ReadAllBytes(path);
            byte[] btag = Encoding.ASCII.GetBytes(tag_name);
            byte[] bnum = HexStringToByteArray(Convert.ToString(tag_num, 16).PadLeft(8, '0'));
            byte[] mdata = btag.Concat(bnum).ToArray<byte>();
            int index = FindIndex(data, mdata);//查询标签及序号的索引
            if (index > -1)
            {
                int elementtype_index = index + 8;
                byte[] belementtype = new byte[2];
                Buffer.BlockCopy(data, elementtype_index, belementtype, 0, 2);
                int ielementtype = Convert.ToInt32(ByteArrayToHexString(belementtype, false), 16);
                string type = Seatch_TagType(ielementtype);//数据类型

                int elementsize_index = index + 10;//size in bytes of one element
                byte[] belementsize = new byte[2];
                Buffer.BlockCopy(data, elementsize_index, belementsize, 0, 2);
                int ielementsize = Convert.ToInt32(ByteArrayToHexString(belementsize, false), 16);//单个元素所占字节

                int numelements_index = index + 12;//number of elements in item
                int datasize_index = index + 16;//size in bytes of item
                byte[] bdatasize = new byte[4];
                Buffer.BlockCopy(data, datasize_index, bdatasize, 0, 4);
                int idatasize = Convert.ToInt32(ByteArrayToHexString(bdatasize, false), 16);//元素所占总字节

                int dataoffset_index = index + 20;//item's data, or offset in file

                int iNumelements = 0;//元素个数
                int iDatasize = 0;//元素所占总字节数
                int iDataoffset = 0;//数据偏移量
                                    //iDatasize=iNumelements*ielementsize
                byte[] bData = null;//现有数据
                #region  数据用处理

                if (type.Equals("short") || type.Equals("long") || type.Equals("float") || type.Equals("double") || type.Equals("date") || type.Equals("time"))//Value
                {
                    string[] strValue = tag_value.Split(new string[] { ",", "-", ":", "/" }, StringSplitOptions.RemoveEmptyEntries);//拆分数据
                    if (type.Equals("short"))//2字节组
                    {
                        string[] ss = Array.ConvertAll<string, string>(strValue, s => Convert.ToString((int)Math.Round(double.Parse(s)), 16).ToUpper().PadLeft(4, '0'));
                        iNumelements = ss.Length;
                        if (ss.Length == 1)
                        {
                            string strM = ss[0] + "0000";
                            bData = HexStringToByteArray(strM);
                        }
                        else
                        {
                            StringBuilder sb = new StringBuilder();
                            for (int j = 0; j < ss.Length; j++)
                            {
                                sb.Append(ss[j]);
                            }
                            bData = HexStringToByteArray(sb.ToString());
                        }

                    }
                    else if (type.Equals("date"))//日期
                    {
                        iNumelements = 1;
                        string ss = Convert.ToString(int.Parse(strValue[0]), 16).ToUpper().PadLeft(4, '0');
                        ss += Convert.ToString(int.Parse(strValue[1]), 16).ToUpper().PadLeft(2, '0');
                        ss += Convert.ToString(int.Parse(strValue[2]), 16).ToUpper().PadLeft(2, '0');
                        bData = HexStringToByteArray(ss);
                    }
                    else if (type.Equals("time"))//时间
                    {
                        iNumelements = 1;
                        StringBuilder sb = new StringBuilder();
                        string ss = "";
                        for (int j = 0; j < strValue.Length; j++)
                        {
                            if (j < 3)
                            {
                                ss = Convert.ToString((int)Math.Round(double.Parse(strValue[j])), 16).ToUpper().PadLeft(2, '0');
                            }
                            else
                            {
                                ss = Convert.ToString((int)Math.Round(double.Parse(strValue[j].Substring(0, 2))), 16).ToUpper().PadLeft(2, '0');
                            }
                            sb.Append(ss);
                        }


                        bData = HexStringToByteArray(sb.ToString());
                    }
                    else if (type.Equals("long"))//四字节
                    {
                        string[] ss = Array.ConvertAll<string, string>(strValue, s => Convert.ToString((int)Math.Round(double.Parse(s)), 16).ToUpper().PadLeft(8, '0'));
                        iNumelements = ss.Length;
                        StringBuilder sb = new StringBuilder();
                        for (int j = 0; j < ss.Length; j++)
                        {
                            sb.Append(ss[j]);
                        }
                        bData = HexStringToByteArray(sb.ToString());
                    }
                    else if (type.Equals("float"))//四字节
                    {
                        iNumelements = strValue.Length;
                        StringBuilder sb = new StringBuilder();
                        for (int j = 0; j < strValue.Length; j++)
                        {
                            float f = float.Parse(strValue[j]);
                            byte[] bbyte = BitConverter.GetBytes(f);
                            Array.Reverse(bbyte);
                            string ss = BitConverter.ToString(bbyte).Replace("-", "").PadLeft(8, '0');
                            sb.Append(ss);

                        }

                        bData = HexStringToByteArray(sb.ToString());
                    }
                    else if (type.Equals("double"))//8字节
                    {
                        iNumelements = strValue.Length;
                        StringBuilder sb = new StringBuilder();
                        for (int j = 0; j < strValue.Length; j++)
                        {
                            double d = double.Parse(strValue[j]);
                            byte[] bbyte = BitConverter.GetBytes(d);
                            Array.Reverse(bbyte);
                            string ss = BitConverter.ToString(bbyte).Replace("-", "").PadLeft(16, '0');
                            sb.Append(ss);

                        }
                        Console.WriteLine(sb.ToString());
                        bData = HexStringToByteArray(sb.ToString());
                    }
                }
                else if (type.Equals("cString"))//refreace
                {
                    string sssss = tag_value + "\0";
                    string strDataV = (tag_value + "\0").PadRight(4, '0');
                    bData = Encoding.ASCII.GetBytes(strDataV);
                    iNumelements = sssss.Length;

                }
                else if (type.Equals("pString"))//refreace
                {
                    List<byte> listB = new List<byte>();
                    byte bDatanum = Convert.ToByte(tag_value.Length);
                    byte[] bDataP = Encoding.ASCII.GetBytes(tag_value);
                    listB.Add(bDatanum);
                    listB.AddRange(bDataP);
                    if (listB.Count < 4)
                    {
                        byte[] bAdd = new byte[4 - listB.Count];
                        listB.AddRange(bAdd);
                    }
                    bData = listB.ToArray();
                    iNumelements = bData.Length;

                }
                else if (type.Equals("char"))//refreace
                {
                    iNumelements = tag_value.Length;
                    tag_value = tag_value.PadRight(4, '0');
                    bData = Encoding.ASCII.GetBytes(tag_value);
                }
                else
                {
                    Console.WriteLine("UNknown");
                    return bmark;
                }
                #endregion
                iDatasize = iNumelements * ielementsize;
                //标签的偏移量
                byte[] bHeaderdataoffset = new byte[4];
                Buffer.BlockCopy(data, 26, bHeaderdataoffset, 0, 4);
                int iHeaderdataoffset = Convert.ToInt32(ByteArrayToHexString(bHeaderdataoffset, false), 16);//新标签的偏移量

                byte[] bHeaderdataSize = new byte[4];
                Buffer.BlockCopy(data, 22, bHeaderdataSize, 0, 4);
                int iHeaderdataSize = Convert.ToInt32(ByteArrayToHexString(bHeaderdataSize, false), 16);//标签的总字节数
                if (bData.Length <= 4)//iDataoffset 为0 新数据
                {
                    if (idatasize <= 4)//原始数据小于4 直接替换  
                    {

                        //元素个数
                        string HexNumelements = Convert.ToString(iNumelements, 16).PadLeft(8, '0');
                        byte[] bNumelements = HexStringToByteArray(HexNumelements);
                        Buffer.BlockCopy(bNumelements, 0, data, numelements_index, bNumelements.Length);

                        //元素总字节大小
                        string Hexdatasize = Convert.ToString(iDatasize, 16).PadLeft(8, '0');
                        byte[] bDatasize = HexStringToByteArray(Hexdatasize);
                        Buffer.BlockCopy(bDatasize, 0, data, datasize_index, bDatasize.Length);


                        Buffer.BlockCopy(bData, 0, data, dataoffset_index, bData.Length);
                    }
                    else//原始数据大于4 需要清空所占位置的偏移量 和数据空间赋值为0后再写入新数据
                    {//
                     //元素个数
                        string HexNumelements = Convert.ToString(iNumelements, 16).PadLeft(8, '0');
                        byte[] bNumelements = HexStringToByteArray(HexNumelements);
                        Buffer.BlockCopy(bNumelements, 0, data, numelements_index, bNumelements.Length);

                        //元素总字节大小
                        string Hexdatasize = Convert.ToString(iDatasize, 16).PadLeft(8, '0');
                        byte[] bDatasize = HexStringToByteArray(Hexdatasize);
                        Buffer.BlockCopy(bDatasize, 0, data, datasize_index, bDatasize.Length);

                        //数据
                        byte[] bdataoffset = new byte[4];
                        Buffer.BlockCopy(data, dataoffset_index, bdataoffset, 0, 4);
                        int idataoffset = Convert.ToInt32(ByteArrayToHexString(bdataoffset, false), 16);//标签元素数据的偏移量
                                                                                                        // Buffer.BlockCopy(data, dataoffset_index, bData, 0, bData.Length);
                        Buffer.BlockCopy(bData, 0, data, dataoffset_index, bData.Length);

                        Clear(ref data, idataoffset, idatasize);

                    }
                }
                else  //iDataoffset 不为0  根据偏移量和数据大小查询原数据所占位置 
                {
                    if (idatasize <= 4)//原始数据小于4  需重新开辟空间存放数据
                    {
                        //先获取标签的偏移量，在标签位置之前添加插入的数据，添加完成后更新标签偏移量
                        //byte[] bHeaderdataoffset = new byte[4];
                        //Buffer.BlockCopy(data, 26, bHeaderdataoffset, 0, 4);
                        //int iHeaderdataoffset = Convert.ToInt32(ByteArrayToHexString(bHeaderdataoffset, false), 16);//新标签的偏移量

                        //元素个数
                        string HexNumelements = Convert.ToString(iNumelements, 16).PadLeft(8, '0');
                        byte[] bNumelements = HexStringToByteArray(HexNumelements);
                        Buffer.BlockCopy(bNumelements, 0, data, numelements_index, bNumelements.Length);

                        //元素总字节大小
                        string Hexdatasize = Convert.ToString(iDatasize, 16).PadLeft(8, '0');
                        byte[] bDatasize = HexStringToByteArray(Hexdatasize);
                        Buffer.BlockCopy(bDatasize, 0, data, datasize_index, bDatasize.Length);

                        //偏移量大小
                        string Hexdataoffset = Convert.ToString(iHeaderdataoffset, 16).PadLeft(8, '0');
                        byte[] bdataoffset = HexStringToByteArray(Hexdataoffset);
                        Buffer.BlockCopy(bdataoffset, 0, data, dataoffset_index, bdataoffset.Length);


                        Insert(ref data, iHeaderdataoffset, bData);

                    }
                    else if (bData.Length <= idatasize)
                    {
                        byte[] bdataoffset = new byte[4];
                        Buffer.BlockCopy(data, dataoffset_index, bdataoffset, 0, 4);
                        int idataoffset = Convert.ToInt32(ByteArrayToHexString(bdataoffset, false), 16);//标签元素数据的偏移量

                        //元素个数
                        string HexNumelements = Convert.ToString(iNumelements, 16).PadLeft(8, '0');
                        byte[] bNumelements = HexStringToByteArray(HexNumelements);
                        Buffer.BlockCopy(bNumelements, 0, data, numelements_index, bNumelements.Length);

                        //元素总字节大小
                        string Hexdatasize = Convert.ToString(iDatasize, 16).PadLeft(8, '0');
                        byte[] bDatasize = HexStringToByteArray(Hexdatasize);
                        Buffer.BlockCopy(bDatasize, 0, data, datasize_index, bDatasize.Length);

                        ////添加新的数据偏移量到标签中
                        //string HexnewAddoffset = Convert.ToString(idataoffset, 16).PadLeft(8, '0');
                        //byte[] bnewAddoffset = HexStringToByteArray(HexnewAddoffset);
                        //Buffer.BlockCopy(bnewAddoffset, 0, data, dataoffset_index, 4);
                        //清空老的数据
                        Clear(ref data, idataoffset, idatasize);//直接删除 后面添加数据 起长度等于标签位置-删除数据的长度
                                                                //在新的数据偏移量位置添加数据
                        Buffer.BlockCopy(bData, 0, data, idataoffset, bData.Length);
                        // Insert(ref data, inewAddoffset, bData);
                    }
                    else//原始数据大于4 需要清空所占位置的偏移量 和数据所占字节空间再写入数据
                    {

                        byte[] bdataoffset = new byte[4];
                        Buffer.BlockCopy(data, dataoffset_index, bdataoffset, 0, 4);
                        int idataoffset = Convert.ToInt32(ByteArrayToHexString(bdataoffset, false), 16);//标签元素数据的偏移量

                        int inewAddoffset = iHeaderdataoffset;//最后数据位置添加新数据

                        //元素个数
                        string HexNumelements = Convert.ToString(iNumelements, 16).PadLeft(8, '0');
                        byte[] bNumelements = HexStringToByteArray(HexNumelements);
                        Buffer.BlockCopy(bNumelements, 0, data, numelements_index, bNumelements.Length);

                        //元素总字节大小
                        string Hexdatasize = Convert.ToString(iDatasize, 16).PadLeft(8, '0');
                        byte[] bDatasize = HexStringToByteArray(Hexdatasize);
                        Buffer.BlockCopy(bDatasize, 0, data, datasize_index, bDatasize.Length);

                        //添加新的数据偏移量到标签中
                        string HexnewAddoffset = Convert.ToString(inewAddoffset, 16).PadLeft(8, '0');
                        byte[] bnewAddoffset = HexStringToByteArray(HexnewAddoffset);
                        Buffer.BlockCopy(bnewAddoffset, 0, data, dataoffset_index, 4);
                        //清空老的数据
                        Clear(ref data, idataoffset, idatasize);//直接删除 后面添加数据 起长度等于标签位置-删除数据的长度
                                                                //在新的数据偏移量位置添加数据
                        Insert(ref data, inewAddoffset, bData);
                    }
                }
                //写数据结束后重写标头
                iDataoffset = data.Length - iHeaderdataSize;
                string HexDataoffset = Convert.ToString(iDataoffset, 16).PadLeft(8, '0');
                byte[] bDataoffset = HexStringToByteArray(HexDataoffset);
                Buffer.BlockCopy(bDataoffset, 0, data, 26, 4);
                File.WriteAllBytes(path, data);
            }

            bmark = true;
            //}
            //catch (Exception ex) { Console.WriteLine(ex.ToString()); }
            return bmark;
        }

        /// <summary>
        /// 返回标签的数据类型
        /// </summary>
        /// <param name="tagname">标签名</param>
        /// <param name="tagnum">标签序号</param>
        /// <returns></returns>
        public string Tag_ABIF_Type(string tagname, int tagnum)
        {
            string tagtype = "";
            if (!tagname.Equals("") && !tagnum.Equals("") && tagname.Length == 4 && File.Exists(path))
            {
                byte[] data = File.ReadAllBytes(path);
                byte[] btagname = Encoding.ASCII.GetBytes(tagname);
                byte[] btagnum = HexStringToByteArray(Convert.ToString(tagnum, 16).PadLeft(8, '0'));
                byte[] btagnamenum = btagname.Concat(btagnum).ToArray();
                int index = FindIndex(data, btagnamenum);//查询标签及序号的索引
                if (index > -1)
                {
                    int elementtype_index = index + 8;
                    byte[] belementtype = new byte[2];
                    Buffer.BlockCopy(data, elementtype_index, belementtype, 0, 2);
                    int ielementtype = Convert.ToInt32(ByteArrayToHexString(belementtype, false), 16);
                    tagtype = Seatch_TagType(ielementtype);//数据类型

                }
            }
            return tagtype;
        }

        /// <summary>
        /// 读取ABIF标签的值
        /// </summary>
        /// <param name="tagname">标签名称</param>
        /// <param name="tagnum">标签序号</param>
        /// <returns>当返回为数组为逗号分隔；当传递参数错误或者查询不到标签时，返回为null</returns>
        public string Tag_ABIF_Value(string tagname, int tagnum)
        {
            string result = "";
            if (!tagname.Equals("") && !tagnum.Equals("") && tagname.Length == 4 && File.Exists(path))
            {
                byte[] data = File.ReadAllBytes(path);
                byte[] btagname = Encoding.ASCII.GetBytes(tagname);
                byte[] btagnum = HexStringToByteArray(Convert.ToString(tagnum, 16).PadLeft(8, '0'));
                byte[] btagnamenum = btagname.Concat(btagnum).ToArray();
                int index = FindIndex(data, btagnamenum);//查询标签及序号的索引
                if (index > -1)
                {
                    int elementtype_index = index + 8;
                    byte[] belementtype = new byte[2];
                    Buffer.BlockCopy(data, elementtype_index, belementtype, 0, 2);
                    int ielementtype = Convert.ToInt32(ByteArrayToHexString(belementtype, false), 16);
                    string type = Seatch_TagType(ielementtype);//数据类型

                    int datasize_index = index + 16;//size in bytes of item
                    byte[] bdatasize = new byte[4];
                    Buffer.BlockCopy(data, datasize_index, bdatasize, 0, 4);
                    int idatasize = Convert.ToInt32(ByteArrayToHexString(bdatasize, false), 16);//元素所占总字节

                    int dataoffset_index = index + 20;//item's data, or offset in file
                    if (idatasize >= 0 && idatasize <= 4)
                    {
                        byte[] bValue = new byte[idatasize];
                        Buffer.BlockCopy(data, dataoffset_index, bValue, 0, bValue.Length);
                        #region  数据用处理

                        if (type.Equals("short") || type.Equals("long") || type.Equals("float") || type.Equals("double") || type.Equals("date") || type.Equals("time"))//Value
                        {
                            if (type.Equals("short"))//2字节组
                            {
                                List<byte[]> list = SplitArray(bValue, 2);
                                for (int i = 0; i < list.Count; i++)
                                {
                                    if (i < list.Count - 1)
                                        result += Convert.ToInt16(ByteArrayToHexString(list[i], false), 16) + ",";
                                    else
                                        result += Convert.ToInt16(ByteArrayToHexString(list[i], false), 16);
                                }

                            }
                            else if (type.Equals("date"))//日期
                            {
                                List<byte[]> list = SplitArray(bValue, 2);
                                string strYear = Convert.ToInt16(ByteArrayToHexString(list[0], false), 16).ToString();
                                string strMonth = Convert.ToInt16(list[1][0]).ToString();
                                string strDay = Convert.ToInt16(list[1][1]).ToString();

                                result = strYear + "-" + strMonth + "-" + strDay;
                            }
                            else if (type.Equals("time"))//时间
                            {
                                string strHour = Convert.ToInt16(bValue[0]).ToString();
                                string strMin = Convert.ToInt16(bValue[1]).ToString();
                                string strSec = Convert.ToInt16(bValue[2]).ToString();
                                string strMSec = Convert.ToInt16(bValue[3]).ToString();
                                result = strHour + ":" + strMin + ":" + strSec + ":" + strMSec;
                            }
                            else if (type.Equals("long"))//四字节
                            {
                                result = Convert.ToInt64(ByteArrayToHexString(bValue, false), 16).ToString();
                            }
                            else if (type.Equals("float"))//四字节
                            {
                                result = BitConverter.ToSingle(bValue, 0).ToString();
                            }
                            else if (type.Equals("double"))//8字节
                            {
                                //不会出现这种状况
                            }
                        }
                        else if (type.Equals("cString"))//refreace
                        {
                            result = Encoding.ASCII.GetString(bValue).Replace("\0", "");
                        }
                        else if (type.Equals("pString"))//refreace
                        {
                            List<byte> listB = bValue.ToList();
                            listB.RemoveAt(0);
                            result = Encoding.ASCII.GetString(listB.ToArray());
                        }
                        else if (type.Equals("char"))//refreace
                        {
                            result = Encoding.ASCII.GetString(bValue);
                        }
                        else
                        {
                            //其他待扩展
                        }
                        #endregion
                    }
                    else
                    {
                        byte[] boffset = new byte[4];
                        Buffer.BlockCopy(data, dataoffset_index, boffset, 0, boffset.Length);
                        Int32 dataindex = Convert.ToInt32(ByteArrayToHexString(boffset, false), 16);//data\

                        byte[] bValue = new byte[idatasize];
                        Buffer.BlockCopy(data, dataindex, bValue, 0, idatasize);

                        #region  数据用处理

                        if (type.Equals("short") || type.Equals("long") || type.Equals("float") || type.Equals("double") || type.Equals("date") || type.Equals("time"))//Value
                        {
                            if (type.Equals("short"))//2字节组
                            {
                                List<byte[]> list = SplitArray(bValue, 2);
                                for (int i = 0; i < list.Count; i++)
                                {
                                    if (i < list.Count - 1)
                                        result += Convert.ToInt16(ByteArrayToHexString(list[i], false), 16) + ",";
                                    else
                                        result += Convert.ToInt16(ByteArrayToHexString(list[i], false), 16);
                                }
                            }
                            else if (type.Equals("date"))//日期
                            {
                                List<byte[]> list = SplitArray(bValue, 2);
                                string strYear = Convert.ToInt16(ByteArrayToHexString(list[0], false), 16).ToString();
                                string strMonth = Convert.ToInt16(list[1][0]).ToString();
                                string strDay = Convert.ToInt16(list[1][1]).ToString();

                                result = strYear + "-" + strMonth + "-" + strDay;
                            }
                            else if (type.Equals("time"))//时间
                            {
                                string strHour = Convert.ToInt16(bValue[0]).ToString();
                                string strMin = Convert.ToInt16(bValue[1]).ToString();
                                string strSec = Convert.ToInt16(bValue[2]).ToString();
                                string strMSec = Convert.ToInt16(bValue[3]).ToString();
                                result = strHour + ":" + strMin + ":" + strSec + ":" + strMSec;
                            }
                            else if (type.Equals("long"))//四字节
                            {
                                List<byte[]> list = SplitArray(bValue, 4);
                                for (int i = 0; i < list.Count; i++)
                                {
                                    if (i < list.Count - 1)
                                        result += Convert.ToInt64(ByteArrayToHexString(list[i], false), 16).ToString() + ",";
                                    else
                                        result += Convert.ToInt64(ByteArrayToHexString(list[i], false), 16).ToString();
                                }
                            }
                            else if (type.Equals("float"))//四字节
                            {
                                List<byte[]> list = SplitArray(bValue, 4);
                                for (int i = 0; i < list.Count; i++)
                                {
                                    if (i < list.Count - 1)
                                        result += BitConverter.ToSingle(list[i], 0).ToString() + ",";
                                    else
                                        result += BitConverter.ToSingle(list[i], 0).ToString();
                                }

                            }
                            else if (type.Equals("double"))//8字节
                            {
                                List<byte[]> list = SplitArray(bValue, 8);
                                for (int i = 0; i < list.Count; i++)
                                {
                                    if (i < list.Count - 1)
                                        result += BitConverter.ToSingle(list[i], 0).ToString() + ",";
                                    else
                                        result += BitConverter.ToSingle(list[i], 0).ToString();
                                }

                            }
                        }
                        else if (type.Equals("cString"))//refreace
                        {
                            result = Encoding.ASCII.GetString(bValue).Replace("\0", "");
                        }
                        else if (type.Equals("pString"))//refreace
                        {
                            List<byte> listB = bValue.ToList();
                            listB.RemoveAt(0);
                            result = Encoding.ASCII.GetString(listB.ToArray());
                        }
                        else if (type.Equals("char"))//refreace
                        {
                            result = Encoding.ASCII.GetString(bValue);
                        }
                        else
                        {
                            //其他待扩展
                        }
                        #endregion
                    }
                }
                else
                {
                    result = null;
                }
            }
            else
            {
                result = null;
            }
            return result;
        }

        /// <summary>
        /// 查询标签是否存在
        /// </summary>
        /// <param name="tagname">标签名称</param>
        /// <returns></returns>
        public bool Search_Tag(string tagname)
        {
            bool bmake = false;
            byte[] data = File.ReadAllBytes(path);
            byte[] bname = Encoding.ASCII.GetBytes(tagname);
            if (FindIndex(data, bname) > -1)
            {
                bmake = true;
            }
            return bmake;
        }

        /// <summary>
        /// 标签总个数
        /// </summary>
        /// <returns></returns>
        public Int32 Num_Dir_Entries()
        {
            byte[] data = File.ReadAllBytes(path);
            byte[] bver = new byte[4];
            Buffer.BlockCopy(data, 18, bver, 0, 4);
            return Convert.ToInt32(ByteArrayToHexString(bver, false), 16);
        }
        /// <summary>
        /// 文件版本号
        /// </summary>
        /// <returns></returns>
        public string File_Version()
        {
            string version = null;
            byte[] data = File.ReadAllBytes(path);
            byte[] bver = new byte[2];
            Buffer.BlockCopy(data, 4, bver, 0, 2);
            float iver = Convert.ToInt16(ByteArrayToHexString(bver, false), 16);
            version = (iver / 100).ToString();
            return version;
        }

        /// <summary>
        /// 判断文件是否为ABIF文件
        /// </summary>
        /// <returns></returns>
        public bool is_ABIF_Format()
        {
            bool bmake = false;
            byte[] data = File.ReadAllBytes(path);
            byte[] bHead = new byte[4];
            Buffer.BlockCopy(data, 0, bHead, 0, 4);
            string ss = Encoding.ASCII.GetString(bHead);
            if (ss.Equals("ABIF"))
            {
                bmake = true;
            }
            return bmake;
        }
       
        /// <summary>
        /// 当前程序版本号
        /// </summary>
        /// <returns></returns>
        public static string CURRENT_VERSION()
        {
            return VERSION;
        }
        #region   辅助函数
        /// <summary>
        /// 数组中插入元素
        /// </summary>
        /// <param name="arr">被插入的数组</param>
        /// <param name="index">开始的索引</param>
        /// <param name="value">要插入的数组</param>
        private void Insert(ref byte[] arr, int index, byte[] value)
        {
            List<byte> list = arr.ToList();//转换为List
            for (int i = 0; i < value.Length; i++)
            {
                list.Insert(index + i, value[i]);//插入
            }
            arr = list.ToArray();//转换为数组
        }
        /// <summary>
        ///  删除数组元素既相应位置元素赋0
        /// </summary>
        /// <param name="arr">要删除的数组</param>
        /// <param name="index">开始索引</param>
        /// <param name="datasize">长度</param>
        private void Clear(ref byte[] arr, int index, int datasize)
        {
            List<byte> list = arr.ToList();//转换为List
            for (int i = 0; i < datasize; i++)
            {
                //list.RemoveAt(index);
                list[index + i] = 0;
            }
            arr = list.ToArray();//转换为数组
        }
        /// <summary>
        /// 转换数据类型
        /// </summary>
        /// <param name="ielementtype"></param>
        /// <returns></returns>
        private string Seatch_TagType(int ielementtype)
        {
            string type = "";
            switch (ielementtype)
            {
                case 1:
                    type = "byte";
                    break;
                case 2:
                    type = "char";
                    break;
                case 3:
                    type = "word";
                    break;
                case 4:
                    type = "short";
                    break;
                case 5:
                    type = "long";
                    break;
                case 7:
                    type = "float";
                    break;
                case 8:
                    type = "double";
                    break;
                case 10:
                    type = "date";
                    break;
                case 11:
                    type = "time";
                    break;
                case 18:
                    type = "pString";
                    break;
                case 19:
                    type = "cString";
                    break;
                case 12:
                    type = "thumb";
                    break;
                case 13:
                    type = "bool";
                    break;
                case 6:
                    type = "rational";
                    break;
                case 9:
                    type = "BCD";
                    break;
                case 14:
                    type = "point";
                    break;
                case 15:
                    type = "rect";
                    break;
                case 16:
                    type = "vPoint";
                    break;
                case 17:
                    type = "vRect";
                    break;
                case 20:
                    type = "tag";
                    break;
                case 128:
                    type = "deltaComp";
                    break;
                case 256:
                    type = "LZWComp";
                    break;
                case 384:
                    type = "deltaLZW";
                    break;
            }


            return type;
        }
        /// <summary>
        /// 转换类型到int
        /// </summary>
        /// <param name="type">数据类型</param>
        /// <returns>对应数字</returns>
        private int switch_Types(string type)
        {
            int iT = -1;
            switch (type)
            {
                case "byte":
                    iT = 1;
                    break;
                case "char":
                    iT = 2;
                    break;
                case "word":
                    iT = 3;
                    break;
                case "short":
                    iT = 4;
                    break;
                case "long":
                    iT = 5;
                    break;
                case "float":
                    iT = 7;
                    break;
                case "double":
                    iT = 8;
                    break;
                case "date":
                    iT = 10;
                    break;
                case "time":
                    iT = 11;
                    break;
                case "pString":
                    iT = 18;
                    break;
                case "cString":
                    iT = 19;
                    break;
                case "thumb":
                    iT = 12;
                    break;
                case "bool":
                    iT = 13;
                    break;
                case "rational":
                    iT = 6;
                    break;
                case "BCD":
                    iT = 9;
                    break;
                case "point":
                    iT = 14;
                    break;
                case "rect":
                    iT = 15;
                    break;
                case "vPoint":
                    iT = 16;
                    break;
                case "vRect":
                    iT = 17;
                    break;
                case "tag":
                    iT = 20;
                    break;
                case "deltaComp":
                    iT = 128;
                    break;
                case "LZWComp":
                    iT = 256;
                    break;
                case "deltaLZW":
                    iT = 384;
                    break;
            }


            return iT;
        }
        // / <summary>
        // / 查找一个byte数组在另一个byte数组第一次出现位置
        // / </summary>
        // / <param name="array">被查找的数组</param>
        // / <param name="array2">要查找的数 组</param>
        // / <returns>找到返回索引，找不到返回-1</returns>
        static int FindIndex(byte[] array, byte[] array2)
        {
            int i, j;
            for (i = 0; i < array.Length; i++)
            {
                if (i + array2.Length <= array.Length)
                {
                    for (j = 0; j < array2.Length; j++)
                    {
                        if (array[i + j] != array2[j])
                            break;
                    }

                    if (j == array2.Length)
                        return i;
                }
                else
                    break;
            }

            return -1;
        }
        /// <summary>
        /// 16进制字符串转字节数组
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public byte[] HexStringToByteArray(string s)
        {
            s = s.Replace(" ", "");
            byte[] buffer = new byte[s.Length / 2];
            for (int i = 0; i < s.Length; i += 2)
                buffer[i / 2] = (byte)Convert.ToByte(s.Substring(i, 2), 16);
            return buffer;
        }
        /// <summary>
        /// 字节数组转16进制字符串
        /// </summary>
        /// <param name="data"></param>
        /// <param name="bFormat">true 空 false 无</param>
        /// <returns></returns>
        public string ByteArrayToHexString(byte[] data, bool bFormat)
        {
            StringBuilder sb = new StringBuilder(data.Length * 3);
            foreach (byte b in data)
                sb.Append(Convert.ToString(b, 16).PadLeft(2, '0').PadRight(3, ' '));
            string strResult = sb.ToString().Trim().ToUpper();
            if (!bFormat) strResult = strResult.Replace(" ", "");
            return strResult;
        }
        /// <summary>
        ///拆分任意数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">数组</param>
        /// <param name="size">每个数组元素的个数</param>
        /// <returns></returns>
        public List<T[]> SplitArray<T>(T[] data, int size)
        {
            List<T[]> list = new List<T[]>();
            for (int i = 0; i < data.Length / size; i++)
            {
                T[] r = new T[size];
                Array.Copy(data, i * size, r, 0, size);
                list.Add(r);
            }
            if (data.Length % size != 0)
            {
                T[] r = new T[data.Length % size];
                Array.Copy(data, data.Length - data.Length % size, r, 0, data.Length % size);
                list.Add(r);
            }
            return list;
        }
        #endregion
    }
}
