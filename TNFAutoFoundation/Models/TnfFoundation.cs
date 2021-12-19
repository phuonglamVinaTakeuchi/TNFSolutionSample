using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNFAutoFoundation.Enums;

namespace TNFAutoFoundation.Models
{
    public class TnfFoundation
    {
        public FoundationTypes FoundationType { get;  }
        public string TypeMark { get;  }
        public Phasings Phasing { get;  }
        public bool IsConstruction => Phasing == Phasings.Construction;
        public bool IsDesign => Phasing == Phasings.Design;
        public RebarDiameter HakamaDiameter { get; }
        public RebarDiameter BesuDiameter { get; }
        public Dimension3d FDimension { get; }
        public Dimension3d HashiraGataDimension { get; }
        public RebarDiameter HashiraGataMainRebar { get; }
        public RebarDiameter HashiraGataHoopRebar { get; }
        public RebarDiameter FoundationStirrupRebar { get; }
        public Dimension2d ColDimension { get;  }
        public ColumnVisibilities ColumnVisibility { get;  }
        public bool IsByX => ColumnVisibility == ColumnVisibilities.ByX;
        public bool IsByY => ColumnVisibility == ColumnVisibilities.ByY;
        public bool IsSquareCol => ColumnVisibility == ColumnVisibilities.SquareColumn;
        public bool HashiraGataVisibility { get; set; }
        public bool Umemodoshi { get; set; }
        public string LevelSymbol { get; set; }
        public int FoundLevelOffset { get; set; }
        public int LeanConcreteThickness { get; set; }
        public bool IsEccentric { get; set; }
        public Dimension2d HashiraGataLocation { get; set; }
        public bool AnchorLean { get; set; }
        public int TopDepth { get; set; }
        public int BottomDepth { get; set; }
        public int BottomWidth { get; set; }
        public int YoboriHeight { get; set; }
        public string HashiraGataRebarName { get; set; }
        public int SubIndex { get; set; } = 0;
        public string Name
        {
            get
            {
                if (FDimension != null)
                {
                    if (SubIndex>0)
                    {
                        return "W" + FDimension.XDimension + "xL" + FDimension.YDimension + "xD" + FDimension.ZDimension +
                               "-" + TypeMark+"."+SubIndex;
                    }
                    else
                    {
                        return "W" + FDimension.XDimension + "xL" + FDimension.YDimension + "xD" + FDimension.ZDimension +
                               "-" + TypeMark;
                    }
                }

                return "";
            }
        }
        public string Description => FoundationType == FoundationTypes.DType ? "台形" : "通常";
        public string FGroup => TypeMark;
        public TnfFoundation(TnfPoco tnf)
        {
            FoundationType = tnf.FoundationType == "DType" ? FoundationTypes.DType : FoundationTypes.NormalType;
            TypeMark = tnf.TypeMark;
            Phasing = tnf.Phasing == "Constructor" ? Phasings.Construction : Phasings.Design;
            HakamaDiameter = new RebarDiameter(tnf.Hakama);
            HashiraGataRebarName = "はかま鉄筋両方"+tnf.Hakama;
            BesuDiameter = new RebarDiameter(tnf.Besu);
            FDimension = new Dimension3d(tnf.FDimension);
            HashiraGataDimension = new Dimension3d(tnf.HashiraGataDimension);
            HashiraGataMainRebar = new RebarDiameter(tnf.HashiraGataMainRebar,true);
            HashiraGataHoopRebar = new RebarDiameter(tnf.HashiraGataHoopRebar);
            FoundationStirrupRebar = new RebarDiameter(tnf.FoundationStirrupRebar);
            ColDimension = new Dimension2d(tnf.ColDimension);
            var colX = ColDimension.XDimension;
            var colY = ColDimension.YDimension;
            switch (tnf.ColumnVisibility)
            {
                case "ByX":
                    ColumnVisibility = ColumnVisibilities.ByX;
                    break;
                case "ByY":
                    ColumnVisibility = ColumnVisibilities.ByY;
                    break;
                default:
                    ColumnVisibility = ColumnVisibilities.SquareColumn;
                    break;
            }
            HashiraGataVisibility = tnf.HashiraGataVisibility=="Yes";
            Umemodoshi = tnf.Umemodoshi == "Yes";
            LevelSymbol = tnf.LevelSymbol;
            FoundLevelOffset = Convert.ToInt32(tnf.FoundLevelOffset);
            LeanConcreteThickness = Convert.ToInt32(tnf.LeanConcreteThickness);
            IsEccentric = tnf.IsEccentric == "Yes";
            if (string.IsNullOrEmpty(tnf.HashiraGataLocation))
            {
                var dX = FDimension.XDimension / 2;
                var dY = FDimension.YDimension / 2;
                var dimensionString = dX + "x" + dY;
                HashiraGataLocation = new Dimension2d(dimensionString);
            }
            else
            {
                HashiraGataLocation = new Dimension2d(tnf.HashiraGataLocation);
            }
            HashiraGataLocation = new Dimension2d(tnf.HashiraGataLocation);
            AnchorLean = tnf.AnchorLean == "Yes";
            TopDepth = Convert.ToInt32(tnf.TopDepth);
            BottomWidth = Convert.ToInt32(tnf.BottomWidth);
            YoboriHeight = Convert.ToInt32(tnf.YoboriHeight);
            BottomDepth = FDimension.ZDimension - TopDepth;
        }
    }
}
