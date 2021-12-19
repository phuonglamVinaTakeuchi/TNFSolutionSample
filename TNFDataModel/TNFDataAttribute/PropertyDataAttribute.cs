using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TNFDataModel.TNFDataAttribute
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PropertyDataAttribute : TNFAttributeBase
    {
        /// <summary>
        /// Index của property can doc du lieu
        /// </summary>
        public int[] PropertyIndexes { get; }
        /// <summary>
        /// Index dong can doc cho Property
        /// </summary>
        public int PropertyLineIndex { get; }
        /// <summary>
        /// Index bat dau trong chuoi cua property can doc du lieu
        /// </summary>
        public int StartAt { get; }
        /// <summary>
        /// Index ket thuc cua property can doc du lieu
        /// </summary>
        public int EndAt {  get; }
        /// <summary>
        /// Property co phai là nested property khong
        /// </summary>
        public bool IsNestedProperties { get; private set; }
        /// <summary>
        /// Doc theo vong lap bat dau tu start at cho de ket thuc cua chuoi truyen vao
        /// </summary>
        public int LoopCount { get; }
        /// <summary>
        /// Co doc thuoc tinh doi tuong bang cach lap ko
        /// </summary>
        public bool ReadByLoop { get; }

        public PropertyDataAttribute(string propertyName, Type typeOf,int[] propertyIndexs): base(propertyName,typeOf)
        {

            PropertyIndexes = propertyIndexs;
            IsNestedProperties = true;
        }
        public PropertyDataAttribute(string propertyName,Type typeOf, int propertyIndex) : base(propertyName,typeOf)
        {
            PropertyIndexes = new int[] { propertyIndex };
            IsNestedProperties = false;
        }
        //public PropertyDataAttribute(string propertyName,Type typeOf, int startAt,int endAt) : this(propertyName,typeOf)
        //{
        //    PropertyIndexes = new int[] { startAt,endAt };
        //    StartAt = startAt;
        //    EndAt = endAt;
        //    IsNestedProperties = true;
        //}
        //public PropertyDataAttribute(string propertyName,Type typeOf, int startAt, int loopCount,bool readByLoop = true) : this(propertyName,typeOf)
        //{
        //    PropertyIndexes = new int[] { startAt };
        //    StartAt = startAt;
        //    LoopCount = loopCount;
        //    IsNestedProperties = true;
        //    ReadByLoop = readByLoop;
        //}

        public override string[] GetContentFromSource(string[] source)
        {
            return this.GetContentFromSource(source,this.PropertyIndexes);
        }
    }
}
