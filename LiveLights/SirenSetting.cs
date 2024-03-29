﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Drawing;
using System.Collections;
using System.Reflection;
using System.Diagnostics;
using System.ComponentModel;
using Rage;

namespace LiveLights
{
    using Utils;

    [XmlRoot(ElementName = "CVehicleModelInfoVarGlobal", Namespace = "")]
    public class CarcolsFile
    {
        [XmlAnyElement("infocomment")]
        public XmlComment ExportInfoComment
        {
            get => new XmlDocument().CreateComment($"  Exported by {Game.LocalPlayer.Name} on " + DateTime.Now.ToString("yyyy-MM-dd @ HH:mm") + $" using LiveLights {EntryPoint.CurrentFileVersion} by PNWParksFan  ");
            set { }
        }

        [XmlAnyElement("linkcomment")]
        public XmlComment LinkComment
        {
            get => new XmlDocument().CreateComment("  LiveLights is open source! Download or learn more at https://github.com/pnwparksfan/rph-live-lights/  ");
            set { }
        }

        // [XmlIgnore]
        [XmlArray("Sirens")]
        [XmlArrayItem("Item")]
        public List<SirenSetting> SirenSettings { get; set; } = new List<SirenSetting>();

    }

    [XmlInclude(typeof(XmlComment))]
    [XmlInclude(typeof(SirenEntry))]
    public class SirenSetting 
    {
        [XmlElement("id")]
        public ValueItem<uint> ID { get; set; } = 0;

        [XmlElement("name")]
        public string Name { get; set; } = "";

        [XmlElement("timeMultiplier")]
        public ValueItem<float> TimeMultiplier { get; set; } = 1.0f;

        [XmlElement("lightFalloffMax")]
        public ValueItem<float> LightFalloffMax { get; set; } = 50f;

        [XmlElement("lightFalloffExponent")]
        public ValueItem<float> LightFalloffExponent { get; set; } = 10f;

        [XmlElement("lightInnerConeAngle")]
        public ValueItem<float> LightInnerConeAngle { get; set; } = 30f;

        [XmlElement("lightOuterConeAngle")]
        public ValueItem<float> LightOuterConeAngle { get; set; } = 60f;

        [XmlElement("lightOffset")]
        public ValueItem<float> LightOffset { get; set; } = 0f;

        [XmlElement("textureName")]
        public string TextureName { get; set; } = Utils.TextureHash.defaultLightTexture;

        [XmlIgnore]
        public uint TextureHash
        {
            get => Utils.TextureHash.StringToHash(TextureName);

            set => TextureName = Utils.TextureHash.HashToString(value);
        }

        [XmlElement("sequencerBpm")]
        public ValueItem<uint> SequencerBPM { get; set; } = 100;

        [XmlElement("leftHeadLight")]
        public SequencerWrapper LeftHeadLightSequencer { get; set; } = 0;

        [XmlElement("rightHeadLight")]
        public SequencerWrapper RightHeadLightSequencer { get; set; } = 0;

        [XmlElement("leftTailLight")]
        public SequencerWrapper LeftTailLightSequencer { get; set; } = 0;

        [XmlElement("rightTailLight")]
        public SequencerWrapper RightTailLightSequencer { get; set; } = 0;

        [XmlElement("leftHeadLightMultiples")]
        public ValueItem<byte> LeftHeadLightMultiples { get; set; } = 1;

        [XmlElement("rightHeadLightMultiples")]
        public ValueItem<byte> RightHeadLightMultiples { get; set; } = 1;

        [XmlElement("leftTailLightMultiples")]
        public ValueItem<byte> LeftTailLightMultiples { get; set; } = 1;

        [XmlElement("rightTailLightMultiples")]
        public ValueItem<byte> RightTailLightMultiples { get; set; } = 1;

        [XmlElement("useRealLights")]
        public ValueItem<bool> UseRealLights { get; set; } = true;

        // Sirens property should always be *deserialized* to read in the sirens array,
        // but should never be *serialized* because this is written by the CommentedSirenSettings
        // below with siren number comments inline
        public bool ShouldSerializeSirens() => false;

        [XmlArray("sirens")]
        [XmlArrayItem("Item")]
        public SirenEntry[] Sirens
        {
            get => sirenList.ToArray();
            set
            {
                sirenList = value.ToList();
                for (int i = 0; i < sirenList.Count; i++)
                {
                    sirenList[i].SirenIdCommentText = "Siren " + (i+1);
                }
            }
        }

        [XmlAnyElement()]
        public XmlNode CommentedSirenSettings
        {
            set { }
            get
            {
                var export = new XmlDocument();
                var root = export.CreateElement("sirens");
                export.AppendChild(root);

                for (int i = 0; i < sirenList.Count; i++)
                {
                    root.AppendChild(export.ImportNode(sirenList[i].SirenIdComment, true));
                    var element = export.ImportNode(Serializer.SerializeToXMLElement<SirenEntry>(sirenList[i]), true);
                    root.AppendChild(element);
                }

                return root;
            }
        }

        [XmlIgnore]
        private List<SirenEntry> sirenList = new List<SirenEntry>();

        public void AddSiren(SirenEntry item)
        {
            item.SirenIdCommentText = "Siren " + (sirenList.Count + 1);
            sirenList.Add(item);
        }
    }

    [XmlType(TypeName = "Item")]
    public class SirenEntry
    {
        [XmlIgnore]
        internal string SirenIdCommentText { get; set; }

        [XmlIgnore]
        [XmlAnyElement("SirenIdComment")]
        public XmlComment SirenIdComment
        {
            get => new XmlDocument().CreateComment($" {SirenIdCommentText} ");
            set => SirenIdCommentText = value.InnerText;
        }

        [XmlElement("rotation")]
        public LightDetailEntry Rotation { get; set; } = new LightDetailEntry();

        [XmlElement("flashiness")]
        public LightDetailEntry Flashiness { get; set; } = new LightDetailEntry();

        [XmlElement("corona")]
        public CoronaEntry Corona { get; set; } = new CoronaEntry();

        [XmlIgnore]
        public Color LightColor { get; set; } = Color.White;

        [XmlElement("color")]
        public ValueItem<string> ColorString
        {
            get => LightColor.ToFormattedHex();
            set
            {
                LightColor = ColorTools.HexToColor(value);
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

    public class CoronaEntry
    {
        [XmlElement("intensity")]
        public ValueItem<float> CoronaIntensity { get; set; } = 50f;

        [XmlElement("size")]
        public ValueItem<float> CoronaSize { get; set; } = 0f;

        [XmlElement("pull")]
        public ValueItem<float> CoronaPull { get; set; } = 0.10f;

        [XmlElement("faceCamera")]
        public ValueItem<bool> CoronaFaceCamera { get; set; } = false;
    }

    public class LightDetailEntry
    {
        [XmlElement("delta")]
        public ValueItem<float> DeltaRad { get; set; } = 0;

        [XmlIgnore]
        public float DeltaDeg
        {
            get => Rage.MathHelper.ConvertRadiansToDegrees(DeltaRad);
            set => DeltaRad = Rage.MathHelper.ConvertDegreesToRadians(value);
        }

        [XmlElement("start")]
        public ValueItem<float> StartRad { get; set; } = 0;

        [XmlIgnore]
        public float StartDeg
        {
            get => Rage.MathHelper.ConvertRadiansToDegrees(StartRad);
            set => StartRad = Rage.MathHelper.ConvertDegreesToRadians(value);
        }

        [XmlElement("speed")]
        public ValueItem<float> Speed { get; set; } = 1.0f;

        [XmlElement("sequencer")]
        public Sequencer Sequence { get; set; } = 0;

        [XmlElement("multiples")]
        public ValueItem<byte> Multiples { get; set; } = 1;

        [XmlElement("direction")]
        public ValueItem<bool> Direction { get; set; } = true;

        [XmlElement("syncToBpm")]
        public ValueItem<bool> SyncToBPM { get; set; } = true;

    }

    [DebuggerDisplay("{Sequence} = {SequenceRaw}")]
    public class Sequencer : ValueItem<uint>
    {
        public static implicit operator Sequencer(uint value) => new Sequencer(value);
        public static implicit operator Sequencer(string value) => new Sequencer(value);
        public static implicit operator uint(Sequencer item) => item.Value;
        public static implicit operator string(Sequencer item) => Convert.ToString(item.Value, 2).PadLeft(32, '0');

        public Sequencer(uint value) : base(value) { }
        public Sequencer(string value) : base(Convert.ToUInt32(value, 2)) { }

        public Sequencer() : base() { }

        [XmlIgnore]
        public uint SequenceRaw 
        {
            get => Value;
            set => Value = value;
        }
                
        [XmlIgnore]
        public string Sequence
        {
            get => Convert.ToString(Value, 2);
            set => Value = Convert.ToUInt32(value, 2);
        }
    }

    [DebuggerDisplay("{Sequencer.Sequence} = {Sequencer.SequenceRaw}")]
    public class SequencerWrapper
    {
        public static implicit operator SequencerWrapper(uint value) => new SequencerWrapper(value);
        public static implicit operator SequencerWrapper(string value) => new SequencerWrapper(value);
        public static implicit operator uint(SequencerWrapper item) => item.Sequencer.SequenceRaw;
        public static implicit operator string(SequencerWrapper item) => item.Sequencer.Sequence;

        public SequencerWrapper(uint value)
        {
            Sequencer = value;
        }

        public SequencerWrapper(string value)
        {
            Sequencer = value;
        }

        public SequencerWrapper()
        {
            Sequencer = new Sequencer();
        }

        [XmlElement("sequencer")]
        public Sequencer Sequencer { get; set; }
    }

    [DebuggerDisplay("{Value}")]
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
        public virtual T Value { get; set; }
    }
}
