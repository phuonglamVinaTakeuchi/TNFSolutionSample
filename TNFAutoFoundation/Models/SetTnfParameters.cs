using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace TNFAutoFoundation.Models
{
    public abstract class SetTnfParameters
    {
        public TnfFoundation TnfFoundation { get; }
        public FamilySymbol TnfSymbol { get; }

        protected SetTnfParameters(TnfFoundation tnfFoundation, FamilySymbol symbol)
        {

            TnfFoundation = tnfFoundation;
            TnfSymbol = symbol;
        }

        private void AddTest(Parameter parameter, Dictionary<string, string> testDic)
        {
            var testValue = parameter.AsValueString();
            testDic.Add(parameter.Definition.Name, testValue);
            testDic.Add(parameter.Definition.Name+"t",parameter.AsString());
        }
        public virtual void SetParameters()
        {
            var parameters = TnfSymbol.Parameters.Cast<Parameter>().ToList();
            var stringTest = new Dictionary<string,string>();
            foreach (var parameter in parameters)
            {
                switch (parameter.Definition.Name)
                {
                    // Tho so chung
                    // Hakama Radius
                    case "Height Offset Form Level":
                        var offset = TnfFoundation.FoundLevelOffset;
                        parameter.Set(offset.MmToInch());
                       //AddTest(parameter,stringTest);
                        break;
                    case "鉄筋_はかま_直径":
                        parameter.Set(TnfFoundation.HakamaDiameter.ReBarRadius.MmToInch());
                       //AddTest(parameter,stringTest);
                        break;
                        // D15
                    //case "D15":
                    //    parameter.Set(1500);
                    //    break;
                    // Hakama Spacing
                    case "鉄筋_はかま_距離_最大":
                        parameter.Set(TnfFoundation.HakamaDiameter.RebarSpacing.MmToInch());
                       //AddTest(parameter,stringTest);
                        break;
                    // Besu Radius
                    case "鉄筋_ベース_直径":
                        parameter.Set(TnfFoundation.BesuDiameter.ReBarRadius.MmToInch());
                        //AddTest(parameter,stringTest);
                        break;
                    // Besu Spacing
                    case "鉄筋_ベース_距離_最大":
                        parameter.Set(TnfFoundation.BesuDiameter.RebarSpacing.MmToInch());
                        //AddTest(parameter,stringTest);
                        break;
                    // HashiraGataHoop Rebar Radius
                    case "柱型_Hoop_直径":
                        parameter.Set(TnfFoundation.HashiraGataHoopRebar.ReBarRadius.MmToInch());
                        //AddTest(parameter,stringTest);
                        break;
                    // HashiraGataHoop Rebar Spacing
                    case "柱型_Hoop_距離_最大":
                        parameter.Set(TnfFoundation.HashiraGataHoopRebar.RebarSpacing.MmToInch());
                        //AddTest(parameter,stringTest);
                        break;
                    // Foundation Stirrup Rebar Radius
                    case "鉄筋_横筋_直径":
                        parameter.Set(TnfFoundation.FoundationStirrupRebar.ReBarRadius.MmToInch());
                       //AddTest(parameter,stringTest);
                        break;
                    // Foundation Stirrup Rebar Spacing
                    case "鉄筋_横筋_最大":
                        parameter.Set(TnfFoundation.HashiraGataHoopRebar.RebarSpacing.MmToInch());
                        //AddTest(parameter,stringTest);
                        break;
                    // Foundation Group Name
                    case "基礎_グループ":
                        parameter.Set(TnfFoundation.FGroup);
                        //AddTest(parameter,stringTest);
                        break;
                    //HaKama Rebar Name
                    case "注釈_鉄筋_はかま":
                        parameter.Set(TnfFoundation.HashiraGataRebarName);
                        //AddTest(parameter,stringTest);
                        break;
                    // Level Symbol
                    case "レベル記号":
                        parameter.Set(TnfFoundation.LevelSymbol);
                        //AddTest(parameter,stringTest);
                        break;
                    // FDmension x
                    case "Length":
                        parameter.Set(TnfFoundation.FDimension.XDimension.MmToInch());
                        //AddTest(parameter,stringTest);
                        break;
                    // FDmension y
                    case "Width":
                        parameter.Set(TnfFoundation.FDimension.YDimension.MmToInch());
                        //AddTest(parameter,stringTest);
                        break;

                    // Col Dimension
                    case "柱_Lx":
                        parameter.Set(TnfFoundation.ColDimension.XDimension.MmToInch());
                        //AddTest(parameter,stringTest);
                        break;
                    case "柱_Ly":
                        parameter.Set(TnfFoundation.ColDimension.YDimension.MmToInch());
                        //AddTest(parameter,stringTest);
                        break;
                    // HashiraGataDimension
                    case "柱型_Lx":
                        parameter.Set(TnfFoundation.HashiraGataDimension.XDimension.MmToInch());
                        //AddTest(parameter,stringTest);
                        break;
                    case "柱型_Ly":
                        parameter.Set(TnfFoundation.HashiraGataDimension.YDimension.MmToInch());
                        //AddTest(parameter,stringTest);
                        break;

                    // HashiraGataMainRebar
                    case "柱型_主筋_本数":
                        parameter.Set(TnfFoundation.HashiraGataMainRebar.RebarQuantities);
                       //AddTest(parameter,stringTest);
                        break;
                    case "柱型_主筋_直径":
                        parameter.Set(TnfFoundation.HashiraGataMainRebar.ReBarRadius.MmToInch());
                        //AddTest(parameter,stringTest);
                        break;

                    // HashiraGataDimension
                    case "柱型_高さ":
                        parameter.Set(TnfFoundation.HashiraGataDimension.ZDimension.MmToInch());
                       //AddTest(parameter,stringTest);
                        break;

                    case "Type Mark":
                        parameter.Set(TnfFoundation.TypeMark);
                        //AddTest(parameter,stringTest);
                        break;
                    // Phasing
                    case "設計":
                        parameter.Set(TnfFoundation.IsDesign ? 1 : 0);
                        //AddTest(parameter,stringTest);
                        break;

                    // Column Visibility
                    case "柱_I_方向Y":
                    case "柱_I_Y方向":
                        parameter.Set(TnfFoundation.IsByY ? 1 : 0);
                        //AddTest(parameter,stringTest);
                        break;
                    case "柱_□":
                        parameter.Set(TnfFoundation.IsSquareCol ? 1 : 0);
                       //AddTest(parameter,stringTest);
                        break;
                    // HashiraGataVisibility
                    case "柱型":
                        parameter.Set(TnfFoundation.HashiraGataVisibility ? 1 : 0);
                        //AddTest(parameter,stringTest);
                        break;
                    case "Description":
                        parameter.Set(TnfFoundation.Description);
                        //AddTest(parameter,stringTest);
                        break;


                    default:

                        break;
                }
            }

        }
    }
}
