using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABIF_Test
{
   
    public class DirEntryValue
    {
        private string _name;  //tag name
        private Int32 _number;  //tag number
        private string _elementtype; //element type code
        private Int16 _elementsize; //size in bytes of one element
        private Int32 _numelements; //number of elements in item
        private Int32 _datasize;  //size in bytes of item
        private string _dataoffset; //item's data, or offset in file
        private string _datahandle;  //reserved
        private byte[] _data;//写入的data数据

        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                _name = value;
            }
        }

        public int Number
        {
            get
            {
                return _number;
            }

            set
            {
                _number = value;
            }
        }

        public string Elementtype
        {
            get
            {
                return _elementtype;
            }

            set
            {
                _elementtype = value;
            }
        }

        public short Elementsize
        {
            get
            {
                return _elementsize;
            }

            set
            {
                _elementsize = value;
            }
        }

        public int Numelements
        {
            get
            {
                return _numelements;
            }

            set
            {
                _numelements = value;
            }
        }

        public int Datasize
        {
            get
            {
                return _datasize;
            }

            set
            {
                _datasize = value;
            }
        }

        public string Dataoffset
        {
            get
            {
                return _dataoffset;
            }

            set
            {
                _dataoffset = value;
            }
        }

        public string Datahandle
        {
            get
            {
                return _datahandle;
            }

            set
            {
                _datahandle = value;
            }
        }

        public byte[] Data
        {
            get
            {
                return _data;
            }

            set
            {
                _data = value;
            }
        }
    }
}
