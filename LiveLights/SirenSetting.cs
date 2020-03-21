using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Drawing;
using System.Collections;

namespace LiveLights
{
    [XmlRoot(ElementName = "CVehicleModelInfoVarGlobal", Namespace = "")]
    public class CarcolsFile
    {
        [XmlArray("Sirens")]
        [XmlArrayItem("Item")]
        public List<SirenSetting> SirenSettings { get; set; } = new List<SirenSetting>();
    }

    // [XmlType(TypeName="Item")]
    public class SirenSetting : IList<SirenEntry>
    {
        [XmlArray("sirens")]
        [XmlArrayItem("Item")]
        public SirenEntry[] Sirens
        {
            get => sirenList.ToArray();
            set
            {
                sirenList = value.ToList();
                for (int i = 1; i <= sirenList.Count; i++)
                {
                    sirenList[i].SirenIdCommentText = "Siren " + i;
                }
            }
        }
        // public List<SirenEntry> Sirens = new List<SirenEntry>();
        // new SirenEntry[20]
        // public SirenEntry[] Sirens = Enumerable.Repeat<SirenEntry>(new SirenEntry(), 20).ToArray();
        private List<SirenEntry> sirenList = new List<SirenEntry>();

        public int Count => sirenList.Count;

        public bool IsReadOnly => false;

        public SirenEntry this[int index] 
        { 
            get => sirenList[index]; 
            set => sirenList[index] = value; 
        }

        public void Add(SirenEntry item)
        {
            if(sirenList.Count < 20)
            {
                item.SirenIdCommentText = "Siren " + (sirenList.Count + 1);
                sirenList.Add(item);
            } else
            {
                throw new IndexOutOfRangeException("A SirenSetting cannot contain more than 20 sirens");
            }
        }

        public void Clear() => sirenList.Clear();

        public bool Contains(SirenEntry item) => sirenList.Contains(item);

        public void CopyTo(SirenEntry[] array, int arrayIndex) => sirenList.CopyTo(array, arrayIndex);

        public IEnumerator<SirenEntry> GetEnumerator() => sirenList.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => sirenList.GetEnumerator();

        public bool Remove(SirenEntry item) => sirenList.Remove(item);

        public int IndexOf(SirenEntry item) => sirenList.IndexOf(item);

        public void Insert(int index, SirenEntry item) => sirenList.Insert(index, item);

        public void RemoveAt(int index) => sirenList.RemoveAt(index);


        // public SirenEntry[] Sirens { get; set; }
    }

    // [XmlType(TypeName="Item")]
    public class SirenEntry
    {
        [XmlIgnore]
        internal string SirenIdCommentText { get; set; }
        
        [XmlAnyElement("SirenIdComment")]
        public XmlComment SirenIdComment
        {
            get => new XmlDocument().CreateComment($" {SirenIdCommentText} ");
            set => SirenIdCommentText = value.InnerText;
        }

        [XmlIgnore]
        public Color LightColor { get; set; } = Color.White;

        [XmlElement("color")]
        public ValueItem<string> ColorString
        {
            get => string.Format("0x{0:X8}", LightColor.ToArgb());
            set
            {
                LightColor = Color.FromArgb(int.Parse(value, System.Globalization.NumberStyles.HexNumber));
            }
        }

        [XmlElement("intensity")]
        public ValueItem<float> Intensity { get; set; } = 0;

        [XmlElement("lightGroup")]
        public ValueItem<byte> LightGroup { get; set; } = 0;

        [XmlElement("rotate")]
        public ValueItem<bool> Rotate { get; set; } = false;

        [XmlElement("scale")]
        public ValueItem<bool> Scale { get; set; } = true;

        [XmlElement("scaleFactor")]
        public ValueItem<byte> ScaleFactor { get; set; } = 2;

        [XmlElement("flash")]
        public ValueItem<bool> Flash { get; set; } = false;

        [XmlElement("light")]
        public ValueItem<bool> Light { get; set; } = true;

        [XmlElement("spotLight")]
        public ValueItem<bool> SpotLight { get; set; } = true;

        [XmlElement("castShadows")]
        public ValueItem<bool> CastShadows { get; set; } = false;
    }

    public class ValueItem<T>
    {
        public static implicit operator ValueItem<T>(T value) => new ValueItem<T>(value);
        public static implicit operator T(ValueItem<T> item) => item.Value;

        public ValueItem() { }

        public ValueItem(T value) 
        {
            this.Value = value;
        }

        [XmlAttribute("value")]
        public T Value { get; set; }
    }
}
